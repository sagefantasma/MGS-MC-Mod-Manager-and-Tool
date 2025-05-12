using System;
using System.Drawing;
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

        public static class AppInfo
        {
            public const string CurrentVersion = "1.0.0.4";
        }
        private void ShowChangelog()
        {
            using (Form changelogForm = new Form())
            {
                changelogForm.Text = "Changelog - What's New!";
                changelogForm.StartPosition = FormStartPosition.CenterParent;
                changelogForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                changelogForm.MinimizeBox = false;
                changelogForm.MaximizeBox = false;
                changelogForm.ClientSize = new Size(595, 525);

                Label lbl = new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                    Text = BuildChangelogText(),
                    AutoSize = false,
                    TextAlign = ContentAlignment.TopLeft,
                    UseMnemonic = false
                };

                lbl.MaximumSize = new Size(int.MaxValue, int.MaxValue);
                lbl.AutoEllipsis = false;
                lbl.AutoSize = false;

                Button btn = new Button
                {
                    Text = "Close",
                    Dock = DockStyle.Bottom,
                    Height = 35
                };
                btn.Click += (_, __) => changelogForm.Close();

                changelogForm.Controls.Add(lbl);
                changelogForm.Controls.Add(btn);

                changelogForm.ShowDialog(this);
            }
        }

        private string BuildChangelogText()
        {
            return
@"Changelog - v1.0.0.4
- Added in more 3D models to the 3D Model and Texture Import/Export tool
- Added in a Help/FAQ button for the Texture Import/Export tool
- Bug fixes and UI improvements for Texture Import/Export tool
- MGSFPSUnlock support added to MGS2 and MGS3 Modding Forms
- MGS3CrouchWalk support added to MGS3 Modding Form
- UI Fixes where MGS3 Modding Form would start from 
top of the page everytime a mod was activated or deactivated

Thanks for using the Mod Manager and Tools! - ANTIBigBoss";
        }

        private void MainMenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();

            var settings = ConfigManager.LoadSettings();

            if (settings.Settings.LastSeenVersion != AppInfo.CurrentVersion)
            {
                ShowChangelog();
                settings.Settings.LastSeenVersion = AppInfo.CurrentVersion;
                ConfigManager.SaveSettings(settings);
            }
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

        private void KoFiLink_Click(object sender, EventArgs e)
        {
            string url = "https://ko-fi.com/antibigboss";

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };

            System.Diagnostics.Process.Start(psi);
        }

        private void HelpFaqButton_Click(object sender, EventArgs e)
        {
            ShowChangelog();
        }
    }
}
