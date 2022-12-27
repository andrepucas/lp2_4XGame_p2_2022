using System;
using UnityEngine;
using TMPro;
using System.Linq;

/// <summary>
/// Panel displayed in Gameplay UI state. Mostly contains HUD.
/// </summary>
public class UIPanelGameplay : UIPanel
{

    [SerializeField] private MapTilesDataSO _mapTilesData;
    [SerializeField] private GameObject _mapResourceStat;
    [SerializeField] private GameObject _resourcesStatLayout;
    private MapData _mapData;

    /// <summary>
    /// Event raised when the the back to menu button is pressed.
    /// </summary>
    public static event Action OnRestart;

    private void OnDisable()
    {
        
    }

    /// <summary>
    /// Sets up panel.
    /// </summary>
    public void SetupPanel()
    {
        foreach (String s in _mapTilesData.ResourceNames)
        {
            Instantiate(_mapResourceStat, _resourcesStatLayout.transform);

            // Mudar o text para o count de todos os recursos desse tipo
            
        }

        ClosePanel();
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
    /// Raises OnRestart event.
    /// </summary>
    /// <remarks>
    /// Called by the 'BACK TO MENU' Unity button, in this panel.
    /// </remarks>
    public void OnBackToMenuButton() => OnRestart?.Invoke();
}
