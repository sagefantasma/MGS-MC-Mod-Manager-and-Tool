using System;
using System.Linq;
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

        private void bakebutton_Click(object sender, EventArgs e)
        {
            string blendFilePath = @"D:\3D Models\MGS3\GRU BOI\Ene_Def.blend";
            string outputImagePath = @"D:\3D Models\MGS3\GRU BOI\PakaBaked.png";

            bool success = CyclesBaker.Bake(blendFilePath, outputImagePath);
            if (success)
                MessageBox.Show("Baking succeeded! Output saved to: " + outputImagePath);
            else
                MessageBox.Show("Baking failed.");
        }

        private void TextureImportExportButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Changing to Texture and 3D Model form from the Mod Resources form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "TextureModelForm");
            TextureModelForm textureModelForm = new TextureModelForm();
            textureModelForm.Show();
            this.Hide();
        }
    }
}
