using System;
using System.IO;
using System.IO.Compression;
using Ecureuil.Core.Models;

namespace Ecureuil.Core.Services {
  public class InstallerService {
    private readonly string _defaultInstallPath;
    private readonly InstallationRegistry _installationRegistry;

    public InstallerService(InstallationRegistry installationRegistry) {
      _installationRegistry = installationRegistry;
      _defaultInstallPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps");
      //default is Program Files, will be editable by the user

      if (!Directory.Exists(_defaultInstallPath))
        Directory.CreateDirectory(_defaultInstallPath);
    }

    //installs an app based on the file type
    /*  zip: extracts the zip to the program folder
        exe: copies the executable to the program folder
        setup: just starts the installer
    */
    public bool InstallApp(AppModel app, string downloadedFilePath) {
      try {
        string targetFolder = Path.Combine(_defaultInstallPath, app.id);
        if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

        string fileType = string.IsNullOrWhiteSpace(app.fileType) ? "zip" : app.FileType.ToLower();
        string executablePath = "";

        switch (fileType) {
          //zip extracts the zip contents to the app folder
          case "zip":
            if (Directory.Exists(targetFolder)) {
              Directory.Delete(targetFolder, true);
            }
            Directory.CreateDirectory(targetFolder);
            ZipFile.ExtractToDirectory(downloadedFilePath, targetFolder);
            executablePath = Path.Combine(targetFolder, app.ExePath ?? "");
            break;

          //exe just copies it into the folder of the app
          case "exe":
            string destExeName = string.IsNullOrWhiteSpace(app.ExePath)
              ? Path.GetFileName(downloadedFilePath)
              : app.ExePath;
            executablePath = Path.Combine(targetFolder, destExeName);
            File.Copy(downloadedFilePath, executablePath, true);
            break;

          //msi or setup just executes it
          case "msi":
          case "setup":
            var process = Process.Start(downloadedFilePath, "");
            process?.WaitForExit();
            executablePath = downloadedFilePath;
            break;

          //fallback is to just copy it
          default:
           string destExeName = string.IsNullOrWhiteSpace(app.ExePath)
              ? Path.GetFileName(downloadedFilePath)
              : app.ExePath;
            executablePath = Path.Combine(targetFolder, destExeName);
            File.Copy(downloadedFilePath, executablePath, true);
          break;
        }

        //generates start menu shortcut, CHECK FOR USER CHOICE
        string shortcutPath = CreateStartShortcut(app, executablePath);

        DateTime now = DateTime.Now;
        app.isInstalled = true;
        app.installedOn = now;
        app.installDirectory = targetFolder;
        app.installedShortcutPath = shortcutPath;

        //save installed app status in the local registry
        _registry.RegisterApp(new InstallationRecord
        {
          AppId = app.id,
          InstalledVersion = app.version,
          InstalledOn = now,
          InstallDirectory = targetFolder,
          ShortcutPath = shortcutPath,
          FileType = fileType,
          SourceId = app.source
        });

        if (File.Exists(downloadedFilePath)) {
          try { File.Delete(downloadedFilePath); } catch { }
        }

        return true;
        
      } catch (Exception ex) {
        Console.WriteLine($"Error in installing {app.name}: {ex.Message}");
      }
    }

    //create a shortcut for the app in the start menu
    //look for the icon also
    private string CreateStartShortcut(AppModel app, string exePath) {
      try {
        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        string shortcutPath = Path.Combine(startMenu, $"{app.Name}.lnk");

        //creating the lnk file
        Type shellType = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
        dynamic shell = Activator.CreateInstance(shellType);

        var shortcut = shell.CreateShortcut(shortcutPath);
        shortcut.TargetPath = exePath; //executable
        shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
        shortcut.Description = app.Description ?? app.Name;

        shortcut.Save();
        return shortcutPath;
      } catch (Exception ex) {
        Console.WriteLine($"Error in creating start menu shortcut: {ex.Message}");
        return null;
      }
    }
  }
}
