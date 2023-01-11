using UnityEngine;

/// <summary>
/// Panel displayed on Map Display UI state. Contains overlaying units.
/// </summary>
public class UIPanelUnitsDisplay : UIPanel
{
    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        ClosePanel();

        // Destroy any existing units.
        foreach (Transform f_child in transform)
            Destroy(f_child.gameObject);
    }

    /// <summary>
    /// Reveals panel.
    /// </summary>
    /// <param name="p_transitionTime">Reveal time (s).</param>
    public void OpenPanel(float p_transitionTime = 0) =>
        base.Open(p_transitionTime);

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0) =>
        base.Close(p_transitionTime);
}
