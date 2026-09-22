using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ecureuil.Core.Helpers;
using Ecureuil.Core.Models;
using Ecureuil.Core.Services;
using Ecureuil.Helpers;
using Ecureuil.Properties;
using System.Threading;
using System.Diagnostics;

namespace Ecureuil
{
    public partial class mainWindow : Form
    {
        private CatalogManager _catalogManager;
        private CacheManager _cacheManager;
        private DownloadService _downloadService;
        private List<AppModel> _loadedApps;
        private List<SourceModel> _loadedSources;
        private SourceModel _currentSelectedSource;
        private Dictionary<string, Image> _sourceIconCache;
        private List<sourceDiscoverModel> _loadedDiscoveredSources;
        private InstallationRegistry _installationRegistry;
        private InstallerService _installerService;
        private ContextMenuStrip _appItemContextMenu;
        private ToolStripMenuItem _appOpenMenuItem;
        private ToolStripMenuItem _appUninstallMenuItem;
        private ToolStripMenuItem _appDetailsMenuItem;
        private bool _showGroupsInApplications = false;
        private List<AppModel> _currentBaseApps;
        private bool _currentFilterDisabledSources = true;
        private bool _isViewingDiscovered = false;
        private List<TreeNode> _navHistory = new List<TreeNode>();
        private int _navHistoryIndex = -1;
        private bool _isNavigatingHistory = false;
        private bool _isSearchPlaceholder = true;
        private bool addSourceChange = false;
        private ToolTip _sourceUrlToolTip;

        // Form1 constructor
        public mainWindow()
        {
            InitializeComponent();
            OpenImageHelper.CleanTempDirectory();
            _catalogManager = new CatalogManager();
            _cacheManager = new CacheManager();
            _downloadService = new DownloadService();
            _installationRegistry = new InstallationRegistry();
            _installerService = new InstallerService(_installationRegistry);
            _loadedApps = new List<AppModel>();
            _loadedSources = new List<SourceModel>();
            _loadedDiscoveredSources = new List<sourceDiscoverModel>();
            _sourceIconCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

            LoadInstalledSourcesFromSettings();
            
            // setups UI with resources text etc
            SetupUI();
            this.FormClosing += new FormClosingEventHandler(mainWindow_FormClosing);
        }

