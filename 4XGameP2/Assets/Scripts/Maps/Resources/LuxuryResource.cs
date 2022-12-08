/// <summary>
/// Contains all the info about a luxury resource.
/// </summary>
public class LuxuryResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Luxury.</value>
    public override string Name => "Luxury";

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>4.</value>
    public override int Coin => 4;

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>-1.</value>
    public override int Food => -1;
}
