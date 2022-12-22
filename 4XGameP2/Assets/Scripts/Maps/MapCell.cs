using System;
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
    /// associated game tile, its terrain sprite and its resources sprites.
    /// </summary>
    public static event Action<GameTile, Sprite, Sprite[]> OnInspectData;

    // Serialized
    [Header("TERRAIN")]
    [Tooltip("Image component of the tile's terrain.")]
    [SerializeField] private Image _image;
    [Tooltip("Sprite of the tile's terrain.")]
    [SerializeField] private Sprite _baseSprite;
    [Tooltip("Sprite of the tile's terrain, when hovered.")]
    [SerializeField] private Sprite _hoveredSprite;
    [Header("RESOURCES")]
    [Tooltip("Game objects that hold each resource image.")]
    [SerializeField] private GameObject[] _resourceObjs;
    [Tooltip("Image components of each resource.")]
    [SerializeField] private Image[] _resourceImages;

    // Reference to the game tile this cell represents.
    private GameTile _tile;

    // Array of resource sprites that this cell is displaying.
    private Sprite[] _activeResourceSprites;

    /// <summary>
    /// Sets up the cell after being instantiated.
    /// </summary>
    /// <param name="p_tile">Game tile that the cell represents.</param>
    public void Initialize(GameTile p_tile)
    {
        // Creates an empty array with slots for all possible resource sprites.
        _activeResourceSprites = new Sprite[6];

        // Saves game tile reference.
        _tile = p_tile;

        // Sets the terrain sprite as the base sprite.
        OnPointerExit();

        // Disable all resource game object components, hiding sprites.
        for (int i = 0; i < _resourceObjs.Length; i++)
            _resourceObjs[i].SetActive(false);

        // Displays the resource sprites present in the game tile reference.
        EnableResourceSprites();
    }

    /// <summary>
    /// Sets the terrain sprite as it's hovered version.
    /// </summary>
    /// <remarks>
    /// Called when the cell is hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerEnter() => _image.sprite = _hoveredSprite;

    /// <summary>
    /// Sets the terrain sprite as it's base version.
    /// </summary>
    /// <remarks>
    /// Called when the cell stop's being hovered, by a Unity event trigger.
    /// </remarks>
    public void OnPointerExit() => _image.sprite = _baseSprite;

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
        OnInspectData?.Invoke(_tile, _baseSprite, _activeResourceSprites);
    }

    /// <summary>
    /// Displays the game tile's resources in this cell by enabling individual
    /// game objects with image components for each one, on top of the terrain
    /// image.
    /// </summary>
    private void EnableResourceSprites()
    {
        // Iterates every resource in the referenced tile.
        foreach (Resource resource in _tile.Resources)
        {
            // Checks it's type.
            switch (resource.Name)
            {
                case "Animals":

                    // Enables game object that corresponds to the animals image.
                    _resourceObjs[0].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[0] = _resourceImages[0].sprite;

                    break;

                case "Fossil Fuel":

                    // Enables game object that corresponds to the fossil fuel image.
                    _resourceObjs[1].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[1] = _resourceImages[1].sprite;

                    break;

                case "Luxury":

                    // Enables game object that corresponds to the luxury image.
                    _resourceObjs[2].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[2] = _resourceImages[2].sprite;

                    break;

                case "Metals":

                    // Enable game object that corresponds to the metals image.
                    _resourceObjs[3].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[3] = _resourceImages[3].sprite;

                    break;

                case "Plants":

                    // Enable game object that corresponds to the plants image.
                    _resourceObjs[4].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[4] = _resourceImages[4].sprite;

                    break;

                case "Pollution":

                    // Enable game object that corresponds to the pollution image.
                    _resourceObjs[5].SetActive(true);

                    // Saves it's sprite as an active sprite.
                    _activeResourceSprites[5] = _resourceImages[5].sprite;

                    break;
            }
        }
    }
}


