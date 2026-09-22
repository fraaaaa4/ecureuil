using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using Ecureuil.Properties;
using Ecureuil.Core.Services;
using Ecureuil.Core.Models;
using Ecureuil.Core.Helpers;

namespace Ecureuil
{
    public partial class settings : Form
    {
        private CacheManager cache;
        private List<SourceModel> loadedSources;

        public settings() : this(new CacheManager(), new List<SourceModel>())
        {
        }

        public settings(CacheManager _cache, List<SourceModel> _loadedSources)
        {
            InitializeComponent();
            cache = _cache;
            loadedSources = _loadedSources;
        }

        private void settings_Load(object sender, EventArgs e)
        {
            TextResource();
            LoadCacheList();
            CacheCheckboxLoadCheck();
            LoadInstallationSettings();
            LoadFileTabSettings();
            if (!changedStuff) ApplyButton.Enabled = false; else ApplyButton.Enabled = true;
            if (Ecureuil.Properties.Settings.Default.enableCache) enableCacheCheckbox.Checked = true;
            else enableCacheCheckbox.Checked = false;

            sourcesCacheListView.ItemChecked += new ItemCheckedEventHandler(cacheListView_ItemChecked);
            appsCacheListView.ItemChecked += new ItemCheckedEventHandler(cacheListView_ItemChecked);
        }

        private void TextResource()
        {
            this.Text = Resources.settings;
            OKButton.Text = Resources.ok;
            ApplyButton.Text = Resources.apply;
            CancelButton.Text = Resources.cancel;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (installRadio2.Checked && !ValidateInstallPath(true))
            {
                return;
            }
            SaveSettings();
            this.Close();
        }

        private bool settingsSaved = false;
        private bool changedStuff = false;

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            if (installRadio2.Checked && !ValidateInstallPath(true))
            {
                return;
            }
            if (!settingsSaved && changedStuff)
            {
                SaveSettings();
                settingsSaved = true;
                ApplyButton.Enabled = false;
            }
        }

        private void SaveSettings()
        {
            Settings.Default.installSetting = installRadio1.Checked;
            Settings.Default.installPath = textBox1.Text;
            Settings.Default.StartMenuLink = startMenuCheck.Checked;
            Settings.Default.Save();
        }

        private void LoadFileTabSettings()
        {
            string defaultCache = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Ecureuil\Cache");
            cacheFileTextbox.Text = defaultCache;

            string installedPath = Settings.Default.installSetting
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps")
                : (Settings.Default.installPath ?? "");
            textBox2.Text = installedPath;

            string defaultScreenshots = Path.Combine(Path.GetTempPath(), "Ecureuil_Screenshots");
            screenshotFileTextbox.Text = defaultScreenshots;
        }

