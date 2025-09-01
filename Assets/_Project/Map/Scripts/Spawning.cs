using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Spawning : NetworkBehaviour
{
	public static Spawning Instance;

	private static List<Transform> spawnPoints = new List<Transform>();

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

	public static void RegisterSpawnPoint(Transform t)
	{
		if (!spawnPoints.Contains(t))
		{
			spawnPoints.Add(t);
		}
	}

	private Transform GetNextSpawnPoint()
	{
		if (spawnQueue.Count == 0)
		{
			if (spawnPoints.Count == 0)
			{
				Debug.LogWarning("No spawn points registered!");
				return null;
			}

			List<Transform> shuffled = new List<Transform>(spawnPoints);
			Shuffle(shuffled);

			foreach (var sp in shuffled)
				spawnQueue.Enqueue(sp);
		}

		return spawnQueue.Dequeue();
	}

	[ServerRpc(RequireOwnership = false)]
	public void RequestSpawnPointServerRpc(ServerRpcParams rpcParams = default)
	{
		Transform nextSpawn = GetNextSpawnPoint();
		if (nextSpawn == null) return;

		RespawnClientRpc(nextSpawn.position, rpcParams.Receive.SenderClientId);
	}

	[ClientRpc]
	private void RespawnClientRpc(Vector3 spawnPos, ulong targetClientId)
	{
		if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

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
		System.Random rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]);
		}
	}
}
