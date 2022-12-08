/// <summary>
/// Contains all the info about a metals resource.
/// </summary>
public class MetalsResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Metals.</value>
    public override string Name => "Metals";

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>3.</value>
    public override int Coin => 3;

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>-1.</value>
    public override int Food => -1;
}
