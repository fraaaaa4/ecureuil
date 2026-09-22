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

      private const string LIBSSL = "libssl-3.dll";

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "OPENSSL_init_ssl")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int OPENSSL_init_ssl(ulong opts, IntPtr settings);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "TLS_client_method")]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr TLS_client_method();

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_CTX_new")]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr SSL_CTX_new(IntPtr method);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_CTX_free")]
      [SuppressUnmanagedCodeSecurity]
      public static extern void SSL_CTX_free(IntPtr ctx);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_new")]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr SSL_new(IntPtr ctx);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_free")]
      [SuppressUnmanagedCodeSecurity]
      public static extern void SSL_free(IntPtr ssl);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_set_fd")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_set_fd(IntPtr ssl, int fd);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_ctrl")]
      [SuppressUnmanagedCodeSecurity]
      public static extern IntPtr SSL_ctrl(IntPtr ssl, int cmd, IntPtr larg, [MarshalAs(UnmanagedType.LPStr)] string parg);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_connect")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_connect(IntPtr ssl);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_write")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_write(IntPtr ssl, byte[] buf, int num);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_read")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_read(IntPtr ssl, byte[] buf, int num);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_shutdown")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_shutdown(IntPtr ssl);

      [DllImport(LIBSSL, CallingConvention = CallingConvention.Cdecl, EntryPoint = "SSL_get_error")]
      [SuppressUnmanagedCodeSecurity]
      public static extern int SSL_get_error(IntPtr ssl, int ret);
    }

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

    private static bool CheckAvailability() {
      string appDir = AppDomain.CurrentDomain.BaseDirectory;
      string cryptoPath = Path.Combine(appDir, "libcrypto-3.dll");
      string sslPath = Path.Combine(appDir, "libssl-3.dll");

      if (!File.Exists(cryptoPath)) {
        _initErrorMessage = "libcrypto-3.dll not found at: " + cryptoPath;
        return false;
      }
      if (!File.Exists(sslPath)) {
        _initErrorMessage = "libssl-3.dll not found at: " + sslPath;
        return false;
      }

      IntPtr hCrypto = Win32Native.LoadLibrary(cryptoPath);
      if (hCrypto == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for libcrypto-3.dll (Win32 Error: " + Marshal.GetLastWin32Error() + ")";
        return false;
      }

      IntPtr hSsl = Win32Native.LoadLibrary(sslPath);
      if (hSsl == IntPtr.Zero) {
        _initErrorMessage = "LoadLibrary failed for libssl-3.dll (Win32 Error: " + Marshal.GetLastWin32Error() + ")";
        return false;
      }

      try {
        Win32Native.OPENSSL_init_ssl(0, IntPtr.Zero);
        return true;
      } catch (Exception ex) {
        _initErrorMessage = "OPENSSL_init_ssl error: " + ex.Message;
        return false;
      }
    }

    public static IntPtr TLS_client_method() {
      return Win32Native.TLS_client_method();
    }

    public static IntPtr SSL_CTX_new(IntPtr method) {
      return Win32Native.SSL_CTX_new(method);
    }

    public static void SSL_CTX_free(IntPtr ctx) {
      Win32Native.SSL_CTX_free(ctx);
    }

    public static IntPtr SSL_new(IntPtr ctx) {
      return Win32Native.SSL_new(ctx);
    }

    public static void SSL_free(IntPtr ssl) {
      Win32Native.SSL_free(ssl);
    }

    public static int SSL_set_fd(IntPtr ssl, int fd) {
      return Win32Native.SSL_set_fd(ssl, fd);
    }

    public static bool SetSniHostname(IntPtr ssl, string hostName) {
      IntPtr res = Win32Native.SSL_ctrl(ssl, SSL_CTRL_SET_TLSEXT_HOSTNAME, new IntPtr(TLSEXT_NAMETYPE_host_name), hostName);
      return res.ToInt64() != 0;
    }

    public static int SSL_connect(IntPtr ssl) {
      return Win32Native.SSL_connect(ssl);
    }

    public static int SSL_write(IntPtr ssl, byte[] buf, int num) {
      return Win32Native.SSL_write(ssl, buf, num);
    }

    public static int SSL_read(IntPtr ssl, byte[] buf, int num) {
      return Win32Native.SSL_read(ssl, buf, num);
    }

    public static int SSL_shutdown(IntPtr ssl) {
      return Win32Native.SSL_shutdown(ssl);
    }

    public static int SSL_get_error(IntPtr ssl, int ret) {
      return Win32Native.SSL_get_error(ssl, ret);
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
