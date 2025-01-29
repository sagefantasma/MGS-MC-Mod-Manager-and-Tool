using System;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class ModResourcesForm : Form
    {
        public ModResourcesForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(ModResourcesForm_FormClosing);
        }

        private void ModResourcesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }

        private void ModResources_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from Mod Resources.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "ModResourcesForm");
            MainMenuForm form1 = new MainMenuForm();
            form1.Show();
            this.Hide();
        }
    }
}
