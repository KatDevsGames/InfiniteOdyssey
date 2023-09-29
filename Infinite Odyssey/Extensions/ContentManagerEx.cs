using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace InfiniteOdyssey.Extensions;

public static class ContentManagerEx
{
    public static IEnumerable<string> GetAssetNames(this ContentManager contentManager, string subdirectoryPath)
    {
        string rootDirectory = contentManager.RootDirectory;
        string searchDirectory = Path.Combine(rootDirectory, subdirectoryPath);
        if (!Directory.Exists(searchDirectory))
            throw new DirectoryNotFoundException("Subdirectory not found: " + searchDirectory);

        foreach (string assetName in TraverseContentDirectory(searchDirectory, rootDirectory))
            yield return assetName;
    }

    private static IEnumerable<string> TraverseContentDirectory(string currentDirectory, string rootDirectory)
    {
        string[] files = Directory.GetFiles(currentDirectory);
        foreach (string file in files)
        {
            // Remove the content root and root directory from the file path
            string assetName = Path.ChangeExtension(file.Replace(rootDirectory, "").Trim('\\', '/'), null);
            yield return assetName;
        }

        string[] subDirectories = Directory.GetDirectories(currentDirectory);
        foreach (string subDirectory in subDirectories)
        foreach (string assetName in TraverseContentDirectory(subDirectory, rootDirectory))
            yield return assetName;
    }
}