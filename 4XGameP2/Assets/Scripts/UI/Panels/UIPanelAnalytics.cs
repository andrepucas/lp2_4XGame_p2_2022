using UnityEngine;
using TMPro;
using System.Linq;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// Panel displayed in Analytics UI state.
/// </summary>
public class UIPanelAnalytics : UIPanel
{
    /// <summary>
    /// Constant string value for the opening animator trigger.
    /// </summary>
    private const string OPEN_TRIGGER = "Open";

    /// <summary>
    /// Constant string value for the closing animator trigger.
    /// </summary>
    private const string CLOSE_TRIGGER = "Close";

    /// <summary>
    /// Constant float value for the answer font size, if numerical.
    /// </summary>
    private const float FONT_SIZE_NUM = 100f;

    /// <summary>
    /// Constant float value for the answer font size, if string.
    /// </summary>
    private const float FONT_SIZE_STRING = 50f;

    // Serialized variables.
    [Header("ANIMATOR")]
    [Tooltip("Animator component of info sub-panel.")]
    [SerializeField] private Animator _subPanelAnim;
    [Header("TEXT DISPLAY")]
    [Tooltip("Text component of sub-panel data title.")]
    [SerializeField] private TMP_Text _titleTxt;
    [Tooltip("Text component of sub-panel data answer.")]
    [SerializeField] private TMP_Text _answerTxt;

    /// <summary>
    /// Unity method, on enable, subscribes to events.
    /// </summary>
    private void OnEnable() => Controller.OnAnalytics += DisplayData;

    /// <summary>
    /// Unity method, on disable, unsubscribes to events.
    /// </summary>
    private void OnDisable() => Controller.OnAnalytics -= DisplayData;

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

        // Activate opening trigger of sub-panel animator.
        _subPanelAnim.SetTrigger(OPEN_TRIGGER);
    }

    /// <summary>
    /// Hides panel.
    /// </summary>
    /// <param name="p_transitionTime">Hiding time (s).</param>
    public void ClosePanel(float p_transitionTime = 0)
    {
        // Activate closing trigger of sub-panel animator.
        _subPanelAnim.SetTrigger(CLOSE_TRIGGER);

        // Hides the panel.
        base.Close(p_transitionTime);
    }

    /// <summary>
    /// Updates displayed data based on the button clicked.
    /// </summary>
    /// <param name="p_index">Index of the button clicked.</param>
    /// <param name="p_mapData">Current map data.</param>
    private void DisplayData(int p_index, MapData p_mapData)
    {
        // String builder instance, for the more complex answers.
        StringBuilder m_stringBuilder = new StringBuilder();

        // Checks the received button index.
        switch (p_index)
        {
            case 1:

                // Displays title for this data analytic.
                _titleTxt.text = "1. No. of tiles without resources";

                // Sets up answer font size to better fit a number.
                _answerTxt.fontSize = FONT_SIZE_NUM;

                // Displays the answer as the number of game tiles where
                // the resources count is equal to 0.
                _answerTxt.text = p_mapData.GameTiles
                    .Count(t => t.Resources.Count == 0)
                    .ToString();

                break;

            case 2:

                // Displays title for this data analytic.
                _titleTxt.text = "2. Average Coin in Mountain tiles";

                // Sets up answer font size to better fit a number.
                _answerTxt.fontSize = FONT_SIZE_NUM;

                // If there are any mountain tiles.
                if (p_mapData.GameTiles.OfType<MountainTile>().Any())
                {
                    // Displays the answer as the average of all coin values belonging
                    // only to mountain tiles.
                    _answerTxt.text = p_mapData.GameTiles
                        .OfType<MountainTile>()
                        .Average(t => t.Coin)
                        .ToString("0.00");
                }

                // If there aren't any, set the answer to 0.
                else _answerTxt.text = "0";

                break;

            case 3:

                // Displays title for this data analytic.
                _titleTxt.text = "3. Existing terrains, alphabetically";

                // Sets up answer font size to better fit a string.
                _answerTxt.fontSize = FONT_SIZE_STRING;

                // Creates an IEnumerable of the different existing
                // terrains' names, ordered alphabetically.
                IEnumerable<string> _existingTerrains = p_mapData.GameTiles
                    .OrderBy(t => t.Name)
                    .Select(t => t.Name)
                    .Distinct();

                // Iterates each terrain name and appends it to the string builder.
                foreach (string f_terrain in _existingTerrains)
                    m_stringBuilder.Append(f_terrain + "\n");

                // Displays the string builder text.
                _answerTxt.text = m_stringBuilder.ToString();

                break;

            case 4:

                // Displays title for this data analytic.
                _titleTxt.text = "4. Tile with least Coin";

                // Sets up answer font size to better fit a string.
                _answerTxt.fontSize = FONT_SIZE_STRING;

                // Saves the first game tile found with the least coin value.
                GameTile m_tile = p_mapData.GameTiles
                    .OrderBy(t => t.Coin)
                    .FirstOrDefault();

                // Appends it's name to the string builder.
                m_stringBuilder.Append(m_tile.Name + "\n\n");

                // Iterates all resources of said tile.
                foreach (Resource r in m_tile.Resources)
                {
                    // Appends each resource's name to the string builder.
                    m_stringBuilder.Append("-" + r.Name + "\n");
                }

                // Calculates relative game tile based on it's list index.
                float m_relativePos = p_mapData.GameTiles.IndexOf(m_tile) *
                    p_mapData.Dimensions_Y / 
                    (float)(p_mapData.Dimensions_Y * p_mapData.Dimensions_X);

                // Y equals the relative position's integer value.
                long m_yCoords = (long)m_relativePos;

                // X equals the relative position's decimal value * Y.
                int m_xCoords = (int)((m_relativePos - m_yCoords) 
                    * p_mapData.Dimensions_X);

                // Appends the coordinates to the string builder.
                m_stringBuilder.Append(
                    "\n" + $"COL: {(m_xCoords + 1).ToString()} | "
                    + $"ROW: {(m_yCoords + 1).ToString()}");

                // Displays the string builder text.
                _answerTxt.text = m_stringBuilder.ToString();

                break;

            case 5:

                // Displays title for this data analytic.
                _titleTxt.text = "5. No. of unique tiles";

                // Sets up answer font size to better fit a number.
                _answerTxt.fontSize = FONT_SIZE_NUM;

                // Displays the answer as the count of every distinct game tile,
                // based on GameTileComparer().
                _answerTxt.text = p_mapData.GameTiles
                    .Distinct(new GameTileComparer())
                    .Count()
                    .ToString();

                break;
        }
    }
}

