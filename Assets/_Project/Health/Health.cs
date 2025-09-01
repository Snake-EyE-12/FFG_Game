using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
	private bool hasFingerGun = false;

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
			Death();
		}
	}

	private void Death()
	{
		// Despawn networked object if it has NetworkObject
		if (TryGetComponent<NetworkObject>(out var netObj) && netObj.IsSpawned)
		{
			netObj.Despawn();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	[ClientRpc]
	private void RemoveFingerGunClientRpc()
	{

	}
}
