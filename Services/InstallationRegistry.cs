using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ecureuil.Core.Models;
using System.Text.Json;

namespace Ecureuil.Core.Services {
  public class InstallationRegistry {
    private readonly string _registryFilePath;
    private Dictionary<string, InstallationRecord> _installedApps;

    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
      WriteIndented = true,
      PropertyNameCaseInsensitive = true
    };

    public InstallationRegistry() {
      string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string folder = Path.Combine(appData, "Ecureuil");
      if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

      _registryFilePath = Path.Combine(folder, "installed_apps.json");
      LoadRegistry();
    }

    private void LoadRegistry() {
      try {
        if (File.Exists(_registryFilePath)) {
          string json = File.ReadAllText(_registryFilePath);

          var list = JsonSerializer.Deserialize<List<InstallationRecord>>(json, _jsonOptions);

          _installedApps = list != null
            ? list.ToDictionary(r => r.AppId, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, InstallationRecord>(StringComparer.OrdinalIgnoreCase);
          return;
        }
      } catch (Exception ex) {
        Console.WriteLine($"Error in loading the installation registry: {ex.Message}");
      }
      _installedApps = new Dictionary<string, InstallationRecord>(StringComparer.OrdinalIgnoreCase);
    }

    //save the entire installation registry to json on disk
    public void SaveRegistry() {
      try {
        string json = JsonSerializer.Serialize(_installedApps.Values.ToList(), _jsonOptions);
        File.WriteAllText(_registryFilePath, json);
      } catch (Exception ex) {
        Console.WriteLine($"Error in saving the installation registry: {ex.Message}");
      }
    }

    public void RegisterApp(InstallationRecord record) {
      if (record == null || string.IsNullOrEmpty(record.AppId)) return;
      _installedApps[record.AppId] = record;
      SaveRegistry();
    }

    public void UnregisterApp(string appId) {
      if (_installedApps.ContainsKey(appId)) {
        _installedApps.Remove(appId);
        SaveRegistry();
      }
    }

    public InstallationRecord GetRecord(string appId) {
      if (_installedApps.TryGetValue(appId, out var record)) return record;
      return null;
    }

    public bool IsInstalled(string appId) {
      return _installedApps.ContainsKey(appId);
    }

    public void ApplyStatusToApps(List<AppModel> apps) {
      if (apps == null) return;

      foreach (var app in apps) {
        var record = GetRecord(app.id);
        if (record != null) {
          app.isInstalled = true;
          app.installedOn = record.InstalledOn;
          app.installedShortcutPath = record.ShortcutPath;
          app.installDirectory = record.InstallDirectory;
        } else {
          app.isInstalled = false;
          app.installedOn = null;
          app.installedShortcutPath = null;
          app.installDirectory = null;
        }
      }
    }
  }
}
