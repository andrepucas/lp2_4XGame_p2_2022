using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIPanelUnits : UIPanel
{

    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;

    [Header("FOLDERS")]
    [Tooltip("Folder game object where the unit icons are stored.")]
    [SerializeField] private Transform _unitIconsFolder;

    [Tooltip("Folder game object where the resource counters are stored.")]
    [SerializeField] private Transform _resourceCountFolder;

    [Tooltip("Folder game object where unit quantity specific game objects are stored.")]
    [SerializeField] private Transform _unitTypeOrCountFolder;

    [Header("GAME OBJECTS")]
    [Tooltip("Game object that needs to be enabled if there's only 1 unit.")]
    [SerializeField] private GameObject _singleUnitObject;

    [Tooltip("Game object that needs to be enabled if there are various units.")]
    [SerializeField] private GameObject _multipleUnitsObject;

    [Header("PREFABS")]
    [Tooltip("Prefab of unit icon.")]
    [SerializeField] private Transform _unitIcon;

    [Tooltip("Prefab of resource counter.")]
    [SerializeField] private Transform _resourceCount;

    // Reference to data regarding the existing resources;
    private PresetResourcesData _resourceData;

    // Collection with all the selected units by the player-
    private ICollection<Unit> _selectedUnits = new List<Unit>();

    // Event that is raised when the unit UI panel is opened.
    public static event Action OnUnitsToDisplay;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable() => Unit.OnUnitView += DisplayUnitsData;

    private void OnDisable() => Unit.OnUnitView -= DisplayUnitsData;

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
        // Activate closing trigger of sub-panel animator.
        _subPanelAnim.SetBool("visible", false);

        // Hides the panel.
        base.Close(p_transitionTime);

        // Destroy all resource images that might be instantiated.
        foreach (Transform resourceImage in _unitIconsFolder)
            Destroy(resourceImage.gameObject);

        // Destroy all resource images that might be instantiated.
        foreach (Transform resourceImage in _resourceCountFolder)
            Destroy(resourceImage.gameObject);

        // Destroy all resource images that might be instantiated.
        foreach (Transform resourceImage in _unitTypeOrCountFolder)
            Destroy(resourceImage.gameObject);

        _selectedUnits.Clear();
    }

    /// <summary>
    /// Updates displayed data based on the units clicked.
    /// </summary>
    /// <param name="_selectedUnits">Collection of units selected by the player.</param>
    private void DisplayUnitsData(Unit m_unit, bool m_isSelected)
    {

        if (_selectedUnits.Count == 0 && m_isSelected == true)
        {
            _selectedUnits.Add(m_unit);
            OnUnitsToDisplay?.Invoke();
        }

        if (m_isSelected == true) _selectedUnits.Add(m_unit);

        else _selectedUnits.Remove(m_unit);


        // Checks if only one unit is selected.
        if (_selectedUnits.Count == 1)
        {
            Debug.Log("Yes");
            // Activates the appropriate game object for a single unit.
            _singleUnitObject.SetActive(true);

            // Changes the text to display the unit's name.
            _unitTypeOrCountFolder.GetComponent<TMP_Text>().text =
            _selectedUnits.ToList()[0].Name.ToString();

            // Instantiates a unit icon.
            Instantiate(_unitIcon, _unitIconsFolder.transform);
        }

        // Otherwise, the player selected multiple units.
        else
        {
            Debug.Log("Yes Yes");
            // Activates the appropriate game object for multiple units.
            _multipleUnitsObject.SetActive(true);

            // Goes through each selected unit
            foreach (Unit unit in _selectedUnits)
            {
                // Instantiates a unit icon for each unit selected.
                Instantiate(_unitIcon, _unitIconsFolder.transform);
            }

            // Updates the text to the total number of selected units.
            _unitTypeOrCountFolder.GetComponent<TMP_Text>().text =
            _selectedUnits.Count.ToString();
        }

        // Collection with all the unique resource types.
        IEnumerable<Resource> m_resourceTypes = _selectedUnits
        .SelectMany(r => r.Resources)
        .GroupBy(t => t.Name)
        .Select(t => t.First());

        // Goes through said types.
        foreach (Resource r in m_resourceTypes)
        {
            // Instantiates a resource counter.
            Transform f_currentCounter =
            Instantiate(_resourceCount, _resourceCountFolder.transform);

            // Updates the image of the resource counter to match the resource
            // type.
            f_currentCounter.GetComponentInChildren<Image>().sprite = r.DefaultSprite;

            // Updates the values of the quantity of resources.
            f_currentCounter.GetComponentInChildren<TMP_Text>().text =
            _selectedUnits.SelectMany(u => u.Resources)
            .Where(u => u.Name == r.Name)
            .Count()
            .ToString();
        }
    }

    private void Move()
    {

    }

    private void Harvest()
    {

    }

    private void Remove()
    {

    }
}
