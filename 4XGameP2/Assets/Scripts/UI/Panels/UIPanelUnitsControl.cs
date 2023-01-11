using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Panel displayed when 1+ units are selected.
/// Contains info regarding selected units and handles action buttons.
/// </summary>
public class UIPanelUnitsControl : UIPanel
{
    /// <summary>
    /// Event raised when the 'REMOVE' units button is pressed.
    /// </summary>
    public static event Action<ICollection<Unit>> OnUnitsRemoved;

    /// <summary>
    /// Event raised when the 'MOVE' units button is toggled.
    /// True = Selecting a move destination.
    /// </summary>
    public static event Action<bool> OnMoveSelect;

    /// <summary>
    /// Event raised when the selected units start/stop moving.
    /// True = Moving.
    /// </summary>
    public static event Action<bool> OnMoving;

    /// <summary>
    /// Event raised after the selected units try to harvest their tiles.
    /// </summary>
    public static event Action OnHarvest;

    /// <summary>
    /// Event raised when a turn ends.
    /// </summary>
    public static event Action OnNewTurn;

    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;
    [Header("UNIT'S TYPE/COUNT")]
    [Tooltip("Text component where the type of the single selected unit or " +
        "number of overall selected units is displayed.")]
    [SerializeField] private TMP_Text _unitTypeOrCountTxt;
    [Tooltip("Text component that follows up the one before. Displays " +
        "\"UNIT(S) SELECTED\".")]
    [SerializeField] private TMP_Text _unitOrUnitsSelectedTxt;
    [Header("UNIT ICONS")]
    [Tooltip("Parent game object where the unit icons are stored.")]
    [SerializeField] private Transform _unitIconsFolder;
    [Tooltip("Prefab of unit icon.")]
    [SerializeField] private GameObject _unitIcon;
    [Tooltip("Limit of displayable icons.")]
    [SerializeField] private int _iconsDisplayedLimit;
    [Tooltip("Prefab of unit overflow count.")]
    [SerializeField] private GameObject _unitOverflowCount;
    [Header("UNIT RESOURCES")]
    [Tooltip("Parent game object where the resource counters are stored.")]
    [SerializeField] private Transform _resourceCountFolder;
    [Tooltip("Prefab of resource counter.")]
    [SerializeField] private GameObject _resourceCount;
    [Header("BUTTONS")]
    [Tooltip("Move button.")]
    [SerializeField] private Button _moveButton;
    [Tooltip("Color for 'MOVE' button to displayed when it toggled OFF.")]
    [SerializeField] private Color _normalColor;
    [Tooltip("Color for 'MOVE' button to displayed when it toggled ON.")]
    [SerializeField] private Color _selectedColor;
    [Tooltip("Buttons to disable when units are moving.")]
    [SerializeField] private Button[] _toggleButtons;
    [Header("MOVEMENT TARGET")]
    [Tooltip("Parent game object where enemies are instantiated.")]
    [SerializeField] private Transform _enemiesFolder;
    [Tooltip("Prefab of units target destination.")]
    [SerializeField] private GameObject _targetDestinationPrefab;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Ongoing Game Data.")]
    [SerializeField] private OngoingGameDataSO _ongoingData;
    [Tooltip("Scriptable Object with Preset Game Data.")]
    [SerializeField] private PresetGameDataSO _gameData;

    // Private list containing displayed selected units.
    private List<Unit> _selectedUnits;

    // Private reference to hold block of button colors.
    private ColorBlock _colorBlock;

