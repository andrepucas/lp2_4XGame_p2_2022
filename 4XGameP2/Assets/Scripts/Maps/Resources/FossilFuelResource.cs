/// <summary>
/// Contains all the info about a fossil fuel resource.
/// </summary>
public class FossilFuelResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Fossil Fuel.</value>
    public override string Name => "Fossil Fuel";

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>5.</value>
    public override int Coin => 5;

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>-3.</value>
    public override int Food => -3;
}
