using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Panel displayed in Map Browser UI state.
/// </summary>
public class UIPanelMapBrowser : UIPanel
{
    /// <summary>
    /// Event raised when the load button is pressed. Includes map data.
    /// </summary>
    public static event Action<MapData> OnLoad;

    // Serialized variables.
    [Header("SCROLL WIDGETS")]
    [Tooltip("Parent game object for all widgets.")]
    [SerializeField] private Transform _widgetsFolder;
    [Tooltip("Map File Widget Prefab.")]
    [SerializeField] private GameObject _mapFileWidget;
    [Tooltip("Map File Generator Widget Prefab.")]
    [SerializeField] private GameObject _mapFileGeneratorWidget;
    [Header("BUTTONS DATA")]
    [Tooltip("Refresh time text component.")]
    [SerializeField] private TMP_Text _refreshTimeText;
    [Tooltip("Load button rect transform component.")]
    [SerializeField] private RectTransform _loadButtonRect;
    [Tooltip("Load button text component.")]
    [SerializeField] private TMP_Text _loadButtonText;
    [Header("AUXILIAR TEXT")]
    [Tooltip("Text displayed when there are 1+ invalid files not being displayed")]
    [SerializeField] private GameObject _invalidFilesMsg;

    // List of instantiated map file widgets.
    private List<MapFileWidget> _widgetsList;

    // Last map file widget selected.
    private MapFileWidget _lastWidgetSelected;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable()
    {
        MapFileWidget.OnSelected += UpdateLastSelected;
        MapFileWidget.OnDeleted += () => InstantiateMapFileWidgets();
        MapFileGeneratorWidget.OnNewMapFile += (p_name) => InstantiateMapFileWidgets(p_name);
    }

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable()
    {
        MapFileWidget.OnSelected -= UpdateLastSelected;
        MapFileWidget.OnDeleted -= () => InstantiateMapFileWidgets();
        MapFileGeneratorWidget.OnNewMapFile -= (p_name) => InstantiateMapFileWidgets(p_name);
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        // Closes panel.
        ClosePanel();

        // Sets up variables. 
        _widgetsList = new List<MapFileWidget>();
        _lastWidgetSelected = null;
    }

