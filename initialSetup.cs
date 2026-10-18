using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ecureuil.Properties;
using Ecureuil.Core.Helpers;

namespace Ecureuil
{
    public partial class initialSetup : Form
    {
        public initialSetup()
        {
            InitializeComponent();
        }

        private int currentPage = 0;

        private void initialSetup_Load(object sender, EventArgs e)
        {
            this.Text = Resources.welcome;
            currentPage = 0;
        }

        private void checkModernWindows()
        {
            string windows = OSver.GetCurrentOSVersion();
            if ((windows.CompareTo("Vista") == 1) && (windows.CompareTo("7") == 1) && (windows.StartsWith("8")) && windows.StartsWith("1"))
            {
                modernWindowsNotice.Show();
            }
            else modernWindowsNotice.Hide();
        }

        private void pageChange(int page)
        {
            currentPage = page;
            backButton.Enabled = (currentPage > 0);

            switch (page)
            {
                case 0:
                    welcomeLabel1.Text = Resources.welcome;
                    welcomeLabel2.Text = Resources.welcome1;
                    EcureuilText1.Text = Resources.welcome2;
                    break;

                case 1:
                    welcomeLabel1.Text = Resources.welcomePage2;
                    welcomeLabel2.Text = Resources.welcome1Page2;
                    EcureuilText1.Text = Resources.welcome2Page2;
                    break;

                case 2:
                    welcomeLabel1.Text = Resources.welcomePage3;
                    welcomeLabel2.Text = Resources.welcome1Page3;
                    EcureuilText1.Text = Resources.welcome2Page3;
                    break;

                case 3:
                    welcomeLabel1.Text = Resources.welcomePage4;
                    welcomeLabel2.Text = Resources.welcome1Page4;
                    EcureuilText1.Text = Resources.welcome2Page4;
                    break;
                case 4:
                    welcomeLabel1.Text = Resources.welcomePage5;
                    welcomeLabel2.Text = Resources.welcome1Page5;
                    EcureuilText1.Text = Resources.welcome2Page5;
                    break;
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            pageChange(currentPage + 1);
            if (currentPage == 4)
            {
                nextButton.Text = "Finish";
            } else if (currentPage == 5){
                this.Close();
            } else {
                nextButton.Text = "Next >";
            }
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                if (currentPage == 4)
                    nextButton.Text = "Next >";
                pageChange(currentPage - 1); 
            }
        }
    }
}