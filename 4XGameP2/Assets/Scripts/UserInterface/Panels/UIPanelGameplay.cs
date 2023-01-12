using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel displayed in Gameplay UI state.
/// Handles units selection, map resources display, and relevant navigation HUD.
/// </summary>
public class UIPanelGameplay : UIPanel
{
    /// <summary>
    /// Event raised when the the back to menu button is pressed.
    /// </summary>
    public static event Action OnRestart;

    /// <summary>
    /// Event raised when a unit is added to the game.
    /// </summary>
    public static event Action<Unit> OnUnitAdded;

    // Serialized variables.
    [Header("TURNS")]
    [Tooltip("Text component holding the current turn number.")]
    [SerializeField] private TMP_Text _turnDisplay;
    [Header("RESOURCES COUNT")]
    [Tooltip("Parent game object of map resource's count.")]
    [SerializeField] private Transform _resourceCountFolder;
    [Tooltip("Prefab of individual map resource's count.")]
    [SerializeField] private GameObject _mapResourceCount;
    [Header("UNITS")]
    [Tooltip("Units display parent object.")]
    [SerializeField] private Transform _unitsFolder;
    [Tooltip("Unit prefab.")]
    [SerializeField] private GameObject _unitPrefab;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Preset Game Data.")]
    [SerializeField] private PresetGameDataSO _presetData;
    [Tooltip("Scriptable Object with Ongoing Game Data.")]
    [SerializeField] private OngoingGameDataSO _ongoingData;

    // Reference to MapData.
    private MapData _mapData;

    // Current turn count.
    private uint _turnCount;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        MapDisplay.OnMapGenerated += SetUpResourceCounters;
        UIPanelUnitsControl.OnHarvest += UpdateResourceCounters;
        UIPanelUnitsControl.OnNewTurn += UpdateTurnCounter;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        MapDisplay.OnMapGenerated -= SetUpResourceCounters;
        UIPanelUnitsControl.OnHarvest -= UpdateResourceCounters;
        UIPanelUnitsControl.OnNewTurn -= UpdateTurnCounter;
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        // Closes the panel.
        ClosePanel();

        // Resets turns.
        _turnCount = 0;
        UpdateTurnCounter();

        // Destroys any existing visual resource count objects, inside it's folder.
        foreach (Transform f_child in _resourceCountFolder)
            Destroy(f_child.gameObject);

        // Iterates all possible resources' preset values.
        foreach (PresetResourcesData f_rData in _presetData.Resources)
        {
            // Instantiates a visual resource count object and updates its sprite
            // to match the resource's default sprite.
            Instantiate(_mapResourceCount, _resourceCountFolder).
                GetComponentInChildren<Image>().sprite = f_rData.DefaultResourceSprite;
        }
    }

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0) => base.Open(p_transitionTime);

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0) => base.Close(p_transitionTime);

    /// <summary>
    /// Updates local map data and counters.
    /// </summary>
    /// <param name="p_mapData">Local map data</param>
    public void SetUpResourceCounters(MapData p_mapData)
    {
        // Saves map data reference and updates counters.
        _mapData = p_mapData;
        UpdateResourceCounters();
    }

    /// <summary>
    /// Updates resource counters.
    /// </summary>
    private void UpdateResourceCounters()
    {
        // Variable that dictates which name to access.
        int m_nameIndex = 0;

        // Goes through each counter.
        foreach (Transform f_counter in _resourceCountFolder)
        {
            // Stores the TMP component.
            TMP_Text f_textComponent = f_counter.GetComponentInChildren<TMP_Text>();

            // Updates text to display number of said resources on the map.
            f_textComponent.text = _mapData.GameTiles
            .SelectMany(t => t.Resources)
            .Where(r => (r.Name.ToLower().Replace(" ", ""))
            .Equals(_presetData.ResourceNames.ToList()[m_nameIndex]))
            .Count().ToString();

            // Increases the variable so the next name is accessed.
            m_nameIndex++;
        }
    }

    /// <summary>
    /// Updates current turn display.
    /// </summary>
    private void UpdateTurnCounter() => _turnDisplay.text = _turnCount++.ToString("00");

    /// <summary>
    /// Instantiates unit in the map. Unit depends on index parameter.
    /// </summary>
    /// <remarks>
    /// Called by the 'ADD UNIT' Unity buttons, at the top of this panel.
    /// </remarks>
    public void OnAddUnit(int p_index)
    {
        // Throws exception if indexed unit doesn't exist.
        if (p_index > _presetData.Units.Count)
            throw new IndexOutOfRangeException("Button index exceeds available units.");

        // Returns if maximum number of units has been spawned in.
        if (_ongoingData.MapUnitsCount == _ongoingData.MapCellsCount) return;

        Vector2 m_randomMapPos;

        // Finds a random map position that doesn't have a unit in it.
        do
        {   m_randomMapPos = new Vector2(
                UnityEngine.Random.Range(0, _mapData.XCols),
                UnityEngine.Random.Range(0, _mapData.YRows));
        }
        while (_ongoingData.MapUnits[m_randomMapPos] != null);

        // Calculates spawn position.
        Vector3 m_worldPos = _ongoingData.MapCells[m_randomMapPos].transform.position;
        m_worldPos.y += (_ongoingData.MapCellSize * _presetData.UnitDisplayOffset);

        // Instantiates unit in it's positions, on an overlaying layer.
        Unit m_unit = Instantiate(_unitPrefab, m_worldPos, Quaternion.identity, 
            _unitsFolder).GetComponent<Unit>();

        // Adds unit to ongoing saved data, together with it's relative position.
        _ongoingData.AddUnitTo(m_unit, m_randomMapPos);

        // Raises event that this unit as been added to the map.
        OnUnitAdded?.Invoke(m_unit);

        // Initializes unit with the given index.
        m_unit.Initialize(_presetData.Units[p_index], m_randomMapPos, 
            _presetData.UnitDisplaySize, _presetData.UnitMoveTime);
    }

    /// <summary>
    /// Raises OnRestart event.
    /// </summary>
    /// <remarks>
    /// Called by the 'BACK TO MENU' Unity button, in this panel.
    /// </remarks>
    public void OnBackToMenuButton() => OnRestart?.Invoke();
}
