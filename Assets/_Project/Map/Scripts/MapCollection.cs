using UnityEngine;

[CreateAssetMenu(fileName = "MapCollection", menuName = "Maps/Map Collection")]
public class MapCollection : ScriptableObject
{
    [Tooltip("List of map textures to use")]
    public Texture2D[] maps;
}
