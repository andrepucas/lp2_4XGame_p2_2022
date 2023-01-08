using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Units drag selection with input from controller.
/// </summary>
public class UnitSelection : MonoBehaviour
{
    // Private collection containing all spawned units.
    private ICollection<Unit> _unitsInGame;

    // Private collection containing all selected units.
    private ICollection<Unit> _unitsSelected;

    /// <summary>
    /// Called by controller on Awake, initializes collections.
    /// </summary>
    public void Initialize()
    {
        _unitsInGame = new HashSet<Unit>();
        _unitsSelected = new HashSet<Unit>();
    }
}
