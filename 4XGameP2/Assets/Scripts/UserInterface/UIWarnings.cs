using UnityEngine;
using TMPro;

public class UIWarnings : MonoBehaviour
{
    [Header("MAP BROWSER")]
    [Tooltip("Warning displayed when files are hidden.")]
    [SerializeField] private GameObject _hiddenFilesWarning;
    [Tooltip("Warning displayed when files can't be loaded.")]
    [SerializeField] private GameObject _invalidFilesWarning;

    public void ClearAll()
    {
        ClearHiddenFilesWarning();
        ClearInvalidFilesWarning();
    }

    public void DisplayHiddenFilesWarning() => _hiddenFilesWarning.SetActive(true);

    public void ClearHiddenFilesWarning() => _hiddenFilesWarning.SetActive(false);

    public void DisplayInvalidFilesWarning() => _invalidFilesWarning.SetActive(true);

    public void ClearInvalidFilesWarning() => _invalidFilesWarning.SetActive(false);
}
