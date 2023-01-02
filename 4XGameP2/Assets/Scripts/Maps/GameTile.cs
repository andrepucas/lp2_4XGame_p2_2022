using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Game Tile</c> Class.
/// Contains all info about each individual game tile.
/// </summary>
public class GameTile
{
    /// <summary>
    /// Read only self implemented property that returns the name of the tile.
    /// </summary>
    /// <value>Name of the tile (terrain).</value>
    public string Name {get;}

    /// <summary>
    /// Read only self implemented property that returns the base coin value of 
    /// the tile.
    /// </summary>
    /// <value>Base Coin of the game tile.</value>
    public int BaseCoin {get;}

    /// <summary>
    /// Read only self implemented property that returns the base food value of 
    /// the tile.
    /// </summary>
    /// <value>Base Food of the game tile.</value>
    public int BaseFood {get;}

    /// <summary>
    /// Read only property that returns the total coin value of this game tile,
    /// based on it's resources and base value.
    /// </summary>
    /// <value>Total Coin value of the game tile.</value>
    public int Coin 
    {
        get
        {
            int resourcesCoinSum = 0;
            foreach (Resource r in Resources) 
                resourcesCoinSum += r.Coin;

            return BaseCoin + resourcesCoinSum;
        }
    }

    /// <summary>
    /// Read only property that returns the total food value of this game tile,
    /// based on it's resources and base value.
    /// </summary>
    /// <value>Total Food of the game tile.</value>
    public int Food
    {
        get
        {
            int resourcesFoodSum = 0;
            foreach (Resource r in Resources)
                resourcesFoodSum += r.Food;

            return BaseFood + resourcesFoodSum;
        }
    }

    /// <summary>
    /// Read only self implemented property that returns the base and hovered
    /// of this tile/terrain.
    /// </summary>
    /// <value>Base and Hovered sprites.</value>
    public IReadOnlyList<Sprite> Sprites {get;}

    /// <summary>
    /// Read only self implemented property that stores all the current resources
    /// of this game tile.
    /// </summary>
    /// <value>Current resources of the game tile.</value>
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
    /// <param name="p_coin">Coin Value.</param>
    /// <param name="p_food">Food Value.</param>
    /// <param name="p_sprites">Base and Hovered sprites.</param>
    public GameTile(string p_name, int p_coin, int p_food, IReadOnlyList<Sprite> p_sprites)
    {
        Name = p_name;
        BaseCoin = p_coin;
        BaseFood = p_food;
        Sprites = p_sprites;

        // Initializes list.
        _resourceList = new List<Resource>();
    }

    /// <summary>
    /// Adds a resource to the list and sorts it alphabetically.
    /// </summary>
    /// <param name="resource">Resource to add to tile.</param>
    public void AddResource(Resource resource)
    {
        _resourceList.Add(resource);
        _resourceList.Sort();
    }

    /// <summary>
    /// Shows all of the tile's important information.
    /// </summary>
    /// <remarks>Specially useful for debugging.</remarks>
    /// <returns>A string with all of the tile's info</returns>
    public override string ToString()
    {
        // String to return.
        string m_info = $"\t || {Name} [BC: {BaseCoin}, BF: {BaseFood}] ||";

        // Goes through each resource.
        foreach (Resource resource in Resources)
        {
            // Stores relevant resource information in temporary string.
            m_info += $" + {resource.Name} [C: {resource.Coin}, F: {resource.Food}]";
        }

        // Stores current total Coin and Food values in temporary string.
        m_info += $" => [C: {Coin}, F: {Food}]";

        // Return said string.
        return m_info;
    }
}
