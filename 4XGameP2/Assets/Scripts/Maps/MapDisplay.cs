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
    private const float PAN_SPEED = 0.2f;

    /// <summary>
    /// Constant value of the map zoom speed.
    /// </summary>
    private const float ZOOM_SPEED = 0.01f;

    /// <summary>
    /// Constant value of the map min zoom.
    /// </summary>
    private const float MIN_ZOOM = 1;

    /// <summary>
    /// Constant value of the map max zoom per cell.
    /// </summary>
    private const float MAX_ZOOM_PER_CELL = 5;

    // Serialized.
    [Header("CELL PREFAB")]
    [Tooltip("Prefab of a tile cell.")]
    [SerializeField] private GameObject _cell;

    // Reference to the grid layout component.
    private GridLayoutGroup _gridLayout;

    // Reference to the content size fitter component.
    private ContentSizeFitter _contentSizeFitter;

    /// Reference to the rect transform component.
    private RectTransform _rectTransform;

    // X axis pivot limit of the rect transform's pivot.
    private float _xPivotLimit;

    // Y axis pivot limit of the rect transform's pivot.
    private float _yPivotLimit;

    // Calculated cell size, depending on the map size.
    private float _cellSize;

    /// <summary>
    /// Called by controller on Awake, gets components references.
    /// </summary>
    public void Initialize()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        _contentSizeFitter = GetComponent<ContentSizeFitter>();
        _rectTransform = GetComponent<RectTransform>();
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

        // Defines pivot limits based on map dimensions.
        _xPivotLimit = 1 / (float)(p_map.XCols * 2);
        _yPivotLimit = 1 / (float)(p_map.YRows * 2);

        // Centers pivot.
        _rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Calculates cell size based on the map dimensions, 
        // using the max X and Y cell sizes as references.
        m_newCellSize.y = MAX_Y_SIZE / p_map.YRows;
        m_newCellSize.x = MAX_X_SIZE / p_map.XCols;

        // Sets both the X and Y to the lowest value out of the 2, making a square.
        if (m_newCellSize.y < m_newCellSize.x) m_newCellSize.x = m_newCellSize.y;
        else m_newCellSize.y = m_newCellSize.x;

        _cellSize = m_newCellSize.x;

        // Resizes grid layout group default cell size.
        _gridLayout.cellSize = m_newCellSize;

        // Constraints the grid layout group to a max of X columns.
        _gridLayout.constraintCount = p_map.XCols;

        // DEBUG: Time when map starts being generated.
        DateTime m_startTime = DateTime.Now;

        // Iterates every game tile in Map Data.
        foreach (GameTile tile in p_map.GameTiles)
        {
            // Instantiates a Cell as a child of this game object.
            Instantiate(_cell, transform).GetComponent<MapCell>().
                Initialize(tile);
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
    /// Tries to move the map, using the rect transform pivot.
    /// </summary>
    /// <remarks>
    /// Using the pivot allows for the zoom to always be centered to the screen.
    /// </remarks>
    /// <param name="p_direction">Direction to move.</param>
    public void TryMove(Vector2 p_direction)
    {
        // If trying to move down.
        if (p_direction == Vector2.down)
        {
            // If pivot hasn't reached it's defined limit.
            if (_rectTransform.pivot.y > _yPivotLimit)
            {
                // Moves map up using it's pivot. 
                // Move speed is relative to the map size (yPivotLimit).
                _rectTransform.pivot += Vector2.down * PAN_SPEED * _yPivotLimit;
                _rectTransform.localPosition = Vector3.zero;

                // Rectifies pivot if it goes over the limit.
                if (_rectTransform.pivot.y < _yPivotLimit)
                {
                    Vector2 m_pivot = _rectTransform.pivot;
                    m_pivot.y = _yPivotLimit;
                    _rectTransform.pivot = m_pivot;
                }
            }
        }

        // If trying to move up.
        else if (p_direction == Vector2.up)
        {
            // If pivot hasn't reached it's defined limit.
            if (_rectTransform.pivot.y < (float)(1f - _yPivotLimit))
            {
                // Moves map down using it's pivot. 
                _rectTransform.pivot += Vector2.up * PAN_SPEED * _yPivotLimit;
                _rectTransform.localPosition = Vector3.zero;

                // Rectifies pivot if it goes over the limit.
                if (_rectTransform.pivot.y > (1 - _yPivotLimit))
                {
                    Vector2 m_pivot = _rectTransform.pivot;
                    m_pivot.y = (1 - _yPivotLimit);
                    _rectTransform.pivot = m_pivot;
                }
            }
        }

        // If trying to move left.
        else if (p_direction == Vector2.left)
        {
            // If pivot hasn't reached it's defined limit.
            if (_rectTransform.pivot.x > _xPivotLimit)
            {
                // Moves map right using it's pivot. 
                _rectTransform.pivot += Vector2.left * PAN_SPEED * _xPivotLimit;
                _rectTransform.localPosition = Vector3.zero;

                // Rectifies pivot if it goes over the limit.
                if (_rectTransform.pivot.x < _xPivotLimit)
                {
                    Vector2 m_pivot = _rectTransform.pivot;
                    m_pivot.x = _xPivotLimit;
                    _rectTransform.pivot = m_pivot;
                }
            }
        }

        // If trying to move right.
        else if (p_direction == Vector2.right)
        {
            // If pivot hasn't reached it's defined limit.
            if (_rectTransform.pivot.x < (float)(1 - _xPivotLimit))
            {
                // Moves map left using it's pivot. 
                _rectTransform.pivot += Vector2.right * PAN_SPEED * _xPivotLimit;
                _rectTransform.localPosition = Vector3.zero;

                // Rectifies pivot if it goes over the limit.
                if (_rectTransform.pivot.x > (1 - _xPivotLimit))
                {
                    Vector2 m_pivot = _rectTransform.pivot;
                    m_pivot.x = (1 - _xPivotLimit);
                    _rectTransform.pivot = m_pivot;
                }
            }
        }
    }

    /// <summary>
    /// Tries to zoom in or out of the map using the rect transform's local scale.
    /// Scaling locally makes it follow it's pivot, which will always be centered.
    /// </summary>
    /// <param name="p_direction">Positive: zoom in, negative: zoom out.</param>
    public void TryZoom(int p_direction)
    {
        // Zooms in while the local scale is less than the defined max zoom per cell.
        if (p_direction > 0 && _rectTransform.localScale.x < MAX_ZOOM_PER_CELL / _cellSize)
            _rectTransform.localScale += _rectTransform.localScale * ZOOM_SPEED;

        // Zooms out.
        else if (p_direction < 0 && _rectTransform.localScale.x > MIN_ZOOM)
            _rectTransform.localScale += _rectTransform.localScale * -ZOOM_SPEED;
    }
}
