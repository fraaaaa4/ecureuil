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
using Ecureuil.Core.Helpers;
using Ecureuil.Core.Models;
using Ecureuil.Core.Services;
using Ecureuil.Helpers;
using Ecureuil.Properties;

namespace Ecureuil
{
    public partial class SourcesDetails : Form
    {
        private SourceModel _model;
        private Image _icon;
        private List<AppModel> _allApps;
        private List<AppModel> _sourceApps;
        private DownloadService _downloadService;
        private CacheManager _cacheManager;
        private ImageList _appImageList;
        private Dictionary<string, Image> _iconCache;
        private bool _isAdded = true;

        public event EventHandler SourceRefreshed;

        public SourcesDetails(SourceModel model, Image icon)
            : this(model, icon, null, null, true, null)
        {
        }

        public SourcesDetails(SourceModel model, Image icon, List<AppModel> allApps, DownloadService downloadService)
            : this(model, icon, allApps, downloadService, true, null)
        {
        }

        public SourcesDetails(SourceModel model, Image icon, List<AppModel> allApps, DownloadService downloadService, bool isAdded)
            : this(model, icon, allApps, downloadService, isAdded, null)
        {
        }

        public SourcesDetails(SourceModel model, Image icon, List<AppModel> allApps, DownloadService downloadService, bool isAdded, CacheManager cacheManager)
        {
            InitializeComponent();
            _model = model;
            _icon = icon;
            _allApps = allApps != null ? allApps : new List<AppModel>();
            _downloadService = downloadService != null ? downloadService : new DownloadService();
            _cacheManager = cacheManager != null ? cacheManager : new CacheManager();
            _sourceApps = new List<AppModel>();
            _iconCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);
            _isAdded = isAdded;

            SetupImageList();
        }

        private void SetupImageList()
        {
            _appImageList = new ImageList();
            _appImageList.ImageSize = new Size(32, 32);
            _appImageList.ColorDepth = ColorDepth.Depth32Bit;

            sourcesAppList.SmallImageList = _appImageList;
            sourcesAppList.LargeImageList = _appImageList;

            appSameCategoryList.SmallImageList = _appImageList;
            appSameCategoryList.LargeImageList = _appImageList;
        }

        private void SourcesDetails_Load(object sender, EventArgs e)
        {
            if (_model == null) return;

            this.Text = (!string.IsNullOrEmpty(_model.name) ? _model.name : _model.id) + " - " + Resources.sourceDetails;
            createdDate.Text = Resources.createdOn;
            appUploaderText.Text = _model.dateCreated.HasValue ? _model.dateCreated.Value.ToString("dd/MM/yyyy") : "-";
            lastUpdated.Text = Resources.lastUpdated;
            lastUpdatedText.Text = _model.lastUpdated.HasValue ? _model.lastUpdated.Value.ToString("dd/MM/yyyy") : "-";
            if (!_model.isEnabled) EnableDisableButton.Image = Resources.enable;
            else EnableDisableButton.Image = Resources.disable;

            // Configure URL label with tooltip and link behavior
            ToolTip urlToolTip = new ToolTip();
            urlToolTip.AutoPopDelay = 10000;
            urlToolTip.InitialDelay = 200;
            urlToolTip.ReshowDelay = 100;

            if (!string.IsNullOrEmpty(_model.url))
            {
                URLText.Text = Resources.link + " 1";
                URLText.Cursor = Cursors.Hand;
                URLText.ForeColor = Color.Blue;
                URLText.Font = new Font(URLText.Font, FontStyle.Underline);
                urlToolTip.SetToolTip(URLText, _model.url);
                URLText.Click += delegate
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(_model.url);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    catch { }
                };
            }
            else
            {
                URLText.Text = "-";
                URLText.Cursor = Cursors.Default;
                URLText.ForeColor = SystemColors.ControlText;
                URLText.Font = new Font(URLText.Font, FontStyle.Regular);
            }

