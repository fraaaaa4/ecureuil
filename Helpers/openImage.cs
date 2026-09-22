using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Ecureuil.Core.Services;

namespace Ecureuil.Helpers
{
    public static class OpenImageHelper
    {
        [DllImport("shell32.dll", EntryPoint = "ShellExecuteA", CharSet = CharSet.Ansi)]
        private static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        public static string ScreenshotTempDirectory
        {
            get
            {
                return Path.Combine(Path.GetTempPath(), "Ecureuil_Screenshots");
            }
        }

        public static void OpenImage(Image image, string filename)
        {
            if (image == null) return;
            try
            {
                // Create dedicated cache directory in temp
                string tempDir = ScreenshotTempDirectory;
                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                string safeName = !string.IsNullOrEmpty(filename) ? filename : "screenshot";
                string tempFile = Path.Combine(tempDir, safeName + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + ".jpg");

                // Save bitmap image
                image.Save(tempFile, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Start viewer process
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.FileName = tempFile;
                    psi.UseShellExecute = true;
                    psi.Verb = "open";
                    Process.Start(psi);
                }
                catch
                {
                    // Fallback using native ShellExecute (Windows 2000 compatibility)
                    ShellExecute(IntPtr.Zero, "open", tempFile, null, null, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening image: " + ex.Message, "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static Bitmap CreateScaledBitmap(Image img, int width, int height)
        {
            if (img == null || width <= 0 || height <= 0) return null;

            Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                float scale = Math.Min((float)width / img.Width, (float)height / img.Height);
                int destW = Math.Max(1, (int)Math.Round(img.Width * scale));
                int destH = Math.Max(1, (int)Math.Round(img.Height * scale));
                int destX = (width - destW) / 2;
                int destY = (height - destH) / 2;

                g.DrawImage(img, new Rectangle(destX, destY, destW, destH));
            }
            return bmp;
        }

        /// <summary>
        /// Generic helper function to truncate any label text with "..." if it exceeds maxWidth
        /// </summary>
        public static void SetTruncatedText(Label label, string fullText, int maxWidth)
        {
            if (string.IsNullOrEmpty(fullText))
            {
                label.Text = "";
                return;
            }

            label.AutoSize = false;
            label.Width = Math.Max(15, maxWidth);

            if (maxWidth <= 20)
            {
                label.Text = "...";
                return;
            }

            Size proposedSize = new Size(maxWidth, int.MaxValue);
            Size fullMeasured = TextRenderer.MeasureText(fullText, label.Font, proposedSize, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

            if (fullMeasured.Width <= maxWidth)
            {
                label.Text = fullText;
            }
            else
            {
                string truncated = fullText;
                while (truncated.Length > 0)
                {
                    truncated = truncated.Substring(0, truncated.Length - 1);
                    Size testSize = TextRenderer.MeasureText(truncated + "...", label.Font, proposedSize, TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
                    if (testSize.Width <= maxWidth)
                    {
                        label.Text = truncated + "...";
                        return;
                    }
                }
                label.Text = "...";
            }
        }

        public static void loadAppIcon(String url, PictureBox appIcon, DownloadService _downloadService)
        {
            loadAppIcon(url, appIcon, _downloadService, null, null);
        }

        public static void loadAppIcon(String url, PictureBox appIcon, DownloadService _downloadService, CacheManager _cacheManager, string cacheKey)
        {
            if (appIcon == null) return;

            // Dispose previous image if present to release unmanaged GDI memory immediately
            if (appIcon.Image != null)
            {
                Image oldImg = appIcon.Image;
                appIcon.Image = null;
                oldImg.Dispose();
            }

            if (!String.IsNullOrEmpty(url))
            {
                try
                {
                    byte[] imageUrl = (_cacheManager != null && !string.IsNullOrEmpty(cacheKey)) ? _cacheManager.LoadImageCache(cacheKey) : null;
                    if (imageUrl != null && imageUrl.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(imageUrl))
                        using (Image tempImg = Image.FromStream(ms))
                        {
                            appIcon.Image = new Bitmap(tempImg);
                        }
                        return;
                    }

                    if (_downloadService != null)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(delegate
                        {
                            try
                            {
                                byte[] downloaded = _downloadService.DownloadData(url);
                                if (downloaded != null && downloaded.Length > 0)
                                {
                                    if (_cacheManager != null && !string.IsNullOrEmpty(cacheKey))
                                    {
                                        _cacheManager.SaveImageCache(cacheKey, downloaded);
                                    }

                                    using (MemoryStream ms = new MemoryStream(downloaded))
                                    using (Image tempImg = Image.FromStream(ms))
                                    {
                                        Bitmap bmp = new Bitmap(tempImg);
                                        if (appIcon.IsHandleCreated && !appIcon.IsDisposed)
                                        {
                                            appIcon.BeginInvoke(new MethodInvoker(delegate
                                            {
                                                if (!appIcon.IsDisposed) appIcon.Image = bmp;
                                            }));
                                        }
                                    }
                                }
                            }
                            catch { }
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading icon: " + ex.Message);
                }
            }
        }

        public static void CleanTempDirectory()
        {
            try
            {
                string tempDir = ScreenshotTempDirectory;
                if (Directory.Exists(tempDir))
                {
                    // Clean up individual files first to delete unlocked ones even if a viewer is open
                    string[] files = Directory.GetFiles(tempDir);
                    for (int i = 0; i < files.Length; i++)
                    {
                        try
                        {
                            File.Delete(files[i]);
                        }
                        catch { }
                    }

                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
