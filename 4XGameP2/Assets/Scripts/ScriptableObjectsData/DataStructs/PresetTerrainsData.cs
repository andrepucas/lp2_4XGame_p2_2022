using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds preset data regarding game terrains.
/// </summary>
[System.Serializable]
public struct PresetTerrainsData
{
    // Serialized variables.
    [Tooltip("Name, as should be displayed.")]
    [SerializeField] private string _name;
    [Tooltip("Sprites for this terrain.\n0 = default; 1 = hover variation.")]
    [SerializeField] private Sprite[] _sprites;
    [Header("STATS")]
    [Tooltip("Coin base value.")]
    [SerializeField] private int _coin;
    [Tooltip("Food base value.")]
    [SerializeField] private int _food;

    // Read only properties that return the serialized variables' values.
    public string Name => _name;
    public string RawName => string.Join("", _name.Split(default(string[]), 
        System.StringSplitOptions.RemoveEmptyEntries)).ToLower();
    public IReadOnlyList<Sprite> Sprites => _sprites;
    public int Coin => _coin;
    public int Food => _food;
}
