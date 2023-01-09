using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Panel displayed when 1+ units are selected.
/// Contains info regarding selected units and action buttons.
/// </summary>
public class UIPanelUnitsControl : UIPanel
{
    /// <summary>
    /// Event raised when the 'REMOVE' units button is pressed.
    /// </summary>
    public static event Action<ICollection<Unit>> OnUnitsRemoved;

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
    [Tooltip("Limit of displayable icons.")]
    [SerializeField] private int _iconsDisplayedLimit;
    [Tooltip("Prefab of unit overflow count.")]
    [SerializeField] private GameObject _unitOverflowCount;
    [Header("UNIT RESOURCES")]
    [Tooltip("Parent game object where the resource counters are stored.")]
    [SerializeField] private Transform _resourceCountFolder;
    [Tooltip("Prefab of resource counter.")]
    [SerializeField] private GameObject _resourceCount;
    [Header("GAME DATA")]
    [Tooltip("Scriptable Object with Ongoing Game Data.")]
    [SerializeField] private OngoingGameDataSO _ongoingData;

    private List<Unit> _selectedUnits;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        UnitSelection.OnUnitsSelected += DisplayUnitsData;
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        UnitSelection.OnUnitsSelected -= DisplayUnitsData;
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
        _selectedUnits = new List<Unit>(p_selectedUnits);

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

        int m_iconsIndex = 0;

        // If there are more icons to display than there is space.
        if (p_selectedUnits.Count > _iconsDisplayedLimit)
        {
            m_iconsIndex = p_selectedUnits.Count - _iconsDisplayedLimit + 1;

            // Instantiates a counter in the first slot, which represents how 
            // many icons are hidden.
            Instantiate(_unitOverflowCount, _unitIconsFolder)
                .GetComponent<TMP_Text>().text = $"+{m_iconsIndex}";
        }

        // Displays most recent unit icons.
        for (int i = m_iconsIndex; i < p_selectedUnits.Count; i++)
        {
            // Instantiates unit icons.
            Instantiate(_unitIcon, _unitIconsFolder)
                .GetComponent<Image>().sprite = _selectedUnits[i].Icon;
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
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Called by the 'MOVE' Unity button, in this panel.
    /// </remarks>
    public void OnMoveButton()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Called by the 'HARVEST' Unity button, in this panel.
    /// </remarks>
    public void OnHarvestButton()
    {

    }

    /// <summary>
    /// Removes every selected unit from the ongoing data collection, destroys
    /// their game objects and raises event containing the units removed.
    /// </summary>
    /// <remarks>
    /// Called by the 'REMOVE' Unity button, in this panel.
    /// </remarks>
    public void OnRemoveButton()
    {
        foreach(Unit f_unit in _selectedUnits)
        {
            _ongoingData.RemoveUnit(f_unit);
            Destroy(f_unit.gameObject);
        }

        OnUnitsRemoved?.Invoke(_selectedUnits);
    }
}
