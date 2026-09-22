using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ecureuil.Properties;
using System.Reflection;

namespace Ecureuil
{
    public partial class aboutWindow : Form
    {
        public aboutWindow()
        {
            InitializeComponent();
        }
       
            
        private void aboutWindow_Load(object sender, EventArgs e)
        {
            this.Text = Resources.about + Application.ProductName;
            appName.Text = Application.ProductName;
            aboutVersion.Text = Resources.version + " " + Application.ProductVersion;

            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            string copyright = "";
            if (attributes.Length > 0)
                copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            copyrightText.Text = copyright;

            if (this.Owner != null && this.Owner.Icon != null) appIcon.Image = Owner.Icon.ToBitmap();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}