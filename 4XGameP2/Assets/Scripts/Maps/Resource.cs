using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// Contains all info about an individual resource.
/// </summary>
public class Resource : IComparable<Resource>
{
    /// <summary>
    /// Read only self implemented property that returns the name of this
    /// resource.
    /// </summary>
    /// <value>Name of the resource.</value>
    public string Name {get;}

    /// <summary>
    /// Read only self implemented property that returns the coin value of this
    /// resource.
    /// </summary>
    /// <value>Coin value of the resource.</value>
    public int Coin {get;}

    /// <summary>
    /// Read only self implemented property that returns the food value of this 
    /// resource.
    /// </summary>
    /// <value>Food of the resource.</value>
    public int Food {get;}

    /// <summary>
    /// Read only self implemented property that returns all sprite variations 
    /// for this resource.
    /// </summary>
    public IReadOnlyDictionary<string, Sprite> SpritesDict {get;}

    public Sprite DefaultSprite {get;}

    /// <summary>
    /// Constructor method. Sets properties' values.
    /// </summary>
    /// <param name="p_name">Name.</param>
    /// <param name="p_coin">Coin value.</param>
    /// <param name="p_food">Food value.</param>
    /// <param name="p_sprites">Sprite variations.</param>
    /// <param name="p_defaultSprite">Default sprite for resource type.</param>
    public Resource(string p_name, int p_coin, int p_food, 
        Dictionary<string, Sprite> p_sprites, Sprite p_defaultSprite)
    {
        Name = p_name;
        Coin = p_coin;
        Food = p_food;
        SpritesDict = p_sprites;
        DefaultSprite = p_defaultSprite;
    }

    /// <summary>
    /// Compares resources using its name, to sort.
    /// </summary>
    /// <param name="other">Resource to compare to.</param>
    /// <returns></returns>
    public int CompareTo(Resource other) => Name.CompareTo(other?.Name);
}
