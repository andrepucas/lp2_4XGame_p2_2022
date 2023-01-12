using System;
using System.Collections.Generic;

/// <summary>
/// Holds all relevant info about a map.
/// </summary>
public class MapData : IComparable<MapData>
{
    /// <summary>
    /// Event raised when data finishes loading. True if data is valid.
    /// </summary>
    public static event Action<bool> OnValidLoadedData;

    /// <summary>
    /// Self implemented property that holds the map name.
    /// </summary>
    /// <value>Name of the map.</value>
    public string Name { get; set; }

    /// <summary>
    /// Read only self implemented property that holds the map X dimensions.
    /// </summary>
    /// <value>Map size in the X axis.</value>
    public int XCols { get; }

    /// <summary>
    /// Read only self implemented property that holds the map Y dimensions.
    /// </summary>
    /// <value>Map size in the Y axis.</value>
    public int YRows { get; }

    /// <summary>
    /// Read only self implemented property that holds all map file lines.
    /// </summary>
    /// <value>String array of each map file line.</value>
    public IReadOnlyList<string> Data { get; }

    /// <summary>
    /// Read only self implemented property that holds a list of all game tiles.
    /// </summary>
    /// <value>List of all game tiles.</value>
    public IReadOnlyList<GameTile> GameTiles => _gameTilesList;

    /// <summary>
    /// Private list of Game Tiles.
    /// </summary>
    private List<GameTile> _gameTilesList;

    // Saves how many lines in the file should be ignored by indexers.
    // Useful to exclude full comment lines.
    private int _linesToIgnore;

    /// <summary>
    /// Constructor method. Initializes the map's properties.
    /// Doesn't convert and save all file info right away. Only it's dimensions,
    /// so that they can be displayed in the widget.
    /// </summary>
    /// <param name="p_name">Name of the map.</param>
    /// <param name="p_data">File lines.</param>
    public MapData (string p_name, string[] p_data)
    {
        // Sets the X and Y map dimentions to 0.
        int _cols = 0;
        int _rows = 0;

        // Holds the first lines of the data file.
        string[] m_firstLine;

        // Sets the number of lines to ignore to 0.
        _linesToIgnore = 0;

        // Continuous loop to check initial lines.
        while (true)
        {
            // If it's empty or starts with a comment, increments lines to ignore
            // and loops again.
            if (p_data[_linesToIgnore].Length == 0 || p_data[_linesToIgnore][0] == '#')
                _linesToIgnore++;

            // If it has content, saves it and breaks the loop.
            else
            {
                m_firstLine = p_data[_linesToIgnore].Split();
                break;
            }
        }

        // If the conversion of both rows and cols value is successful.
        if (m_firstLine.Length == 2 && Int32.TryParse(m_firstLine[0], out _rows) 
            && Int32.TryParse(m_firstLine[1], out _cols))
        {
            // Sets the Rows and Cols properties.
            YRows = _rows;
            XCols = _cols;

            // Increments lines to ignore, so that future indexers start
            // after this dimensions line.
            _linesToIgnore++;
        }

        // If the conversion isn't possible, do nothing.
        // It will be recognized as an invalid map later.

        // Saves name and data according to parameters.
        Name = p_name;
        Data = p_data;

        // Creates an empty list of game tiles.
        _gameTilesList = new List<GameTile>(XCols * YRows);
    }

    /// <summary>
    /// Converts the data from the file into game tile objects and saves it.
    /// </summary>
    public void LoadGameTilesData(PresetGameDataSO p_gameData)
    {
        // Holds a file line at a time.
        string m_line;

        // Array of all strings in a line.
        string[] m_lineStrings;

        // Line index where a comment (#) is found.
        int m_commentIndex;

        // Counts how many terrains/resources are checked.
        int m_checkCount;

        // Temporarily holds reference to preset values.
        PresetTerrainsData m_terrainsData;
        PresetResourcesData m_resourcesData;

        // Controls whether all resources were successfully read or not.
        bool _failedResource = false;

        // Iterates all file data info. 
        // Skips ignored lines, already handled in constructor.
        for (int i = _linesToIgnore; i < Data.Count; i++)
        {
            // Saves the current line.
            m_line = Data[i].ToLower();

            // Looks for a "#" in the string.
            m_commentIndex = m_line.IndexOf("#");

            // If there is one, ignores everything that comes after it.
            if (m_commentIndex >= 0) m_line = m_line.Substring(0, m_commentIndex);

            // If a comment occupies the full line or it's simply empty, ignore it.
            if (m_line.Length == 0)
            {
                _linesToIgnore++;
                continue;
            }

            // Splits line into individual strings, by its spaces.
            m_lineStrings = m_line.ToLower().Trim().Split();

            // Resets check count.
            m_checkCount = 0;

            // Iterates collection of all valid game terrains.
            for (int t = 0; t < p_gameData.Terrains.Count; t++)
            {
                // If the first word matches a valid terrain name.
                if (m_lineStrings[0] == p_gameData.Terrains[t].RawName)
                {
                    // Temporarily holds that terrain's preset values.
                    m_terrainsData = p_gameData.Terrains[t];

                    // Initializes and adds a Game Tile with its respective preset 
                    // values to game tiles list.
                    _gameTilesList.Insert(i - _linesToIgnore, new GameTile(
                        m_terrainsData.Name,
                        m_terrainsData.Coin,
                        m_terrainsData.Food,
                        m_terrainsData.Sprites));

                    break;
                }

                // Increments control number of terrains checked.
                m_checkCount++;
            }

            // If the terrain wasn't found, increments lines to ignore.
            if (m_checkCount == p_gameData.Terrains.Count)
                _linesToIgnore++;

            // If there are more strings in this line, looks for resources.
            if (m_lineStrings.Length > 0)
            {
                // Iterates each word, starting by the 2nd, the first resource.
                for (int s = 1; s < m_lineStrings.Length; s++)
                {
                    // Resets check count.
                    m_checkCount = 0;

                    // Iterates all valid game resources.
                    for (int r = 0; r < p_gameData.Resources.Count; r++)
                    {
                        // If this word matches a valid resource name.
                        if (m_lineStrings[s] == p_gameData.Resources[r].RawName)
                        {
                            // Temporarily holds that resource's preset values.
                            m_resourcesData = p_gameData.Resources[r];

                            // Initializes and adds a Resource with its respective 
                            // preset values to the resources list, in this tile.
                            GameTiles[i - _linesToIgnore].AddResource(new Resource(
                                m_resourcesData.Name,
                                m_resourcesData.Coin,
                                m_resourcesData.Food,
                                p_gameData.GetSpriteDictOf(m_resourcesData.Name),
                                m_resourcesData.DefaultResourceSprite));

                            break;
                        }

                        // Increments check count.
                        m_checkCount++;
                    }

                    // If the resource wasn't found, set as a failed resource.
                    if (m_checkCount == p_gameData.Resources.Count)
                        _failedResource = true;
                }
            }
        }

        // If the map's dimensions don't match with the number of game tiles saved
        // or if atleast one resource couldn't be read, raise invalid data event.
        if ((XCols * YRows) != GameTiles.Count || _failedResource)
            OnValidLoadedData?.Invoke(false);

        // Otherwise, this map is valid to be load.
        else OnValidLoadedData?.Invoke(true);
    }

    /// <summary>
    /// Compares map datas using the name string..
    /// </summary>
    /// <param name="other">Map data to compare.</param>
    /// <returns></returns>
    public int CompareTo(MapData other) => Name.CompareTo(other?.Name);
}
