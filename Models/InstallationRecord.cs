using System;

namespace Ecureuil.Core.Models {
  public class InstallationRecord {
    public string AppId { get; set; }
    public string InstalledVersion { get; set; }
    public DateTime? InstalledOn { get; set; }
    public string InstallDirectory { get; set; }
    public string ShortcutPath { get; set; }
    public string FileType { get; set; }
    public string SourceId { get; set; }
  }
}
