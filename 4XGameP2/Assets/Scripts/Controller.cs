using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game states and input.
/// </summary>
public class Controller : MonoBehaviour
{
    // Serialized variables.
    [Tooltip("Reference to map display.")]
    [SerializeField] private MapDisplay _mapDisplay;
    [Tooltip("Reference to unit selection.")]
    [SerializeField] private UnitSelection _unitSelection;
    [Tooltip("UI Warnings reference")]
    [SerializeField] private UIWarnings _warnings;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Preset Game Data")]
    [SerializeField] private PresetGameDataSO _presetData;

    // Reference to the generic User Interface.
    private IUserInterface _userInterface;

    // Reference to the map data currently selected.
    private MapData _selectedMap;

    // Reference to the current Game State enum.
    private GameStates _currentState;

    // Control variables for managing game states.
    private bool _isMapDisplayed;
    private bool _isInspecting;
    private bool _isControllingUnits;

    // Control variables for input.
    private float _mouseDownTime;
    private float _dragDelay = 0.1f;

    /// <summary>
    /// Unity method, program starts here.
    /// </summary>
    private void Awake()
    {
        // Saves specific user interface for this program (UI panels).
        _userInterface = FindObjectOfType<PanelsUserInterface>();

        // Initializes user interface and map display.
        _userInterface.Initialize();
        _mapDisplay.Initialize();
        _unitSelection.Initialize();

        // Clear all warnings.
        _warnings.ClearAll();
    }

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelPreStart.OnPromptRevealed += () => StartCoroutine(WaitForPreStartKey());
        UIPanelMapBrowser.OnLoad += TryToLoadMap;
        MapData.OnValidLoadedData += HandleLoadedDataStatus;
        UIPanelGameplay.OnRestart += () => ChangeGameState(GameStates.PRE_START);
        MapDisplay.OnMapGenerated += (_) => ChangeGameState(GameStates.GAMEPLAY);
        MapCell.OnInspectView += () => ChangeGameState(GameStates.INSPECTOR);
        UnitSelection.OnUnitsSelected += HandleUnitsSelected;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        UIPanelPreStart.OnPromptRevealed -= () => StopCoroutine(WaitForPreStartKey());
        UIPanelMapBrowser.OnLoad -= TryToLoadMap;
        MapData.OnValidLoadedData -= HandleLoadedDataStatus;
        UIPanelGameplay.OnRestart -= () => ChangeGameState(GameStates.PRE_START);
        MapDisplay.OnMapGenerated -= (_) => ChangeGameState(GameStates.GAMEPLAY);
        MapCell.OnInspectView -= () => ChangeGameState(GameStates.INSPECTOR);
        UnitSelection.OnUnitsSelected -= HandleUnitsSelected;
    }

    /// <summary>
    /// Unity method, called after Awake() and OnEnable().
    /// </summary>
    private void Start()
    {
        // Sets starting game state as PRE-START.
        ChangeGameState(GameStates.PRE_START);
    }

    /// <summary>
    /// Unity method, called every frame.
    /// </summary>
    private void Update()
    {
        // Input for Inspector game state
        if (_currentState == GameStates.INSPECTOR)
        {
            // Backs out from inspector state.
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
                ChangeGameState(GameStates.GAMEPLAY);
        }

        // Input for Gameplay game state (when the Map is displayed and controllable).
        if (_currentState == GameStates.GAMEPLAY || _currentState == GameStates.UNITS_CONTROL)
        {
            // Map panning.
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _mapDisplay.TryMove(Vector2.left);

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _mapDisplay.TryMove(Vector2.right);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _mapDisplay.TryMove(Vector2.up);

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _mapDisplay.TryMove(Vector2.down);

            // Map zooming.
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Plus))
                _mapDisplay.TryZoom(1);

            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Minus))
                _mapDisplay.TryZoom(-1);

            // Units selection.
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _unitSelection.StartSelectionBox();
                _mouseDownTime = Time.time;
            }

            else if (Input.GetKey(KeyCode.Mouse0) && _mouseDownTime + _dragDelay < Time.time)
                _unitSelection.ResizeSelectionBox();

            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _unitSelection.EndSelectionBox();
                _mouseDownTime = 0;
            }

            else if (Input.GetKeyDown(KeyCode.Mouse1))
                _unitSelection.DeselectAll();
        }
    }

    /// <summary>
    /// Handles game state changes.
    /// </summary>
    /// <param name="p_gameState">New game state.</param>
    private void ChangeGameState(GameStates p_gameState)
    {
        // Saves as current game state.
        _currentState = p_gameState;

        // Checks current state.
        switch (_currentState)
        {
            case GameStates.PRE_START:

                // Resets control variables.
                _isMapDisplayed = false;
                _isInspecting = false;
                _isControllingUnits = false;

                // Sets UI state to pre-start.
                _userInterface.ChangeUIState(UIStates.PRE_START);

                break;

            case GameStates.MAP_BROWSER:

                // Sets UI state to map-browser.
                _userInterface.ChangeUIState(UIStates.MAP_BROWSER);

                break;

            case GameStates.LOAD_MAP:

                // Sets UI state to load-map.
                _userInterface.ChangeUIState(UIStates.LOAD_MAP);

                // Centers map display.
                _mapDisplay.transform.localPosition = Vector3.zero;
                _mapDisplay.transform.localScale = Vector3.one;

                // Generates map with loaded data.
                _mapDisplay.GenerateMap(_selectedMap);

                break;

            case GameStates.GAMEPLAY:

                // If map isn't being displayed.
                if (!_isMapDisplayed)
                {
                    // Sets UI state to display map.
                    _isMapDisplayed = true;
                    _userInterface.ChangeUIState(UIStates.DISPLAY_MAP);
                }

                // If is inspecting.
                else if (_isInspecting)
                {
                    // Sets UI state to resume from inspector.
                    _isInspecting = false;
                    _userInterface.ChangeUIState(UIStates.RESUME_FROM_INSPECTOR);
                }

                // Otherwise, must be in units control.
                else
                {
                    // Sets UI state to resume from units control.
                    _isControllingUnits = false;
                    _userInterface.ChangeUIState(UIStates.RESUME_FROM_UNITS_CONTROL);
                }

                break;

            case GameStates.INSPECTOR:

                // Sets UI state to inspector.
                _isInspecting = true;
                _userInterface.ChangeUIState(UIStates.INSPECTOR);

                break;

            case GameStates.UNITS_CONTROL:

                // Sets UI state to units control.
                _isControllingUnits = true;
                _userInterface.ChangeUIState(UIStates.UNITS_CONTROL);
                
                break;
        }
    }

    /// <summary>
    /// Sets game state to Map Browser when a key is pressed.
    /// </summary>
    /// <returns>null while user doesn't press a key.</returns>
    private IEnumerator WaitForPreStartKey()
    {
        // Waits for any input.
        while (!Input.anyKey) yield return null;

        // Sets game state to Map Browser.
        ChangeGameState(GameStates.MAP_BROWSER);
    }

    /// <summary>
    /// Updates selected map variable and to load game tiles data.
    /// </summary>
    /// <param name="p_map">Map Data to be saved.</param>
    public void TryToLoadMap(MapData p_map)
    {
        // Clear warnings.
        _warnings.ClearInvalidFilesWarning();

        // Saves parameter map data as selected map.
        _selectedMap = p_map;

        // Loads tiles' data inside the selected map.
        _selectedMap.LoadGameTilesData(_presetData);
    }

    /// <summary>
    /// Handles what to do next based if the selected map's data is valid or not.
    /// </summary>
    /// <param name="p_isValid">True is map's data is valid.</param>
    private void HandleLoadedDataStatus(bool p_isValid)
    {
        // If it's valid, generate map.
        if (p_isValid) ChangeGameState(GameStates.LOAD_MAP);

        // If not, display warning.
        else _warnings.DisplayInvalidFilesWarning();
    }

    /// <summary>
    /// Handles units control game state, depending on number of selected units.
    /// </summary>
    /// <param name="p_unitsSelected">Selected units.</param>
    private void HandleUnitsSelected(ICollection<Unit> p_unitsSelected)
    {
        // If units control panel isn't being displayed and there are units selected.
        if (!_isControllingUnits && p_unitsSelected.Count > 0)
            ChangeGameState(GameStates.UNITS_CONTROL);

        // If units control panel is being displayed, but there are no units selected.
        else if (_isControllingUnits && p_unitsSelected.Count == 0)
            ChangeGameState(GameStates.GAMEPLAY);
    }
}
