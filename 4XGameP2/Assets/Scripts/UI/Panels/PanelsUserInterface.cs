using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Implements UI in the form of Unity panels.
/// </summary>
public class PanelsUserInterface : MonoBehaviour, IUserInterface
{
    // Serialized variables.
    [Header("ON START")]
    [Tooltip("Time (s) for the background to be revealed.")]
    [SerializeField][Range(0, 3)] private float _bgRevealTime;
    [Tooltip("Background image reference.")]
    [SerializeField] private Image _background;
    [Header("PANELS")]
    [Tooltip("Time (s) for panel in and out fades.")]
    [SerializeField][Range(0, 1)] private float _panelTransitionTime;
    [Tooltip("Pre start panel.")]
    [SerializeField] private UIPanelPreStart _preStart;
    [Tooltip("Map browser panel.")]
    [SerializeField] private UIPanelMapBrowser _mapBrowser;
    [Tooltip("Gameplay panel.")]
    [SerializeField] private UIPanelGameplay _gameplay;
    [Tooltip("Map display panel.")]
    [SerializeField] private UIPanelMapDisplay _mapDisplay;
    [Tooltip("Inspector panel.")]
    [SerializeField] private UIPanelInspector _inspector;

    // Color that holds the background image color.
    private Color _bgColor;

    /// <summary>
    /// Saves the original background color and hides it.
    /// </summary>
    public void Initialize()
    {
        // Saves background color.
        _bgColor = _background.color;

        // Sets its alpha to 0.
        _bgColor.a = 0;

        // Updates background color.
        _background.color = _bgColor;
    }

    /// <summary>
    /// Handles UI state changes.
    /// </summary>
    /// <param name="p_uiState">New UI state.</param>
    public void ChangeUIState(UIStates p_uiState)
    {
        // Checks current UI state.
        switch(p_uiState)
        {
            case UIStates.PRE_START:

                // Sets up all serialized panels.
                SetupAllPanels();

                // Starts coroutine of initial reveal.
                StartCoroutine(StartDelayAndReveal());

                break;

            case UIStates.MAP_BROWSER:

                // Closes Pre Start panel and opens Map Browser panel.
                _preStart.ClosePanel();
                _mapBrowser.OpenPanel(_panelTransitionTime);

                break;

            case UIStates.LOAD_MAP:

                // Closes Map Browser panel.
                _mapBrowser.ClosePanel();

                break;

            case UIStates.DISPLAY_MAP:

                // Opens Gameplay and Map Display panels.
                _gameplay.OpenPanel(_panelTransitionTime);
                _mapDisplay.OpenPanel(_panelTransitionTime);

                break;

            case UIStates.INSPECTOR:

                // Closes Gameplay panel and opens Inspector panel.
                _gameplay.ClosePanel();
                _inspector.OpenPanel();

                break;

            case UIStates.RESUME_FROM_INSPECTOR:

                // Closes Inspector panel and opens Gameplay panel.
                _inspector.ClosePanel(_panelTransitionTime);
                _gameplay.OpenPanel(_panelTransitionTime);

                break;

        }
    }

    /// <summary>
    /// Sets up all serialized panels.
    /// </summary>
    private void SetupAllPanels()
    {
        _preStart.SetupPanel();
        _mapBrowser.SetupPanel();
        _gameplay.SetupPanel();
        _mapDisplay.SetupPanel();
        _inspector.SetupPanel();
    }

    /// <summary>
    /// After revealing the background, reveals the Pre Start panel.
    /// </summary>
    /// <returns>null while the background is being revealed.</returns>
    private IEnumerator StartDelayAndReveal()
    {
        // Sets up elapsed time for lerp.
        float m_elapsedTime = 0;

        // While the background isn't fully revealed.
        while (_background.color.a < 1)
        {
            // Lerp the background color alpha value from 0 to 1.
            _bgColor.a = Mathf.Lerp(0, 1, (m_elapsedTime/_bgRevealTime));
            _background.color = _bgColor;

            // Updates elapsed time based on current time and start time.
            m_elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Reveals Pre Start panel, with the same timing as the background reveal.
        _preStart.OpenPanel(_panelTransitionTime);
    }
}
