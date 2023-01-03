using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a game tile.
/// </summary>
public class MapCell : MonoBehaviour
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

    /// <summary>
    /// Self implemented property that returns true if there's a Unit in the 
    /// game object.
    /// </summary>
    /// <typeparam name="Unit">Unit class.</typeparam>
    /// <returns>True if a unit is found in this game object.</returns>
    public bool HasUnit => GetComponentInChildren<Unit>();

    // Reference to the game tile this cell represents.
    private GameTile _tile;

    // List containing actively displayed resource sprites.
    private List<Sprite> _activeRSpritesList;

    /// <summary>
    /// Sets up the cell after being instantiated.
    /// </summary>
    /// <param name="p_tile">Game tile that the cell represents.</param>
    public void Initialize(GameTile p_tile)
    {
        // Saves game tile reference.
        _tile = p_tile;

        // Creates list of active displayed resources.
        _activeRSpritesList = new List<Sprite>();

        // Destroy any resource images that might be instantiated.
        foreach (Transform resourceImage in _resourceImgFolder)
            Destroy(resourceImage.gameObject);

        // Displays the resource sprites present in the game tile reference.
        EnableResourceSprites();

        // Sets the terrain sprite as the base sprite.
        OnPointerExit();
    }

    /// <summary>
    /// Sets the terrain sprite as it's hovered version.
    /// </summary>
    /// <remarks>
    /// Called when the cell is hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerEnter() => _terrainImg.sprite = _tile.Sprites[1];

    /// <summary>
    /// Sets the terrain sprite as it's base version.
    /// </summary>
    /// <remarks>
    /// Called when the cell stop's being hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerExit() => _terrainImg.sprite = _tile.Sprites[0];

    /// <summary>
    /// Raises event that this cell has been clicked, so that inspector
    /// can be enabled.
    /// </summary>
    /// <remarks>
    /// Called when the cell is clicked, by a Unity event trigger.
    /// </remarks>
    public void OnClick()
    {
        OnInspectView?.Invoke();
        OnInspectData?.Invoke(_tile, _activeRSpritesList);
    }

    /// <summary>
    /// Displays resources by instantiating individual image game objects 
    /// on top of the terrain and updating it's sprite.
    /// </summary>
    private void EnableResourceSprites()
    {
        Image m_resourceImage;

        // Iterate tile resources.
        for (int i = 0; i < _tile.Resources.Count; i++)
        {
            // Instantiates image for this resource and saves its image component.
            m_resourceImage = Instantiate(_resourceImgPrefab, _resourceImgFolder).
                GetComponent<Image>();

            // Updates it's sprite.
            m_resourceImage.sprite = _tile.Resources[i].SpritesDict[_tile.Name];

            // Saves it in a list.
            _activeRSpritesList.Add(m_resourceImage.sprite);
        }
    }
}


