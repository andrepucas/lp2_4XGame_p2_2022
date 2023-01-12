using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;

/// <summary>
/// Responsible for getting the input necessary to generate a new map file.
/// </summary>
/// <remarks>
/// Displayed at the bottom of a scrollable list in Maps Browser.
/// </remarks>
public class MapFileGeneratorWidget : MonoBehaviour
{
    /// <summary>
    /// Event raised when a map file is generated. Includes the file name.
    /// </summary>
    public static event Action<string> OnNewMapFile;

    // Serialized variables.
    [Header("MAP GENERATION DATA")]
    [Tooltip("Scriptable object with terrains and resources used for map generation.")]
    [SerializeField] private PresetGameDataSO _generateData;
    [Header("NAME INPUT")]
    [Tooltip("Input field component for the map name.")]
    [SerializeField] private TMP_InputField _nameInput;
    [Tooltip("Placeholder text displayed by the name input field.")]
    [SerializeField] private TMP_Text _placeholderName;
    [Header("X DIMENSIONS INPUT")]
    [Tooltip("Input field component for the map X dimensions.")]
    [SerializeField] private TMP_InputField _sizeXInput;
    [Tooltip("Placeholder text displayed by the X dimensions input field.")]
    [SerializeField] private TMP_Text _placeholderSizeX;
    [Header("Y DIMENSIONS INPUT")]
    [Tooltip("Input field component for the map Y dimensions.")]
    [SerializeField] private TMP_InputField _sizeYInput;
    [Tooltip("Placeholder text displayed by the Y dimensions input field.")]
    [SerializeField] private TMP_Text _placeholderSizeY;

    // String that holds the name for the generated map.
    private string _name;

    // Integer that holds the X dimensions for the generated map.
    private int _xCols;

    // Integer that holds the Y dimensions for the generated map.
    private int _yRows;

    /// <summary>
    /// Unity method, called on game start. Sets up default values.
    /// </summary>
    private void Start()
    {
        // Saves the placeholders' text as default generation values.
        _name = _placeholderName.text;
        _xCols = Int32.Parse(_placeholderSizeX.text);
        _yRows = Int32.Parse(_placeholderSizeY.text);
    }

    /// <summary>
    /// Validates the player's input name.
    /// </summary>
    /// <remarks>
    /// Called by the name input field, on end edit. 
    /// This happens when the player clicks away, escape or enter.
    /// </remarks>
    public void OnNameEdited() =>
        _nameInput.text = MapFileNameValidator.Validate(_nameInput.text);

    /// <summary>
    /// Gathers all the player's input and generates a new map file.
    /// </summary>
    /// <remarks>
    /// Called by the '+' Unity button, in this game object.
    /// </remarks>
    public void OnAddButton()
    {
        // If player specified a name for the map file. Saves it.
        if (_nameInput.text != "") _name = _nameInput.text;

        // If default name wasn't changed. Validates it.
        else _name = MapFileNameValidator.Validate(_name);

        // If player specified a value for the map X dimensions. Saves it.
        if (_sizeXInput.text != "") _xCols = Int32.Parse(_sizeXInput.text);

        // If player specified a value for the map Y dimensions. Saves it.
        if (_sizeYInput.text != "") _yRows = Int32.Parse(_sizeYInput.text);

        // Generates the new map file.
        MapFilesBrowser.GenerateNewMapFile(_name, _yRows, _xCols, _generateData);

        // Raises event that a new map has been generated, with its name.
        OnNewMapFile?.Invoke(_name);

        // De-selects anything that might be selected.
        EventSystem.current.SetSelectedGameObject(null);
    }
}
