using System;
using System.IO;
using System.Text.RegularExpressions;
#if NET45
using System.Windows.Forms;
#endif
namespace WinLib;
public class Dirs
{
    public static string RemoveTrailingSlashes(string path)
    {
        string result = Regex.Replace(path, @"[/\\]+$", "");
        return result;
    }
    /// <summary>
    /// Creates a relative path from one file or folder to another.
    /// </summary>
    /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
    /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
    /// <returns>The relative path from the start directory to the end path.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fromPath" /> or <paramref name="toPath" /> is <c>null</c>.</exception>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetRelativePath(string fromPath, string toPath)
    {
        // https://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
        if (string.IsNullOrEmpty(fromPath))
        {
            throw new ArgumentNullException("fromPath");
        }
        if (string.IsNullOrEmpty(toPath))
        {
            throw new ArgumentNullException("toPath");
        }
        Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
        Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));
        if (fromUri.Scheme != toUri.Scheme)
        {
            return toPath;
        }
        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());
        if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase))
        {
            relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
        return relativePath;
    }
    private static string AppendDirectorySeparatorChar(string path)
    {
        // Append a slash only if the path is a directory and does not have a slash.
        if (!Path.HasExtension(path) &&
            !path.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            return path + Path.DirectorySeparatorChar;
        }
        return path;
    }
    public static string GetTempPath(string? prefix = null, string? suffix = null)
    {
        if (prefix == null) prefix = "";
        if (suffix == null) suffix = "";
        string guid = Util.GuidString();
        return Path.Combine(Path.GetTempPath(), prefix + guid + suffix);
    }
    public static string GetCurrentDirectory()
    {
        return System.IO.Directory.GetCurrentDirectory();
    }
    public static void SetCurrentDirectory(string path)
    {
        Dirs.Prepare(path);
        System.IO.Directory.SetCurrentDirectory(path);
    }
    public static string ExeDirPath()
    {
#if NET45
        string path = Application.ExecutablePath;
        //return Directory.GetParent(path)!.FullName;
        Dirs.Parent(path);
#else
        return System.AppContext.BaseDirectory;
#endif
    }
    public static string ProfilePath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }
    //public static string ProfilePath(string name)
    //{
    //    return ProfilePath() + Path.DirectorySeparatorChar + name;
    //}
    public static string ProfilePath(string orgName, string appName)
    {
        string baseFolder = SpecialFolderPath(Environment.SpecialFolder.UserProfile);
        return $"{baseFolder}{Path.DirectorySeparatorChar}{orgName}{Path.DirectorySeparatorChar}{appName}";
    }
    public static string ProfilePath(string appName)
    {
        string baseFolder = SpecialFolderPath(Environment.SpecialFolder.UserProfile);
        return $"{baseFolder}{Path.DirectorySeparatorChar}{appName}";
    }
    public static string DocumentsPath()
    {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }
    public static string DocumentsPath(string name)
    {
        return DocumentsPath() + Path.DirectorySeparatorChar + name;
    }
    public static string SpecialFolderPath(Environment.SpecialFolder folder)
    {
        return System.Environment.GetFolderPath(folder);
    }
    public static string AppDataFolderPath()
    {
        string baseFolder = SpecialFolderPath(Environment.SpecialFolder.ApplicationData);
        return baseFolder;
    }
    public static string AppDataFolderPath(string orgName, string appName)
    {
        string baseFolder = SpecialFolderPath(Environment.SpecialFolder.ApplicationData);
        return $"{baseFolder}{Path.DirectorySeparatorChar}{orgName}{Path.DirectorySeparatorChar}{appName}";
    }
    public static string AppDataFolderPath(string appName)
    {
        string baseFolder = SpecialFolderPath(Environment.SpecialFolder.ApplicationData);
        return $"{baseFolder}{Path.DirectorySeparatorChar}{appName}";
    }
    public static void CreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        Directory.CreateDirectory(path);
    }
    public static void Prepare(string dirPath)
    {
        Dirs.CreateDirectory(dirPath);
    }
    public static void PrepareForFile(string filePath)
    {
        Prepare(Path.GetDirectoryName(filePath) !);
    }
    public static string GetParent(string path)
    {
        return Directory.GetParent(path) !.FullName;
    }
    public static string GetFullPath(string path)
    {
        return Path.GetFullPath(path);
    }
    public static string GetFileName(string path)
    {
        return Path.GetFileName(path);
    }
    public static string GetFileNameWithoutExtension(string path)
    {
        return Path.GetFileNameWithoutExtension(path);
    }
    public static string GetDirectory(string path)
    {
        return Dirs.GetFullPath(Path.GetDirectoryName(path) !);
    }
    public static string GetExtension(string path)
    {
        return Path.GetExtension(path);
    }
}