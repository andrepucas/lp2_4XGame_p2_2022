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

    // Serialized
    [Header("TERRAIN")]
    [Tooltip("Image component of the tile's terrain.")]
    [SerializeField] private Image _terrainImg;
    [Header("RESOURCES")]
    [Tooltip("Parent game object of resource images.")]
    [SerializeField] private Transform _resourceImgFolder;
    [Tooltip("Prefab of resource image.")]
    [SerializeField] private GameObject _resourceImgPrefab;

    // Reference to the game tile this cell represents.
    public GameTile Tile { get; private set; }

    // List containing actively displayed resource sprites.
    private List<Sprite> _activeRSpritesList;

    // Private pointer control variables.
    private Vector3 _mouseDownPos, _mouseClickDelta;

    /// <summary>
    /// Sets up the cell after being instantiated.
    /// </summary>
    /// <param name="p_tile">Game tile that the cell represents.</param>
    public void Initialize(GameTile p_tile)
    {
        // Saves game tile reference.
        Tile = p_tile;

        // Creates list of active displayed resources.
        _activeRSpritesList = new List<Sprite>();

        // Destroy any resource images that might be instantiated.
        foreach (Transform resourceImage in _resourceImgFolder)
            Destroy(resourceImage.gameObject);

        // Displays the resource sprites present in the game tile reference.
        EnableResourceSprites();

        // Sets the terrain sprite as the base sprite.
        _terrainImg.sprite = Tile.Sprites[0];
    }

    /// <summary>
    /// Displays resources by instantiating individual image game objects 
    /// on top of the terrain and updating it's sprite.
    /// </summary>
    private void EnableResourceSprites()
    {
        Image m_resourceImage;

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
    public void OnPointerDown(PointerEventData p_pointerData) =>
        _mouseDownPos = Input.mousePosition;

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
        _mouseClickDelta = Input.mousePosition - _mouseDownPos;

        // Only raises events if the clicked button is the left one and wasn't 
        // being dragged.
        if (p_pointerData.button == PointerEventData.InputButton.Left &&
            _mouseClickDelta.sqrMagnitude < 1)
        {
            OnInspectView?.Invoke();
            OnInspectData?.Invoke(Tile, _activeRSpritesList);
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
        if (!Input.GetKey(KeyCode.Mouse0))
            _terrainImg.sprite = Tile.Sprites[1];
    }

    /// <summary>
    /// Sets the terrain sprite as it's base version.
    /// </summary>
    /// <param name="p_pointerData">Pointer event data.</param>
    /// <remarks>
    /// Called when the cell stop's being hovered, by IPointerExitHandler.
    /// </remarks>
    public void OnPointerExit(PointerEventData p_pointerData) =>
        _terrainImg.sprite = Tile.Sprites[0];
}


