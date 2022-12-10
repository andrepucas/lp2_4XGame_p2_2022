using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Responsible for representing and manipulating a map file.
/// </summary>
/// <remarks>
/// Displayed in a scrollable list inside Maps Browser.
/// </remarks>
public class MapFileWidget : MonoBehaviour
{
    /// <summary>
    /// Event raised when anything in this widget is selected.
    /// Includes a self reference.
    /// </summary>
    public static event Action<MapFileWidget> OnSelected;

    /// <summary>
    /// Event raised when this widget is deleted.
    /// </summary>
    public static event Action OnDeleted;

    /// <summary>
    /// Readonly color for the normal state.
    /// </summary>
    /// <returns>The color that represents the normal state.</returns>
    private readonly Color32 NORMAL_COLOR = new Color32(255, 50, 100, 255);

    /// <summary>
    /// Readonly color for the selected state.
    /// </summary>
    /// <returns>The color that represents the selected state.</returns>
    private readonly Color32 SELECTED_COLOR = new Color32(255, 200, 100, 255);

    // Serialized variables.
    [Header("COMPONENTS")]
    [Tooltip("Button component that surround the entire widget.")]
    [SerializeField] private Button _widgetButton;
    [Tooltip("Input field component for the map name.")]
    [SerializeField] private TMP_InputField _nameInput;
    [Tooltip("Input field component for the map X dimensions.")]
    [SerializeField] private TMP_InputField _sizeXDisplay;
    [Tooltip("Input field component for the map Y dimensions.")]
    [SerializeField] private TMP_InputField _sizeYDisplay;
    [Tooltip("Button component to edit the map name.")]
    [SerializeField] private Button _editNameButton;

    /// <summary>
    /// Self-implemented property with a private set which holds the data of 
    /// the map this widget represents.
    /// </summary>
    /// <value></value>
    public MapData MapData {get; private set;}

    // Color block to temporarily hold and switch button colors.
    private ColorBlock _buttonColors;

    // Integer that controls if the name is being edited or not.
    private int _editNameToggleIndex;

    // String that holds the name before being modified.
    private string _nameBeforeEdit;

    /// <summary>
    /// Unity method, called on game start. Sets up default states.
    /// </summary>
    private void Start()
    {
        // Sets the name input field to non-interactable.
        _nameInput.interactable = false;

        // Sets the local color block to the edit name button colors.
        _buttonColors = _editNameButton.colors;

        // Updates edit name button colors to match the name input field state.
        UpdateEditNameButtonColors();

        // Sets the edit name toggle index to 0.
        _editNameToggleIndex = 0;
    }

    /// <summary>
    /// Saves map data and sets up default input field values.
    /// </summary>
    /// <param name="p_mapData">Map data that this widget represents.</param>
    public void Initialize(MapData p_mapData)
    {
        // Saves map data.
        MapData = p_mapData;

        // Sets up default input field values, based on map data.
        _nameInput.text = MapData.Name;
        _sizeXDisplay.text = MapData.XCols.ToString();
        _sizeYDisplay.text = MapData.YRows.ToString();
    }

    /// <summary>
    /// Updates colors of the edit name button manually, thus allowing it to look
    /// 'pressed' or 'selected' while the user edits the file name.
    /// </summary>
    private void UpdateEditNameButtonColors()
    {
        // If the name input field is active.
        if (_nameInput.interactable)
        {
            // Set local button colors as the selected color.
            _buttonColors.normalColor = SELECTED_COLOR;
            _buttonColors.pressedColor = SELECTED_COLOR;
            _buttonColors.selectedColor = SELECTED_COLOR;
        }

        // If its disabled.
        else 
        {
            // Set local button colors as the normal color.
            _buttonColors.normalColor = NORMAL_COLOR;
            _buttonColors.pressedColor = NORMAL_COLOR;
            _buttonColors.selectedColor = NORMAL_COLOR;
        }

        // Update the button colors.
        _editNameButton.colors = _buttonColors;
    }

