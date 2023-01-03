using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for displaying and controlling the map (move and scale).
/// </summary>
public class MapDisplay : MonoBehaviour
{
    /// <summary>
    /// Event raised when the map finishes generating.
    /// </summary>
    public static event Action<MapData> OnMapGenerated;

    /// <summary>
    /// Constant value of the max cell size in the Y axis.
    /// </summary>
    private const float MAX_Y_SIZE = 13f;

    /// <summary>
    /// Constant value of the max cell size in the X axis.
    /// </summary>
    private const float MAX_X_SIZE = 28f;

    /// <summary>
    /// Constant  value of the map panning speed.
    /// </summary>
    private const float PAN_SPEED = 0.02f;

    /// <summary>
    /// Constant value of the map zoom speed.
    /// </summary>
    private const float ZOOM_SPEED = 0.01f;

    /// <summary>
    /// Constant value of the map max zoom per cell.
    /// </summary>
    private const float CELL_ZOOM_RATIO = 2;

    // Serialized.
    [Header("CELL PREFAB")]
    [Tooltip("Prefab of a tile cell.")]
    [SerializeField] private GameObject _cell;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Ongoing Game Data")]
    [SerializeField] private OngoingGameDataSO _ongoingData;

    // Reference to the grid layout component.
    private GridLayoutGroup _gridLayout;

    // Reference to the content size fitter component.
    private ContentSizeFitter _contentSizeFitter;

    // X axis view limit for the map. Constrains camera position when panning.
    private float _maxCamViewWidth;

    // Y axis view limit for the map. Constrains camera position when panning.
    private float _maxCamViewHeight;

    // Calculated cell size, depending on the map size.
    private float _cellSize;

    // Reference to the main and only camera
    private Camera _cam;

    // Stores values of max and min orthographic cam sizes, for zoom limits.
    private float _camMaxSize, _camMinSize;

