using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all relevant info about a map.
/// </summary>
public class MapData : IComparable<MapData>
{
    /// <summary>
    /// Public set self implemented property that holds the map name.
    /// </summary>
    /// <value>Name of the map.</value>
    public string Name {get; set;}

    /// <summary>
    /// Read only self implemented property that holds the map X dimensions.
    /// </summary>
    /// <value>Map size in the X axis.</value>
    public int XCols {get;}

    /// <summary>
    /// Read only self implemented property that holds the map Y dimensions.
    /// </summary>
    /// <value>Map size in the Y axis.</value>
    public int YRows {get;}

    /// <summary>
    /// Read only self implemented property that holds all map file lines.
    /// </summary>
    /// <value>String array of each map file line.</value>
    public string[] Data {get;}

    /// <summary>
    /// Private set self implemented property that holds a list of all game tiles.
    /// </summary>
    /// <value>List of all game tiles.</value>
    public List<GameTile> GameTiles {get; private set;}

    // Saves how many lines in the file should be ignored by indexers.
    // Useful to exclude full comment lines.
    private int _linesToIgnore;

    /// <summary>
    /// Constructor method. Initializes the map's properties.
    /// Doesn't convert and save all file info right away. Only it's dimensions
    /// so that they can be displayed in the widget.
    /// </summary>
    /// <param name="p_name">Name of the map.</param>
    /// <param name="p_data">File lines.</param>
    public MapData (string p_name, string[] p_data)
    {
        int _cols = 0;
        int _rows = 0;
        string[] m_firstLine;

        _linesToIgnore = 0;

        // Continuos loop to check initial lines.
        while (true)
        {
            // If it's empty or starts with a comment, increment lines to ignore
            // and loop again.
            if (p_data[_linesToIgnore].Length == 0 || p_data[_linesToIgnore][0] == '#')
                _linesToIgnore++;

            // If it has content, save it and break the loop.
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
            // Set the Rows and Cols properties.
            YRows = _rows;
            XCols = _cols;

            // Increment lines to ignore, so that future indexers start
            // after this dimensions line.
            _linesToIgnore++;
        }

        // If the conversion isn't possible, this is an invalid map.

        // Saves name and data according to parameters.
        Name = p_name;
        Data = p_data;

        // Creates an empty list of game tiles.
        GameTiles = new List<GameTile>(XCols * YRows);
    }

    /// <summary>
    /// Converts the data from the file into game tile objects and saves it.
    /// </summary>
    public void LoadGameTilesData()
    {
        // Holds one line of the file.
        string m_line;
        
        // Line index where a comment (#) is found.
        int m_commentIndex;

        // Array of all string found in a line.
        string[] m_lineStrings;

        // Iterates all file data info. 
        // Skips first line, which was already handled in constructor.
        for (int i = _linesToIgnore; i < Data.Length; i++)
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

            // Splits line into individual strings, separated by spaces.
            m_lineStrings = Data[i].Split();

            // Handles and instantiates the game tile (first string).
            switch (m_lineStrings[0])
            {
                case "desert":

                    // Adds a new desert tile to the game tiles list.
                    GameTiles.Insert(i - _linesToIgnore, new DesertTile());
                    break;

                case "hills":

                    // Adds a new hills tile to the game tiles list.
                    GameTiles.Insert(i - _linesToIgnore, new HillsTile());
                    break;

                case "mountain":

                    // Adds a new mountain tile to the game tiles list.
                    GameTiles.Insert(i - _linesToIgnore, new MountainTile());
                    break;

                case "plains":

                    // Adds a new plains tile to the game tiles list.
                    GameTiles.Insert(i - _linesToIgnore, new PlainsTile());
                    break;

                case "water":

                    // Adds a new water tile to the game tiles list.
                    GameTiles.Insert(i - _linesToIgnore, new WaterTile());
                    break;
            }

            // If there are more strings in that line.
            if (m_lineStrings.Length > 0)
            {
                // Iterates each one, starting on the second one, the first resource.
                for (int s = 1; s < m_lineStrings.Length; s++)
                {
                    // Handles and instantiates the resource.
                    switch (m_lineStrings[s])
                    {
                        case "plants":

                            // Adds a new plants resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new PlantsResource());
                            break;

                        case "animals":

                            // Adds a new animals resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new AnimalsResource());
                            break;

                        case "metals":

                            // Adds a new metals resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new MetalsResource());
                            break;

                        case "fossilfuel":

                            // Adds a new fossil fuel resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new FossilFuelResource());
                            break;

                        case "luxury":

                            // Adds a new luxury resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new LuxuryResource());
                            break;

                        case "pollution":

                            // Adds a new pollution resource to the game tile.
                            GameTiles[i - _linesToIgnore].AddResource(new PollutionResource());
                            break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Compares map data using the name string.
    /// </summary>
    /// <param name="other">Map data to compare.</param>
    /// <returns></returns>
    public int CompareTo(MapData other) => Name.CompareTo(other?.Name);
}