    // Control variables for moving units.
    private bool _isSelectingMove, _isMoving;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UnitSelection.OnUnitsSelected += DisplayUnitsData;
        MapCell.OnTargeted += (p_targetCell) => StartCoroutine(MovingUnitsTo(p_targetCell));
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UnitSelection.OnUnitsSelected -= DisplayUnitsData;
        MapCell.OnTargeted -= (p_targetCell) => { };
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        _colorBlock = _moveButton.colors;
        ClosePanel();
    }

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0)
    {
        // Reveals the panel.
        base.Open(p_transitionTime);

        // Activates opening trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", true);
    }

    /// <summary>
    /// Hides panel and destroys all the visual unit info.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0)
    {
        // Resets movement control variables.
        _isSelectingMove = false;
        _isMoving = false;

        // Activate closing trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", false);

        // Hides the panel.
        base.Close(p_transitionTime);

        // Destroys instantiated objects.
        DestroyPrefabs();

        // Toggles all buttons on and resets display of move button.
        UpdateButtons();
    }

    /// <summary>
    /// Destroys instantiated unit icons + resource count prefabs.
    /// </summary>
    private void DestroyPrefabs()
    {
        // Destroys all unit icons that might be instantiated.
        foreach (Transform unitIcon in _unitIconsFolder)
            Destroy(unitIcon.gameObject);

        // Destroy all resource counts that might be instantiated.
        foreach (Transform resourceCount in _resourceCountFolder)
            Destroy(resourceCount.gameObject);
    }

    /// <summary>
    /// Toggles selected buttons on or off, depending if units are moving.
    /// Changes color of 'MOVE' button to manually display if it's selected.
    /// </summary>
    private void UpdateButtons()
    {
        // Buttons are toggled off if selecting units move destination or moving.
        foreach(Button f_btn in _toggleButtons)
            f_btn.interactable = !(_isSelectingMove || _isMoving);

        // Disables move button while units are moving.
        _moveButton.interactable = !_isMoving;

        // Updates move button colors.
        if (_isSelectingMove)
        {
            _colorBlock.normalColor = _selectedColor;
            _colorBlock.pressedColor = _selectedColor;
            _colorBlock.selectedColor = _selectedColor;
        }

        else
        {
            _colorBlock.normalColor = _normalColor;
            _colorBlock.pressedColor = _normalColor;
            _colorBlock.selectedColor = _normalColor;
        }

        _moveButton.colors = _colorBlock;
    }

    /// <summary>
    /// Displays data based on the units selected. 
    /// Reveals & Hides panel if no units are selected.
    /// </summary>
    /// <param name="_selectedUnits"></param>
    private void DisplayUnitsData(ICollection<Unit> p_selectedUnits)
    {
        _selectedUnits = new List<Unit>(p_selectedUnits);

        HashSet<string> m_possibleResources = new HashSet<string>();

        // Destroy left-over instantiated prefabs.
        DestroyPrefabs();

        // If there's only one unit selected.
        if (p_selectedUnits.Count == 1)
        {
            _unitTypeOrCountTxt.text = p_selectedUnits.First().Name.ToUpper();
            _unitOrUnitsSelectedTxt.text = "UNIT SELECTED";
        }

        // If there's more than one unit selected.
        else
        {
            _unitTypeOrCountTxt.text = p_selectedUnits.Count.ToString();
            _unitOrUnitsSelectedTxt.text = "UNITS SELECTED";
        }

        // Control variable
        int m_iconsIndex = 0;

        // If there are more icons to display than there is space.
        if (p_selectedUnits.Count > _iconsDisplayedLimit)
        {
            m_iconsIndex = p_selectedUnits.Count - _iconsDisplayedLimit + 1;

            // Instantiates a counter in the first slot, which represents how 
            // many icons are hidden.
            Instantiate(_unitOverflowCount, _unitIconsFolder)
                .GetComponent<TMP_Text>().text = $"+{m_iconsIndex}";
        }

        // Displays most recent unit icons.
        for (int i = m_iconsIndex; i < p_selectedUnits.Count; i++)
        {
            // Instantiates unit icons.
            Instantiate(_unitIcon, _unitIconsFolder)
                .GetComponent<Image>().sprite = _selectedUnits[i].Icon;
        }

        // Clears possible resources hash set.
        m_possibleResources.Clear();

        // Goes through selected units.
        foreach (Unit _currentUnit in p_selectedUnits)
        {
            // Goes through each resource name that the current unit can collect.
            foreach (string f_resourceName in _currentUnit.ResourceNamesToCollect)
            {
                // Adds resource name to previous hash set.
                m_possibleResources.Add(f_resourceName);
            }
        }

        // Variable that current counter.
        GameObject m_rCounter;

        // Goes through each possible resource name.
        foreach (string f_resourceName in m_possibleResources)
        {
            // Instantiates a resource counter.
            m_rCounter = Instantiate(_resourceCount, _resourceCountFolder);

            // Updates the image of the resource counter to match the resource
            // type.
            m_rCounter.GetComponentInChildren<Image>().sprite = _gameData.ResourceDefaultSprites[f_resourceName];

            // Updates the counter value, with the number of this resource 
            // across all selected units.
            m_rCounter.GetComponentInChildren<TMP_Text>().text = p_selectedUnits
                .SelectMany(u => u.Resources)
                .Count(u => u.Name == f_resourceName)
                .ToString("00");
        }
    }

    /// <summary>
    /// Toggles units movement mode.
    /// </summary>
    /// <remarks>
    /// Called by the 'MOVE' Unity button, in this panel.
    /// </remarks>
    public void OnMoveButton()
    {
        _isSelectingMove = !_isSelectingMove;
        OnMoveSelect?.Invoke(_isSelectingMove);

        // Toggles affected buttons and update move button display.
        UpdateButtons();

        // Toggles target selection.
        if (_isSelectingMove) StartCoroutine(SelectingMoveTarget());
        else StopCoroutine(SelectingMoveTarget());
    }

    /// <summary>
    /// Handles units movement target selection mode.
    /// </summary>
    /// <returns>Null.</returns>
    private IEnumerator SelectingMoveTarget()
    {
        // While right click isn't pressed.
        while (!Input.GetMouseButtonDown(1))
            yield return null;

        // Calls move button again, to negate variables.
        OnMoveButton();
    }

    /// <summary>
    /// Handles selected units' movement, advancing a turn each time a set of units move.
    /// Units stop moving when blocked or when they reach the destination.
    /// </summary>
    /// <param name="p_targetCell">Target map cell to move to.</param>
    /// <returns>Time in seconds a unit takes to move.</returns>
    private IEnumerator MovingUnitsTo(MapCell p_targetCell)
    {
        ISet<Unit> m_movingUnits = new HashSet<Unit>(_selectedUnits);
        ISet<Unit> m_blockedUnits = new HashSet<Unit>();

        YieldInstruction m_waitForUnitToMove = new WaitForSeconds(_gameData.UnitMoveTime * 1.25f);

        _isMoving = true;
        OnMoving?.Invoke(_isMoving);

        // Toggle move button. Stops selecting move mode and updates buttons.
        OnMoveButton();

        // Calculates destination target prompt spawn position.
        Vector3 m_spawnPos = p_targetCell.transform.position;
        m_spawnPos.y += (_ongoingData.MapCellSize * _gameData.UnitDisplayOffset);

        // Instantiates destination target prompt on top of target cell.
        UnitTarget m_target = Instantiate(_targetDestinationPrefab, m_spawnPos,
            Quaternion.identity, _enemiesFolder).GetComponent<UnitTarget>();

        // Initializes target.
        m_target.Initialize(_gameData.UnitDisplaySize);

        Vector2 m_nextMove;
        Vector3 m_worldPosMove;

        // While there are moving units.
        while (m_movingUnits.Count > 0)
        {
            // Iterates every moving unit.
            foreach (Unit f_unit in m_movingUnits)
            {
                // Saves unit's next move towards destination.
                m_nextMove = f_unit.GetNextMoveTowards(p_targetCell.MapPosition);

                // +++ DEBUG +++ //
                Debug.Log(f_unit.Name.ToUpper() + "'S POSSIBLE MOVE: " + m_nextMove);

                // If next move isn't out of the map's bounds.
                if (_ongoingData.MapCells.ContainsKey(m_nextMove))
                {
                    // If the cell the unit is moving to isn't already occupied.
                    if (_ongoingData.MapUnits[m_nextMove] == null)
                    {
                        // Calculates world position for this unit to move to.
                        m_worldPosMove = _ongoingData.MapCells[m_nextMove].transform.position;
                        m_worldPosMove.y += (_ongoingData.MapCellSize * _gameData.UnitDisplayOffset);

                        // Moves unit.
                        _ongoingData.MoveUnitTo(f_unit, m_nextMove);
                        f_unit.MoveTo(m_nextMove, m_worldPosMove);

                        // Iterates next unit.
                        continue;
                    }
                }

                // +++ DEBUG +++ //
                Debug.Log(f_unit.Name.ToUpper() + " BLOCKED");

                // If the unit didn't move, add it to blocked units collection.
                m_blockedUnits.Add(f_unit);
            }

            // Removes blocked units from moving units collection.
            m_movingUnits.ExceptWith(m_blockedUnits);

            // Clears blocked units.
            m_blockedUnits.Clear();

            // Waits for units to move and ends turn.
            if (m_movingUnits.Count > 0)
            {
                yield return m_waitForUnitToMove;
                OnNewTurn?.Invoke();
            }
        }

        _isMoving = false;
        OnMoving?.Invoke(_isMoving);

        p_targetCell.OnPointerExit(null);
        UpdateButtons();
    }

    /// <summary>
    /// Selected units try to harvest resources from the tile they are on.
    /// Depending on the unit, may also add a resource to the game tile.
    /// </summary>
    /// <remarks>
    /// Called by the 'HARVEST' Unity button, in this panel.
    /// </remarks>
    public void OnHarvestButton()
    {
        // Stores tile that unit is standing on.
        GameTile m_targetTile;

        // Controls if a resource has been collected.
        bool m_resourceCollected = false;

        // Controls duplicate resources when trying to add to the tile.
        bool m_dupResource = false;

        // For each selected unit.
        foreach (Unit f_unit in _selectedUnits)
        {
            // Sets resource collected control to false;
            m_resourceCollected = false;

            // Gets the unit's tile.
            m_targetTile = _ongoingData.MapCells[f_unit.MapPosition].Tile;

            // Iterates resource names this unit can collect.
            for (int i = 0; i < f_unit.ResourceNamesToCollect.Count; i++)
            {
                // Iterates resources present in this game tile.
                foreach (Resource f_resource in m_targetTile.Resources)
                {
                    // If the resource's name the unit collects matches this one.
                    if (f_resource.Name == f_unit.ResourceNamesToCollect[i])
                    {
                        // Adds resource to unit.
                        f_unit.AddResource(f_resource);

                        // Removes resource from tile.
                        m_targetTile.RemoveResource(f_resource);

                        // Sets resource collected control to true.
                        m_resourceCollected = true;

                        break;
                    }
                }
            }

            // If the unit managed to collect any resource.
            if (m_resourceCollected)
            {
                // Iterates resource names this unit can generate.
                for (int i = 0; i < f_unit.ResourceNamesToGenerate.Count; i++)
                {
                    // Sets duplicate resource control to false.
                    m_dupResource = false;

                    // Iterates resources present in this game tile.
                    foreach (Resource f_resource in m_targetTile.Resources)
                    {
                        // If the resource's name the unit generates matches this one.
                        if (f_resource.Name == f_unit.ResourceNamesToGenerate[i])
                        {
                            // It already exists in the tile.
                            // Sets duplicate resource control to false.
                            m_dupResource = true;

                            break;
                        }
                    }

                    // If the tile doesn't have the resource the unit is generating.
                    if (!m_dupResource)
                    {
                        // Iterates data of every possible game resource.
                        foreach (PresetResourcesData f_resourceData in _gameData.Resources)
                        {
                            // If the resource the units is generating is found.
                            if (f_resourceData.Name == f_unit.ResourceNamesToGenerate[i])
                            {
                                // Adds the resource to the tile.
                                m_targetTile.AddResource(new Resource(
                                    f_resourceData.Name,
                                    f_resourceData.Coin,
                                    f_resourceData.Food,
                                    _gameData.GetSpriteDictOf(f_resourceData.Name),
                                    f_resourceData.DefaultResourceSprite));
                            }
                        }
                    }
                }
            }

            // Updates map cell's sprites.
            _ongoingData.MapCells[f_unit.MapPosition].UpdateResourceSprites();
        }

        // Raises event that harvest action has been completed.
        if (m_resourceCollected) OnHarvest?.Invoke();

        // Raises event that turn has ended.
        OnNewTurn?.Invoke();

        // Updates data in this panel.
        DisplayUnitsData(_selectedUnits);
    }

    /// <summary>
    /// Removes every selected unit from the ongoing data collection, destroys
    /// their game objects and raises event containing the units removed.
    /// </summary>
    /// <remarks>
    /// Called by the 'REMOVE' Unity button, in this panel.
    /// </remarks>
    public void OnRemoveButton()
    {
        foreach (Unit f_unit in _selectedUnits)
        {
            _ongoingData.RemoveUnit(f_unit);
            Destroy(f_unit.gameObject);
        }

        OnUnitsRemoved?.Invoke(_selectedUnits);
    }
}
