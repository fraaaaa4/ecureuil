using System;
using System.IO;

namespace Ecureuil.Core.Services {
  public class CacheManager {
    private readonly string _appdataPath;
    private readonly string _cachePath;
      public string CachePath;

      public bool IsCacheEnabled
      {
          get
          {
              try
              {
                  return Ecureuil.Properties.Settings.Default.enableCache;
              }
              catch
              {
                  return true;
              }
          }
      }

    public CacheManager() {
      // Cartella cache locale in AppData\Local\Ecureuil\Cache
      string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      _appdataPath = Path.Combine(localAppData, "Ecureuil");
      _cachePath = Path.Combine(_appdataPath, "Cache");
      CachePath = _cachePath;

      if (!Directory.Exists(_appdataPath)) {
        Directory.CreateDirectory(_appdataPath);
      }
      if (!Directory.Exists(_cachePath)) {
        Directory.CreateDirectory(_cachePath);
      }
    }

    // user_sources contiene la lista salvata delle sorgenti
    public void SaveUserSources(string jsonContent) {
      SaveFile(Path.Combine(_appdataPath, "user_sources.json"), jsonContent);
    }

    public string LoadUserSources() {
      return LoadFile(Path.Combine(_appdataPath, "user_sources.json"));
    }

    public void SaveSourceInfoCache(string sourceId, string jsonContent) {
      if (string.IsNullOrEmpty(sourceId)) return;
      if (!IsCacheEnabled) return;
      SaveFile(Path.Combine(_cachePath, "source_info_" + SanitizeFileName(sourceId) + ".json"), jsonContent);
    }

    public string LoadSourceInfoCache(string sourceId) {
      if (string.IsNullOrEmpty(sourceId)) return null;
      if (!IsCacheEnabled) return null;
      return LoadFile(Path.Combine(_cachePath, "source_info_" + SanitizeFileName(sourceId) + ".json"));
    }

    public void SaveSourceCatalogCache(string sourceId, string jsonContent) {
      if (string.IsNullOrEmpty(sourceId)) return;
      if (!IsCacheEnabled) return;
      SaveFile(Path.Combine(_cachePath, "cache_catalog_" + SanitizeFileName(sourceId) + ".json"), jsonContent);
    }

    public string LoadSourceCatalogCache(string sourceId) {
      if (string.IsNullOrEmpty(sourceId)) return null;
      if (!IsCacheEnabled) return null;
      return LoadFile(Path.Combine(_cachePath, "cache_catalog_" + SanitizeFileName(sourceId) + ".json"));
    }

    public void SaveAppCache(string appId, string jsonContent) {
      if (string.IsNullOrEmpty(appId)) return;
      if (!IsCacheEnabled) return;
      SaveFile(Path.Combine(_cachePath, "cache_app_" + SanitizeFileName(appId) + ".json"), jsonContent);
    }

    public string LoadAppCache(string appId) {
      if (string.IsNullOrEmpty(appId)) return null;
      if (!IsCacheEnabled) return null;
      return LoadFile(Path.Combine(_cachePath, "cache_app_" + SanitizeFileName(appId) + ".json"));
    }

    public bool isAppCacheValid(string appId, TimeSpan maxAge) {
      if (string.IsNullOrEmpty(appId)) return false;
      if (!IsCacheEnabled) return false;
      string fileName = "cache_app_" + SanitizeFileName(appId) + ".json";
      string filePath = Path.Combine(_cachePath, fileName);

      if (!File.Exists(filePath)) return false;

      DateTime lastWriteTime = File.GetLastWriteTime(filePath);
      return (DateTime.Now - lastWriteTime) < maxAge;
    }

    // Controlla se la cache e ancora valida rispetto al maxAge
    public bool isSourceCacheValid(string sourceId, TimeSpan maxAge) {
      if (string.IsNullOrEmpty(sourceId)) return false;
      if (!IsCacheEnabled) return false;
      string fileName = "cache_catalog_" + SanitizeFileName(sourceId) + ".json";
      string filePath = Path.Combine(_cachePath, fileName);

      if (!File.Exists(filePath)) return false;

      DateTime lastWriteTime = File.GetLastWriteTime(filePath);
      return (DateTime.Now - lastWriteTime) < maxAge;
    }

