using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Ecureuil.Core.Models;
using Ecureuil.Properties;

namespace Ecureuil.Core.Services {
  public class InstallerService {
    private readonly string _defaultInstallPath;
    private readonly InstallationRegistry _installationRegistry;

    public InstallerService(InstallationRegistry installationRegistry) {
      _installationRegistry = installationRegistry;

      string basePath = null;
      try {
        if (!Settings.Default.installSetting && !string.IsNullOrEmpty(Settings.Default.installPath)) {
          basePath = Settings.Default.installPath;
        }
      } catch { }

      if (string.IsNullOrEmpty(basePath)) {
        basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps");
      }

      _defaultInstallPath = basePath;

      try {
        if (!Directory.Exists(_defaultInstallPath)) {
          Directory.CreateDirectory(_defaultInstallPath);
        }
      } catch { }
    }

    private string argsInstall(List<string> installParameters) {
      return (installParameters != null && installParameters.Count > 0)
        ? string.Join(" ", installParameters.ToArray())
        : "";
    }

    [DllImport("shell32.dll", EntryPoint = "ShellExecute", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

    public static void OpenAddRemovePrograms() {
      // 1. ShellExecute on control.exe (Windows searches %windir% and system32 automatically)
      try {
        IntPtr res = ShellExecute(IntPtr.Zero, "open", "control.exe", "appwiz.cpl", null, 1);
        if ((long)res > 32) return;
      } catch { }

      // 2. ShellExecute on rundll32.exe
      try {
        IntPtr res = ShellExecute(IntPtr.Zero, "open", "rundll32.exe", "shell32.dll,Control_RunDLL appwiz.cpl", null, 1);
        if ((long)res > 32) return;
      } catch { }

      // 3. cmd.exe /c start "" appwiz.cpl (Executes shell association command in all Windows versions)
      try {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c start \"\" appwiz.cpl");
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        Process p = Process.Start(psi);
        if (p != null) return;
      } catch { }

      // 4. cmd.exe /c control.exe appwiz.cpl
      try {
        ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", "/c control.exe appwiz.cpl");
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        Process p = Process.Start(psi);
        if (p != null) return;
      } catch { }

      // 5. Direct ProcessStartInfo
      try {
        ProcessStartInfo psi = new ProcessStartInfo("control.exe", "appwiz.cpl");
        psi.UseShellExecute = true;
        Process.Start(psi);
        return;
      } catch { }

      // 6. Direct Process.Start fallback
      try {
        Process.Start("appwiz.cpl");
      } catch { }
    }

    // Installs app based on the type of file
    public bool InstallApp(AppModel app, string downloadedFilePath) {
      if (app == null || string.IsNullOrEmpty(downloadedFilePath) || !File.Exists(downloadedFilePath)) {
        return false;
      }

      try {
        string fileType = "";
        if (!string.IsNullOrEmpty(app.fileType)) {
          fileType = app.fileType.Trim().ToLower().TrimStart('.');
        } else if (!string.IsNullOrEmpty(downloadedFilePath)) {
          fileType = Path.GetExtension(downloadedFilePath).Trim().ToLower().TrimStart('.');
        }
        if (string.IsNullOrEmpty(fileType)) fileType = "zip";

        string targetFolder = Path.Combine(_defaultInstallPath, app.id);
        string executablePath = "";
        string shortcutPath = null;
        bool isInstallerPackage = false;

        if (fileType == "msi" || downloadedFilePath.EndsWith(".msi", StringComparison.OrdinalIgnoreCase)) {
          isInstallerPackage = true;
          string args = argsInstall(app.installParameters);
          ProcessStartInfo psi = new ProcessStartInfo();
          psi.FileName = "msiexec.exe";
          psi.Arguments = "/i \"" + downloadedFilePath + "\"" + (!string.IsNullOrEmpty(args) ? (" " + args) : "");
          psi.UseShellExecute = true;

          Process process = Process.Start(psi);
          if (process != null) {
            process.WaitForExit();
            if (process.ExitCode != 0 && process.ExitCode != 3010) {
              return false;
            }
          }
        } else if (fileType == "exe" || downloadedFilePath.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)) {
          if (!app.isPortable) {
            // Installer EXE (setup / wizard wizard that installs into OS)
            isInstallerPackage = true;
            string args = argsInstall(app.installParameters);
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = downloadedFilePath;
            psi.Arguments = args;
            psi.UseShellExecute = true;

            Process process = Process.Start(psi);
            if (process != null) {
              process.WaitForExit();
              if (process.ExitCode != 0 && process.ExitCode != 3010) {
                return false;
              }
            }
          } else {
            // Portable standalone EXE managed by Ecureuil
            if (!Directory.Exists(targetFolder)) {
              Directory.CreateDirectory(targetFolder);
            }
            string destExeName = !string.IsNullOrEmpty(app.downloadPath)
              ? app.downloadPath
              : Path.GetFileName(downloadedFilePath);
            executablePath = Path.Combine(targetFolder, destExeName);
            File.Copy(downloadedFilePath, executablePath, true);
            shortcutPath = CreateStartShortcut(app, executablePath);
          }
        } else if (fileType == "zip" || fileType == "7z" || downloadedFilePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) || downloadedFilePath.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)) {
          if (!app.isPortable) {
            // Installer packaged inside a ZIP/7Z archive
            isInstallerPackage = true;
            string tempExtractDir = Path.Combine(Path.GetTempPath(), "Ecureuil_Setup_" + Guid.NewGuid().ToString("N"));
            if (Directory.Exists(tempExtractDir)) {
              Directory.Delete(tempExtractDir, true);
            }
            Directory.CreateDirectory(tempExtractDir);

            try {
              if (fileType == "7z" || downloadedFilePath.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)) {
                if (!Extract7z(downloadedFilePath, tempExtractDir)) {
                  return false;
                }
              } else {
                ExtractZip(downloadedFilePath, tempExtractDir);
              }

              string targetInstaller = !string.IsNullOrEmpty(app.downloadPath)
                ? Path.Combine(tempExtractDir, app.downloadPath)
                : "";

              if (string.IsNullOrEmpty(targetInstaller) || !File.Exists(targetInstaller)) {
                string[] exes = Directory.GetFiles(tempExtractDir, "*.exe", SearchOption.AllDirectories);
                if (exes != null && exes.Length > 0) {
                  targetInstaller = exes[0];
                } else {
                  string[] msis = Directory.GetFiles(tempExtractDir, "*.msi", SearchOption.AllDirectories);
                  if (msis != null && msis.Length > 0) {
                    targetInstaller = msis[0];
                  }
                }
              }

              if (string.IsNullOrEmpty(targetInstaller) || !File.Exists(targetInstaller)) {
                return false;
              }

              string args = argsInstall(app.installParameters);
              ProcessStartInfo psi = new ProcessStartInfo();
              if (targetInstaller.EndsWith(".msi", StringComparison.OrdinalIgnoreCase)) {
                psi.FileName = "msiexec.exe";
                psi.Arguments = "/i \"" + targetInstaller + "\"" + (!string.IsNullOrEmpty(args) ? (" " + args) : "");
              } else {
                psi.FileName = targetInstaller;
                psi.Arguments = args;
              }
              psi.WorkingDirectory = Path.GetDirectoryName(targetInstaller);
              psi.UseShellExecute = true;

              Process process = Process.Start(psi);
              if (process != null) {
                process.WaitForExit();
                if (process.ExitCode != 0 && process.ExitCode != 3010) {
                  return false;
                }
              }
            } finally {
              try {
                if (Directory.Exists(tempExtractDir)) {
                  Directory.Delete(tempExtractDir, true);
                }
              } catch { }
            }
          } else {
            // Portable ZIP or 7Z archive managed by Ecureuil
            if (Directory.Exists(targetFolder)) {
              Directory.Delete(targetFolder, true);
            }
            Directory.CreateDirectory(targetFolder);

            if (fileType == "7z" || downloadedFilePath.EndsWith(".7z", StringComparison.OrdinalIgnoreCase)) {
              if (!Extract7z(downloadedFilePath, targetFolder)) {
                return false;
              }
            } else {
              ExtractZip(downloadedFilePath, targetFolder);
            }

            string targetExe = !string.IsNullOrEmpty(app.downloadPath)
              ? Path.Combine(targetFolder, app.downloadPath)
              : "";

            if (string.IsNullOrEmpty(targetExe) || !File.Exists(targetExe)) {
              string[] exes = Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories);
              if (exes != null && exes.Length > 0) {
                targetExe = exes[0];
              }
            }

