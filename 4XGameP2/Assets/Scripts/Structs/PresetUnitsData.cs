using System.Collections.Generic;
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
    [Tooltip("Icon Sprites")]
    [SerializeField] private Sprite _baseIcon, _frontIcon;
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
    public Sprite BaseIcon => _baseIcon;
    public Sprite FrontIcon => _frontIcon;
    public MovementType Movement => _movement;
    public IReadOnlyList<string> ResourceNamesToCollect => _resourceNamesToCollect;
    public IReadOnlyList<string> ResourceNamesToGenerate => _resourceNamesToGenerate;
}
