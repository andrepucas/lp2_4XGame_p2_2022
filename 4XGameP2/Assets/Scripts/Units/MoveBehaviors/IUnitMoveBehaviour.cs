using UnityEngine;

/// <summary>
/// Interface that represents a unit's moving behaviour.
/// </summary>
public interface IUnitMoveBehaviour
{
    /// <summary>
    /// Returns next Vector2 move from given position towards target position.
    /// </summary>
    /// <param name="p_start">Start Vector2 position.</param>
    /// <param name="p_target">Target Vector2 position.</param>
    /// <returns>Next Vector2 Move.</returns>
    Vector2 GetNextMove(Vector2 p_start, Vector2 p_target);
}
