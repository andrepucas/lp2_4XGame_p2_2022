using System.Collections.Generic;

/// <summary>
/// <c>Game Tile</c> Class.
/// Contains all info about each individual game tile.
/// </summary>
public class GameTile
{
    /// <summary>
    /// Read only self implemented property that stores the name of the tile.
    /// </summary>
    /// <value>Name of the tile.</value>
    public string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores the base coin value of 
    /// this game tile.
    /// </summary>
    /// <value>Base Coin of the game tile.</value>
    public int BaseCoin { get; }

    /// <summary>
    /// Read only self implemented property that stores the base food value of 
    /// this game tile.
    /// </summary>
    /// <value>Base Food of the game tile.</value>
    public int BaseFood { get; }

    /// <summary>
    /// Read only property that stores the total coin value of this game tile,
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
    /// Read only property that stores the total food value of this game tile,
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
    /// Read only self implemented property that stores all the current resources
    /// of this game tile.
    /// </summary>
    /// <value>Current resources of the game tile.</value>
    public IReadOnlyCollection<Resource> Resources => _resourceList;

    /// <summary>
    /// Private list of Resources.
    /// </summary>
    private List<Resource> _resourceList;

    //
    public GameTile(string p_name, int p_coin, int p_food)
    {
        Name = p_name;
        BaseCoin = p_coin;
        BaseFood = p_food;

        _resourceList = new List<Resource>();
    }

    /// <summary>
    /// Adds a resource to tile resource ICollection.
    /// </summary>
    /// <param name="resource">Resource to add to tile.</param>
    public void AddResource(Resource resource) => _resourceList.Add(resource);

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
