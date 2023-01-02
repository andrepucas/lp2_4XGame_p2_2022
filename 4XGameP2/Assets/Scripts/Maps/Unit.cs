using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    /// <summary>
    /// Read only self implemented property that stores the name of the unit.
    /// </summary>
    /// <value>Name of the unit (type).</value>
    public string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores all the current resources
    /// of this unit.
    /// </summary>
    /// <value>Current resources of the unit.</value>
    public IReadOnlyList<Resource> Resources => _resourceList;

    /// <summary>
    /// Private list of Resources.
    /// </summary>
    private List<Resource> _resourceList;

    /// <summary>
    /// Constructor method. 
    /// Sets properties' values and initializes resources list.
    /// </summary>
    /// <param name="p_name">Name.</param>
    public Unit(string p_name)
    {
        Name = p_name;

        // Initializes list.
        _resourceList = new List<Resource>();
    }



    public void MoveTowardsTile(GameTile selectedTile)
    {

    }

    public void HarvestCurrentTile(GameTile currentTile)
    {

    }

}
