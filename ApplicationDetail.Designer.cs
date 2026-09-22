namespace Ecureuil
{
    partial class ApplicationDetail
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApplicationDetail));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Source details");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("File details");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Compatibility");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Languages");
            this.appDetailToolstrip = new System.Windows.Forms.ToolStrip();
            this.installToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.openAppToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.uninstallToolbarButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyLinkButton = new System.Windows.Forms.ToolStripButton();
            this.downloadFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.appBanner = new System.Windows.Forms.Panel();
            this.appSystemCompatibility = new System.Windows.Forms.Label();
            this.appSize = new System.Windows.Forms.Label();
            this.appDeveloper = new System.Windows.Forms.Label();
            this.appName = new System.Windows.Forms.Label();
            this.appIcon = new System.Windows.Forms.PictureBox();
            this.detailsBanner = new System.Windows.Forms.Panel();
            this.appIsTrial = new System.Windows.Forms.Label();
            this.appLanguageText = new System.Windows.Forms.Label();
            this.appLanguage = new System.Windows.Forms.Label();
            this.appCompatibilityText = new System.Windows.Forms.Label();
            this.appExtensionText = new System.Windows.Forms.Label();
            this.appCompatibility = new System.Windows.Forms.Label();
            this.appExtension = new System.Windows.Forms.Label();
            this.appUploaderText = new System.Windows.Forms.Label();
            this.appSourceText = new System.Windows.Forms.Label();
            this.appUploader = new System.Windows.Forms.Label();
            this.appSource = new System.Windows.Forms.Label();
            this.isPortable = new System.Windows.Forms.Label();
            this.isImmersive = new System.Windows.Forms.Label();
            this.descriptionPanel = new System.Windows.Forms.Panel();
            this.appDescription = new System.Windows.Forms.Label();
            this.appAdditional = new System.Windows.Forms.TabControl();
            this.appScreenshotTab = new System.Windows.Forms.TabPage();
            this.appScreenshots = new System.Windows.Forms.FlowLayoutPanel();
            this.appInstallTab = new System.Windows.Forms.TabPage();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.appUninstallParameters = new System.Windows.Forms.Label();
            this.appInstructionParameterList = new System.Windows.Forms.ListBox();
            this.instructionParameters = new System.Windows.Forms.Label();
            this.appInstallationInstruction = new System.Windows.Forms.ListBox();
            this.appInstallationLabel1 = new System.Windows.Forms.Label();
            this.appSourceTab = new System.Windows.Forms.TabPage();
            this.appSameCategoryList = new System.Windows.Forms.ListView();
            this.appSimilarText = new System.Windows.Forms.Label();
            this.appSourceTabText = new System.Windows.Forms.Label();
            this.appSourceDetail = new System.Windows.Forms.TabPage();
            this.detailsTreeView = new System.Windows.Forms.TreeView();
            this.screenshotImageList = new System.Windows.Forms.ImageList(this.components);
            this.appDetailToolstrip.SuspendLayout();
            this.appBanner.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appIcon)).BeginInit();
            this.detailsBanner.SuspendLayout();
            this.descriptionPanel.SuspendLayout();
            this.appAdditional.SuspendLayout();
            this.appScreenshotTab.SuspendLayout();
            this.appInstallTab.SuspendLayout();
            this.appSourceTab.SuspendLayout();
            this.appSourceDetail.SuspendLayout();
            this.SuspendLayout();
            // 
            // appDetailToolstrip
            // 
            this.appDetailToolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.installToolbarButton,
            this.openAppToolbarButton,
            this.uninstallToolbarButton,
            this.toolStripSeparator1,
            this.copyLinkButton,
            this.downloadFile,
            this.toolStripSeparator2});
            this.appDetailToolstrip.Location = new System.Drawing.Point(0, 0);
            this.appDetailToolstrip.Name = "appDetailToolstrip";
            this.appDetailToolstrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.appDetailToolstrip.Size = new System.Drawing.Size(486, 25);
            this.appDetailToolstrip.TabIndex = 0;
            this.appDetailToolstrip.Text = "toolStrip1";
            // 
            // installToolbarButton
            // 
            this.installToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.installToolbarButton.Image = global::Ecureuil.Properties.Resources.install;
            this.installToolbarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.installToolbarButton.Name = "installToolbarButton";
            this.installToolbarButton.Size = new System.Drawing.Size(23, 22);
            this.installToolbarButton.Text = "Install";
            this.installToolbarButton.Click += new System.EventHandler(this.installToolbarButton_Click);
            // 
            // openAppToolbarButton
            // 
            this.openAppToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openAppToolbarButton.Enabled = false;
            this.openAppToolbarButton.Image = ((System.Drawing.Image)(resources.GetObject("openAppToolbarButton.Image")));
            this.openAppToolbarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openAppToolbarButton.Name = "openAppToolbarButton";
            this.openAppToolbarButton.Size = new System.Drawing.Size(23, 22);
            this.openAppToolbarButton.Text = "Open app";
            this.openAppToolbarButton.Click += new System.EventHandler(this.openAppToolbarButton_Click);
            // 
            // uninstallToolbarButton
            // 
            this.uninstallToolbarButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uninstallToolbarButton.Enabled = false;
            this.uninstallToolbarButton.Image = global::Ecureuil.Properties.Resources.uninstall;
            this.uninstallToolbarButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uninstallToolbarButton.Name = "uninstallToolbarButton";
            this.uninstallToolbarButton.Size = new System.Drawing.Size(23, 22);
            this.uninstallToolbarButton.Text = "Uninstall app";
            this.uninstallToolbarButton.Click += new System.EventHandler(this.uninstallToolbarButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // copyLinkButton
            // 
            this.copyLinkButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.copyLinkButton.Image = global::Ecureuil.Properties.Resources.copylink;
            this.copyLinkButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyLinkButton.Name = "copyLinkButton";
            this.copyLinkButton.Size = new System.Drawing.Size(23, 22);
            this.copyLinkButton.Text = "Copy link";
            this.copyLinkButton.Click += new System.EventHandler(this.copyLinkButton_Click);
            // 
            // downloadFile
            // 
            this.downloadFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.downloadFile.Image = global::Ecureuil.Properties.Resources.download;
            this.downloadFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.downloadFile.Name = "downloadFile";
            this.downloadFile.Size = new System.Drawing.Size(23, 22);
            this.downloadFile.Text = "Download button";
            this.downloadFile.Click += new System.EventHandler(this.downloadFile_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // appBanner
            // 
            this.appBanner.Controls.Add(this.appSystemCompatibility);
            this.appBanner.Controls.Add(this.appSize);
            this.appBanner.Controls.Add(this.appDeveloper);
            this.appBanner.Controls.Add(this.appName);
            this.appBanner.Controls.Add(this.appIcon);
            this.appBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.appBanner.Location = new System.Drawing.Point(0, 25);
            this.appBanner.Name = "appBanner";
            this.appBanner.Size = new System.Drawing.Size(486, 47);
            this.appBanner.TabIndex = 1;
            // 
            // appSystemCompatibility
            // 
            this.appSystemCompatibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appSystemCompatibility.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appSystemCompatibility.Location = new System.Drawing.Point(230, 8);
            this.appSystemCompatibility.Name = "appSystemCompatibility";
            this.appSystemCompatibility.Size = new System.Drawing.Size(244, 15);
            this.appSystemCompatibility.TabIndex = 5;
            this.appSystemCompatibility.Text = "Is it compatible with my PC?";
            this.appSystemCompatibility.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // appSize
            // 
            this.appSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appSize.Location = new System.Drawing.Point(260, 25);
            this.appSize.Name = "appSize";
            this.appSize.Size = new System.Drawing.Size(214, 15);
            this.appSize.TabIndex = 4;
            this.appSize.Text = "App size";
            this.appSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // appDeveloper
            // 
            this.appDeveloper.AutoSize = true;
            this.appDeveloper.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appDeveloper.Location = new System.Drawing.Point(50, 25);
            this.appDeveloper.Name = "appDeveloper";
            this.appDeveloper.Size = new System.Drawing.Size(85, 15);
            this.appDeveloper.TabIndex = 2;
            this.appDeveloper.Text = "App developer";
            // 
            // appName
            // 
            this.appName.AutoSize = true;
            this.appName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appName.Location = new System.Drawing.Point(50, 6);
            this.appName.Name = "appName";
            this.appName.Size = new System.Drawing.Size(82, 18);
            this.appName.TabIndex = 1;
            this.appName.Text = "App name";
            // 
            // appIcon
            // 
            this.appIcon.Location = new System.Drawing.Point(10, 5);
            this.appIcon.Name = "appIcon";
            this.appIcon.Size = new System.Drawing.Size(34, 35);
            this.appIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.appIcon.TabIndex = 0;
            this.appIcon.TabStop = false;
            // 
            // detailsBanner
            // 
            this.detailsBanner.Controls.Add(this.appIsTrial);
            this.detailsBanner.Controls.Add(this.appLanguageText);
            this.detailsBanner.Controls.Add(this.appLanguage);
            this.detailsBanner.Controls.Add(this.appCompatibilityText);
            this.detailsBanner.Controls.Add(this.appExtensionText);
            this.detailsBanner.Controls.Add(this.appCompatibility);
            this.detailsBanner.Controls.Add(this.appExtension);
            this.detailsBanner.Controls.Add(this.appUploaderText);
            this.detailsBanner.Controls.Add(this.appSourceText);
            this.detailsBanner.Controls.Add(this.appUploader);
            this.detailsBanner.Controls.Add(this.appSource);
            this.detailsBanner.Controls.Add(this.isPortable);
            this.detailsBanner.Controls.Add(this.isImmersive);
            this.detailsBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.detailsBanner.Location = new System.Drawing.Point(0, 72);
            this.detailsBanner.Name = "detailsBanner";
            this.detailsBanner.Size = new System.Drawing.Size(486, 47);
            this.detailsBanner.TabIndex = 2;
            // 
            // appIsTrial
            // 
            this.appIsTrial.AutoSize = true;
            this.appIsTrial.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appIsTrial.Location = new System.Drawing.Point(400, 21);
            this.appIsTrial.Name = "appIsTrial";
            this.appIsTrial.Size = new System.Drawing.Size(34, 13);
            this.appIsTrial.TabIndex = 17;
            this.appIsTrial.Text = "isTrial";
            // 
            // appLanguageText
            // 
            this.appLanguageText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appLanguageText.Location = new System.Drawing.Point(400, 3);
            this.appLanguageText.Name = "appLanguageText";
            this.appLanguageText.Size = new System.Drawing.Size(80, 13);
            this.appLanguageText.TabIndex = 16;
            this.appLanguageText.Text = "label1";
            // 
            // appLanguage
            // 
            this.appLanguage.AutoSize = true;
            this.appLanguage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appLanguage.Location = new System.Drawing.Point(335, 3);
            this.appLanguage.Name = "appLanguage";
            this.appLanguage.Size = new System.Drawing.Size(67, 13);
            this.appLanguage.TabIndex = 14;
            this.appLanguage.Text = "Language:";
            // 
            // appCompatibilityText
            // 
            this.appCompatibilityText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appCompatibilityText.Location = new System.Drawing.Point(280, 21);
            this.appCompatibilityText.Name = "appCompatibilityText";
            this.appCompatibilityText.Size = new System.Drawing.Size(50, 13);
            this.appCompatibilityText.TabIndex = 13;
            this.appCompatibilityText.Text = "label2";
            // 
            // appExtensionText
            // 
            this.appExtensionText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appExtensionText.Location = new System.Drawing.Point(265, 3);
            this.appExtensionText.Name = "appExtensionText";
            this.appExtensionText.Size = new System.Drawing.Size(65, 13);
            this.appExtensionText.TabIndex = 12;
            this.appExtensionText.Text = "label1";
            // 
            // appCompatibility
            // 
            this.appCompatibility.AutoSize = true;
            this.appCompatibility.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appCompatibility.Location = new System.Drawing.Point(195, 21);
            this.appCompatibility.Name = "appCompatibility";
            this.appCompatibility.Size = new System.Drawing.Size(82, 13);
            this.appCompatibility.TabIndex = 11;
            this.appCompatibility.Text = "Compatibility:";
            // 
            // appExtension
            // 
            this.appExtension.AutoSize = true;
            this.appExtension.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appExtension.Location = new System.Drawing.Point(195, 3);
            this.appExtension.Name = "appExtension";
            this.appExtension.Size = new System.Drawing.Size(66, 13);
            this.appExtension.TabIndex = 10;
            this.appExtension.Text = "Extension:";
            // 
            // appUploaderText
            // 
            this.appUploaderText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appUploaderText.Location = new System.Drawing.Point(145, 21);
            this.appUploaderText.Name = "appUploaderText";
            this.appUploaderText.Size = new System.Drawing.Size(45, 13);
            this.appUploaderText.TabIndex = 9;
            this.appUploaderText.Text = "label2";
            // 
            // appSourceText
            // 
            this.appSourceText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.appSourceText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appSourceText.Location = new System.Drawing.Point(135, 3);
            this.appSourceText.Name = "appSourceText";
            this.appSourceText.Size = new System.Drawing.Size(55, 13);
            this.appSourceText.TabIndex = 8;
            this.appSourceText.Text = "label1";
            this.appSourceText.Click += new System.EventHandler(this.appSourceText_Click);
            // 
            // appUploader
            // 
            this.appUploader.AutoSize = true;
            this.appUploader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appUploader.Location = new System.Drawing.Point(85, 21);
            this.appUploader.Name = "appUploader";
            this.appUploader.Size = new System.Drawing.Size(62, 13);
            this.appUploader.TabIndex = 7;
            this.appUploader.Text = "Uploader:";
            // 
            // appSource
            // 
            this.appSource.AutoSize = true;
            this.appSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appSource.Location = new System.Drawing.Point(85, 3);
            this.appSource.Name = "appSource";
            this.appSource.Size = new System.Drawing.Size(51, 13);
            this.appSource.TabIndex = 6;
            this.appSource.Text = "Source:";
            // 
            // isPortable
            // 
            this.isPortable.AutoSize = true;
            this.isPortable.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isPortable.Location = new System.Drawing.Point(6, 21);
            this.isPortable.Name = "isPortable";
            this.isPortable.Size = new System.Drawing.Size(53, 13);
            this.isPortable.TabIndex = 5;
            this.isPortable.Text = "isPortable";
            // 
            // isImmersive
            // 
            this.isImmersive.AutoSize = true;
            this.isImmersive.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.isImmersive.Location = new System.Drawing.Point(6, 3);
            this.isImmersive.Name = "isImmersive";
            this.isImmersive.Size = new System.Drawing.Size(61, 13);
            this.isImmersive.TabIndex = 4;
            this.isImmersive.Text = "isImmersive";
            // 
            // descriptionPanel
            // 
            this.descriptionPanel.AutoScroll = true;
            this.descriptionPanel.Controls.Add(this.appDescription);
            this.descriptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.descriptionPanel.Location = new System.Drawing.Point(0, 119);
            this.descriptionPanel.Name = "descriptionPanel";
            this.descriptionPanel.Size = new System.Drawing.Size(486, 60);
            this.descriptionPanel.TabIndex = 3;
            // 
            // appDescription
            // 
            this.appDescription.AutoSize = true;
            this.appDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.appDescription.Location = new System.Drawing.Point(0, 0);
            this.appDescription.Margin = new System.Windows.Forms.Padding(0);
            this.appDescription.MaximumSize = new System.Drawing.Size(460, 0);
            this.appDescription.Name = "appDescription";
            this.appDescription.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.appDescription.Size = new System.Drawing.Size(151, 25);
            this.appDescription.TabIndex = 0;
            this.appDescription.Text = "Lorem ipsum dolor sit amet";
            // 
            // appAdditional
            // 
            this.appAdditional.Controls.Add(this.appScreenshotTab);
            this.appAdditional.Controls.Add(this.appInstallTab);
            this.appAdditional.Controls.Add(this.appSourceTab);
            this.appAdditional.Controls.Add(this.appSourceDetail);
            this.appAdditional.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appAdditional.Location = new System.Drawing.Point(0, 179);
            this.appAdditional.Name = "appAdditional";
            this.appAdditional.SelectedIndex = 0;
            this.appAdditional.Size = new System.Drawing.Size(486, 140);
            this.appAdditional.TabIndex = 4;
            // 
            // appScreenshotTab
            // 
            this.appScreenshotTab.Controls.Add(this.appScreenshots);
            this.appScreenshotTab.Location = new System.Drawing.Point(4, 22);
            this.appScreenshotTab.Name = "appScreenshotTab";
            this.appScreenshotTab.Padding = new System.Windows.Forms.Padding(3);
            this.appScreenshotTab.Size = new System.Drawing.Size(478, 114);
            this.appScreenshotTab.TabIndex = 0;
            this.appScreenshotTab.Text = "Screenshots";
            this.appScreenshotTab.UseVisualStyleBackColor = true;
            // 
            // appScreenshots
            // 
            this.appScreenshots.AutoScroll = true;
            this.appScreenshots.Dock = System.Windows.Forms.DockStyle.Fill;
            this.appScreenshots.Location = new System.Drawing.Point(3, 3);
            this.appScreenshots.Name = "appScreenshots";
            this.appScreenshots.Size = new System.Drawing.Size(472, 108);
            this.appScreenshots.TabIndex = 0;
            this.appScreenshots.WrapContents = false;
            this.appScreenshots.DoubleClick += new System.EventHandler(this.appScreenshots_DoubleClick);
            // 
            // appInstallTab
            // 
            this.appInstallTab.Controls.Add(this.listBox1);
            this.appInstallTab.Controls.Add(this.appUninstallParameters);
            this.appInstallTab.Controls.Add(this.appInstructionParameterList);
            this.appInstallTab.Controls.Add(this.instructionParameters);
            this.appInstallTab.Controls.Add(this.appInstallationInstruction);
            this.appInstallTab.Controls.Add(this.appInstallationLabel1);
            this.appInstallTab.Location = new System.Drawing.Point(4, 22);
            this.appInstallTab.Name = "appInstallTab";
            this.appInstallTab.Padding = new System.Windows.Forms.Padding(3);
            this.appInstallTab.Size = new System.Drawing.Size(478, 114);
            this.appInstallTab.TabIndex = 1;
            this.appInstallTab.Text = "Installation";
            this.appInstallTab.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(251, 81);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(217, 43);
            this.listBox1.TabIndex = 5;
            // 
            // appUninstallParameters
            // 
            this.appUninstallParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appUninstallParameters.AutoSize = true;
            this.appUninstallParameters.Location = new System.Drawing.Point(250, 65);
            this.appUninstallParameters.Name = "appUninstallParameters";
            this.appUninstallParameters.Size = new System.Drawing.Size(105, 13);
            this.appUninstallParameters.TabIndex = 4;
            this.appUninstallParameters.Text = "Uninstall parameters:";
            // 
            // appInstructionParameterList
            // 
            this.appInstructionParameterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.appInstructionParameterList.FormattingEnabled = true;
            this.appInstructionParameterList.Location = new System.Drawing.Point(12, 81);
            this.appInstructionParameterList.Name = "appInstructionParameterList";
            this.appInstructionParameterList.Size = new System.Drawing.Size(215, 43);
            this.appInstructionParameterList.TabIndex = 3;
            // 
            // instructionParameters
            // 
            this.instructionParameters.AutoSize = true;
            this.instructionParameters.Location = new System.Drawing.Point(11, 65);
            this.instructionParameters.Name = "instructionParameters";
            this.instructionParameters.Size = new System.Drawing.Size(92, 13);
            this.instructionParameters.TabIndex = 2;
            this.instructionParameters.Text = "Install parameters:";
            // 
            // appInstallationInstruction
            // 
            this.appInstallationInstruction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appInstallationInstruction.FormattingEnabled = true;
            this.appInstallationInstruction.Location = new System.Drawing.Point(12, 27);
            this.appInstallationInstruction.Name = "appInstallationInstruction";
            this.appInstallationInstruction.Size = new System.Drawing.Size(456, 30);
            this.appInstallationInstruction.TabIndex = 1;
            // 
            // appInstallationLabel1
            // 
            this.appInstallationLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appInstallationLabel1.Location = new System.Drawing.Point(12, 9);
            this.appInstallationLabel1.Name = "appInstallationLabel1";
            this.appInstallationLabel1.Size = new System.Drawing.Size(456, 15);
            this.appInstallationLabel1.TabIndex = 0;
            this.appInstallationLabel1.Text = "Here are the instruction steps the developer detailed that the user should do upo" +
                "n downloading:";
            // 
            // appSourceTab
            // 
            this.appSourceTab.Controls.Add(this.appSameCategoryList);
            this.appSourceTab.Controls.Add(this.appSimilarText);
            this.appSourceTab.Controls.Add(this.appSourceTabText);
            this.appSourceTab.Location = new System.Drawing.Point(4, 22);
            this.appSourceTab.Name = "appSourceTab";
            this.appSourceTab.Size = new System.Drawing.Size(478, 114);
            this.appSourceTab.TabIndex = 2;
            this.appSourceTab.Text = "Source";
            this.appSourceTab.UseVisualStyleBackColor = true;
            // 
            // appSameCategoryList
            // 
            this.appSameCategoryList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appSameCategoryList.Location = new System.Drawing.Point(12, 45);
            this.appSameCategoryList.Name = "appSameCategoryList";
            this.appSameCategoryList.Size = new System.Drawing.Size(456, 76);
            this.appSameCategoryList.TabIndex = 20;
            this.appSameCategoryList.UseCompatibleStateImageBehavior = false;
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
            // appSourceDetail
            // 
            this.appSourceDetail.Controls.Add(this.detailsTreeView);
            this.appSourceDetail.Location = new System.Drawing.Point(4, 22);
            this.appSourceDetail.Name = "appSourceDetail";
            this.appSourceDetail.Size = new System.Drawing.Size(478, 114);
            this.appSourceDetail.TabIndex = 3;
            this.appSourceDetail.Text = "Details";
            this.appSourceDetail.UseVisualStyleBackColor = true;
            // 
            // detailsTreeView
            // 
            this.detailsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsTreeView.Location = new System.Drawing.Point(0, 0);
            this.detailsTreeView.Name = "detailsTreeView";
            treeNode1.Name = "detailsSource";
            treeNode1.Text = "Source details";
            treeNode2.Name = "fileDetails";
            treeNode2.Text = "File details";
            treeNode3.Name = "compatibilityDetails";
            treeNode3.Text = "Compatibility";
            treeNode4.Name = "languagesDetails";
            treeNode4.Text = "Languages";
            this.detailsTreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4});
            this.detailsTreeView.Size = new System.Drawing.Size(478, 114);
            this.detailsTreeView.TabIndex = 0;
            // 
            // screenshotImageList
            // 
            this.screenshotImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this.screenshotImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.screenshotImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // ApplicationDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 319);
            this.Controls.Add(this.appAdditional);
            this.Controls.Add(this.descriptionPanel);
            this.Controls.Add(this.detailsBanner);
            this.Controls.Add(this.appBanner);
            this.Controls.Add(this.appDetailToolstrip);
            this.MinimumSize = new System.Drawing.Size(480, 320);
            this.Name = "ApplicationDetail";
            this.Text = "Detail of an application";
            this.Resize += new System.EventHandler(this.ApplicationDetail_Resize);
            this.Load += new System.EventHandler(this.ApplicationDetail_Load);
            this.appDetailToolstrip.ResumeLayout(false);
            this.appDetailToolstrip.PerformLayout();
            this.appBanner.ResumeLayout(false);
            this.appBanner.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appIcon)).EndInit();
            this.detailsBanner.ResumeLayout(false);
            this.detailsBanner.PerformLayout();
            this.descriptionPanel.ResumeLayout(false);
            this.descriptionPanel.PerformLayout();
            this.appAdditional.ResumeLayout(false);
            this.appScreenshotTab.ResumeLayout(false);
            this.appInstallTab.ResumeLayout(false);
            this.appInstallTab.PerformLayout();
            this.appSourceTab.ResumeLayout(false);
            this.appSourceDetail.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip appDetailToolstrip;
        private System.Windows.Forms.ToolStripButton installToolbarButton;
        private System.Windows.Forms.ToolStripButton openAppToolbarButton;
        private System.Windows.Forms.ToolStripButton uninstallToolbarButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Panel appBanner;
        private System.Windows.Forms.Label appName;
        private System.Windows.Forms.PictureBox appIcon;
        private System.Windows.Forms.Label appDeveloper;
        private System.Windows.Forms.Label appSystemCompatibility;
        private System.Windows.Forms.Label appSize;
        private System.Windows.Forms.Panel detailsBanner;
        private System.Windows.Forms.Label appUploaderText;
        private System.Windows.Forms.Label appSourceText;
        private System.Windows.Forms.Label appUploader;
        private System.Windows.Forms.Label appSource;
        private System.Windows.Forms.Label isPortable;
        private System.Windows.Forms.Label isImmersive;
        private System.Windows.Forms.Label appCompatibilityText;
        private System.Windows.Forms.Label appExtensionText;
        private System.Windows.Forms.Label appCompatibility;
        private System.Windows.Forms.Label appExtension;
        private System.Windows.Forms.Label appIsTrial;
        private System.Windows.Forms.Label appLanguageText;
        private System.Windows.Forms.Label appLanguage;
        private System.Windows.Forms.Panel descriptionPanel;
        private System.Windows.Forms.Label appDescription;
        private System.Windows.Forms.TabControl appAdditional;
        private System.Windows.Forms.TabPage appScreenshotTab;
        private System.Windows.Forms.TabPage appInstallTab;
        private System.Windows.Forms.TabPage appSourceTab;
        private System.Windows.Forms.ImageList screenshotImageList;
        private System.Windows.Forms.FlowLayoutPanel appScreenshots;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label appUninstallParameters;
        private System.Windows.Forms.ListBox appInstructionParameterList;
        private System.Windows.Forms.Label instructionParameters;
        private System.Windows.Forms.ListBox appInstallationInstruction;
        private System.Windows.Forms.Label appInstallationLabel1;
        private System.Windows.Forms.Label appSourceTabText;
        private System.Windows.Forms.ListView appSameCategoryList;
        private System.Windows.Forms.Label appSimilarText;
        private System.Windows.Forms.TabPage appSourceDetail;
        private System.Windows.Forms.TreeView detailsTreeView;
        private System.Windows.Forms.ToolStripButton copyLinkButton;
        private System.Windows.Forms.ToolStripButton downloadFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}