using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Ecureuil.Core.Helpers;

namespace Ecureuil.Core.Services {
  public delegate void DownloadProgressChangedHandler(int percentage, long bytesReceived, long totalBytes);

  public class DownloadService {
    private readonly int _timeoutSeconds;

    public DownloadService() {
      _timeoutSeconds = 30;
    }

    public DownloadService(int timeoutSeconds) {
      _timeoutSeconds = timeoutSeconds;
    }

    // Downloads byte array (e.g. images) from a URL
    public byte[] DownloadData(string url) {
      if (string.IsNullOrEmpty(url)) return null;
      try {
        using (MemoryStream ms = new MemoryStream()) {
          if (DownloadUrlToStream(url, ms, null, 0)) {
            return ms.ToArray();
          }
        }
      } catch { }

      return DownloadDataFallback(url);
    }

    // Downloads text content from a URL, using OpenSSL 3.x for HTTPS if available
    public string DownloadString(string url) {
      if (string.IsNullOrEmpty(url)) return null;
      try {
        using (MemoryStream ms = new MemoryStream()) {
          if (DownloadUrlToStream(url, ms, null, 0)) {
            return Encoding.UTF8.GetString(ms.ToArray());
          }
        }
      } catch (Exception ex) {
        if (!OpenSslNative.IsAvailable && url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) {
          throw new Exception("OpenSSL 3.x is not available: " + OpenSslNative.InitErrorMessage);
        }
        throw new Exception("Download failed: " + ex.Message, ex);
      }

      return DownloadStringFallback(url);
    }

    // Downloads file to destination path, using OpenSSL 3.x for HTTPS if available
    public bool DownloadFile(string url, string destinationPath, DownloadProgressChangedHandler progress) {
      if (string.IsNullOrEmpty(url)) return false;

      string dir = Path.GetDirectoryName(destinationPath);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }

      using (FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None)) {
        return DownloadUrlToStream(url, fs, progress, 0);
      }
    }

    private bool DownloadUrlToStream(string url, Stream outputStream, DownloadProgressChangedHandler progress, int redirectCount) {
      if (redirectCount > 10) {
        throw new Exception("Too many HTTP redirects.");
      }

      Uri uri = new Uri(url);
      if (uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)) {
        if (OpenSslNative.IsAvailable) {
          return DownloadOverOpenSsl(uri, outputStream, progress, redirectCount);
        } else {
          return DownloadStreamFallback(url, outputStream, progress, redirectCount);
        }
      } else {
        return DownloadStreamFallback(url, outputStream, progress, redirectCount);
      }
    }

    private bool DownloadOverOpenSsl(Uri uri, Stream outputStream, DownloadProgressChangedHandler progress, int redirectCount) {
      int port = uri.Port > 0 ? uri.Port : 443;
      string host = uri.Host;
      string pathAndQuery = uri.PathAndQuery;

      IPHostEntry hostEntry = Dns.GetHostEntry(host);
      if (hostEntry.AddressList.Length == 0) {
        throw new SocketException((int)SocketError.HostNotFound);
      }

      IPAddress targetAddress = hostEntry.AddressList[0];
      Socket socket = new Socket(targetAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
      socket.ReceiveTimeout = _timeoutSeconds * 1000;
      socket.SendTimeout = _timeoutSeconds * 1000;

      try {
        socket.Connect(new IPEndPoint(targetAddress, port));
        using (OpenSslStream sslStream = new OpenSslStream(socket, host)) {
          string requestHeader = "GET " + pathAndQuery + " HTTP/1.1\r\n" +
                                 "Host: " + host + "\r\n" +
                                 "User-Agent: Ecureuil/1.0 (Windows NT)\r\n" +
                                 "Accept: */*\r\n" +
                                 "Connection: close\r\n\r\n";

          byte[] reqBytes = Encoding.ASCII.GetBytes(requestHeader);
          sslStream.Write(reqBytes, 0, reqBytes.Length);

          // Read response headers
          MemoryStream headerStream = new MemoryStream();
          byte[] singleByte = new byte[1];
          int headerEndState = 0;

          while (sslStream.Read(singleByte, 0, 1) > 0) {
            byte b = singleByte[0];
            headerStream.WriteByte(b);

            if (headerEndState == 0 && b == '\r') headerEndState = 1;
            else if (headerEndState == 1 && b == '\n') headerEndState = 2;
            else if (headerEndState == 2 && b == '\r') headerEndState = 3;
            else if (headerEndState == 3 && b == '\n') break;
            else headerEndState = (b == '\r') ? 1 : 0;
          }

          string headerText = Encoding.ASCII.GetString(headerStream.ToArray());
          int statusCode = ParseStatusCode(headerText);

          // Handle 301/302/303/307/308 Redirects
          if (statusCode == 301 || statusCode == 302 || statusCode == 303 || statusCode == 307 || statusCode == 308) {
            string redirectUrl = ParseHeader(headerText, "Location");
            if (!string.IsNullOrEmpty(redirectUrl)) {
              Uri redirectUri = new Uri(uri, redirectUrl);
              // Reset stream position if possible before following redirect
              if (outputStream.CanSeek) {
                outputStream.SetLength(0);
                outputStream.Position = 0;
              }
              return DownloadUrlToStream(redirectUri.AbsoluteUri, outputStream, progress, redirectCount + 1);
            }
          }

          if (statusCode >= 400) {
            throw new Exception("HTTP server responded with error code: " + statusCode);
          }

          long contentLength = ParseContentLength(headerText);
          bool isChunked = IsChunkedTransfer(headerText);

          if (isChunked) {
            ReadChunkedBody(sslStream, outputStream, progress);
          } else {
            byte[] buffer = new byte[8192];
            long totalReadBytes = 0;
            int readBytes = 0;

            while ((readBytes = sslStream.Read(buffer, 0, buffer.Length)) > 0) {
              outputStream.Write(buffer, 0, readBytes);
              totalReadBytes += readBytes;

              if (progress != null && contentLength > 0) {
                int percentage = (int)((totalReadBytes * 100) / contentLength);
                progress(percentage, totalReadBytes, contentLength);
              }

              if (contentLength > 0 && totalReadBytes >= contentLength) {
                break;
              }
            }
          }
        }
        return true;
      } finally {
        if (socket != null) {
          try { socket.Close(); } catch { }
        }
      }
    }

    private int ParseStatusCode(string headers) {
      string[] lines = headers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
      if (lines.Length > 0) {
        string[] parts = lines[0].Split(' ');
        if (parts.Length >= 2) {
          int code;
          if (int.TryParse(parts[1], out code)) return code;
        }
      }
      return 200;
    }

    private string ParseHeader(string headers, string headerName) {
      string[] lines = headers.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < lines.Length; i++) {
        if (lines[i].StartsWith(headerName + ":", StringComparison.OrdinalIgnoreCase)) {
          return lines[i].Substring(headerName.Length + 1).Trim();
        }
      }
      return null;
    }

    private bool IsChunkedTransfer(string headers) {
      string te = ParseHeader(headers, "Transfer-Encoding");
      return te != null && te.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void ReadChunkedBody(Stream sslStream, Stream outputStream, DownloadProgressChangedHandler progress) {
      byte[] singleByte = new byte[1];
      while (true) {
        StringBuilder sizeHex = new StringBuilder();
        while (sslStream.Read(singleByte, 0, 1) > 0) {
          char c = (char)singleByte[0];
          if (c == '\n') break;
          if (c != '\r') sizeHex.Append(c);
        }

        string hexStr = sizeHex.ToString().Trim();
        int semicolon = hexStr.IndexOf(';');
        if (semicolon >= 0) hexStr = hexStr.Substring(0, semicolon).Trim();
        if (string.IsNullOrEmpty(hexStr)) break;

        int chunkSize = Convert.ToInt32(hexStr, 16);
        if (chunkSize <= 0) {
          sslStream.Read(singleByte, 0, 1);
          sslStream.Read(singleByte, 0, 1);
          break;
        }

        byte[] chunkBuffer = new byte[chunkSize];
        int totalChunkRead = 0;
        while (totalChunkRead < chunkSize) {
          int r = sslStream.Read(chunkBuffer, totalChunkRead, chunkSize - totalChunkRead);
          if (r <= 0) break;
          totalChunkRead += r;
        }

        outputStream.Write(chunkBuffer, 0, totalChunkRead);

        sslStream.Read(singleByte, 0, 1);
        sslStream.Read(singleByte, 0, 1);
      }
    }

    private long ParseContentLength(string headers) {
      string val = ParseHeader(headers, "Content-Length");
      if (val != null) {
        long len;
        if (long.TryParse(val, out len)) return len;
      }
      return -1;
    }

    private byte[] DownloadDataFallback(string url) {
      HttpWebRequest request = null;
      HttpWebResponse response = null;
      try {
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Timeout = _timeoutSeconds * 1000;
        request.UserAgent = "Ecureuil/1.0 (Windows NT)";
        request.Proxy = WebRequest.DefaultWebProxy;

        response = (HttpWebResponse)request.GetResponse();
        using (Stream stream = response.GetResponseStream())
        using (MemoryStream ms = new MemoryStream()) {
          byte[] buffer = new byte[8192];
          int read;
          while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
            ms.Write(buffer, 0, read);
          }
          return ms.ToArray();
        }
      } catch {
        return null;
      } finally {
        if (response != null) response.Close();
      }
    }

    private string DownloadStringFallback(string url) {
      HttpWebRequest request = null;
      HttpWebResponse response = null;
      StreamReader reader = null;
      try {
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Timeout = _timeoutSeconds * 1000;
        request.UserAgent = "Ecureuil/1.0 (Windows NT)";
        request.Proxy = WebRequest.DefaultWebProxy;

        response = (HttpWebResponse)request.GetResponse();
        using (Stream stream = response.GetResponseStream()) {
          reader = new StreamReader(stream);
          return reader.ReadToEnd();
        }
      } catch (Exception ex) {
        throw new Exception("Fallback download error: " + ex.Message, ex);
      } finally {
        if (reader != null) reader.Close();
        if (response != null) response.Close();
      }
    }

    private bool DownloadStreamFallback(string url, Stream outputStream, DownloadProgressChangedHandler progress, int redirectCount) {
      if (redirectCount > 10) {
        throw new Exception("Too many HTTP redirects.");
      }

      HttpWebRequest request = null;
      HttpWebResponse response = null;
      Stream responseStream = null;

      try {
        request = (HttpWebRequest)WebRequest.Create(url);
        request.Timeout = _timeoutSeconds * 1000;
        request.UserAgent = "Ecureuil/1.0 (Windows NT)";
        request.Proxy = WebRequest.DefaultWebProxy;
        request.AllowAutoRedirect = false; // Gestiamo i redirect manualmente per monitorare eventuali cambi schema

        response = (HttpWebResponse)request.GetResponse();
        int statusCode = (int)response.StatusCode;

        if (statusCode == 301 || statusCode == 302 || statusCode == 303 || statusCode == 307 || statusCode == 308) {
          string redirectUrl = response.Headers["Location"];
          if (!string.IsNullOrEmpty(redirectUrl)) {
            Uri redirectUri = new Uri(new Uri(url), redirectUrl);
            if (outputStream.CanSeek) {
              outputStream.SetLength(0);
              outputStream.Position = 0;
            }
            return DownloadUrlToStream(redirectUri.AbsoluteUri, outputStream, progress, redirectCount + 1);
          }
        }

        long totalBytes = response.ContentLength;
        responseStream = response.GetResponseStream();

        byte[] buffer = new byte[8192];
        long totalReadBytes = 0;
        int readBytes = 0;

        while ((readBytes = responseStream.Read(buffer, 0, buffer.Length)) > 0) {
          outputStream.Write(buffer, 0, readBytes);
          totalReadBytes += readBytes;

          if (progress != null && totalBytes > 0) {
            int percentage = (int)((totalReadBytes * 100) / totalBytes);
            progress(percentage, totalReadBytes, totalBytes);
          }
        }

        return true;
      } catch (Exception ex) {
        throw new Exception("Download error: " + ex.Message, ex);
      } finally {
        if (responseStream != null) responseStream.Close();
        if (response != null) response.Close();
      }
    }

    private bool DownloadFileFallback(string url, string destinationPath, DownloadProgressChangedHandler progress) {
      string dir = Path.GetDirectoryName(destinationPath);
      if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }

      using (FileStream fs = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None)) {
        return DownloadStreamFallback(url, fs, progress, 0);
      }
    }
  }
}
