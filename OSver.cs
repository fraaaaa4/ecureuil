using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Ecureuil.Core.Helpers {
  public static class OSver {

  //needs to check also device architecture for RT
  [StructLayout(LayoutKind.Sequential)]
  private struct SYSTEM_INFO {
    public ushort wProcessorArchitecture;
    public ushort wReserved;
    [MarshalAs(UnmnagedType.ByValArray, SizeConst = 60)]
    private byte[] _reservedPadding; //Windows needs a pointer as big as the entire SYSTEM_INFO
  }

  [DllImport("kernel32.dll", SetLastError = false)]
  private static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

  private const ushort PROCESSOR_ARCHITECTURE_32 = 0; //x86-32
  private const ushort PROCESSOR_ARCHITECTURE_ARM32 = 5; //arm32
  private const ushort PROCESSOR_ARCHITECTURE_64 = 9; //x86-64
  private const ushort PROCESSOR_ARCHITECTURE_ARM64 = 12; //arm64

  public static string GetArchitecture(){
    try {
      SYSTEM_INFO sysInfo;
      GetNativeSystemInfo(out sysInfo);

      switch (sysInfo.wProcessorArchitecture) {
        case PROCESSOR_ARCHITECTURE_32:
          return "x86-32";
        break;
        case PROCESSOR_ARCHITECTURE_64:
          return "x86-64";
        break;
        case PROCESSOR_ARCHITECTURE_ARM32:
          return "arm32";
        break;
        case PROCESSOR_ARCHITECTURE_ARM64:
          return "arm64";
        break;
        default:
          return "unknown";
        break;
      }
    catch (Exception ex) {
      Console.WriteLine($"Error in getting processor architecture: {ex.Message}");
      return null;
    }
  }
  
  //GET IT FROM LOGIX maybe
    public static string GetCurrentOSVersion() {
      var version = Environment.OSVersion.Version;

      switch (version.Major) {
        case 10: //10 or 11
          return "10";
        break;

        case 6:
          switch (version.Minor) {
            case 4:
              return "8.2"; //build 9941 or stuff like that
            break;
            case 3:
              return "8.1"; //build 9600
            break;
            case 2:
              return "8.0"; //build 9200
            break;
            case 1:
              return "7"; //build 7600
            break;
            case 0:
              return "Vista"; //build 6000
            break;
          }
        break;

        case 5:
          switch (version.Minor) {
            case 2:
            case 1:
              return "XP"; //S2003, XP 64-bit or XP; 3790 or 2600
            break;

            case 0:
              return "2000"; //build 2195
            break;
          }
        break;

        case 4:
          return "NT 4";
        break;
      }

      return "Windows"; //fallback
    }

    //verifies compatibility list of the app with your current Windows version
    public static bool IsAppCompatible(List<String> appCompatibilityList) {
      if (appCompatibilityList == null || appCompatibilityList.Count == 0) return true;

      return appCompatibilityList.Contains(GetCurrentOSVersion());
    }

    //verifies app architecture with your current one
    public static bool IsArchCompatible(string appArch) {
      if (string.IsNullOrWhiteSpace(appArch) || appArch.Equals("neutral", StringComparison.OrdinalIgnoreCase))
        return true;

      string currentArch = GetArchitecture();
      if (currentArch.Equals(appArch, StringComparison.OrdinalIgnoreCase)) return true;

      if (currentArch == "x86-64" && (appArch.Equals("x86", StringComparison.OrdinalIgnoreCase) || appArch.Equals("x86-32", StringComparison.OrdinalIgnoreCase))) return true;

      if (currentArch == "arm64" && (appArch.Equals("arm32", StringComparison.OrdinalIgnoreCase) || appArch.Equals("arm", StringComparison.OrdinalIgnoreCase))) return true;

      return false;
    }
  }
}
