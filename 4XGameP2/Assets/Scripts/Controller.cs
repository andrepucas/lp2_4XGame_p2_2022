using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages game states and input.
/// </summary>
public class Controller : MonoBehaviour
{
    // Serialized variables.
    [Tooltip("Reference to map display component.")]
    [SerializeField] private MapDisplay _mapDisplay;
    [Tooltip("UI Warnings reference")]
    [SerializeField] private UIWarnings _warnings;
    [Header("MAP TILES DATA")]
    [Tooltip("Scriptable Object with all Map Tiles Data")]
    [SerializeField] private MapTilesDataSO _data;

    // Reference to the generic User Interface.
    private IUserInterface _userInterface;

    // Reference to the map data currently selected.
    private MapData _selectedMap;

    // Reference to the current Game State enum.
    private GameStates _currentState;

    // Control variables for managing game states.
    private bool _isMapDisplayed;

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

        // Clear all warnings.
        _warnings.ClearAll();
    }

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelPreStart.OnPromptRevealed += () => StartCoroutine(WaitForPreStartKey());
        // UIPanelMapBrowser.OnLoad += TryToLoadMap;
        MapData.OnValidLoadedData += HandleLoadedDataStatus;
        UIPanelGameplay.OnRestart += () => ChangeGameState(GameStates.PRE_START);
        _mapDisplay.OnMapGenerated += () => ChangeGameState(GameStates.GAMEPLAY);
        MapCell.OnInspectView += () => ChangeGameState(GameStates.PAUSE);
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes from events.
    /// </summary>
    private void OnDisable()
    {
        UIPanelPreStart.OnPromptRevealed -= () => StopCoroutine(WaitForPreStartKey());
        // UIPanelMapBrowser.OnLoad -= TryToLoadMap;
        MapData.OnValidLoadedData -= HandleLoadedDataStatus;
        UIPanelGameplay.OnRestart -= () => ChangeGameState(GameStates.PRE_START);
        _mapDisplay.OnMapGenerated -= () => ChangeGameState(GameStates.GAMEPLAY);
        MapCell.OnInspectView -= () => ChangeGameState(GameStates.PAUSE);
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
    /// Unity method, called on a fixed interval of time.
    /// </summary>
    private void FixedUpdate()
    {
        // Input for Gameplay game state (when the Map is displayed and controllable).
        if (_currentState == GameStates.GAMEPLAY)
        {
            // Tries to move map left.
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                _mapDisplay.TryMove(Vector2.left);

            // Tries to move map right.
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                _mapDisplay.TryMove(Vector2.right);

            // Tries to move map up.
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                _mapDisplay.TryMove(Vector2.up);

            // Tries to move map down.
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                _mapDisplay.TryMove(Vector2.down);

            // Tries to zoom in.
            if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Plus))
                _mapDisplay.TryZoom(1);

            // Tries to zoom out.
            if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Minus))
                _mapDisplay.TryZoom(-1);
        }
    }

    /// <summary>
    /// Unity method, called every frame.
    /// </summary>
    private void Update()
    {
        // Input for Pause game state (either in inspector or analytics mode).
        if (_currentState == GameStates.PAUSE)
        {
            // Backs out from pause game state.
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(0))
                ChangeGameState(GameStates.GAMEPLAY);
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

                // Updates map displayed control variable.
                _isMapDisplayed = false;

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
                    // Updates map displayed control variable.
                    _isMapDisplayed = true;

                    // Sets UI state to display map.
                    _userInterface.ChangeUIState(UIStates.DISPLAY_MAP);
                }

                // Otherwise, Sets UI state to resume from inspector.
                else _userInterface.ChangeUIState(UIStates.RESUME_FROM_INSPECTOR);

                break;

            case GameStates.PAUSE:

                // Sets UI state to inspector.
                _userInterface.ChangeUIState(UIStates.INSPECTOR);

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
        _selectedMap.LoadGameTilesData(_data);
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
}
