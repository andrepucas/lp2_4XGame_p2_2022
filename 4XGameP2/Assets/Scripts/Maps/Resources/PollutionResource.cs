/// <summary>
/// Contains all the info about a pollution resource.
/// </summary>
public class PollutionResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Pollution.</value>
    public override string Name => "Pollution";

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>-3.</value>
    public override int Coin => -3;

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>-3.</value>
    public override int Food => -3;
}
