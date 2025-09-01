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

	private void Awake()
	{
		col = GetComponent<Collider>();
		rend = GetComponent<Renderer>();
	}

	public void HitPlayer()
	{
		if (!IsServer)
		{
			HitPlayerServerRpc();
			return;
		}

		HandleHit();
	}

	[ServerRpc(RequireOwnership = false)]
	private void HitPlayerServerRpc(ServerRpcParams rpcParams = default)
	{
		HandleHit();
	}

	private void HandleHit()
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
				DisablePlayerClientRpc();

				StartCoroutine(RespawnRoutine());
			}
		}
	}

	private IEnumerator RespawnRoutine()
	{
		rend.enabled = false;
		col.enabled = false;

		Rigidbody rb = transform.parent.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		yield return new WaitForSeconds(respawnDelay);

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
	public void DisablePlayerClientRpc()
	{
		col.enabled = false;
		rend.enabled = false;
		dead = true;
	}

	[ClientRpc]
	public void EnablePlayerClientRpc()
	{
		col.enabled = true;
		rend.enabled = true;
		dead = false;
	}

	[ClientRpc]
	private void RemoveFingerGunClientRpc()
	{
		// play animation / UI effect here
	}
}
