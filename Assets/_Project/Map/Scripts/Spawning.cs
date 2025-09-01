using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Singleton manager for spawn points. Server owns the queue.
/// Clients request a spawn point and get it via ClientRpc.
/// </summary>
public class Spawning : NetworkBehaviour
{
	public static Spawning Instance;

	public List<Transform> spawnPoints = new List<Transform>();

	private Queue<Transform> spawnQueue = new Queue<Transform>();
	private System.Random rng = new System.Random();

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
	/// Server-only: refill the queue by shuffling all spawn points.
	/// </summary>
	private void RefillQueue()
	{
		List<Transform> shuffled = new List<Transform>(spawnPoints);
		int n = shuffled.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			Transform temp = shuffled[k];
			shuffled[k] = shuffled[n];
			shuffled[n] = temp;
		}

		foreach (var sp in shuffled) spawnQueue.Enqueue(sp);
		Debug.Log("Refilled with: " + spawnQueue.Count);
	}

	/// <summary>
	/// Client requests a spawn point. Server responds via ClientRpc.
	/// </summary>
	[ServerRpc(RequireOwnership = false)]
	public void RequestSpawnPointServerRpc(ServerRpcParams rpcParams = default)
	{
		if (!IsServer) return;

		if (spawnQueue.Count == 0) RefillQueue();
		Transform sp = spawnQueue.Dequeue();

		Debug.Log("Request got: " + sp.position);
		// Send the spawn position back only to the requesting client
		RespondSpawnPointClientRpc(sp.position, rpcParams.Receive.SenderClientId);
	}
	[ClientRpc]
	private void RespondSpawnPointClientRpc(Vector3 spawnPos, ulong clientId)
	{
		if (NetworkManager.Singleton.LocalClientId != clientId) return;

		if (PlayerMovement.LocalInstance != null)
		{
			PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
		}
		else
		{
			// Queue the spawn position to be applied later
			StartCoroutine(WaitForLocalInstance(spawnPos));
		}
	}

	private IEnumerator WaitForLocalInstance(Vector3 spawnPos)
	{
		while (PlayerMovement.LocalInstance == null)
			yield return null;

		PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
	}
}
