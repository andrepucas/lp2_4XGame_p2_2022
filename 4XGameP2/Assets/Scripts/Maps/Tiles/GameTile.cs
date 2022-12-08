using System.Collections.Generic;

/// <summary>
/// <c>Game Tile</c> Class.
/// Contains all the generic info about each individual game tile.
/// </summary>
public abstract class GameTile
{
    /// <summary>
    /// Read only self implemented property that stores the name of the tile.
    /// </summary>
    /// <value>Name of the tile.</value>
    public abstract string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores the base coin value of 
    /// this game tile.
    /// </summary>
    /// <value>Base Coin of the game tile.</value>
    public abstract int BaseCoin { get; }

    /// <summary>
    /// Read only self implemented property that stores the base food value of 
    /// this game tile.
    /// </summary>
    /// <value>Base Food of the game tile.</value>
    public abstract int BaseFood { get; }

    /// <summary>
    /// Self implemented property that stores the total coin value of this game
    /// tile.
    /// </summary>
    /// <value>Total Coin value of the game tile.</value>
    public abstract int Coin { get; protected set; }

    /// <summary>
    /// Self implemented property that stores the total food value of this game 
    /// tile.
    /// </summary>
    /// <value>Total Food of the game tile.</value>
    public abstract int Food { get; protected set; }

    /// <summary>
    /// Read only self implemented property that stores all the current resources
    /// of this game tile.
    /// </summary>
    /// <value>Current resources of the game tile.</value>
    public abstract ICollection<Resource> Resources { get; }

    /// <summary>
    /// Adds a resource to tile resource ICollection.
    /// </summary>
    /// <param name="resource">Resource to add to tile.</param>
    public abstract void AddResource(Resource resource);

    /// <summary>
    /// Shows all of the tile's important information.
    /// </summary>
    /// <remarks>Specially useful for debugging.</remarks>
    /// <returns>A string with all of the tile's info</returns>
    public override string ToString()
    {
        // Temporary empty string.
        string m_info = "";

        // Goes through each resource.
        foreach (Resource resource in Resources)
        {
            // Stores relevant resource information in temporary string.
            m_info += $" + {resource.GetType().Name} [C: {resource.Coin}, F: {resource.Food}]";
        }

        // Stores current total Coin and Food values in temporary string.
        m_info += $" => [C: {Coin}, F: {Food}]";

        // Return said string.
        return m_info;
    }
}
