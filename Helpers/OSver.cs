using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Ecureuil.Core.Helpers {
  public static class OSver {

    // Struct per rilevare architettura (x86, ARM, x64, ARM64)
    [StructLayout(LayoutKind.Sequential)]
    private struct SYSTEM_INFO {
      public ushort wProcessorArchitecture;
      public ushort wReserved;
      public uint dwPageSize;
      public IntPtr lpMinimumApplicationAddress;
      public IntPtr lpMaximumApplicationAddress;
      public IntPtr dwActiveProcessorMask;
      public uint dwNumberOfProcessors;
      public uint dwProcessorType;
      public uint dwAllocationGranularity;
      public ushort wProcessorLevel;
      public ushort wProcessorRevision;
    }

    [DllImport("kernel32.dll", SetLastError = false)]
    private static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

    [DllImport("kernel32.dll", SetLastError = false)]
    private static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

    private const ushort PROCESSOR_ARCHITECTURE_32 = 0; // x86-32
    private const ushort PROCESSOR_ARCHITECTURE_ARM32 = 5; // arm32
    private const ushort PROCESSOR_ARCHITECTURE_64 = 9; // x86-64
    private const ushort PROCESSOR_ARCHITECTURE_ARM64 = 12; // arm64

    public static string GetArchitecture() {
      try {
        SYSTEM_INFO sysInfo;
        try {
          // GetNativeSystemInfo e presente da Windows XP/2003 in poi
          GetNativeSystemInfo(out sysInfo);
        } catch {
          // Fallback per Windows 2000 / NT 4
          GetSystemInfo(out sysInfo);
        }

        switch (sysInfo.wProcessorArchitecture) {
          case PROCESSOR_ARCHITECTURE_32:
            return "x86-32";
          case PROCESSOR_ARCHITECTURE_64:
            return "x86-64";
          case PROCESSOR_ARCHITECTURE_ARM32:
            return "arm32";
          case PROCESSOR_ARCHITECTURE_ARM64:
            return "arm64";
          default:
            return "unknown";
        }
      } catch (Exception ex) {
        Console.WriteLine("Error in getting processor architecture: " + ex.Message);
        return "unknown";
      }
    }

    public static string GetCurrentOSVersion() {
        string currentVersion = null;
        using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
        {
            if (key != null)
            {
                currentVersion = key.GetValue("CurrentVersion") as string;
            }
            else
            {
                return "No.";
            }
        }

        switch (currentVersion)
        {
            case "4.0": return "95";
            case "4.10": return "98";
            case "4.90": return "ME";
            case "10":
                return Environment.OSVersion.Version.Build >= 21996 ? "11" : "10";
            case "6.4": return "8.2";
            case "6.3": return "8.1";
            case "6.2": return "8.0";
            case "6.1": return "7";
            case "6.0": return "Vista";
            case "5.2": return "XP/2003";
            case "5.1": return "XP";
            case "5.0": return "2000";
            case "4": return "NT 4";
            case "3": return "NT 3.51";
            default: return "Windows";
        }
    }

    // Verify compatibility with current OS
    public static bool IsAppCompatible(List<string> appCompatibilityList) {
      if (appCompatibilityList == null || appCompatibilityList.Count == 0) {
        return true;
      }

      string currentOS = GetCurrentOSVersion();
      for (int i = 0; i < appCompatibilityList.Count; i++) {
        if (string.Compare(appCompatibilityList[i], currentOS, true) == 0) {
          return true;
        }
      }

      return false;
    }

    // Verifica compatibilita architettura
    public static bool IsArchCompatible(string appArch) {
      if (string.IsNullOrEmpty(appArch) || string.Compare(appArch, "neutral", true) == 0) {
        return true;
      }

      string currentArch = GetArchitecture();
      if (string.Compare(currentArch, appArch, true) == 0) {
        return true;
      }

      if (currentArch == "x86-64" && (string.Compare(appArch, "x86", true) == 0 || string.Compare(appArch, "x86-32", true) == 0)) {
        return true;
      }

      if (currentArch == "arm64" && (string.Compare(appArch, "arm32", true) == 0 || string.Compare(appArch, "arm", true) == 0)) {
        return true;
      }

      return false;
    }
  }
}
