using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Panel displayed in Inspector UI state.
/// </summary>
public class UIPanelInspector : UIPanel
{
    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;
    [Header("IMAGES")]
    [Tooltip("Image component of tile terrain.")]
    [SerializeField] private Image _tileImage;
    [Tooltip("Parent game object of resource images.")]
    [SerializeField] private Transform _resourceImagesFolder;
    [Tooltip("Prefab of resource image.")]
    [SerializeField] private GameObject _resourceImagePrefab;
    [Header("DATA")]
    [Tooltip("Parent game object of data.")]
    [SerializeField] private Transform _dataFolder;
    [Tooltip("Text component of tile name.")]
    [SerializeField] private TMP_Text _tileNameTxt;
    [Tooltip("Text component of tile resources count.")]
    [SerializeField] private TMP_Text _tileResourcesCountTxt;
    [Tooltip("Prefab of resource inspect data.")]
    [SerializeField] private GameObject _rInspectPrefab;
    [Tooltip("Toggleable empty space.")]
    [SerializeField] private GameObject _emptySpace;
    [Header("COIN")]
    [Tooltip("Text component of tile base coin.")]
    [SerializeField] private TMP_Text _tileBaseCoinTxt;
    [Tooltip("Text component of tile total coin.")]
    [SerializeField] private TMP_Text _tileTotalCoinTxt;
    [Header("FOOD")]
    [Tooltip("Text component of tile base food.")]
    [SerializeField] private TMP_Text _tileBaseFoodTxt;
    [Tooltip("Text component of tile total food.")]
    [SerializeField] private TMP_Text _tileTotalFoodTxt;

    // Stores the visual components regarding resources.  
    private List<GameObject> _displayedRData;

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
    public void SetupPanel()
    {
        _displayedRData = new List<GameObject>();
        ClosePanel();
    }

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0)
    {
        // Reveals the panel.
        base.Open(p_transitionTime);

        // Activate opening trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", true);
    }

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0)
    {
        // Activates closing trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", false);

        // Hides the panel.
        base.Close(p_transitionTime);

        // Disables empty grid space.
        _emptySpace.SetActive(false);

        // Destroys all resource data that might be instantiated.
        foreach (GameObject resourceData in _displayedRData)
            Destroy(resourceData);

        // Destroys all resource images that might be instantiated.
        foreach (Transform resourceImage in _resourceImagesFolder)
            Destroy(resourceImage.gameObject);

        // Clears resources visual components in inspector.
        _displayedRData.Clear();
    }

    /// <summary>
    /// Updates displayed data based on the game tile cell clicked.
    /// </summary>
    /// <param name="p_tile">Game tile clicked.</param>
    /// <param name="p_resourceSprites">Tile's resources sprites.</param>
    private void DisplayData(GameTile p_tile, 
        List<Sprite> p_resourceSprites)
    {
        // Displays tile name.
        _tileNameTxt.text = p_tile.Name;

        // Displays tile view sprite to match the base clicked cell.
        _tileImage.sprite = p_tile.Sprites[0];

        // Displays base coin and food values of the tile.
        _tileBaseCoinTxt.text = p_tile.BaseCoin.ToString();
        _tileBaseFoodTxt.text = p_tile.BaseFood.ToString();

        // Displays total coin and food values of the tile.
        _tileTotalCoinTxt.text = p_tile.Coin.ToString();
        _tileTotalFoodTxt.text = p_tile.Food.ToString();

        // Displays the number of resources.
        _tileResourcesCountTxt.text = 
            ($"No. of resources: " + p_tile.Resources.Count.ToString());

        // If there are resources.
        if (p_tile.Resources.Count > 0)
        {
            // Stores the current resource's game object.
            GameObject m_resourceDataObj;

            // Stores text components of data object.
            TMP_Text[] m_textData;

            // Stores the current resource.
            Resource m_resource;
            
            // Enables empty grid space.
            _emptySpace.SetActive(true);

            // Iterates existing resources.
            for (int i = 0; i < p_tile.Resources.Count; i++)
            {
                // Instantiates and adds a Resource Data Object to grid.
                m_resourceDataObj = Instantiate(_rInspectPrefab, _dataFolder);
                _displayedRData.Add(m_resourceDataObj);

                // Updates sprite to match the resource's default sprite.
                m_resourceDataObj.GetComponentInChildren<Image>().sprite = 
                    p_tile.Resources[i].DefaultSprite;

                // Accesses text components of data object (name, coin and food).
                m_textData = m_resourceDataObj.GetComponentsInChildren<TMP_Text>();

                // Temporarily holds resource reference.
                m_resource = p_tile.Resources[i];

                // Displays resource's name, coin and food values.
                m_textData[0].text = m_resource.Name.ToUpper();
                m_textData[1].text = m_resource.Coin.ToString("+ 0;- 0;0");
                m_textData[2].text = m_resource.Food.ToString("+ 0;- 0;0");

                // Instantiates and updates resource sprite.
                Instantiate(_resourceImagePrefab, _resourceImagesFolder).
                    GetComponent<Image>().sprite = p_resourceSprites[i];
            }
        }
    }
}
