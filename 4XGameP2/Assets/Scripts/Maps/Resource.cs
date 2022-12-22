/// <summary>
/// Contains all info about an individual resource.
/// </summary>
public class Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Name of the resource.</value>
    public string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>Coin value of the resource.</value>
    public int Coin { get; }

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>Food of the resource.</value>
    public int Food { get; }

    public Resource(string p_name, int p_coin, int p_food)
    {
        Name = p_name;
        Coin = p_coin;
        Food = p_food;
    }
}
