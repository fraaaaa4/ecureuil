namespace Ecureuil
{
    partial class aboutWindow
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
            this.appName = new System.Windows.Forms.Label();
            this.aboutVersion = new System.Windows.Forms.Label();
            this.copyrightText = new System.Windows.Forms.Label();
            this.tryMeOut = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.appIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.appIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // appName
            // 
            this.appName.AutoSize = true;
            this.appName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.appName.Location = new System.Drawing.Point(64, 9);
            this.appName.Name = "appName";
            this.appName.Size = new System.Drawing.Size(61, 18);
            this.appName.TabIndex = 0;
            this.appName.Text = "Ecureuil";
            // 
            // aboutVersion
            // 
            this.aboutVersion.AutoSize = true;
            this.aboutVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aboutVersion.Location = new System.Drawing.Point(123, 11);
            this.aboutVersion.Name = "aboutVersion";
            this.aboutVersion.Size = new System.Drawing.Size(41, 15);
            this.aboutVersion.TabIndex = 1;
            this.aboutVersion.Text = "label1";
            // 
            // copyrightText
            // 
            this.copyrightText.AutoSize = true;
            this.copyrightText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyrightText.Location = new System.Drawing.Point(64, 33);
            this.copyrightText.Name = "copyrightText";
            this.copyrightText.Size = new System.Drawing.Size(35, 13);
            this.copyrightText.TabIndex = 2;
            this.copyrightText.Text = "label1";
            // 
            // tryMeOut
            // 
            this.tryMeOut.AutoSize = true;
            this.tryMeOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tryMeOut.Location = new System.Drawing.Point(12, 65);
            this.tryMeOut.Name = "tryMeOut";
            this.tryMeOut.Size = new System.Drawing.Size(64, 13);
            this.tryMeOut.TabIndex = 3;
            this.tryMeOut.Text = "Hyperdemo!";
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(170, 56);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(65, 24);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "OK";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // appIcon
            // 
            this.appIcon.Location = new System.Drawing.Point(15, 9);
            this.appIcon.Name = "appIcon";
            this.appIcon.Size = new System.Drawing.Size(40, 40);
            this.appIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.appIcon.TabIndex = 5;
            this.appIcon.TabStop = false;
            // 
            // aboutWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(245, 91);
            this.Controls.Add(this.appIcon);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.tryMeOut);
            this.Controls.Add(this.copyrightText);
            this.Controls.Add(this.aboutVersion);
            this.Controls.Add(this.appName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "aboutWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "About ";
            this.Load += new System.EventHandler(this.aboutWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.appIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label appName;
        private System.Windows.Forms.Label aboutVersion;
        private System.Windows.Forms.Label copyrightText;
        private System.Windows.Forms.Label tryMeOut;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.PictureBox appIcon;
    }
}