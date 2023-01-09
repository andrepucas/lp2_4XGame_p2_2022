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

    /// <summary>
    /// Self implemented property that holds the number of map cells.
    /// </summary>
    /// <value>Number of cells in the generated mad.</value>
    public int MapCellsCount {get; private set;}

    /// <summary>
    /// Self implemented property that holds the value of a map cell size.
    /// </summary>
    /// <value>Size of a map cell after being generated.</value>
    public float MapCellSize {get; private set;}

    /// <summary>
    /// Self implemented property that holds the number of spawned map units.
    /// </summary>
    /// <value>Number of units in the map.</value>
    public int MapUnitsCount {get; private set;}

    // Private dictionary containing cells and respective positions.
    private Dictionary<Vector2, MapCell> _mapCells;

    // Private dictionary containing units and respective positions.
    private Dictionary<Vector2, Unit> _mapUnits;

    /// <summary>
    /// Saves map info and creates new dictionaries.
    /// </summary>
    /// <param name="p_mapSize">Number of map cells.</param>
    /// <param name="p_cellSize">Map cell size.</param>
    public void NewMap(int p_mapSize, float p_cellSize)
    {
        MapCellsCount = p_mapSize;
        MapCellSize = p_cellSize;

        _mapCells = new Dictionary<Vector2, MapCell>();
        _mapUnits = new Dictionary<Vector2, Unit>();

        MapUnitsCount = 0;
    }

    /// <summary>
    /// Adds elements to dictionaries as map is generated.
    /// </summary>
    /// <param name="p_cell">Map Cell.</param>
    /// <param name="p_mapPos">Map relative position.</param>
    public void SetupMapInfo(MapCell p_cell, Vector2 p_mapPos)
    {
        // In each map position, saves its respective cell.
        _mapCells.Add(p_mapPos, p_cell);

        // Saves map positions with null Units, as these don't exist yet.
        _mapUnits.Add(p_mapPos, null);
    }

    /// <summary>
    /// Adds unit to map dictionary.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    /// <param name="p_mapPos">Relative map position.</param>
    public void AddUnitTo(Unit p_unit, Vector2 p_mapPos)
    {
        _mapUnits[p_mapPos] = p_unit;
        MapUnitsCount++;
    }

    /// <summary>
    /// Removes unit from map dictionary (sets it to null).
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    public void RemoveUnit(Unit p_unit)
    {
        _mapUnits[p_unit.MapPosition] = null;
        MapUnitsCount--;
    }

    /// <summary>
    /// Removes unit from old relative map position and adds it to the new 
    /// relative map position.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    /// <param name="p_newMapPos">New relative map position.</param>
    public void MoveUnitTo(Unit p_unit, Vector2 p_newMapPos)
    {
        _mapUnits[p_unit.MapPosition] = null;

        _mapUnits[p_newMapPos] = p_unit;
        p_unit.MapPosition = p_newMapPos;
    }
}
