using System.Collections;
using UnityEngine;

/// <summary>
/// Displays a target icon on top of a map cell, where units are moving to.
/// </summary>
public class UnitTarget : MonoBehaviour
{
    // Serialized variables.
    [Header("COMPONENTS")]
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Header("PARAMETERS")]
    [Tooltip("Time in seconds it takes this target to fade away.")]
    [SerializeField] private float _fadeOutTime;

    // Private Vector2 to easily replace size delta of rect transform.
    private Vector2 _rectSize;

    // Private float value that holds the ratio size of this unit target.
    private float _displaySizeRatio;

    /// <summary>
    /// Sets up target after being instantiated, like a constructor.
    /// </summary>
    public void Initialize(float p_sizeRatio)
    {
        // Updates scale to remain consistent.
        _displaySizeRatio = p_sizeRatio;
        _rectSize = _rectTransform.sizeDelta;
        UpdateScale(Camera.main.orthographicSize);
    }

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        MapDisplay.OnCamZoom += UpdateScale;
        UIPanelUnitsControl.OnMoving += FadeOutWhenReached;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        MapDisplay.OnCamZoom -= UpdateScale;
        UIPanelUnitsControl.OnMoving -= FadeOutWhenReached;
    }

    /// <summary>
    /// Updates scale when camera zooms, so that the icon always remains the
    /// same size.
    /// </summary>
    /// <param name="p_camZoom">Camera's orthographic size value.</param>
    private void UpdateScale(float p_camZoom)
    {
        // Calculates size based on camera zoom and preset size ratio.
        _rectSize.x = p_camZoom * _displaySizeRatio;

        // Equals width and height and sets it as the new rect transform's size.
        _rectSize.y = _rectSize.x;
        _rectTransform.sizeDelta = _rectSize;
    }

    /// <summary>
    /// Destroy this game object when a unit reached it (Units stop moving).
    /// </summary>
    /// <param name="p_moving">Units moving status.</param>
    private void FadeOutWhenReached(bool p_moving)
    {
        if (!p_moving) StartCoroutine(FadingOut());
    }

    /// <summary>
    /// Coroutine that fades out this target, before destroying it.
    /// </summary>
    /// <returns>Null.</returns>
    private IEnumerator FadingOut()
    {
        float m_elapsedTime = 0;

        // While the canvas isn't fully hidden.
        while (_canvasGroup.alpha > 0)
        {
            // Lerps the canvas alpha value from 1 to 0.
            _canvasGroup.alpha = Mathf.Lerp(1, 0, (m_elapsedTime/_fadeOutTime));

            // Updates elapsed time based on unscaled delta time.
            m_elapsedTime += Time.unscaledDeltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
}
