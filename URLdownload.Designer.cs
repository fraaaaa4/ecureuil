namespace Ecureuil
{
    partial class URLdownload
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
            this.downloadLabel1 = new System.Windows.Forms.Label();
            this.FileDetailsBox = new System.Windows.Forms.GroupBox();
            this.detailsList = new System.Windows.Forms.ListBox();
            this.progressGroup = new System.Windows.Forms.GroupBox();
            this.sizeText = new System.Windows.Forms.Label();
            this.size = new System.Windows.Forms.Label();
            this.timeRemainingText = new System.Windows.Forms.Label();
            this.timeEplasedText = new System.Windows.Forms.Label();
            this.timeRemaining = new System.Windows.Forms.Label();
            this.timeElapsed = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pauseButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadProgress = new System.Windows.Forms.ProgressBar();
            this.FileDetailsBox.SuspendLayout();
            this.progressGroup.SuspendLayout();
            this.saveContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // downloadLabel1
            // 
            this.downloadLabel1.Location = new System.Drawing.Point(13, 13);
            this.downloadLabel1.Name = "downloadLabel1";
            this.downloadLabel1.Size = new System.Drawing.Size(267, 36);
            this.downloadLabel1.TabIndex = 0;
            this.downloadLabel1.Text = "You\'re trying to download:";
            // 
            // FileDetailsBox
            // 
            this.FileDetailsBox.Controls.Add(this.detailsList);
            this.FileDetailsBox.Location = new System.Drawing.Point(14, 58);
            this.FileDetailsBox.Name = "FileDetailsBox";
            this.FileDetailsBox.Size = new System.Drawing.Size(267, 75);
            this.FileDetailsBox.TabIndex = 1;
            this.FileDetailsBox.TabStop = false;
            this.FileDetailsBox.Text = "groupBox1";
            // 
            // detailsList
            // 
            this.detailsList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsList.FormattingEnabled = true;
            this.detailsList.Location = new System.Drawing.Point(3, 16);
            this.detailsList.Name = "detailsList";
            this.detailsList.Size = new System.Drawing.Size(261, 56);
            this.detailsList.TabIndex = 0;
            // 
            // progressGroup
            // 
            this.progressGroup.Controls.Add(this.sizeText);
            this.progressGroup.Controls.Add(this.size);
            this.progressGroup.Controls.Add(this.timeRemainingText);
            this.progressGroup.Controls.Add(this.timeEplasedText);
            this.progressGroup.Controls.Add(this.timeRemaining);
            this.progressGroup.Controls.Add(this.timeElapsed);
            this.progressGroup.Location = new System.Drawing.Point(14, 139);
            this.progressGroup.Name = "progressGroup";
            this.progressGroup.Size = new System.Drawing.Size(267, 85);
            this.progressGroup.TabIndex = 2;
            this.progressGroup.TabStop = false;
            this.progressGroup.Text = "groupBox1";
            // 
            // sizeText
            // 
            this.sizeText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sizeText.Location = new System.Drawing.Point(120, 58);
            this.sizeText.Name = "sizeText";
            this.sizeText.Size = new System.Drawing.Size(135, 13);
            this.sizeText.TabIndex = 5;
            this.sizeText.Text = "0 KB";
            this.sizeText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // size
            // 
            this.size.AutoSize = true;
            this.size.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.size.Location = new System.Drawing.Point(9, 58);
            this.size.Name = "size";
            this.size.Size = new System.Drawing.Size(41, 13);
            this.size.TabIndex = 4;
            this.size.Text = "label1";
            // 
            // timeRemainingText
            // 
            this.timeRemainingText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeRemainingText.Location = new System.Drawing.Point(120, 40);
            this.timeRemainingText.Name = "timeRemainingText";
            this.timeRemainingText.Size = new System.Drawing.Size(135, 13);
            this.timeRemainingText.TabIndex = 3;
            this.timeRemainingText.Text = "--";
            this.timeRemainingText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timeEplasedText
            // 
            this.timeEplasedText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeEplasedText.Location = new System.Drawing.Point(120, 22);
            this.timeEplasedText.Name = "timeEplasedText";
            this.timeEplasedText.Size = new System.Drawing.Size(135, 13);
            this.timeEplasedText.TabIndex = 2;
            this.timeEplasedText.Text = "00:00:00";
            this.timeEplasedText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timeRemaining
            // 
            this.timeRemaining.AutoSize = true;
            this.timeRemaining.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeRemaining.Location = new System.Drawing.Point(9, 40);
            this.timeRemaining.Name = "timeRemaining";
            this.timeRemaining.Size = new System.Drawing.Size(41, 13);
            this.timeRemaining.TabIndex = 1;
            this.timeRemaining.Text = "label1";
            // 
            // timeElapsed
            // 
            this.timeElapsed.AutoSize = true;
            this.timeElapsed.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeElapsed.Location = new System.Drawing.Point(9, 22);
            this.timeElapsed.Name = "timeElapsed";
            this.timeElapsed.Size = new System.Drawing.Size(41, 13);
            this.timeElapsed.TabIndex = 0;
            this.timeElapsed.Text = "label1";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(205, 255);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // pauseButton
            // 
            this.pauseButton.Enabled = false;
            this.pauseButton.Location = new System.Drawing.Point(124, 255);
            this.pauseButton.Name = "pauseButton";
            this.pauseButton.Size = new System.Drawing.Size(75, 23);
            this.pauseButton.TabIndex = 4;
            this.pauseButton.Text = "Pause";
            this.pauseButton.UseVisualStyleBackColor = true;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(43, 255);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 5;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveContextMenu
            // 
            this.saveContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.saveContextMenu.Name = "saveContextMenu";
            this.saveContextMenu.Size = new System.Drawing.Size(136, 48);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // downloadProgress
            // 
            this.downloadProgress.Location = new System.Drawing.Point(14, 230);
            this.downloadProgress.Name = "downloadProgress";
            this.downloadProgress.Size = new System.Drawing.Size(267, 14);
            this.downloadProgress.TabIndex = 6;
            // 
            // URLdownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 288);
            this.Controls.Add(this.downloadProgress);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.pauseButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.progressGroup);
            this.Controls.Add(this.FileDetailsBox);
            this.Controls.Add(this.downloadLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "URLdownload";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Download a file";
            this.Load += new System.EventHandler(this.URLdownload_Load);
            this.FileDetailsBox.ResumeLayout(false);
            this.progressGroup.ResumeLayout(false);
            this.progressGroup.PerformLayout();
            this.saveContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label downloadLabel1;
        private System.Windows.Forms.GroupBox FileDetailsBox;
        private System.Windows.Forms.ListBox detailsList;
        private System.Windows.Forms.GroupBox progressGroup;
        private System.Windows.Forms.Label timeElapsed;
        private System.Windows.Forms.Label timeRemaining;
        private System.Windows.Forms.Label sizeText;
        private System.Windows.Forms.Label size;
        private System.Windows.Forms.Label timeRemainingText;
        private System.Windows.Forms.Label timeEplasedText;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button pauseButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ContextMenuStrip saveContextMenu;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ProgressBar downloadProgress;
    }
}