    /// <summary>
    /// Manually selects this widget.
    /// </summary>
    public void Select()
    {
        // Disables button that covers the whole widget.
        _widgetButton.interactable = false;

        // Raises event that this widget is selected.
        OnSelected?.Invoke(this);
    }

    /// <summary>
    /// Manually de-selects this widget.
    /// </summary>
    public void DeSelect()
    {
        // Enables button that covers the whole widget.
        _widgetButton.interactable = true;
    }

    /// <summary>
    /// Disables and enables the name input field.
    /// </summary>
    /// <remarks>
    /// Called by the *pencil* Unity button, in this game object.
    /// </remarks>
    public void OnEditNameButton()
    {
        // Increments the edit name toggle index.
        _editNameToggleIndex++;

        // If its an odd number.
        if (_editNameToggleIndex % 2 != 0)
        {
            // If the input field is already enabled.
            if (_nameInput.interactable)
            {
                // Increments again, since it was just incremented on NameEdited()
                // but the user is trying to disable edit mode by pressing this button.
                _editNameToggleIndex++;

                // Makes Unity event system de-select everything.
                EventSystem.current.SetSelectedGameObject(null);
            }

            // If it isn't.
            else
            {
                // Enables it and manually selects it.
                _nameBeforeEdit = _nameInput.text;
                _nameInput.interactable = true;
                _nameInput.Select();
            }
        }

        // Updates edit name button colors to match the name input field state.
        UpdateEditNameButtonColors();
    }

    /// <summary>
    /// Deletes this file widget and it's corresponding map file.
    /// </summary>
    /// <remarks>
    /// Called by the 'X' Unity button, in this game object.
    /// </remarks>
    public void OnDeleteButton()
    {
        // Deletes map file.
        MapFilesBrowser.DeleteMapFile(MapData.Name);

        // Raises event that this widget as been deleted.
        OnDeleted?.Invoke();

        // Destroy this game object after 0.1s.
        Destroy(this.gameObject, .1f);
    }

    /// <summary>
    /// Validates the player's input name, updates the file and map data names, 
    /// and disables input.
    /// </summary>
    /// <remarks>
    /// Called by the name input field, on end edit. 
    /// This happens when the player clicks away, escape or enter.
    /// </remarks>
    public void OnNameEdited()
    {
        // If any change was made, validates new name 
        if (_nameBeforeEdit != _nameInput.text)
            _nameInput.text = MapFileNameValidator.Validate(_nameInput.text);

        // Updates file and map data names.
        MapFilesBrowser.RenameMapFile(MapData.Name, _nameInput.text);
        MapData.Name = _nameInput.text;

        // Increments the edit name toggle index.
        _editNameToggleIndex++;

        // Disables name input field after a short delay.
        StartCoroutine(DisableAfterDelay());
    }

    /// <summary>
    /// Waits some time before disabling the input field.
    /// </summary>
    /// <remarks>
    /// 1. Waiting prevents an event system conflict when disabling 
    /// the input and clicking away at the same time.
    /// 2. Allows the edit button method to detect it as enabled still, thus
    /// allowing it to be properly handled when the user tries to leave edit mode
    /// by pressing the button.
    /// </remarks>
    /// <returns>WaitForSeconds(.1f)</returns>
    private IEnumerator DisableAfterDelay()
    {
        // Waits 0.1s.
        yield return new WaitForSeconds(.1f);

        // Sets the name input as non-interactable.
        _nameInput.interactable = false;

        // Updates edit name button colors to match the name input field state.
        UpdateEditNameButtonColors();
    }

    /// <summary>
    /// Overrides GetHasCode() to consider the map data's name instead.
    /// </summary>
    /// <returns>Integer hash code of the map data's name.</returns>
    public override int GetHashCode() => MapData.Name.GetHashCode();

    /// <summary>
    /// Overrides Equals(object) to return true if both objects have the 
    /// same map data's name.
    /// </summary>
    /// <param name="p_obj">Other MapFileWidget</param>
    /// <returns>True if both map data's names are equal.</returns>
    public override bool Equals(object p_obj) => 
        (p_obj as MapFileWidget)?.MapData.Name == MapData.Name;
}
