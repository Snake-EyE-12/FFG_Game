using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	private bool hasFingerGun = false;
	[SerializeField] private float respawnDelay = 3f;
	private Collider col;
	private Renderer rend;

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
			Debug.Log("Handling hit");
			if (IsServer)
			{
				// Hide player on all clients
				DisablePlayerClientRpc();

				// Start respawn coroutine only on server
				StartCoroutine(RespawnRoutine());
			}
		}
	}

	private IEnumerator RespawnRoutine()
	{
		// hide visuals
		rend.enabled = false;
		col.enabled = false;

		// stop physics
		Rigidbody rb = transform.parent.GetComponent<Rigidbody>();
		if (rb != null) rb.isKinematic = true;

		yield return new WaitForSeconds(respawnDelay);

		// Tell the owner client to request a spawn
		RequestSpawnClientRpc(OwnerClientId);
	}

	// Targeted ClientRpc: only tells the owning client to request spawn
	[ClientRpc]
	private void RequestSpawnClientRpc(ulong targetClientId)
	{
		if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

		PlayerMovement.LocalInstance.RequestSpawnFromServer();
	}

	// Called from PlayerMovement after the server sends the spawn point
	public void OnReceivedSpawn(Vector3 spawnPos)
	{
		Transform playerTransform = transform.parent;
		Rigidbody rb = playerTransform.GetComponent<Rigidbody>();

		// Stop movement
		if (rb != null) rb.linearVelocity = Vector3.zero;

		// Move to spawn
		playerTransform.position = spawnPos;

		// Re-enable physics
		if (rb != null) rb.isKinematic = false;

		// Show player on all clients
		EnablePlayerClientRpc();
	}

	[ClientRpc]
	public void DisablePlayerClientRpc()
	{
		col.enabled = false;
		rend.enabled = false;
	}

	[ClientRpc]
	public void EnablePlayerClientRpc()
	{
		col.enabled = true;
		rend.enabled = true;
	}

	[ClientRpc]
	private void RemoveFingerGunClientRpc()
	{
		// play animation / UI effect here
	}
}
