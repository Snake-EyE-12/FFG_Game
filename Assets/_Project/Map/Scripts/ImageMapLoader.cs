using System.Collections.Generic;
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


    // COLORS FOR LEVEL BUILDING
    readonly Color blue = new Color(0f, 0f, 1f);          // #0000FF - AIR
    readonly Color green = new Color(0f, 1f, 0f);          // #00FF00 - JUST GROUND
    readonly Color yellow = new Color(1f, 1f, 0f);          // #FFFF00 - 1 HIGH COVER
    readonly Color red = new Color(1f, 0f, 0f);          // #FF0000 - 2 HIGH COVER
    readonly Color cyan = new Color(0f, 1f, 1f);        // 00FFFF - SPAWN POINT

    private void Awake()
    {
        mapIndex = Random.Range(0, mapCollection.maps.Length);

        LoadMap();

        GetComponent<MeshCombiner>().CombineMeshesPerMaterial();

        Spawning.spawnPlane = transform;
    }

    void LoadMap()
    {
        Spawning.spawnPoints = new List<Transform>();

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
                // Flip Y so the map appears top-down correctly
                int flippedY = map.height - y - 1;
                Color pixel = map.GetPixel(x, flippedY);
                SpawnFromPixel(pixel, x, y);
            }
        }
    }

    void SpawnFromPixel(Color pixel, int x, int y)
    {
        Vector3 basePos = new Vector3(x * blockSize, -.5f, y * blockSize);
        
        if(IsCloseColor(pixel, cyan)) // SPAWNPOINT
        {
            GameObject spawnBlock = Instantiate(spawnpointPrefab, basePos, Quaternion.identity, transform);
            Spawning.spawnPoints.Add(spawnBlock.transform.GetChild(0));
        }

        int stackHeight = -1;

        if (IsCloseColor(pixel, blue)) return;
        else if (IsCloseColor(pixel, green)) stackHeight = 1;
        else if (IsCloseColor(pixel, yellow)) stackHeight = 2;
        else if (IsCloseColor(pixel, red)) stackHeight = 3;

        if (stackHeight == -1) return;


        // always spawn ground
        Instantiate(groundPrefab, basePos, Quaternion.identity, transform);

        // layers of cover (1 or 2)
        for (int i = 1; i < stackHeight; i++)
        {
            Vector3 pos = basePos + new Vector3(0, i * blockSize - .5f, 0);
            Instantiate(coverPrefab, pos, Quaternion.identity, transform);
        }
    }

    bool IsCloseColor(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < colorTolerance &&
               Mathf.Abs(a.g - b.g) < colorTolerance &&
               Mathf.Abs(a.b - b.b) < colorTolerance;
    }
}
