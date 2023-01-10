using UnityEngine;

/// <summary>
/// Displays a target icon on top of a map cell, where units are moving to.
/// </summary>
public class UnitTarget : MonoBehaviour
{
    // Serialized variables.
    [SerializeField] private RectTransform _rectTransform;

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
    private void OnEnable() => MapDisplay.OnCamZoom += UpdateScale;

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable() => MapDisplay.OnCamZoom -= UpdateScale;

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
}
