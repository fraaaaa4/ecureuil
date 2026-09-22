namespace Ecureuil
{
    partial class SourcesDetails
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SourcesDetails));
            this.sourceBanner = new System.Windows.Forms.Panel();
            this.sourceCreation = new System.Windows.Forms.Label();
            this.sourceAuthor = new System.Windows.Forms.Label();
            this.sourceName = new System.Windows.Forms.Label();
            this.sourceIcon = new System.Windows.Forms.PictureBox();
            this.detailsBanner = new System.Windows.Forms.Panel();
            this.originalURLText = new System.Windows.Forms.Label();
            this.URLText = new System.Windows.Forms.Label();
            this.originalURL = new System.Windows.Forms.Label();
            this.URL = new System.Windows.Forms.Label();
            this.appUploaderText = new System.Windows.Forms.Label();
            this.lastUpdatedText = new System.Windows.Forms.Label();
            this.createdDate = new System.Windows.Forms.Label();
            this.lastUpdated = new System.Windows.Forms.Label();
            this.isEnabled = new System.Windows.Forms.Label();
            this.isLocal = new System.Windows.Forms.Label();
            this.sourceAdditional = new System.Windows.Forms.TabControl();
            this.sourceAppsTab = new System.Windows.Forms.TabPage();
            this.sourcesAppList = new System.Windows.Forms.ListView();
            this.appsToolStrip = new System.Windows.Forms.ToolStrip();
            this.OrderBy = new System.Windows.Forms.ToolStripLabel();
            this.orderByComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.groupBy = new System.Windows.Forms.ToolStripLabel();
            this.groupByComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.viewOptions = new System.Windows.Forms.ToolStripSplitButton();
            this.largeIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallIconsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceLinksTab = new System.Windows.Forms.TabPage();
            this.appSameCategoryList = new System.Windows.Forms.ListView();
            this.appSimilarText = new System.Windows.Forms.Label();
            this.appSourceTabText = new System.Windows.Forms.Label();
            this.appScreenshots = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sourceDescription = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.EnableDisableButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.sourceBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceIcon)).BeginInit();
            this.detailsBanner.SuspendLayout();
            this.sourceAdditional.SuspendLayout();
            this.sourceAppsTab.SuspendLayout();
            this.appsToolStrip.SuspendLayout();
            this.sourceLinksTab.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sourceBanner
            // 
            this.sourceBanner.Controls.Add(this.sourceCreation);
            this.sourceBanner.Controls.Add(this.sourceAuthor);
            this.sourceBanner.Controls.Add(this.sourceName);
            this.sourceBanner.Controls.Add(this.sourceIcon);
            this.sourceBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.sourceBanner.Location = new System.Drawing.Point(0, 25);
            this.sourceBanner.Name = "sourceBanner";
            this.sourceBanner.Size = new System.Drawing.Size(486, 47);
            this.sourceBanner.TabIndex = 2;
            // 
            // sourceCreation
            // 
            this.sourceCreation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceCreation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceCreation.Location = new System.Drawing.Point(260, 16);
            this.sourceCreation.Name = "sourceCreation";
            this.sourceCreation.Size = new System.Drawing.Size(214, 15);
            this.sourceCreation.TabIndex = 4;
            this.sourceCreation.Text = "App size";
            this.sourceCreation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // sourceAuthor
            // 
            this.sourceAuthor.AutoSize = true;
            this.sourceAuthor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceAuthor.Location = new System.Drawing.Point(50, 25);
            this.sourceAuthor.Name = "sourceAuthor";
            this.sourceAuthor.Size = new System.Drawing.Size(85, 15);
            this.sourceAuthor.TabIndex = 2;
            this.sourceAuthor.Text = "App developer";
            // 
            // sourceName
            // 
            this.sourceName.AutoSize = true;
            this.sourceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sourceName.Location = new System.Drawing.Point(50, 6);
            this.sourceName.Name = "sourceName";
            this.sourceName.Size = new System.Drawing.Size(82, 18);
            this.sourceName.TabIndex = 1;
            this.sourceName.Text = "App name";
            // 
            // sourceIcon
            // 
            this.sourceIcon.Location = new System.Drawing.Point(10, 5);
            this.sourceIcon.Name = "sourceIcon";
            this.sourceIcon.Size = new System.Drawing.Size(34, 35);
            this.sourceIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.sourceIcon.TabIndex = 0;
            this.sourceIcon.TabStop = false;
            // 
            // detailsBanner
            // 
            this.detailsBanner.Controls.Add(this.originalURLText);
            this.detailsBanner.Controls.Add(this.URLText);
            this.detailsBanner.Controls.Add(this.originalURL);
            this.detailsBanner.Controls.Add(this.URL);
            this.detailsBanner.Controls.Add(this.appUploaderText);
            this.detailsBanner.Controls.Add(this.lastUpdatedText);
            this.detailsBanner.Controls.Add(this.createdDate);
            this.detailsBanner.Controls.Add(this.lastUpdated);
            this.detailsBanner.Controls.Add(this.isEnabled);
            this.detailsBanner.Controls.Add(this.isLocal);
            this.detailsBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.detailsBanner.Location = new System.Drawing.Point(0, 72);
            this.detailsBanner.Name = "detailsBanner";
            this.detailsBanner.Size = new System.Drawing.Size(486, 47);
            this.detailsBanner.TabIndex = 3;
            // 
            // originalURLText
            // 
            this.originalURLText.AutoEllipsis = true;
            this.originalURLText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.originalURLText.Location = new System.Drawing.Point(330, 21);
            this.originalURLText.Name = "originalURLText";
            this.originalURLText.Size = new System.Drawing.Size(148, 13);
            this.originalURLText.TabIndex = 13;
            // 
            // URLText
            // 
            this.URLText.AutoEllipsis = true;
            this.URLText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URLText.Location = new System.Drawing.Point(283, 3);
            this.URLText.Name = "URLText";
            this.URLText.Size = new System.Drawing.Size(195, 13);
            this.URLText.TabIndex = 12;
            this.URLText.Text = "-";
            // 
            // originalURL
            // 
            this.originalURL.AutoSize = true;
            this.originalURL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.originalURL.Location = new System.Drawing.Point(245, 21);
            this.originalURL.Name = "originalURL";
            this.originalURL.Size = new System.Drawing.Size(83, 13);
            this.originalURL.TabIndex = 11;
            this.originalURL.Text = "Original URL:";
            // 
            // URL
            // 
            this.URL.AutoSize = true;
            this.URL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.URL.Location = new System.Drawing.Point(245, 3);
            this.URL.Name = "URL";
            this.URL.Size = new System.Drawing.Size(36, 13);
            this.URL.TabIndex = 10;
            this.URL.Text = "URL:";
            // 
            // appUploaderText
            // 
            this.appUploaderText.AutoSize = true;
            this.appUploaderText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appUploaderText.Location = new System.Drawing.Point(168, 21);
            this.appUploaderText.Name = "appUploaderText";
            this.appUploaderText.Size = new System.Drawing.Size(10, 13);
            this.appUploaderText.TabIndex = 9;
            this.appUploaderText.Text = "-";
            // 
            // lastUpdatedText
            // 
            this.lastUpdatedText.AutoSize = true;
            this.lastUpdatedText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastUpdatedText.Location = new System.Drawing.Point(180, 3);
            this.lastUpdatedText.Name = "lastUpdatedText";
            this.lastUpdatedText.Size = new System.Drawing.Size(10, 13);
            this.lastUpdatedText.TabIndex = 8;
            this.lastUpdatedText.Text = "-";
            // 
            // createdDate
            // 
            this.createdDate.AutoSize = true;
            this.createdDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.createdDate.Location = new System.Drawing.Point(95, 21);
            this.createdDate.Name = "createdDate";
            this.createdDate.Size = new System.Drawing.Size(73, 13);
            this.createdDate.TabIndex = 7;
            this.createdDate.Text = "Created on:";
            // 
            // lastUpdated
            // 
            this.lastUpdated.AutoSize = true;
            this.lastUpdated.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lastUpdated.Location = new System.Drawing.Point(95, 3);
            this.lastUpdated.Name = "lastUpdated";
            this.lastUpdated.Size = new System.Drawing.Size(85, 13);
            this.lastUpdated.TabIndex = 6;
            this.lastUpdated.Text = "Last updated:";
            // 
            // isEnabled
            // 
            this.isEnabled.AutoSize = true;
            this.isEnabled.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isEnabled.Location = new System.Drawing.Point(6, 21);
            this.isEnabled.Name = "isEnabled";
            this.isEnabled.Size = new System.Drawing.Size(46, 13);
            this.isEnabled.TabIndex = 5;
            this.isEnabled.Text = "Enabled";
            // 
            // isLocal
            // 
            this.isLocal.AutoSize = true;
            this.isLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isLocal.Location = new System.Drawing.Point(6, 3);
            this.isLocal.Name = "isLocal";
            this.isLocal.Size = new System.Drawing.Size(79, 13);
            this.isLocal.TabIndex = 4;
            this.isLocal.Text = "Remote source";
            // 
            // sourceAdditional
            // 
            this.sourceAdditional.Controls.Add(this.sourceAppsTab);
            this.sourceAdditional.Controls.Add(this.sourceLinksTab);
            this.sourceAdditional.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceAdditional.Location = new System.Drawing.Point(0, 160);
            this.sourceAdditional.Name = "sourceAdditional";
            this.sourceAdditional.SelectedIndex = 0;
            this.sourceAdditional.Size = new System.Drawing.Size(486, 159);
            this.sourceAdditional.TabIndex = 5;
            // 
            // sourceAppsTab
            // 
            this.sourceAppsTab.Controls.Add(this.sourcesAppList);
            this.sourceAppsTab.Controls.Add(this.appsToolStrip);
            this.sourceAppsTab.Location = new System.Drawing.Point(4, 22);
            this.sourceAppsTab.Name = "sourceAppsTab";
            this.sourceAppsTab.Padding = new System.Windows.Forms.Padding(3);
            this.sourceAppsTab.Size = new System.Drawing.Size(478, 133);
            this.sourceAppsTab.TabIndex = 0;
            this.sourceAppsTab.Text = "Apps";
            this.sourceAppsTab.UseVisualStyleBackColor = true;
            // 
            // sourcesAppList
            // 
            this.sourcesAppList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourcesAppList.FullRowSelect = true;
            this.sourcesAppList.Location = new System.Drawing.Point(3, 28);
            this.sourcesAppList.Name = "sourcesAppList";
            this.sourcesAppList.Size = new System.Drawing.Size(472, 102);
            this.sourcesAppList.TabIndex = 0;
            this.sourcesAppList.UseCompatibleStateImageBehavior = false;
            this.sourcesAppList.View = System.Windows.Forms.View.Details;
            this.sourcesAppList.DoubleClick += new System.EventHandler(this.sourcesAppList_DoubleClick);
            // 
            // appsToolStrip
            // 
            this.appsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OrderBy,
            this.orderByComboBox,
            this.toolStripSeparator1,
            this.groupBy,
            this.groupByComboBox,
            this.toolStripSeparator2,
            this.viewOptions});
            this.appsToolStrip.Location = new System.Drawing.Point(3, 3);
            this.appsToolStrip.Name = "appsToolStrip";
            this.appsToolStrip.Size = new System.Drawing.Size(472, 25);
            this.appsToolStrip.TabIndex = 2;
            this.appsToolStrip.Text = "toolStrip1";
            // 
            // OrderBy
            // 
            this.OrderBy.Name = "OrderBy";
            this.OrderBy.Size = new System.Drawing.Size(54, 22);
            this.OrderBy.Text = "Order by:";
            // 
            // orderByComboBox
            // 
            this.orderByComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.orderByComboBox.DropDownWidth = 121;
            this.orderByComboBox.Items.AddRange(new object[] {
            "Name",
            "App developer",
            "Uploader",
            "Size",
            "Original date",
            "Date uploaded"});
            this.orderByComboBox.Name = "orderByComboBox";
            this.orderByComboBox.Size = new System.Drawing.Size(110, 25);
            this.orderByComboBox.SelectedIndexChanged += new System.EventHandler(this.orderByComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // groupBy
            // 
            this.groupBy.Name = "groupBy";
            this.groupBy.Size = new System.Drawing.Size(55, 22);
            this.groupBy.Text = "Group by:";
            // 
            // groupByComboBox
            // 
            this.groupByComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.groupByComboBox.DropDownWidth = 121;
            this.groupByComboBox.Items.AddRange(new object[] {
            "(None)",
            "Category",
            "Author",
            "Uploader",
            "Immersive tag",
            "Architecture",
            "Portable tag"});
            this.groupByComboBox.Name = "groupByComboBox";
            this.groupByComboBox.Size = new System.Drawing.Size(100, 25);
            this.groupByComboBox.SelectedIndexChanged += new System.EventHandler(this.groupByComboBox_SelectedIndexChanged);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // viewOptions
            // 
            this.viewOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.viewOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeIconsToolStripMenuItem,
            this.smallIconsToolStripMenuItem,
            this.detailsToolStripMenuItem,
            this.listToolStripMenuItem,
            this.tileToolStripMenuItem});
            this.viewOptions.Image = ((System.Drawing.Image)(resources.GetObject("viewOptions.Image")));
            this.viewOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.viewOptions.Name = "viewOptions";
            this.viewOptions.Size = new System.Drawing.Size(32, 22);
            this.viewOptions.Text = "View options";
            // 
            // largeIconsToolStripMenuItem
            // 
            this.largeIconsToolStripMenuItem.Name = "largeIconsToolStripMenuItem";
            this.largeIconsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.largeIconsToolStripMenuItem.Text = "Large icons";
            this.largeIconsToolStripMenuItem.Click += new System.EventHandler(this.largeIconsToolStripMenuItem_Click);
            // 
            // smallIconsToolStripMenuItem
            // 
            this.smallIconsToolStripMenuItem.Name = "smallIconsToolStripMenuItem";
            this.smallIconsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.smallIconsToolStripMenuItem.Text = "Small icons";
            this.smallIconsToolStripMenuItem.Click += new System.EventHandler(this.smallIconsToolStripMenuItem_Click);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.detailsToolStripMenuItem.Text = "Details";
            this.detailsToolStripMenuItem.Click += new System.EventHandler(this.detailsToolStripMenuItem_Click);
            // 
            // listToolStripMenuItem
            // 
            this.listToolStripMenuItem.Name = "listToolStripMenuItem";
            this.listToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.listToolStripMenuItem.Text = "List";
            this.listToolStripMenuItem.Click += new System.EventHandler(this.listToolStripMenuItem_Click);
            // 
            // tileToolStripMenuItem
            // 
            this.tileToolStripMenuItem.Name = "tileToolStripMenuItem";
            this.tileToolStripMenuItem.Size = new System.Drawing.Size(139, 22);
            this.tileToolStripMenuItem.Text = "Tile";
            this.tileToolStripMenuItem.Click += new System.EventHandler(this.tileToolStripMenuItem_Click);
            // 
            // sourceLinksTab
            // 
            this.sourceLinksTab.Controls.Add(this.appSameCategoryList);
            this.sourceLinksTab.Controls.Add(this.appSimilarText);
            this.sourceLinksTab.Controls.Add(this.appSourceTabText);
            this.sourceLinksTab.Location = new System.Drawing.Point(4, 22);
            this.sourceLinksTab.Name = "sourceLinksTab";
            this.sourceLinksTab.Size = new System.Drawing.Size(478, 133);
            this.sourceLinksTab.TabIndex = 2;
            this.sourceLinksTab.Text = "Creator links";
            this.sourceLinksTab.UseVisualStyleBackColor = true;
            // 
            // appSameCategoryList
            // 
            this.appSameCategoryList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appSameCategoryList.FullRowSelect = true;
            this.appSameCategoryList.Location = new System.Drawing.Point(12, 45);
            this.appSameCategoryList.Name = "appSameCategoryList";
            this.appSameCategoryList.Size = new System.Drawing.Size(456, 101);
            this.appSameCategoryList.TabIndex = 20;
            this.appSameCategoryList.UseCompatibleStateImageBehavior = false;
            this.appSameCategoryList.View = System.Windows.Forms.View.Details;
            this.appSameCategoryList.DoubleClick += new System.EventHandler(this.appSameCategoryList_DoubleClick);
            // 
            // appSimilarText
            // 
            this.appSimilarText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appSimilarText.Location = new System.Drawing.Point(12, 26);
            this.appSimilarText.Name = "appSimilarText";
            this.appSimilarText.Size = new System.Drawing.Size(454, 15);
            this.appSimilarText.TabIndex = 19;
            this.appSimilarText.Text = "Apps from the same category and source:";
            // 
            // appSourceTabText
            // 
            this.appSourceTabText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appSourceTabText.Location = new System.Drawing.Point(12, 9);
            this.appSourceTabText.Name = "appSourceTabText";
            this.appSourceTabText.Size = new System.Drawing.Size(454, 15);
            this.appSourceTabText.TabIndex = 1;
            this.appSourceTabText.Text = "The app comes from $ source, from $ category.";
            // 
            // appScreenshots
            // 
            this.appScreenshots.Location = new System.Drawing.Point(0, 0);
            this.appScreenshots.Name = "appScreenshots";
            this.appScreenshots.Size = new System.Drawing.Size(200, 100);
            this.appScreenshots.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(200, 100);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 100);
            this.panel1.TabIndex = 0;
            // 
            // sourceDescription
            // 
            this.sourceDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.sourceDescription.Location = new System.Drawing.Point(0, 119);
            this.sourceDescription.Margin = new System.Windows.Forms.Padding(20, 0, 0, 20);
            this.sourceDescription.Name = "sourceDescription";
            this.sourceDescription.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.sourceDescription.Size = new System.Drawing.Size(486, 41);
            this.sourceDescription.TabIndex = 6;
            this.sourceDescription.Text = "Lorem ipsum dolor sit amet";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EnableDisableButton,
            this.refreshButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(486, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // EnableDisableButton
            // 
            this.EnableDisableButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.EnableDisableButton.Image = ((System.Drawing.Image)(resources.GetObject("EnableDisableButton.Image")));
            this.EnableDisableButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EnableDisableButton.Name = "EnableDisableButton";
            this.EnableDisableButton.Size = new System.Drawing.Size(23, 22);
            this.EnableDisableButton.Text = "Enable / Disable";
            this.EnableDisableButton.Click += new System.EventHandler(this.EnableDisableButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = global::Ecureuil.Properties.Resources.refresh1;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "Refresh source";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // SourcesDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 319);
            this.Controls.Add(this.sourceAdditional);
            this.Controls.Add(this.sourceDescription);
            this.Controls.Add(this.detailsBanner);
            this.Controls.Add(this.sourceBanner);
            this.Controls.Add(this.toolStrip1);
            this.Name = "SourcesDetails";
            this.ShowInTaskbar = false;
            this.Text = "Details of a source";
            this.Load += new System.EventHandler(this.SourcesDetails_Load);
            this.sourceBanner.ResumeLayout(false);
            this.sourceBanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sourceIcon)).EndInit();
            this.detailsBanner.ResumeLayout(false);
            this.detailsBanner.PerformLayout();
            this.sourceAdditional.ResumeLayout(false);
            this.sourceAppsTab.ResumeLayout(false);
            this.sourceAppsTab.PerformLayout();
            this.appsToolStrip.ResumeLayout(false);
            this.appsToolStrip.PerformLayout();
            this.sourceLinksTab.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel sourceBanner;
        private System.Windows.Forms.Label sourceCreation;
        private System.Windows.Forms.Label sourceAuthor;
        private System.Windows.Forms.Label sourceName;
        private System.Windows.Forms.PictureBox sourceIcon;
        private System.Windows.Forms.Panel detailsBanner;
        private System.Windows.Forms.Label URLText;
        private System.Windows.Forms.Label URL;
        private System.Windows.Forms.Label appUploaderText;
        private System.Windows.Forms.Label lastUpdatedText;
        private System.Windows.Forms.Label createdDate;
        private System.Windows.Forms.Label lastUpdated;
        private System.Windows.Forms.Label isEnabled;
        private System.Windows.Forms.Label isLocal;
        private System.Windows.Forms.Label originalURLText;
        private System.Windows.Forms.Label originalURL;
        private System.Windows.Forms.TabControl sourceAdditional;
        private System.Windows.Forms.TabPage sourceAppsTab;
        private System.Windows.Forms.FlowLayoutPanel appScreenshots;
        private System.Windows.Forms.TabPage sourceLinksTab;
        private System.Windows.Forms.ListView appSameCategoryList;
        private System.Windows.Forms.Label appSimilarText;
        private System.Windows.Forms.Label appSourceTabText;
        private System.Windows.Forms.Label sourceDescription;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStrip appsToolStrip;
        private System.Windows.Forms.ToolStripLabel OrderBy;
        private System.Windows.Forms.ToolStripComboBox orderByComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel groupBy;
        private System.Windows.Forms.ToolStripComboBox groupByComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSplitButton viewOptions;
        private System.Windows.Forms.ToolStripMenuItem largeIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallIconsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tileToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView sourcesAppList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton EnableDisableButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
    }
}