using System.Collections.Generic;

/// <summary>
/// <c>Desert Tile</c> Class.
/// Contains all info about a desert tile.
/// </summary>
public class DesertTile : GameTile
{
    /// <summary>
    /// Read only self implemented property that stores the name of the tile.
    /// </summary>
    /// <value>Desert.</value>
    public override string Name => "Desert";

    /// <summary>
    /// Read only self implemented property that stores the base coin value of 
    /// this game tile.
    /// </summary>
    /// <value>0.</value>
    public override int BaseCoin => 0;

    /// <summary>
    /// Read only self implemented property that stores the base food value of 
    /// this game tile.
    /// </summary>
    /// <value>0.</value>
    public override int BaseFood => 0;

    /// <summary>
    /// Property that stores the total coin value of this game tile.
    /// </summary>
    /// <value>Total Coin value of the game tile.</value>
    public override int Coin { get; protected set; }

    /// <summary>
    /// Property that stores the total food value of this game tile.
    /// </summary>
    /// <value>Total Food of the game tile.</value>
    public override int Food { get; protected set; }

    /// <summary>
    /// Overrides IEnumerable<Resource> and stores it in resourceList.
    /// </summary>
    public override ICollection<Resource> Resources => resourceList;

    /// <summary>
    /// Creates a list of the Resource type.
    /// </summary>
    /// <typeparam name="Resource">Resources present in game tiles.</typeparam>
    /// <returns>Stores resources in current tile.</returns>
    private List<Resource> resourceList = new List<Resource>();

    /// <summary>
    /// Constructor method. Initializes a properties' values.
    /// </summary>
    /// <param name="Coin">Desert Tile's total coin value.</param>
    /// <param name="Food">Desert Tile's total food value.</param>
    public DesertTile()
    {
        // Sets base coin and food values.
        Coin = BaseCoin;
        Food = BaseFood;
    }

    /// <summary>
    /// Adds a resource to tile resourceList.
    /// </summary>
    /// <param name="resource">Resource to add to tile.</param>
    public override void AddResource(Resource resource)
    {
        // Adds resource to List.
        resourceList.Add(resource);

        // Adds individual resource values to the tile's total values.
        Coin += resource.Coin;
        Food += resource.Food;
    }

    /// <summary>
    /// Shows all of the tile's important information.
    /// </summary>
    /// <remarks>Specially useful for debugging.</remarks>
    /// <returns>A string with all of the tile's info</returns>
    public override string ToString()
    {
        // Returns the tile's important information.
        return $"{GetType().Name} TILE [C: {BaseCoin}, F: {BaseFood}] " + base.ToString();
    }
}
