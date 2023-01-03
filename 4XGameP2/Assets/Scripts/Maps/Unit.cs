using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    /// <summary>
    /// Constant float ratio value of display's size.
    /// </summary>
    private const float DISPLAY_SIZE_RATIO = 0.2f;

    /// <summary>
    /// Constant float ratio value of display's offset.
    /// </summary>
    private const float DISPLAY_OFFSET_RATIO = -0.25f;

    // Serialized
    [Header("COMPONENTS")]
    [Tooltip("Rect Transform component.")]
    [SerializeField] private RectTransform _rectTransform;
    [Tooltip("Base Image component.")]
    [SerializeField] private Image _baseImg;
    [Tooltip("Icon Image component.")]
    [SerializeField] private Image _iconImg;
    [Tooltip("Selected Ring Game Object component.")]
    [SerializeField] private GameObject _selectedRing;
    [Header("COLORS")]
    [Tooltip("Unit color normally.")]
    [SerializeField] private Color _normalColor;
    [Tooltip("Unit color when hovered.")]
    [SerializeField] private Color _hoveredColor;
    [Tooltip("Unit color when selected.")]
    [SerializeField] private Color _selectedColor;
    [Tooltip("Seconds to transition in-between colors")]
    [SerializeField] private float _colorFadeTime;

    /// <summary>
    /// Read only self implemented property that stores the name of the unit.
    /// </summary>
    /// <value>Name of the unit (type).</value>
    public string Name { get; }

    /// <summary>
    /// Read only self implemented property that stores all the current resources
    /// of this unit.
    /// </summary>
    /// <value>Current resources of the unit.</value>
    public IReadOnlyList<Resource> Resources => _resourceList;

    // Private list of Resources.
    private List<Resource> _resourceList;

    // Private reference to the relative map position. Ex: (0,1).
    private Vector2 _mapPos;

    // Private Vector2 that handles the rectTransform's size modifications.
    private Vector2 _rectSize;

    // Private status to control if this unit is selected or not.
    private bool _isSelected;

    /// <summary>
    /// Constructor method. 
    /// Sets properties' values and initializes resources list.
    /// </summary>
    /// <param name="p_name">Name.</param>
    public Unit(string p_name)
    {
        Name = p_name;

        // Initializes list.
        _resourceList = new List<Resource>();
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
    /// Sets up unit after being instantiated, like a constructor.
    /// </summary>
    /// <param name="p_mapPos">Relative position (map).</param>
    /// <param name="p_worldPos">Absolute position (world).</param>
    /// <param name="p_cellSize">Size of map cells.</param>
    public void Initialize(Vector2 p_mapPos, Vector3 p_worldPos, float p_cellSize)
    {
        // Saves relative map position.
        _mapPos = p_mapPos;

        // Positions unit on top of a map cell, with a slight offset.
        Vector3 m_position = p_worldPos;
        m_position.y += (p_cellSize * DISPLAY_OFFSET_RATIO);
        transform.position = m_position;

        // Updates scale to remain consistent.
        _rectSize = _rectTransform.sizeDelta;
        UpdateScale(Camera.main.orthographicSize);

        // Updates sprite.
        _iconImg.color = Color.clear;
        OnPointerExit();

        // Deselects unit.
        _isSelected = false;
        _selectedRing.SetActive(false);
    }

    /// <summary>
    /// Updates scale when camera zooms, so that the icon always remains the
    /// same size.
    /// </summary>
    /// <param name="p_camZoom">Camera's orthographic size value.</param>
    private void UpdateScale(float p_camZoom)
    {
        // Calculates size based on camera zoom and preset size ratio.
        _rectSize.x = p_camZoom * DISPLAY_SIZE_RATIO;

        // Equals width and height and sets it as the new rect transform's size.
        _rectSize.y = _rectSize.x;
        _rectTransform.sizeDelta = _rectSize;
    }

    /// <summary>
    /// Fades the icon color to hovered.
    /// </summary>
    /// <remarks>
    /// Called when the unit is hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerEnter()
    {
        // Stops (just in case) and starts the Color Fade coroutine.
        StopCoroutine(ColorFadeTo(Color.clear));
        StartCoroutine(ColorFadeTo(_hoveredColor));
    }

    /// <summary>
    /// Fades the icon color to normal or selected, depending on state.
    /// </summary>
    /// <remarks>
    /// Called when the unit stop's being hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerExit()
    {
        // Stops coroutine just in case.
        StopCoroutine(ColorFadeTo(Color.clear));

        // Starts coroutine depending on unit's selected state.
        if (_isSelected) StartCoroutine(ColorFadeTo(_selectedColor));
        else StartCoroutine(ColorFadeTo(_normalColor));
    }

    /// <summary>
    /// Selects / Deselects this unit.
    /// </summary>
    /// <remarks>
    /// Called when the unit is clicked, by a Unity event trigger.
    /// </remarks>
    public void OnClick()
    {
        // Enables or disable the selected animated ring around the unit.
        if (!_isSelected) _selectedRing.SetActive(true);
        else _selectedRing.SetActive(false);

        // Inverts selected status.
        _isSelected = !_isSelected;
    }

    /// <summary>
    /// Fades the current icon color to the received target color using the
    /// serialized fade time (s).
    /// </summary>
    /// <param name="p_targetColor">Target icon color.</param>
    /// <returns>Null.</returns>
    private IEnumerator ColorFadeTo(Color p_targetColor)
    {
        // Saves current icon color.
        Color m_startColor = _iconImg.color;

        // Cycles for the duration of the color fade time.
        float m_elapsedTime = 0;
        while(m_elapsedTime < _colorFadeTime)
        {
            // Lerps icon color.
            _iconImg.color = Color.Lerp(m_startColor, p_targetColor, 
                (m_elapsedTime/_colorFadeTime));

            m_elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Explicitly sets icon color to target color after fade time.
        // (Prevents color lerp inaccuracies).
        _iconImg.color = p_targetColor;
    }

    public void MoveTowardsTile(GameTile selectedTile)
    {

    }

    public void HarvestCurrentTile(GameTile currentTile)
    {

    }
}
