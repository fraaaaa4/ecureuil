using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
      OperatingSystem os = Environment.OSVersion;
      Version version = os.Version;

      if (os.Platform == PlatformID.Win32Windows) {
        if (version.Major == 4) {
          if (version.Minor == 0) return "95";
          if (version.Minor == 10) return "98";
          if (version.Minor == 90) return "ME";
        }
      }

      switch (version.Major) {
        case 10:
          if (version.Build >= 22000) return "11";
          return "10";

        case 6:
          switch (version.Minor) {
            case 4:
              return "8.2";
            case 3:
              return "8.1";
            case 2:
              return "8.0";
            case 1:
              return "7";
            case 0:
              return "Vista";
          }
          break;

        case 5:
          switch (version.Minor) {
            case 2:
            case 1:
              return "XP";
            case 0:
              return "2000";
          }
          break;

        case 4:
          return "NT 4";

        case 3:
          if (version.Minor == 51) return "NT 3.51";
          break;
      }

      return "Windows";
    }

    // Verifica compatibilita della lista OS con la versione corrente
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
