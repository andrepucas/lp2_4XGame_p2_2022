using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles Units drag selection with input from controller.
/// </summary>
public class UnitSelection : MonoBehaviour
{
    public static event Action<ICollection<Unit>> OnUnitsSelected;

    // Serialized variables.
    [Tooltip("Manipulable rect transform component.")]
    [SerializeField] private RectTransform _selectionBox;

    // Private collection containing all spawned units.
    private IList<Unit> _unitsInGame;

    // Private collection containing all hovered units.
    private ISet<Unit> _unitsHovered;

    // Private collection containing all selected units.
    private ISet<Unit> _unitsSelected;

    // Camera reference.
    private Camera _cam;

    // Reference to where the mouse position started dragging.
    private Vector2 _startMousePos;

    // References to calculate the selection box size.
    private float _width, _height;

    // Reference to selection box bounds.
    private Bounds _bounds;

    // Vector2 used for temporary, every frame, calculations.
    private Vector2 _auxV2;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelGameplay.OnUnitAdded += (p_unit) => { _unitsInGame.Add(p_unit); };
        UIPanelUnitsControl.OnUnitsRemoved += RemoveUnits;
        Unit.OnEnter += Hover;
        Unit.OnExit += StopHover;
        Unit.OnClick += ClickSelect;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UIPanelGameplay.OnUnitAdded -= (p_unit) => { };
        UIPanelUnitsControl.OnUnitsRemoved -= RemoveUnits;
        Unit.OnEnter -= Hover;
        Unit.OnExit -= StopHover;
        Unit.OnClick -= ClickSelect;
    }

    /// <summary>
    /// Called by controller whenever a new map is generated.
    /// Resets variables.
    /// </summary>
    public void Reset()
    {
        _unitsInGame = new List<Unit>();
        _unitsHovered = new HashSet<Unit>();
        _unitsSelected = new HashSet<Unit>();

        _cam = Camera.main;
        _auxV2 = new Vector2();
    }

    /// <summary>
    /// Resets selection box and saves starting point.
    /// </summary>
    public void StartSelectionBox()
    {
        _selectionBox.sizeDelta = Vector2.zero;
        _selectionBox.gameObject.SetActive(true);
        _startMousePos = Input.mousePosition;
    }

    /// <summary>
    /// Resizes selection box.
    /// Hovers units within it and deselects units outside it.
    /// </summary>
    public void ResizeSelectionBox()
    {
        // Sets width and height based on mouse position, relative to it's start.
        _width = Input.mousePosition.x - _startMousePos.x;
        _height = Input.mousePosition.y - _startMousePos.y;

        // Sets selection box's anchored position (centered).
        _auxV2.x = _width / 2;
        _auxV2.y = _height / 2;
        _selectionBox.anchoredPosition = _startMousePos + _auxV2;

        // Sets selection box's size delta.
        _auxV2.x = Mathf.Abs(_width);
        _auxV2.y = Mathf.Abs(_height);
        _selectionBox.sizeDelta = _auxV2;

        // Creates current selection box's bounds.
        _bounds = new Bounds(_selectionBox.anchoredPosition, _selectionBox.sizeDelta);

        // Iterates all spawned units.
        for (int i = 0; i < _unitsInGame.Count; i++)
        {
            // If this units is within the selection box's bounds.
            if (UnitIsInSelectionBox(_unitsInGame[i].transform.position,
                _unitsInGame[i].SelectableRadius, _bounds))
            {
                // If it's not already hovered, hover it.
                if (!_unitsHovered.Contains(_unitsInGame[i]))
                    Hover(_unitsInGame[i]);
            }

            // If it's outside and is selected, deselect it.
            else if (_unitsSelected.Contains(_unitsInGame[i]))
                Deselect(_unitsInGame[i]);

            // If it's outside and is hovered, stop hovering it.
            else if (_unitsHovered.Contains(_unitsInGame[i]))
                StopHover(_unitsInGame[i]);
        }
    }

    /// <summary>
    /// Verifies if either the center or edges of the unit are within the
    /// selection box's bounds.
    /// </summary>
    /// <param name="p_centerPos">Center position of the unit.</param>
    /// <param name="p_radius">Radius value of the unit.</param>
    /// <param name="p_bounds">Selection box bounds.</param>
    /// <returns>True, if there are units inside the selection box.</returns>
    private bool UnitIsInSelectionBox(Vector3 p_centerPos, float p_radius, Bounds p_bounds)
    {
        // Saves center and edge positions.
        Vector2[] m_pos = new Vector2[]
        {
            _cam.WorldToScreenPoint(p_centerPos),
            _cam.WorldToScreenPoint(p_centerPos + new Vector3(p_radius, p_radius)),
            _cam.WorldToScreenPoint(p_centerPos + new Vector3(-p_radius, p_radius)),
            _cam.WorldToScreenPoint(p_centerPos + new Vector3(p_radius, -p_radius)),
            _cam.WorldToScreenPoint(p_centerPos + new Vector3(-p_radius, -p_radius)),
        };

        // If at least one of these positions is within bounds, return true.
        for (int i = 0; i < m_pos.Length; i++)
        {
            if (m_pos[i].x > p_bounds.min.x && m_pos[i].x < p_bounds.max.x &&
            m_pos[i].y > p_bounds.min.y && m_pos[i].y < p_bounds.max.y)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Disables selection box.
    /// Selects all units currently hovered.
    /// </summary>
    public void EndSelectionBox()
    {
        // Sets the size of the selection box to 0 and disables the game object.
        _selectionBox.sizeDelta = Vector2.zero;
        _selectionBox.gameObject.SetActive(false);

        // Selects hovered units.
        foreach (Unit f_unit in _unitsHovered)
            Select(f_unit);

        // Clears hovered units list.
        _unitsHovered.Clear();
    }

    /// <summary>
    /// Adds / Removes clicked selection from selected units when either control
    /// key is pressed or only selects clicked select if neither controls are
    /// pressed.
    /// </summary>
    /// <param name="p_unit">Clicked unit.</param>
    private void ClickSelect(Unit p_unit)
    {
        // Disables hovering over the unit.
        StopHover(p_unit);

        // If left control or right control are also pressed.
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            // If this unit isn't yet selected, selects it.
            if (!_unitsSelected.Contains(p_unit)) Select(p_unit);

            // If it is, deselects it.
            else Deselect(p_unit);
        }

        // If neither control keys are being held.
        else
        {
            // Only select this unit, deselects all others.
            DeselectAll();
            Select(p_unit);
        }
    }

    /// <summary>
    /// Adds to hover list and manually hovers unit.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    private void Hover(Unit p_unit)
    {
        _unitsHovered.Add(p_unit);
        p_unit.OnHover();
    }

    /// <summary>
    /// Removes from hover list and manually stops hovering unit.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    private void StopHover(Unit p_unit)
    {
        _unitsHovered.Remove(p_unit);

        if (_unitsSelected.Contains(p_unit)) p_unit.OnSelect();
        else p_unit.OnDeselect();
    }

    /// <summary>
    /// Adds to selected list and manually selects unit.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    private void Select(Unit p_unit)
    {
        _unitsSelected.Add(p_unit);
        p_unit.OnSelect();

        // Raise units selected event.
        OnUnitsSelected?.Invoke(_unitsSelected);
    }

    /// <summary>
    /// Removes from selected and hovered lists and manually deselects unit.
    /// </summary>
    /// <param name="p_unit">Unit.</param>
    private void Deselect(Unit p_unit)
    {
        _unitsSelected.Remove(p_unit);
        p_unit.OnDeselect();

        // Raise units selected event.
        OnUnitsSelected?.Invoke(_unitsSelected);
    }

    /// <summary>
    /// Manually deselects all units and clears selected and hovered lists.
    /// </summary>
    public void DeselectAll()
    {
        IList<Unit> _auxList = new List<Unit>(_unitsSelected);

        foreach (Unit f_unit in _auxList)
            Deselect(f_unit);

        _unitsSelected.Clear();
        _unitsHovered.Clear();
    }

    /// <summary>
    /// Removes units from collections.
    /// </summary>
    /// <param name="p_unitsToRemove">Units to remove.</param>
    private void RemoveUnits(ICollection<Unit> p_unitsToRemove)
    {
        foreach (Unit f_unit in p_unitsToRemove)
        {
            _unitsInGame.Remove(f_unit);
            _unitsHovered.Remove(f_unit);
            _unitsSelected.Remove(f_unit);
        }

        OnUnitsSelected?.Invoke(_unitsSelected);
    }
}
