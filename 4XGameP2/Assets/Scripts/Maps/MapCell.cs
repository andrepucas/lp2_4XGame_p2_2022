using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Displays a game tile.
/// </summary>
public class MapCell : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Event raised when this cell is clicked.
    /// </summary>
    public static event Action OnInspectView;

    /// <summary>
    /// Event raised when this cell is clicked, after view. Includes the 
    /// associated game tile and its resources' sprites.
    /// </summary>
    public static event Action<GameTile, List<Sprite>> OnInspectData;

    /// <summary>
    /// Event raised when this cell is targeted for units to move towards.
    /// </summary>
    public static event Action<MapCell> OnTargeted;

    // Serialized
    [Header("TERRAIN")]
    [Tooltip("Image component of the tile's terrain.")]
    [SerializeField] private Image _terrainImg;
    [Header("RESOURCES")]
    [Tooltip("Parent game object of resource images.")]
    [SerializeField] private Transform _resourceImgFolder;
    [Tooltip("Prefab of resource image.")]
    [SerializeField] private GameObject _resourceImgPrefab;

    /// <summary>
    /// Self implemented property that holds the Game Tile it represents.
    /// </summary>
    /// <value></value>
    public GameTile Tile { get; private set; }

    /// <summary>
    /// Public self implemented property that stores the cell's relative map position.
    /// </summary>
    /// <value>Relative map position. Ex: (0, 1).</value>
    public Vector2 MapPosition { get; private set; }

    // List containing actively displayed resource sprites.
    private List<Sprite> _activeRSpritesList;

    // Private pointer control variables.
    private Vector3 _mouseDownPos, _mouseClickDelta;

    // Private control for units selecting a destination cell and moving.
    private bool _moveSelecting, _moving;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelUnitsControl.OnMoveSelect += (p_selecting) => _moveSelecting = p_selecting;
        UIPanelUnitsControl.OnMoving += (p_moving) => { _moving = p_moving; };
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UIPanelUnitsControl.OnMoveSelect -= (p_selecting) => { };
        UIPanelUnitsControl.OnMoving -= (p_moving) => { };
    }

    /// <summary>
    /// Sets up the cell after being instantiated.
    /// </summary>
    /// <param name="p_tile">Game tile that the cell represents.</param>
    public void Initialize(GameTile p_tile, Vector2 p_mapPosition)
    {
        // Saves game tile reference.
        Tile = p_tile;

        // Saves relative map position.
        MapPosition = p_mapPosition;

        // Creates list of active displayed resources.
        _activeRSpritesList = new List<Sprite>();

        // Displays the resource sprites present in the game tile reference.
        UpdateResourceSprites();

        // Sets the terrain sprite as the base sprite.
        _terrainImg.sprite = Tile.Sprites[0];
    }

    /// <summary>
    /// Displays resources by instantiating individual image game objects 
    /// on top of the terrain and updating it's sprite.
    /// </summary>
    public void UpdateResourceSprites()
    {
        Image m_resourceImage;

        // Destroy any resource images that might be instantiated.
        foreach (Transform resourceImage in _resourceImgFolder)
            Destroy(resourceImage.gameObject);

        // Clears list of active displayed resources.
        _activeRSpritesList.Clear();

        // Iterate tile resources.
        for (int i = 0; i < Tile.Resources.Count; i++)
        {
            // Instantiates image for this resource and saves its image component.
            m_resourceImage = Instantiate(_resourceImgPrefab, _resourceImgFolder).
                GetComponent<Image>();

            // Updates it's sprite.
            m_resourceImage.sprite = Tile.Resources[i].SpritesDict[Tile.Name];

            // Saves it in a list.
            _activeRSpritesList.Add(m_resourceImage.sprite);
        }
    }

    /// <summary>
    /// Registers the time when a cell starts being clicked.
    /// This prevents cells from being miss-clicked after a drag selection.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell starts to be clicked, on pointer down,
    /// by IPointerDownHandler.
    /// </remarks>
    public void OnPointerDown(PointerEventData p_pointerData)
    {
        // Ignores method if units are moving.
        if (_moving) return;

        // Registers mouse click position.
        _mouseDownPos = Input.mousePosition;
    }

    /// <summary>
    /// Raises event that this cell has been clicked, so that inspector
    /// can be enabled.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell is LEFT clicked, by IPointerClickHandler.
    /// </remarks>
    public void OnPointerClick(PointerEventData p_pointerData)
    {
        // Ignores method if units are moving.
        if (_moving) return;

        // Calculates mouse drag since it was pressed down.
        _mouseClickDelta = Input.mousePosition - _mouseDownPos;

        // If clicked with the left mouse button and click wasn't dragged.
        if (p_pointerData.button == PointerEventData.InputButton.Left &&
            _mouseClickDelta.sqrMagnitude < 2)
        {
            // If units are trying to select next move destination.
            if (_moveSelecting)
            {
                // Targets this cell.
                OnTargeted?.Invoke(this);
            }

            // Otherwise inspects cell.
            else
            {
                OnInspectView?.Invoke();
                OnInspectData?.Invoke(Tile, _activeRSpritesList);
            }
        }
    }

    /// <summary>
    /// Sets the terrain sprite as it's hovered version, if the mouse isn't 
    /// being pressed.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell is hovered, by IPointerEnterHandler.
    /// </remarks>
    public void OnPointerEnter(PointerEventData p_pointerData)
    {
        // Ignores method if units are moving or if mouse was already being pressed.
        if (_moving || Input.GetMouseButton(0)) return;

        // Sets terrain sprite as hovered version.
        _terrainImg.sprite = Tile.Sprites[1];
    }

    /// <summary>
    /// Sets the terrain sprite as it's base version.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell stop's being hovered, by IPointerExitHandler.
    /// </remarks>
    public void OnPointerExit(PointerEventData p_pointerData)
    {
        // Ignores method if units are moving or if mouse was already being pressed.
        if (_moving || Input.GetMouseButton(0)) return;

        // Sets terrain sprite as normal version.
        _terrainImg.sprite = Tile.Sprites[0];
    }
}


