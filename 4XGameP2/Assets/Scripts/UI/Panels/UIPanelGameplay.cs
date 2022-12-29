using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Panel displayed in Gameplay UI state. Mostly contains HUD.
/// </summary>
public class UIPanelGameplay : UIPanel
{
    /// <summary>
    /// Event raised when the the back to menu button is pressed.
    /// </summary>
    public static event Action OnRestart;

    // Serialized variables.
    [Header("RESOURCES COUNT")]
    [Tooltip("Parent game object of map resource's count.")]
    [SerializeField] private Transform _resourceCountFolder;
    [Tooltip("Prefab of individual map resource's count.")]
    [SerializeField] private GameObject _mapResourceCount;
    [Header("MAP DATA")]
    [Tooltip("Scriptable Object with all Map Tiles Data")]
    [SerializeField] private MapTilesDataSO _mapTilesData;

    /// Reference to MapData.
    private MapData _mapData;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        Debug.Log(1);
        MapDisplay.OnMapGenerated += SetUpCounters;
    }

    private void OnDisable()
    {
        Debug.Log(2);
        MapDisplay.OnMapGenerated -= SetUpCounters;
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        ClosePanel();

        // Destroys any existing visual resource count objects, inside it's folder.
        foreach (Transform f_child in _resourceCountFolder)
            Destroy(f_child.gameObject);

        // Iterates all possible resources' preset values.
        foreach (PresetValues f_rValue in _mapTilesData.Resources)
        {
            // Instantiates a visual resource count object and updates its sprite
            // to match the resource's default sprite.
            Instantiate(_mapResourceCount, _resourceCountFolder).
                GetComponentInChildren<Image>().sprite = f_rValue.DefaultResourceSprite;
        }
    }

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0) => base.Open(p_transitionTime);

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0) => base.Close(p_transitionTime);

    /// <summary>
    /// Updates local map data and counters.
    /// </summary>
    /// <param name="p_mapData">Local map data</param>
    public void SetUpCounters(MapData p_mapData)
    {
        _mapData = p_mapData;
        UpdateResourceCounters();
    }

    /// <summary>
    /// Updates resource counters.
    /// </summary>
    private void UpdateResourceCounters()
    {
        // Variable that dictates which name to access.
        int m_nameIndex = 0;

        // Goes through each counter.
        foreach (Transform f_counter in _resourceCountFolder)
        {
            // Stores the TMP component.
            TMP_Text f_textComponent = f_counter.GetComponentInChildren<TMP_Text>();

            // Updates text to display number of said resources on the map.
            f_textComponent.text = _mapData.GameTiles
            .SelectMany(t => t.Resources)
            .Where(r => (r.Name.ToLower().Replace(" ", ""))
            .Equals(_mapTilesData.ResourceNames.ToList()[m_nameIndex]))
            .Count().ToString();

            // Increases the variable so the next name is accessed.
            m_nameIndex++;
        }
    }

    /// <summary>
    /// Raises OnRestart event.
    /// </summary>
    /// <remarks>
    /// Called by the 'BACK TO MENU' Unity button, in this panel.
    /// </remarks>
    public void OnBackToMenuButton() => OnRestart?.Invoke();
}
