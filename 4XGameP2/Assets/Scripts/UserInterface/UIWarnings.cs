using UnityEngine;

/// <summary>
/// Handles UI warnings regarding invalid files.
/// </summary>
public class UIWarnings : MonoBehaviour
{
    // Serialized variables.
    [Header("MAP BROWSER")]
    [Tooltip("Warning displayed when files are hidden.")]
    [SerializeField] private GameObject _hiddenFilesWarning;
    [Tooltip("Warning displayed when files can't be loaded.")]
    [SerializeField] private GameObject _invalidFilesWarning;

    /// <summary>
    /// Clears warnings.
    /// </summary>
    public void ClearAll()
    {
        ClearHiddenFilesWarning();
        ClearInvalidFilesWarning();
    }

    /// <summary>
    /// Enables hidden files warning game object.
    /// </summary>
    public void DisplayHiddenFilesWarning() => _hiddenFilesWarning.SetActive(true);

    /// <summary>
    /// Disables hidden files warning game object.
    /// </summary>
    public void ClearHiddenFilesWarning() => _hiddenFilesWarning.SetActive(false);

    /// <summary>
    /// Enables invalid files warning game object.
    /// </summary>
    public void DisplayInvalidFilesWarning() => _invalidFilesWarning.SetActive(true);

    /// <summary>
    /// Disables invalid files warning game object.
    /// </summary>
    public void ClearInvalidFilesWarning() => _invalidFilesWarning.SetActive(false);
}
