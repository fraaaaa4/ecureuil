using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ecureuil.Core.Helpers;
using Ecureuil.Helpers;
using Ecureuil.Core.Models;
using Ecureuil.Core.Services;
using Ecureuil.Properties;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Ecureuil
{
    public partial class ApplicationDetail : Form
    {
        private AppModel _item = null;
        private DownloadService _downloadService = null;
        private List<AppModel> _loadedApps = null;
        private List<SourceModel> _loadedSources = null;
        private Image _appIconImage = null;
        private CacheManager _cacheManager = null;
        private InstallationRegistry _installationRegistry = null;
        private InstallerService _installerService = null;

        // Raw values saved for dynamic responsive truncation on Resize
        private string _fullSourceText = "";
        private string _fullUploaderText = "";
        private string _fullExtensionText = "";
        private string _fullCompatibilityText = "";
        private string _fullLanguageText = "";

        public ApplicationDetail()
        {
            InitializeComponent();
            _installationRegistry = new InstallationRegistry();
            _installerService = new InstallerService(_installationRegistry);
            UpdateToolbarState();
        }

        public ApplicationDetail(AppModel item, DownloadService downloadService, List<AppModel> loadedApps)
            : this(item, downloadService, loadedApps, null, null, null)
        {
        }

        public ApplicationDetail(AppModel item, DownloadService downloadService, List<AppModel> loadedApps, Image appIconImage)
            : this(item, downloadService, loadedApps, appIconImage, null, null)
        {
        }

        public ApplicationDetail(AppModel item, DownloadService downloadService, List<AppModel> loadedApps, Image appIconImage, CacheManager cacheManager)
            : this(item, downloadService, loadedApps, appIconImage, cacheManager, null)
        {
        }

        public ApplicationDetail(AppModel item, DownloadService downloadService, List<AppModel> loadedApps, Image appIconImage, CacheManager cacheManager, List<SourceModel> loadedSources)
        {
            InitializeComponent();
            _item = item;
            _downloadService = downloadService != null ? downloadService : new DownloadService();
            _cacheManager = cacheManager;
            _loadedApps = loadedApps != null ? loadedApps : new List<AppModel>();
            _appIconImage = appIconImage;
            _loadedSources = loadedSources;

            _installationRegistry = new InstallationRegistry();
            _installerService = new InstallerService(_installationRegistry);

            if (_item != null && _installationRegistry != null)
            {
                InstallationRecord rec = _installationRegistry.GetRecord(_item.id);
                if (rec != null)
                {
                    _item.isInstalled = true;
                    _item.installedOn = rec.InstalledOn;
                    _item.installedShortcutPath = rec.ShortcutPath;
                    _item.installDirectory = rec.InstallDirectory;
                }
                else
                {
                    _item.isInstalled = false;
                    _item.installedOn = null;
                    _item.installedShortcutPath = null;
                    _item.installDirectory = null;
                }
            }

            this.FormClosed += new FormClosedEventHandler(delegate(object sender, FormClosedEventArgs e)
            {
                OpenImageHelper.CleanTempDirectory();
            });

            // Re-create app cache if missing or if app is opened
            if (_item != null && _cacheManager != null && !string.IsNullOrEmpty(_item.id))
            {
                try
                {
                    string existingCache = _cacheManager.LoadAppCache(_item.id);
                    if (string.IsNullOrEmpty(existingCache))
                    {
                        string appJson = MiniJson.Serialize(_item);
                        _cacheManager.SaveAppCache(_item.id, appJson);
                    }
                }
                catch { }
            }

            loadAppDetails(_item);
            UpdateToolbarState();
        }

        private void UpdateToolbarState()
        {
            bool installed = (_item != null && _item.isInstalled);
            bool isPortableOrZip = (_item != null && string.Compare((_item.fileType ?? "").TrimStart('.'), "msi", StringComparison.OrdinalIgnoreCase) != 0 && (_item.isPortable || string.Compare((_item.fileType ?? "").TrimStart('.'), "zip", StringComparison.OrdinalIgnoreCase) == 0));

            installToolbarButton.Enabled = !installed;
            openAppToolbarButton.Enabled = installed && isPortableOrZip && (!string.IsNullOrEmpty(_item.installedShortcutPath) || !string.IsNullOrEmpty(_item.installDirectory));
            uninstallToolbarButton.Enabled = installed;
        }

        // changes the name of all the controls in the app detail to use Resources variables
        public void textToResource()
        {
            appSource.Text = Resources.source;
            appUploader.Text = Resources.appSourceTextUploader;
            appExtension.Text = Resources.extension;
            appCompatibility.Text = Resources.compatibility;
            appLanguage.Text = Resources.languages;
        }

        private void ApplicationDetail_Load(object sender, EventArgs e)
        {
            LayoutDetailsBanner();
            UpdateToolbarState();
        }

        private void ApplicationDetail_Resize(object sender, EventArgs e)
        {
            LayoutDetailsBanner();
            if (appDescription != null && descriptionPanel != null)
            {
                appDescription.MaximumSize = new Size(Math.Max(100, descriptionPanel.ClientSize.Width - 20), 0);
            }
        }

        /// <summary>
        /// Generic helper function to truncate any label text with "..." if it exceeds maxWidth
        /// </summary>
        public static void SetTruncatedText(Label label, string fullText, int maxWidth)
        {
            OpenImageHelper.SetTruncatedText(label, fullText, maxWidth);
        }

        // Recalculates horizontal positions: Column 4 anchored to the right, and Columns 2 & 3 dynamically sharing the center space
        private void LayoutDetailsBanner()
        {
            if (_item == null || detailsBanner == null || detailsBanner.Width <= 0) return;

            int totalWidth = detailsBanner.Width;

            // Column 1: isImmersive / isPortable (Fixed Left)
            isImmersive.Left = 8;
            isPortable.Left = 8;

            // Column 4 (Right-Anchored): Language / isTrial
            int col4RightMargin = 8;
            int col4LanguageHeaderWidth = TextRenderer.MeasureText(appLanguage.Text, appLanguage.Font).Width;
            int col4MaxContentWidth = 85; // default width for language string e.g. "en-US, it-IT"
            int col4TotalWidth = col4LanguageHeaderWidth + col4MaxContentWidth + 8;
            int col4Left = totalWidth - col4TotalWidth - col4RightMargin;
            if (col4Left < 340) col4Left = 340;

            appLanguage.Left = col4Left;
            appIsTrial.Left = col4Left;

            int col4ContentLeft = col4Left + col4LanguageHeaderWidth + 4;
            appLanguageText.Left = col4ContentLeft;
            int col4AvailableWidth = Math.Max(20, totalWidth - col4ContentLeft - col4RightMargin);
            SetTruncatedText(appLanguageText, _fullLanguageText, col4AvailableWidth);

            // Middle area between Column 1 and Column 4 is shared dynamically between Column 2 and Column 3
            int startX = 85;
            int middleAvailableWidth = col4Left - startX - 16;
            if (middleAvailableWidth < 180) middleAvailableWidth = 180;

            // Column 2 takes ~40% of the middle area, Column 3 takes ~60%
            int col2Left = startX;
            int col3Left = startX + (int)(middleAvailableWidth * 0.40);

            // Position Column 2: Source / Uploader (Dynamically expands with width)
            appSource.Left = col2Left;
            appUploader.Left = col2Left;

            int col2HeaderWidth = Math.Max(
                TextRenderer.MeasureText(appSource.Text, appSource.Font).Width,
                TextRenderer.MeasureText(appUploader.Text, appUploader.Font).Width);

            int col2ContentLeft = col2Left + col2HeaderWidth + 4;
            appSourceText.Left = col2ContentLeft;
            appUploaderText.Left = col2ContentLeft;

            int col2AvailableWidth = Math.Max(20, col3Left - col2ContentLeft - 10);
            SetTruncatedText(appSourceText, _fullSourceText, col2AvailableWidth);
            SetTruncatedText(appUploaderText, _fullUploaderText, col2AvailableWidth);

            // Position Column 3: Extension / Compatibility (Dynamically expands with width)
            appExtension.Left = col3Left;
            appCompatibility.Left = col3Left;

            int col3HeaderWidth = Math.Max(
                TextRenderer.MeasureText(appExtension.Text, appExtension.Font).Width,
                TextRenderer.MeasureText(appCompatibility.Text, appCompatibility.Font).Width);

            int col3ContentLeft = col3Left + col3HeaderWidth + 4;
            appExtensionText.Left = col3ContentLeft;
            appCompatibilityText.Left = col3ContentLeft;

            int col3AvailableWidth = Math.Max(20, col4Left - col3ContentLeft - 10);
            SetTruncatedText(appExtensionText, _fullExtensionText, col3AvailableWidth);
            SetTruncatedText(appCompatibilityText, _fullCompatibilityText, col3AvailableWidth);
        }

        private void loadAppDetails(AppModel item)
        {
            if (item == null) return;

            // 1. Title and Icon
            this.Text = (item.name ?? item.id) + Resources.appDetailTitle;
            appName.Text = item.name ?? item.id;
            appDeveloper.Text = !string.IsNullOrEmpty(item.author) ? Resources.appAuthor + item.author : "";
            appSize.Text = sizeFormat.FormattedSize(item.size);

            if (_appIconImage != null)
            {
                appIcon.Image = _appIconImage;
            }
            else
            {
                Helpers.OpenImageHelper.loadAppIcon(item.iconUrl, appIcon, _downloadService, _cacheManager, "app_" + (item.id ?? ""));
            }

            // 2. Compatibility check (Banner)
            bool isCompatible = OSver.IsAppCompatible(item.compatibility);
            bool isArchComp = OSver.IsArchCompatible(item.architecture);

            if (isCompatible && isArchComp)
            {
                appSystemCompatibility.Text = Resources.appCompatible + OSver.GetCurrentOSVersion() + " " + OSver.GetArchitecture() + ")";
                appSystemCompatibility.ForeColor = Color.Green;
            }
            else
            {
                appSystemCompatibility.Text = Resources.appNotCompatible + OSver.GetCurrentOSVersion() + " " + OSver.GetArchitecture() + ")";
                appSystemCompatibility.ForeColor = Color.Red;
            }

            // 3. Store full texts for responsive layout
            isImmersive.Text = item.isImmersive ? Resources.appImmersive : Resources.appDesktop;
            isPortable.Text = item.isPortable ? Resources.appPortable : Resources.appInstaller;

            _fullSourceText = item.source ?? Resources.notAvailable;
            _fullUploaderText = item.uploader ?? Resources.notAvailable;
            _fullExtensionText = !string.IsNullOrEmpty(item.fileType) ? "." + item.fileType.TrimStart('.') : Resources.notAvailable;

            if (item.compatibility != null && item.compatibility.Count > 0)
            {
                _fullCompatibilityText = string.Join(", ", item.compatibility.ToArray());
            }
            else
            {
                _fullCompatibilityText = Resources.appAll;
            }

            if (item.language != null && item.language.Count > 0)
            {
                _fullLanguageText = string.Join(", ", item.language.ToArray());
            }
            else
            {
                _fullLanguageText = Resources.appLanguageNeutral;
            }

            appIsTrial.Text = item.isTrial ? Resources.appTrial : Resources.appFree;

            // Apply responsive column positions and text truncation
            LayoutDetailsBanner();

            // 4. Description
            appDescription.AutoSize = true;
            appDescription.MaximumSize = new Size(descriptionPanel.ClientSize.Width - 20, 0);
            appDescription.Text = item.description ?? Resources.appNoDescription;

            // 5. Populate Details Tab (detailsTreeView)
            detailsTreeView.BeginUpdate();
            detailsTreeView.Nodes.Clear();

            TreeNode sourceNode = new TreeNode(Resources.sourceDetails);
            sourceNode.Nodes.Add(Resources.appSourceText1 + (item.source ?? Resources.notAvailable) + Resources.source2);

            // Populate all categories
            if (item.category != null && item.category.Count > 0)
            {
                for (int c = 0; c < item.category.Count; c++)
                {
                    sourceNode.Nodes.Add(Resources.category + item.category[c]);
                }
            }
            else
            {
                sourceNode.Nodes.Add(Resources.appSourceText4 + Resources.generalCategory + Resources.appSourceText3);
            }
            sourceNode.Nodes.Add(Resources.appSourceTextUploader + (item.uploader ?? Resources.notAvailable));
            sourceNode.Nodes.Add(Resources.dateCreated + " " + (item.dateOriginal.HasValue ? item.dateOriginal.Value.ToString(Resources.dateFormat) : "-"));
            sourceNode.Nodes.Add(Resources.dateUpload + " " + (item.dateSource.HasValue ? item.dateSource.Value.ToString(Resources.dateFormat) : "-"));

            TreeNode fileNode = new TreeNode(Resources.fileDetails);
            fileNode.Nodes.Add(isImmersive.Text);
            fileNode.Nodes.Add(isPortable.Text);
            fileNode.Nodes.Add(appIsTrial.Text);
            fileNode.Nodes.Add(Resources.architecture + item.architecture);
            fileNode.Nodes.Add(Resources.extension + _fullExtensionText);

            TreeNode compatNode = new TreeNode(Resources.compatibility);
            if (item.compatibility != null && item.compatibility.Count > 0)
            {
                for (int comp = 0; comp < item.compatibility.Count; comp++)
                {
                    compatNode.Nodes.Add(Resources.windowsPrefix + item.compatibility[comp]);
                }
            }
            else
            {
                compatNode.Nodes.Add(Resources.allWindowsVersion);
            }

            TreeNode langNode = new TreeNode(Resources.languages);
            if (item.language != null && item.language.Count > 0)
            {
                for (int l = 0; l < item.language.Count; l++)
                {
                    langNode.Nodes.Add(item.language[l]);
                }
            }
            else
            {
                langNode.Nodes.Add(Resources.appLanguageNeutral);
            }

            detailsTreeView.Nodes.Add(sourceNode);
            detailsTreeView.Nodes.Add(fileNode);
            detailsTreeView.Nodes.Add(compatNode);
            detailsTreeView.Nodes.Add(langNode);
            detailsTreeView.ExpandAll();
            detailsTreeView.EndUpdate();

            // 6. Screenshots Tab (FlowLayoutPanel) - Loaded in background thread to keep window instant
            appScreenshots.Controls.Clear();
            if (item.screenshots != null && item.screenshots.Count > 0)
            {
                List<string> screenshotUrls = new List<string>(item.screenshots);
                string appId = item.id;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    for (int i = 0; i < screenshotUrls.Count; i++)
                    {
                        string screenshotUrl = screenshotUrls[i];
                        if (string.IsNullOrEmpty(screenshotUrl)) continue;

                        try
                        {
                            string screenshotKey = "app_" + appId + "_shot_" + i;
                            byte[] imgBytes = _cacheManager != null ? _cacheManager.LoadImageCache(screenshotKey) : null;
                            if (imgBytes == null || imgBytes.Length == 0)
                            {
                                imgBytes = _downloadService.DownloadData(screenshotUrl);
                                if (imgBytes != null && imgBytes.Length > 0 && _cacheManager != null)
                                {
                                    _cacheManager.SaveImageCache(screenshotKey, imgBytes);
                                }
                            }

                            if (imgBytes != null && imgBytes.Length > 0)
                            {
                                Bitmap bmp = null;
                                using (MemoryStream ms = new MemoryStream(imgBytes))
                                using (Image img = Image.FromStream(ms))
                                {
                                    bmp = new Bitmap(img);
                                }

                                if (this.IsHandleCreated && !this.IsDisposed && bmp != null)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        if (this.IsDisposed) { bmp.Dispose(); return; }
                                        PictureBox pb = new PictureBox();
                                        pb.Image = bmp;
                                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                                        pb.Size = new Size(180, 120);
                                        pb.Margin = new Padding(5);
                                        pb.BorderStyle = BorderStyle.FixedSingle;
                                        pb.Cursor = Cursors.Hand;

                                        pb.DoubleClick += new EventHandler(delegate(object sender, EventArgs e)
                                        {
                                            PictureBox clickedPb = sender as PictureBox;
                                            if (clickedPb != null && clickedPb.Image != null)
                                            {
                                                OpenImageHelper.OpenImage(clickedPb.Image, appId);
                                            }
                                        });

                                        appScreenshots.Controls.Add(pb);
                                    }));
                                }
                            }
                        }
                        catch { }
                    }
                });
            }

            if (item.links != null && item.links.Count > 0)
            {
                int linkIndex = 1;
                foreach (String link in item.links.ToArray())
                {
                    if (String.IsNullOrEmpty(link)) continue;
                    ToolStripMenuItem itemTool = new ToolStripMenuItem(Resources.link + " " + linkIndex, null);
                    itemTool.Tag = link;
                    itemTool.Click += new EventHandler(openLink_Click);

                    appDetailToolstrip.Items.Add(itemTool);
                    linkIndex++;
                }
            }

            // 7. Installation Tab
            appInstallationInstruction.Items.Clear();
            if (item.instructionSteps != null)
            {
                for (int i = 0; i < item.instructionSteps.Count; i++)
                {
                    appInstallationInstruction.Items.Add((i + 1) + ". " + item.instructionSteps[i]);
                }
            }

            appInstructionParameterList.Items.Clear();
            if (item.installParameters != null)
            {
                for (int i = 0; i < item.installParameters.Count; i++)
                {
                    appInstructionParameterList.Items.Add(item.installParameters[i]);
                }
            }

            listBox1.Items.Clear();
            if (item.uninstallParameters != null)
            {
                for (int i = 0; i < item.uninstallParameters.Count; i++)
                {
                    listBox1.Items.Add(item.uninstallParameters[i]);
                }
            }

            // 8. Source Tab (Populating appSameCategoryList ListView)
            string mainCategory = (item.category != null && item.category.Count > 0) ? item.category[0] : Resources.generalCategory;
            appSourceTabText.Text = Resources.appSourceText1 + (item.source ?? Resources.appSourceUnknown) + Resources.appSourceText2 + mainCategory + Resources.appSourceText3;

            appSameCategoryList.View = View.Details;
            appSameCategoryList.FullRowSelect = true;
            appSameCategoryList.GridLines = true;
            appSameCategoryList.Columns.Clear();
            appSameCategoryList.Columns.Add(Resources.appSourceColumn1, 140);
            appSameCategoryList.Columns.Add(Resources.appSourceColumn2, 60);
            appSameCategoryList.Columns.Add(Resources.appSourceColumn3, 100);
            appSameCategoryList.Columns.Add(Resources.appSourceColumn4, 110);
            appSameCategoryList.Items.Clear();

            for (int i = 0; i < _loadedApps.Count; i++)
            {
                AppModel other = _loadedApps[i];
                if (other == null || other.id == item.id) continue;

                bool matchesCategory = false;
                if (other.category != null && item.category != null)
                {
                    for (int c = 0; c < other.category.Count; c++)
                    {
                        if (item.category.Contains(other.category[c]))
                        {
                            matchesCategory = true;
                            break;
                        }
                    }
                }

                if (matchesCategory || string.Compare(other.source, item.source, true) == 0)
                {
                    ListViewItem lvi = new ListViewItem(other.name ?? other.id);
                    lvi.SubItems.Add(other.version ?? "");
                    lvi.SubItems.Add(other.author ?? "");

                    bool isOtherComp = OSver.IsAppCompatible(other.compatibility) && OSver.IsArchCompatible(other.architecture);
                    string compStatus = isOtherComp ? Resources.appSourceCompatible : Resources.appSourceNotCompatible;
                    lvi.SubItems.Add(compStatus ?? (isOtherComp ? "Yes" : "No"));
                    lvi.Tag = other;
                    appSameCategoryList.Items.Add(lvi);
                }
            }
        }

        private void EventClickLink(object sender, EventArgs e)
        {

        }

        private void appScreenshots_DoubleClick(object sender, EventArgs e)
        {
            PictureBox clicked = sender as PictureBox;
            if (clicked != null && clicked.Image != null)
            {
                OpenImageHelper.OpenImage(clicked.Image, _item.id ?? Resources.screenshot);
            }
        }

        private void copyLinkButton_Click(object sender, EventArgs e)
        {
            if (_item != null && !string.IsNullOrEmpty(_item.downloadUrl)) Clipboard.SetText(_item.downloadUrl);
        }

        private void downloadFile_Click(object sender, EventArgs e)
        {
            string dateOrigStr = (_item != null && _item.dateOriginal.HasValue) ? _item.dateOriginal.Value.ToString(Resources.dateFormat) : "-";
            string dateSrcStr = (_item != null && _item.dateSource.HasValue) ? _item.dateSource.Value.ToString(Resources.dateFormat) : "-";
            URLdownload download = new URLdownload(_item.name, _item.source, _item.downloadUrl, _item.fileType, dateOrigStr, dateSrcStr, _item.size);
            download.Location = this.Location;
            download.Show();
        }

        private void openLink_Click(object sender, EventArgs e)
        {
            ToolStripItem clicked = (ToolStripItem)sender as ToolStripItem;
            if (clicked != null && clicked.Tag != null)
            {
                String url = clicked.Tag.ToString();
                try
                {
                    Process.Start(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, Resources.error + " " + ex.ToString(), Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private SourceModel FindSourceModel(string sourceNameOrId)
        {
            if (string.IsNullOrEmpty(sourceNameOrId)) return null;

            if (_loadedSources != null)
            {
                for (int i = 0; i < _loadedSources.Count; i++)
                {
                    SourceModel sm = _loadedSources[i];
                    if (sm != null && (string.Compare(sm.id, sourceNameOrId, true) == 0 ||
                                       string.Compare(sm.name, sourceNameOrId, true) == 0))
                    {
                        return sm;
                    }
                }
            }

            if (_cacheManager != null)
            {
                try
                {
                    string userSourcesJson = _cacheManager.LoadUserSources();
                    if (!string.IsNullOrEmpty(userSourcesJson))
                    {
                        CatalogManager cm = new CatalogManager();
                        List<SourceModel> parsed = cm.ParseSourcesJson(userSourcesJson);
                        if (parsed != null)
                        {
                            for (int i = 0; i < parsed.Count; i++)
                            {
                                SourceModel sm = parsed[i];
                                if (sm != null && (string.Compare(sm.id, sourceNameOrId, true) == 0 ||
                                                   string.Compare(sm.name, sourceNameOrId, true) == 0))
                                {
                                    return sm;
                                }
                            }
                        }
                    }
                }
                catch { }
            }

            SourceModel fallback = new SourceModel();
            fallback.id = sourceNameOrId;
            fallback.name = sourceNameOrId;
            fallback.author = "Unknown";
            fallback.description = "Source " + sourceNameOrId;
            fallback.isEnabled = true;
            fallback.dateCreated = DateTime.Now;
            fallback.lastUpdated = DateTime.Now;
            return fallback;
        }

        private void appSourceText_Click(object sender, EventArgs e)
        {
            if (_item != null && !string.IsNullOrEmpty(_item.source))
            {
                this.Cursor = Cursors.WaitCursor;
                SourceModel sm = FindSourceModel(_item.source);
                Image sourceIcon = null;
                if (sm != null && !string.IsNullOrEmpty(sm.iconUrl) && _downloadService != null)
                {
                    try
                    {
                        byte[] data = _cacheManager != null ? _cacheManager.LoadImageCache("src_" + (!string.IsNullOrEmpty(sm.id) ? sm.id : sm.name)) : null;
                        if (data == null || data.Length == 0)
                        {
                            data = _downloadService.DownloadData(sm.iconUrl);
                        }
                        if (data != null && data.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(data))
                            {
                                sourceIcon = new Bitmap(Image.FromStream(ms));
                            }
                        }
                    }
                    catch { }
                }

                SourcesDetails sd = new SourcesDetails(sm, sourceIcon, _loadedApps, _downloadService);
                sd.Shown += delegate
                {
                    this.Cursor = Cursors.Default;
                };
                sd.Show();
            }
        }

        private void appSameCategoryList_DoubleClick(object sender, EventArgs e)
        {
            if (appSameCategoryList.SelectedItems.Count > 0)
            {
                AppModel selectedApp = appSameCategoryList.SelectedItems[0].Tag as AppModel;
                if (selectedApp != null)
                {
                    ApplicationDetail ad = new ApplicationDetail(selectedApp, _downloadService, _loadedApps, null, _cacheManager, _loadedSources);
                    ad.Show();
                }
            }
        }

        private void installToolbarButton_Click(object sender, EventArgs e)
        {
            if (_item == null) return;
            if (_installerService == null) _installerService = new InstallerService(_installationRegistry ?? new InstallationRegistry());

            URLdownload installerForm = new URLdownload(_item, _installerService);
            installerForm.Location = this.Location;
            if (installerForm.ShowDialog(this) == DialogResult.OK)
            {
                UpdateToolbarState();
            }
        }

        private void openAppToolbarButton_Click(object sender, EventArgs e)
        {
            if (_item == null) return;

            try
            {
                if (!string.IsNullOrEmpty(_item.installedShortcutPath) && File.Exists(_item.installedShortcutPath))
                {
                    Process.Start(_item.installedShortcutPath);
                }
                else if (!string.IsNullOrEmpty(_item.installDirectory) && Directory.Exists(_item.installDirectory))
                {
                    string targetExe = !string.IsNullOrEmpty(_item.downloadPath) ? Path.Combine(_item.installDirectory, _item.downloadPath) : "";
                    if (!string.IsNullOrEmpty(targetExe) && File.Exists(targetExe))
                    {
                        Process.Start(targetExe);
                    }
                    else
                    {
                        string[] exes = Directory.GetFiles(_item.installDirectory, "*.exe", SearchOption.AllDirectories);
                        if (exes != null && exes.Length > 0)
                        {
                            Process.Start(exes[0]);
                        }
                        else
                        {
                            Process.Start(_item.installDirectory);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, Resources.error + " " + ex.Message, Resources.error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void uninstallToolbarButton_Click(object sender, EventArgs e)
        {
            if (_item == null) return;

            bool isPortableOrZip = (_item != null && string.Compare((_item.fileType ?? "").TrimStart('.'), "msi", StringComparison.OrdinalIgnoreCase) != 0 && (_item.isPortable || string.Compare((_item.fileType ?? "").TrimStart('.'), "zip", StringComparison.OrdinalIgnoreCase) == 0));
            if (!isPortableOrZip)
            {
                if (_installationRegistry != null)
                {
                    _installationRegistry.UnregisterApp(_item.id);
                }
                _item.isInstalled = false;
                _item.installedOn = null;
                _item.installedShortcutPath = null;
                _item.installDirectory = null;

                UpdateToolbarState();
                InstallerService.OpenAddRemovePrograms();
                MessageBox.Show(this, "Since this is installed, remember to uninstall it from Control Panel.", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DialogResult dr = MessageBox.Show(this, "Are you sure you want to uninstall " + (_item.name ?? _item.id) + "?", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    if (_installerService == null) _installerService = new InstallerService(_installationRegistry ?? new InstallationRegistry());

                    bool uninstalled = _installerService.UninstallApp(_item);
                    if (uninstalled)
                    {
                        UpdateToolbarState();
                        MessageBox.Show(this, (_item.name ?? _item.id) + " uninstalled successfully.", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this, "Failed to uninstall " + (_item.name ?? _item.id), Resources.error ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}