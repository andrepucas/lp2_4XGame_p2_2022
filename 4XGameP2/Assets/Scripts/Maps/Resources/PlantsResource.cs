/// <summary>
/// Contains all the info about a plants resource.
/// </summary>
public class PlantsResource : Resource
{
    /// <summary>
    /// Read only self implemented property that stores the name of this
    /// resource.
    /// </summary>
    /// <value>Plants.</value>
    public override string Name => "Plants";

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
    /// <value>2.</value>
    public override int Food => 2;
}