            // Configure Original URL label with tooltip and link behavior
            if (!string.IsNullOrEmpty(_model.originalURL))
            {
                originalURLText.Text = Resources.link + " 2";
                originalURLText.Cursor = Cursors.Hand;
                originalURLText.ForeColor = Color.Blue;
                originalURLText.Font = new Font(originalURLText.Font, FontStyle.Underline);
                urlToolTip.SetToolTip(originalURLText, _model.originalURL);
                originalURLText.Click += delegate
                {
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(_model.originalURL);
                        psi.UseShellExecute = true;
                        Process.Start(psi);
                    }
                    catch { }
                };
            }
            else
            {
                originalURLText.Text = "-";
                originalURLText.Cursor = Cursors.Default;
                originalURLText.ForeColor = SystemColors.ControlText;
                originalURLText.Font = new Font(originalURLText.Font, FontStyle.Regular);
            }

            // Filter apps belonging to this source first so metadata fallbacks work
            FilterSourceApps();

            if (_icon != null)
            {
                sourceIcon.Image = _icon;
            }
            else if (!string.IsNullOrEmpty(_model.iconUrl))
            {
                string iconUrl = _model.iconUrl;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        byte[] data = _downloadService.DownloadData(iconUrl);
                        if (data != null && data.Length > 0)
                        {
                            using (MemoryStream ms = new MemoryStream(data))
                            using (Image temp = Image.FromStream(ms))
                            {
                                Bitmap bmp = new Bitmap(35, 35);
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                                    g.DrawImage(temp, 0, 0, 35, 35);
                                }
                                if (this.IsHandleCreated && !this.IsDisposed)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        if (!this.IsDisposed) sourceIcon.Image = bmp;
                                    }));
                                }
                            }
                        }
                    }
                    catch { }
                });
            }
            else
            {
                sourceIcon.Image = null;
            }

            string sName = !string.IsNullOrEmpty(_model.name) ? _model.name : (!string.IsNullOrEmpty(_model.id) ? _model.id : Resources.source);
            string sAuthor = !string.IsNullOrEmpty(_model.author) ? _model.author : Resources.appSourceUnknown;

            this.Text = sName + " - " + Resources.sourceDetails;
            sourceName.Text = sName;
            sourceAuthor.Text = sAuthor;
            sourceDescription.Text = !string.IsNullOrEmpty(_model.description) ? _model.description : Resources.appNoDescription;
            sourceCreation.Text = Resources.createdOn + " " + (_model.dateCreated.HasValue ? _model.dateCreated.Value.ToString("dd/MM/yyyy") : "-");
            isLocal.Text = (_model.isLocal) ? Resources.localSource : Resources.remoteSource;
            UpdateEnabledLabel();

            appSameCategoryList.ShowItemToolTips = true;
            sourcesAppList.ShowItemToolTips = true;

            // Set up columns for ListView
            SetupAppsListViewColumns();
            SetupCreatorLinksListViewColumns();

            // Populate Creator Links Tab
            PopulateCreatorLinks();

            // Default selections for ComboBoxes
            if (orderByComboBox.Items.Count > 0) orderByComboBox.SelectedIndex = 0; // "Name"
            if (groupByComboBox.Items.Count > 0) groupByComboBox.SelectedIndex = 0; // "(None)"

            ApplyFilterAndDisplay();
        }

        private void SetupCreatorLinksListViewColumns()
        {
            appSameCategoryList.Columns.Clear();
            appSameCategoryList.Columns.Add(Resources.name ?? "Name", 120);
            appSameCategoryList.Columns.Add(Resources.link ?? "URL", 330);
        }

        private void SetupAppsListViewColumns()
        {
            sourcesAppList.Columns.Clear();

            string selectedOrder = orderByComboBox.SelectedItem != null ? orderByComboBox.SelectedItem.ToString() : "Name";
            string selectedGroup = groupByComboBox.SelectedItem != null ? groupByComboBox.SelectedItem.ToString() : "(None)";

            // Base standard columns
            sourcesAppList.Columns.Add(Resources.name ?? "Name", 130);
            sourcesAppList.Columns.Add(Resources.version ?? "Version", 60);
            sourcesAppList.Columns.Add(Resources.author ?? "Author", 90);
            sourcesAppList.Columns.Add(Resources.size ?? "Size", 70);

            // Dynamically add columns if relevant order/group is chosen
            if (selectedOrder == Resources.uploader || selectedGroup == "Uploader")
            {
                sourcesAppList.Columns.Add(Resources.uploader ?? "Uploader", 90);
            }
            if (selectedOrder == Resources.originalDate)
            {
                sourcesAppList.Columns.Add(Resources.originalDate, 85);
            }
            if (selectedOrder == Resources.uploadedDate)
            {
                sourcesAppList.Columns.Add(Resources.uploadedDate, 85);
            }
            if (selectedGroup == Resources.architecture2)
            {
                sourcesAppList.Columns.Add(Resources.architecture ?? Resources.architecture2, 80);
            }
            if (selectedGroup == Resources.category2)
            {
                sourcesAppList.Columns.Add(Resources.category ?? Resources.category2, 85);
            }

            sourcesAppList.Columns.Add(Resources.description ?? Resources.description, 180);
        }

        private void FilterSourceApps()
        {
            _sourceApps.Clear();
            if (_model == null) return;

            string srcId = _model.id ?? "";
            string srcName = _model.name ?? "";

            if (_allApps != null)
            {
                for (int i = 0; i < _allApps.Count; i++)
                {
                    AppModel app = _allApps[i];
                    if (app == null) continue;

                    if ((!string.IsNullOrEmpty(srcId) && string.Compare(app.source, srcId, true) == 0) ||
                        (!string.IsNullOrEmpty(srcName) && string.Compare(app.source, srcName, true) == 0))
                    {
                        _sourceApps.Add(app);
                    }
                }
            }

            // If this source's apps are not in _allApps, fetch them dynamically from the source URL
            if (_sourceApps.Count == 0 && !string.IsNullOrEmpty(_model.url) && _downloadService != null)
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        string indexJson = _downloadService.DownloadString(_model.url);
                        if (!string.IsNullOrEmpty(indexJson))
                        {
                            CatalogManager cm = new CatalogManager();
                            List<AppModel> fetchedApps = cm.ParseIndexJson(indexJson);
                            if (fetchedApps != null && fetchedApps.Count > 0)
                            {
                                List<AppModel> matchingApps = new List<AppModel>();
                                for (int a = 0; a < fetchedApps.Count; a++)
                                {
                                    AppModel app = fetchedApps[a];
                                    if (app == null) continue;

                                    if (string.IsNullOrEmpty(app.source))
                                    {
                                        app.source = !string.IsNullOrEmpty(srcName) ? srcName : srcId;
                                    }

                                    if ((!string.IsNullOrEmpty(srcId) && string.Compare(app.source, srcId, true) == 0) ||
                                        (!string.IsNullOrEmpty(srcName) && string.Compare(app.source, srcName, true) == 0))
                                    {
                                        matchingApps.Add(app);
                                    }
                                }

                                if (!this.IsDisposed && this.IsHandleCreated)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        _sourceApps.Clear();
                                        _sourceApps.AddRange(matchingApps);
                                        ApplyFilterAndDisplay();
                                    }));
                                }
                            }
                        }
                    }
                    catch { }
                });
            }
        }

        private void PopulateCreatorLinks()
        {
            appSameCategoryList.Items.Clear();

            string srcName = _model != null && !string.IsNullOrEmpty(_model.name) ? _model.name : "this";
            string cats = (_model != null && _model.categories != null && _model.categories.Count > 0)
                ? string.Join(", ", _model.categories.ToArray())
                : Resources.generalCategory;

            appSourceTabText.Text = Resources.source + " " + srcName + " | " + Resources.category + cats;
            appSimilarText.Text = Resources.linksName + srcName + ":";

            // If authorLink present in SourceModel, add as links in list
            int linkCount = 0;
            if (_model != null && _model.authorLink != null && _model.authorLink.Count > 0)
            {
                for (int i = 0; i < _model.authorLink.Count; i++)
                {
                    string link = _model.authorLink[i];
                    if (string.IsNullOrEmpty(link)) continue;

                    linkCount++;
                    ListViewItem item = new ListViewItem(Resources.link + " " + linkCount);
                    item.SubItems.Add(link);
                    item.ToolTipText = link;
                    item.Tag = link;
                    appSameCategoryList.Items.Add(item);
                }
            }
            else if (!string.IsNullOrEmpty(_model.url))
            {
                linkCount++;
                ListViewItem item = new ListViewItem(Resources.link + " " + linkCount);
                item.SubItems.Add(_model.url);
                item.ToolTipText = _model.url;
                item.Tag = _model.url;
                appSameCategoryList.Items.Add(item);
            }
        }

        private void ApplyFilterAndDisplay()
        {
            if (_sourceApps == null) return;

            // Re-setup columns according to current order and group selection
            SetupAppsListViewColumns();

            List<AppModel> displayList = new List<AppModel>(_sourceApps);

            // 1. Sort / Order By
            string selectedOrder = orderByComboBox.SelectedItem != null ? orderByComboBox.SelectedItem.ToString() : "Name";
            displayList.Sort(delegate(AppModel a, AppModel b)
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

            // 2. Render in sourcesAppList
            sourcesAppList.BeginUpdate();
            sourcesAppList.Items.Clear();
            sourcesAppList.Groups.Clear();

            string selectedGroup = groupByComboBox.SelectedItem != null ? groupByComboBox.SelectedItem.ToString() : Resources.none;
            bool useGroups = !string.IsNullOrEmpty(selectedGroup) && selectedGroup != Resources.none;
            sourcesAppList.ShowGroups = useGroups;

            Dictionary<string, ListViewGroup> groupMap = new Dictionary<string, ListViewGroup>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < displayList.Count; i++)
            {
                AppModel app = displayList[i];
                if (app == null) continue;

                ListViewItem item = new ListViewItem(app.name ?? app.id);
                item.SubItems.Add(app.version ?? "");
                item.SubItems.Add(app.author ?? "");
                item.SubItems.Add(sizeFormat.FormattedSize(app.size));

                if (selectedOrder == Resources.uploader || selectedGroup == Resources.uploader)
                {
                    item.SubItems.Add(app.uploader ?? "");
                }
                if (selectedOrder == Resources.originalDate)
                {
                    item.SubItems.Add(app.dateOriginal.HasValue ? app.dateOriginal.Value.ToString(Resources.dateFormat) : "-");
                }
                if (selectedOrder == Resources.uploadedDate)
                {
                    item.SubItems.Add(app.dateSource.HasValue ? app.dateSource.Value.ToString(Resources.dateFormat) : "-");
                }
                if (selectedGroup == Resources.architecture2)
                {
                    item.SubItems.Add(app.architecture ?? "");
                }
                if (selectedGroup == Resources.category2)
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
                        sourcesAppList.Groups.Add(grp);
                        groupMap[groupKey] = grp;
                    }
                    item.Group = grp;
                }

                // Async icon loading if not present
                if (!string.IsNullOrEmpty(app.id) && _appImageList.Images.ContainsKey(app.id))
                {
                    item.ImageKey = app.id;
                }
                else if (!string.IsNullOrEmpty(app.iconUrl))
                {
                    LoadAppIconAsync(app.id, app.iconUrl, item);
                }

                sourcesAppList.Items.Add(item);
            }

            sourcesAppList.EndUpdate();
        }

        private string GetGroupKey(AppModel app, string groupBy)
        {
            if (string.Compare(groupBy, Resources.category2, true) == 0 || string.Compare(groupBy, "Category", true) == 0)
            {
                return (app.category != null && app.category.Count > 0 && !string.IsNullOrEmpty(app.category[0]))
                    ? app.category[0] : (Resources.other ?? "Other");
            }
            else if (string.Compare(groupBy, Resources.author, true) == 0 || string.Compare(groupBy, "Author", true) == 0)
            {
                return !string.IsNullOrEmpty(app.author) ? app.author : (Resources.unknownAuthor ?? "Unknown Author");
            }
            else if (string.Compare(groupBy, Resources.uploader, true) == 0 || string.Compare(groupBy, "Uploader", true) == 0)
            {
                return !string.IsNullOrEmpty(app.uploader) ? app.uploader : (Resources.unknownUploader ?? "Unknown Uploader");
            }
            else if (string.Compare(groupBy, Resources.immersiveTag, true) == 0 || string.Compare(groupBy, "Immersive tag", true) == 0)
            {
                return app.isImmersive ? (Resources.immersiveApp ?? "Immersive App") : (Resources.standardApp ?? "Standard App");
            }
            else if (string.Compare(groupBy, Resources.architecture2, true) == 0 || string.Compare(groupBy, "Architecture", true) == 0)
            {
                return !string.IsNullOrEmpty(app.architecture) ? app.architecture : (Resources.generic ?? "Generic");
            }
            else if (string.Compare(groupBy, Resources.portableTag, true) == 0 || string.Compare(groupBy, "Portable tag", true) == 0)
            {
                return app.isPortable ? (Resources.portable ?? "Portable") : (Resources.installer ?? "Installer");
            }
            else
            {
                return Resources.general ?? "General";
            }
        }

        private void LoadAppIconAsync(string appId, string iconUrl, ListViewItem item)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    byte[] data = _downloadService.DownloadData(iconUrl);
                    if (data != null && data.Length > 0)
                    {
                        using (MemoryStream ms = new MemoryStream(data))
                        {
                            using (Image img = Image.FromStream(ms))
                            {
                                Bitmap bmp = OpenImageHelper.CreateScaledBitmap(img, 32, 32);

                                if (!this.IsDisposed && this.IsHandleCreated && bmp != null)
                                {
                                    this.BeginInvoke(new MethodInvoker(delegate
                                    {
                                        if (_appImageList != null && !_appImageList.Images.ContainsKey(appId))
                                        {
                                            _appImageList.Images.Add(appId, bmp);
                                        }
                                        if (item != null && sourcesAppList != null && !this.IsDisposed)
                                        {
                                            item.ImageKey = appId;
                                        }
                                    }));
                                }
                            }
                        }
                    }
                }
                catch { }
            });
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            try
            {
                // Clear and detach ListView items
                if (sourcesAppList != null)
                {
                    sourcesAppList.Items.Clear();
                    sourcesAppList.Groups.Clear();
                    sourcesAppList.SmallImageList = null;
                    sourcesAppList.LargeImageList = null;
                }
                if (appSameCategoryList != null)
                {
                    appSameCategoryList.Items.Clear();
                    appSameCategoryList.SmallImageList = null;
                    appSameCategoryList.LargeImageList = null;
                }

                // Dispose ImageList
                if (_appImageList != null)
                {
                    _appImageList.Images.Clear();
                    _appImageList.Dispose();
                    _appImageList = null;
                }

                // Clear picture box reference
                if (sourceIcon != null)
                {
                    sourceIcon.Image = null;
                }

                if (_sourceApps != null)
                {
                    _sourceApps.Clear();
                    _sourceApps = null;
                }
                _allApps = null;
                _model = null;

                // Explicit garbage collection on dialog close
                GC.Collect();
            }
            catch { }
        }

        private void orderByComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilterAndDisplay();
        }

        private void groupByComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilterAndDisplay();
        }

        private void largeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourcesAppList.View = View.LargeIcon;
        }

        private void smallIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourcesAppList.View = View.SmallIcon;
        }

        private void detailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourcesAppList.View = View.Details;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourcesAppList.View = View.List;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sourcesAppList.View = View.Tile;
        }

        private void sourcesAppList_DoubleClick(object sender, EventArgs e)
        {
            if (sourcesAppList.SelectedItems.Count > 0)
            {
                AppModel app = sourcesAppList.SelectedItems[0].Tag as AppModel;
                if (app != null)
                {
                    Image appIcon = null;
                    if (!string.IsNullOrEmpty(app.id) && _appImageList.Images.ContainsKey(app.id))
                    {
                        appIcon = _appImageList.Images[app.id];
                    }

                    List<SourceModel> srcList = null;
                    if (_model != null)
                    {
                        srcList = new List<SourceModel>();
                        srcList.Add(_model);
                    }
                    ApplicationDetail ad = new ApplicationDetail(app, _downloadService, _allApps, appIcon, _cacheManager, srcList);
                    ad.Show();
                }
            }
        }

        private void appSameCategoryList_DoubleClick(object sender, EventArgs e)
        {
            if (appSameCategoryList.SelectedItems.Count > 0)
            {
                string url = appSameCategoryList.SelectedItems[0].Tag as string;
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        Process.Start(url);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, Resources.linkError + " " + ex.Message, Resources.error2, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public event EventHandler SourceEnabledChanged;
        public event EventHandler SourceAddRequested;

        private void UpdateEnabledLabel()
        {
            if (!_isAdded)
            {
                isEnabled.Visible = false;
                EnableDisableButton.Visible = false;
                EnableDisableButton.Enabled = false;
                if (addSourceButton != null)
                {
                    addSourceButton.Visible = true;
                    addSourceButton.Enabled = true;
                }
                return;
            }

            if (addSourceButton != null)
            {
                addSourceButton.Visible = false;
                addSourceButton.Enabled = false;
            }

            isEnabled.Visible = true;
            EnableDisableButton.Visible = true;
            EnableDisableButton.Enabled = true;
            isEnabled.Text = (_model != null && _model.isEnabled) ? Resources.enabled : Resources.disabled;
            EnableDisableButton.Text = (_model != null && _model.isEnabled) ? Resources.disableSource : Resources.enableSource;
            if (_model != null)
            {
                EnableDisableButton.Image = _model.isEnabled ? Resources.disable : Resources.enable;
            }
        }

        private void addSourceButton_Click(object sender, EventArgs e)
        {
            if (_model == null) return;
            _isAdded = true;
            _model.isEnabled = true;
            UpdateEnabledLabel();

            if (SourceAddRequested != null)
            {
                SourceAddRequested(this, EventArgs.Empty);
            }
            else if (SourceRefreshed != null)
            {
                SourceRefreshed(this, EventArgs.Empty);
            }

            MessageBox.Show(this, Resources.sourceAdded ?? "Source added successfully.", Resources.ecureuil ?? "Ecureuil", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EnableDisableButton_Click(object sender, EventArgs e)
        {
            if (!_isAdded || _model == null) return;
            _model.isEnabled = !_model.isEnabled;
            UpdateEnabledLabel();

            if (SourceEnabledChanged != null)
            {
                SourceEnabledChanged(this, EventArgs.Empty);
            }

            MessageBox.Show(this, Resources.sourceIsNow + " " + (_model.isEnabled ? Resources.enabled2 : Resources.disabled2), Resources.ecureuil, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            if (_model == null || string.IsNullOrEmpty(_model.url)) return;

            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    string json = _downloadService.DownloadString(_model.url);
                    if (!string.IsNullOrEmpty(json))
                    {
                        CatalogManager cm = new CatalogManager();
                        List<AppModel> freshApps = cm.ParseIndexJson(json);

                        string sId = !string.IsNullOrEmpty(_model.id) ? _model.id : _model.name;

                        // Save updated catalog cache and individual app caches
                        if (_cacheManager != null)
                        {
                            if (!string.IsNullOrEmpty(sId))
                            {
                                _cacheManager.SaveSourceCatalogCache(sId, json);
                            }
                            if (freshApps != null)
                            {
                                for (int a = 0; a < freshApps.Count; a++)
                                {
                                    AppModel app = freshApps[a];
                                    if (app != null && !string.IsNullOrEmpty(app.id))
                                    {
                                        string appJson = MiniJson.Serialize(app);
                                        _cacheManager.SaveAppCache(app.id, appJson);
                                    }
                                }
                            }
                        }

                        if (!this.IsDisposed && this.IsHandleCreated)
                        {
                            this.BeginInvoke(new MethodInvoker(delegate
                            {
                                _model.lastUpdated = DateTime.Now;
                                lastUpdatedText.Text = _model.lastUpdated.Value.ToString(Resources.dateFormat);

                                if (freshApps != null && freshApps.Count > 0)
                                {
                                    // Remove old apps from this source and add updated ones
                                    string srcId = _model.id ?? "";
                                    string srcName = _model.name ?? "";

                                    for (int i = _allApps.Count - 1; i >= 0; i--)
                                    {
                                        if (string.Compare(_allApps[i].source, srcId, true) == 0 ||
                                            string.Compare(_allApps[i].source, srcName, true) == 0)
                                        {
                                            _allApps.RemoveAt(i);
                                        }
                                    }

                                    for (int j = 0; j < freshApps.Count; j++)
                                    {
                                        if (string.IsNullOrEmpty(freshApps[j].source))
                                        {
                                            freshApps[j].source = !string.IsNullOrEmpty(_model.name) ? _model.name : _model.id;
                                        }
                                        _allApps.Add(freshApps[j]);
                                    }

                                    FilterSourceApps();
                                    ApplyFilterAndDisplay();
                                }

                                if (SourceRefreshed != null)
                                {
                                    SourceRefreshed(this, EventArgs.Empty);
                                }

                                MessageBox.Show(this, Resources.sourceRefreshed, Resources.ecureuil, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }));
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate
                        {
                            MessageBox.Show(this, Resources.sourceRefreshFail + " " + ex.Message, Resources.error2, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }));
                    }
                }
            });
        }
    }
}