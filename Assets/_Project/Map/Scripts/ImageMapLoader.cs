using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ImageMapLoader : MonoBehaviour
{
	[Header("Map Data")]
	public MapCollection mapCollection;
	public int mapIndex = 0; // which map in the array to load

	[Header("Prefabs")]
	public GameObject groundPrefab;
	public GameObject coverPrefab;
	public GameObject spawnpointPrefab;

	[Header("Block Settings")]
	public float blockSize = 1f;

	[Header("Pixel Matching")]
	[Range(0f, 0.1f)]
	public float colorTolerance = 0.01f;

	// COLORS
	readonly Color blue = new Color(0f, 0f, 1f);   // AIR
	readonly Color green = new Color(0f, 1f, 0f);  // JUST GROUND
	readonly Color yellow = new Color(1f, 1f, 0f); // 1 HIGH COVER
	readonly Color red = new Color(1f, 0f, 0f);    // 2 HIGH COVER
	readonly Color cyan = new Color(0f, 1f, 1f);   // SPAWN POINT

	private void Awake()
	{
		mapIndex = Random.Range(0, mapCollection.maps.Length);
		LoadMap();

		// Combine *only* static ground
		GetComponent<MeshCombiner>().CombineMeshesPerMaterial();
	}

	void LoadMap()
	{
		if (mapCollection == null || mapCollection.maps.Length == 0)
		{
			Debug.LogError("No maps assigned in MapCollection!");
			return;
		}

		if (mapIndex < 0 || mapIndex >= mapCollection.maps.Length)
		{
			Debug.LogError("Map index out of range!");
			return;
		}

		Texture2D map = mapCollection.maps[mapIndex];
		if (map == null)
		{
			Debug.LogError("Selected map texture is null!");
			return;
		}

		for (int y = 0; y < map.height; y++)
		{
			for (int x = 0; x < map.width; x++)
			{
				int flippedY = map.height - y - 1; // flip Y to match top-down
				Color pixel = map.GetPixel(x, flippedY);
				SpawnFromPixel(pixel, x, y);
			}
		}
	}

	void SpawnFromPixel(Color pixel, int x, int y)
	{
		Vector3 basePos = new Vector3(x * blockSize, -.5f, y * blockSize);

		if (IsCloseColor(pixel, cyan))
		{
			GameObject spawnBlock = Instantiate(spawnpointPrefab, basePos, Quaternion.identity, transform);
			Transform spawnTransform = spawnBlock.transform.GetChild(0);

			if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
			{
				Spawning.Instance.RegisterSpawnPoint(spawnTransform);
			}
		}

		int stackHeight = -1;
		if (IsCloseColor(pixel, blue)) return;
		else if (IsCloseColor(pixel, green)) stackHeight = 1;
		else if (IsCloseColor(pixel, yellow)) stackHeight = 2;
		else if (IsCloseColor(pixel, red)) stackHeight = 3;

		if (stackHeight == -1) return;

		Instantiate(groundPrefab, basePos, Quaternion.identity, transform);

		if (!NetworkManager.Singleton.IsServer) return;
		for (int i = 1; i < stackHeight; i++)
		{
			Vector3 pos = basePos + new Vector3(0, i * blockSize, 0);
			NetworkObject cover = Instantiate(coverPrefab, pos, Quaternion.identity, transform).GetComponent<NetworkObject>();
			cover.Spawn();
		}
	}


	bool IsCloseColor(Color a, Color b)
	{
		return Mathf.Abs(a.r - b.r) < colorTolerance &&
			   Mathf.Abs(a.g - b.g) < colorTolerance &&
			   Mathf.Abs(a.b - b.b) < colorTolerance;
	}
}