    /// <summary>
    /// Reveals panel and instantiates map file widgets.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0)
    {
        // Reveals the panel.
        base.Open(p_transitionTime);

        // Instantiates map file widgets of existing files.
        InstantiateMapFileWidgets();
    }

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0) => 
        base.Close(p_transitionTime);

    /// <summary>
    /// Instantiates map file widgets, based on existing map files.
    /// </summary>
    /// <param name="p_newWidgetName">Widget that was just created with 
    /// default empty value.</param>
    private void InstantiateMapFileWidgets(string p_newWidgetName = "")
    {
        // Widget reference.
        MapFileWidget m_fileWidget;

        // Destroys any existing widgets in the set parent game object.
        foreach (Transform f_widget in _widgetsFolder)
            GameObject.Destroy(f_widget.gameObject);

        // Clears widgets list and invalid files message.
        _widgetsList.Clear();
        _invalidFilesMsg.SetActive(false);

        // If map files exist.
        if (MapFilesBrowser.GetMapsList().Count > 0)
        {
            // Iterates all Map Data gotten from existing files.
            foreach (MapData f_map in MapFilesBrowser.GetMapsList())
            {
                // If the map couldn't convert it's file dimensions, it's invalid
                // and should not be displayed.
                if (f_map.YRows == 0)
                {
                    // Display msg saying invalid files are being hidden.
                    _invalidFilesMsg.SetActive(true);

                    // Ignore this map.
                    continue;
                }
                
                // Instantiates a new map file widget.
                m_fileWidget = Instantiate(_mapFileWidget, Vector3.zero, 
                    Quaternion.identity, _widgetsFolder).GetComponent<MapFileWidget>();

                // Initializes it and adds to list.
                m_fileWidget.Initialize(f_map);
                _widgetsList.Add(m_fileWidget);
            }

            if (_widgetsList.Count != 0)
            {
                // Pre select widget based on the widget name parameter.
                if (p_newWidgetName == "") PreSelectWidget();
                else PreSelectWidget(p_newWidgetName);
            }

            else Debug.Log("No valid map files.");
        }

        // Else there are no map files.
        else Debug.Log("No map files.");

        // Instantiates map file generator widget at the end.
        Instantiate(_mapFileGeneratorWidget, Vector3.zero, 
            Quaternion.identity).transform.SetParent(_widgetsFolder, false);

        // Updates refreshed time.
        DisplayCurrentTime();
    }

    /// <summary>
    /// Tries to find and select the last selected widget.
    /// </summary>
    private void PreSelectWidget()
    {
        // Sets a temporary widget reference to the first of the list,
        // in case the last widget selected can't be found.
        MapFileWidget m_widget = _widgetsList[0];

        // Only looks for the last widget selected if there was one.
        if (_lastWidgetSelected != null)
        {
            // Iterates the instantiated widgets list.
            foreach(MapFileWidget f_widget in _widgetsList)
            {
                // If the widget is found.
                if (f_widget.Equals(_lastWidgetSelected))
                {
                    // Sets it as the temporary widget variable.
                    m_widget = f_widget;
                    break;
                }
            }
        }

        // Selects the widget.
        m_widget.Select();
    }

    /// <summary>
    /// Selects a widget based on it's name (widget just created by the user).
    /// </summary>
    /// <param name="p_newWidgetName">Name of the widget.</param>
    private void PreSelectWidget(string p_newWidgetName)
    {
        // Iterates the instantiated widgets list.
        foreach(MapFileWidget f_widget in _widgetsList)
        {
            // IIf the widget's name is found.
            if (f_widget.MapData.Name == p_newWidgetName)
            {
                // Selects it.
                f_widget.Select();
                break;
            }
        }
    }

    /// <summary>
    /// Display the current time as HH:MM:SS, in the refreshed time text.
    /// </summary>
    private void DisplayCurrentTime()
    {
        _refreshTimeText.text = ("Last updated at " + 
            System.DateTime.Now.Hour.ToString("D2") + ":" + 
            System.DateTime.Now.Minute.ToString("D2") + ":" + 
            System.DateTime.Now.Second.ToString("D2"));
    }

    /// <summary>
    /// Updates the last selected widget.
    /// </summary>
    /// <param name="p_selectedWidget">Widget that was just selected.</param>
    private void UpdateLastSelected(MapFileWidget p_selectedWidget)
    {
        // If another widget is already selected, de-select it.
        if (_lastWidgetSelected != null && _lastWidgetSelected != p_selectedWidget) 
            _lastWidgetSelected.DeSelect();

        // Updates the variable.
        _lastWidgetSelected = p_selectedWidget;

        // Updates the load button with newly selected widget's name.
        StartCoroutine(UpdateLoadButton(_lastWidgetSelected.MapData.Name));
    }

    /// <summary>
    /// Updates the load button's text and size.
    /// </summary>
    /// <param name="p_mapName">Map name to add to button.</param>
    /// <returns>Waits for the end of the frame.</returns>
    private IEnumerator UpdateLoadButton(string p_mapName)
    {
        // Auxiliar vector 2 with the load button width and height.
        Vector2 m_loadButtonSize = _loadButtonRect.sizeDelta;

        // Updates load button text.
        _loadButtonText.text = "LOAD " + p_mapName.ToUpper();

        // Waits for the text bounds to adjust with the content size fitter component.
        yield return new WaitForEndOfFrame();

        // Updates the button width, based on the new text bounds.
        m_loadButtonSize.x = _loadButtonText.textBounds.size.x + 15;
        _loadButtonRect.sizeDelta = m_loadButtonSize;
    }

    /// <summary>
    /// Re-instantiates the displayed widgets.
    /// </summary>
    /// <remarks>
    /// Called by the 'Refresh' Unity button, in this panel.
    /// </remarks>
    public void OnRefreshButton()
    {
        // Instantiates the map widgets.
        InstantiateMapFileWidgets();

        // De-select anything that might be selected.
        EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Raises on load event, sending out the selected map data.
    /// </summary>
    /// <remarks>
    /// Called by the 'LOAD MAP' Unity button, in this panel.
    /// </remarks>
    public void OnLoadButton() => OnLoad?.Invoke(_lastWidgetSelected.MapData);
}
