using System;
using System.Collections.Generic;

namespace Ecureuil.Core.Models {

  // Struttura dati equivalente al file app JSON
  public class AppModel {
    private string _id;
    private string _name;
    private string _version;
    private string _author;
    private string _uploader;
    private string _source;
    private List<string> _category;
    private string _description;

    private DateTime? _dateOriginal;
    private DateTime? _dateSource;

    private long _size;
    private List<string> _dependencies;
    private List<string> _instructionSteps;
    private bool _isImmersive;
    private List<string> _links;

    private string _fileType;
    private List<string> _installParameters;
    private string _downloadPath;
    private List<string> _uninstallParameters;
    private bool _isPortable;
    private List<string> _compatibility;
    private string _architecture;

    private string _iconUrl;
    private List<string> _screenshots;
    private string _downloadUrl;
    private List<string> _language;
    private bool _isTrial;

    // Proprieta locali per stato installazione
    private bool _isInstalled;
    private DateTime? _installedOn;
    private string _installedShortcutPath;
    private string _installDirectory;

    public string id { get { return _id; } set { _id = value; } }
    public string name { get { return _name; } set { _name = value; } }
    public string version { get { return _version; } set { _version = value; } }
    public string author { get { return _author; } set { _author = value; } }
    public string uploader { get { return _uploader; } set { _uploader = value; } }
    public string source { get { return _source; } set { _source = value; } }
    public List<string> category { get { return _category; } set { _category = value; } }
    public string description { get { return _description; } set { _description = value; } }

    public DateTime? dateOriginal { get { return _dateOriginal; } set { _dateOriginal = value; } }
    public DateTime? dateSource { get { return _dateSource; } set { _dateSource = value; } }

    public long size { get { return _size; } set { _size = value; } }
    public List<string> dependencies { get { return _dependencies; } set { _dependencies = value; } }
    public List<string> instructionSteps { get { return _instructionSteps; } set { _instructionSteps = value; } }
    public bool isImmersive { get { return _isImmersive; } set { _isImmersive = value; } }
    public List<string> links { get { return _links; } set { _links = value; } }

    public string fileType { get { return _fileType; } set { _fileType = value; } }
    public List<string> installParameters { get { return _installParameters; } set { _installParameters = value; } }
    public string downloadPath { get { return _downloadPath; } set { _downloadPath = value; } }
    public List<string> uninstallParameters { get { return _uninstallParameters; } set { _uninstallParameters = value; } }
    public bool isPortable { get { return _isPortable; } set { _isPortable = value; } }
    public List<string> compatibility { get { return _compatibility; } set { _compatibility = value; } }
    public string architecture { get { return _architecture; } set { _architecture = value; } }

    public string iconUrl { get { return _iconUrl; } set { _iconUrl = value; } }
    public List<string> screenshots { get { return _screenshots; } set { _screenshots = value; } }
    public string downloadUrl { get { return _downloadUrl; } set { _downloadUrl = value; } }
    public List<string> language { get { return _language; } set { _language = value; } }
    public bool isTrial { get { return _isTrial; } set { _isTrial = value; } }

    // Locale
    public bool isInstalled { get { return _isInstalled; } set { _isInstalled = value; } }
    public DateTime? installedOn { get { return _installedOn; } set { _installedOn = value; } }
    public string installedShortcutPath { get { return _installedShortcutPath; } set { _installedShortcutPath = value; } }
    public string installDirectory { get { return _installDirectory; } set { _installDirectory = value; } }

    public AppModel() {
      _category = new List<string>();
      _dependencies = new List<string>();
      _instructionSteps = new List<string>();
      _links = new List<string>();
      _installParameters = new List<string>();
      _uninstallParameters = new List<string>();
      _compatibility = new List<string>();
      _screenshots = new List<string>();
      _language = new List<string>();
    }
    }
  }

    public static class sizeFormat {
        public static string FormattedSize (long _size)
        {
                if (_size < 1024)
                {
                    return _size.ToString() + " B";
                }
                if (_size < 1024 * 1024)
                {
                    return (_size / 1024.0).ToString("F1") + " KB";
                }
                return (_size / (1024.0 * 1024.0)).ToString("F1") + " MB";
    }
}
