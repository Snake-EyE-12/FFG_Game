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
				Debug.Log("is server");
				// Tell all clients to hide this player
				DisablePlayerClientRpc();

				// Start respawn coroutine only on server
				StartCoroutine(RespawnRoutineServer());
			}
			else
			{
				Debug.Log("not server");
			}
		}
	}
	private IEnumerator RespawnRoutineServer()
	{
		Debug.Log("Respawn routine server pre");
		yield return new WaitForSeconds(respawnDelay);
		Debug.Log("Respawn routine server post");

		// Ask the server to give the spawn point to the owning client
		PlayerMovement owner = transform.parent.gameObject.GetComponent<PlayerMovement>();
		if (owner != null)
		{
			Debug.Log("found owner");
			owner.RequestSpawnFromServer();
			EnablePlayerClientRpc();
		}
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
