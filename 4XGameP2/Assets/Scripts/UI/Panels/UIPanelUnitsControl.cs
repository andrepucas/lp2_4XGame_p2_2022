using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIPanelUnitsControl : UIPanel
{
    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;
    [Header("UNIT'S TYPE/COUNT")]
    [Tooltip("Text component where the type of the single selected unit or " +
        "number of overall selected units is displayed.")]
    [SerializeField] private TMP_Text _unitTypeOrCountTxt;
    [Tooltip("Text component that follows up the one before. Displays " +
        "\"UNIT(S) SELECTED\".")]
    [SerializeField] private TMP_Text _unitOrUnitsSelectedTxt;
    [Header("UNIT ICONS")]
    [Tooltip("Parent game object where the unit icons are stored.")]
    [SerializeField] private Transform _unitIconsFolder;
    [Tooltip("Prefab of unit icon.")]
    [SerializeField] private GameObject _unitIcon;
    [Header("UNIT RESOURCES")]
    [Tooltip("Parent game object where the resource counters are stored.")]
    [SerializeField] private Transform _resourceCountFolder;
    [Tooltip("Prefab of resource counter.")]
    [SerializeField] private GameObject _resourceCount;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UIPanelGameplay.OnNewSelectedUnits += DisplayUnitsData;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UIPanelGameplay.OnNewSelectedUnits += DisplayUnitsData;
    }

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

        // Activates opening trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", true);
    }

    /// <summary>
    /// Hides panel and destroys all the visual unit info.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0)
    {
        // Activate closing trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", false);

        // Hides the panel.
        base.Close(p_transitionTime);

        // Destroys instantiated objects.
        DestroyPrefabs();
    }

    /// <summary>
    /// Destroys instantiated unit icons + resource count prefabs.
    /// </summary>
    private void DestroyPrefabs()
    {
        // Destroys all unit icons that might be instantiated.
        foreach (Transform unitIcon in _unitIconsFolder)
            Destroy(unitIcon.gameObject);

        // Destroy all resource counts that might be instantiated.
        foreach (Transform resourceCount in _resourceCountFolder)
            Destroy(resourceCount.gameObject);
    }

    /// <summary>
    /// Displays data based on the units selected. 
    /// Reveals & Hides panel if no units are selected.
    /// </summary>
    /// <param name="_selectedUnits"></param>
    private void DisplayUnitsData(ICollection<Unit> p_selectedUnits)
    {
        // Destroy left-over instantiated prefabs.
        DestroyPrefabs();

        // If there's only one unit selected.
        if (p_selectedUnits.Count == 1)
        {
            _unitTypeOrCountTxt.text = p_selectedUnits.First().Name.ToUpper();
            _unitOrUnitsSelectedTxt.text = "UNIT SELECTED";
        }

        // If there's more than one unit selected.
        else
        {
            _unitTypeOrCountTxt.text = p_selectedUnits.Count.ToString();
            _unitOrUnitsSelectedTxt.text = "UNITS SELECTED";
        }

        Image m_unitIconImg;

        // Displays each unit's icon.
        foreach (Unit f_unit in p_selectedUnits)
        {
            m_unitIconImg = Instantiate(_unitIcon, _unitIconsFolder).GetComponent<Image>();
            m_unitIconImg.sprite = f_unit.Icon;
        }

        // Collection with all unique resource types, across all selected units.
        IEnumerable<Resource> m_resourceTypes = p_selectedUnits
            .SelectMany(r => r.Resources)
            .GroupBy(t => t.Name)
            .Select(t => t.First());

        GameObject m_rCounter;
        
        // For each unique resource.
        foreach (Resource r in m_resourceTypes)
        {
            // Instantiates a resource counter.
            m_rCounter = Instantiate(_resourceCount, _resourceCountFolder);

            // Updates the image of the resource counter to match the resource
            // type.
            m_rCounter.GetComponentInChildren<Image>().sprite = r.DefaultSprite;

            // Updates the counter value, with the number of this resource 
            // across all selected units.
            m_rCounter.GetComponentInChildren<TMP_Text>().text = p_selectedUnits
                .SelectMany(u => u.Resources)
                .Count(u => u.Name == r.Name)
                .ToString();
        }

        // If this panel is still closed, reveal it.
        if (!_subPanelAnim.GetBool("visible")) OpenPanel();
    }
}
