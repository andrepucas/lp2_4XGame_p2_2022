using UnityEngine;

/// <summary>
/// Specific unit movement behaviour, Moore.
/// Makes it so the next move can go in 8 2D directions: 
/// up, down, left or right + diagonals.
/// </summary>
public class MooreMoveBehaviour : MonoBehaviour, IUnitMoveBehaviour
{
    /// <summary>
    /// Returns next move towards the target position. 
    /// </summary>
    /// <param name="p_start">Start position.</param>
    /// <param name="p_target">Target position.</param>
    /// <returns>Start position + (Up, Down, Left, Right or Diagonal) Vector.</returns>
    public Vector2 GetNextMove(Vector2 p_start, Vector2 p_target)
    {
        // Sets next move base as start position.
        Vector2 m_nextMove = p_start;

        // Calculates normalized distance vector to target.
        Vector2 m_distance = (p_target - p_start).normalized;

        // If there is no distance between start and target position, return empty move.
        if (m_distance.sqrMagnitude == 0) return m_nextMove;

        // Rounds up normalized distance vector to return one of the 8 directions.
        m_nextMove.x += Mathf.RoundToInt(m_distance.x);
        m_nextMove.y += Mathf.RoundToInt(m_distance.y);

        // Returns move.
        return m_nextMove;
    }
}
