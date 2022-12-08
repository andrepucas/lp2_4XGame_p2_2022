/// <summary>
/// Contains all the generic info about this individual resource.
/// </summary>
public abstract class Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Name of the resource.</value>
    public abstract string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>Coin value of the resource.</value>
    public abstract int Coin { get; }

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>Food of the resource.</value>
    public abstract int Food { get; }
}
