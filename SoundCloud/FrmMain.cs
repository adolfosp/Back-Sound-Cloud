using System;
using System.Windows.Forms;
using Bunifu.UI.WinForms;

namespace SoundCloud
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnMin_Click(object sender, EventArgs e)
        {

            bunifuFormDock1.WindowState = BunifuFormDock.FormWindowStates.Minimized;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
