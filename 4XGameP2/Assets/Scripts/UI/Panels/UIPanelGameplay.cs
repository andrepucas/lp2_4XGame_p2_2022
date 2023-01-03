using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Panel displayed in Gameplay UI state. Mostly contains HUD.
/// </summary>
public class UIPanelGameplay : UIPanel
{
    /// <summary>
    /// Event raised when the the back to menu button is pressed.
    /// </summary>
    public static event Action OnRestart;

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
    [Tooltip("Unit prefab.")]
    [SerializeField] private GameObject _unitPrefab;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Preset Game Data.")]
    [SerializeField] private PresetGameDataSO _presetData;
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
    }

    private void OnDisable()
    {
        MapDisplay.OnMapGenerated -= SetUpResourceCounters;
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        ClosePanel();

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
    /// <param name="p_turnsToAdd">Number of turns to add.</param>
    private void UpdateTurnCounter(uint p_turnsToAdd = 0)
    {
        _turnCount += p_turnsToAdd;
        _turnDisplay.text = _turnCount.ToString("00");
    }

    public void OnAddUnit()
    {
        Debug.Log("Adding Unit");

        MapCell m_randomMapCell;

        // Finds a random map cell that doesn't have a unit in it.
        do {m_randomMapCell = _ongoingData.MapCells[GetRandomMapPos()];}
        while (m_randomMapCell.HasUnit);

        // Instantiates unit in it.
        Instantiate(_unitPrefab, m_randomMapCell.transform)
            .GetComponent<Unit>().Initialize(_presetData.Units[0].Color);
    }

    private Vector2 GetRandomMapPos()
    {
        return new Vector2(
            UnityEngine.Random.Range(0, _mapData.XCols),
            UnityEngine.Random.Range(0, _mapData.YRows));
    }

    /// <summary>
    /// Raises OnRestart event.
    /// </summary>
    /// <remarks>
    /// Called by the 'BACK TO MENU' Unity button, in this panel.
    /// </remarks>
    public void OnBackToMenuButton() => OnRestart?.Invoke();
}