            executablePath = targetExe;
            if (!string.IsNullOrEmpty(executablePath) && File.Exists(executablePath)) {
              shortcutPath = CreateStartShortcut(app, executablePath);
            }
          }
        } else {
          // Fallback standalone copy
          if (!Directory.Exists(targetFolder)) {
            Directory.CreateDirectory(targetFolder);
          }
          string fallbackExeName = !string.IsNullOrEmpty(app.downloadPath)
            ? app.downloadPath
            : Path.GetFileName(downloadedFilePath);
          executablePath = Path.Combine(targetFolder, fallbackExeName);
          File.Copy(downloadedFilePath, executablePath, true);
          shortcutPath = CreateStartShortcut(app, executablePath);
        }

        DateTime now = DateTime.Now;
        app.isInstalled = true;
        app.installedOn = now;
        app.installDirectory = isInstallerPackage ? null : targetFolder;
        app.installedShortcutPath = shortcutPath;

        // Salva lo stato nel registro locale
        if (_installationRegistry != null) {
          InstallationRecord record = new InstallationRecord();
          record.AppId = app.id;
          record.InstalledVersion = app.version;
          record.InstalledOn = now;
          record.InstallDirectory = isInstallerPackage ? null : targetFolder;
          record.ShortcutPath = shortcutPath;
          record.FileType = fileType;
          record.SourceId = app.source;

          _installationRegistry.RegisterApp(record);
        }

        // Elimina sempre il file temporaneo scaricato (MSI, setup EXE o zip)
        if (File.Exists(downloadedFilePath)) {
          try {
            File.Delete(downloadedFilePath);
          } catch { }
        }

        return true;
      } catch (Exception ex) {
        Console.WriteLine("Error in installing " + app.name + ": " + ex.Message);
        return false;
      }
    }

    public bool UninstallApp(AppModel app) {
      if (app == null) return false;
      try {
        if (!string.IsNullOrEmpty(app.installedShortcutPath) && File.Exists(app.installedShortcutPath)) {
          try { File.Delete(app.installedShortcutPath); } catch { }
        }

        if (!string.IsNullOrEmpty(app.installDirectory) && Directory.Exists(app.installDirectory)) {
          try { Directory.Delete(app.installDirectory, true); } catch { }
        }

        if (app.uninstallParameters != null && app.uninstallParameters.Count > 0) {
          string uninstCmd = app.uninstallParameters[0];
          string uninstArgs = app.uninstallParameters.Count > 1
            ? string.Join(" ", app.uninstallParameters.GetRange(1, app.uninstallParameters.Count - 1).ToArray())
            : "";
          try {
            Process uninstProc = Process.Start(uninstCmd, uninstArgs);
            if (uninstProc != null) uninstProc.WaitForExit();
          } catch { }
        }

        app.isInstalled = false;
        app.installedOn = null;
        app.installDirectory = null;
        app.installedShortcutPath = null;

        if (_installationRegistry != null) {
          _installationRegistry.UnregisterApp(app.id);
        }
        return true;
      } catch (Exception ex) {
        Console.WriteLine("Error uninstalling " + app.name + ": " + ex.Message);
        return false;
      }
    }

    // Estrazione ZIP pura managed in C# (.NET 2.0 / Windows 2000 / RT) con fallback su Shell.Application e 7-Zip
    private void ExtractZip(string zipFilePath, string destinationDirectory) {
      // 1. Estrazione diretta managed via DeflateStream (funziona nativamente su Windows 2000, XP, Vista, 7, 8, RT, 10, 11)
      if (ExtractZipManaged(zipFilePath, destinationDirectory)) {
        return;
      }

      // 2. Fallback COM Shell.Application (se supportato dal sistema es. XP/7/10)
      try {
        Type shellType = Type.GetTypeFromProgID("Shell.Application");
        if (shellType != null) {
          string fullZipPath = Path.GetFullPath(zipFilePath);
          string fullDestPath = Path.GetFullPath(destinationDirectory);

          object shell = Activator.CreateInstance(shellType);
          try {
            object destFolder = shellType.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[] { fullDestPath });
            object zipFile = shellType.InvokeMember("NameSpace", BindingFlags.InvokeMethod, null, shell, new object[] { fullZipPath });

            if (destFolder != null && zipFile != null) {
              Type folderType = zipFile.GetType();
              object items = folderType.InvokeMember("Items", BindingFlags.InvokeMethod, null, zipFile, null);

              Type destFolderType = destFolder.GetType();
              destFolderType.InvokeMember("CopyHere", BindingFlags.InvokeMethod, null, destFolder, new object[] { items, 20 });
              return;
            }
          } finally {
            if (shell != null && Marshal.IsComObject(shell)) {
              Marshal.ReleaseComObject(shell);
            }
          }
        }
      } catch { }

      // 3. Fallback a 7-Zip se installato
      if (Extract7z(zipFilePath, destinationDirectory)) {
        return;
      }

      throw new InvalidOperationException("Could not extract ZIP archive on this system.");
    }

    private static bool ExtractZipManaged(string zipFilePath, string destinationDirectory) {
      if (string.IsNullOrEmpty(zipFilePath) || !File.Exists(zipFilePath)) return false;
      if (string.IsNullOrEmpty(destinationDirectory)) return false;

      if (!Directory.Exists(destinationDirectory)) {
        Directory.CreateDirectory(destinationDirectory);
      }

      try {
        using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
        using (BinaryReader reader = new BinaryReader(fs)) {
          long eocdOffset = FindEOCD(fs, reader);
          if (eocdOffset < 0) return false;

          fs.Seek(eocdOffset + 10, SeekOrigin.Begin);
          ushort totalEntries = reader.ReadUInt16();
          uint centralDirSize = reader.ReadUInt32();
          uint centralDirOffset = reader.ReadUInt32();

          fs.Seek(centralDirOffset, SeekOrigin.Begin);
          for (int i = 0; i < totalEntries; i++) {
            uint signature = reader.ReadUInt32();
            if (signature != 0x02014b50) break;

            reader.ReadUInt16(); // version made by
            reader.ReadUInt16(); // version needed
            ushort flags = reader.ReadUInt16();
            ushort method = reader.ReadUInt16();
            reader.ReadUInt16(); // mod time
            reader.ReadUInt16(); // mod date
            uint crc32 = reader.ReadUInt32();
            uint compressedSize = reader.ReadUInt32();
            uint uncompressedSize = reader.ReadUInt32();
            ushort fileNameLen = reader.ReadUInt16();
            ushort extraFieldLen = reader.ReadUInt16();
            ushort commentLen = reader.ReadUInt16();
            reader.ReadUInt16(); // disk number start
            reader.ReadUInt16(); // internal attrs
            uint externalAttrs = reader.ReadUInt32();
            uint localHeaderOffset = reader.ReadUInt32();

            byte[] fileNameBytes = reader.ReadBytes(fileNameLen);
            string entryName = ((flags & (1 << 11)) != 0)
              ? Encoding.UTF8.GetString(fileNameBytes)
              : Encoding.Default.GetString(fileNameBytes);

            if (extraFieldLen > 0) fs.Seek(extraFieldLen, SeekOrigin.Current);
            if (commentLen > 0) fs.Seek(commentLen, SeekOrigin.Current);

            long currentCdPos = fs.Position;

            entryName = entryName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            string targetPath = Path.Combine(destinationDirectory, entryName);

            bool isDirectory = entryName.EndsWith(Path.DirectorySeparatorChar.ToString()) || (externalAttrs & 0x10) != 0;

            if (isDirectory) {
              if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);
            } else {
              string targetDir = Path.GetDirectoryName(targetPath);
              if (!string.IsNullOrEmpty(targetDir) && !Directory.Exists(targetDir)) {
                Directory.CreateDirectory(targetDir);
              }

              fs.Seek(localHeaderOffset, SeekOrigin.Begin);
              uint localSig = reader.ReadUInt32();
              if (localSig == 0x04034b50) {
                fs.Seek(22, SeekOrigin.Current);
                ushort localNameLen = reader.ReadUInt16();
                ushort localExtraLen = reader.ReadUInt16();
                fs.Seek(localNameLen + localExtraLen, SeekOrigin.Current);

                using (FileStream outStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write)) {
                  if (method == 0) {
                    CopyStreamBytes(fs, outStream, (long)uncompressedSize);
                  } else if (method == 8) {
                    using (DeflateStream deflate = new DeflateStream(fs, CompressionMode.Decompress, true)) {
                      CopyStreamBytes(deflate, outStream, (long)uncompressedSize);
                    }
                  }
                }
              }
            }

            fs.Seek(currentCdPos, SeekOrigin.Begin);
          }

          return true;
        }
      } catch (Exception ex) {
        Console.WriteLine("Managed zip extraction exception: " + ex.Message);
        return false;
      }
    }

    private static void CopyStreamBytes(Stream source, Stream destination, long totalBytesToRead) {
      byte[] buffer = new byte[8192];
      long bytesRemaining = totalBytesToRead;
      while (bytesRemaining > 0) {
        int toRead = (int)Math.Min((long)buffer.Length, bytesRemaining);
        int read = source.Read(buffer, 0, toRead);
        if (read <= 0) break;
        destination.Write(buffer, 0, read);
        bytesRemaining -= read;
      }
    }

    private static long FindEOCD(Stream stream, BinaryReader reader) {
      long length = stream.Length;
      if (length < 22) return -1;

      long maxBack = Math.Min(65557, length);
      long startPos = length - maxBack;
      stream.Seek(startPos, SeekOrigin.Begin);
      byte[] buffer = reader.ReadBytes((int)maxBack);

      for (int i = buffer.Length - 22; i >= 0; i--) {
        if (buffer[i] == 0x50 && buffer[i + 1] == 0x4b && buffer[i + 2] == 0x05 && buffer[i + 3] == 0x06) {
          return startPos + i;
        }
      }
      return -1;
    }

    private static string Find7ZipExecutable() {
      string pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
      string p1 = Path.Combine(pf, @"7-Zip\7z.exe");
      if (File.Exists(p1)) return p1;
      string p2 = Path.Combine(pf, @"7-Zip\7za.exe");
      if (File.Exists(p2)) return p2;
      string p3 = Path.Combine(pf, @"EcureuilApps\7-zip\7z.exe");
      if (File.Exists(p3)) return p3;
      return "7z.exe";
    }

    private bool Extract7z(string archivePath, string destinationDirectory) {
      string sevenZipExe = Find7ZipExecutable();
      try {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = sevenZipExe;
        psi.Arguments = "x -y -o\"" + destinationDirectory + "\" \"" + archivePath + "\"";
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        Process p = Process.Start(psi);
        if (p != null) {
          p.WaitForExit();
          return p.ExitCode == 0;
        }
      } catch (Exception ex) {
        Console.WriteLine("7-Zip extraction exception: " + ex.Message);
      }
      return false;
    }

    // Crea il collegamento (.lnk) nel Menu Start usando WScript.Shell via Reflection (compatibile C# 2.0)
    private string CreateStartShortcut(AppModel app, string exePath) {
      try {
        if (!Settings.Default.StartMenuLink) {
          return null;
        }

        if (string.IsNullOrEmpty(exePath)) return null;

        string startMenu = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        string appName = !string.IsNullOrEmpty(app.name) ? app.name : "App";
        string shortcutPath = Path.Combine(startMenu, appName + ".lnk");

        Type shellType = Type.GetTypeFromProgID("WScript.Shell");
        if (shellType == null) {
          shellType = Type.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8"));
        }

        if (shellType == null) {
          return null;
        }

        object shell = Activator.CreateInstance(shellType);
        try {
          object shortcut = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { shortcutPath });
          if (shortcut != null) {
            string workDir = Path.GetDirectoryName(exePath);
            if (string.IsNullOrEmpty(workDir)) workDir = _defaultInstallPath;

            Type shortcutType = shortcut.GetType();
            shortcutType.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut, new object[] { exePath });
            shortcutType.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, shortcut, new object[] { workDir });
            shortcutType.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut, new object[] { !string.IsNullOrEmpty(app.description) ? app.description : appName });
            shortcutType.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut, null);

            if (Marshal.IsComObject(shortcut)) {
              Marshal.ReleaseComObject(shortcut);
            }
          }
        } finally {
          if (shell != null && Marshal.IsComObject(shell)) {
            Marshal.ReleaseComObject(shell);
          }
        }

        return shortcutPath;
      } catch (Exception ex) {
        Console.WriteLine("Error in creating start menu shortcut: " + ex.Message);
        return null;
      }
    }
  }
}
