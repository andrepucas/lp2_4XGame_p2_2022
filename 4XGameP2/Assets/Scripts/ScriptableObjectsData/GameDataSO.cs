using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds a set of values.
/// </summary>
[System.Serializable]
public struct PresetValues
{
    [Header("UNIVERSAL")]
    [Tooltip("Name, as should be displayed.")]
    [SerializeField] private string _name;
    [Tooltip("Coin base value.")]
    [SerializeField] private int _coin;
    [Tooltip("Food base value.")]
    [SerializeField] private int _food;
    [Tooltip("Sprites for this terrain/resource. Can never be null.\n" + 
    "For terrains: 0 = default; 1 = hover variation\n" +
    "For resources: terrain variations. (If it doesn't vary, only insert one. " +
    "If at least one varies, specify variations for each terrain type.")]
    [SerializeField] private Sprite[] _sprites;
    [Header("FOR RESOURCES ONLY")]
    [Tooltip("Sprite that is to be displayed outside the cell's context. " + 
    "Must be full-res.")]
    [SerializeField] private Sprite _defaultSprite;

    public string Name => _name;
    public int Coin => _coin;
    public int Food => _food;
    public IReadOnlyList<Sprite> Sprites => _sprites;
    public Sprite DefaultResourceSprite => _defaultSprite;

    // Get name in lowercase and without spaces.
    // https://stackoverflow.com/questions/6219454/efficient-way-to-remove-all-whitespace-from-string/30732794#30732794
    public string RawName => string.Join("", _name.Split(default(string[]), 
        System.StringSplitOptions.RemoveEmptyEntries)).ToLower();
}

/// <summary>
/// Holds all possible terrains and resources's values.
/// </summary>
[CreateAssetMenu(fileName = "GameData", menuName = "Data/Game Data")]
public class GameDataSO : ScriptableObject
{
    // Serialized
    [Header("PRESET VALUES")]
    [Tooltip("All possible terrains.")]
    [SerializeField] private PresetValues[] _terrains;
    [Tooltip("All possible resources.")]
    [SerializeField] private PresetValues[] _resources;

    /// <summary>
    /// Readonly self-implemented property list of all possible terrains.
    /// </summary>
    public IReadOnlyList<PresetValues> Terrains => _terrains;

    /// <summary>
    /// Readonly self-implemented property list of all possible resources.
    /// </summary>
    public IReadOnlyList<PresetValues> Resources => _resources;

    /// <summary>
    /// Property that returns all terrains' raw names.
    /// </summary>
    /// <value>String IEnumerable of all terrains' raw names.</value>
    public IEnumerable<string> TerrainNames
    {
        get
        {
            foreach (PresetValues t in _terrains)
                yield return t.RawName;
        }
    }

    /// <summary>
    /// Property that returns all resources' raw names.
    /// </summary>
    /// <value>String IEnumerable of all resources' raw names.</value>
    public IEnumerable<string> ResourceNames
    {
        get
        {
            foreach (PresetValues r in _resources)
                yield return r.RawName;
        }
    }

    /// <summary>
    /// Method that returns a dictionary of the terrain sprite variations for
    /// a given resource.
    /// </summary>
    /// <param name="p_resource">Resource's Name.</param>
    /// <returns>Terrain sprite variations for resource.</returns>
    public Dictionary<string, Sprite> GetSpriteDictOf(string p_resource)
    {
        Dictionary<string, Sprite> m_rSprites = new Dictionary<string, Sprite>();
        IReadOnlyList<Sprite> m_sprites = null;

        // Iterates all valid resources.
        foreach (PresetValues resource in Resources)
        {
            // Finds given resource, based on it's name.
            if (resource.Name == p_resource)
            {
                // Saves it's sprites.
                m_sprites = resource.Sprites;
                break;
            }
        }

        // Iterates all valid terrains.
        for (int i = 0; i < Terrains.Count; i++)
        {
            // If the number of sprite variations is greater than the current 
            // terrain index, save this terrain name and its correspondent 
            // sprite variation.
            if (i < m_sprites.Count)
                m_rSprites.Add(Terrains[i].Name, m_sprites[i]);

            // If not (there are less variations than terrains), save this terrain
            // name with the first sprite.
            else m_rSprites.Add(Terrains[i].Name, m_sprites[0]);
        }

        // Return dictionary.
        return m_rSprites;
    }
}