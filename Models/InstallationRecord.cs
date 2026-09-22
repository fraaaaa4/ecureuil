using System;

namespace Ecureuil.Core.Models {
  public class InstallationRecord {
    private string _appId;
    private string _installedVersion;
    private DateTime? _installedOn;
    private string _installDirectory;
    private string _shortcutPath;
    private string _fileType;
    private string _sourceId;

    public string AppId {
      get { return _appId; }
      set { _appId = value; }
    }

    public string InstalledVersion {
      get { return _installedVersion; }
      set { _installedVersion = value; }
    }

    public DateTime? InstalledOn {
      get { return _installedOn; }
      set { _installedOn = value; }
    }

    public string InstallDirectory {
      get { return _installDirectory; }
      set { _installDirectory = value; }
    }

    public string ShortcutPath {
      get { return _shortcutPath; }
      set { _shortcutPath = value; }
    }

    public string FileType {
      get { return _fileType; }
      set { _fileType = value; }
    }

    public string SourceId {
      get { return _sourceId; }
      set { _sourceId = value; }
    }
  }
}