    /// <summary>
    /// Called by controller on Awake, gets components references.
    /// </summary>
    public void Initialize()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();
        _cam = Camera.main;
        _camMaxSize = _cam.orthographicSize;
    }

    /// <summary>
    /// Generates and instantiates the game map.
    /// </summary>
    /// <param name="grid">Map data.</param>
    public void GenerateMap(MapData p_map)
    {
        Vector2 m_newCellSize;

        // Enables grid generation components.
        _contentSizeFitter.enabled = true;
        _gridLayout.enabled = true;

        // Resets camera.
        _cam.transform.position = Vector3.zero;
        _cam.orthographicSize = _camMaxSize;

        // Calculates cell size based on the map dimensions, 
        // using the max X and Y cell sizes as references.
        m_newCellSize.y = MAX_Y_SIZE / p_map.YRows;
        m_newCellSize.x = MAX_X_SIZE / p_map.XCols;

        // Sets both the X and Y to the lowest value out of the 2, making a square.
        if (m_newCellSize.y < m_newCellSize.x) m_newCellSize.x = m_newCellSize.y;
        else m_newCellSize.y = m_newCellSize.x;

        _cellSize = m_newCellSize.x;

        // Defines camera view limits based on the map dimensions.
        _maxCamViewWidth = (p_map.XCols * _cellSize) / 2 - (_cellSize / 2);
        _maxCamViewHeight = (p_map.YRows * _cellSize) / 2 - (_cellSize / 2);

        // Calculates and stores the cam min size, for zooming purposes, based
        // on the cell size and it's predefined ratio.
        _camMinSize = _cellSize * CELL_ZOOM_RATIO;

        // Resizes grid layout group default cell size.
        _gridLayout.cellSize = m_newCellSize;

        // Constraints the grid layout group to a max of X columns.
        _gridLayout.constraintCount = p_map.XCols;

        // DEBUG: Time when map starts being generated.
        DateTime m_startTime = DateTime.Now;

        MapCell m_mapCell;
        Vector2 m_mapPosition;
        float m_rows = 0;
        float m_cols = 0;

        _ongoingData.NewMap();

        // Iterates every game tile in Map Data.
        for (int i = 0; i < p_map.GameTiles.Count; i++)
        {
            // Instantiates a cell as a child of this game object.
            m_mapCell = Instantiate(_cell, transform).GetComponent<MapCell>();

            // Initializes it.
            m_mapCell.Initialize(p_map.GameTiles[i]);

            // Saves current map position.
            m_mapPosition.y = m_rows;
            m_mapPosition.x = m_cols;

            // Saves cell together with it's relative map position.
            _ongoingData.SaveMapInfo(m_mapCell, m_mapPosition);

            // Increment map position.
            m_cols++;
            if (m_cols == p_map.XCols)
            {
                m_cols = 0;
                m_rows++;
            }
        }

        // DEBUG: Displays time map took to generate.
        Debug.Log("Map generated in: " + (DateTime.Now - m_startTime));

        // Raise event that map was generated.
        OnMapGenerated?.Invoke(p_map);

        // Disable grid components after 0.5 seconds.
        Invoke("DisableGridLayout", 0.5f);
    }

    /// <summary>
    /// Disables grid components after the grid is instantiated.
    /// </summary>
    /// <remarks>
    /// Boosts performance by cutting off automatic component calls.
    /// </remarks>
    private void DisableGridLayout()
    {
        _contentSizeFitter.enabled = false;
        _gridLayout.enabled = false;
    }

    /// <summary>
    /// Tries to move the map, using the camera's transform position.
    /// </summary>
    /// <param name="p_direction">Direction to move.</param>
    public void TryMove(Vector2 p_direction)
    {
        // If trying to move down.
        if (p_direction == Vector2.down)
        {
            // If cam's position is above the map's min. height limit.
            if (_cam.transform.position.y > (-_maxCamViewHeight))
            {
                // Moves camera down, with speed relative to zoom.
                _cam.transform.position += 
                    Vector3.down * PAN_SPEED * _cam.orthographicSize;

                // Prevents cam position from going under its min. height limit.
                if (_cam.transform.position.y < (-_maxCamViewHeight))
                {
                    Vector3 m_positionFix = _cam.transform.position;
                    m_positionFix.y = (-_maxCamViewHeight);

                    _cam.transform.position = m_positionFix;
                }
            }
        }

        // If trying to move up.
        else if (p_direction == Vector2.up)
        {
            // If cam's position is below the map's max. height limit.
            if (_cam.transform.position.y < _maxCamViewHeight)
            {
                // Moves camera up, with speed relative to zoom.
                _cam.transform.position += 
                    Vector3.up * PAN_SPEED * _cam.orthographicSize;

                // Prevents cam position from going over its max. height limit.
                if (_cam.transform.position.y > _maxCamViewHeight)
                {
                    Vector3 m_positionFix = _cam.transform.position;
                    m_positionFix.y = _maxCamViewHeight;

                    _cam.transform.position = m_positionFix;
                }
            }
        }

        // If trying to move left.
        else if (p_direction == Vector2.left)
        {
            // If cam's position is above the map's min. width limit.
            if (_cam.transform.position.x > (-_maxCamViewWidth))
            {
                // Moves camera left, with speed relative to zoom.
                _cam.transform.position += 
                    Vector3.left * PAN_SPEED * _cam.orthographicSize;

                // Prevents cam position from going under its min. width limit.
                if (_cam.transform.position.y < (-_maxCamViewHeight))
                {
                    Vector3 m_positionFix = _cam.transform.position;
                    m_positionFix.x = (-_maxCamViewWidth);

                    _cam.transform.position = m_positionFix;
                }
            }
        }

        // If trying to move right.
        else if (p_direction == Vector2.right)
        {
            // If cam's position is under the map's max. width limit.
            if (_cam.transform.position.x < _maxCamViewWidth)
            {
                // Moves camera right, with speed relative to zoom.
                _cam.transform.position += 
                    Vector3.right * PAN_SPEED * _cam.orthographicSize;

                // Prevents cam position from going over its max. width limit.
                if (_cam.transform.position.y > _maxCamViewHeight)
                {
                    Vector3 m_positionFix = _cam.transform.position;
                    m_positionFix.x = _maxCamViewWidth;

                    _cam.transform.position = m_positionFix;
                }
            }
        }
    }

    /// <summary>
    /// Tries to zoom in or out of the map using the camera's orthographic size.
    /// </summary>
    /// <param name="p_direction">Positive: zoom in, negative: zoom out.</param>
    public void TryZoom(int p_direction)
    {
        // Zooms in.
        if (p_direction > 0 && _cam.orthographicSize > _camMinSize)
        {
            _cam.orthographicSize -= _cam.orthographicSize * ZOOM_SPEED;

            // Prevents cam size from going under its limit.
            if (_cam.orthographicSize < _camMinSize)
                _cam.orthographicSize = _camMinSize;
        }


        // Zooms out.
        else if (p_direction < 0 && _cam.orthographicSize < _camMaxSize)
        {
            _cam.orthographicSize += _cam.orthographicSize * ZOOM_SPEED;

            // Prevents cam size from going over its limit.
            if (_cam.orthographicSize > _camMaxSize) 
                _cam.orthographicSize = _camMaxSize;
        }
    }
}
