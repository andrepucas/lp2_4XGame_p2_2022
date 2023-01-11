using System.Text.RegularExpressions;

/// <summary>
/// Validates file names so they don't have illegal path characters.
/// </summary>
public static class MapFileNameValidator
{
    /// <summary>
    /// Regex holding illegal path characters.
    /// </summary>
    /// <returns>Illegal characters.</returns>
    private static readonly Regex ILLEGAL_CHARS = 
        new Regex("[#%&{}\\<>*?/$!'\":@+`|= ]");

    /// <summary>
    /// Replaces the string's illegal characters with '_'.
    /// </summary>
    /// <param name="p_fileName">File name.</param>
    /// <returns>Valid file name.</returns>
    public static string Validate(string p_fileName)
    {
        // Replaces the illegal chars from name with "_".
        p_fileName = ILLEGAL_CHARS.Replace(p_fileName, "_");

        // Modifies the name in order to failsafe against duplicates.
        p_fileName = MapFilesBrowser.DupNameProtection(p_fileName);

        return p_fileName;
    }
}
