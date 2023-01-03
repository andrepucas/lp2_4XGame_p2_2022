using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all ongoing game data.
/// </summary>
[CreateAssetMenu(fileName = "OngoingGameData", menuName = "Data/Ongoing Game Data")]
public class OngoingGameDataSO : ScriptableObject
{
    /// <summary>
    /// Readonly dictionary that stores all cells and respective map positions.
    /// </summary>
    public IReadOnlyDictionary<Vector2, MapCell> MapCells => _mapCells;

    /// <summary>
    /// Readonly dictionary that stores all units and respective map positions.
    /// </summary>
    public IReadOnlyDictionary<Vector2, Unit> MapUnits => _mapUnits;

    // Private dictionary containing cells and respective positions.
    private Dictionary<Vector2, MapCell> _mapCells;

    // Private dictionary containing units and respective positions.
    private Dictionary<Vector2, Unit> _mapUnits;

    public void NewMap()
    {
        _mapCells = new Dictionary<Vector2, MapCell>();
        _mapUnits = new Dictionary<Vector2, Unit>();
    }

    public void SetupMapInfo(MapCell p_cell, Vector2 p_mapPos)
    {
        _mapCells.Add(p_mapPos, p_cell);
        _mapUnits.Add(p_mapPos, null);
    }

    public void AddUnitTo(Unit p_unit, Vector2 p_mapPos) => 
        _mapUnits[p_mapPos] = p_unit;

    public void RemoveUnitFrom(Unit p_unit, Vector2 p_mapPos) => 
        _mapUnits[p_mapPos] = null;

    public void MoveUnitFromTo(Unit p_unit, Vector2 p_oldMapPos, Vector2 p_newMapPos)
    {
        _mapUnits[p_oldMapPos] = null;
        _mapUnits[p_newMapPos] = p_unit;
    }
}
