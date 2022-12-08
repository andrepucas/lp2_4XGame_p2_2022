using UnityEngine;

/// <summary>
/// Holds the data necessary to generate new maps. All terrains and resources.
/// </summary>
[CreateAssetMenu(fileName = "MapFileGeneratorData", menuName = "Data/Map File Generator Data")]
public class MapFileGeneratorDataSO : ScriptableObject
{
    // Serialized
    [Header("MAP GENERATOR DATA")]
    [Tooltip("All possible terrains.")]
    [SerializeField] private string[] _terrains;
    [Tooltip("All possible resources.")]
    [SerializeField] private string[] _resources;

    /// <summary>
    /// String array of all possible terrain names.
    /// </summary>
    public string[] Terrains => _terrains;

    /// <summary>
    /// String array of all possible resource names.
    /// </summary>
    public string[] Resources => _resources;
}
