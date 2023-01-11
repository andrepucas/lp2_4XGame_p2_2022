using System;
using UnityEngine;

/// <summary>
/// Specific unit movement behaviour, Von Neumann.
/// Makes it so the next move can only go in one of the 4 2D main directions:
/// up, down, left or right.
/// </summary>
public class VonNeumannMoveBehaviour : MonoBehaviour, IUnitMoveBehaviour
{
    /// <summary>
    /// Returns next move towards the target position. 
    /// Can only return up, down, left or right vectors.
    /// </summary>
    /// <param name="p_start">Start position.</param>
    /// <param name="p_target">Target position.</param>
    /// <returns>Next position.</returns>
    public Vector2 GetNextMove(Vector2 p_start, Vector2 p_target)
    {
        // Sets next move base as start position.
        Vector2 m_nextMove = p_start;

        // Calculates distance vector to target.
        Vector2 m_distance = p_target - p_start;

        // If there is no distance between start and target position, return empty move.
        if (m_distance.sqrMagnitude == 0) return m_nextMove;

        // If the path in the x axis is longer than the path in the y axis.
        if (Mathf.Abs(m_distance.x) > Mathf.Abs(m_distance.y))
        {
            // If this distance is positive, return move right (1, 0).
            if (m_distance.x >= 0) 
                m_nextMove += Vector2.right;

            // If it's negative, return move left (-1, 0).
            else 
                m_nextMove += Vector2.left;
        }

        // If the path in the y axis is longer
        else
        {
            // If this distance is positive, return move up (0, 1).
            if (m_distance.y >= 0) 
                m_nextMove += Vector2.up;

            // If it's negative, return move down (0, -1).
            else 
                m_nextMove += Vector2.down;
        }

        // Returns move.
        return m_nextMove;
    }
}
