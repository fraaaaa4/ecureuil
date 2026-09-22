using System;
using System.Collections.Generic;
using System.IO;
using Ecureuil.Core.Helpers;
using Ecureuil.Core.Models;

namespace Ecureuil.Core.Services {
  public class InstallationRegistry {
    private readonly string _registryFilePath;
    private Dictionary<string, InstallationRecord> _installedApps;

    public InstallationRegistry() {
      string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
      string folder = Path.Combine(appData, "Ecureuil");
      if (!Directory.Exists(folder)) {
        Directory.CreateDirectory(folder);
      }

      _registryFilePath = Path.Combine(folder, "installed_apps.json");
      LoadRegistry();
    }

    private void LoadRegistry() {
      try {
        if (File.Exists(_registryFilePath)) {
          string json = File.ReadAllText(_registryFilePath);
          List<InstallationRecord> list = MiniJson.Deserialize<List<InstallationRecord>>(json);

          _installedApps = new Dictionary<string, InstallationRecord>(StringComparer.OrdinalIgnoreCase);
          if (list != null) {
            for (int i = 0; i < list.Count; i++) {
              InstallationRecord rec = list[i];
              if (rec != null && !string.IsNullOrEmpty(rec.AppId)) {
                _installedApps[rec.AppId] = rec;
              }
            }
          }
          return;
        }
      } catch (Exception ex) {
        Console.WriteLine("Error in loading the installation registry: " + ex.Message);
      }
      _installedApps = new Dictionary<string, InstallationRecord>(StringComparer.OrdinalIgnoreCase);
    }

    // Salva l'intero registro installazioni su disco
    public void SaveRegistry() {
      try {
        List<InstallationRecord> records = new List<InstallationRecord>();
        foreach (KeyValuePair<string, InstallationRecord> kvp in _installedApps) {
          records.Add(kvp.Value);
        }
        string json = MiniJson.Serialize(records);
        File.WriteAllText(_registryFilePath, json);
      } catch (Exception ex) {
        Console.WriteLine("Error in saving the installation registry: " + ex.Message);
      }
    }

    public void RegisterApp(InstallationRecord record) {
      if (record == null || string.IsNullOrEmpty(record.AppId)) return;
      _installedApps[record.AppId] = record;
      SaveRegistry();
    }

    public void UnregisterApp(string appId) {
      if (!string.IsNullOrEmpty(appId) && _installedApps.ContainsKey(appId)) {
        _installedApps.Remove(appId);
        SaveRegistry();
      }
    }

    public InstallationRecord GetRecord(string appId) {
      if (string.IsNullOrEmpty(appId)) return null;
      InstallationRecord record;
      if (_installedApps.TryGetValue(appId, out record)) {
        return record;
      }
      return null;
    }

    public bool IsInstalled(string appId) {
      if (string.IsNullOrEmpty(appId)) return false;
      return _installedApps.ContainsKey(appId);
    }

    public void ApplyStatusToApps(List<AppModel> apps) {
      if (apps == null) return;

      for (int i = 0; i < apps.Count; i++) {
        AppModel app = apps[i];
        if (app == null) continue;

        InstallationRecord record = GetRecord(app.id);
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
