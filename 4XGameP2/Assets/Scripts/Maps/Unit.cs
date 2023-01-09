using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, 
    IPointerExitHandler
{
    public static event Action<Unit> OnClick;
    public static event Action<Unit> OnEnter;
    public static event Action<Unit> OnExit;

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
    [SerializeField] private Image _frontImg;
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
    /// Self implemented property that stores the name of the unit.
    /// </summary>
    /// <value>Name of the unit (type).</value>
    public string Name {get; private set;}

    /// <summary>
    /// Self implemented property that stores the unit's icon sprite.
    /// </summary>
    /// <value></value>
    public Sprite Icon {get; private set;}

    /// <summary>
    /// Public self implemented property that stores the unit's relative map position.
    /// </summary>
    /// <value>Relative map position. Ex: (0, 1).</value>
    public Vector2 MapPosition {get; set;}

    /// <summary>
    /// Read only self implemented property that stores all unit's resources.
    /// </summary>
    /// <value>Current resources of the unit.</value>
    public IReadOnlyList<Resource> Resources => _resourceList;

    // Private list of Resources.
    private List<Resource> _resourceList;

    // Private arrays of resource's names to collect and generate.
    private string[] _resourceNamesToCollect, _resourceNamesToGenerate;

    // Private Vector2 that handles the rectTransform's size modifications.
    private Vector2 _rectSize;

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
    public void Initialize(PresetUnitsData p_unitData, Vector2 p_mapPos,
        Vector3 p_worldPos, float p_cellSize)
    {
        // Sets unit name.
        Name = p_unitData.Name;

        _resourceList = new List<Resource>();

        // Sets unit sprites.
        _baseImg.sprite = p_unitData.BaseIcon;
        _frontImg.sprite = p_unitData.FrontIcon;

        Icon = _frontImg.sprite;
        _frontImg.color = Color.clear;

        // Saves names of resources this unit should collect & generate.
        _resourceNamesToCollect = p_unitData.ResourceNamesToCollect;
        _resourceNamesToGenerate = p_unitData.ResourceNamesToGenerate;

        // Saves relative map position.
        MapPosition = p_mapPos;

        // Positions unit on top of a map cell, with a slight offset.
        Vector3 m_position = p_worldPos;
        m_position.y += (p_cellSize * DISPLAY_OFFSET_RATIO);
        transform.position = m_position;

        // Updates scale to remain consistent.
        _rectSize = _rectTransform.sizeDelta;
        UpdateScale(Camera.main.orthographicSize);

        // Deselects unit.
        OnDeselect();
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
    /// Called by Unit Selection.
    /// </remarks>
    public void OnHover()
    {
        // Stops (just in case) and starts the Color Fade coroutine.
        StopCoroutine(ColorFadeTo(Color.clear));
        StartCoroutine(ColorFadeTo(_hoveredColor));
    }

    /// <summary>
    /// Selects this unit.
    /// </summary>
    /// <remarks>
    /// Called by Unit Selection.
    /// </remarks>
    public void OnSelect()
    {
        _selectedRing.SetActive(true);
        StopCoroutine(ColorFadeTo(Color.clear));
        StartCoroutine(ColorFadeTo(_selectedColor));
    }

    /// <summary>
    /// De-selects this unit.
    /// </summary>
    /// <remarks>
    /// Called by Unit Selection.
    /// </remarks>
    public void OnDeselect()
    {
        _selectedRing.SetActive(false);
        StopCoroutine(ColorFadeTo(Color.clear));
        StartCoroutine(ColorFadeTo(_normalColor));
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
        Color m_startColor = _frontImg.color;

        // Cycles for the duration of the color fade time.
        float m_elapsedTime = 0;
        while (m_elapsedTime < _colorFadeTime)
        {
            // Lerps icon color.
            _frontImg.color = Color.Lerp(m_startColor, p_targetColor,
                (m_elapsedTime / _colorFadeTime));

            m_elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Explicitly sets icon color to target color after fade time.
        // (Prevents color lerp inaccuracies).
        _frontImg.color = p_targetColor;
    }

    /// <summary>
    /// Raises event that unit has been LEFT clicked.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell is clicked, by IPointerClickHandler.
    /// </remarks>
    public void OnPointerClick(PointerEventData p_pointerData)
    {
        if (p_pointerData.button == PointerEventData.InputButton.Left)
            OnClick?.Invoke(this);
    }

    /// <summary>
    /// Raises event that unit is being hovered.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell is hovered, by IPointerEnterHandler.
    /// </remarks>
    public void OnPointerEnter(PointerEventData p_pointerData) =>
        OnEnter?.Invoke(this);

    /// <summary>
    /// Raises event that unit is no longer being hovered.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell stops being hovered, by IPointerExitHandler.
    /// </remarks>
    public void OnPointerExit(PointerEventData p_pointerData) =>
        OnExit?.Invoke(this);
}
