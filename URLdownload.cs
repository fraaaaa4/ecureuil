using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ecureuil.Core.Services;
using Ecureuil.Properties;
using Ecureuil.Core.Models;

namespace Ecureuil
{
    public partial class URLdownload : Form
    {
        private String appName = null;
        private String downloadURL = null;
        private String sourceName = null;
        private String extension = null;
        private String dateOriginal = null;
        private String dateUpload = null;
        private long sizeItem = (long)0.0;
        private bool _autoInstall = false;
        private AppModel _appModel = null;
        private InstallerService _installerService = null;

        private Thread _downloadThread = null;
        private bool _isDownloading = false;
        private bool _isCancelled = false;
        private Stopwatch _stopwatch = new Stopwatch();

        private long _bytesReceived = 0;
        private long _totalBytes = 0;
        private int _currentPercentage = 0;

        public URLdownload(String _appName, String _sourceName, String _downloadURL, String _extension, String _dateOriginal, String _dateUpload, long _sizeItem)
        {
            InitializeComponent();
            appName = _appName;
            downloadURL = _downloadURL;
            sourceName = _sourceName;
            extension = _extension;
            dateOriginal = _dateOriginal;
            dateUpload = _dateUpload;
            sizeItem = _sizeItem;
        }

        public URLdownload(AppModel app, InstallerService installerService)
            : this(app.name, app.source, app.downloadUrl, app.fileType, app.dateOriginal.HasValue ? app.dateOriginal.Value.ToString("dd/MM/yyyy") : "-",
            app.dateSource.HasValue ? app.dateSource.Value.ToString("dd/MM/yyyy") : "-", app.size)
        {
            _autoInstall = true; _appModel = app; _installerService = installerService;
        }

        private void URLdownload_Load(object sender, EventArgs e)
        {
            this.Text = Resources.downloadFile + " - " + appName;
            downloadLabel1.Text = Resources.downloadFile2 + " " + appName + " " + Resources.downloadFile3 + " " + sourceName;

            FileDetailsBox.Text = Resources.fileDetails;

            detailsList.Items.Add(Resources.size + ":" + sizeFormat.FormattedSize(sizeItem));
            detailsList.Items.Add(Resources.extension + "." + extension);
            detailsList.Items.Add(Resources.dateCreated + " " + dateOriginal);
            detailsList.Items.Add(Resources.dateUpload + " " + dateUpload);

            timeElapsed.Text = Resources.timeElapsed;
            timeRemaining.Text = Resources.time;
            size.Text = Resources.size;
            progressGroup.Text = Resources.progress;

            timeEplasedText.Text = Resources.defaultZeroTime;
            timeRemainingText.Text = Resources.defaultDashTime;
            sizeText.Text = Resources.defaultZeroSize;

            saveToolStripMenuItem.Text = Resources.save;
            saveToolStripMenuItem.ToolTipText = Resources.saveTooltip;
            saveAsToolStripMenuItem.Text = Resources.saveAs;
            saveAsToolStripMenuItem.ToolTipText = Resources.saveAsTooltip;

            if (_autoInstall)
            {
                saveButton.Visible = false;
                this.Text = "Installing - " + appName;
                downloadLabel1.Text = "Download and installing " + appName + "...";
                string ext = !string.IsNullOrEmpty(extension) ? extension.TrimStart('.') : "tmp";
                string tempPath = Path.Combine(Path.GetTempPath(), "ecureuil_" + (_appModel.id ?? "app") + "." + ext);
                StartDownload(tempPath);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (_isDownloading) return;
            Button btn = (Button)sender;
            saveContextMenu.Show(btn, new Point(0, btn.Height));
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isDownloading) return;

            string targetFolder = Settings.Default.downloadPath;

            // If configured downloadPath is missing or empty, ask user to pick folder or default to Desktop/Downloads
            if (string.IsNullOrEmpty(targetFolder) || !Directory.Exists(targetFolder))
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = Ecureuil.Properties.Resources.selectDownload;
                    if (fbd.ShowDialog(this) == DialogResult.OK)
                    {
                        targetFolder = fbd.SelectedPath;
                        Settings.Default.downloadPath = targetFolder;
                        Settings.Default.Save();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            string filename = GetDefaultFileName();
            string fullPath = Path.Combine(targetFolder, filename);

            StartDownload(fullPath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isDownloading) return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string ext = (extension ?? "").TrimStart('.');
                if (!string.IsNullOrEmpty(ext))
                {
                    sfd.Filter = ext.ToUpperInvariant() + " " + Ecureuil.Properties.Resources.files + " (*." + ext + ")|*." + ext + "|" + Ecureuil.Properties.Resources.allFiles + " (*.*)|*.*";
                    sfd.DefaultExt = ext;
                }
                else
                {
                    sfd.Filter = Ecureuil.Properties.Resources.allFiles + " (*.*)|*.*";
                }

                sfd.FileName = GetDefaultFileName();

                if (!string.IsNullOrEmpty(Settings.Default.downloadPath) && Directory.Exists(Settings.Default.downloadPath))
                {
                    sfd.InitialDirectory = Settings.Default.downloadPath;
                }

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    // Update downloadPath with chosen folder
                    try
                    {
                        Settings.Default.downloadPath = Path.GetDirectoryName(sfd.FileName);
                        Settings.Default.Save();
                    }
                    catch { }

                    StartDownload(sfd.FileName);
                }
            }
        }

