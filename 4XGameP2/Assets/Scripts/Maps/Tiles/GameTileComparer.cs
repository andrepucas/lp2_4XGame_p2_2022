using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Handles Linq distinct to conduct game tile comparisons. 
/// </summary>
public class GameTileComparer : IEqualityComparer<GameTile>
{
    // Reference to String Builder.
    StringBuilder _stringB;

    // IEnumerables to store resources names to compare.
    IEnumerable<string> _firstRStrings; 
    IEnumerable<string> _secondRStrings; 

    /// <summary>
    /// Compares the tiles' names and resources names.
    /// </summary>
    /// <param name="p_first">First of two game tiles to compare.</param>
    /// <param name="p_second">Second of two game tiles to compare.</param>
    /// <returns>true if the tile's names and resources are the same. |
    /// false if the tile's names and resources are not the same.</returns>
    public bool Equals(GameTile p_first, GameTile p_second)
    {
        // Saves the tiles string names in the corresponding variables. 
        _firstRStrings = p_first.Resources.Select(t => t.Name);
        _secondRStrings = p_second.Resources.Select(t => t.Name);

        // Returns true if the tile's names and resources are the same.
        return p_first.Name == p_second.Name && _firstRStrings.All(_secondRStrings.Contains);
    }

    /// <summary>
    /// Compares the game tiles Hash Code.
    /// </summary>
    /// <param name="p_tile">Game Tile to compare to.</param>
    /// <returns>true if the game tiles Hash Codes are the same | 
    /// false if the game tiles Hash Codes aren't the same.</returns>
    public int GetHashCode(GameTile p_tile)
    {
        // Local String Builder instance. 
        _stringB = new StringBuilder();

        // Goes through each resource and adds its name to string.
        foreach (string r in p_tile.Resources.Select(t => t.Name))
            _stringB.Append(r);

        // Returns true if the game tiles' Hash Codes are the same.
        return p_tile.Name.GetHashCode() ^ _stringB.ToString().GetHashCode();
    }
}
