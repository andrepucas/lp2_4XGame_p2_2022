/// <summary>
/// Generic UI interface. 
/// </summary>
public interface IUserInterface
{
    /// <summary>
    /// Initializes the UI with whatever it needs, called on controller Awake.
    /// </summary>
    public void Initialize();

    /// <summary>
    /// Handles UI state changes.
    /// </summary>
    /// <param name="p_uiState">New UI state.</param>
    public void ChangeUIState(UIStates p_uiState);
}