    public void SaveImageCache(string key, byte[] data) {
      if (string.IsNullOrEmpty(key) || data == null || data.Length == 0) return;
      if (!IsCacheEnabled) return;
      try {
        string imgPath = Path.Combine(_cachePath, "img_" + SanitizeFileName(key) + ".dat");
        File.WriteAllBytes(imgPath, data);
      } catch { }
    }

    public byte[] LoadImageCache(string key) {
      if (string.IsNullOrEmpty(key)) return null;
      if (!IsCacheEnabled) return null;
      try {
        string imgPath = Path.Combine(_cachePath, "img_" + SanitizeFileName(key) + ".dat");
        if (File.Exists(imgPath)) {
          return File.ReadAllBytes(imgPath);
        }
      } catch { }
      return null;
    }

    public void DeleteImageCache(string key) {
      if (string.IsNullOrEmpty(key)) return;
      try {
        string imgPath = Path.Combine(_cachePath, "img_" + SanitizeFileName(key) + ".dat");
        if (File.Exists(imgPath)) {
          File.Delete(imgPath);
        }
      } catch { }
    }

    public void ClearAppCache(string appId) {
      if (string.IsNullOrEmpty(appId)) return;
      string sId = SanitizeFileName(appId);

      DeleteFile(Path.Combine(_cachePath, "cache_app_" + sId + ".json"));
      DeleteFile(Path.Combine(_cachePath, "app_" + sId + ".json"));
      DeleteMatchingDatFiles(sId, false);
    }

    public void ClearSourceCache(string sourceId) {
      if (string.IsNullOrEmpty(sourceId)) return;
      string sId = SanitizeFileName(sourceId);

      DeleteFile(Path.Combine(_cachePath, "source_info_" + sId + ".json"));
      DeleteFile(Path.Combine(_cachePath, "cache_catalog_" + sId + ".json"));
      DeleteMatchingDatFiles(sId, true);
    }

    private void DeleteMatchingDatFiles(string sId, bool isSource) {
      try {
        if (Directory.Exists(_cachePath)) {
          string[] allFiles = Directory.GetFiles(_cachePath);
          for (int i = 0; i < allFiles.Length; i++) {
            string fileName = Path.GetFileName(allFiles[i]);
            if (fileName == null) continue;
            string fnLower = fileName.ToLowerInvariant();
            if (fnLower.EndsWith(".dat")) {
              bool matches = isSource ?
                (fnLower == "img_" + sId + ".dat" ||
                 fnLower == "img_src_" + sId + ".dat" ||
                 fnLower == "img_disc_" + sId + ".dat" ||
                 fnLower.StartsWith("img_src_" + sId + "_") ||
                 fnLower.StartsWith("img_disc_" + sId + "_") ||
                 fnLower.StartsWith("img_" + sId + "_")) :
                (fnLower == "img_" + sId + ".dat" ||
                 fnLower == "img_app_" + sId + ".dat" ||
                 fnLower.StartsWith("img_app_" + sId + "_") ||
                 fnLower.StartsWith("img_" + sId + "_"));

              if (matches) {
                DeleteFile(allFiles[i]);
              }
            }
          }
        }
      } catch { }
    }

    private void SaveFile(string fullPath, string content) {
      try {
        File.WriteAllText(fullPath, content);
      } catch (Exception ex) {
        Console.WriteLine("Error in saving: " + ex.Message);
      }
    }

    private string LoadFile(string fullPath) {
      try {
        if (File.Exists(fullPath)) {
          return File.ReadAllText(fullPath);
        }
      } catch (Exception ex) {
        Console.WriteLine("Error in loading file: " + ex.Message);
      }
      return null;
    }

    private void DeleteFile(string fullPath) {
      try {
        if (File.Exists(fullPath)) {
          File.Delete(fullPath);
        }
      } catch (Exception ex) {
        Console.WriteLine("Error in deleting: " + ex.Message);
      }
    }

    private string SanitizeFileName(string sourceId) {
      char[] invalidChars = Path.GetInvalidFileNameChars();
      for (int i = 0; i < invalidChars.Length; i++) {
        sourceId = sourceId.Replace(invalidChars[i], '_');
      }
      return sourceId.ToLowerInvariant();
    }
  }
}
