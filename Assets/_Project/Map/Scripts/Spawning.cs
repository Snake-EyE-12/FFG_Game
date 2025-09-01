using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawning : NetworkBehaviour
{
	public static Spawning Instance;

	private Queue<Transform> spawnQueue = new Queue<Transform>();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	/// <summary>
	/// Register a single spawn point. Call from ImageMapLoader or similar.
	/// Only the host/server should do this.
	/// </summary>
	public void RegisterSpawnPoint(Transform t)
	{
		Debug.Log("Register point at " + t.position);
		spawnQueue.Enqueue(t);
	}

	public void ShuffleSpawnPoints()
	{

	}

	/// <summary>
	/// Get next spawn point from queue (server only)
	/// </summary>
	private Transform GetNextSpawnPoint()
	{
		Debug.Log("Getting next spawn point");
		if (spawnQueue.Count == 0)
		{
			Debug.LogWarning("Spawn queue empty!");
			return null;
		}

		Transform t = spawnQueue.Dequeue();
		Debug.Log("Get point at " + t.position);
		spawnQueue.Enqueue(t); // optional: rotate queue
		return t;
	}

	/// <summary>
	/// Called by client to request a spawn position (initial spawn or respawn)
	/// </summary>
	[ServerRpc(RequireOwnership = false)]
	public void RequestSpawnPointServerRpc(ServerRpcParams rpcParams = default)
	{
		Debug.Log("Requesting spawn point server rpc");
		Transform nextSpawn = GetNextSpawnPoint();
		if (nextSpawn == null) return;
		Debug.Log("Got next spawn " + nextSpawn.position.ToString());

		// send spawn position back to only requesting client
		RespawnClientRpc(nextSpawn.position, rpcParams.Receive.SenderClientId);
	}

	/// <summary>
	/// Server tells the client where to spawn
	/// </summary>
	[ClientRpc]
	private void RespawnClientRpc(Vector3 spawnPos, ulong targetClientId)
	{
		Debug.Log("Respawn Client RPC");
		if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

		if (PlayerMovement.LocalInstance != null)
		{
			Debug.Log("Found local instance");
			PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
		}
		else
		{
			// In case the local player hasn't spawned yet
			StartCoroutine(WaitForLocalPlayer(spawnPos));
		}
	}

	private System.Collections.IEnumerator WaitForLocalPlayer(Vector3 spawnPos)
	{
		while (PlayerMovement.LocalInstance == null)
			yield return null;

		PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
	}
}
