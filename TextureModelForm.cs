using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class TextureModelForm : Form
    {
        private ElementHost elementHost;
        private ModelViewerControl modelViewerControl;
        private Panel panelTextures;
        private string currentModelPath;
        private string currentMtlPath;
        private readonly List<string> _allModels = new List<string>
        {
        "MGS3 Snake SE", "MGS3 GRU", "MGS3 KGB", "MGS3 Ocelot Unit", "MGS3 Officer",
        "MGS2 Snake Tanker", "MGS2 Raiden", "MGS2 Tanker Guards", "MGS2 Big Shell Guards", "MGS2 NYPD"
        };

        public TextureModelForm()
        {
            InitializeComponent();
            MinimumSize = new Size(800, 600);
            BackColor = Color.Black;

            panelTextures = new Panel
            {
                Name = "panelTextures",
                AutoScroll = true,
                Location = new Point(0, 0),
                Size = new Size(ClientSize.Width / 2, ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            Controls.Add(panelTextures);

            elementHost = new ElementHost
            {
                Name = "elementHost3D",
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            modelViewerControl = new ModelViewerControl();
            elementHost.Child = modelViewerControl;
            Controls.Add(elementHost);
            elementHost.BringToFront();
            AdjustElementHostSize();

            ShowMgs2ModelsCheckBox.CheckedChanged += FilterModels;
            ShowMgs3ModelsCheckBox.CheckedChanged += FilterModels;
            ShowMgs2ModelsCheckBox.Checked = true;
            ShowMgs3ModelsCheckBox.Checked = true;

            Load += TextureModelForm_Load;
            FormClosing += TextureModelForm_FormClosing;
        }

        private async void TextureModelForm_Load(object sender, EventArgs e)
        {
            var config = ConfigManager.LoadSettings();
            if (!await SetupModToolsAndAssetsAsync(config))
            {
                ReturnToMainMenu();
                Hide();
                return;
            }

            RefreshModelSelection();

            if (!string.IsNullOrEmpty(currentMtlPath) && File.Exists(currentMtlPath))
                RestoreAllMtlReferencesToOriginal(currentMtlPath);
        }

        private void FilterModels(object sender, EventArgs e)
        {
            RefreshModelSelection();
        }

        private void RefreshModelSelection()
        {
            ModelSelectionComboBox.Items.Clear();

            if (ShowMgs2ModelsCheckBox.Checked)
                foreach (var m in _allModels.Where(x => x.StartsWith("MGS2")))
                    ModelSelectionComboBox.Items.Add(m);

            if (ShowMgs3ModelsCheckBox.Checked)
                foreach (var m in _allModels.Where(x => x.StartsWith("MGS3")))
                    ModelSelectionComboBox.Items.Add(m);

            if (ModelSelectionComboBox.Items.Count > 0)
                ModelSelectionComboBox.SelectedIndex = 0;
        }

        private async Task<bool> SetupModToolsAndAssetsAsync(ConfigSettings config)
        {
            if (!CheckAndPromptForModToolsPath(config))
                return false;

            try
            {
                await new DownloadManager().EnsureModToolsDownloaded(config.ModToolsPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error downloading mod tools: " + ex.Message);
                return false;
            }

            if (!CheckAndPromptForGimpConsolePath(config))
                return false;

            if (!CheckAndPromptForPythonPath(config))
                return false;

            if (!CheckAndPromptForGimpPythonScriptPath(config))
                return false;

            return true;
        }

        private void PopulateModelSelection()
        {
            ModelSelectionComboBox.Items.Clear();
            ModelSelectionComboBox.Items.AddRange(new[] {
                "MGS3 Snake SE", "MGS3 GRU", "MGS3 KGB", "MGS3 Ocelot Unit", "MGS3 Officer",
                "MGS2 Snake Tanker", "MGS2 Raiden", "MGS2 Tanker Guards", "MGS2 Big Shell Guards", "MGS2 NYPD"
            });
            ModelSelectionComboBox.SelectedIndex = 0;
        }

        private bool CheckAndPromptForModToolsPath(ConfigSettings config)
        {
            if (!config.ModToolsFolderSet)
            {
                var res = MessageBox.Show(
                    $"Set up mod tools folder at:\n{config.ModToolsPath}",
                    "Mod Tools Folder", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel) return false;
                if (res == DialogResult.No)
                {
                    using var fbd = new FolderBrowserDialog { SelectedPath = config.ModToolsPath };
                    if (fbd.ShowDialog() == DialogResult.OK)
                        config.ModToolsPath = Path.Combine(fbd.SelectedPath, "MGS Modding Tools");
                    else return false;
                }
                config.ModToolsFolderSet = true;
                ConfigManager.SaveSettings(config);
            }
            return true;
        }

        private bool CheckAndPromptForGimpConsolePath(ConfigSettings config)
        {
            if (!string.IsNullOrWhiteSpace(config.GimpConsolePath) && File.Exists(config.GimpConsolePath))
                return true;

            var defaultExe = @"C:\Program Files\GIMP 2\bin\gimp-console-2.10.exe";
            if (File.Exists(defaultExe))
            {
                config.GimpConsolePath = defaultExe;
                ConfigManager.SaveSettings(config);
                return true;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "Locate gimp-console-2.10.exe",
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                config.GimpConsolePath = ofd.FileName;
                ConfigManager.SaveSettings(config);
                return true;
            }
            return false;
        }

        private bool CheckAndPromptForPythonPath(ConfigSettings cfg)
        {
            string[] tries = {
        cfg.PythonExePath,
        "python"
    };

            foreach (var exe in tries.Where(t => !string.IsNullOrWhiteSpace(t)))
            {
                try
                {
                    var psi = new ProcessStartInfo(exe, "--version")
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    using var proc = Process.Start(psi);
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        cfg.PythonExePath = exe;
                        ConfigManager.SaveSettings(cfg);
                        return true;
                    }
                }
                catch { }
            }

            var msg = "Python was not found on your system.\n" +
                      "Would you like to locate python.exe, or download it?";
            var res = MessageBox.Show(
                msg,
                "Python Not Found",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1
            );

            if (res == DialogResult.Yes)
            {
                using var ofd = new OpenFileDialog
                {
                    Title = "Locate python.exe",
                    Filter = "Executable|python.exe|All Files|*.*"
                };
                if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
                {
                    cfg.PythonExePath = ofd.FileName;
                    ConfigManager.SaveSettings(cfg);
                    return true;
                }
            }
            else if (res == DialogResult.No)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://www.python.org/downloads/",
                    UseShellExecute = true
                });
            }
            return false;
        }


        private bool CheckAndPromptForGimpPythonScriptPath(ConfigSettings config)
        {
            var defaultScript = config.Assets.PythonScriptPath;

            if (File.Exists(defaultScript))
            {
                config.GimpPythonScriptPath = defaultScript;
                ConfigManager.SaveSettings(config);
                return true;
            }

            using var ofd = new OpenFileDialog
            {
                Title = "Locate PythonFU.py",
                Filter = "Python Files (*.py)|*.py|All Files (*.*)|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                config.GimpPythonScriptPath = ofd.FileName;
                ConfigManager.SaveSettings(config);
                return true;
            }

            return false;
        }


        private void TextureModelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!string.IsNullOrEmpty(currentMtlPath) && File.Exists(currentMtlPath))
                RestoreAllMtlReferencesToOriginal(currentMtlPath);
            Application.Exit();
        }

        private void AdjustElementHostSize()
        {
            var half = ClientSize.Width / 2;
            elementHost.Location = new Point(half, 0);
            elementHost.Size = new Size(half, ClientSize.Height);
        }

        private void ReturnToMainMenu()
        {
            GuiManager.UpdateLastFormLocation(Location);
            GuiManager.LogFormLocation(this, nameof(TextureModelForm));
            new MainMenuForm().Show();
            Hide();
        }

        private void LoadGruButton_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            panelTextures.Controls.Clear();

            string selectedModel = ModelSelectionComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedModel))
            {
                MessageBox.Show("Please select a model from the dropdown.");
                return;
            }

            string folder = string.Empty;
            string modelFile = string.Empty;
            string mtlFile = string.Empty;
            string[] textureFiles = null;

            if (selectedModel == "MGS3 Snake SE")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake SE");
                modelFile = "MGS3 Snake.obj";
                mtlFile = "MGS3 Snake.mtl";
                textureFiles = new string[]
                {
                    "sna_def_olive.bmp.png",
                    "sna_def_hand.bmp.png",
                    "cord_ovl_alp.bmp.png",
                    "cqcc_tex02.bmp.png",
                    "cqck_tex02.bmp.png",
                    "sna_bandana_def.bmp.png",
                    "sna_bk.bmp.png",
                    "sna_def_hair_base.bmp.png",
                    "sna_def_hair_front_ovl_alp.bmp.png",
                    "sna_def_vr_eye.bmp.png",
                    "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
                    "sna_foot_sp.bmp.png",
                    "sna_foot_vr.bmp.png",
                    "sna_hair_back_ovl_alp.bmp.png",
                    "sna_hair_front_ovl_alp.bmp.png",
                    "sna_hair_layer_ovl_alp.bmp.png",
                    "sna_item_hm.bmp.png",
                    "sna_mgs3_antena.bmp.png",
                    "sna_mgs3_arm.bmp.png",
                    "sna_mgs3_belt.bmp_77380cc45d92a52a1e8da9f59a6ea891.png",
                    "sna_mgs3_belt_side.bmp.png",
                    "sna_mgs3_gh.bmp.png",
                    "sna_mgs3_gun_hol.bmp.png",
                    "sna_mgs3_halo_tape.bmp.png",
                    "sna_mgs3_hh.bmp.png",
                    "sna_mgs3_musen.bmp.png",
                    "sna_mgs3_naked_belt.bmp_387276427ee88d88dbacbd0ae1f73fd7.png",
                    "sna_mgs3_teeth.bmp.png",
                    "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
                    "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
                    "sna_mtg_ovl_alp.bmp.png",
                    "sna_snif_def.bmp.png",
                    "svknf_grip.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 GRU")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 GRU");
                modelFile = "ene_defout.obj";
                mtlFile = "ene_defout.mtl";
                textureFiles = new string[]
                {
                    "ene_def_headtop.bmp.png",
                    "ene_def_headunder.bmp.png",
                    "ene_def_neck.bmp.png",
                    "ene_def_eye.bmp.png",
                    "ene_def_pa-ka.bmp.png",
                    "ene_def_body.bmp.png",
                    "ene_def_arm.bmp.png",
                    "ene_def_leg.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 KGB")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 KGB");
                modelFile = "ene_kgb.obj";
                mtlFile = "ene_kgb.mtl";
                textureFiles = new string[]
                {
                    "e_grene-do.bmp.png",
                    "e_musenki.bmp.png",
                    "e_musenki_belt.bmp.png",
                    "ene_flame_boots3.bmp.png",
                    "ene_flame_boots4.bmp.png",
                    "ene_kgb_bag.bmp.png",
                    "ene_kgb_best.bmp.png",
                    "ene_kgb_body.bmp.png",
                    "ene_kgb_boots1.bmp.png",
                    "ene_kgb_boots2.bmp.png",
                    "ene_kgb_eye_open.bmp.png",
                    "ene_kgb_face.bmp.png",
                    "ene_kgb_hand.bmp.png",
                    "ene_kgb_hankachi.bmp.png",
                    "ene_kgb_pa-ka.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Ocelot Unit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Ocelot Unit");
                modelFile = "ene_spe.obj";
                mtlFile = "ene_spe.mtl";
                textureFiles = new string[]
                {
                    "oce_def_bere.bmp.png",
                    "ene_spe_head.bmp.png",
                    "ene_spe_eye_open.bmp.png",
                    "ene_spe_body.bmp.png",
                    "ene_spe_hand.bmp.png",
                    "ene_spe_body_belt_alp_ovl.bmp.png",
                    "oce_def_body_belt.bmp.png",
                    "ene_spe_dr_magpouch.bmp.png",
                    "ene_spe_dr_magpouch2.bmp.png",
                    "oce_def_grene-do.bmp.png",
                    "ene_spe_stboxmagpouch.bmp.png",
                    "oce_def_makarofu.bmp.png",
                    "ene_spe_sspo-ti.bmp.png",
                    "ene_spe_mizu.bmp.png",
                    "ene_spe_boots1.bmp.png",
                    "ene_spe_boots2_ovl_sub_alp.bmp.png",
                    "ene_spe_boots3.bmp.png",
                    "ene_spe_boots4.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Officer")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Officer");
                modelFile = "ene_ind.obj";
                mtlFile = "ene_ind.mtl";
                textureFiles = new string[]
                {
                    "ene_ind_head_mark.bmp.png",
                    "ene_ind_head_belt.bmp.png",
                    "ene_ind_hat_def.bmp.png",
                    "ene_ind_head_front.bmp.png",
                    "ene_ind_face_a.bmp.png",
                    "ene_ind_eye_open_b.bmp.png",
                    "ene_ind_body.bmp.png",
                    "ene_ind_body_belt.bmp.png",
                    "ene_ind_body_belt_alp_ovl.bmp.png",
                    "ene_ind_hand.bmp.png",
                    "ene_ind_makarofu.bmp.png",
                    "ene_def_akmagpouch_bag.bmp.png",
                    "e_sspo-ti.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Snake Tanker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Snake Tanker");
                modelFile = "sna_def.obj";
                mtlFile = "sna_def.mtl";
                textureFiles = new string[]
                {
                    "sna_arm_ss.bmp.png",
                    "sna_belt_leg.bmp.png",
                    "sna_body_ss01dt.bmp.png",
                    "sna_foot01dt.bmp.png",
                    "sna_hair3.bmp.png",
                    "sna_hand1dt.bmp.png",
                    "sna_hand2.bmp.png",
                    "sna_hi_bdn01dt.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_face01dt.bmp.png",
                    "sna_hi_meziri.bmp.png",
                    "sna_leg_r01dt.bmp.png",
                    "sna_leg_ss.bmp.png",
                    "sna_m9_glip.bmp.png",
                    "sna_p1dt.bmp.png",
                    "sna_shoul01dt.bmp.png",
                    "sna_toe01dt.bmp.png",
                    "sna_toe02dt.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Raiden")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Raiden");
                modelFile = "rai_def_mt.obj";
                mtlFile = "rai_def_mt.mtl";
                textureFiles = new string[]
                {
                    "rai_arm_fix.bmp.png",
                    "rai_arm_fix_ovl_sub_alp.bmp.png",
                    "rai_body_01_fix.bmp.png",
                    "rai_body_01_fix_ovl_sub_alp.bmp.png",
                    "rai_body_02_fix.bmp.png",
                    "rai_body_02_fix_ovl_sub_alp.bmp.png",
                    "rai_face_dt01.bmp.png",
                    "rai_finger_fix.bmp.png",
                    "rai_finger_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_fix.bmp.png",
                    "rai_foot_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_01_fix.bmp.png",
                    "rai_hand_01_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_02_fix.bmp.png",
                    "rai_hand_02_fix_ovl_sub_alp.bmp.png",
                    "rai_leg_l_fix.bmp.png",
                    "rai_leg_l_fix_ovl_sub_alp.bmp.png",
                    "rai_leg_r_fix.bmp.png",
                    "rai_leg_r_fix_ovl_sub_alp.bmp.png",
                    "rai_rist_fix.bmp.png",
                    "rai_rist_fix_ovl_sub_alp.bmp.png",
                    "rai_toe01_fix.bmp.png",
                    "rai_toe02_fix.bmp.png",
                    "rai_toe_01_fix_ovl_sub_alp.bmp.png",
                    "rai_toe_02_fix_ovl_sub_alp.bmp.png",
                    "rai_watch_dt01.bmp.png",
                    "rai_watch_dt01_ovl_sub_alp.bmp.png",
                    "rai_watch_dt02.bmp.png",
                    "rai_watch_dt02_ovl_sub_alp.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Tanker Guards")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Tanker Guards");
                modelFile = "gbs_def.obj";
                mtlFile = "gbs_def.mtl";
                textureFiles = new string[]
                {
                    "gbs_face.bmp.png",
                    "gbs_head_top.bmp.png",
                    "gbs_head_side.bmp.png",
                    "gbs_head_back.bmp.png",
                    "gbs_eye_open.bmp.png",
                    "gbs_eri.bmp.png",
                    "gbs_body_ss.bmp.png",
                    "gbs_arm_ss.bmp.png",
                    "gbs_wrist.bmp.png",
                    "gbs_p2_fro.bmp.png",
                    "gbs_p2_side.bmp.png",
                    "gbs_belt.bmp.png",
                    "gbs_leg_ss.bmp.png",
                    "gbs_boot_ss.bmp.png",
                    "gbs_boot_bot1.bmp.png",
                    "gbs_boot_bot2.bmp.png",
                    "gbs_toe2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Big Shell Guards")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Big Shell Guards");
                modelFile = "gps_def_mt.obj";
                mtlFile = "gps_def_mt.mtl";
                textureFiles = new string[]
                {
                    "gps_mask.bmp.png",
                    "gps_eye_def.bmp.png",
                    "gps_eri.bmp.png",
                    "gps_arm_ss2.bmp.png",
                    "gps_body_ss.bmp.png",
                    "gps_leg_ss.bmp.png",
                    "gps_poket.bmp.png",
                    "gps_poket2.bmp.png",
                    "gps_boot_ss.bmp.png",
                    "gps_toe2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 NYPD")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 NYPD");
                modelFile = "NYPD (Armed).obj";
                mtlFile = "NYPD (Armed).mtl";
                textureFiles = new string[]
                {
                    "nyp_armd_body.bmp.png",
                    "nyp_armd_face.bmp.png",
                    "nyp_armd_helmet.bmp.png",
                    "nyp_armd_police.bmp.png",
                    "nypo_arm.bmp.png",
                    "nypo_emblem.bmp.png",
                    "nypo_folder.bmp.png",
                    "nypo_hand.bmp.png",
                    "nypo_leg.bmp.png",
                    "nypo_shoe.bmp.png",
                    "nypo_unit.bmp.png"
                };
            }
            else
            {
                MessageBox.Show("Unknown model selected.");
                return;
            }

            string folderPath = folder;
            currentModelPath = Path.Combine(folderPath, modelFile);
            currentMtlPath = Path.Combine(folderPath, mtlFile);
            if (File.Exists(currentMtlPath))
            {
                RestoreAllMtlReferencesToOriginal(currentMtlPath);
            }
            modelViewerControl.LoadModel(currentModelPath);

            int w = 335, h = 127;
            int xPos = panelTextures.ClientSize.Width - w - 40;
            int yPos = 10, spacing = 40;
            int labelHeight = 20;

            foreach (string tex in textureFiles)
            {
                string texPath = Path.Combine(folderPath, tex);

                string name = Path.GetFileNameWithoutExtension(tex);
                string resolution;
                using (var temp = LoadImageNoLock(texPath))
                {
                    resolution = temp != null
                        ? $"{temp.Width}×{temp.Height}"
                        : "Unknown";
                }

                Label lbl = new Label
                {
                    Text = $"{name}  {resolution}",
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(30, 30, 30),
                    Location = new Point(xPos, yPos),
                    Size = new Size(w, labelHeight)
                };
                panelTextures.Controls.Add(lbl);

                PictureBox pb = new PictureBox
                {
                    Location = new Point(xPos, yPos + labelHeight),
                    Size = new Size(w, h),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = texPath
                };

                Image img = LoadImageNoLock(texPath);
                if (img != null)
                {
                    pb.SizeMode = (img.Width < pb.Width && img.Height < pb.Height)
                        ? PictureBoxSizeMode.CenterImage
                        : PictureBoxSizeMode.Zoom;
                    pb.Image = img;
                }
                else
                {
                    pb.BackColor = Color.DarkRed;
                }
                panelTextures.Controls.Add(pb);

                Button btnChange = new Button
                {
                    Text = "Change Texture",
                    Location = new Point(xPos, yPos + labelHeight + h + 5),
                    Size = new Size(100, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnChange.Click += ChangeTexture_Click;
                panelTextures.Controls.Add(btnChange);

                Button btnRestore = new Button
                {
                    Text = "Restore Default",
                    Location = new Point(xPos + 110, yPos + labelHeight + h + 5),
                    Size = new Size(110, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnRestore.Click += RestoreOneTextureDefault_Click;
                panelTextures.Controls.Add(btnRestore);

                yPos += labelHeight + h + spacing + 30;
            }
        }

        private void ChangeTexture_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            PictureBox pb = (PictureBox)btn.Tag;
            string oldTexPath = pb.Tag.ToString();

            modelViewerControl.ClearModel();


            string newTexPath = GetNextSuffixPath(oldTexPath);

            using (OpenFileDialog dlg = new OpenFileDialog { Filter = "PNG Files|*.png|All Files|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        RenameMtlTextureReference(currentMtlPath,
                            Path.GetFileName(oldTexPath),
                            Path.GetFileName(newTexPath));

                        File.Copy(dlg.FileName, newTexPath, true);

                        pb.Tag = newTexPath;
                        pb.Image?.Dispose();
                        pb.Image = LoadImageNoLock(newTexPath);

                        modelViewerControl.LoadModel(currentModelPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error changing texture:\n" + ex.Message);
                    }
                }
            }
        }

        private void RestoreOneTextureDefault_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            PictureBox pb = (PictureBox)btn.Tag;
            string newTexPath = pb.Tag.ToString();
            string oldTexPath = RemoveSuffix(newTexPath);

            try
            {
                modelViewerControl.ClearModel();


                RenameMtlTextureReference(currentMtlPath,
                    Path.GetFileName(newTexPath),
                    Path.GetFileName(oldTexPath));

                pb.Tag = oldTexPath;
                pb.Image?.Dispose();
                pb.Image = LoadImageNoLock(oldTexPath);

                modelViewerControl.LoadModel(currentModelPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring default:\n" + ex.Message);
            }
        }

        private void RenameMtlTextureReference(string mtlPath, string oldName, string newName)
        {
            if (!File.Exists(mtlPath))
                return;
            string text = File.ReadAllText(mtlPath);
            if (text.Contains(oldName))
            {
                text = text.Replace(oldName, newName);
                File.WriteAllText(mtlPath, text);
            }
        }

        private string GetNextSuffixPath(string originalPath)
        {
            string dir = Path.GetDirectoryName(originalPath);
            string fNoExt = Path.GetFileNameWithoutExtension(originalPath);
            string ext = Path.GetExtension(originalPath);

            fNoExt = StripNumericSuffix(fNoExt);

            int i = 1;
            string candidate;
            while (true)
            {
                candidate = Path.Combine(dir, fNoExt + "_" + i + ext);
                if (!File.Exists(candidate))
                    break;
                i++;
                if (i > 999)
                    throw new IOException("Couldn't find new suffix name up to _999.");
            }
            return candidate;
        }

        private string StripNumericSuffix(string fNoExt)
        {
            int idx = fNoExt.LastIndexOf('_');
            if (idx < 0)
                return fNoExt;
            string suffix = fNoExt.Substring(idx + 1);
            if (suffix.All(char.IsDigit))
                return fNoExt.Substring(0, idx);
            return fNoExt;
        }

        private string RemoveSuffix(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileNoExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            fileNoExt = StripNumericSuffix(fileNoExt);
            return Path.Combine(dir, fileNoExt + ext);
        }

        private void RestoreAllMtlReferencesToOriginal(string mtlPath)
        {
            string text = File.ReadAllText(mtlPath);
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("map_Kd", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string oldFile = parts[1];
                        string dir = Path.GetDirectoryName(Path.Combine(Path.GetDirectoryName(mtlPath), oldFile));
                        string fixedPath = RemoveSuffix(Path.Combine(dir, oldFile));
                        string justFileName = Path.GetFileName(fixedPath);

                        lines[i] = parts[0] + " " + justFileName;
                    }
                }
            }
            text = string.Join(Environment.NewLine, lines);
            File.WriteAllText(mtlPath, text);
        }

        private Image LoadImageNoLock(string path)
        {
            if (!File.Exists(path))
                return null;
            using (var fs = File.OpenRead(path))
            {
                return Image.FromStream(fs);
            }
        }

        private void CtxrToPng_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing CTXR files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.ctxr");
                        foreach (string ctxrPath in files)
                        {
                            string pngPath = Path.ChangeExtension(ctxrPath, ".png");
                            try
                            {
                                CtxrConverter.CtxrToPng(ctxrPath, pngPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {ctxrPath}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select a CTXR file";
                    ofd.Filter = "CTXR Files|*.ctxr";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string ctxrPath = ofd.FileName;
                        string pngPath = Path.ChangeExtension(ctxrPath, ".png");
                        try
                        {
                            CtxrConverter.CtxrToPng(ctxrPath, pngPath);
                            MessageBox.Show($"Converted:\n{ctxrPath}\n→\n{pngPath}");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error: {ex.Message}");
                        }
                    }
                }
            }
        }

        private async void PngToCtxr_Click(object sender, EventArgs e)
        {
            var cfg = ConfigManager.LoadSettings();
            if (!CheckAndPromptForModToolsPath(cfg) ||
                !CheckAndPromptForGimpPythonScriptPath(cfg) ||
                !CheckAndPromptForPythonPath(cfg))
            {
                MessageBox.Show("Setup was cancelled or incomplete.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var pythonExe = cfg.PythonExePath;
            var gimpScript = cfg.GimpPythonScriptPath;
            var ctxrToolExe = Path.Combine(cfg.ModToolsPath, "CtxrTool.exe");

            DialogResult choice = MessageBox.Show(
                "Convert full folder? (Yes = folder, No = single file)",
                "PNG → CTXR Conversion",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );
            if (choice == DialogResult.Cancel) return;

            string[] pngFiles;
            if (choice == DialogResult.Yes)
            {
                using var fbd = new FolderBrowserDialog { Description = "Select folder of PNGs" };
                if (fbd.ShowDialog() != DialogResult.OK) return;
                pngFiles = Directory.GetFiles(fbd.SelectedPath, "*.png");
                if (pngFiles.Length == 0)
                {
                    MessageBox.Show("No PNGs found.", "Nothing to do",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                using var ofd = new OpenFileDialog { Filter = "PNG Files|*.png" };
                if (ofd.ShowDialog() != DialogResult.OK) return;
                pngFiles = new[] { ofd.FileName };
            }

            using var progressForm = new Form
            {
                Text = "Converting PNG → CTXR",
                Width = 400,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };
            var bar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = pngFiles.Length,
                Value = 0
            };
            progressForm.Controls.Add(bar);
            progressForm.Show(this);

            for (int i = 0; i < pngFiles.Length; i++)
            {
                var png = pngFiles[i];
                var dds = Path.ChangeExtension(png, ".dds");

                try
                {
                    var psi = new ProcessStartInfo(pythonExe,
                        $"\"{gimpScript}\" \"{png}\" \"{dds}\"")
                    {
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    };
                    using var proc = Process.Start(psi);
                    var err = proc?.StandardError.ReadToEnd();
                    proc?.WaitForExit();

                    if (proc?.ExitCode != 0 || !File.Exists(dds))
                        Debug.WriteLine($"PNG→DDS failed for {Path.GetFileName(png)}: {err}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"PNG→DDS exception for {Path.GetFileName(png)}: {ex.Message}");
                }

                if (File.Exists(dds))
                {
                    try
                    {
                        CtxrConverter.DdsToCtxr(dds, ctxrToolExe);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"DDS→CTXR exception for {Path.GetFileName(dds)}: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine($"Skipping CTXR: .dds missing for {Path.GetFileName(png)}");
                }

                bar.Value = i + 1;
                Application.DoEvents();
            }

            progressForm.Close();

            MessageBox.Show(
                $"Conversion complete:\nProcessed {pngFiles.Length} file{(pngFiles.Length > 1 ? "s" : "")}.",
                "Done",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private async void PngToDds_Click(object sender, EventArgs e)
        {
            var cfg = ConfigManager.LoadSettings();
            if (!CheckAndPromptForModToolsPath(cfg) ||
                !CheckAndPromptForGimpPythonScriptPath(cfg) ||
                !CheckAndPromptForPythonPath(cfg))
            {
                MessageBox.Show("Setup was cancelled or incomplete.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var pythonExe = cfg.PythonExePath;
            var script = cfg.GimpPythonScriptPath;

            var choice = MessageBox.Show(
                "Convert full folder? (Yes = folder, No = single file)",
                "PNG → DDS (via Python)",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );
            if (choice == DialogResult.Cancel)
                return;

            string[] files;
            if (choice == DialogResult.Yes)
            {
                using var fbd = new FolderBrowserDialog { Description = "Select folder of PNGs" };
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                files = Directory.GetFiles(fbd.SelectedPath, "*.png");
                if (files.Length == 0)
                {
                    MessageBox.Show("No PNGs found in that folder.", "Nothing to do",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            else
            {
                using var ofd = new OpenFileDialog { Filter = "PNG Files|*.png" };
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                files = new[] { ofd.FileName };
            }

            using var progressForm = new Form
            {
                Text = "Converting PNG → DDS",
                Width = 400,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };
            var bar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = files.Length,
                Value = 0,
            };
            progressForm.Controls.Add(bar);
            progressForm.Show(this);

            for (int i = 0; i < files.Length; i++)
            {
                var inPng = files[i];
                var outDds = Path.ChangeExtension(inPng, ".dds");

                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = pythonExe,
                        Arguments = $"\"{script}\" \"{inPng}\" \"{outDds}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardError = true
                    };
                    using var proc = Process.Start(psi);
                    var err = proc?.StandardError.ReadToEnd();
                    proc?.WaitForExit();

                    if (!File.Exists(outDds))
                        Debug.WriteLine($"Failed {Path.GetFileName(inPng)}: {err}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception on {Path.GetFileName(inPng)}: {ex.Message}");
                }

                bar.Value = i + 1;
                Application.DoEvents();
            }

            progressForm.Close();

            MessageBox.Show(
                $"Conversion complete:\nProcessed {files.Length} file{(files.Length > 1 ? "s" : "")}.",
                "Done",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void DdsToCtxr_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            if (!CheckAndPromptForModToolsPath(config))
            {
                MessageBox.Show("Mod tools folder setup was cancelled.");
                return;
            }

            string modToolsPath = config.ModToolsPath;
            string ctxrToolExe = Path.Combine(modToolsPath, "CtxrTool.exe");

            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing DDS files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.dds");
                        foreach (string ddsPath in files)
                        {
                            try
                            {
                                CtxrConverter.DdsToCtxr(ddsPath, ctxrToolExe);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {ddsPath}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Filter = "DDS Files|*.dds" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string ddsPath = ofd.FileName;
                        try
                        {
                            CtxrConverter.DdsToCtxr(ddsPath, ctxrToolExe);
                            MessageBox.Show("CTXR created successfully in the same folder as the DDS file.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error converting DDS to CTXR: {ex.Message}");
                        }
                    }
                }
            }
        }

        private void btnLoadObj_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "3D Model Files|*.fbx;*.obj;*.dae|All Files|*.*"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    modelViewerControl.LoadModel(ofd.FileName);
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            ReturnToMainMenu();
        }

        private async void CreateModButton_Click(object sender, EventArgs e)
        {
            string modName = PromptForInput("Enter mod name (required):", "New Mod");
            if (string.IsNullOrWhiteSpace(modName))
            {
                MessageBox.Show("Mod name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string modImagePath = null;
            if (MessageBox.Show("Do you want to include a picture for your mod?", "Mod Picture",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                using var ofd = new OpenFileDialog
                {
                    Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp",
                    Title = "Select a mod picture"
                };
                if (ofd.ShowDialog() == DialogResult.OK)
                    modImagePath = ofd.FileName;
            }

            string modDescription = PromptForInput("Enter mod description (optional):", "Mod Description");

            var cfg = ConfigManager.LoadSettings();
            string selectedModel = ModelSelectionComboBox.SelectedItem?.ToString() ?? "";
            bool isMgs2 = selectedModel.StartsWith("MGS2", StringComparison.OrdinalIgnoreCase);
            string baseModsPath = isMgs2 ? cfg.MGS2ModFolderPath : cfg.MGS3ModFolderPath;

            string modFolder = Path.Combine(baseModsPath, modName);
            Directory.CreateDirectory(modFolder);
            string detailsFolder = Path.Combine(modFolder, "Mod Details");
            Directory.CreateDirectory(detailsFolder);

            if (!string.IsNullOrEmpty(modImagePath))
                File.Copy(modImagePath, Path.Combine(detailsFolder, "Mod Image.png"), true);

            if (!string.IsNullOrEmpty(modDescription))
                File.WriteAllText(Path.Combine(detailsFolder, "Mod Info.txt"), modDescription);

            if (!CheckAndPromptForModToolsPath(cfg) ||
                !CheckAndPromptForGimpPythonScriptPath(cfg) ||
                !CheckAndPromptForPythonPath(cfg))
            {
                MessageBox.Show("Setup was cancelled or incomplete.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string pythonExe = cfg.PythonExePath;
            string gimpScript = cfg.GimpPythonScriptPath;
            string ctxrToolExe = Path.Combine(cfg.ModToolsPath, "CtxrTool.exe");

            string conversionFolder = Path.Combine(modFolder, modName, "textures", "flatlist", "ovr_stm", "_win");
            Directory.CreateDirectory(conversionFolder);

            using var progressForm = new Form
            {
                Text = "Building Mod Textures",
                Width = 400,
                Height = 100,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent
            };
            var bar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Minimum = 0,
                Maximum = panelTextures.Controls.OfType<PictureBox>()
                          .Count(pb => Path.GetExtension(pb.Tag.ToString()).Equals(".png", StringComparison.OrdinalIgnoreCase)),
                Value = 0
            };
            progressForm.Controls.Add(bar);
            progressForm.Show(this);

            foreach (Control ctrl in panelTextures.Controls)
            {
                if (ctrl is PictureBox pb && pb.Tag is string pngPath
                    && Path.GetExtension(pngPath).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    string ddsPath = Path.ChangeExtension(pngPath, ".dds");
                    try
                    {
                        var psi = new ProcessStartInfo(pythonExe,
                            $"\"{gimpScript}\" \"{pngPath}\" \"{ddsPath}\"")
                        {
                            UseShellExecute = false,
                            CreateNoWindow = true,
                            RedirectStandardError = true
                        };
                        using var proc = Process.Start(psi);
                        string err = proc.StandardError.ReadToEnd();
                        proc.WaitForExit();
                        if (proc.ExitCode != 0 || !File.Exists(ddsPath))
                            Debug.WriteLine($"PNG→DDS failed: {err}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"PNG→DDS exception: {ex.Message}");
                    }

                    if (File.Exists(ddsPath))
                    {
                        try { CtxrConverter.DdsToCtxr(ddsPath, ctxrToolExe); }
                        catch (Exception ex) { Debug.WriteLine($"DDS→CTXR exception: {ex.Message}"); }
                    }

                    string ctxrPath = Path.ChangeExtension(pngPath, ".ctxr");
                    if (File.Exists(ctxrPath))
                    {
                        string cleanName = Path.GetFileName(RemoveSuffix(ctxrPath));
                        File.Copy(ctxrPath, Path.Combine(conversionFolder, cleanName), true);
                    }

                    bar.Value++;
                    Application.DoEvents();
                }
            }

            progressForm.Close();

            if (cfg.Mods.ActiveMods.ContainsKey(modName))
                cfg.Mods.ActiveMods[modName] = true;
            else
                cfg.Mods.ActiveMods.Add(modName, true);
            ConfigManager.SaveSettings(cfg);

            string gameLabel = isMgs2 ? "MGS2" : "MGS3";
            MessageBox.Show($"Mod created successfully in the {gameLabel} Mods folder.",
                            "Mod Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private string PromptForInput(string prompt, string title)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Width = 500;
                inputForm.Height = 150;
                inputForm.Text = title;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.StartPosition = FormStartPosition.CenterScreen;
                inputForm.MinimizeBox = false;
                inputForm.MaximizeBox = false;

                Label lblPrompt = new Label() { Left = 50, Top = 20, Text = prompt, AutoSize = true };
                TextBox txtInput = new TextBox() { Left = 50, Top = 50, Width = 400 };
                Button btnOk = new Button() { Text = "OK", Left = 350, Width = 100, Top = 80, DialogResult = DialogResult.OK };

                btnOk.Click += (sender, e) => { inputForm.Close(); };

                inputForm.Controls.Add(lblPrompt);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOk);
                inputForm.AcceptButton = btnOk;

                return inputForm.ShowDialog() == DialogResult.OK ? txtInput.Text.Trim() : "";
            }
        }

        
    }
}