using System;
using System.IO;

namespace Ecureuil.Core.Services {
  public class CacheManager {
    private readonly string _appdataPath;
    private readonly string _cachePath;

    public CacheManager() 
      //the local cache folder is the AppData one, so in AppData\Local\Ecureuil\Cache
      _appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocaApplicationData), "Ecureuil");
      _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocaApplicationData), "Ecureuil", "Cache");

      if (!Directory.Exists(_cachePath) Directory.CreateDirectory(_cachePath);
      if (!Directory.Exists(_appdataPath) Directory.CreateDirectory(_appdataPath);
    }

    //user_sources is the complete list of all sources on your computer
    public void SaveUserSources(string jsonContent) {
      SaveFile(Path.Combine(_appdataPath, "user_sources.json"), jsonContent);
    }

    public string LoadUserSources() {
      return LoadFile(Path.Combine(_appdataPath, "user_sources.json"));
    }

    public void SaveSourceInfoCache(string sourceId, string jsonContent) {
      if (string.IsNullOrWhiteSpace(sourceId)) return;
      SaveFile(Path.Combine(_cachePath, $"source_info_{SanitizeFileName(sourceId)}.json"), jsonContent);
    }

    public void LoadSourceInfoCache(string sourceId) {
      if (string.IsNullOrWhiteSpace(sourceId)) return;
      return LoadFile(Path.Combine(_cachePath, $"source_info_{SanitizeFileName(sourceId)}.json"));
    }

    public void SaveSourceCatalogCache(string sourceId, string jsonContent) {
      if (string.IsNullOrWhiteSpace(sourceId)) return;
      SaveFile(Path.Combine(_cachePath, $"cache_catalog_{SanitizeFileName(sourceId)}.json"), jsonContent);
    }

    public void LoadSourceCatalogCache(string sourceId) {
      if (string.IsNullOrWhiteSpace(sourceId)) return;
      return LoadFile(Path.Combine(_cachePath, $"cache_catalog_{SanitizeFileName(sourceId)}.json"));
    }

    //if a cache is older then the max time defined by the user then it's old and is marked as "should be updated"
    public bool isSourceCacheValid(string sourceId, TimeSpan maxAge) {
      if (string.IsNullOrWhiteSpace(sourceId)) return false;

      string fileName = $"cache_catalog_{SanitizeFileName(sourceId)}.json";
      string filePath = Path.Combine(_cachePath, fileName);

      if (!File.Exists(filePath)) return false;

      DateTime lastWriteTime = File.GetLastWriteTime(filePath);
      return (DateTime.Now - lastWriteTime) < maxAge;
    }

    public void ClearSourceCache(string sourceId) {
      if (string.IsNullOrWhiteSpace(sourceId)) return;

      //Deletes both the info of the source from disk, and the index itself
      string infoFile = Path.Combine(_cachePath, $"source_info_{SanitizeFileName(sourceId)}.json");
      string catalogFile = Path.Combine(_cachePath, $"cache_catalog_{SanitizeFileName(sourceId)}.json");

      DeleteFile(infoFile); DeleteFile(catalogFile);
    }

    private void SaveFile(string fullPath, string content) {
      try {
        File.WriteAllText(fullPath, content);
      } catch (Exception ex) {
        Console.WriteLine($"Error in saving: {ex.Message}");
      }
    }

    private string LoadFile(string fullPath) {
      try {
        if (File.Exists(fullPath)) return File.ReadAllText(fullPath);
      } catch (Exception ex) {
        Console.WriteLine($"Error in loading file: {ex.Message}");
      }
      return null;
    }

    private void DeleteFile(string fullPath) {
      try {
        File.Delete(fullPath);
      } catch (Exception ex) {
        Console.WriteLine($"Error in deleting: {ex.Message}");
      }
    }

    private string SanitizeFileName(string sourceId) {
      foreach (char c in Path.GetInvalidFileNameChars()) 
        sourceId = sourceId.Replace(c, '_');
        return sourceId.ToLowerInvariant();
    }
  }
}