        //when closes, saves settings
        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //Settings.Default.showSidebar = !splitContainer1.Panel1Collapsed;
                //Settings.Default.showToolbar = mainToolStrip.Visible;
                Settings.Default.Save();
            }
            catch { }
            SaveInstalledSourcesToSettings();
        }

        // loads sources from the sources cache or settings variable
        private void LoadInstalledSourcesFromSettings()
        {
            try
            {
                // first, clear the apps loaded before
                if (_loadedApps == null) _loadedApps = new List<AppModel>();
                _loadedApps.Clear();

                // try to load user sources from CacheManager first, then fallback to Settings
                string savedJson = _cacheManager != null ? _cacheManager.LoadUserSources() : null;
                if (string.IsNullOrEmpty(savedJson))
                {
                    savedJson = Settings.Default.installedSourcesData;
                }

                if (!string.IsNullOrEmpty(savedJson))
                {
                    List<SourceModel> savedSources = _catalogManager.ParseSourcesJson(savedJson);
                    if (savedSources != null && savedSources.Count > 0)
                    {
                        _loadedSources = savedSources;
                    }
                }

                // load apps for all sources using cache first (offline), or network if missing/expired
                if (_loadedSources != null)
                {
                    for (int i = 0; i < _loadedSources.Count; i++)
                    {
                        LoadAndCacheSourceApps(_loadedSources[i], false);
                    }
                }

                if (_installationRegistry != null && _loadedApps != null)
                {
                    _installationRegistry.ApplyStatusToApps(_loadedApps);
                }
            }
            catch { }
        }

        // save the sources installed to the cache or Settings
        public void SaveInstalledSourcesToSettings()
        {
            try
            {
                string json = (_loadedSources != null && _loadedSources.Count > 0) ? MiniJson.Serialize(_loadedSources) : "";
                Settings.Default.installedSourcesData = json;
                if (_cacheManager != null)
                {
                    _cacheManager.SaveUserSources(json);
                }
                Settings.Default.Save();
            }
            catch { }
        }

        // setups the whole UI with variables and all
        private void SetupUI()
        {
            // populate base navigation tree structure
            PopulateNavigationTree();

            _appItemContextMenu = new ContextMenuStrip();
            _appOpenMenuItem = new ToolStripMenuItem(Resources.open, null, appOpenMenuItem_Click);
            _appUninstallMenuItem = new ToolStripMenuItem(Resources.uninstallString, null, appUninstallMenuItem_Click);
            _appDetailsMenuItem = new ToolStripMenuItem(Resources.details, null, appDetailsMenuItem_Click);

            _appItemContextMenu.Items.Add(_appOpenMenuItem);
            _appItemContextMenu.Items.Add(_appUninstallMenuItem);
            _appItemContextMenu.Items.Add(new ToolStripSeparator());
            _appItemContextMenu.Items.Add(_appDetailsMenuItem);

            _appItemContextMenu.Opening += new CancelEventHandler(appItemContextMenu_Opening);
            categoryItemView.ContextMenuStrip = _appItemContextMenu;

            categoryItemView.Columns.Clear();
            categoryItemView.Columns.Add(Resources.name, 140, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.version, 60, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.author, 90, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.size, 70, HorizontalAlignment.Right);
            categoryItemView.Columns.Add(Resources.description, 180, HorizontalAlignment.Left);

            // ensure sourcePanel is hidden at startup
            HideSourcePanel();

            // names of UI elements should be the ones in Resources for easier editing
            discoverSourcesText1.Text = Resources.discoverSources;
            sourcesTitle.Text = Resources.secondCategoryNode;
            sourcesDescription.Text = Resources.sourcesDescription;
            installedSourcesText.Text = Resources.installedSourcesList;
            addSource.Text = Resources.add;
            deleteSource.Text = Resources.delete;
            propertiesSources.Text = Resources.properties;
            sourcesGroup.Text = Resources.sourcesGroup;
            addSourcesText.Text = Resources.addSourcesText;
            discoverLink.Text = Resources.discoverSources;
            refreshButton.Text = Resources.refresh;
            refreshContextMenu.Items[0].Text = Resources.refreshOnly;
            refreshContextMenu.Items[1].Text = Resources.refreshAll;

            addSourceContextText.Text = Resources.addSourceURL;
            discoverSourcesToolStripMenuItem.Text = Resources.discoverSources;

            btnBack.Text = Resources.back ?? "Back";
            btnBack.ToolTipText = Resources.back ?? "Back";
            btnForward.Text = Resources.forward ?? "Forward";
            btnForward.ToolTipText = Resources.forward ?? "Forward";
            btnRefresh.Text = Resources.refresh ?? "Refresh";
            btnRefresh.ToolTipText = Resources.refreshCurrentTooltip ?? "Refresh current view";
            lblOrderBy.Text = Resources.orderByLabel ?? "Order by:";
            lblGroupBy.Text = Resources.groupByLabel ?? "Group by:";
            txtSearch.Text = Resources.searchPlaceholder ?? "Search...";
            txtSearch.ToolTipText = Resources.searchTooltip ?? "Search in current view";
            txtSearch.ForeColor = SystemColors.ControlText;

            if (cmbOrderBy.Items.Count > 0 && cmbOrderBy.SelectedIndex < 0) cmbOrderBy.SelectedIndex = 0;
            if (cmbGroupBy.Items.Count > 0 && cmbGroupBy.SelectedIndex < 0) cmbGroupBy.SelectedIndex = 0;

            navigationBarToolStripMenuItem.Text = Resources.toolbar ?? "Toolbar";

            try
            {
                bool isToolbarVisible = Settings.Default.showToolbar;
                mainToolStrip.Visible = isToolbarVisible;
                navigationBarToolStripMenuItem.Checked = isToolbarVisible;

                bool isSidebarVisible = Settings.Default.showSidebar;
                splitContainer1.Panel1Collapsed = !isSidebarVisible;
                sidebarToolStripMenuItem.Checked = isSidebarVisible;
            }
            catch
            {
                navigationBarToolStripMenuItem.Checked = mainToolStrip.Visible;
                sidebarToolStripMenuItem.Checked = !splitContainer1.Panel1Collapsed;
            }

            UpdateControlsAvailabilityForCurrentView(false);

            loadViewSettings();
            toolbarIconCheck();

            // Add sources thing in Sources
            addYourOwnLabel.Text = Resources.addYourOwn;
            addYourOwnLink.Text = Resources.addYourOwn2;
            addYourOwnLink.Links.Clear();
            addYourOwnLink.Links.Add(0, addYourOwnLink.Text.Length, "https://raw.githubusercontent.com/fraaaaa4/ecureuil-discovery-sources/main/README.md");
        }

        private void mainWindow_Resize(object sender, EventArgs e)
        {
            LayoutSourceBanner();
        }

        // dynamically positions and truncates sourcePanel labels
        // detail about a source when opened from the left treeView
        private void LayoutSourceBanner()
        {
            if (sourcePanel == null || sourcePanel.Width <= 0 || _currentSelectedSource == null) return;

            int totalWidth = sourcePanel.Width;

            int leftColX = 50;

            // Measure and layout right-aligned labels (Description & Date)
            string descText = !string.IsNullOrEmpty(_currentSelectedSource.description)
                ? _currentSelectedSource.description
                : Resources.appNoDescription;

            string creationDateStr = _currentSelectedSource.dateCreated.HasValue
                ? _currentSelectedSource.dateCreated.Value.ToString(Resources.dateFormat)
                : "-";
            string dateText = Resources.createdOn + creationDateStr;

            int rightColWidth = Math.Max(120, (int)(totalWidth * 0.45));
            int rightColX = totalWidth - rightColWidth - 10;

            sourceDescription.Left = rightColX;
            sourceDescription.Top = 8;
            OpenImageHelper.SetTruncatedText(sourceDescription, descText, rightColWidth);

            sourceCreation.Left = rightColX;
            sourceCreation.Top = 29;
            OpenImageHelper.SetTruncatedText(sourceCreation, dateText, rightColWidth);

            // left side labels (Name & Author) occupy available space between icon and right labels
            int leftAvailableWidth = Math.Max(60, rightColX - leftColX - 10);

            string nameText = !string.IsNullOrEmpty(_currentSelectedSource.name)
                ? _currentSelectedSource.name
                : (!string.IsNullOrEmpty(_currentSelectedSource.id) ? _currentSelectedSource.id : Resources.source);
            sourceName.Left = leftColX;
            sourceName.Top = 7;
            OpenImageHelper.SetTruncatedText(sourceName, nameText, leftAvailableWidth);

            string authorText = !string.IsNullOrEmpty(_currentSelectedSource.author)
                ? _currentSelectedSource.author
                : Resources.appSourceUnknown;
            sourceAuthor.Left = leftColX;
            sourceAuthor.Top = 29;
            OpenImageHelper.SetTruncatedText(sourceAuthor, authorText, leftAvailableWidth);
        }

        // Download/cache an image and return a scaled bitmap
        private Image LoadScaledIcon(string iconUrl, string cacheKey, int width, int height)
        {
            if (string.IsNullOrEmpty(iconUrl)) return null;
            try
            {
                byte[] iconData = (_cacheManager != null && !string.IsNullOrEmpty(cacheKey)) ? _cacheManager.LoadImageCache(cacheKey) : null;
                Image result = null;

                if (iconData != null && iconData.Length > 0)
                {
                    try
                    {
                        using (MemoryStream ms = new MemoryStream(iconData))
                        using (Image img = Image.FromStream(ms))
                        {
                            result = OpenImageHelper.CreateScaledBitmap(img, width, height);
                        }
                    }
                    catch
                    {
                        // cache was corrupted or not a valid image, purge it
                        if (_cacheManager != null && !string.IsNullOrEmpty(cacheKey))
                        {
                            _cacheManager.DeleteImageCache(cacheKey);
                        }
                        iconData = null;
                        result = null;
                    }
                }

                if (result == null)
                {
                    // download it instead of trying to get it from the cache
                    iconData = _downloadService.DownloadData(iconUrl);
                    if (iconData != null && iconData.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(iconData))
                        using (Image img = Image.FromStream(ms))
                        {
                            result = OpenImageHelper.CreateScaledBitmap(img, width, height);
                        }

                        if (_cacheManager != null && !string.IsNullOrEmpty(cacheKey))
                        {
                            _cacheManager.SaveImageCache(cacheKey, iconData);
                        }
                    }
                }

                return result;
            }
            catch { }
            return null;
        }

        // load, cache, and merge apps for a source
        private void LoadAndCacheSourceApps(SourceModel sm, bool forceDownload)
        {
            if (sm == null || string.IsNullOrEmpty(sm.url)) return;
            try
            {
                string cacheKey = !string.IsNullOrEmpty(sm.id) ? sm.id : sm.name;
                string indexJson = null;

                if (!forceDownload && _cacheManager != null && _cacheManager.isSourceCacheValid(cacheKey, TimeSpan.FromHours(24)))
                {
                    indexJson = _cacheManager.LoadSourceCatalogCache(cacheKey);
                }

                if (string.IsNullOrEmpty(indexJson))
                {
                    indexJson = _downloadService.DownloadString(sm.url);
                    if (!string.IsNullOrEmpty(indexJson) && _cacheManager != null)
                    {
                        _cacheManager.SaveSourceCatalogCache(cacheKey, indexJson);
                    }
                }

                if (string.IsNullOrEmpty(indexJson) && _cacheManager != null)
                {
                    indexJson = _cacheManager.LoadSourceCatalogCache(cacheKey);
                }

                if (!string.IsNullOrEmpty(indexJson))
                {
                    List<AppModel> apps = _catalogManager.ParseIndexJson(indexJson);
                    if (apps != null)
                    {
                        for (int a = 0; a < apps.Count; a++)
                        {
                            AppModel app = apps[a];
                            if (app == null) continue;
                            if (string.IsNullOrEmpty(app.source)) app.source = !string.IsNullOrEmpty(sm.id) ? sm.id : sm.name;
                            if (string.Compare(app.source, sm.id, true) != 0 && string.Compare(app.source, sm.name, true) != 0) continue;

                            bool appExists = false;
                            for (int ex = 0; ex < _loadedApps.Count; ex++)
                            {
                                if (string.Compare(_loadedApps[ex].id, app.id, true) == 0 && string.Compare(_loadedApps[ex].source, app.source, true) == 0)
                                {
                                    _loadedApps[ex] = app;
                                    appExists = true;
                                    break;
                                }
                            }
                            if (!appExists)
                            {
                                _loadedApps.Add(app);
                            }

                            if (_cacheManager != null && !string.IsNullOrEmpty(app.id))
                            {
                                string appJson = MiniJson.Serialize(app);
                                _cacheManager.SaveAppCache(app.id, appJson);
                            }
                        }
                    }
                }
            }
            catch { }
        }

        // loads sources.json catalog
        public void LoadSourcesJson(string sourcesUrl)
        {
            this.Cursor = Cursors.WaitCursor;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string jsonContent = _downloadService.DownloadString(sourcesUrl);
                    if (string.IsNullOrEmpty(jsonContent))
                    {
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                // failure
                                this.Cursor = Cursors.Default;
                                MessageBox.Show(this, Resources.downloadSourceError + sourcesUrl, Resources.downloadSourceErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }));
                        }
                        return;
                    }

                    List<SourceModel> sources = _catalogManager.ParseSourcesJson(jsonContent);
                    if (sources != null && sources.Count > 0)
                    {
                        if (_loadedSources == null) _loadedSources = new List<SourceModel>();
                        if (_loadedApps == null) _loadedApps = new List<AppModel>();

                        for (int i = 0; i < sources.Count; i++)
                        {
                            SourceModel sm = sources[i];
                            if (sm == null) continue;

                            bool found = false;
                            for (int j = 0; j < _loadedSources.Count; j++)
                            {
                                if (string.Compare(_loadedSources[j].id, sm.id, true) == 0)
                                {
                                    _loadedSources[j] = sm;
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                _loadedSources.Add(sm);
                            }

                            LoadAndCacheSourceApps(sm, true);
                        }

                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                if (_installationRegistry != null && _loadedApps != null)
                                {
                                    _installationRegistry.ApplyStatusToApps(_loadedApps);
                                }
                                SaveInstalledSourcesToSettings();
                                PopulateNavigationTree();
                                PopulateInstalledSourcesList();
                                DisplayApps(_loadedApps, _showGroupsInApplications);
                                this.Cursor = Cursors.Default;
                            }));
                        }
                    }
                    else
                    {
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                this.Cursor = Cursors.Default;
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.Cursor = Cursors.Default;
                            Console.WriteLine(Resources.error + ex.Message);
                        }));
                    }
                }
            });
        }

        // add source text; add source when clicking down the Enter key
        private void addSourceContextText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ProcessAddSourceUrl();
            }
        }

        // downloads and parses sources into the JSON from URL using OpenSSL
        private void LoadDiscoveredSources(string url)
        {
            this.Cursor = Cursors.WaitCursor;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string jsonContent = _downloadService.DownloadString(url);
                    if (string.IsNullOrEmpty(jsonContent))
                    {
                        if (this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                this.Cursor = Cursors.Default;
                            }));
                        }
                        return;
                    }
                    List<sourceDiscoverModel> newDiscover = _catalogManager.ParseDiscoveryJson(jsonContent);
                    if (newDiscover == null) newDiscover = new List<sourceDiscoverModel>();

                    Dictionary<string, List<SourceModel>> sourcesCache = new Dictionary<string, List<SourceModel>>(StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < newDiscover.Count; i++)
                    {
                        sourceDiscoverModel disc = newDiscover[i];
                        if (disc != null && !string.IsNullOrEmpty(disc.url))
                        {
                            try
                            {
                                List<SourceModel> parsed = null;
                                // if the source is already in the cache, then get it from the cache and not from the internet
                                if (sourcesCache.ContainsKey(disc.url))
                                {
                                    parsed = sourcesCache[disc.url];
                                }
                                else
                                {
                                    string sourceJson = _downloadService.DownloadString(disc.url);
                                    if (!string.IsNullOrEmpty(sourceJson))
                                    {
                                        parsed = _catalogManager.ParseSourcesJson(sourceJson);
                                        sourcesCache[disc.url] = parsed;
                                    }
                                }

                                if (parsed != null && parsed.Count > 0)
                                {
                                    SourceModel matched = null;
                                    for (int s = 0; s < parsed.Count; s++)
                                    {
                                        if (parsed[s] != null && string.Compare(parsed[s].id, disc.id, true) == 0)
                                        {
                                            matched = parsed[s];
                                            break;
                                        }
                                    }

                                    if (matched == null && parsed.Count == 1)
                                    {
                                        matched = parsed[0];
                                    }

                                    if (matched != null)
                                    {
                                        disc.source = matched;
                                        if (string.IsNullOrEmpty(disc.name)) disc.name = matched.name;
                                        disc.author = matched.author;
                                        disc.iconURL = matched.iconUrl;
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    _loadedDiscoveredSources = newDiscover;
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.Cursor = Cursors.Default;
                            DisplayDiscoveredSources(_loadedDiscoveredSources);
                        }));
                    }
                }
                catch
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.Cursor = Cursors.Default;
                        }));
                    }
                }
            });
        }

        // discovered sources in listView
        private void DisplayDiscoveredSources(List<sourceDiscoverModel> discovered)
        {
            _isViewingDiscovered = true;
            categoryItemView.BeginUpdate();
            categoryItemView.Items.Clear();
            categoryItemView.Groups.Clear();

            categoryItemView.Columns.Clear();
            categoryItemView.Columns.Add(Resources.name ?? "Name", 160, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.author ?? "Author", 120, HorizontalAlignment.Left);

            string query = GetCurrentSearchQuery();

            if (discovered != null && discovered.Count > 0)
            {
                for (int i = 0; i < discovered.Count; i++)
                {
                    sourceDiscoverModel disc = discovered[i];
                    if (disc == null) continue;

                    string displayName = !string.IsNullOrEmpty(disc.name) ? disc.name : disc.id;

                    if (!string.IsNullOrEmpty(query))
                    {
                        bool matches = false;
                        if (!string.IsNullOrEmpty(displayName) && displayName.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) matches = true;
                        if (!matches && !string.IsNullOrEmpty(disc.id) && disc.id.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) matches = true;
                        if (!matches && !string.IsNullOrEmpty(disc.author) && disc.author.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) matches = true;
                        if (!matches && !string.IsNullOrEmpty(disc.url) && disc.url.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) matches = true;
                        if (!matches) continue;
                    }

                    ListViewItem item = new ListViewItem(displayName);
                    item.SubItems.Add(disc.author ?? "");
                    item.Tag = disc;

                    if (!string.IsNullOrEmpty(disc.iconURL))
                    {
                        string iconKey = "disc_" + (disc.id ?? displayName);
                        if (_appImageList.Images.ContainsKey(iconKey))
                        {
                            item.ImageKey = iconKey;
                        }
                        else
                        {
                            LoadDiscoveryIconAsync(iconKey, disc.iconURL, item);
                        }
                    }
                    categoryItemView.Items.Add(item);
                }
            }
            categoryItemView.EndUpdate();
        }

        // loads icons of discovered services
        private void LoadDiscoveryIconAsync(string iconKey, string iconUrl, ListViewItem item)
        {
            if (string.IsNullOrEmpty(iconKey) || string.IsNullOrEmpty(iconUrl)) return;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    Image icon = LoadScaledIcon(iconUrl, iconKey, 32, 32);
                    if (icon != null && this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            if (_appImageList != null && !_appImageList.Images.ContainsKey(iconKey))
                            {
                                _appImageList.Images.Add(iconKey, icon);
                            }
                            if (item != null && categoryItemView != null && !this.IsDisposed)
                            {
                                item.ImageKey = iconKey;
                            }
                        }));
                    }
                }
                catch { }
            });
        }

        // downloads and parses the index JSON from the specified URL using OpenSSL
        private void LoadSourceIndex(string url)
        {
            this.Cursor = Cursors.WaitCursor;
            ThreadPool.QueueUserWorkItem(delegate {
            try
            {
                string jsonContent = _downloadService.DownloadString(url);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    MessageBox.Show(Resources.downloadSourceError + url,
                                    Resources.downloadSourceErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<AppModel> newApps = _catalogManager.ParseIndexJson(jsonContent);
                if (newApps == null)
                {
                    newApps = new List<AppModel>();
                }

                string discoveredSourceId = null;
                if (newApps.Count > 0)
                {
                    for (int a = 0; a < newApps.Count; a++)
                    {
                        if (!string.IsNullOrEmpty(newApps[a].source))
                        {
                            discoveredSourceId = newApps[a].source;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(discoveredSourceId))
                {
                    discoveredSourceId = Path.GetFileNameWithoutExtension(url);
                    if (string.IsNullOrEmpty(discoveredSourceId)) discoveredSourceId = Resources.source3;
                }

                if (_loadedSources == null) _loadedSources = new List<SourceModel>();
                SourceModel matchedSource = null;
                for (int s = 0; s < _loadedSources.Count; s++)
                {
                    if (MatchesSource(_loadedSources[s], url) || MatchesSource(_loadedSources[s], discoveredSourceId))
                    {
                        matchedSource = _loadedSources[s];
                        break;
                    }
                }

                if (matchedSource == null)
                {
                    matchedSource = new SourceModel(discoveredSourceId, discoveredSourceId,
                        url, "", true, DateTime.Now, Resources.unknownAuthor, Resources.source3 + " " + discoveredSourceId);
                    _loadedSources.Add(matchedSource);
                }

                matchedSource.url = url;
                matchedSource.lastUpdated = DateTime.Now;

                // merge apps into _loadedApps
                if (_loadedApps == null) _loadedApps = new List<AppModel>();
                for (int i = _loadedApps.Count - 1; i >= 0; i--)
                {
                    if (_loadedApps[i] != null &&
                        (string.Compare(_loadedApps[i].source, discoveredSourceId, true) == 0 ||
                         string.Compare(_loadedApps[i].source, matchedSource.id, true) == 0 ||
                         string.Compare(_loadedApps[i].source, matchedSource.name, true) == 0))
                    {
                        _loadedApps.RemoveAt(i);
                    }
                }
                for (int a = 0; a < newApps.Count; a++)
                {
                    if (string.IsNullOrEmpty(newApps[a].source))
                    {
                        newApps[a].source = matchedSource.name;
                    }
                    _loadedApps.Add(newApps[a]);

                    if (_cacheManager != null && !string.IsNullOrEmpty(newApps[a].id))
                    {
                        string appJson = MiniJson.Serialize(newApps[a]);
                        _cacheManager.SaveAppCache(newApps[a].id, appJson);
                    }
                }

                if (_installationRegistry != null && _loadedApps != null)
                {
                    _installationRegistry.ApplyStatusToApps(_loadedApps);
                }

                SaveInstalledSourcesToSettings();
                PopulateNavigationTree();
                PopulateInstalledSourcesList();
                DisplayApps(_loadedApps, _showGroupsInApplications);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.downloadSourceError2 + ex.ToString(),
                                Resources.downloadSourceErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            });
        }

        // verifies if a source is the same to another
        // it must be the same to upload them via the id, but maybe it doesn't always happen
        private bool MatchesSource(SourceModel sm, string key)
        {
            if (sm == null || string.IsNullOrEmpty(key)) return false;
            return string.Compare(sm.id, key, true) == 0 ||
                   string.Compare(sm.name, key, true) == 0 ||
                   string.Compare(sm.url, key, true) == 0 ||
                   string.Compare(sm.originalURL, key, true) == 0;
        }

        // helper to check if an app belongs to an enabled source
        private bool IsAppEnabled(AppModel app)
        {
            if (app == null) return false;
            if (string.IsNullOrEmpty(app.source)) return true;
            if (_loadedSources == null || _loadedSources.Count == 0) return true;

            for (int s = 0; s < _loadedSources.Count; s++)
            {
                if (MatchesSource(_loadedSources[s], app.source))
                {
                    return _loadedSources[s].isEnabled;
                }
            }
            return true;
        }

        // Populates the left navigation tree with root nodes, dynamic categories, and dynamic sources from index
        private void PopulateNavigationTree()
        {
            categoryNavigationView.BeginUpdate();
            categoryNavigationView.Nodes.Clear();

            if (_navHistory != null) _navHistory.Clear();
            _navHistoryIndex = -1;
            UpdateNavButtons();

            // 1. Root Node: Applications (All)
            TreeNode appsNode = new TreeNode(Resources.firstCategoryNode ?? "Applications");
            appsNode.Name = "all_apps";
            appsNode.Tag = "all_apps";

            Dictionary<string, int> categoryCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            if (_loadedApps != null)
            {
                for (int i = 0; i < _loadedApps.Count; i++)
                {
                    AppModel app = _loadedApps[i];
                    if (app == null || !IsAppEnabled(app)) continue;

                    if (app.category != null && app.category.Count > 0)
                    {
                        for (int c = 0; c < app.category.Count; c++)
                        {
                            string cat = app.category[c];
                            if (!string.IsNullOrEmpty(cat))
                            {
                                int currentCount;
                                categoryCounts.TryGetValue(cat, out currentCount);
                                categoryCounts[cat] = currentCount + 1;
                            }
                        }
                    }
                    else
                    {
                        string unk = Resources.other ?? "Other";
                        int currentCount;
                        categoryCounts.TryGetValue(unk, out currentCount);
                        categoryCounts[unk] = currentCount + 1;
                    }
                }
            }

            foreach (KeyValuePair<string, int> pair in categoryCounts)
            {
                TreeNode catNode = new TreeNode(pair.Key + " (" + pair.Value + ")");
                catNode.Tag = pair.Key;
                catNode.Name = "cat_" + pair.Key;
                appsNode.Nodes.Add(catNode);
            }

            // 2. Root Node: Sources (Populated dynamically from registered sources and loaded apps)
            TreeNode sourcesNode = new TreeNode(Resources.secondCategoryNode ?? "Sources");
            sourcesNode.Name = Resources.sourcesNode;
            sourcesNode.Tag = Resources.sourcesNode;

            HideSourcePanel();

            TreeNode discoverNode = new TreeNode(Resources.discoverSources ?? "Discover sources");
            discoverNode.Name = Resources.discoverNode;
            discoverNode.Tag = Resources.discoverNode;
            sourcesNode.Nodes.Add(discoverNode);

            Dictionary<string, bool> addedSourceKeys = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            if (_loadedSources != null)
            {
                for (int s = 0; s < _loadedSources.Count; s++)
                {
                    SourceModel sm = _loadedSources[s];
                    if (sm == null) continue;
                    string displayName = !string.IsNullOrEmpty(sm.name) ? sm.name : (!string.IsNullOrEmpty(sm.id) ? sm.id : "Source");
                    string key = !string.IsNullOrEmpty(sm.id) ? sm.id : displayName;
                    if (!addedSourceKeys.ContainsKey(key) && !addedSourceKeys.ContainsKey(displayName))
                    {
                        TreeNode srcNode = new TreeNode(displayName);
                        srcNode.Name = Resources.source2 + "_" + key;
                        srcNode.Tag = key;
                        sourcesNode.Nodes.Add(srcNode);
                        addedSourceKeys[key] = true;
                        addedSourceKeys[displayName] = true;
                    }
                }
            }

            if (_loadedApps != null)
            {
                for (int a = 0; a < _loadedApps.Count; a++)
                {
                    AppModel app = _loadedApps[a];
                    if (app == null || string.IsNullOrEmpty(app.source)) continue;
                    string sName = app.source;
                    if (!addedSourceKeys.ContainsKey(sName))
                    {
                        SourceModel sm = FindSourceModel(sName);
                        string displayName = (sm != null && !string.IsNullOrEmpty(sm.name)) ? sm.name : sName;
                        TreeNode srcNode = new TreeNode(displayName);
                        srcNode.Name = Resources.source2 + "_" + sName;
                        srcNode.Tag = sName;
                        sourcesNode.Nodes.Add(srcNode);
                        addedSourceKeys[sName] = true;
                        addedSourceKeys[displayName] = true;
                    }
                }
            }

            // 3. Root Node: Installed
            TreeNode installedNode = new TreeNode(Resources.thirdCategoryNode ?? "Installed components");
            installedNode.Name = Resources.installedNode;
            installedNode.Tag = Resources.installedNode;

            categoryNavigationView.Nodes.Add(appsNode);
            categoryNavigationView.Nodes.Add(sourcesNode);
            categoryNavigationView.Nodes.Add(installedNode);

            appsNode.Expand();
            sourcesNode.Expand();
            categoryNavigationView.EndUpdate();
        }

        // Renders the list of apps inside categoryItemView with optional category grouping, rich details and downloads icons
        private void DisplayApps(List<AppModel> apps, bool useCategoryGroups)
        {
            DisplayApps(apps, useCategoryGroups, true);
        }

        private void DisplayApps(List<AppModel> apps, bool useCategoryGroups, bool filterDisabledSources)
        {
            _isViewingDiscovered = false;
            _currentBaseApps = apps;
            _currentFilterDisabledSources = filterDisabledSources;
            ApplyCurrentViewFilterAndDisplay();
        }

        private void SetupAppsListViewColumns()
        {
            categoryItemView.Columns.Clear();
            string selectedOrder = cmbOrderBy.SelectedItem != null ? cmbOrderBy.SelectedItem.ToString() : "Name";
            string selectedGroup = cmbGroupBy.SelectedItem != null ? cmbGroupBy.SelectedItem.ToString() : "(None)";

            categoryItemView.Columns.Add(Resources.name ?? "Name", 140, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.version ?? "Version", 60, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.author ?? "Author", 90, HorizontalAlignment.Left);
            categoryItemView.Columns.Add(Resources.size ?? "Size", 70, HorizontalAlignment.Right);

            // in order to work, Resources.uploader must be the same as here
            if (string.Compare(selectedOrder, Resources.uploader, true) == 0 ||
                string.Compare(selectedGroup, Resources.uploader, true) == 0)
            {
                categoryItemView.Columns.Add(Resources.uploader ?? "Uploader", 90, HorizontalAlignment.Left);
            }
            if (string.Compare(selectedOrder, Resources.originalDate, true) == 0)
            {
                categoryItemView.Columns.Add(Resources.originalDate ?? "Original date", 85, HorizontalAlignment.Left);
            }
            if (string.Compare(selectedOrder, Resources.uploadedDate, true) == 0)
            {
                categoryItemView.Columns.Add(Resources.uploadedDate ?? "Date uploaded", 85, HorizontalAlignment.Left);
            }
            if (string.Compare(selectedGroup, Resources.architecture2, true) == 0)
            {
                categoryItemView.Columns.Add(Resources.architecture ?? Resources.architecture2 ?? "Architecture", 80, HorizontalAlignment.Left);
            }
            if (string.Compare(selectedGroup, Resources.category2, true) == 0)
            {
                categoryItemView.Columns.Add(Resources.category ?? Resources.category2 ?? "Category", 85, HorizontalAlignment.Left);
            }

            categoryItemView.Columns.Add(Resources.description ?? "Description", 180, HorizontalAlignment.Left);
        }

        private void ApplyCurrentViewFilterAndDisplay()
        {
            if (_isViewingDiscovered)
            {
                DisplayDiscoveredSources(_loadedDiscoveredSources);
                return;
            }

            List<AppModel> apps = _currentBaseApps != null ? _currentBaseApps : _loadedApps;
            if (apps == null) apps = new List<AppModel>();

            // filter out disabled sources if requested
            List<AppModel> visibleApps = new List<AppModel>();
            for (int i = 0; i < apps.Count; i++)
            {
                AppModel app = apps[i];
                if (app == null) continue;
                if (_currentFilterDisabledSources && !IsAppEnabled(app)) continue;
                visibleApps.Add(app);
            }

            // filter by search query if any
            string query = GetCurrentSearchQuery();
            if (!string.IsNullOrEmpty(query))
            {
                List<AppModel> searchFiltered = new List<AppModel>();
                for (int i = 0; i < visibleApps.Count; i++)
                {
                    AppModel app = visibleApps[i];
                    if (AppMatchesSearch(app, query))
                    {
                        searchFiltered.Add(app);
                    }
                }
                visibleApps = searchFiltered;
            }

            // sort by OrderBy ComboBox
            string selectedOrder = cmbOrderBy.SelectedItem != null ? cmbOrderBy.SelectedItem.ToString() : "Name";
            visibleApps.Sort(delegate(AppModel a, AppModel b)
            {
                if (a == null && b == null) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                if (string.Compare(selectedOrder, Resources.appDeveloper, true) == 0 ||
                    string.Compare(selectedOrder, Resources.author, true) == 0 ||
                    string.Compare(selectedOrder, "App developer", true) == 0 ||
                    string.Compare(selectedOrder, "Author", true) == 0)
                {
                    return string.Compare(a.author ?? "", b.author ?? "", true);
                }
                else if (string.Compare(selectedOrder, Resources.uploader, true) == 0 ||
                         string.Compare(selectedOrder, "Uploader", true) == 0)
                {
                    return string.Compare(a.uploader ?? "", b.uploader ?? "", true);
                }
                else if (string.Compare(selectedOrder, Resources.size, true) == 0 ||
                         string.Compare(selectedOrder, "Size", true) == 0)
                {
                    return a.size.CompareTo(b.size);
                }
                else if (string.Compare(selectedOrder, Resources.originalDate, true) == 0 ||
                         string.Compare(selectedOrder, "Original date", true) == 0)
                {
                    DateTime da = a.dateOriginal ?? DateTime.MinValue;
                    DateTime db = b.dateOriginal ?? DateTime.MinValue;
                    return da.CompareTo(db);
                }
                else if (string.Compare(selectedOrder, Resources.uploadedDate, true) == 0 ||
                         string.Compare(selectedOrder, "Date uploaded", true) == 0)
                {
                    DateTime ua = a.dateSource ?? DateTime.MinValue;
                    DateTime ub = b.dateSource ?? DateTime.MinValue;
                    return ua.CompareTo(ub);
                }
                else
                {
                    return string.Compare(a.name ?? a.id ?? "", b.name ?? b.id ?? "", true);
                }
            });

            // setup columns
            SetupAppsListViewColumns();

            // group by GroupBy ComboBox
            string selectedGroup = cmbGroupBy.SelectedItem != null ? cmbGroupBy.SelectedItem.ToString() : (Resources.none ?? "(None)");
            bool useGroups = !string.IsNullOrEmpty(selectedGroup) &&
                             string.Compare(selectedGroup, Resources.none, true) != 0 &&
                             string.Compare(selectedGroup, "(None)", true) != 0;

            categoryItemView.BeginUpdate();
            categoryItemView.Items.Clear();
            categoryItemView.Groups.Clear();
            categoryItemView.ShowGroups = useGroups;
            categoryItemView.TileSize = new Size(260, 52);

            Dictionary<string, ListViewGroup> groupMap = new Dictionary<string, ListViewGroup>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < visibleApps.Count; i++)
            {
                AppModel app = visibleApps[i];
                if (app == null) continue;

                string appDisplayName = app.name ?? app.id;
                ListViewItem item = new ListViewItem(appDisplayName);
                item.SubItems.Add(app.version ?? "");
                item.SubItems.Add(app.author ?? "");
                item.SubItems.Add(sizeFormat.FormattedSize(app.size));

                if (string.Compare(selectedOrder, Resources.uploader, true) == 0 ||
                    string.Compare(selectedGroup, Resources.uploader, true) == 0)
                {
                    item.SubItems.Add(app.uploader ?? "");
                }
                if (string.Compare(selectedOrder, Resources.originalDate, true) == 0)
                {
                    item.SubItems.Add(app.dateOriginal.HasValue ? app.dateOriginal.Value.ToString(Resources.dateFormat) : "-");
                }
                if (string.Compare(selectedOrder, Resources.uploadedDate, true) == 0)
                {
                    item.SubItems.Add(app.dateSource.HasValue ? app.dateSource.Value.ToString(Resources.dateFormat) : "-");
                }
                if (string.Compare(selectedGroup, Resources.architecture2, true) == 0)
                {
                    item.SubItems.Add(app.architecture ?? "");
                }
                if (string.Compare(selectedGroup, Resources.category2, true) == 0)
                {
                    string catStr = (app.category != null && app.category.Count > 0) ? string.Join(", ", app.category.ToArray()) : "";
                    item.SubItems.Add(catStr);
                }

                item.SubItems.Add(app.description ?? "");
                item.Tag = app;

                if (useGroups)
                {
                    string groupKey = GetGroupKey(app, selectedGroup);
                    ListViewGroup grp;
                    if (!groupMap.TryGetValue(groupKey, out grp))
                    {
                        grp = new ListViewGroup(groupKey, groupKey);
                        categoryItemView.Groups.Add(grp);
                        groupMap[groupKey] = grp;
                    }
                    item.Group = grp;
                }

                // load icon if available async
                if (!string.IsNullOrEmpty(app.iconUrl))
                {
                    if (_appImageList.Images.ContainsKey(app.id))
                    {
                        item.ImageKey = app.id;
                    }
                    else
                    {
                        LoadAppIconAsync(app.id, app.iconUrl, item);
                    }
                }

                if (_cacheManager != null && !string.IsNullOrEmpty(app.id))
                {
                    try
                    {
                        string appCache = _cacheManager.LoadAppCache(app.id);
                        if (string.IsNullOrEmpty(appCache))
                        {
                            string appJson = MiniJson.Serialize(app);
                            _cacheManager.SaveAppCache(app.id, appJson);
                        }
                    }
                    catch { }
                }

                categoryItemView.Items.Add(item);
            }

            categoryItemView.EndUpdate();
        }

        private void LoadAppIconAsync(string appId, string iconUrl, ListViewItem item)
        {
            if (string.IsNullOrEmpty(appId) || string.IsNullOrEmpty(iconUrl)) return;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    Image icon = LoadScaledIcon(iconUrl, Resources.app + "_" + appId, 32, 32);
                    if (icon != null && this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            if (_appImageList != null && !_appImageList.Images.ContainsKey(appId))
                            {
                                _appImageList.Images.Add(appId, icon);
                            }
                            if (item != null && categoryItemView != null && !this.IsDisposed)
                            {
                                item.ImageKey = appId;
                            }
                        }));
                    }
                }
                catch { }
            });
        }

        private string GetGroupKey(AppModel app, string groupBy)
        {
            if (app == null) return Resources.general ?? "General";

            if (string.Compare(groupBy, Resources.category2, true) == 0)
            {
                return (app.category != null && app.category.Count > 0 && !string.IsNullOrEmpty(app.category[0]))
                    ? app.category[0] : (Resources.other ?? "Other");
            }
            else if (string.Compare(groupBy, Resources.author, true) == 0)
            {
                return !string.IsNullOrEmpty(app.author) ? app.author : (Resources.unknownAuthor ?? "Unknown Author");
            }
            else if (string.Compare(groupBy, Resources.uploader, true) == 0)
            {
                return !string.IsNullOrEmpty(app.uploader) ? app.uploader : (Resources.unknownUploader ?? "Unknown Uploader");
            }
            else if (string.Compare(groupBy, Resources.immersiveTag, true) == 0)
            {
                return app.isImmersive ? (Resources.immersiveApp ?? "Immersive App") : (Resources.standardApp ?? "Standard App");
            }
            else if (string.Compare(groupBy, Resources.architecture2, true) == 0)
            {
                return !string.IsNullOrEmpty(app.architecture) ? app.architecture : (Resources.generic ?? "Generic");
            }
            else if (string.Compare(groupBy, Resources.portableTag, true) == 0)
            {
                return app.isPortable ? (Resources.portable ?? "Portable") : (Resources.installer ?? "Installer");
            }
            else
            {
                return Resources.general ?? "General";
            }
        }

        private string GetCurrentSearchQuery()
        {
            if (_isSearchPlaceholder) return "";
            string query = (txtSearch.Text ?? "").Trim();
            if (string.Compare(query, Resources.searchPlaceholder, true) == 0)
            {
                return "";
            }
            return query;
        }

        private bool AppMatchesSearch(AppModel app, string query)
        {
            if (app == null) return false;
            if (string.IsNullOrEmpty(query)) return true;

            if (!string.IsNullOrEmpty(app.name) && app.name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (!string.IsNullOrEmpty(app.id) && app.id.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (!string.IsNullOrEmpty(app.author) && app.author.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (!string.IsNullOrEmpty(app.uploader) && app.uploader.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (!string.IsNullOrEmpty(app.description) && app.description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            if (app.category != null)
            {
                for (int c = 0; c < app.category.Count; c++)
                {
                    if (!string.IsNullOrEmpty(app.category[c]) && app.category[c].IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // hides sourcePanel
        private void HideSourcePanel()
        {
            sourcePanel.Visible = false;
            _currentSelectedSource = null;
            sourceIcon.Image = null;
            discoverSources.Visible = false;
            sourcesPanel.Visible = false;
        }

        // populates source details banner and loads source icon
        private void sourceLoad(SourceModel source)
        {
            if (source == null) return;
            _currentSelectedSource = source;
            discoverSources.Visible = false;
            sourcesPanel.Visible = false;
            sourcePanel.Visible = true;
            sourcePanel.SendToBack();
            categoryItemView.BringToFront();

            Image cachedImg;
            if (!string.IsNullOrEmpty(source.iconUrl) && _sourceIconCache.TryGetValue(source.iconUrl, out cachedImg))
            {
                sourceIcon.Image = cachedImg;
            }
            else
            {
                sourceIcon.Image = null;
                if (!string.IsNullOrEmpty(source.iconUrl))
                {
                    string iconUrl = source.iconUrl;
                    string cacheKey = Resources.sourceMinus + "_" + (!string.IsNullOrEmpty(source.id) ? source.id : source.name);
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        Image bmp = LoadScaledIcon(iconUrl, cacheKey, 35, 35);
                        if (bmp != null && this.IsHandleCreated && !this.IsDisposed)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                if (_currentSelectedSource != null && _currentSelectedSource.iconUrl == iconUrl)
                                {
                                    _sourceIconCache[iconUrl] = bmp;
                                    sourceIcon.Image = bmp;
                                }
                            }));
                        }
                    });
                }
            }
            LayoutSourceBanner();
        }

        // in the sources tab, you shouldn't be able to search or order or group by
        private void UpdateControlsAvailabilityForCurrentView(bool isSourcesRoot)
        {
            lblOrderBy.Enabled = !isSourcesRoot;
            cmbOrderBy.Enabled = !isSourcesRoot;
            lblGroupBy.Enabled = !isSourcesRoot;
            cmbGroupBy.Enabled = !isSourcesRoot;
            txtSearch.Enabled = !isSourcesRoot;
            appViewToolStripMenuItem.Enabled = !isSourcesRoot;
        }

        // handles selection in categoryNavigationView
        private void categoryNavigationView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;

            if (!_isNavigatingHistory)
            {
                if (_navHistoryIndex >= 0 && _navHistoryIndex < _navHistory.Count - 1)
                {
                    _navHistory.RemoveRange(_navHistoryIndex + 1, _navHistory.Count - (_navHistoryIndex + 1));
                }
                _navHistory.Add(e.Node);
                _navHistoryIndex = _navHistory.Count - 1;
                UpdateNavButtons();
            }

            string nodeName = (e.Node.Name ?? "").Trim();
            string tagStr = (e.Node.Tag as string) ?? "";
            string nodeText = (e.Node.Text ?? "").Trim();

            // applications root (All apps)
            if (string.Compare(nodeName, Resources.all_apps, true) == 0 ||
                string.Compare(nodeName, Resources.applications, true) == 0 ||
                string.Compare(tagStr, Resources.all_apps, true) == 0 ||
                string.Compare(nodeText, Resources.ApplicationsText, true) == 0)
            {
                UpdateControlsAvailabilityForCurrentView(false);
                HideSourcePanel();
                categoryItemView.BringToFront();
                DisplayApps(_loadedApps, _showGroupsInApplications);
                return;
            }

            // discover sources sub-node
            if (string.Compare(nodeName, Resources.discoverNode, true) == 0 ||
                string.Compare(tagStr, Resources.discoverNode, true) == 0 ||
                string.Compare(nodeText, Resources.discoverSources, true) == 0)
            {
                UpdateControlsAvailabilityForCurrentView(false);
                HideSourcePanel();
                discoverSources.Visible = true;
                discoverSources.SendToBack();
                categoryItemView.BringToFront();
                string discoveryUrl = Resources.discoveryURL;
                LoadDiscoveredSources(discoveryUrl);
                return;
            }

            // sources root node
            if (string.Compare(nodeName, Resources.sourcesNode, true) == 0 ||
                string.Compare(tagStr, Resources.sourcesNode, true) == 0 ||
                string.Compare(nodeText, Resources.source4, true) == 0)
            {
                UpdateControlsAvailabilityForCurrentView(true);
                HideSourcePanel();
                sourcesPanel.Visible = true;
                sourcesPanel.Dock = DockStyle.Fill;
                sourcesPanel.BringToFront();
                PopulateInstalledSourcesList();
                DisplayApps(new List<AppModel>(), false);
                return;
            }

            // installed components root node
            if (string.Compare(nodeName, Resources.installedNode, true) == 0 ||
                string.Compare(tagStr, Resources.installedNode, true) == 0 ||
                string.Compare(nodeText, Resources.thirdCategoryNode, true) == 0)
            {
                UpdateControlsAvailabilityForCurrentView(false);
                HideSourcePanel();
                List<AppModel> installed = new List<AppModel>();
                if (_loadedApps != null)
                {
                    for (int i = 0; i < _loadedApps.Count; i++)
                    {
                        if (_loadedApps[i] != null && _loadedApps[i].isInstalled && IsAppEnabled(_loadedApps[i]))
                        {
                            installed.Add(_loadedApps[i]);
                        }
                    }
                }
                DisplayApps(installed, false);
                return;
            }

            // category filter sub-node under Applications
            if (nodeName.StartsWith(Resources.categoryNode + "_") || (!string.IsNullOrEmpty(tagStr) && e.Node.Parent != null && string.Compare(e.Node.Parent.Name, Resources.all_apps, true) == 0))
            {
                UpdateControlsAvailabilityForCurrentView(false);
                HideSourcePanel();
                string categoryToFilter = !string.IsNullOrEmpty(tagStr) ? tagStr : nodeName.Substring(4);
                List<AppModel> filtered = new List<AppModel>();
                if (_loadedApps != null)
                {
                    for (int i = 0; i < _loadedApps.Count; i++)
                    {
                        AppModel app = _loadedApps[i];
                        if (app != null && app.category != null && IsAppEnabled(app))
                        {
                            for (int c = 0; c < app.category.Count; c++)
                            {
                                if (string.Compare(app.category[c], categoryToFilter, true) == 0)
                                {
                                    filtered.Add(app);
                                    break;
                                }
                            }
                        }
                    }
                }
                DisplayApps(filtered, false);
                return;
            }

            // specific Source sub-node under Sources
            if (nodeName.StartsWith(Resources.source2 + "_") || (e.Node.Parent != null && string.Compare(e.Node.Parent.Name, Resources.sourcesNode, true) == 0))
            {
                UpdateControlsAvailabilityForCurrentView(false);
                string sourceId = !string.IsNullOrEmpty(tagStr) ? tagStr : (nodeName.StartsWith(Resources.source2 + "_") ? nodeName.Substring(7) : nodeText);

                HideSourcePanel();
                SourceModel foundSource = FindSourceModel(sourceId);
                sourceLoad(foundSource);

                List<AppModel> filtered = new List<AppModel>();
                if (_loadedApps != null)
                {
                    for (int i = 0; i < _loadedApps.Count; i++)
                    {
                        AppModel app = _loadedApps[i];
                        if (app != null &&
                            (string.Compare(app.source, sourceId, true) == 0 ||
                             (foundSource.id != null && string.Compare(app.source, foundSource.id, true) == 0) ||
                             (foundSource.name != null && string.Compare(app.source, foundSource.name, true) == 0)))
                        {
                            filtered.Add(app);
                        }
                    }
                }
                DisplayApps(filtered, true, false);
                discoverSources.Visible = false;
                sourcesPanel.Visible = false;
                sourcePanel.SendToBack();
                categoryItemView.BringToFront();
                return;
            }

            // default fallback: clear view
            UpdateControlsAvailabilityForCurrentView(false);
            HideSourcePanel();
            DisplayApps(new List<AppModel>(), false);
        }

        // manages back and forward buttons
        private void UpdateNavButtons()
        {
            btnBack.Enabled = _navHistoryIndex > 0;
            btnForward.Enabled = _navHistoryIndex >= 0 && _navHistoryIndex < _navHistory.Count - 1;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (_navHistoryIndex > 0)
            {
                _navHistoryIndex--;
                _isNavigatingHistory = true;
                categoryNavigationView.SelectedNode = _navHistory[_navHistoryIndex];
                _isNavigatingHistory = false;
                UpdateNavButtons();
            }
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (_navHistoryIndex >= 0 && _navHistoryIndex < _navHistory.Count - 1)
            {
                _navHistoryIndex++;
                _isNavigatingHistory = true;
                categoryNavigationView.SelectedNode = _navHistory[_navHistoryIndex];
                _isNavigatingHistory = false;
                UpdateNavButtons();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshCurrentView();
        }

        private void cmbOrderBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentViewFilterAndDisplay();
        }

        private void cmbGroupBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyCurrentViewFilterAndDisplay();
        }

        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (_isSearchPlaceholder || string.Compare(txtSearch.Text, Resources.searchPlaceholder, true) == 0 || string.Compare(txtSearch.Text, Resources.searchPlaceholder, true) == 0)
            {
                _isSearchPlaceholder = false;
                txtSearch.Text = "";
                txtSearch.ForeColor = SystemColors.WindowText;
            }
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty((txtSearch.Text ?? "").Trim()))
            {
                _isSearchPlaceholder = true;
                txtSearch.Text = Resources.searchPlaceholder ?? "Search...";
                txtSearch.ForeColor = SystemColors.GrayText;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!_isSearchPlaceholder && string.Compare(txtSearch.Text, Resources.searchPlaceholder, true) != 0 && string.Compare(txtSearch.Text, "Search...", true) != 0)
            {
                ApplyCurrentViewFilterAndDisplay();
            }
            else if (string.IsNullOrEmpty(txtSearch.Text))
            {
                ApplyCurrentViewFilterAndDisplay();
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sourcesUrl = Resources.discoveryURL;
            LoadSourcesJson(sourcesUrl);
        }

        private void categoryItemView_DoubleClick(object sender, EventArgs e)
        {
            if (categoryItemView.SelectedItems.Count > 0)
            {
                ListViewItem item = categoryItemView.SelectedItems[0];
                sourceDiscoverModel disc = item.Tag as sourceDiscoverModel;
                if (disc != null)
                {
                    SourceModel sm = disc.source;
                    if (sm == null) sm = new SourceModel(disc.id, disc.name ?? disc.id, disc.author, disc.url, disc.iconURL);

                    Image icon = null;
                    string iconKey = Resources.disc + "_" + (disc.id ?? (!string.IsNullOrEmpty(disc.name) ? disc.name : ""));
                    if (!string.IsNullOrEmpty(iconKey) && _appImageList.Images.ContainsKey(iconKey))
                     icon = _appImageList.Images[iconKey];

                    bool isAdded = false;
                    if (_loadedSources != null)
                    {
                        for (int s = 0; s < _loadedSources.Count; s++)
                        {
                            if (_loadedSources[s] != null &&
                                (string.Compare(_loadedSources[s].id, sm.id, true) == 0 ||
                                 string.Compare(_loadedSources[s].name, sm.name, true) == 0))
                            {
                                isAdded = true;
                                break;
                            }
                        }
                    }

                    SourcesDetails sd = new SourcesDetails(sm, icon, _loadedApps, _downloadService, isAdded, _cacheManager);
                    EventHandler onSrcRefreshed = delegate
                    {
                        PopulateNavigationTree();
                        RefreshCurrentView();
                    };
                    sd.SourceRefreshed += onSrcRefreshed;
                    sd.FormClosed += delegate
                    {
                        sd.SourceRefreshed -= onSrcRefreshed;
                        sd.Dispose();
                    };
                    sd.Show();
                    return;
                }

                AppModel selectedApp = item.Tag as AppModel;
                if (selectedApp != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    Image cachedIcon = null;
                    if (!string.IsNullOrEmpty(selectedApp.id) && _appImageList.Images.ContainsKey(selectedApp.id))
                    {
                        cachedIcon = _appImageList.Images[selectedApp.id];
                    }

                    ApplicationDetail appDetail = new ApplicationDetail(selectedApp, _downloadService, _loadedApps, cachedIcon, _cacheManager, _loadedSources);
                    appDetail.Shown += delegate
                    {
                        this.Cursor = Cursors.Default;
                    };
                    appDetail.Show();
                }
            }
        }

        private void appItemContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (categoryItemView.SelectedItems.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            ListViewItem lvi = categoryItemView.SelectedItems[0];
            AppModel app = lvi.Tag as AppModel;
            if (app == null)
            {
                e.Cancel = true;
                return;
            }

            bool isInstalled = app.isInstalled;
            bool isPortableOrZip = (app != null && string.Compare((app.fileType ?? "").TrimStart('.'), Resources.msi, StringComparison.OrdinalIgnoreCase) != 0 && (app.isPortable || string.Compare((app.fileType ?? "").TrimStart('.'), Resources.zip, StringComparison.OrdinalIgnoreCase) == 0));

            _appOpenMenuItem.Enabled = isInstalled && isPortableOrZip && (!string.IsNullOrEmpty(app.installedShortcutPath) || !string.IsNullOrEmpty(app.installDirectory));
            _appUninstallMenuItem.Enabled = isInstalled;
            _appUninstallMenuItem.Text = isPortableOrZip ? Resources.uninstallString : Resources.deleteRemove;
            _appDetailsMenuItem.Enabled = true;
        }

        private void appOpenMenuItem_Click(object sender, EventArgs e)
        {
            if (categoryItemView.SelectedItems.Count > 0)
            {
                ListViewItem item = categoryItemView.SelectedItems[0];
                AppModel app = item.Tag as AppModel;
                if (app != null)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(app.installedShortcutPath) && File.Exists(app.installedShortcutPath))
                        {
                            Process.Start(app.installedShortcutPath);
                        }
                        else if (!string.IsNullOrEmpty(app.installDirectory) && Directory.Exists(app.installDirectory))
                        {
                            string targetExe = !string.IsNullOrEmpty(app.downloadPath) ? Path.Combine(app.installDirectory, app.downloadPath) : "";
                            if (!string.IsNullOrEmpty(targetExe) && File.Exists(targetExe))
                            {
                                Process.Start(targetExe);
                            }
                            else
                            {
                                string[] exes = Directory.GetFiles(app.installDirectory, "*." + Resources.exe, SearchOption.AllDirectories);
                                if (exes != null && exes.Length > 0)
                                {
                                    Process.Start(exes[0]);
                                }
                                else
                                {
                                    Process.Start(app.installDirectory);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, (Resources.error2 ?? "Error") + ": " + ex.Message, Resources.error2 ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void appUninstallMenuItem_Click(object sender, EventArgs e)
        {
            if (categoryItemView.SelectedItems.Count > 0)
            {
                ListViewItem item = categoryItemView.SelectedItems[0];
                AppModel app = item.Tag as AppModel;
                if (app != null && app.isInstalled)
                {
                    bool isPortableOrZip = (app != null && string.Compare((app.fileType ?? "").TrimStart('.'), "msi", StringComparison.OrdinalIgnoreCase) != 0 && (app.isPortable || string.Compare((app.fileType ?? "").TrimStart('.'), "zip", StringComparison.OrdinalIgnoreCase) == 0));
                    if (!isPortableOrZip)
                    {
                        // System installer (MSI or setup EXE)
                        if (_installationRegistry != null)
                        {
                            _installationRegistry.UnregisterApp(app.id);
                        }
                        app.isInstalled = false;
                        app.installedOn = null;
                        app.installedShortcutPath = null;
                        app.installDirectory = null;

                        InstallerService.OpenAddRemovePrograms();
                        MessageBox.Show(this, Resources.uninstallWarning, Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshCurrentView();
                    }
                    else
                    {
                        // Portable / ZIP
                        DialogResult dr = MessageBox.Show(this, Resources.uninstallQuestion + " " + (app.name ?? app.id) + "?", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (dr == DialogResult.Yes)
                        {
                            if (_installerService == null)
                            {
                                _installerService = new InstallerService(_installationRegistry ?? new InstallationRegistry());
                            }
                            bool ok = _installerService.UninstallApp(app);
                            if (ok)
                            {
                                MessageBox.Show(this, (app.name ?? app.id) + " " + Resources.uninstallSuccess + ".", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                RefreshCurrentView();
                            }
                            else
                            {
                                MessageBox.Show(this, Resources.uninstallFailure + " " + (app.name ?? app.id), Resources.error ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        private void appDetailsMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView_DoubleClick(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadViewSettings()
        {
            try
            {
                string savedView = Settings.Default.appListView;
                if (!string.IsNullOrEmpty(savedView))
                {
                    switch (savedView.ToLower())
                    {
                        case "largeicon":
                        case "largeicons":
                            categoryItemView.View = View.LargeIcon;
                            break;
                        case "smallicon":
                        case "smallicons":
                            categoryItemView.View = View.SmallIcon;
                            break;
                        case "list":
                            categoryItemView.View = View.List;
                            break;
                        case "tile":
                            categoryItemView.View = View.Tile;
                            break;
                        default:
                            categoryItemView.View = View.Details;
                            break;
                    }
                }
            }
            catch { }
            checkListView();
        }

        private void saveViewSettings()
        {
            try
            {
                RefreshCurrentView();
                Settings.Default.appListView = categoryItemView.View.ToString();
                Settings.Default.Save();
            }
            catch { }
        }

        private void checkListView()
        {
            largeIconsToolStripMenuItem.Checked = (categoryItemView.View == View.LargeIcon);
            smallIconsToolStripMenuItem.Checked = (categoryItemView.View == View.SmallIcon);
            detailsToolStripMenuItem.Checked = (categoryItemView.View == View.Details);
            listToolStripMenuItem.Checked = (categoryItemView.View == View.List);
            tileToolStripMenuItem.Checked = (categoryItemView.View == View.Tile);
            saveViewSettings();
        }

        private void RefreshCurrentView()
        {
            if (categoryNavigationView.SelectedNode != null)
            {
                categoryNavigationView_AfterSelect(categoryNavigationView, new TreeViewEventArgs(categoryNavigationView.SelectedNode));
            }
            else
            {
                DisplayApps(_loadedApps, _showGroupsInApplications);
            }
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView.View = View.LargeIcon;
            checkListView();
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView.View = View.SmallIcon;
            checkListView();
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView.View = View.Details;
            checkListView();
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView.View = View.List;
            checkListView();
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            categoryItemView.View = View.Tile;
            checkListView();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (refreshContextMenu.Visible == false)
                refreshContextMenu.Show(btn, new Point(0, btn.Height));
            else
                refreshContextMenu.Hide();
        }

        private void discoverSourcesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (categoryNavigationView.Nodes.Find(Resources.discoverNode, true).Length > 0)
            {
                categoryNavigationView.SelectedNode = categoryNavigationView.Nodes.Find(Resources.discoverNode, true)[0];
                categoryNavigationView.Focus();
            }
        }

        private void PopulateInstalledSourcesList()
        {
            installedSources.BeginUpdate();
            installedSources.Items.Clear();

            Dictionary<string, bool> addedNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            if (_loadedSources != null)
            {
                for (int s = 0; s < _loadedSources.Count; s++)
                {
                    SourceModel sm = _loadedSources[s];
                    if (sm == null) continue;
                    string name = !string.IsNullOrEmpty(sm.name) ? sm.name : sm.id;
                    if (!string.IsNullOrEmpty(name) && !addedNames.ContainsKey(name))
                    {
                        installedSources.Items.Add(name);
                        addedNames[name] = true;
                    }
                }
            }

            if (_loadedApps != null)
            {
                for (int a = 0; a < _loadedApps.Count; a++)
                {
                    AppModel app = _loadedApps[a];
                    if (app == null || string.IsNullOrEmpty(app.source)) continue;
                    if (!addedNames.ContainsKey(app.source))
                    {
                        SourceModel sm = FindSourceModel(app.source);
                        string name = (sm != null && !string.IsNullOrEmpty(sm.name)) ? sm.name : app.source;
                        if (!addedNames.ContainsKey(name))
                        {
                            installedSources.Items.Add(name);
                            addedNames[name] = true;
                            addedNames[app.source] = true;
                        }
                    }
                }
            }

            installedSources.EndUpdate();
            bool hasSelection = installedSources.SelectedItem != null;
            deleteSource.Enabled = hasSelection;
            propertiesSources.Enabled = hasSelection;
            refreshSelected.Enabled = hasSelection;
        }

        private void propertiesSources_Click(object sender, EventArgs e)
        {
            OpenSelectedSourceDetails();
        }

        private void installedSources_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedSourceDetails();
        }

        private void OpenSelectedSourceDetails()
        {
            if (installedSources.SelectedItem == null) return;
            string selSourceName = installedSources.SelectedItem.ToString();
            SourceModel sm = FindSourceModel(selSourceName);
            OpenSourceDetailsDialog(sm, GetSourceIcon(sm));
        }

        private void OpenSourceDetailsDialog(SourceModel sm, Image icon)
        {
            if (sm == null) return;
            this.Cursor = Cursors.WaitCursor;
            SourcesDetails sd = new SourcesDetails(sm, icon, _loadedApps, _downloadService, true, _cacheManager);
            EventHandler onEnabledChanged = delegate
            {
                SaveInstalledSourcesToSettings();
                PopulateNavigationTree();
                PopulateInstalledSourcesList();
                RefreshCurrentView();
            };

            EventHandler onSourceRefreshed = delegate
            {
                PopulateNavigationTree();
                RefreshCurrentView();
            };
            sd.SourceEnabledChanged += onEnabledChanged;
            sd.SourceRefreshed += onSourceRefreshed;
            sd.Shown += delegate
            {
                this.Cursor = Cursors.Default;
            };

            sd.FormClosed += delegate
            {
                sd.SourceEnabledChanged -= onEnabledChanged;
                sd.SourceRefreshed -= onSourceRefreshed;
                sd.Dispose();
            };

            sd.Show();
        }

        private SourceModel FindSourceModel(string sourceNameOrId)
        {
            if (string.IsNullOrEmpty(sourceNameOrId)) return null;

            if (_loadedSources != null)
            {
                for (int i = 0; i < _loadedSources.Count; i++)
                {
                    SourceModel sm = _loadedSources[i];
                    if (MatchesSource(sm, sourceNameOrId))
                    {
                        return sm;
                    }
                }
            }

            SourceModel fallback = new SourceModel();
            fallback.id = sourceNameOrId;
            fallback.name = sourceNameOrId;
            fallback.author = Resources.unknownAuthor;
            fallback.description = Resources.source3 + " " + sourceNameOrId;
            fallback.isEnabled = true;
            fallback.dateCreated = DateTime.Now;
            fallback.lastUpdated = DateTime.Now;

            if (_loadedSources == null) _loadedSources = new List<SourceModel>();
            _loadedSources.Add(fallback);
            SaveInstalledSourcesToSettings();

            return fallback;
        }

        private Image GetSourceIcon(SourceModel sm)
        {
            if (sm == null || string.IsNullOrEmpty(sm.iconUrl)) return null;

            Image img;
            if (_sourceIconCache.TryGetValue(sm.iconUrl, out img))
            {
                return img;
            }

            string iconCacheKey = Resources.sourceMinus + "_" + (!string.IsNullOrEmpty(sm.id) ? sm.id : sm.name);
            Image bmp = LoadScaledIcon(sm.iconUrl, iconCacheKey, 35, 35);
            if (bmp != null)
            {
                _sourceIconCache[sm.iconUrl] = bmp;
                return bmp;
            }
            return null;
        }

        private void deleteSource_Click(object sender, EventArgs e)
        {
            if (installedSources.SelectedItem == null)
            {
                DialogResult allRes = MessageBox.Show(this, Resources.deleteAllSourcesConfirm, Resources.ecureuil, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (allRes == DialogResult.Yes)
                {
                    if (_loadedSources != null) _loadedSources.Clear();
                    if (_loadedApps != null) _loadedApps.Clear();
                    SaveInstalledSourcesToSettings();
                    PopulateNavigationTree();
                    PopulateInstalledSourcesList();
                    DisplayApps(_loadedApps, false);
                }
                return;
            }

            string sourceToDelete = installedSources.SelectedItem.ToString();
            string confirmMsg = string.Format(Resources.deleteSourceConfirm, sourceToDelete);
            DialogResult res = MessageBox.Show(this, confirmMsg, Resources.confirmDeleteTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                SourceModel matchedSource = FindSourceModel(sourceToDelete);
                string delId = (matchedSource != null && !string.IsNullOrEmpty(matchedSource.id)) ? matchedSource.id : sourceToDelete;
                string delName = (matchedSource != null && !string.IsNullOrEmpty(matchedSource.name)) ? matchedSource.name : sourceToDelete;

                if (_loadedSources != null)
                {
                    for (int i = _loadedSources.Count - 1; i >= 0; i--)
                    {
                        SourceModel sm = _loadedSources[i];
                        if (sm != null && (MatchesSource(sm, sourceToDelete) || MatchesSource(sm, delId) || MatchesSource(sm, delName)))
                        {
                            _loadedSources.RemoveAt(i);
                        }
                    }
                }

                if (_loadedApps != null)
                {
                    for (int a = _loadedApps.Count - 1; a >= 0; a--)
                    {
                        string src = _loadedApps[a] != null ? _loadedApps[a].source : null;
                        if (!string.IsNullOrEmpty(src) &&
                            (string.Compare(src, sourceToDelete, true) == 0 ||
                             string.Compare(src, delId, true) == 0 ||
                             string.Compare(src, delName, true) == 0))
                        {
                            _loadedApps.RemoveAt(a);
                        }
                    }
                }

                if (_cacheManager != null)
                {
                    _cacheManager.ClearSourceCache(delId);
                    _cacheManager.ClearSourceCache(delName);
                }

                SaveInstalledSourcesToSettings();
                PopulateNavigationTree();
                PopulateInstalledSourcesList();
                DisplayApps(_loadedApps, false);
            }
        }

        private void addSource_Click(object sender, EventArgs e)
        {
            ResetAddSourceTextIfEmpty();
            Button btn = (Button)sender;
            addContextMenu.Show(btn, new Point(0, btn.Height));
        }

        private void sidebarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool newSidebarState = splitContainer1.Panel1Collapsed;
            sidebarToolStripMenuItem.Checked = newSidebarState;
            splitContainer1.Panel1Collapsed = !newSidebarState;

            try
            {
                Settings.Default.showSidebar = newSidebarState;
                Settings.Default.Save();
            }
            catch { }
        }

        private void navigationBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool newToolbarState = !mainToolStrip.Visible;
            navigationBarToolStripMenuItem.Checked = newToolbarState;
            mainToolStrip.Visible = newToolbarState;

            try
            {
                Settings.Default.showToolbar = newToolbarState;
                Settings.Default.Save();
            }
            catch { }
        }

        private void aboutEcureuilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutWindow aboutWindow = new aboutWindow();
            aboutWindow.Location = this.Location;
            aboutWindow.ShowDialog(this);
        }

        private void sourceName_Click(object sender, EventArgs e)
        {
            if (_currentSelectedSource != null)
            {
                OpenSourceDetailsDialog(_currentSelectedSource, sourceIcon.Image);
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings SettingsDialog = new settings(_cacheManager, _loadedSources);
            SettingsDialog.ShowDialog();
        }

        private void installedSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool hasSelection = (installedSources.SelectedItem != null);
            deleteSource.Enabled = hasSelection;
            refreshSelected.Enabled = hasSelection;
            propertiesSources.Enabled = hasSelection;
        }

        private void addContextMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ResetAddSourceTextIfEmpty();
        }

        private void addContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ResetAddSourceTextIfEmpty();
        }

        private void ResetAddSourceTextIfEmpty()
        {
            string current = (addSourceContextText.Text ?? "").Trim();
            if (string.IsNullOrEmpty(current) || string.Compare(current, Resources.add + " " + Resources.addSourceURL, true) == 0 || string.Compare(current, Resources.addSourceURL, true) == 0)
            {
                addSourceContextText.Text = Resources.add + " " + Resources.addSourceURL ?? "Add source URL";
                addSourceChange = false;
            }
        }

        private void ClearAddSourceTextIfPlaceholder()
        {
            string current = (addSourceContextText.Text ?? "").Trim();
            if (string.Compare(current, Resources.add + " " + Resources.addSourceURL, true) == 0 || string.Compare(current, Resources.addSourceURL, true) == 0)
            {
                addSourceContextText.Text = "";
                addSourceChange = true;
            }
        }

        private void addSourceContextText_Enter(object sender, EventArgs e)
        {
            ClearAddSourceTextIfPlaceholder();
        }

        private void addSourceContextText_Click(object sender, EventArgs e)
        {
            ClearAddSourceTextIfPlaceholder();
        }

        private void addSourceContextText_Leave(object sender, EventArgs e)
        {
            ResetAddSourceTextIfEmpty();
        }

        private void addSourceContextText_TextChanged(object sender, EventArgs e)
        {
            string current = (addSourceContextText.Text ?? "").Trim();
            if (string.IsNullOrEmpty(current))
            {
                addSourceChange = false;
            }
        }

        private void ShowSourceUrlWarning(string message, string title)
        {
            if (_sourceUrlToolTip == null)
            {
                _sourceUrlToolTip = new ToolTip();
                _sourceUrlToolTip.IsBalloon = true;
                _sourceUrlToolTip.ToolTipIcon = ToolTipIcon.Warning;
            }
            _sourceUrlToolTip.ToolTipTitle = !string.IsNullOrEmpty(title) ? title : Resources.invalidUrlTitle;
            Control targetControl = addSourceContextText.TextBox != null ? (Control)addSourceContextText.TextBox : addSourceContextText.Control;
            if (targetControl != null && targetControl.Visible)
            {
                _sourceUrlToolTip.Show(message, targetControl, 0, targetControl.Height, 3500);
            }
        }

        private void ProcessAddSourceUrl()
        {
            string url = (addSourceContextText.Text ?? "").Trim();
            if (string.IsNullOrEmpty(url) || string.Compare(url, Resources.add + " " + Resources.addSourceURL, true) == 0 || string.Compare(url, Resources.addSourceURL, true) == 0)
            {
                ShowSourceUrlWarning(Resources.pleaseEnterSourceUrl, Resources.missingUrlTitle);
                return;
            }

            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                ShowSourceUrlWarning(Resources.urlMustStartWithHttp, Resources.invalidUrlTitle);
                return;
            }

            Uri parsedUri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out parsedUri) || (parsedUri.Scheme != Uri.UriSchemeHttp && parsedUri.Scheme != Uri.UriSchemeHttps))
            {
                ShowSourceUrlWarning(Resources.pleaseEnterValidUrl, Resources.invalidUrlTitle);
                return;
            }

            if (_sourceUrlToolTip != null)
            {
                Control targetControl = addSourceContextText.TextBox != null ? (Control)addSourceContextText.TextBox : addSourceContextText.Control;
                if (targetControl != null) _sourceUrlToolTip.Hide(targetControl);
            }

            addContextMenu.Close();
            addSourceContextText.Text = Resources.addSourceURL;
            addSourceChange = false;
            LoadSourcesJson(url);
        }

        private void addSourceContextText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)13)
            {
                e.Handled = true;
                ProcessAddSourceUrl();
            }
        }

        private void textOnlyMenuItem_Click(object sender, EventArgs e)
        {
            textOnlyToolbar();
        }

        private void iconsOnlyMenuItem_Click(object sender, EventArgs e)
        {
            iconOnlyToolbar();
        }

        private void textIconMenuItem_Click(object sender, EventArgs e)
        {
            textAndIconOnlyToolbar();
        }

        private void SetToolbarDisplayStyle(ToolStripItemDisplayStyle style, int settingValue)
        {
            for (int i = 0; i < mainToolStrip.Items.Count; i++)
            {
                mainToolStrip.Items[i].DisplayStyle = style;
            }
            textOnlyMenuItem.Checked = (settingValue == 0);
            iconsOnlyMenuItem.Checked = (settingValue == 1);
            textIconMenuItem.Checked = (settingValue == 2);
            Settings.Default.textOrIconForm1 = settingValue;
            Settings.Default.Save();
        }

        private void textOnlyToolbar()
        {
            SetToolbarDisplayStyle(ToolStripItemDisplayStyle.Text, 0);
        }

        private void iconOnlyToolbar()
        {
            SetToolbarDisplayStyle(ToolStripItemDisplayStyle.Image, 1);
        }

        private void textAndIconOnlyToolbar()
        {
            SetToolbarDisplayStyle(ToolStripItemDisplayStyle.ImageAndText, 2);
        }

        private void toolbarIconCheck()
        {
            switch (Settings.Default.textOrIconForm1)
            {
                case 0:
                    textOnlyToolbar();
                    break;
                case 1:
                    iconOnlyToolbar();
                    break;
                case 2:
                    textAndIconOnlyToolbar();
                    break;
            }
        }

        private void discoverLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            TreeNode[] nodes = categoryNavigationView.Nodes.Find(Resources.discoverNode, true);
            if (nodes != null && nodes.Length > 0)
            {
                categoryNavigationView.SelectedNode = nodes[0];
                categoryNavigationView.Focus();
            }
        }

        private void addYourOwnLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel lbl = sender as LinkLabel;
            string url = lbl != null ? lbl.Text : null;

            if (e.Link != null && e.Link.LinkData is string)
                url = (string)e.Link.LinkData;
            else if (lbl != null && lbl.Tag is string)
                url = (string)lbl.Tag;

            if (string.IsNullOrEmpty(url)) return;

            this.Cursor = Cursors.WaitCursor;
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string tempDir = Path.Combine(Path.GetTempPath(), Resources.ecureuil ?? "Ecureuil");
                    if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    if (string.IsNullOrEmpty(fileName)) fileName = Resources.readmeFile;
                    string localPath = Path.Combine(tempDir, fileName);

                    bool success = _downloadService.DownloadFile(url, localPath, null);
                    if (success && File.Exists(localPath))
                    {
                        Process.Start(localPath);
                    }
                }
                catch (Exception ex)
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            MessageBox.Show(ex.Message, Resources.error2 ?? "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                }
                finally
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            this.Cursor = Cursors.Default;
                        }));
                    }
                }
            });
        }
    }
}