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
    /// Event raised when the 'MOVE' units button is toggled. True = on.
    /// </summary>
    public static event Action<bool> OnSelectingMoveTarget;

    /// <summary>
    /// Event raised when the units harvest resources.
    /// </summary>
    public static event Action OnHarvest;

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
    [SerializeField] private PresetResourcesData _resourceData;

    // Private list containing displayed selected units.
    private List<Unit> _selectedUnits;

    // Private reference to hold block of button colors.
    private ColorBlock _colorBlock;

    // Control variables for moving units.
    private bool _isSelectingTarget;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UnitSelection.OnUnitsSelected += DisplayUnitsData;
        MapCell.OnTargeted += MoveUnitsTo;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UnitSelection.OnUnitsSelected -= DisplayUnitsData;
        MapCell.OnTargeted -= MoveUnitsTo;
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
        // Resets control moving variable.
        _isSelectingTarget = false;

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
        // Toggles affected buttons.
        foreach (Button f_btn in _toggleButtons)
            f_btn.interactable = !_isSelectingTarget;

        // Updates move button colors.
        if (_isSelectingTarget)
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
                .ToString();
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
        _isSelectingTarget = !_isSelectingTarget;
        OnSelectingMoveTarget?.Invoke(_isSelectingTarget);

        // Toggles affected buttons and update move button display.
        UpdateButtons();

        // Toggles target selection.
        if (_isSelectingTarget) StartCoroutine(SelectingTarget());
        else StopCoroutine(SelectingTarget());
    }

    /// <summary>
    /// Handles units movement target selection mode.
    /// </summary>
    /// <returns>Null.</returns>
    private IEnumerator SelectingTarget()
    {
        // While right click isn't pressed.
        while (!Input.GetMouseButtonDown(1))
            yield return null;

        // Calls move button again, to negate variables.
        OnMoveButton();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_targetCell"></param>
    private void MoveUnitsTo(MapCell _targetCell)
    {
        Debug.Log("Units moving to " + _targetCell.Tile.Name);
        OnMoveButton();

        Instantiate(_targetDestinationPrefab, _enemiesFolder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Called by the 'HARVEST' Unity button, in this panel.
    /// </remarks>
    public void OnHarvestButton()
    {
        // Tile that the unit is standing on
        GameTile m_targetTile;
        bool m_resourceCollected = false;

        // For each selected unit
        foreach (Unit f_unit in _selectedUnits)
        {
            // Gets the tiles position
            m_targetTile = _ongoingData.MapCells[f_unit.MapPosition].Tile;

            foreach (Resource f_tileResource in m_targetTile.Resources)
            {
                Debug.Log("BEFORE - " + f_tileResource.Name);
            }

            for (int i = 0; i < f_unit.ResourceNamesToCollect.Count; i++)
            {
                // If the resource's name is on the unit's resourceToCollect list
                foreach (Resource f_currentResource in m_targetTile.Resources)
                {
                    if (f_currentResource.Name == f_unit.ResourceNamesToCollect[i])
                    {
                        // Adds resource to unit
                        f_unit.AddResource(f_currentResource);

                        // Removes resource from tile
                        m_targetTile.RemoveResource(f_currentResource);

                        m_resourceCollected = true;

                        break;
                    }
                }
            }

            if (m_resourceCollected)
            {
                for (int i = 0; i < f_unit.ResourceNamesToGenerate.Count; i++)
                {
                    foreach (PresetResourcesData f_resourceToCompare in _gameData.Resources)
                    {
                        if (f_resourceToCompare.Name == f_unit.ResourceNamesToGenerate[i])
                        {
                            m_targetTile.AddResource(new Resource(
                                f_resourceToCompare.Name,
                                f_resourceToCompare.Coin,
                                f_resourceToCompare.Food,
                                _gameData.GetSpriteDictOf(f_resourceToCompare.Name),
                                f_resourceToCompare.DefaultResourceSprite));

                            break;
                        }
                    }

                }
            }

            OnHarvest?.Invoke();
            DisplayUnitsData(_selectedUnits);

            foreach (Resource f_tileResource in m_targetTile.Resources)
            {
                Debug.Log("AFTER - " + f_tileResource.Name);
            }
        }
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
