/// <summary>
/// Contains all the info about a animals resource.
/// </summary>
public class AnimalsResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Animals.</value>
    public override string Name => "Animals";

    /// <summary>
    /// Read only self implemented property that stores the coin value of this
    /// resource.
    /// </summary>
    /// <value>1.</value>
    public override int Coin => 1;

    /// <summary>
    /// Read only self implemented property that stores the food value of this 
    /// resource.
    /// </summary>
    /// <value>3.</value>
    public override int Food => 3;
}
