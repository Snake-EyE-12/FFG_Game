using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawning : NetworkBehaviour
{
	public static Spawning Instance;

	private static List<Vector3> spawnPositions = new List<Vector3>();
	private Queue<Vector3> spawnQueue = new Queue<Vector3>();

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
	}

	// Register a spawn point (server-only)
	public static void RegisterSpawnPoint(Vector3 pos)
	{
		if (!spawnPositions.Contains(pos))
		{
			spawnPositions.Add(pos);
			Debug.Log($"[Spawning] Registered spawn point at {pos}");
		}
	}

	// Unregister a spawn point (server-only)
	public static void UnregisterSpawnPoint(Vector3 pos)
	{
		if (spawnPositions.Remove(pos))
		{
			Debug.Log($"[Spawning] Unregistered spawn point at {pos}");
		}

		RebuildQueue();
	}

	private static void RebuildQueue()
	{
		if (Instance == null) return;

		Instance.spawnQueue.Clear();
		var shuffled = new List<Vector3>(spawnPositions);
		Shuffle(shuffled);
		foreach (var sp in shuffled)
			Instance.spawnQueue.Enqueue(sp);
	}

	private Vector3? GetNextSpawnPoint()
	{
		if (spawnQueue.Count == 0)
		{
			if (spawnPositions.Count == 0)
			{
				Debug.LogWarning("[Spawning] No spawn points registered!");
				return null;
			}

			var shuffled = new List<Vector3>(spawnPositions);
			Shuffle(shuffled);

			foreach (var sp in shuffled)
				spawnQueue.Enqueue(sp);
		}

		return spawnQueue.Dequeue();
	}

	[ServerRpc(RequireOwnership = false)]
	public void RequestSpawnPointServerRpc(ServerRpcParams rpcParams = default)
	{
		Vector3? next = GetNextSpawnPoint();
		if (next == null) return;

		var clientRpcParams = new ClientRpcParams
		{
			Send = new ClientRpcSendParams
			{
				TargetClientIds = new[] { rpcParams.Receive.SenderClientId }
			}
		};

		RespawnClientRpc(next.Value, clientRpcParams);
	}

	[ClientRpc]
	private void RespawnClientRpc(Vector3 spawnPos, ClientRpcParams clientRpcParams = default)
	{
		if (PlayerMovement.LocalInstance != null)
		{
			PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
		}
		else
		{
			StartCoroutine(WaitForLocalPlayer(spawnPos));
		}
	}

	private System.Collections.IEnumerator WaitForLocalPlayer(Vector3 spawnPos)
	{
		while (PlayerMovement.LocalInstance == null)
			yield return null;

		PlayerMovement.LocalInstance.OnReceivedSpawnPoint(spawnPos);
	}

	private static void Shuffle<T>(IList<T> list)
	{
		var rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]);
		}
	}
}
