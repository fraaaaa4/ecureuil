namespace Ecureuil
{
    partial class initialSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(initialSetup));
            this.welcomeLabel1 = new System.Windows.Forms.Label();
            this.welcomeLabel2 = new System.Windows.Forms.Label();
            this.controlGroupBox = new System.Windows.Forms.GroupBox();
            this.nextButton = new System.Windows.Forms.Button();
            this.backButton = new System.Windows.Forms.Button();
            this.EcureuilText1 = new System.Windows.Forms.Label();
            this.modernWindowsNotice = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.controlGroupBox.SuspendLayout();
            this.modernWindowsNotice.SuspendLayout();
            this.SuspendLayout();
            // 
            // welcomeLabel1
            // 
            this.welcomeLabel1.AutoSize = true;
            this.welcomeLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.welcomeLabel1.Location = new System.Drawing.Point(13, 13);
            this.welcomeLabel1.Name = "welcomeLabel1";
            this.welcomeLabel1.Size = new System.Drawing.Size(162, 20);
            this.welcomeLabel1.TabIndex = 0;
            this.welcomeLabel1.Text = "Welcome to Ecureuil";
            // 
            // welcomeLabel2
            // 
            this.welcomeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.welcomeLabel2.Location = new System.Drawing.Point(15, 31);
            this.welcomeLabel2.Name = "welcomeLabel2";
            this.welcomeLabel2.Size = new System.Drawing.Size(333, 14);
            this.welcomeLabel2.TabIndex = 1;
            this.welcomeLabel2.Text = "Your Win32 desktop package manager for many versions of Windows";
            // 
            // controlGroupBox
            // 
            this.controlGroupBox.Controls.Add(this.backButton);
            this.controlGroupBox.Controls.Add(this.nextButton);
            this.controlGroupBox.Location = new System.Drawing.Point(0, 282);
            this.controlGroupBox.Name = "controlGroupBox";
            this.controlGroupBox.Size = new System.Drawing.Size(368, 47);
            this.controlGroupBox.TabIndex = 2;
            this.controlGroupBox.TabStop = false;
            // 
            // nextButton
            // 
            this.nextButton.Location = new System.Drawing.Point(282, 14);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 0;
            this.nextButton.Text = "Next >";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // backButton
            // 
            this.backButton.Enabled = false;
            this.backButton.Location = new System.Drawing.Point(201, 14);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(75, 23);
            this.backButton.TabIndex = 1;
            this.backButton.Text = "< Back";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // EcureuilText1
            // 
            this.EcureuilText1.Location = new System.Drawing.Point(17, 56);
            this.EcureuilText1.Name = "EcureuilText1";
            this.EcureuilText1.Size = new System.Drawing.Size(331, 127);
            this.EcureuilText1.TabIndex = 3;
            this.EcureuilText1.Text = resources.GetString("EcureuilText1.Text");
            // 
            // modernWindowsNotice
            // 
            this.modernWindowsNotice.Controls.Add(this.label1);
            this.modernWindowsNotice.Location = new System.Drawing.Point(20, 188);
            this.modernWindowsNotice.Name = "modernWindowsNotice";
            this.modernWindowsNotice.Size = new System.Drawing.Size(328, 71);
            this.modernWindowsNotice.TabIndex = 4;
            this.modernWindowsNotice.TabStop = false;
            this.modernWindowsNotice.Visible = false;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(315, 47);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ecureuil has noticed you\'re using a version of Windows with UAC. In order to let " +
                "Ecureuil install apps to \"Program Files\", make sure you\'ve run it as Administrat" +
                "or.";
            // 
            // initialSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 329);
            this.Controls.Add(this.modernWindowsNotice);
            this.Controls.Add(this.EcureuilText1);
            this.Controls.Add(this.controlGroupBox);
            this.Controls.Add(this.welcomeLabel2);
            this.Controls.Add(this.welcomeLabel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "initialSetup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Welcome to Ecureuil";
            this.Load += new System.EventHandler(this.initialSetup_Load);
            this.controlGroupBox.ResumeLayout(false);
            this.modernWindowsNotice.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label welcomeLabel1;
        private System.Windows.Forms.Label welcomeLabel2;
        private System.Windows.Forms.GroupBox controlGroupBox;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Label EcureuilText1;
        private System.Windows.Forms.GroupBox modernWindowsNotice;
        private System.Windows.Forms.Label label1;
    }
}