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

        private List<AppModel> _loadedApps = null;
        private CacheManager _cacheManager = null;

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

        public URLdownload(AppModel app, InstallerService installerService, List<AppModel> loadedApps, CacheManager cacheManager)
            : this(app != null ? app.name : null, app != null ? app.source : null, app != null ? app.downloadUrl : null, app != null ? app.fileType : null,
            (app != null && app.dateOriginal.HasValue) ? app.dateOriginal.Value.ToString("dd/MM/yyyy") : "-",
            (app != null && app.dateSource.HasValue) ? app.dateSource.Value.ToString("dd/MM/yyyy") : "-",
            app != null ? app.size : 0)
        {
            _autoInstall = true;
            _appModel = app;
            _installerService = installerService;
            _loadedApps = loadedApps;
            _cacheManager = cacheManager;
        }

        public URLdownload(AppModel app, InstallerService installerService)
            : this(app, installerService, null, null)
        {
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
                this.Text = (Resources.installer ?? "Installing") + " - " + appName;
                downloadLabel1.Text = string.Format(Resources.preparingInstallation, appName);
                
                _isDownloading = true;
                _isCancelled = false;
                saveButton.Enabled = false;
                downloadProgress.Value = 0;
                _stopwatch.Reset();
                _stopwatch.Start();

                _downloadThread = new Thread(new ThreadStart(AutoInstallWorker));
                _downloadThread.IsBackground = true;
                _downloadThread.Start();
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

        private void UpdateStatusUI(string text)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (this.IsDisposed) return;
                    downloadLabel1.Text = text;
                    downloadProgress.Value = 0;
                    sizeText.Text = Resources.defaultZeroSize;
                    timeEplasedText.Text = Resources.defaultZeroTime;
                    timeRemainingText.Text = Resources.defaultDashTime;
                }));
            }
            catch { }
        }

        private void SetProgressMarquee(bool marquee)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;
            try
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    if (this.IsDisposed) return;
                    downloadProgress.Style = marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                }));
            }
            catch { }
        }

        private void AutoInstallWorker()
        {
            DownloadService downloadService = new DownloadService(60);

            // 1. Process Dependencies if any
            if (_appModel != null && _appModel.dependencies != null && _appModel.dependencies.Count > 0)
            {
                string mainAppTargetFolder = Path.Combine(_installerService != null ? _installerService.DefaultInstallPath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps"), _appModel.id);

                for (int i = 0; i < _appModel.dependencies.Count; i++)
                {
                    if (_isCancelled) return;
                    string rawDep = _appModel.dependencies[i];
                    if (string.IsNullOrEmpty(rawDep)) continue;

                    DependencySpec spec = DependencySpec.Parse(rawDep);
                    if (spec == null) continue;

                    if (!ProcessSingleDependency(spec, downloadService, mainAppTargetFolder))
                    {
                        _isCancelled = true;
                        try { this.BeginInvoke(new MethodInvoker(delegate { this.Close(); })); } catch { }
                        return;
                    }
                }
            }

            if (_isCancelled) return;

            // 2. Download Main App
            UpdateStatusUI(string.Format(Resources.downloadingApp, appName));
            string mainExt = !string.IsNullOrEmpty(extension) ? extension.TrimStart('.') : "tmp";
            string mainTempPath = Path.Combine(Path.GetTempPath(), "ecureuil_" + (_appModel != null && _appModel.id != null ? _appModel.id : "app") + "." + mainExt);

            bool success = false;
            string errorMsg = null;
            try
            {
                success = downloadService.DownloadFile(downloadURL, mainTempPath, OnDownloadProgress);
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
                        OnDownloadCompleted(success, mainTempPath, errorMsg);
                    }));
                }
                catch { }
            }
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
                    downloadLabel1.Text = string.Format(Resources.installingApp, appName);
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

        private bool ProcessSingleDependency(DependencySpec spec, DownloadService downloadService, string mainAppTargetFolder)
        {
            string depName = null;
            string downloadUrl = null;
            string fileType = "tmp";
            string origFileName = null;
            AppModel targetApp = null;

            if (spec.IsEcureuil && !string.IsNullOrEmpty(spec.AppId))
            {
                if (_cacheManager == null)
                {
                    try { _cacheManager = new CacheManager(); } catch { }
                }

                targetApp = _installerService != null ? _installerService.FindApp(spec.SourceId, spec.AppId, _loadedApps, _cacheManager) : null;
                if (targetApp == null || string.IsNullOrEmpty(targetApp.downloadUrl))
                {
                    string srcMsg = !string.IsNullOrEmpty(spec.SourceId) ? string.Format(Resources.depFromSource, spec.SourceId) : "";
                    return PromptContinueOnMissing(string.Format(Resources.depNotFound, spec.AppId, srcMsg, appName), Resources.depMissingTitle);
                }

                depName = targetApp.name ?? targetApp.id;
                downloadUrl = targetApp.downloadUrl;
                fileType = !string.IsNullOrEmpty(targetApp.fileType) ? targetApp.fileType.TrimStart('.') : "tmp";
                origFileName = depName + "." + fileType;
            }
            else if (!string.IsNullOrEmpty(spec.HttpUrl))
            {
                downloadUrl = spec.HttpUrl;
                try
                {
                    Uri u = new Uri(spec.HttpUrl);
                    origFileName = Path.GetFileName(u.AbsolutePath);
                    string p = Path.GetExtension(u.AbsolutePath);
                    if (!string.IsNullOrEmpty(p)) fileType = p.TrimStart('.');
                }
                catch { }
                if (string.IsNullOrEmpty(origFileName)) origFileName = "dep_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "." + fileType;
                depName = origFileName;
            }
            else
            {
                return true;
            }

            UpdateStatusUI(string.Format(Resources.depDownloading, depName));
            string depTempPath = Path.Combine(Path.GetTempPath(), "ecureuil_dep_" + Guid.NewGuid().ToString("N") + "." + fileType);

            bool dlSuccess = downloadService.DownloadFile(downloadUrl, depTempPath, OnDownloadProgress);
            if (dlSuccess && File.Exists(depTempPath) && !_isCancelled)
            {
                SetProgressMarquee(true);
                if (spec.Action == DependencyAction.Install)
                {
                    UpdateStatusUI(string.Format(Resources.depInstalling, depName));
                    if (_installerService != null)
                    {
                        if (targetApp == null)
                        {
                            targetApp = new AppModel();
                            targetApp.id = "dep_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                            targetApp.name = Path.GetFileNameWithoutExtension(origFileName);
                            targetApp.fileType = fileType;
                            targetApp.isPortable = false;
                        }
                        _installerService.InstallApp(targetApp, depTempPath);
                    }
                }
                else
                {
                    string statusMsg = (spec.Action == DependencyAction.CopySystem)
                        ? string.Format(Resources.depCopyingSystem ?? "Copying dependency: {0} to Windows...", depName)
                        : string.Format(Resources.depCopying ?? "Copying dependency: {0}...", depName);
                    UpdateStatusUI(statusMsg);
                    if (_installerService != null)
                    {
                        _installerService.DeployDependency(depTempPath, spec.Action, mainAppTargetFolder, origFileName);
                    }
                }
                SetProgressMarquee(false);
                return true;
            }
            else if (!_isCancelled)
            {
                string errMsg = (spec.IsEcureuil)
                    ? string.Format(Resources.depDownloadError, depName, appName)
                    : string.Format(Resources.depDownloadUrlError, downloadUrl, appName);
                return PromptContinueOnMissing(errMsg, Resources.depDownloadErrorTitle);
            }

            return false;
        }

        private bool PromptContinueOnMissing(string message, string title)
        {
            bool continueAnyway = false;
            try
            {
                this.Invoke(new MethodInvoker(delegate
                {
                    DialogResult dr = MessageBox.Show(this,
                        message,
                        title + " - " + (Resources.ecureuil ?? "Ecureuil"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    continueAnyway = (dr == DialogResult.Yes);
                }));
            }
            catch { }
            return continueAnyway;
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