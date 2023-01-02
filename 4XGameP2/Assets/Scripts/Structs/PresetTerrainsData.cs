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
    [Tooltip("Coin base value.")]
    [SerializeField] private int _coin;
    [Tooltip("Food base value.")]
    [SerializeField] private int _food;
    [Tooltip("Sprites for this terrain.\n0 = default; 1 = hover variation.")]
    [SerializeField] private Sprite[] _sprites;

    // Read only properties that return the serialized variables' values.
    public string Name => _name;
    public string RawName => string.Join("", _name.Split(default(string[]), 
        System.StringSplitOptions.RemoveEmptyEntries)).ToLower();
    public int Coin => _coin;
    public int Food => _food;
    public IReadOnlyList<Sprite> Sprites => _sprites;

    // Return name in lowercase and without spaces.
    // https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
}
