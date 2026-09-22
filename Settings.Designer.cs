namespace Ecureuil
{
    partial class settings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.cacheTab = new System.Windows.Forms.TabPage();
            this.sizeGroupBox = new System.Windows.Forms.GroupBox();
            this.enableCacheLabel = new System.Windows.Forms.Label();
            this.enableCacheCheckbox = new System.Windows.Forms.CheckBox();
            this.cacheInnerTabControl = new System.Windows.Forms.TabControl();
            this.sourcesTab = new System.Windows.Forms.TabPage();
            this.sourcesCacheListView = new System.Windows.Forms.ListView();
            this.colSourceHeaderName = new System.Windows.Forms.ColumnHeader();
            this.colSourceHeaderUpdated = new System.Windows.Forms.ColumnHeader();
            this.appsTab = new System.Windows.Forms.TabPage();
            this.appsCacheListView = new System.Windows.Forms.ListView();
            this.colAppHeaderName = new System.Windows.Forms.ColumnHeader();
            this.colAppHeaderUpdated = new System.Windows.Forms.ColumnHeader();
            this.refreshCacheButton = new System.Windows.Forms.Button();
            this.deleteCacheButton = new System.Windows.Forms.Button();
            this.selectedCacheSizeLabel = new System.Windows.Forms.Label();
            this.installTab = new System.Windows.Forms.TabPage();
            this.startGroup = new System.Windows.Forms.GroupBox();
            this.startMenuLabel = new System.Windows.Forms.Label();
            this.startMenuCheck = new System.Windows.Forms.CheckBox();
            this.installingGroup = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.installRadio2 = new System.Windows.Forms.RadioButton();
            this.installLabel3 = new System.Windows.Forms.Label();
            this.label1nstallLabel2 = new System.Windows.Forms.Label();
            this.installRadio1 = new System.Windows.Forms.RadioButton();
            this.installationLabel1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.fileTab = new System.Windows.Forms.TabPage();
            this.cacheFileGroupBox = new System.Windows.Forms.GroupBox();
            this.cacheFileLabel = new System.Windows.Forms.Label();
            this.cacheFileTextbox = new System.Windows.Forms.TextBox();
            this.browseCacheFile = new System.Windows.Forms.Button();
            this.exploreCacheFileButton = new System.Windows.Forms.Button();
            this.defaultCacheFile = new System.Windows.Forms.Button();
            this.installedFileGroupbox = new System.Windows.Forms.GroupBox();
            this.installedFileDefault = new System.Windows.Forms.Button();
            this.installedFileExplore = new System.Windows.Forms.Button();
            this.installedFileBrowse = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.installedFileLabel = new System.Windows.Forms.Label();
            this.screenshotFileGroupbox = new System.Windows.Forms.GroupBox();
            this.screenshotFileDefaultButton = new System.Windows.Forms.Button();
            this.screenshotExploreButton = new System.Windows.Forms.Button();
            this.screenshotFileBrowse = new System.Windows.Forms.Button();
            this.screenshotFileTextbox = new System.Windows.Forms.TextBox();
            this.screenshotFileLabel = new System.Windows.Forms.Label();
            this.desktopGroup = new System.Windows.Forms.GroupBox();
            this.desktopLabel = new System.Windows.Forms.Label();
            this.desktopCheckbox = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.cacheTab.SuspendLayout();
            this.sizeGroupBox.SuspendLayout();
            this.cacheInnerTabControl.SuspendLayout();
            this.sourcesTab.SuspendLayout();
            this.appsTab.SuspendLayout();
            this.installTab.SuspendLayout();
            this.startGroup.SuspendLayout();
            this.installingGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.fileTab.SuspendLayout();
            this.cacheFileGroupBox.SuspendLayout();
            this.installedFileGroupbox.SuspendLayout();
            this.screenshotFileGroupbox.SuspendLayout();
            this.desktopGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.cacheTab);
            this.tabControl1.Controls.Add(this.installTab);
            this.tabControl1.Controls.Add(this.fileTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(380, 381);
            this.tabControl1.TabIndex = 0;
            // 
            // cacheTab
            // 
            this.cacheTab.Controls.Add(this.sizeGroupBox);
            this.cacheTab.Location = new System.Drawing.Point(4, 22);
            this.cacheTab.Name = "cacheTab";
            this.cacheTab.Padding = new System.Windows.Forms.Padding(3);
            this.cacheTab.Size = new System.Drawing.Size(372, 355);
            this.cacheTab.TabIndex = 0;
            this.cacheTab.Text = "Cache";
            this.cacheTab.UseVisualStyleBackColor = true;
            // 
            // sizeGroupBox
            // 
            this.sizeGroupBox.Controls.Add(this.enableCacheLabel);
            this.sizeGroupBox.Controls.Add(this.enableCacheCheckbox);
            this.sizeGroupBox.Controls.Add(this.cacheInnerTabControl);
            this.sizeGroupBox.Controls.Add(this.refreshCacheButton);
            this.sizeGroupBox.Controls.Add(this.deleteCacheButton);
            this.sizeGroupBox.Controls.Add(this.selectedCacheSizeLabel);
            this.sizeGroupBox.Location = new System.Drawing.Point(8, 6);
            this.sizeGroupBox.Name = "sizeGroupBox";
            this.sizeGroupBox.Size = new System.Drawing.Size(356, 340);
            this.sizeGroupBox.TabIndex = 0;
            this.sizeGroupBox.TabStop = false;
            this.sizeGroupBox.Text = "Cache";
            // 
            // enableCacheLabel
            // 
            this.enableCacheLabel.Location = new System.Drawing.Point(10, 37);
            this.enableCacheLabel.Name = "enableCacheLabel";
            this.enableCacheLabel.Size = new System.Drawing.Size(336, 26);
            this.enableCacheLabel.TabIndex = 5;
            this.enableCacheLabel.Text = "If enabled, Ecureuil saves locally sources and app data you\'ve opened previously." +
                "";
            // 
            // enableCacheCheckbox
            // 
            this.enableCacheCheckbox.AutoSize = true;
            this.enableCacheCheckbox.Location = new System.Drawing.Point(14, 19);
            this.enableCacheCheckbox.Name = "enableCacheCheckbox";
            this.enableCacheCheckbox.Size = new System.Drawing.Size(92, 17);
            this.enableCacheCheckbox.TabIndex = 4;
            this.enableCacheCheckbox.Text = "Enable cache";
            this.enableCacheCheckbox.UseVisualStyleBackColor = true;
            this.enableCacheCheckbox.CheckedChanged += new System.EventHandler(this.enableCacheCheckbox_CheckedChanged);
            // 
            // cacheInnerTabControl
            // 
            this.cacheInnerTabControl.Controls.Add(this.sourcesTab);
            this.cacheInnerTabControl.Controls.Add(this.appsTab);
            this.cacheInnerTabControl.Location = new System.Drawing.Point(10, 72);
            this.cacheInnerTabControl.Name = "cacheInnerTabControl";
            this.cacheInnerTabControl.SelectedIndex = 0;
            this.cacheInnerTabControl.Size = new System.Drawing.Size(336, 223);
            this.cacheInnerTabControl.TabIndex = 0;
            // 
            // sourcesTab
            // 
            this.sourcesTab.Controls.Add(this.sourcesCacheListView);
            this.sourcesTab.Location = new System.Drawing.Point(4, 22);
            this.sourcesTab.Name = "sourcesTab";
            this.sourcesTab.Padding = new System.Windows.Forms.Padding(3);
            this.sourcesTab.Size = new System.Drawing.Size(328, 197);
            this.sourcesTab.TabIndex = 0;
            this.sourcesTab.Text = "Sources";
            this.sourcesTab.UseVisualStyleBackColor = true;
            // 
            // sourcesCacheListView
            // 
            this.sourcesCacheListView.CheckBoxes = true;
            this.sourcesCacheListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSourceHeaderName,
            this.colSourceHeaderUpdated});
            this.sourcesCacheListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcesCacheListView.FullRowSelect = true;
            this.sourcesCacheListView.Location = new System.Drawing.Point(3, 3);
            this.sourcesCacheListView.Name = "sourcesCacheListView";
            this.sourcesCacheListView.ShowItemToolTips = true;
            this.sourcesCacheListView.Size = new System.Drawing.Size(322, 191);
            this.sourcesCacheListView.TabIndex = 0;
            this.sourcesCacheListView.UseCompatibleStateImageBehavior = false;
            this.sourcesCacheListView.View = System.Windows.Forms.View.Details;
            // 
            // colSourceHeaderName
            // 
            this.colSourceHeaderName.Text = "Name";
            this.colSourceHeaderName.Width = 175;
            // 
            // colSourceHeaderUpdated
            // 
            this.colSourceHeaderUpdated.Text = "Last updated";
            this.colSourceHeaderUpdated.Width = 125;
            // 
            // appsTab
            // 
            this.appsTab.Controls.Add(this.appsCacheListView);
            this.appsTab.Location = new System.Drawing.Point(4, 22);
            this.appsTab.Name = "appsTab";
            this.appsTab.Padding = new System.Windows.Forms.Padding(3);
            this.appsTab.Size = new System.Drawing.Size(328, 197);
            this.appsTab.TabIndex = 1;
            this.appsTab.Text = "Apps";
            this.appsTab.UseVisualStyleBackColor = true;
            // 
            // appsCacheListView
            // 
            this.appsCacheListView.CheckBoxes = true;
            this.appsCacheListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colAppHeaderName,
            this.colAppHeaderUpdated});
            this.appsCacheListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appsCacheListView.FullRowSelect = true;
            this.appsCacheListView.Location = new System.Drawing.Point(3, 3);
            this.appsCacheListView.Name = "appsCacheListView";
            this.appsCacheListView.ShowItemToolTips = true;
            this.appsCacheListView.Size = new System.Drawing.Size(322, 191);
            this.appsCacheListView.TabIndex = 0;
            this.appsCacheListView.UseCompatibleStateImageBehavior = false;
            this.appsCacheListView.View = System.Windows.Forms.View.Details;
            // 
            // colAppHeaderName
            // 
            this.colAppHeaderName.Text = "Name";
            this.colAppHeaderName.Width = 175;
            // 
            // colAppHeaderUpdated
            // 
            this.colAppHeaderUpdated.Text = "Last updated";
            this.colAppHeaderUpdated.Width = 125;
            // 
            // refreshCacheButton
            // 
            this.refreshCacheButton.Location = new System.Drawing.Point(170, 303);
            this.refreshCacheButton.Name = "refreshCacheButton";
            this.refreshCacheButton.Size = new System.Drawing.Size(70, 25);
            this.refreshCacheButton.TabIndex = 3;
            this.refreshCacheButton.Text = "Refresh";
            this.refreshCacheButton.UseVisualStyleBackColor = true;
            this.refreshCacheButton.Click += new System.EventHandler(this.refreshCacheButton_Click);
            // 
            // deleteCacheButton
            // 
            this.deleteCacheButton.Location = new System.Drawing.Point(246, 303);
            this.deleteCacheButton.Name = "deleteCacheButton";
            this.deleteCacheButton.Size = new System.Drawing.Size(100, 25);
            this.deleteCacheButton.TabIndex = 2;
            this.deleteCacheButton.Text = "Delete selected";
            this.deleteCacheButton.UseVisualStyleBackColor = true;
            this.deleteCacheButton.Click += new System.EventHandler(this.deleteCacheButton_Click);
            // 
            // selectedCacheSizeLabel
            // 
            this.selectedCacheSizeLabel.AutoSize = true;
            this.selectedCacheSizeLabel.Location = new System.Drawing.Point(12, 309);
            this.selectedCacheSizeLabel.Name = "selectedCacheSizeLabel";
            this.selectedCacheSizeLabel.Size = new System.Drawing.Size(49, 13);
            this.selectedCacheSizeLabel.TabIndex = 1;
            this.selectedCacheSizeLabel.Text = "Size: 0 B";
            // 
            // installTab
            // 
            this.installTab.Controls.Add(this.desktopGroup);
            this.installTab.Controls.Add(this.startGroup);
            this.installTab.Controls.Add(this.installingGroup);
            this.installTab.Location = new System.Drawing.Point(4, 22);
            this.installTab.Name = "installTab";
            this.installTab.Padding = new System.Windows.Forms.Padding(3);
            this.installTab.Size = new System.Drawing.Size(372, 355);
            this.installTab.TabIndex = 1;
            this.installTab.Text = "Installation";
            this.installTab.UseVisualStyleBackColor = true;
            // 
            // startGroup
            // 
            this.startGroup.Controls.Add(this.startMenuLabel);
            this.startGroup.Controls.Add(this.startMenuCheck);
            this.startGroup.Location = new System.Drawing.Point(8, 185);
            this.startGroup.Name = "startGroup";
            this.startGroup.Size = new System.Drawing.Size(356, 79);
            this.startGroup.TabIndex = 12;
            this.startGroup.TabStop = false;
            this.startGroup.Text = "Start Menu";
            // 
            // startMenuLabel
            // 
            this.startMenuLabel.Location = new System.Drawing.Point(14, 38);
            this.startMenuLabel.Name = "startMenuLabel";
            this.startMenuLabel.Size = new System.Drawing.Size(327, 26);
            this.startMenuLabel.TabIndex = 6;
            this.startMenuLabel.Text = "If enabled, Ecureuil creates a link of your installed app directly in your Start " +
                "menu. Note that this works only with portable apps.";
            // 
            // startMenuCheck
            // 
            this.startMenuCheck.AutoSize = true;
            this.startMenuCheck.Location = new System.Drawing.Point(17, 19);
            this.startMenuCheck.Name = "startMenuCheck";
            this.startMenuCheck.Size = new System.Drawing.Size(226, 17);
            this.startMenuCheck.TabIndex = 0;
            this.startMenuCheck.Text = "Add to my Start menu a link to the program";
            this.startMenuCheck.UseVisualStyleBackColor = true;
            this.startMenuCheck.CheckedChanged += new System.EventHandler(this.startMenuCheck_CheckedChanged);
            // 
            // installingGroup
            // 
            this.installingGroup.Controls.Add(this.browseButton);
            this.installingGroup.Controls.Add(this.textBox1);
            this.installingGroup.Controls.Add(this.installRadio2);
            this.installingGroup.Controls.Add(this.installLabel3);
            this.installingGroup.Controls.Add(this.label1nstallLabel2);
            this.installingGroup.Controls.Add(this.installRadio1);
            this.installingGroup.Controls.Add(this.installationLabel1);
            this.installingGroup.Location = new System.Drawing.Point(8, 7);
            this.installingGroup.Name = "installingGroup";
            this.installingGroup.Size = new System.Drawing.Size(356, 170);
            this.installingGroup.TabIndex = 1;
            this.installingGroup.TabStop = false;
            this.installingGroup.Text = "Installation";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(266, 130);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(75, 23);
            this.browseButton.TabIndex = 11;
            this.browseButton.Text = "Browse";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(36, 132);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(224, 20);
            this.textBox1.TabIndex = 10;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // installRadio2
            // 
            this.installRadio2.AutoSize = true;
            this.installRadio2.Location = new System.Drawing.Point(17, 111);
            this.installRadio2.Name = "installRadio2";
            this.installRadio2.Size = new System.Drawing.Size(136, 17);
            this.installRadio2.TabIndex = 9;
            this.installRadio2.TabStop = true;
            this.installRadio2.Text = "Custom installation path";
            this.installRadio2.UseVisualStyleBackColor = true;
            this.installRadio2.CheckedChanged += new System.EventHandler(this.installRadio2_CheckedChanged);
            // 
            // installLabel3
            // 
            this.installLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.installLabel3.Location = new System.Drawing.Point(34, 84);
            this.installLabel3.Name = "installLabel3";
            this.installLabel3.Size = new System.Drawing.Size(304, 24);
            this.installLabel3.TabIndex = 8;
            this.installLabel3.Text = "Current default installation path:";
            // 
            // label1nstallLabel2
            // 
            this.label1nstallLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1nstallLabel2.Location = new System.Drawing.Point(34, 57);
            this.label1nstallLabel2.Name = "label1nstallLabel2";
            this.label1nstallLabel2.Size = new System.Drawing.Size(304, 24);
            this.label1nstallLabel2.TabIndex = 7;
            this.label1nstallLabel2.Text = "Installed files will be saved in an EcureuilApps folder, inside your Program File" +
                "s program.";
            // 
            // installRadio1
            // 
            this.installRadio1.AutoSize = true;
            this.installRadio1.Location = new System.Drawing.Point(17, 39);
            this.installRadio1.Name = "installRadio1";
            this.installRadio1.Size = new System.Drawing.Size(135, 17);
            this.installRadio1.TabIndex = 6;
            this.installRadio1.TabStop = true;
            this.installRadio1.Text = "Default installation path";
            this.installRadio1.UseVisualStyleBackColor = true;
            this.installRadio1.CheckedChanged += new System.EventHandler(this.installRadio1_CheckedChanged);
            // 
            // installationLabel1
            // 
            this.installationLabel1.Location = new System.Drawing.Point(14, 19);
            this.installationLabel1.Name = "installationLabel1";
            this.installationLabel1.Size = new System.Drawing.Size(324, 17);
            this.installationLabel1.TabIndex = 5;
            this.installationLabel1.Text = "Directory in which the installation files are saved:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.OKButton);
            this.panel1.Controls.Add(this.CancelButton);
            this.panel1.Controls.Add(this.ApplyButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 381);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(380, 39);
            this.panel1.TabIndex = 1;
            // 
            // OKButton
            // 
            this.OKButton.Location = new System.Drawing.Point(131, 8);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(212, 8);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ApplyButton
            // 
            this.ApplyButton.Location = new System.Drawing.Point(293, 8);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 23);
            this.ApplyButton.TabIndex = 0;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            this.ApplyButton.Click += new System.EventHandler(this.ApplyButton_Click);
            // 
            // fileTab
            // 
            this.fileTab.Controls.Add(this.screenshotFileGroupbox);
            this.fileTab.Controls.Add(this.installedFileGroupbox);
            this.fileTab.Controls.Add(this.cacheFileGroupBox);
            this.fileTab.Location = new System.Drawing.Point(4, 22);
            this.fileTab.Name = "fileTab";
            this.fileTab.Size = new System.Drawing.Size(372, 355);
            this.fileTab.TabIndex = 2;
            this.fileTab.Text = "Files and folders";
            this.fileTab.UseVisualStyleBackColor = true;
            // 
            // cacheFileGroupBox
            // 
            this.cacheFileGroupBox.Controls.Add(this.defaultCacheFile);
            this.cacheFileGroupBox.Controls.Add(this.exploreCacheFileButton);
            this.cacheFileGroupBox.Controls.Add(this.browseCacheFile);
            this.cacheFileGroupBox.Controls.Add(this.cacheFileTextbox);
            this.cacheFileGroupBox.Controls.Add(this.cacheFileLabel);
            this.cacheFileGroupBox.Location = new System.Drawing.Point(8, 7);
            this.cacheFileGroupBox.Name = "cacheFileGroupBox";
            this.cacheFileGroupBox.Size = new System.Drawing.Size(356, 110);
            this.cacheFileGroupBox.TabIndex = 0;
            this.cacheFileGroupBox.TabStop = false;
            this.cacheFileGroupBox.Text = "Cache";
            // 
            // cacheFileLabel
            // 
            this.cacheFileLabel.AutoSize = true;
            this.cacheFileLabel.Location = new System.Drawing.Point(14, 26);
            this.cacheFileLabel.Name = "cacheFileLabel";
            this.cacheFileLabel.Size = new System.Drawing.Size(223, 13);
            this.cacheFileLabel.TabIndex = 0;
            this.cacheFileLabel.Text = "Path of stored cache items (data and images):";
            // 
            // cacheFileTextbox
            // 
            this.cacheFileTextbox.Location = new System.Drawing.Point(17, 42);
            this.cacheFileTextbox.Name = "cacheFileTextbox";
            this.cacheFileTextbox.Size = new System.Drawing.Size(316, 20);
            this.cacheFileTextbox.TabIndex = 1;
            this.cacheFileTextbox.TextChanged += new System.EventHandler(this.cacheFileTextbox_TextChanged);
            // 
            // browseCacheFile
            // 
            this.browseCacheFile.Location = new System.Drawing.Point(258, 68);
            this.browseCacheFile.Name = "browseCacheFile";
            this.browseCacheFile.Size = new System.Drawing.Size(75, 23);
            this.browseCacheFile.TabIndex = 2;
            this.browseCacheFile.Text = "Browse";
            this.browseCacheFile.UseVisualStyleBackColor = true;
            this.browseCacheFile.Click += new System.EventHandler(this.browseCacheFile_Click);
            // 
            // exploreCacheFileButton
            // 
            this.exploreCacheFileButton.Location = new System.Drawing.Point(177, 68);
            this.exploreCacheFileButton.Name = "exploreCacheFileButton";
            this.exploreCacheFileButton.Size = new System.Drawing.Size(75, 23);
            this.exploreCacheFileButton.TabIndex = 3;
            this.exploreCacheFileButton.Text = "Explore";
            this.exploreCacheFileButton.UseVisualStyleBackColor = true;
            this.exploreCacheFileButton.Click += new System.EventHandler(this.exploreCacheFileButton_Click);
            // 
            // defaultCacheFile
            // 
            this.defaultCacheFile.Location = new System.Drawing.Point(17, 68);
            this.defaultCacheFile.Name = "defaultCacheFile";
            this.defaultCacheFile.Size = new System.Drawing.Size(117, 23);
            this.defaultCacheFile.TabIndex = 4;
            this.defaultCacheFile.Text = "Restore to default";
            this.defaultCacheFile.UseVisualStyleBackColor = true;
            this.defaultCacheFile.Click += new System.EventHandler(this.defaultCacheFile_Click);
            // 
            // installedFileGroupbox
            // 
            this.installedFileGroupbox.Controls.Add(this.installedFileDefault);
            this.installedFileGroupbox.Controls.Add(this.installedFileExplore);
            this.installedFileGroupbox.Controls.Add(this.installedFileBrowse);
            this.installedFileGroupbox.Controls.Add(this.textBox2);
            this.installedFileGroupbox.Controls.Add(this.installedFileLabel);
            this.installedFileGroupbox.Location = new System.Drawing.Point(8, 123);
            this.installedFileGroupbox.Name = "installedFileGroupbox";
            this.installedFileGroupbox.Size = new System.Drawing.Size(356, 110);
            this.installedFileGroupbox.TabIndex = 5;
            this.installedFileGroupbox.TabStop = false;
            this.installedFileGroupbox.Text = "Installed files";
            // 
            // installedFileDefault
            // 
            this.installedFileDefault.Location = new System.Drawing.Point(17, 68);
            this.installedFileDefault.Name = "installedFileDefault";
            this.installedFileDefault.Size = new System.Drawing.Size(117, 23);
            this.installedFileDefault.TabIndex = 4;
            this.installedFileDefault.Text = "Restore to default";
            this.installedFileDefault.UseVisualStyleBackColor = true;
            this.installedFileDefault.Click += new System.EventHandler(this.installedFileDefault_Click);
            // 
            // installedFileExplore
            // 
            this.installedFileExplore.Location = new System.Drawing.Point(177, 68);
            this.installedFileExplore.Name = "installedFileExplore";
            this.installedFileExplore.Size = new System.Drawing.Size(75, 23);
            this.installedFileExplore.TabIndex = 3;
            this.installedFileExplore.Text = "Explore";
            this.installedFileExplore.UseVisualStyleBackColor = true;
            this.installedFileExplore.Click += new System.EventHandler(this.installedFileExplore_Click);
            // 
            // installedFileBrowse
            // 
            this.installedFileBrowse.Location = new System.Drawing.Point(258, 68);
            this.installedFileBrowse.Name = "installedFileBrowse";
            this.installedFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.installedFileBrowse.TabIndex = 2;
            this.installedFileBrowse.Text = "Browse";
            this.installedFileBrowse.UseVisualStyleBackColor = true;
            this.installedFileBrowse.Click += new System.EventHandler(this.installedFileBrowse_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(17, 42);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(316, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // installedFileLabel
            // 
            this.installedFileLabel.AutoSize = true;
            this.installedFileLabel.Location = new System.Drawing.Point(14, 26);
            this.installedFileLabel.Name = "installedFileLabel";
            this.installedFileLabel.Size = new System.Drawing.Size(111, 13);
            this.installedFileLabel.TabIndex = 0;
            this.installedFileLabel.Text = "Path of installed apps:";
            // 
            // screenshotFileGroupbox
            // 
            this.screenshotFileGroupbox.Controls.Add(this.screenshotFileDefaultButton);
            this.screenshotFileGroupbox.Controls.Add(this.screenshotExploreButton);
            this.screenshotFileGroupbox.Controls.Add(this.screenshotFileBrowse);
            this.screenshotFileGroupbox.Controls.Add(this.screenshotFileTextbox);
            this.screenshotFileGroupbox.Controls.Add(this.screenshotFileLabel);
            this.screenshotFileGroupbox.Location = new System.Drawing.Point(8, 243);
            this.screenshotFileGroupbox.Name = "screenshotFileGroupbox";
            this.screenshotFileGroupbox.Size = new System.Drawing.Size(356, 110);
            this.screenshotFileGroupbox.TabIndex = 6;
            this.screenshotFileGroupbox.TabStop = false;
            this.screenshotFileGroupbox.Text = "Screenshots";
            // 
            // screenshotFileDefaultButton
            // 
            this.screenshotFileDefaultButton.Location = new System.Drawing.Point(17, 68);
            this.screenshotFileDefaultButton.Name = "screenshotFileDefaultButton";
            this.screenshotFileDefaultButton.Size = new System.Drawing.Size(117, 23);
            this.screenshotFileDefaultButton.TabIndex = 4;
            this.screenshotFileDefaultButton.Text = "Restore to default";
            this.screenshotFileDefaultButton.UseVisualStyleBackColor = true;
            this.screenshotFileDefaultButton.Click += new System.EventHandler(this.screenshotFileDefaultButton_Click);
            // 
            // screenshotExploreButton
            // 
            this.screenshotExploreButton.Location = new System.Drawing.Point(177, 68);
            this.screenshotExploreButton.Name = "screenshotExploreButton";
            this.screenshotExploreButton.Size = new System.Drawing.Size(75, 23);
            this.screenshotExploreButton.TabIndex = 3;
            this.screenshotExploreButton.Text = "Explore";
            this.screenshotExploreButton.UseVisualStyleBackColor = true;
            this.screenshotExploreButton.Click += new System.EventHandler(this.screenshotExploreButton_Click);
            // 
            // screenshotFileBrowse
            // 
            this.screenshotFileBrowse.Location = new System.Drawing.Point(258, 68);
            this.screenshotFileBrowse.Name = "screenshotFileBrowse";
            this.screenshotFileBrowse.Size = new System.Drawing.Size(75, 23);
            this.screenshotFileBrowse.TabIndex = 2;
            this.screenshotFileBrowse.Text = "Browse";
            this.screenshotFileBrowse.UseVisualStyleBackColor = true;
            this.screenshotFileBrowse.Click += new System.EventHandler(this.screenshotFileBrowse_Click);
            // 
            // screenshotFileTextbox
            // 
            this.screenshotFileTextbox.Location = new System.Drawing.Point(17, 42);
            this.screenshotFileTextbox.Name = "screenshotFileTextbox";
            this.screenshotFileTextbox.Size = new System.Drawing.Size(316, 20);
            this.screenshotFileTextbox.TabIndex = 1;
            this.screenshotFileTextbox.TextChanged += new System.EventHandler(this.screenshotFileTextbox_TextChanged);
            // 
            // screenshotFileLabel
            // 
            this.screenshotFileLabel.AutoSize = true;
            this.screenshotFileLabel.Location = new System.Drawing.Point(14, 26);
            this.screenshotFileLabel.Name = "screenshotFileLabel";
            this.screenshotFileLabel.Size = new System.Drawing.Size(136, 13);
            this.screenshotFileLabel.TabIndex = 0;
            this.screenshotFileLabel.Text = "Path of saved screenshots:";
            // 
            // desktopGroup
            // 
            this.desktopGroup.Controls.Add(this.desktopLabel);
            this.desktopGroup.Controls.Add(this.desktopCheckbox);
            this.desktopGroup.Location = new System.Drawing.Point(8, 270);
            this.desktopGroup.Name = "desktopGroup";
            this.desktopGroup.Size = new System.Drawing.Size(356, 79);
            this.desktopGroup.TabIndex = 13;
            this.desktopGroup.TabStop = false;
            this.desktopGroup.Text = "Desktop";
            // 
            // desktopLabel
            // 
            this.desktopLabel.Location = new System.Drawing.Point(14, 38);
            this.desktopLabel.Name = "desktopLabel";
            this.desktopLabel.Size = new System.Drawing.Size(327, 26);
            this.desktopLabel.TabIndex = 6;
            this.desktopLabel.Text = "If enabled, Ecureuil creates a link of your installed app directly in your Deskto" +
                "p. Note that this works only with portable apps.";
            // 
            // desktopCheckbox
            // 
            this.desktopCheckbox.AutoSize = true;
            this.desktopCheckbox.Location = new System.Drawing.Point(17, 19);
            this.desktopCheckbox.Name = "desktopCheckbox";
            this.desktopCheckbox.Size = new System.Drawing.Size(215, 17);
            this.desktopCheckbox.TabIndex = 0;
            this.desktopCheckbox.Text = "Add to my Desktop a link to the program";
            this.desktopCheckbox.UseVisualStyleBackColor = true;
            // 
            // settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 420);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "settings";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.settings_Load);
            this.tabControl1.ResumeLayout(false);
            this.cacheTab.ResumeLayout(false);
            this.sizeGroupBox.ResumeLayout(false);
            this.sizeGroupBox.PerformLayout();
            this.cacheInnerTabControl.ResumeLayout(false);
            this.sourcesTab.ResumeLayout(false);
            this.appsTab.ResumeLayout(false);
            this.installTab.ResumeLayout(false);
            this.startGroup.ResumeLayout(false);
            this.startGroup.PerformLayout();
            this.installingGroup.ResumeLayout(false);
            this.installingGroup.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.fileTab.ResumeLayout(false);
            this.cacheFileGroupBox.ResumeLayout(false);
            this.cacheFileGroupBox.PerformLayout();
            this.installedFileGroupbox.ResumeLayout(false);
            this.installedFileGroupbox.PerformLayout();
            this.screenshotFileGroupbox.ResumeLayout(false);
            this.screenshotFileGroupbox.PerformLayout();
            this.desktopGroup.ResumeLayout(false);
            this.desktopGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage cacheTab;
        private System.Windows.Forms.TabPage installTab;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button OKButton;
        private new System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button ApplyButton;
        private System.Windows.Forms.GroupBox sizeGroupBox;
        private System.Windows.Forms.TabControl cacheInnerTabControl;
        private System.Windows.Forms.TabPage sourcesTab;
        private System.Windows.Forms.TabPage appsTab;
        private System.Windows.Forms.ListView sourcesCacheListView;
        private System.Windows.Forms.ColumnHeader colSourceHeaderName;
        private System.Windows.Forms.ColumnHeader colSourceHeaderUpdated;
        private System.Windows.Forms.ListView appsCacheListView;
        private System.Windows.Forms.ColumnHeader colAppHeaderName;
        private System.Windows.Forms.ColumnHeader colAppHeaderUpdated;
        private System.Windows.Forms.Button refreshCacheButton;
        private System.Windows.Forms.Button deleteCacheButton;
        private System.Windows.Forms.Label selectedCacheSizeLabel;
        private System.Windows.Forms.CheckBox enableCacheCheckbox;
        private System.Windows.Forms.Label enableCacheLabel;
        private System.Windows.Forms.GroupBox installingGroup;
        private System.Windows.Forms.Label label1nstallLabel2;
        private System.Windows.Forms.RadioButton installRadio1;
        private System.Windows.Forms.Label installationLabel1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.RadioButton installRadio2;
        private System.Windows.Forms.Label installLabel3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox startGroup;
        private System.Windows.Forms.CheckBox startMenuCheck;
        private System.Windows.Forms.Label startMenuLabel;
        private System.Windows.Forms.TabPage fileTab;
        private System.Windows.Forms.GroupBox cacheFileGroupBox;
        private System.Windows.Forms.Button exploreCacheFileButton;
        private System.Windows.Forms.Button browseCacheFile;
        private System.Windows.Forms.TextBox cacheFileTextbox;
        private System.Windows.Forms.Label cacheFileLabel;
        private System.Windows.Forms.GroupBox installedFileGroupbox;
        private System.Windows.Forms.Button installedFileDefault;
        private System.Windows.Forms.Button installedFileExplore;
        private System.Windows.Forms.Button installedFileBrowse;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label installedFileLabel;
        private System.Windows.Forms.Button defaultCacheFile;
        private System.Windows.Forms.GroupBox desktopGroup;
        private System.Windows.Forms.Label desktopLabel;
        private System.Windows.Forms.CheckBox desktopCheckbox;
        private System.Windows.Forms.GroupBox screenshotFileGroupbox;
        private System.Windows.Forms.Button screenshotFileDefaultButton;
        private System.Windows.Forms.Button screenshotExploreButton;
        private System.Windows.Forms.Button screenshotFileBrowse;
        private System.Windows.Forms.TextBox screenshotFileTextbox;
        private System.Windows.Forms.Label screenshotFileLabel;
    }
}