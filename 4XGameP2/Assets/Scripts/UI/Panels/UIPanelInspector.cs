using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel displayed in Inspector UI state.
/// </summary>
public class UIPanelInspector : UIPanel
{
    /// <summary>
    /// Constant string value for the opening animator trigger.
    /// </summary>
    private const string OPEN_TRIGGER = "Open";

    /// <summary>
    /// Constant string value for the closing animator trigger.
    /// </summary>
    private const string CLOSE_TRIGGER = "Close";

    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;
    [Header("IMAGES DISPLAY")]
    [Tooltip("Image component of tile terrain.")]
    [SerializeField] private Image _tileImage;
    [Tooltip("Image components of tile resources.")]
    [SerializeField] private Image[] _resourcesImages;
    [Header("GENERAL DISPLAY")]
    [Tooltip("Text component of tile name.")]
    [SerializeField] private TMP_Text _tileNameTxt;
    [Tooltip("Text component of tile resources count.")]
    [SerializeField] private TMP_Text _tileResourcesCountTxt;
    [Tooltip("Game objects of toggeable resources names.")]
    [SerializeField] private GameObject[] _resourcesNamesObjs;
    [Header("COIN DISPLAY")]
    [Tooltip("Text component of tile base coin.")]
    [SerializeField] private TMP_Text _tileBaseCoinTxt;
    [Tooltip("Text component of tile total coin.")]
    [SerializeField] private TMP_Text _tileTotalCoinTxt;
    [Tooltip("Game objects of toggeable resources coin values.")]
    [SerializeField] private GameObject[] _resourcesCoinObjs;
    [Header("FOOD DISPLAY")]
    [Tooltip("Text component of tile base food.")]
    [SerializeField] private TMP_Text _tileBaseFoodTxt;
    [Tooltip("Text component of tile total food.")]
    [SerializeField] private TMP_Text _tileTotalFoodTxt;
    [Tooltip("Game objects of toggeable resources food values.")]
    [SerializeField] private GameObject[] _resourcesFoodObjs;
    [Header("EMPTY GRID SPACES")]
    [Tooltip("Game objects of toggeable empty spaces.")]
    [SerializeField] private GameObject[] _emptySpaces;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable() => MapCell.OnInspectData += DisplayData;

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable() => MapCell.OnInspectData -= DisplayData;

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel() => ClosePanel();

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0)
    {
        // Reveals the panel.
        base.Open(p_transitionTime);

        // Activate opening trigger of sub-panel animator.
        _subPanelAnim.SetTrigger(OPEN_TRIGGER);
    }

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0)
    {
        // Activate closing trigger of sub-panel animator.
        _subPanelAnim.SetTrigger(CLOSE_TRIGGER);

        // Hides the panel.
        base.Close(p_transitionTime);

        // Disables empty grid spaces.
        for (int i = 0; i < _emptySpaces.Length; i++)
            _emptySpaces[i].SetActive(true);

        // Loops length of possible existing resources (6, for now).
        for (int i = 0; i < _resourcesImages.Length; i++)
        {
            // Disables sprite, name, coin and food data.
            _resourcesImages[i].gameObject.SetActive(false);
            _resourcesNamesObjs[i].SetActive(false);
            _resourcesCoinObjs[i].SetActive(false);
            _resourcesFoodObjs[i].SetActive(false);
        }
    }

    /// <summary>
    /// Updates displayed data based on the game tile cell clicked.
    /// </summary>
    /// <param name="p_tile">Game tile clicked.</param>
    /// <param name="p_baseSprite">Tile's terrain sprite.</param>
    /// <param name="p_resourceSprites">Tile's resources sprites.</param>
    private void DisplayData(GameTile p_tile, Sprite p_baseSprite, 
        Sprite[] p_resourceSprites)
    {
        // Displays tile name.
        _tileNameTxt.text = p_tile.Name;

        // Displays tile view sprite to match the base clicked cell.
        _tileImage.sprite = p_baseSprite;

        // Displays base coin and food values of the tile.
        _tileBaseCoinTxt.text = p_tile.BaseCoin.ToString();
        _tileBaseFoodTxt.text = p_tile.BaseFood.ToString();

        // Display total coin and food values of the tile.
        _tileTotalCoinTxt.text = p_tile.Coin.ToString();
        _tileTotalFoodTxt.text = p_tile.Food.ToString();

        // Displays the number of resources.
        _tileResourcesCountTxt.text = 
            ($"No. of resources: " + p_tile.Resources.Count.ToString());

        // If there are resources.
        if (p_tile.Resources.Count > 0)
        {
            // Enables empty grid spaces.
            for (int i = 0; i < _emptySpaces.Length; i++)
                _emptySpaces[i].SetActive(true);

            // Loops length of possible existing resources (6, for now).
            for (int i = 0; i < _resourcesImages.Length; i++)
            {
                // If this resource exists in the clicked cell.
                if (p_resourceSprites[i] != null)
                {
                    // Syncs sprites.
                    _resourcesImages[i].sprite = p_resourceSprites[i];

                    // Enables sprite, name, coin and food data.
                    _resourcesImages[i].gameObject.SetActive(true);
                    _resourcesNamesObjs[i].SetActive(true);
                    _resourcesCoinObjs[i].SetActive(true);
                    _resourcesFoodObjs[i].SetActive(true);
                }
            }
        }
    }
}
