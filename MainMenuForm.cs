using System;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(MainMenuForm_FormClosing);
        }

        private void MainMenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
        }

        private void MG1FormSwap_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MG1 form from the Main Menu form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MG1ModdingForm");
            MG1andMG2ModdingForm form2 = new MG1andMG2ModdingForm();
            form2.Show();
            this.Hide();
        }

        private void MGS1FormSwap_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MGS1 form from the Main Menu form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS1ModdingForm");
            MGS1ModdingForm form4 = new MGS1ModdingForm();
            form4.Show();
            this.Hide();
        }

        private void MGS2FormSwap_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MGS2 form from the Main Menu form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS2ModdingForm");
            MGS2ModdingForm form5 = new MGS2ModdingForm();
            form5.Show();
            this.Hide();
        }

        private void MGS3FormSwap_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MGS3 form from the Main Menu form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MGS3ModdingForm form6 = new MGS3ModdingForm();
            form6.Show();
            this.Hide();
        }

        private void ModResourcesFormSwap_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the Mod Resources form from the Main Menu form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "ModResourcesForm");
            ModResourcesForm form7 = new ModResourcesForm();
            form7.Show();
            this.Hide();
        }
    }
}
