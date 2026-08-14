using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ecureuil.Core.Services {
  public class DownloadService {
    private readonly HttpClient _httpClient;
    private readonly int timeout = 30; //This will be in application settings

    public DownloadService() {
      _httpClient = new HttpClient();
      _httpClient.Timeout = TimeSpan.FromSeconds(timeout); 
    }

    public async Task<string> DownloadString(string url) {
      try {
        return await _httpClient.GetStringAsync(url);
      } catch (Exception ex) {
        Console.WriteLine($"Error in downloading from {url}: {ex.Message}");
      }
    }

    public async Task<bool> DownloadFile(string url, string destinationPath, IProgress<int> progress = null) {
      return await DownloadFileAsync(url, destinationPath, progress);
    }

    private async Task<bool> DownloadFileAsync(string url, string destinationPath, IProgress<int> progress) {
      try {
        using (var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead)) {
          var totalBytes = response.Content.Headers.ContentLength ?? -1L;
          using (var stream = await response.Content.ReadAsStreamAsync())
          using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true)) {
            var buffer = new byte[8192];
            long totalReadBytes = 0;
            int readBytes;

            while ((readBytes = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
              await fileStream.WriteAsync(buffer, 0, readBytes);
              totalReadBytes += readBytes;

              if (totalBytes != -1 && progress != null) {
                int percentage = (int)totalReadBytes / totalBytes * 100;
                progre.Report(percentage);
              }
            }
          }
        }
        return true;
      } catch {
        return false;
      }
    }
        
  }
}
