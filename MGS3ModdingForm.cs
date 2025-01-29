using System;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MGS3ModdingForm : Form
    {
        public MGS3ModdingForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(MGS3ModdingForm_FormClosing);
        }

        private void MGS3ModdingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }

        private void MGS3ModdingForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from MGS3.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MainMenuForm form1 = new MainMenuForm();
            form1.Show();
            this.Hide();
        }
    }
}
