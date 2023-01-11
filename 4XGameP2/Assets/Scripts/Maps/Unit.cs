using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Contains info about each Map Unit.
/// </summary>
public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler,
    IPointerExitHandler
{
    /// <summary>
    /// Event raised when this unit is clicked.
    /// </summary>
    public static event Action<Unit> OnClick;

    /// <summary>
    /// Event raised when this unit is hovered.
    /// </summary>
    public static event Action<Unit> OnEnter;

    /// <summary>
    /// Event raised when this unit stops being hovered.
    /// </summary>
    public static event Action<Unit> OnExit;

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
    public string Name { get; private set; }

    /// <summary>
    /// Self implemented property that stores the unit's icon sprite.
    /// </summary>
    /// <value></value>
    public Sprite Icon { get; private set; }

    /// <summary>
    /// Self implemented property that stores the unit's relative map position.
    /// </summary>
    /// <value>Relative map position. Ex: (0, 1).</value>
    public Vector2 MapPosition { get; private set; }

    /// <summary>
    /// Readonly self implemented property that returns this unit's selectable radius.
    /// </summary>
    public float SelectableRadius => (_rectSize.x / 2) * 0.8f;

    /// <summary>
    /// Read only self implemented property that stores all unit's resources.
    /// </summary>S
    /// <value>Current resources of the unit.</value>
    public IReadOnlyList<Resource> Resources => _resourceList;

    // Private list of Resources.
    private List<Resource> _resourceList;

    // Private arrays of resource's names to collect and generate.
    public IReadOnlyList<string> ResourceNamesToCollect { get; private set; }
    public IReadOnlyList<string> ResourceNamesToGenerate { get; private set; }

    // Private Vector2 that handles the rectTransform's size modifications.
    private Vector2 _rectSize;

    // Private float value that holds the ratio size of this unit.
    private float _displaySizeRatio;

    // Private float value that holds cell to cell movement duration of this unit.
    private float _moveTime;

    // Control variables so that unit can't be selected when moving.
    private bool _moveSelecting, _moving;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        MapDisplay.OnCamZoom += UpdateScale;
        UIPanelUnitsControl.OnMoveSelect += (p_selecting) => _moveSelecting = p_selecting;
        UIPanelUnitsControl.OnMoving += (p_moving) => { _moving = p_moving; };
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        MapDisplay.OnCamZoom -= UpdateScale;
        UIPanelUnitsControl.OnMoveSelect -= (p_selecting) => { };
        UIPanelUnitsControl.OnMoving -= (p_moving) => { };
    }

    /// <summary>
    /// Sets up unit after being instantiated, like a constructor.
    /// </summary>
    /// <param name="p_unitData">Data relative to this unit.</param>
    /// <param name="p_mapPos">Relative position (map).</param>
    /// <param name="p_sizeRatio">Ratio size of this unit at all times.</param>
    public void Initialize(PresetUnitsData p_unitData, Vector2 p_mapPos,
        float p_sizeRatio, float p_moveTime)
    {
        // Sets unit name and move time.
        Name = p_unitData.Name;
        _moveTime = p_moveTime;

        _resourceList = new List<Resource>();

        // Sets unit sprites.
        _baseImg.sprite = p_unitData.BaseIcon;
        _frontImg.sprite = p_unitData.FrontIcon;

        Icon = _frontImg.sprite;
        _frontImg.color = Color.clear;

        // Saves names of resources this unit should collect & generate.
        ResourceNamesToCollect = p_unitData.ResourceNamesToCollect;
        ResourceNamesToGenerate = p_unitData.ResourceNamesToGenerate;

        // Saves relative map position.
        MapPosition = p_mapPos;

        // Updates scale to remain consistent.
        _displaySizeRatio = p_sizeRatio;
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
        _rectSize.x = p_camZoom * _displaySizeRatio;

        // Equals width and height and sets it as the new rect transform's size.
        _rectSize.y = _rectSize.x;
        _rectTransform.sizeDelta = _rectSize;
    }

    /// <summary>
    /// Adds a resource to this unit's private resource list.
    /// </summary>
    /// <param name="resource">Resource to add.</param>
    public void AddResource(Resource resource) => _resourceList.Add(resource);

    public Vector2 GetNextMoveTowards(Vector2 p_mapPos)
    {
        return MapPosition + Vector2.one;
    }
    
    public void MoveTo(Vector2 p_mapPos, Vector3 p_worldPos)
    {
        MapPosition = p_mapPos;

        StartCoroutine(MovingTo(p_worldPos));
    }

    private IEnumerator MovingTo(Vector3 p_worldPos)
    {
        float m_elapsedTime = 0;
        Vector3 m_startPos = transform.position;

        while (transform.position != p_worldPos)
        {
            transform.position = Vector3.Lerp(m_startPos, p_worldPos, (m_elapsedTime/_moveTime));

            m_elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        Debug.Log(Name.ToUpper() + " MOVED TO " + transform.position);
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
        // Ignores method if units are preparing to or moving.
        if (_moveSelecting || _moving) return;

        // If clicked with the left mouse button.
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
    public void OnPointerEnter(PointerEventData p_pointerData)
    {
        // Ignores method if units are preparing to or moving.
        if (_moveSelecting || _moving) return;

        OnEnter?.Invoke(this);
    }

    /// <summary>
    /// Raises event that unit is no longer being hovered.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell stops being hovered, by IPointerExitHandler.
    /// </remarks>
    public void OnPointerExit(PointerEventData p_pointerData)
    {
        // Ignores method if units are preparing to or moving.
        if (_moveSelecting || _moving) return;

        OnExit?.Invoke(this);
    }
}
