using UnityEngine;
using Unity.Netcode;

public class SpawnPoint : MonoBehaviour
{
	private void OnEnable()
	{
		if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
		{
			Spawning.RegisterSpawnPoint(transform.position);
		}
	}


	// we dont wanna unregister becaues spawnpoint transforms do get turned off

	//private void OnDisable()
	//{
	//	if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
	//	{
	//		Spawning.UnregisterSpawnPoint(transform.position);
	//	}
	//}

	//private void OnDestroy()
	//{
	//	if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
	//	{
	//		Spawning.UnregisterSpawnPoint(transform.position);
	//	}
	//}
}
