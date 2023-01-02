using UnityEngine;

/// <summary>
/// Holds preset data regarding game units.
/// </summary>
[System.Serializable]
public struct PresetUnitsData
{
    // Serialized variables.
    [Tooltip("Name, as should be displayed.")]
    [SerializeField] private string _name;
    [Tooltip("Icon Sprite, in full res.")]
    [SerializeField] private Sprite _icon;
    [Tooltip("Icon Sprite to be displayed in a cell.")]
    [SerializeField] private Sprite _cellSprite;
    [Tooltip("Icon Sprite to be displayed in a cell.")]
    [SerializeField] private Color _color;
    [Header("ACTIONS")]
    [Tooltip("Unit's movement type.")]
    [SerializeField] private MovementType _movement;
    [Tooltip("String name of resources this unit collects.")]
    [SerializeField] private string[] _resourceNamesToCollect;
    [Tooltip("String name of resources this unit generates after collecting.")]
    [SerializeField] private string[] _resourceNamesToGenerate;

    // Read only properties that return the serialized variables' values.
    public string Name => _name;
    public string RawName => string.Join("", _name.Split(default(string[]), 
        System.StringSplitOptions.RemoveEmptyEntries)).ToLower();
    public Sprite Icon => _icon;
    public Sprite CellSprite => _cellSprite;
    public Color Color => _color;
    public MovementType Movement => _movement;
    public string[] ResourceNamesToCollect => _resourceNamesToCollect;
    public string[] ResourceNamesToGenerate => _resourceNamesToGenerate;
}
