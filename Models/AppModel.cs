using System;
using System.Collections.Generic;

namespace Ecureuil.Core.Models {

  //equivalent of your app json in the source
  public class AppModel {
    public string id { get; set; }
    public string name { get; set; }
    public string version { get; set; }
    public string author { get; set; }
    public string uploader { get; set; }
    public string source { get; set; }
    public List<string> category { get; set; }
    public string description { get; set; }

    public DateTime? dateOriginal { get; set; }
    public DateTime? dateSource { get; set; }

    public long size { get; set; }
    public List<string> dependencies { get; set; }
    public List<string> instructionSteps { get; set; }
    public bool isImmersive { get; set; }
    public List<string> links { get; set; }

    public string fileType { get; set; }
    public List<string> installParameters { get; set; }
    public string downloadPath { get; set; }
    public List<string> uninstallParameters { get; set; }
    public bool isPortable { get; set; }
    public List<string> compatibility { get; set; }
    public string architecture { get; set; }

    public string iconUrl { get; set; }
    public List<string> screenshots { get; set; }
    public string downloadUrl { get; set; }
    public List<string> language { get; set; }
    public bool isTrial { get; set; }

    //local - check if app is installed or not
    public bool isInstalled { get; set; }
    public DateTime? installedOn { get; set; }
    public string installedShortcutPath { get; set; }
    public string installDirectory { get; set; }

    public AppModel() {
      category = new List<string>();
      dependencies = new List<string>();
      instructionSteps = new List<string>();
      links = new List<string>();
      installParameters = new List<string>();
      uninstallParameters = new List<string>();
      compatibility = new List<string>();
      screenshots = new List<string>();
      language = new List<string>();
    }

    //converts bytes into the right measure
    public string FormattedSize {
      get {
        if (size < 1024) return $"{size} B";
        if (size < 1024 * 1024) return $"{size / 1024.0:F1} KB";
        return $"{size / (1024.0 * 1024.0):F1} MB";
      }
    }
  }
}
