using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all ongoing game data.
/// </summary>
[CreateAssetMenu(fileName = "OngoingGameData", menuName = "Data/Ongoing Game Data")]
public class OngoingGameDataSO : ScriptableObject
{
    /// <summary>
    /// Readonly dictionary that returns all cells and respective map positions.
    /// </summary>
    public IReadOnlyDictionary<Vector2, MapCell> MapCells => _mapCells;

    // Private dictionary containing cells and respective positions.
    private Dictionary<Vector2, MapCell> _mapCells;

    public void NewMap() => 
        _mapCells = new Dictionary<Vector2, MapCell>();

    public void SaveMapInfo(MapCell p_cell, Vector2 p_mapPos) =>
        _mapCells.Add(p_mapPos, p_cell);
}
