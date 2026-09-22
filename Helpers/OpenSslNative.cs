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
    private delegate int OPENSSL_init_ssl_delegate(ulong opts, IntPtr settings);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr TLS_client_method_delegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SSL_CTX_new_delegate(IntPtr method);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SSL_CTX_free_delegate(IntPtr ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SSL_new_delegate(IntPtr ctx);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SSL_free_delegate(IntPtr ssl);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_set_fd_delegate(IntPtr ssl, int fd);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SSL_ctrl_delegate(IntPtr ssl, int cmd, IntPtr larg, [MarshalAs(UnmanagedType.LPStr)] string parg);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_connect_delegate(IntPtr ssl);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_write_delegate(IntPtr ssl, byte[] buf, int num);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_read_delegate(IntPtr ssl, byte[] buf, int num);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_shutdown_delegate(IntPtr ssl);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SSL_get_error_delegate(IntPtr ssl, int ret);

    private static OPENSSL_init_ssl_delegate _OPENSSL_init_ssl;
    private static TLS_client_method_delegate _TLS_client_method;
    private static SSL_CTX_new_delegate _SSL_CTX_new;
    private static SSL_CTX_free_delegate _SSL_CTX_free;
    private static SSL_new_delegate _SSL_new;
    private static SSL_free_delegate _SSL_free;
    private static SSL_set_fd_delegate _SSL_set_fd;
    private static SSL_ctrl_delegate _SSL_ctrl;
    private static SSL_connect_delegate _SSL_connect;
    private static SSL_write_delegate _SSL_write;
    private static SSL_read_delegate _SSL_read;
    private static SSL_shutdown_delegate _SSL_shutdown;
    private static SSL_get_error_delegate _SSL_get_error;

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

    private static T GetProcDelegate<T>(IntPtr hModule, string procName) where T : class {
      IntPtr pProc = Win32Native.GetProcAddress(hModule, procName);
      if (pProc == IntPtr.Zero) {
        throw new EntryPointNotFoundException("Unable to find function: " + procName);
      }
      return Marshal.GetDelegateForFunctionPointer(pProc, typeof(T)) as T;
    }

    private static bool CheckAvailability() {
      string appDir = AppDomain.CurrentDomain.BaseDirectory;

      string[] cryptoCandidates = new string[] {
        Path.Combine(appDir, "libcrypto-3-arm.dll"),
        Path.Combine(appDir, "libcrypto-3.dll")
      };

      string[] sslCandidates = new string[] {
        Path.Combine(appDir, "libssl-3-arm.dll"),
        Path.Combine(appDir, "libssl-3.dll")
      };

      IntPtr hCrypto = IntPtr.Zero;
      string loadedCryptoPath = null;
      int lastCryptoError = 0;

      for (int i = 0; i < cryptoCandidates.Length; i++) {
        if (File.Exists(cryptoCandidates[i])) {
          hCrypto = Win32Native.LoadLibrary(cryptoCandidates[i]);
          if (hCrypto != IntPtr.Zero) {
            loadedCryptoPath = cryptoCandidates[i];
            break;
          }
          lastCryptoError = Marshal.GetLastWin32Error();
        }
      }

      if (hCrypto == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for libcrypto-3.dll (Win32 Error: " + lastCryptoError + ")";
        return false;
      }

      IntPtr hSsl = IntPtr.Zero;
      int lastSslError = 0;

      for (int i = 0; i < sslCandidates.Length; i++) {
        if (File.Exists(sslCandidates[i])) {
          hSsl = Win32Native.LoadLibrary(sslCandidates[i]);
          if (hSsl != IntPtr.Zero) {
            break;
          }
          lastSslError = Marshal.GetLastWin32Error();
        }
      }

      if (hSsl == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for libssl-3.dll (Win32 Error: " + lastSslError + ")";
        return false;
      }

      try {
        _OPENSSL_init_ssl = GetProcDelegate<OPENSSL_init_ssl_delegate>(hSsl, "OPENSSL_init_ssl");
        _TLS_client_method = GetProcDelegate<TLS_client_method_delegate>(hSsl, "TLS_client_method");
        _SSL_CTX_new = GetProcDelegate<SSL_CTX_new_delegate>(hSsl, "SSL_CTX_new");
        _SSL_CTX_free = GetProcDelegate<SSL_CTX_free_delegate>(hSsl, "SSL_CTX_free");
        _SSL_new = GetProcDelegate<SSL_new_delegate>(hSsl, "SSL_new");
        _SSL_free = GetProcDelegate<SSL_free_delegate>(hSsl, "SSL_free");
        _SSL_set_fd = GetProcDelegate<SSL_set_fd_delegate>(hSsl, "SSL_set_fd");
        _SSL_ctrl = GetProcDelegate<SSL_ctrl_delegate>(hSsl, "SSL_ctrl");
        _SSL_connect = GetProcDelegate<SSL_connect_delegate>(hSsl, "SSL_connect");
        _SSL_write = GetProcDelegate<SSL_write_delegate>(hSsl, "SSL_write");
        _SSL_read = GetProcDelegate<SSL_read_delegate>(hSsl, "SSL_read");
        _SSL_shutdown = GetProcDelegate<SSL_shutdown_delegate>(hSsl, "SSL_shutdown");
        _SSL_get_error = GetProcDelegate<SSL_get_error_delegate>(hSsl, "SSL_get_error");

        _OPENSSL_init_ssl(0, IntPtr.Zero);
        return true;
      } catch (Exception ex) {
        _initErrorMessage = "OpenSSL initialization error: " + ex.Message;
        return false;
      }
    }

    public static IntPtr TLS_client_method() {
      return _TLS_client_method();
    }

    public static IntPtr SSL_CTX_new(IntPtr method) {
      return _SSL_CTX_new(method);
    }

    public static void SSL_CTX_free(IntPtr ctx) {
      _SSL_CTX_free(ctx);
    }

    public static IntPtr SSL_new(IntPtr ctx) {
      return _SSL_new(ctx);
    }

    public static void SSL_free(IntPtr ssl) {
      _SSL_free(ssl);
    }

    public static int SSL_set_fd(IntPtr ssl, int fd) {
      return _SSL_set_fd(ssl, fd);
    }

    public static bool SetSniHostname(IntPtr ssl, string hostName) {
      IntPtr res = _SSL_ctrl(ssl, SSL_CTRL_SET_TLSEXT_HOSTNAME, new IntPtr(TLSEXT_NAMETYPE_host_name), hostName);
      return res.ToInt64() != 0;
    }

    public static int SSL_connect(IntPtr ssl) {
      return _SSL_connect(ssl);
    }

    public static int SSL_write(IntPtr ssl, byte[] buf, int num) {
      return _SSL_write(ssl, buf, num);
    }

    public static int SSL_read(IntPtr ssl, byte[] buf, int num) {
      return _SSL_read(ssl, buf, num);
    }

    public static int SSL_shutdown(IntPtr ssl) {
      return _SSL_shutdown(ssl);
    }

    public static int SSL_get_error(IntPtr ssl, int ret) {
      return _SSL_get_error(ssl, ret);
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
