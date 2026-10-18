using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Ecureuil.Core.Helpers {
  /// <summary>
  /// OpenSSL 3.x native helper for legacy Windows systems.
  /// </summary>
  [SuppressUnmanagedCodeSecurity]
  public static class OpenSslNative {
    [SuppressUnmanagedCodeSecurity]
    private static class Win32Native {
      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

      [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int OpenSslInitSslDelegate(ulong opts, IntPtr settings);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr TlsClientMethodDelegate();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr SslCtxNewDelegate(IntPtr method);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void SslCtxFreeDelegate(IntPtr ctx);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr SslNewDelegate(IntPtr ctx);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate void SslFreeDelegate(IntPtr ssl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslSetFdDelegate(IntPtr ssl, int fd);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate IntPtr SslCtrlDelegate(IntPtr ssl, int cmd, IntPtr larg, [MarshalAs(UnmanagedType.LPStr)] string parg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslConnectDelegate(IntPtr ssl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslWriteDelegate(IntPtr ssl, byte[] buf, int num);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslReadDelegate(IntPtr ssl, byte[] buf, int num);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslShutdownDelegate(IntPtr ssl);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [SuppressUnmanagedCodeSecurity]
    private delegate int SslGetErrorDelegate(IntPtr ssl, int ret);

    private static OpenSslInitSslDelegate _fnInitSsl;
    private static TlsClientMethodDelegate _fnTlsClientMethod;
    private static SslCtxNewDelegate _fnSslCtxNew;
    private static SslCtxFreeDelegate _fnSslCtxFree;
    private static SslNewDelegate _fnSslNew;
    private static SslFreeDelegate _fnSslFree;
    private static SslSetFdDelegate _fnSslSetFd;
    private static SslCtrlDelegate _fnSslCtrl;
    private static SslConnectDelegate _fnSslConnect;
    private static SslWriteDelegate _fnSslWrite;
    private static SslReadDelegate _fnSslRead;
    private static SslShutdownDelegate _fnSslShutdown;
    private static SslGetErrorDelegate _fnSslGetError;

    private static IntPtr _hCrypto = IntPtr.Zero;
    private static IntPtr _hSsl = IntPtr.Zero;

    private const int SSL_CTRL_SET_TLSEXT_HOSTNAME = 55;
    private const int TLSEXT_NAMETYPE_host_name = 0;

    private static bool _initialized = false;
    private static bool _available = false;
    private static string _initErrorMessage = "";

    public static string InitErrorMessage {
      get { return _initErrorMessage; }
    }

    public static bool IsAvailable {
      get {
        if (!_initialized) {
          try {
            _available = CheckAvailability();
          } catch (Exception ex) {
            _initErrorMessage = ex.Message;
            _available = false;
          }
          _initialized = true;
        }
        return _available;
      }
    }

    private static T GetExport<T>(IntPtr hSsl, IntPtr hCrypto, string procName) where T : class {
      IntPtr proc = Win32Native.GetProcAddress(hSsl, procName);
      if (proc == IntPtr.Zero && hCrypto != IntPtr.Zero) {
        proc = Win32Native.GetProcAddress(hCrypto, procName);
      }
      if (proc == IntPtr.Zero) {
        throw new EntryPointNotFoundException("Function not found in OpenSSL: " + procName + " (Win32 Error: " + Marshal.GetLastWin32Error() + ")");
      }
      return Marshal.GetDelegateForFunctionPointer(proc, typeof(T)) as T;
    }

    private static bool CheckAvailability() {
      if (IntPtr.Size != 4) {
        _initErrorMessage = "OpenSSL native DLLs are 32-bit; current process is 64-bit.";
        return false;
      }

      string arch = OSver.GetArchitecture();
      bool isArm = string.Equals(arch, "arm32", StringComparison.OrdinalIgnoreCase);

      string appDir = AppDomain.CurrentDomain.BaseDirectory;
      string cryptoPath;
      string sslPath;

      if (isArm) {
        // Su Windows RT / ARM32, libunwind.dll e una dipendenza necessaria di OpenSSL ARM32
        string unwindPath = Path.Combine(appDir, "libunwind.dll");
        if (File.Exists(unwindPath)) {
          Win32Native.LoadLibrary(unwindPath);
        }

        cryptoPath = Path.Combine(appDir, "libcrypto-3-arm.dll");
        if (!File.Exists(cryptoPath)) cryptoPath = Path.Combine(appDir, "libcrypto-3.dll");

        sslPath = Path.Combine(appDir, "libssl-3-arm.dll");
        if (!File.Exists(sslPath)) sslPath = Path.Combine(appDir, "libssl-3.dll");
      } else {
        // x86 32-bit
        cryptoPath = Path.Combine(appDir, "libcrypto-3.dll");
        sslPath = Path.Combine(appDir, "libssl-3.dll");
      }

      if (!File.Exists(cryptoPath)) {
        _initErrorMessage = Path.GetFileName(cryptoPath) + " not found at: " + cryptoPath;
        return false;
      }
      if (!File.Exists(sslPath)) {
        _initErrorMessage = Path.GetFileName(sslPath) + " not found at: " + sslPath;
        return false;
      }

      _hCrypto = Win32Native.LoadLibrary(cryptoPath);
      if (_hCrypto == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for " + Path.GetFileName(cryptoPath) + " (Win32 Error: " + Marshal.GetLastWin32Error() + ")";
        return false;
      }

      _hSsl = Win32Native.LoadLibrary(sslPath);
      if (_hSsl == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for " + Path.GetFileName(sslPath) + " (Win32 Error: " + Marshal.GetLastWin32Error() + ")";
        return false;
      }

      try {
        _fnInitSsl = GetExport<OpenSslInitSslDelegate>(_hSsl, _hCrypto, "OPENSSL_init_ssl");
        _fnTlsClientMethod = GetExport<TlsClientMethodDelegate>(_hSsl, _hCrypto, "TLS_client_method");
        _fnSslCtxNew = GetExport<SslCtxNewDelegate>(_hSsl, _hCrypto, "SSL_CTX_new");
        _fnSslCtxFree = GetExport<SslCtxFreeDelegate>(_hSsl, _hCrypto, "SSL_CTX_free");
        _fnSslNew = GetExport<SslNewDelegate>(_hSsl, _hCrypto, "SSL_new");
        _fnSslFree = GetExport<SslFreeDelegate>(_hSsl, _hCrypto, "SSL_free");
        _fnSslSetFd = GetExport<SslSetFdDelegate>(_hSsl, _hCrypto, "SSL_set_fd");
        _fnSslCtrl = GetExport<SslCtrlDelegate>(_hSsl, _hCrypto, "SSL_ctrl");
        _fnSslConnect = GetExport<SslConnectDelegate>(_hSsl, _hCrypto, "SSL_connect");
        _fnSslWrite = GetExport<SslWriteDelegate>(_hSsl, _hCrypto, "SSL_write");
        _fnSslRead = GetExport<SslReadDelegate>(_hSsl, _hCrypto, "SSL_read");
        _fnSslShutdown = GetExport<SslShutdownDelegate>(_hSsl, _hCrypto, "SSL_shutdown");
        _fnSslGetError = GetExport<SslGetErrorDelegate>(_hSsl, _hCrypto, "SSL_get_error");

        _fnInitSsl(0, IntPtr.Zero);
        return true;
      } catch (Exception ex) {
        _initErrorMessage = "OpenSSL init error: " + ex.Message;
        return false;
      }
    }

    public static IntPtr TLS_client_method() {
      return _fnTlsClientMethod();
    }

    public static IntPtr SSL_CTX_new(IntPtr method) {
      return _fnSslCtxNew(method);
    }

    public static void SSL_CTX_free(IntPtr ctx) {
      _fnSslCtxFree(ctx);
    }

    public static IntPtr SSL_new(IntPtr ctx) {
      return _fnSslNew(ctx);
    }

    public static void SSL_free(IntPtr ssl) {
      _fnSslFree(ssl);
    }

    public static int SSL_set_fd(IntPtr ssl, int fd) {
      return _fnSslSetFd(ssl, fd);
    }

    public static bool SetSniHostname(IntPtr ssl, string hostName) {
      IntPtr res = _fnSslCtrl(ssl, SSL_CTRL_SET_TLSEXT_HOSTNAME, new IntPtr(TLSEXT_NAMETYPE_host_name), hostName);
      return res.ToInt64() != 0;
    }

    public static int SSL_connect(IntPtr ssl) {
      return _fnSslConnect(ssl);
    }

    public static int SSL_write(IntPtr ssl, byte[] buf, int num) {
      return _fnSslWrite(ssl, buf, num);
    }

    public static int SSL_read(IntPtr ssl, byte[] buf, int num) {
      return _fnSslRead(ssl, buf, num);
    }

    public static int SSL_shutdown(IntPtr ssl) {
      return _fnSslShutdown(ssl);
    }

    public static int SSL_get_error(IntPtr ssl, int ret) {
      return _fnSslGetError(ssl, ret);
    }
  }

  /// <summary>
  /// Stream implementation over OpenSSL SSL* handle.
  /// </summary>
  [SuppressUnmanagedCodeSecurity]
  public class OpenSslStream : Stream {
    private IntPtr _ctx;
    private IntPtr _ssl;
    private Socket _socket;
    private bool _disposed = false;

    public OpenSslStream(Socket socket, string hostName) {
      if (!OpenSslNative.IsAvailable) {
        throw new NotSupportedException("OpenSSL 3.x is not available: " + OpenSslNative.InitErrorMessage);
      }

      _socket = socket;
      IntPtr method = OpenSslNative.TLS_client_method();
      if (method == IntPtr.Zero) {
        throw new InvalidOperationException("Failed to get TLS_client_method from OpenSSL.");
      }

      _ctx = OpenSslNative.SSL_CTX_new(method);
      if (_ctx == IntPtr.Zero) {
        throw new InvalidOperationException("Failed to create SSL_CTX.");
      }

      _ssl = OpenSslNative.SSL_new(_ctx);
      if (_ssl == IntPtr.Zero) {
        OpenSslNative.SSL_CTX_free(_ctx);
        _ctx = IntPtr.Zero;
        throw new InvalidOperationException("Failed to create SSL handle.");
      }

      int socketDescriptor = GetSocketDescriptor(socket);

      OpenSslNative.SSL_set_fd(_ssl, socketDescriptor);
      OpenSslNative.SetSniHostname(_ssl, hostName);

      int connectResult = OpenSslNative.SSL_connect(_ssl);
      if (connectResult <= 0) {
        int err = OpenSslNative.SSL_get_error(_ssl, connectResult);
        Dispose();
        throw new IOException("SSL_connect failed with error code: " + err);
      }
    }

    private int GetSocketDescriptor(Socket sock) {
      try {
        FieldInfo field = typeof(Socket).GetField("m_Handle", BindingFlags.Instance | BindingFlags.NonPublic);
        if (field != null) {
          object val = field.GetValue(sock);
          if (val is IntPtr) return ((IntPtr)val).ToInt32();
          if (val is SafeHandle) return ((SafeHandle)val).DangerousGetHandle().ToInt32();
        }
      } catch { }

      return sock.Handle.ToInt32();
    }

    public override bool CanRead {
      get { return !_disposed; }
    }

    public override bool CanSeek {
      get { return false; }
    }

    public override bool CanWrite {
      get { return !_disposed; }
    }

    public override long Length {
      get { throw new NotSupportedException(); }
    }

    public override long Position {
      get { throw new NotSupportedException(); }
      set { throw new NotSupportedException(); }
    }

    public override void Flush() {
    }

    public override int Read(byte[] buffer, int offset, int count) {
      if (_disposed || _ssl == IntPtr.Zero) {
        throw new ObjectDisposedException("OpenSslStream");
      }

      if (offset == 0) {
        return OpenSslNative.SSL_read(_ssl, buffer, count);
      }

      byte[] temp = new byte[count];
      int read = OpenSslNative.SSL_read(_ssl, temp, count);
      if (read > 0) {
        Array.Copy(temp, 0, buffer, offset, read);
      }
      return read;
    }

    public override void Write(byte[] buffer, int offset, int count) {
      if (_disposed || _ssl == IntPtr.Zero) {
        throw new ObjectDisposedException("OpenSslStream");
      }

      if (offset == 0) {
        OpenSslNative.SSL_write(_ssl, buffer, count);
        return;
      }

      byte[] temp = new byte[count];
      Array.Copy(buffer, offset, temp, 0, count);
      OpenSslNative.SSL_write(_ssl, temp, count);
    }

    public override long Seek(long offset, SeekOrigin origin) {
      throw new NotSupportedException();
    }

    public override void SetLength(long value) {
      throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing) {
      if (!_disposed) {
        _disposed = true;
        if (_ssl != IntPtr.Zero) {
          try { OpenSslNative.SSL_shutdown(_ssl); } catch { }
          try { OpenSslNative.SSL_free(_ssl); } catch { }
          _ssl = IntPtr.Zero;
        }
        if (_ctx != IntPtr.Zero) {
          try { OpenSslNative.SSL_CTX_free(_ctx); } catch { }
          _ctx = IntPtr.Zero;
        }
      }
      base.Dispose(disposing);
    }
  }
}
