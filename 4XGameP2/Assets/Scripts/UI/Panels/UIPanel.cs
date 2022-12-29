using UnityEngine;
using System.Collections;

/// <summary>
/// Handles opening and closing panel behaviours.
/// </summary>
public abstract class UIPanel : MonoBehaviour
{
    // Serialized variables.
    [Tooltip("Canvas group in this game object.")]
    [SerializeField] private CanvasGroup _canvasGroup;

    /// <summary>
    /// Enables and reveals panel.
    /// </summary>
    /// <param name="p_transition">Reveal time (s).</param>
    protected void Open(float p_transition)
    {
        // Enables panel game object.
        //gameObject.SetActive(true);

        // If transition time is 0.
        if (p_transition == 0)
        {
            // Directly reveals and enables everything in canvas group.
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        // Otherwise, reveal it over time.
        else StartCoroutine(RevealOverTime(p_transition));
    }

    /// <summary>
    /// Hides and disables panel.
    /// </summary>
    /// <param name="p_transition">Hiding time (s).</param>
    protected void Close(float p_transition)
    {
        // Stops blocking raycasts right away.
        _canvasGroup.blocksRaycasts = false;

        // If transition time is 0.
        if (p_transition == 0)
        {
            // Directly hides and disables everything in canvas group, 
            // including the game object.
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            //gameObject.SetActive(false);
        }

        // Otherwise, hides and disables over time.
        else StartCoroutine(HideOverTime(p_transition));
    }

    /// <summary>
    /// Reveals panel over specified time.
    /// </summary>
    /// <param name="p_transition">Specified time (s).</param>
    /// <returns>null while canvas isn't fully revealed.</returns>
    private IEnumerator RevealOverTime(float p_transition)
    {
        // Sets elapsed time to 0.
        float m_elapsedTime = 0;

        // Sets as interactable right away, to display correct child UI colors.
        _canvasGroup.interactable = true;

        // While the canvas isn't fully revealed.
        while (_canvasGroup.alpha < 1)
        {
            // Lerps the canvas alpha value from 0 to 1.
            _canvasGroup.alpha = Mathf.Lerp(0, 1, (m_elapsedTime/p_transition));

            // Updates elapsed time based on unscaled delta time.
            m_elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        // Fully reveals and enables canvas group elements.
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hides panel over specified time, and disables it.
    /// </summary>
    /// <param name="p_transition">Specified time (s).</param>
    /// <returns>null while panel isn't fully hidden.</returns>
    private IEnumerator HideOverTime(float p_transition)
    {
        // Sets elapsed time to 0.
        float m_elapsedTime = 0;

        // While the canvas isn't fully hidden.
        while (_canvasGroup.alpha > 0)
        {
            // Lerps the canvas alpha value from 1 to 0.
            _canvasGroup.alpha = Mathf.Lerp(1, 0, (m_elapsedTime/p_transition));

            // Updates elapsed time based on unscaled delta time.
            m_elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        // Fully hides and disables canvas group elements.
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;

        // Disables panel game object.
        //gameObject.SetActive(false);
    }
}
