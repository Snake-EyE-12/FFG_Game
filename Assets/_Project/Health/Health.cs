using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class Health : NetworkBehaviour
{
	private bool hasFingerGun = false;
	[SerializeField] private float respawnDelay = 3f;
	private Collider col;
	private Renderer rend;
	[HideInInspector] public bool dead = false;
	public PlayerMovement pm;
	public SpriteRenderer sr;

	private void Awake()
	{
		col = GetComponent<Collider>();
		rend = GetComponent<Renderer>();
	}

	public void HitPlayer(Vector3 hitDir)
	{
		if (!IsServer)
		{
			HitPlayerServerRpc(hitDir);
			return;
		}

		HandleHit(hitDir);
	}

	[ServerRpc(RequireOwnership = false)]
	private void HitPlayerServerRpc(Vector3 hitDir, ServerRpcParams rpcParams = default)
	{
		HandleHit(hitDir);
	}

	private void HandleHit(Vector3 hitDir)
	{
		if (hasFingerGun)
		{
			hasFingerGun = false;
			RemoveFingerGunClientRpc();
		}
		else
		{
			if (IsServer)
			{
				Die(hitDir);
			}
		}
	}

	private void Die(Vector3 hitDir)
	{
		DisablePlayerClientRpc(hitDir);

		StartCoroutine(RespawnRoutine());
	}

	private IEnumerator RespawnRoutine()
	{
		Rigidbody rb = transform.parent.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		yield return new WaitForSeconds(respawnDelay);
		HideSpriteClientRpc();

		RequestSpawnClientRpc(OwnerClientId);
	}

	[ClientRpc]
	private void RequestSpawnClientRpc(ulong targetClientId)
	{
		if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

		PlayerMovement.LocalInstance.RequestSpawnFromServer();
	}

	public void OnReceivedSpawn(Vector3 spawnPos)
	{
		Transform playerTransform = transform.parent;
		Rigidbody rb = playerTransform.GetComponent<Rigidbody>();

		if (rb != null) rb.linearVelocity = Vector3.zero;

		playerTransform.position = spawnPos;

		if (rb != null) rb.isKinematic = false;

		NotifyRespawnedServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void NotifyRespawnedServerRpc(ServerRpcParams rpcParams = default)
	{
		EnablePlayerClientRpc();
	}

	[ClientRpc]
	public void DisablePlayerClientRpc(Vector3 hitDir)
	{
		pm.OnDeath();
		float angle = PlayerMovement.GetAngleFromDir(hitDir);
		pm.animationController.DoDeathFrame(angle);
		col.enabled = false;
		dead = true;
	}

	[ClientRpc]
	public void EnablePlayerClientRpc()
	{
		StartCoroutine(EnableAfterDelay());
	}

	private IEnumerator EnableAfterDelay()
	{
		yield return new WaitForSeconds(0.3f);

		pm.ChangeState(PlayerMovement.MoveState.IDLE);
		yield return new WaitForSeconds(0.2f);
		col.enabled = true;
		dead = false;

		if (sr != null) sr.enabled = true;
	}

	[ClientRpc]
	private void RemoveFingerGunClientRpc()
	{
		// play animation / UI effect here
	}

	public void RanIntoPickup(PickUp pickUp)
	{
		pickUp.PickUpItem();
		hasFingerGun = true;
	}

	[ClientRpc]
	private void HideSpriteClientRpc()
	{
		if (sr != null)
			sr.enabled = false;
	}

	[ClientRpc]
	private void ShowSpriteClientRpc()
	{
		if (sr != null)
			sr.enabled = true;
	}
}
