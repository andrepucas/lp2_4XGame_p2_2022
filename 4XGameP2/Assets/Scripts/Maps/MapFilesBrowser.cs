using System;
using System.Collections.Generic;
using System.IO;
using ImportedGenerator;

/// <summary>
/// Locates and manipulates map files.
/// </summary>
public static class MapFilesBrowser
{
    /// <summary>
    /// Constant string value for the maps folder name.
    /// </summary>
    private const string FOLDER = "map4xfiles";

    // Path string. Combination of desktop system path with the folder's name.
    private static string _path = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.Desktop), FOLDER);

    /// <summary>
    /// Locates and returns all the map files in the folder.
    /// </summary>
    /// <returns>List of maps found in map files folder.</returns>
    public static List<MapData> GetMapsList()
    {
        // Creates a list of map datas.
        List<MapData> _mapsList = new List<MapData>();

        // If folder doesn´t exist, stops here.
        if (!Directory.Exists(_path)) return _mapsList;

        // Creates a directory info based on the folder path.
        DirectoryInfo m_info = new DirectoryInfo(_path);

        // Iterates every existing file in the directory.
        foreach(FileInfo file in m_info.GetFiles("*.map4x"))
        {
            // Instantiates a new map data and adds it to the list of maps.
            _mapsList.Add(new MapData(
                Path.GetFileNameWithoutExtension(file.FullName), 
                File.ReadAllLines(file.FullName)));
        }

        // Sorts the map data list (alphabetically).
        _mapsList.Sort();

        // Returns it.
        return _mapsList;
    }

    /// <summary>
    /// Renames a map file.
    /// </summary>
    /// <param name="p_oldName">Original file name.</param>
    /// <param name="p_newName">New file name.</param>
    public static void RenameMapFile(string p_oldName, string p_newName)
    {
        // Concatenates the file extension to both names.
        p_oldName += ".map4x";
        p_newName += ".map4x";

        // Renames file.
        System.IO.File.Move(
            Path.Combine(_path, p_oldName), Path.Combine(_path, p_newName));
    }

    /// <summary>
    /// Deletes a map file.
    /// </summary>
    /// <param name="p_fileName">File name.</param>
    public static void DeleteMapFile(string p_fileName)
    {
        // Concatenates the file extension.
        p_fileName += ".map4x";

        // Deletes file.
        System.IO.File.Delete(Path.Combine(_path, p_fileName));
    }

    /// <summary>
    /// Generates a new map file, using an imported map generator.
    /// </summary>
    /// <param name="p_name">Name of the map file.</param>
    /// <param name="p_yRows">Y dimensions of map.</param>
    /// <param name="p_xCols">X dimensions of map.</param>
    /// <param name="p_data">Generator parameters.</param>
    /// <returns>Name of generated map.</returns>
    public static string GenerateNewMapFile(string p_name, int p_yRows, 
        int p_xCols, MapTilesDataSO p_data)
    {
        Generator m_generator;
        Map m_map;

        // Concatenates file extension to map name.
        string m_fileName = p_name + ".map4x";

        // If maps folder doesn't exist yet, create it.
        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

        // Creates new generator with generator data.
        m_generator = new Generator(p_data.TerrainNames, p_data.ResourceNames);

        // Generates the new map.
        m_map = m_generator.CreatePCGMap(p_yRows, p_xCols);

        // Creates map file.
        m_generator.SaveMap(m_map, Path.Combine(_path, m_fileName));

        // Returns the name of the map.
        return p_name;
    }

    /// <summary>
    /// Looks for other files with the same name and modifies name to look
    /// like the next version of existing files.
    /// </summary>
    /// <param name="p_name">File name to look for.</param>
    /// <returns>Name_VERSION, if duplicates are found.</returns>
    public static string DupNameProtection(string p_name)
    {
        // Integer that stores the times a file with the same name is found.
        int m_sameNameFileCount = 0;

        // If file with this name already exists.
        while (File.Exists(Path.Combine(_path, (p_name + ".map4x"))))
        {
            // Increments same name count.
            m_sameNameFileCount++;

            // If name already has a _N appended to it, remove it.
            if (m_sameNameFileCount > 1)
                p_name = p_name.Substring(0, p_name.Length-2);

            // Adds _N to the end of the name.
            p_name += ("_" + m_sameNameFileCount);
        }

        // Returns the name. Modified or not.
        return p_name;
    }
}
