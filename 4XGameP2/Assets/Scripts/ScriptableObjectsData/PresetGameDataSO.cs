using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all preset game data.
/// </summary>
[CreateAssetMenu(fileName = "PresetGameData", menuName = "Data/Preset Game Data")]
public class PresetGameDataSO : ScriptableObject
{
    // Serialized
    [Tooltip("All possible terrains.")]
    [SerializeField] private PresetTerrainsData[] _terrains;
    [Tooltip("All possible resources.")]
    [SerializeField] private PresetResourcesData[] _resources;
    [Tooltip("All possible units.")]
    [SerializeField] private PresetUnitsData[] _units;
    [Tooltip("Float ratio value of unit display's size.")]
    [SerializeField] private float _unitDisplaySize = 0.2f;
    [Tooltip("Float ratio value of unit display's offset.")]
    [SerializeField] private float _unitDisplayOffset = -0.25f;

    /// <summary>
    /// Readonly self-implemented property list of all possible terrains.
    /// </summary>
    public IReadOnlyList<PresetTerrainsData> Terrains => _terrains;

    /// <summary>
    /// Readonly self-implemented property list of all possible resources.
    /// </summary>
    public IReadOnlyList<PresetResourcesData> Resources => _resources;

    /// <summary>
    /// Readonly self-implemented property list of all possible units.
    /// </summary>
    public IReadOnlyList<PresetUnitsData> Units => _units;

    /// <summary>
    /// Readonly self implemented property that returns screen size ratio of a unit.
    /// </summary>
    public float UnitDisplaySize => _unitDisplaySize;

    /// <summary>
    /// Readonly self implemented property that returns cell offset ratio of a unit.
    /// </summary>
    public float UnitDisplayOffset => _unitDisplayOffset;

    /// <summary>
    /// Property that returns all terrains' raw names.
    /// </summary>
    /// <value>String IEnumerable of all terrains' raw names.</value>
    public IEnumerable<string> TerrainNames
    {
        get
        {
            foreach (PresetTerrainsData t in _terrains)
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
            foreach (PresetResourcesData r in _resources)
                yield return r.RawName;
        }
    }


    /// <summary>
    /// MISSING COMMENTS <--------------------------------------------------------------------------------------------------------------------
    /// </summary>
    /// <value></value>
    public IReadOnlyDictionary<string, Sprite> ResourceDefaultSprites
    {
        get
        {
            Dictionary<string, Sprite> _resourceIcons = new Dictionary<string, Sprite>();

            foreach (PresetResourcesData f_resource in Resources)
                _resourceIcons.Add(f_resource.Name, f_resource.DefaultResourceSprite);

            return _resourceIcons;
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
        foreach (PresetResourcesData resource in Resources)
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
