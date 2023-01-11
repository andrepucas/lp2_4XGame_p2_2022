using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Panel displayed on Pre Start UI state.
/// </summary>
public class UIPanelPreStart : UIPanel
{
    /// <summary>
    /// Event raised when the input prompt is revealed.
    /// </summary>
    public static event Action OnPromptRevealed;

    // Serialized variables.
    [Header("INPUT PROMPT")]
    [Tooltip("\"Press any key\" prompt.")]
    [SerializeField] private TMP_Text _prompt;
    [Tooltip("Prompt fade in time (s).")]
    [SerializeField][Range(0, 1)] private float _fadeTime;

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        // Closes panel.
        ClosePanel();

        // Sets the prompt alpha value to 0.
        _prompt.alpha = 0;
    }

    /// <summary>
    /// Reveals panel and prompt.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0)
    {
        // Reveals the panel.
        base.Open(p_transitionTime);

        // Start coroutine to reveal the prompt.
        StartCoroutine(PromptFadeIn(p_transitionTime));
    }

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0) => 
        base.Close(p_transitionTime);

    /// <summary>
    /// Reveals the prompt after the panel is fully revealed.
    /// </summary>
    /// <param name="p_revealTime">Panel reveal time (s).</param>
    /// <returns> real time (s) for the panel to be revealed AND 
    /// null while the prompt isn't fully revealed.</returns>
    private IEnumerator PromptFadeIn(float p_revealTime)
    {
        // Sets elapsed time to 0.
        float m_elapsedTime = 0;

        // Waits for the panel to be revealed.
        yield return new WaitForSecondsRealtime(p_revealTime);

        // Raises event that the prompt is starting to be revealed.
        OnPromptRevealed?.Invoke();

        // While the prompt isn't fully revealed.
        while (_prompt.alpha < 1)
        {
            // Lerps the prompt's alpha value from 0 to 1.
            _prompt.alpha = Mathf.Lerp(0, 1, (m_elapsedTime/_fadeTime));

            // Updates elapsed time based on unscaled delta time.
            m_elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        // Fully reveals the prompt.
        _prompt.alpha = 1;
    }
}
