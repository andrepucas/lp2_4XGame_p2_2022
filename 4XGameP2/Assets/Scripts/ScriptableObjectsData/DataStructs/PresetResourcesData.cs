using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds preset data regarding game resources.
/// </summary>
[System.Serializable]
public struct PresetResourcesData
{
    // Serialized variables.
    [Tooltip("Name, as should be displayed.")]
    [SerializeField] private string _name;
    [Tooltip("Terrain resource variations for this resource.\n" + 
        "If it doesn't vary, only insert one.\n" +
        "If at least one varies, specify variations for each terrain type.")]
    [SerializeField] private Sprite[] _sprites;
    [Tooltip("Sprite that is to be displayed outside the cell's context. " + 
        "Must be full-res.")]
    [SerializeField] private Sprite _defaultSprite;
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
    public Sprite DefaultResourceSprite => _defaultSprite;
    public int Coin => _coin;
    public int Food => _food;
}
