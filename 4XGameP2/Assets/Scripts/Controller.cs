using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages game states and input.
/// </summary>
public class Controller : MonoBehaviour
{
    /// <summary>
    /// Event raised when a analytics button is pressed. 
    /// Included its index and map data.
    /// </summary>
    public static event Action<int, MapData> OnAnalytics;

    // Serialized variables.
    [Tooltip("Reference to map display component.")]
    [SerializeField] private MapDisplay _mapDisplay;

    // Reference to the generic User Interface.
    private IUserInterface _userInterface;

    // Reference to the map data currently selected.
    private MapData _selectedMap;

    // Reference to the current Game State enum.
    private GameStates _currentState;

    // Control variables for managing game states.
    private bool _isMapDisplayed;
    private bool _inAnalytics;

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
    }

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelPreStart.OnPromptRevealed += () => StartCoroutine(WaitForPreStartKey());
        UIPanelMapBrowser.OnLoad += SaveMap;
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
        UIPanelMapBrowser.OnLoad -= SaveMap;
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

                // Loads tiles' data inside the selected map.
                _selectedMap.LoadGameTilesData();

                // Generates it with the loaded data.
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

                // If currently viewing Analytics data.
                else if (_inAnalytics)
                {
                    // Updates analytics control variable.
                    _inAnalytics = false;

                    // Sets UI state to resume from analytics.
                    _userInterface.ChangeUIState(UIStates.RESUME_FROM_ANALYTICS);
                }

                // Otherwise, Sets UI state to resume from inspector.
                else _userInterface.ChangeUIState(UIStates.RESUME_FROM_INSPECTOR);

                break;

            case GameStates.PAUSE:

                // If in analytics, sets UI state to analytics.
                if (_inAnalytics) _userInterface.ChangeUIState(UIStates.ANALYTICS);

                // Else, sets UI state to inspector.
                else _userInterface.ChangeUIState(UIStates.INSPECTOR);

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
    /// Updates selected map variable and sets the game state to Load Map.
    /// </summary>
    /// <param name="p_map">Map Data to be saved.</param>
    private void SaveMap(MapData p_map)
    {
        // Saves parameter map data as selected map.
        _selectedMap = p_map;

        // Sets game state to Load Map.
        ChangeGameState(GameStates.LOAD_MAP);
    }

    /// <summary>
    /// Sets game state to Pause and sends out Analytics data.
    /// </summary>
    /// <param name="p_index">Button number clicked.</param>
    /// <remarks>
    /// Called by the '1', '2', '3', '4' and '5' Unity buttons, 
    /// in the gameplay panel.
    /// </remarks>
    public void OnAnalyticsButton(int p_index)
    {
        // Updates analytics control variable.
        _inAnalytics = true;

        // Sets game state to Pause.
        ChangeGameState(GameStates.PAUSE);

        // Raises event that controller has enabled analytics.
        OnAnalytics(p_index, _selectedMap);
    }
}
