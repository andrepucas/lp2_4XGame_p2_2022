using UnityEngine;

/// <summary>
/// Specific unit movement behaviour, Moore.
/// Makes it so the next move can go in 8 2D directions: 
/// up, down, left or right + diagonals.
/// </summary>
public class MooreMoveBehaviour : MonoBehaviour, IUnitMoveBehaviour
{
    public Vector2 GetNextMove(Vector2 p_start, Vector2 p_target)
    {
        return p_start - Vector2.one;
    }
}