        private string GetDefaultFileName()
        {
            string name = appName ?? "file";
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }

            string ext = (extension ?? "").Trim();
            if (!string.IsNullOrEmpty(ext))
            {
                if (!ext.StartsWith(".")) ext = "." + ext;
                if (!name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                {
                    name += ext;
                }
            }

            return name;
        }

        private void StartDownload(string destinationPath)
        {
            if (string.IsNullOrEmpty(downloadURL))
            {
                MessageBox.Show(this, Ecureuil.Properties.Resources.downloadURL, Ecureuil.Properties.Resources.error2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _isDownloading = true;
            _isCancelled = false;
            saveButton.Enabled = false;
            downloadProgress.Value = 0;
            _stopwatch.Reset();
            _stopwatch.Start();

            _downloadThread = new Thread(new ParameterizedThreadStart(DownloadWorker));
            _downloadThread.IsBackground = true;
            _downloadThread.Start(destinationPath);
        }

        private void DownloadWorker(object targetPathObj)
        {
            string destinationPath = (string)targetPathObj;
            bool success = false;
            string errorMsg = null;

            try
            {
                DownloadService service = new DownloadService(60);
                success = service.DownloadFile(downloadURL, destinationPath, OnDownloadProgress);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                success = false;
            }

            _stopwatch.Stop();

            if (!_isCancelled)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(delegate()
                    {
                        OnDownloadCompleted(success, destinationPath, errorMsg);
                    }));
                }
                catch { }
            }
        }

        private void OnDownloadProgress(int percentage, long bytesReceived, long totalBytes)
        {
            if (_isCancelled) return;

            _bytesReceived = bytesReceived;
            _totalBytes = totalBytes;
            _currentPercentage = Math.Max(0, Math.Min(100, percentage));

            try
            {
                this.BeginInvoke(new MethodInvoker(UpdateProgressUI));
            }
            catch { }
        }

        private void UpdateProgressUI()
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            downloadProgress.Value = _currentPercentage;

            // 1. Time Elapsed
            TimeSpan elapsed = _stopwatch.Elapsed;
            timeEplasedText.Text = string.Format("{0:00}:{1:00}:{2:00}", (int)elapsed.TotalHours, elapsed.Minutes, elapsed.Seconds);

            // 2. Size received / total
            if (_totalBytes > 0)
            {
                sizeText.Text = sizeFormat.FormattedSize(_bytesReceived) + " / " + sizeFormat.FormattedSize(_totalBytes);
            }
            else
            {
                sizeText.Text = sizeFormat.FormattedSize(_bytesReceived);
            }

            // 3. Time Remaining calculation
            if (_totalBytes > 0 && _bytesReceived > 0 && elapsed.TotalSeconds > 0)
            {
                double bytesPerSecond = _bytesReceived / elapsed.TotalSeconds;
                if (bytesPerSecond > 0)
                {
                    long remainingBytes = _totalBytes - _bytesReceived;
                    if (remainingBytes > 0)
                    {
                        double remainingSecs = remainingBytes / bytesPerSecond;
                        TimeSpan remaining = TimeSpan.FromSeconds(remainingSecs);
                        timeRemainingText.Text = string.Format("{0:00}:{1:00}:{2:00}", (int)remaining.TotalHours, remaining.Minutes, remaining.Seconds);
                    }
                    else
                    {
                        timeRemainingText.Text = Resources.defaultZeroTime;
                    }
                }
            }
        }

        private void OnDownloadCompleted(bool success, string destinationPath, string errorMsg)
        {
            _isDownloading = false;

            //saveButton.Enabled = true;

            if (success)
            {
                if (_autoInstall && _installerService != null && _appModel != null)
                {
                    downloadLabel1.Text = "Installing " + appName + "...";
                    downloadProgress.Style = ProgressBarStyle.Marquee;

                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        bool installed = _installerService.InstallApp(_appModel, destinationPath);
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                if (installed)
                                {
                                    MessageBox.Show(this, appName + " has been installed successfully.", Resources.ecureuil, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    this.DialogResult = DialogResult.OK;
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show(this, "Error while installing " + appName, Resources.error2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    this.Close();
                                }
                            }));
                        }
                    });
                    return;
                }
                downloadProgress.Value = 100;
                timeRemainingText.Text = Resources.defaultZeroTime;
                if (_totalBytes > 0)
                {
                    sizeText.Text = sizeFormat.FormattedSize(_totalBytes) + " / " + sizeFormat.FormattedSize(_totalBytes);
                }

                MessageBox.Show(this, (Resources.downloadComplete ?? "Download complete.") + "\n\n" + destinationPath, Resources.ecureuil, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(this, Resources.downloadFailed + (errorMsg != null ? (": " + errorMsg) : "."), Resources.error2, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (_isDownloading)
            {
                _isCancelled = true;
                if (_downloadThread != null && _downloadThread.IsAlive)
                {
                    try { _downloadThread.Abort(); } catch { }
                }
            }

            this.Close();
        }
    }
}