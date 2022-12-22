using UnityEngine;

[System.Serializable]
public struct TileValues
{
    [SerializeField] private string _name;
    [SerializeField] private int _coin;
    [SerializeField] private int _food;

    public string Name => _name;
    public int Coin => _coin;
    public int Food => _food;

    public string RawName => string.Join("", _name.Split(default(string[]), 
        System.StringSplitOptions.RemoveEmptyEntries)).ToLower(); // https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
}

/// <summary>
/// Holds the data necessary to generate new maps. All terrains and resources.
/// </summary>
[CreateAssetMenu(fileName = "MapTilesData", menuName = "Data/Map Tile Data")]
public class MapTilesDataSO : ScriptableObject
{
    // Serialized
    [Header("MAP TILES DATA")]
    [Tooltip("All possible terrains.")]
    [SerializeField] private TileValues[] _terrains;
    [Tooltip("All possible resources.")]
    [SerializeField] private TileValues[] _resources;

    /// <summary>
    /// Tile Info array of all possible terrains.
    /// </summary>
    public TileValues[] Terrains => _terrains;

    /// <summary>
    /// Tile Info array of all possible resources.
    /// </summary>
    public TileValues[] Resources => _resources;
}