        private void OpenFolderInExplorer(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                Process.Start("explorer.exe", "\"" + folderPath + "\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.error2 ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCacheList()
        {
            sourcesCacheListView.BeginUpdate();
            appsCacheListView.BeginUpdate();

            sourcesCacheListView.Items.Clear();
            appsCacheListView.Items.Clear();

            if (cache != null && !string.IsNullOrEmpty(cache.CachePath) && Directory.Exists(cache.CachePath))
            {
                DirectoryInfo dir = new DirectoryInfo(cache.CachePath);
                FileInfo[] cacheFiles = dir.GetFiles("*.json");

                // Also check user_sources.json in AppData root if exists
                string userSourcesPath = Path.Combine(Path.GetDirectoryName(cache.CachePath), "user_sources.json");
                if (File.Exists(userSourcesPath))
                {
                    FileInfo usf = new FileInfo(userSourcesPath);
                    ListViewItem usItem = new ListViewItem(Resources.userSourcesIndex);
                    usItem.SubItems.Add(usf.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                    usItem.ToolTipText = usf.FullName;
                    usItem.Tag = usf;
                    sourcesCacheListView.Items.Add(usItem);
                }

                for (int i = 0; i < cacheFiles.Length; i++)
                {
                    FileInfo file = cacheFiles[i];
                    string name = file.Name;

                    if (name.StartsWith("cache_catalog_") || name.StartsWith("source_info_"))
                    {
                        string displayName = name
                            .Replace("cache_catalog_", Resources.catalogPrefix)
                            .Replace("source_info_", Resources.infoPrefix)
                            .Replace(".json", "");

                        ListViewItem item = new ListViewItem(displayName);
                        item.SubItems.Add(file.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                        item.ToolTipText = file.FullName;
                        item.Tag = file;
                        sourcesCacheListView.Items.Add(item);
                    }
                    else if (name.StartsWith("cache_app_") || name.StartsWith("app_"))
                    {
                        string displayName = name
                            .Replace("cache_app_", "")
                            .Replace("app_", "")
                            .Replace(".json", "");

                        ListViewItem item = new ListViewItem(displayName);
                        item.SubItems.Add(file.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                        item.ToolTipText = file.FullName;
                        item.Tag = file;
                        appsCacheListView.Items.Add(item);
                    }
                    else
                    {
                        // Default to sources tab for other json cache files
                        string displayName = name.Replace(".json", "");
                        ListViewItem item = new ListViewItem(displayName);
                        item.SubItems.Add(file.LastWriteTime.ToString("dd/MM/yyyy HH:mm"));
                        item.ToolTipText = file.FullName;
                        item.Tag = file;
                        sourcesCacheListView.Items.Add(item);
                    }
                }
            }

            sourcesCacheListView.EndUpdate();
            appsCacheListView.EndUpdate();

            UpdateSelectedCacheSize();
        }

        private void cacheListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateSelectedCacheSize();
        }

        private void UpdateSelectedCacheSize()
        {
            if (sourcesCacheListView == null || appsCacheListView == null || selectedCacheSizeLabel == null) return;

            long totalSelectedBytes = 0;
            try
            {
                totalSelectedBytes += GetSelectedItemsCacheSize(sourcesCacheListView, true);
                totalSelectedBytes += GetSelectedItemsCacheSize(appsCacheListView, false);
            }
            catch { }

            selectedCacheSizeLabel.Text = Resources.sizePrefix + sizeFormat.FormattedSize(totalSelectedBytes);
        }

        private long GetSelectedItemsCacheSize(ListView listView, bool isSource)
        {
            if (listView == null || listView.Items == null) return 0;
            long total = 0;
            string[] datFiles = null;

            for (int i = 0; i < listView.Items.Count; i++)
            {
                ListViewItem item = listView.Items[i];
                if (item != null && item.Checked)
                {
                    FileInfo fi = item.Tag as FileInfo;
                    if (fi != null && fi.Exists)
                    {
                        total += fi.Length;
                        string name = fi.Name;
                        if (datFiles == null && !string.IsNullOrEmpty(fi.DirectoryName) && Directory.Exists(fi.DirectoryName))
                        {
                            datFiles = Directory.GetFiles(fi.DirectoryName, "*.dat");
                        }

                        if (datFiles != null && datFiles.Length > 0)
                        {
                            string id = name.Replace("cache_catalog_", "").Replace("source_info_", "").Replace("cache_app_", "").Replace("app_", "").Replace(".json", "").ToLowerInvariant();
                            for (int p = 0; p < datFiles.Length; p++)
                            {
                                string fn = Path.GetFileName(datFiles[p]).ToLowerInvariant();
                                bool matches = isSource ?
                                    (fn == "img_" + id + ".dat" || fn == "img_src_" + id + ".dat" || fn == "img_disc_" + id + ".dat" ||
                                     fn.StartsWith("img_src_" + id + "_") || fn.StartsWith("img_disc_" + id + "_") || fn.StartsWith("img_" + id + "_")) :
                                    (fn == "img_" + id + ".dat" || fn == "img_app_" + id + ".dat" ||
                                     fn.StartsWith("img_app_" + id + "_") || fn.StartsWith("img_" + id + "_"));

                                if (matches)
                                {
                                    try { total += new FileInfo(datFiles[p]).Length; } catch { }
                                }
                            }
                        }
                    }
                }
            }
            return total;
        }

        private void deleteCacheButton_Click(object sender, EventArgs e)
        {
            List<FileInfo> filesToDelete = new List<FileInfo>();

            if (sourcesCacheListView != null && sourcesCacheListView.Items != null)
            {
                for (int i = 0; i < sourcesCacheListView.Items.Count; i++)
                {
                    if (sourcesCacheListView.Items[i].Checked)
                    {
                        FileInfo fi = sourcesCacheListView.Items[i].Tag as FileInfo;
                        if (fi != null && fi.Exists) filesToDelete.Add(fi);
                    }
                }
            }

            if (appsCacheListView != null && appsCacheListView.Items != null)
            {
                for (int i = 0; i < appsCacheListView.Items.Count; i++)
                {
                    if (appsCacheListView.Items[i].Checked)
                    {
                        FileInfo fi = appsCacheListView.Items[i].Tag as FileInfo;
                        if (fi != null && fi.Exists) filesToDelete.Add(fi);
                    }
                }
            }

            if (filesToDelete.Count == 0)
            {
                MessageBox.Show(this, Resources.selectCacheToDelete, Resources.cacheTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string confirmDeleteMsg = string.Format(Resources.deleteCacheConfirm, filesToDelete.Count);
            DialogResult dr = MessageBox.Show(this, confirmDeleteMsg, Resources.deleteCacheTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                for (int i = 0; i < filesToDelete.Count; i++)
                {
                    FileInfo fi = filesToDelete[i];
                    try
                    {
                        string name = fi.Name;
                        if (name.StartsWith("cache_catalog_") || name.StartsWith("source_info_"))
                        {
                            string srcId = name.Replace("cache_catalog_", "").Replace("source_info_", "").Replace(".json", "");
                            if (cache != null) cache.ClearSourceCache(srcId);
                        }
                        else if (name.StartsWith("cache_app_") || name.StartsWith("app_"))
                        {
                            string appId = name.Replace("cache_app_", "").Replace("app_", "").Replace(".json", "");
                            if (cache != null) cache.ClearAppCache(appId);
                        }
                        else
                        {
                            fi.Delete();
                        }
                    }
                    catch { }
                }

                LoadCacheList();
            }
        }

        private void refreshCacheButton_Click(object sender, EventArgs e)
        {
            List<string> sourcesToRefresh = new List<string>();
            List<string> appsToRefresh = new List<string>();

            if (sourcesCacheListView != null && sourcesCacheListView.Items != null)
            {
                for (int i = 0; i < sourcesCacheListView.Items.Count; i++)
                {
                    if (sourcesCacheListView.Items[i].Checked)
                    {
                        FileInfo fi = sourcesCacheListView.Items[i].Tag as FileInfo;
                        if (fi != null)
                        {
                            string name = fi.Name;
                            if (name.StartsWith("cache_catalog_"))
                            {
                                string srcId = name.Substring("cache_catalog_".Length).Replace(".json", "");
                                if (!sourcesToRefresh.Contains(srcId)) sourcesToRefresh.Add(srcId);
                            }
                            else if (name.StartsWith("source_info_"))
                            {
                                string srcId = name.Substring("source_info_".Length).Replace(".json", "");
                                if (!sourcesToRefresh.Contains(srcId)) sourcesToRefresh.Add(srcId);
                            }
                            else if (string.Compare(name, "user_sources.json", true) == 0)
                            {
                                if (loadedSources != null)
                                {
                                    for (int s = 0; s < loadedSources.Count; s++)
                                    {
                                        if (loadedSources[s] != null && !string.IsNullOrEmpty(loadedSources[s].id))
                                        {
                                            if (!sourcesToRefresh.Contains(loadedSources[s].id)) sourcesToRefresh.Add(loadedSources[s].id);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (appsCacheListView != null && appsCacheListView.Items != null)
            {
                for (int i = 0; i < appsCacheListView.Items.Count; i++)
                {
                    if (appsCacheListView.Items[i].Checked)
                    {
                        FileInfo fi = appsCacheListView.Items[i].Tag as FileInfo;
                        if (fi != null)
                        {
                            string name = fi.Name;
                            string appId = name.Replace("cache_app_", "").Replace("app_", "").Replace(".json", "");
                            if (!appsToRefresh.Contains(appId)) appsToRefresh.Add(appId);
                        }
                    }
                }
            }

            int totalSelected = sourcesToRefresh.Count + appsToRefresh.Count;

            if (totalSelected == 0)
            {
                DialogResult dr = MessageBox.Show(this, Resources.refreshAllCacheConfirm, Resources.refreshCacheTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes)
                {
                    LoadCacheList();
                    return;
                }

                if (loadedSources != null)
                {
                    for (int i = 0; i < loadedSources.Count; i++)
                    {
                        if (loadedSources[i] != null && !string.IsNullOrEmpty(loadedSources[i].id))
                        {
                            sourcesToRefresh.Add(loadedSources[i].id);
                        }
                    }
                }
            }

            refreshCacheButton.Enabled = false;
            deleteCacheButton.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                DownloadService downloadService = new DownloadService();
                CatalogManager catalogManager = new CatalogManager();

                // 1. Refresh selected/all sources
                if (loadedSources != null && sourcesToRefresh.Count > 0)
                {
                    for (int s = 0; s < loadedSources.Count; s++)
                    {
                        SourceModel sm = loadedSources[s];
                        if (sm == null || string.IsNullOrEmpty(sm.url)) continue;

                        string srcId = !string.IsNullOrEmpty(sm.id) ? sm.id : sm.name;
                        if (sourcesToRefresh.Contains(srcId) || sourcesToRefresh.Contains(sm.id) || sourcesToRefresh.Contains(sm.name))
                        {
                            try
                            {
                                string newIndexJson = downloadService.DownloadString(sm.url);
                                if (!string.IsNullOrEmpty(newIndexJson) && cache != null)
                                {
                                    cache.SaveSourceCatalogCache(srcId, newIndexJson);

                                    List<AppModel> parsedApps = catalogManager.ParseIndexJson(newIndexJson);
                                    if (parsedApps != null)
                                    {
                                        for (int a = 0; a < parsedApps.Count; a++)
                                        {
                                            AppModel app = parsedApps[a];
                                            if (app != null && !string.IsNullOrEmpty(app.id))
                                            {
                                                string appJson = MiniJson.Serialize(app);
                                                cache.SaveAppCache(app.id, appJson);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                }

                // 2. Refresh specific selected apps if applicable
                if (appsToRefresh.Count > 0 && loadedSources != null)
                {
                    for (int s = 0; s < loadedSources.Count; s++)
                    {
                        SourceModel sm = loadedSources[s];
                        if (sm == null || string.IsNullOrEmpty(sm.url)) continue;

                        try
                        {
                            string indexJson = downloadService.DownloadString(sm.url);
                            if (!string.IsNullOrEmpty(indexJson))
                            {
                                List<AppModel> parsedApps = catalogManager.ParseIndexJson(indexJson);
                                if (parsedApps != null)
                                {
                                    for (int a = 0; a < parsedApps.Count; a++)
                                    {
                                        AppModel app = parsedApps[a];
                                        if (app != null && appsToRefresh.Contains(app.id))
                                        {
                                            string appJson = MiniJson.Serialize(app);
                                            cache.SaveAppCache(app.id, appJson);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        refreshCacheButton.Enabled = true;
                        deleteCacheButton.Enabled = true;
                        this.Cursor = Cursors.Default;
                        LoadCacheList();
                    }));
                }
            });
        }

        private void enableCacheCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            CacheCheckboxCheck();
            changedStuff = true;
            ApplyButton.Enabled = true;
        }

        private void CacheCheckboxCheck()
        {
            Ecureuil.Properties.Settings.Default.enableCache = enableCacheCheckbox.Checked;
            Ecureuil.Properties.Settings.Default.Save();

            sourcesCacheListView.Enabled = enableCacheCheckbox.Checked;
            appsCacheListView.Enabled = enableCacheCheckbox.Checked;
            refreshCacheButton.Enabled = enableCacheCheckbox.Checked;
            deleteCacheButton.Enabled = enableCacheCheckbox.Checked;

            settingsSaved = false;
        }

        private void CacheCheckboxLoadCheck()
        {
            enableCacheCheckbox.Checked = Ecureuil.Properties.Settings.Default.enableCache;

            sourcesCacheListView.Enabled = enableCacheCheckbox.Checked;
            appsCacheListView.Enabled = enableCacheCheckbox.Checked;
            refreshCacheButton.Enabled = enableCacheCheckbox.Checked;
            deleteCacheButton.Enabled = enableCacheCheckbox.Checked;
        }

        private bool isLoadingSettings = false;

        private void LoadInstallationSettings()
        {
            isLoadingSettings = true;
            try
            {
                string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps");
                installLabel3.Text = "Current default installation path:\n" + defaultPath;

                bool isDefault = Settings.Default.installSetting;
                installRadio1.Checked = isDefault;
                installRadio2.Checked = !isDefault;
                textBox1.Text = Settings.Default.installPath ?? "";
                startMenuCheck.Checked = Settings.Default.StartMenuLink;

                UpdateInstallationControlsState();
            }
            finally
            {
                isLoadingSettings = false;
            }
        }

        private void UpdateInstallationControlsState()
        {
            bool custom = installRadio2.Checked;
            textBox1.Enabled = custom;
            browseButton.Enabled = custom;
        }

        private ToolTip _installPathToolTip;

        public void ShowPathWarning(Control targetControl, string message, string title)
        {
            if (_installPathToolTip == null)
            {
                _installPathToolTip = new ToolTip();
                _installPathToolTip.IsBalloon = true;
                _installPathToolTip.ToolTipIcon = ToolTipIcon.Warning;
            }
            _installPathToolTip.ToolTipTitle = !string.IsNullOrEmpty(title) ? title : Resources.invalidUrlTitle;
            if (targetControl != null && targetControl.Visible)
            {
                _installPathToolTip.Show(message, targetControl, 0, targetControl.Height, 3500);
                _installPathToolTip.Hide(targetControl);
                _installPathToolTip.Show(message, targetControl, 0, targetControl.Height, 3500);
            }
        }

        public void HidePathWarning(Control targetControl)
        {
            if (_installPathToolTip != null && targetControl != null)
            {
                _installPathToolTip.Hide(targetControl);
            }
        }

        public bool ValidateInstallPath(bool showWarning)
        {
            if (installRadio2.Checked)
            {
                string path = (textBox1.Text ?? "").Trim();
                if (string.IsNullOrEmpty(path))
                {
                    if (showWarning)
                    {
                        ShowPathWarning(textBox1, "Please specify an installation path.", "Invalid Path");
                    }
                    return false;
                }

                try
                {
                    if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || !Path.IsPathRooted(path))
                    {
                        if (showWarning)
                        {
                            ShowPathWarning(textBox1, "The specified path is not a valid absolute path.", "Invalid Path");
                        }
                        return false;
                    }
                }
                catch
                {
                    if (showWarning)
                    {
                        ShowPathWarning(textBox1, "The specified path is invalid.", "Invalid Path");
                    }
                    return false;
                }
            }

            HidePathWarning(textBox1);
            return true;
        }

        private void installRadio1_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            UpdateInstallationControlsState();
            HidePathWarning(textBox1);
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void installRadio2_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            UpdateInstallationControlsState();
            if (installRadio2.Checked)
            {
                ValidateInstallPath(true);
            }
            else
            {
                HidePathWarning(textBox1);
            }
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;

            if (installRadio2.Checked)
            {
                ValidateInstallPath(true);
            }
            else
            {
                HidePathWarning(textBox1);
            }

            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text) && Directory.Exists(textBox1.Text))
            {
                folderBrowserDialog1.SelectedPath = textBox1.Text;
            }
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                changedStuff = true;
                settingsSaved = false;
                ApplyButton.Enabled = true;
            }
        }

        private void startMenuCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        #region Files and Folders Tab Handlers

        private void cacheFileTextbox_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void exploreCacheFileButton_Click(object sender, EventArgs e)
        {
            string path = !string.IsNullOrEmpty(cacheFileTextbox.Text)
                ? cacheFileTextbox.Text
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Ecureuil\Cache");
            OpenFolderInExplorer(path);
        }

        private void browseCacheFile_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cacheFileTextbox.Text) && Directory.Exists(cacheFileTextbox.Text))
            {
                folderBrowserDialog1.SelectedPath = cacheFileTextbox.Text;
            }
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                cacheFileTextbox.Text = folderBrowserDialog1.SelectedPath;
                changedStuff = true;
                settingsSaved = false;
                ApplyButton.Enabled = true;
            }
        }

        private void defaultCacheFile_Click(object sender, EventArgs e)
        {
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Ecureuil\Cache");
            cacheFileTextbox.Text = defaultPath;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            // Sync with textBox1 on installation tab
            textBox1.Text = textBox2.Text;
            installRadio2.Checked = true;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void installedFileExplore_Click(object sender, EventArgs e)
        {
            string path = !string.IsNullOrEmpty(textBox2.Text)
                ? textBox2.Text
                : (Settings.Default.installSetting
                    ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps")
                    : (Settings.Default.installPath ?? ""));
            OpenFolderInExplorer(path);
        }

        private void installedFileBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox2.Text) && Directory.Exists(textBox2.Text))
            {
                folderBrowserDialog1.SelectedPath = textBox2.Text;
            }
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                installRadio2.Checked = true;
                changedStuff = true;
                settingsSaved = false;
                ApplyButton.Enabled = true;
            }
        }

        private void installedFileDefault_Click(object sender, EventArgs e)
        {
            string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "EcureuilApps");
            textBox2.Text = defaultPath;
            textBox1.Text = defaultPath;
            installRadio1.Checked = true;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void screenshotFileTextbox_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        private void screenshotExploreButton_Click(object sender, EventArgs e)
        {
            string path = !string.IsNullOrEmpty(screenshotFileTextbox.Text)
                ? screenshotFileTextbox.Text
                : Path.Combine(Path.GetTempPath(), "Ecureuil_Screenshots");
            OpenFolderInExplorer(path);
        }

        private void screenshotFileBrowse_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(screenshotFileTextbox.Text) && Directory.Exists(screenshotFileTextbox.Text))
            {
                folderBrowserDialog1.SelectedPath = screenshotFileTextbox.Text;
            }
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                screenshotFileTextbox.Text = folderBrowserDialog1.SelectedPath;
                changedStuff = true;
                settingsSaved = false;
                ApplyButton.Enabled = true;
            }
        }

        private void screenshotFileDefaultButton_Click(object sender, EventArgs e)
        {
            string defaultPath = Path.Combine(Path.GetTempPath(), "Ecureuil_Screenshots");
            screenshotFileTextbox.Text = defaultPath;
            changedStuff = true;
            settingsSaved = false;
            ApplyButton.Enabled = true;
        }

        #endregion
    }
}