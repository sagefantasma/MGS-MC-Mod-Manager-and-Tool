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
        "MGS3 Snake SE","MGS3 Snake Sneaking Suit", "MGS3 Tanya (Eva)", "MGS3 Raikov", "MGS3 GRU", "MGS3 KGB", "MGS3 Ocelot Unit", "MGS3 Officer", "MGS3 Scientist",
        "MGS2 Snake Tanker", "MGS2 Pliskin", "MGS2 Tuxedo Snake", "MGS2 Snake (MGS1)", "MGS2 Raiden", "MGS2 Raiden Ninja", "MGS2 Raiden Scuba","MGS2 Tanker Guards", "MGS2 Big Shell Guards", "MGS2 Cypher", "MGS2 Ames", "MGS2 Marine", "MGS2 Meryl", "MGS2 Ocelot", "MGS2 Olga Ninja", "MGS2 Olga Plant", "MGS2 Olga Tanker", "MGS2 Otacon", "MGS2 Scott Dolph", "MGS2 Seal", "MGS2 Solidus", "MGS2 Stillman", "MGS2 Fatman Bombs", "MGS2 Directional Microphone", "MGS2 Item Box 1", "MGS2 Item Box 2", "MGS2 M4", "MGS2 M9", "MGS2 Coolant Spray", "MGS2 Socom", "MGS2 SAA", "MGS2 USP"

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
            else if (selectedModel == "MGS3 Snake Sneaking Suit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Sneaking Suit");
                modelFile = "Snake.obj";
                mtlFile = "Snake.mtl";
                textureFiles = new string[]
                {
                    "cqcc_tex02.bmp.png",
                    "cqck_tex02.bmp.png",
                    "sna_bandana_def.bmp.png",
                    "sna_bk.bmp.png",
                    "sna_def_hair_base.bmp.png",
                    "sna_def_hair_front_ovl_alp.bmp.png",
                    "sna_def_vr_eye.bmp.png",
                    "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
                    "sna_hair_back_ovl_alp.bmp.png",
                    "sna_hair_front_ovl_alp.bmp.png",
                    "sna_hair_layer_ovl_alp.bmp.png",
                    "sna_item_hm.bmp.png",
                    "sna_mgs3_antena.bmp.png",
                    "sna_mgs3_belt_side.bmp.png",
                    "sna_mgs3_gantai.bmp.png",
                    "sna_mgs3_gh.bmp.png",
                    "sna_mgs3_gun_hol.bmp.png",
                    "sna_mgs3_halo_tape.bmp.png",
                    "sna_mgs3_hh.bmp.png",
                    "sna_mgs3_musen.bmp.png",
                    "sna_mgs3_teeth.bmp.png",
                    "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
                    "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
                    "sna_mtg_ovl_alp.bmp.png",
                    "sna_naked_cord_ovl_alp.bmp.png",
                    "sna_snif_def.bmp.png",
                    "sna_ss_arm.bmp.png",
                    "sna_ss_belt_body.bmp.png",
                    "sna_ss_belt_crotch.bmp.png",
                    "sna_ss_belt_waist.bmp.png",
                    "sna_ss_body.bmp.png",
                    "sna_ss_boots.bmp.png",
                    "sna_ss_finger.bmp.png",
                    "sna_ss_knee.bmp.png",
                    "sna_ss_rope_ovl_alp.bmp.png",
                    "sna_ss_shoulder_new.bmp.png",
                    "sna_ss_side.bmp.png",
                    "sna_ss_text_ovl_alp.bmp",
                    "sna_ss_thigh.bmp.png",
                    "svknf_grip.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Tanya (Eva)")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Tanya (Eva)");
                modelFile = "TanyaEva.obj";
                mtlFile = "TanyaEva.mtl";
                textureFiles = new string[]
                {
                    "eve_def_skin_fix.bmp.png",
                    "eve_def_teeth.bmp.png",
                    "eve_eye_new.bmp.png",
                    "eve_eyelashes_fix_ovl_alp.bmp.png",
                    "eve_item_glasses_frame.bmp.png",
                    "eve_item_glasses_ovl_alp.bmp.png",
                    "eve_tac_arm_new.bmp.png",
                    "eve_tac_badge_ovl_alp.bmp.png",
                    "eve_tac_body_new.bmp.png",
                    "eve_tac_boots_new.bmp.png",
                    "eve_tac_button_ovl_alp.bmp.png",
                    "eve_tac_face_fix_new.bmp.png",
                    "eve_tac_hair_back_all_ovl_alp.bmp.png",
                    "eve_tac_hair_fro_ovl_alp.bmp.png",
                    "eve_tac_hand_new_fix.bmp.png",
                    "eve_tac_leg_left.bmp.png",
                    "eve_tac_leg_right.bmp.png",
                    "eve_tac_nail_ovl_alp.bmp.png",
                    "eve_tac_pants.bmp.png",
                    "eve_tac_star_ovl_alp.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 GRU")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 GRU");
                modelFile = "ene_defout.obj";
                mtlFile = "ene_defout.mtl";
                textureFiles = new string[]
                {
                    "e_grene-do.bmp.png",
                    "e_magpo-ti.bmp.png",
                    "e_map.bmp.png",
                    "e_musenki.bmp.png",
                    "e_musenki_belt.bmp.png",
                    "e_pa.bmp.png",
                    "e_rig.bmp.png",
                    "e_rigbelt_alp_ovl.bmp.png",
                    "e_skoppu.bmp.png",
                    "e_sspo-ti.bmp.png",
                    "e_tento.bmp.png",
                    "e_tentobelto_alp_ovl.bmp.png",
                    "ene_def_akmagpouch_bag.bmp.png",
                    "ene_def_arm.bmp.png",
                    "ene_def_body.bmp.png",
                    "ene_def_eye_open.bmp.png",
                    "ene_def_headtop.bmp.png",
                    "ene_def_headunder.bmp.png",
                    "ene_def_leg.bmp.png",
                    "ene_def_neck.bmp.png",
                    "ene_def_pa-ka.bmp.png",
                    "ene_flame_boots3.bmp.png",
                    "ene_flame_boots4.bmp.png",
                    "ene_kgb_boots1.bmp.png",
                    "ene_kgb_boots2.bmp.png",
                    "ene_kgb_hand.bmp.png"
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
            else if (selectedModel == "MGS3 Raikov")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Raikov");
                modelFile = "raikov.obj";
                mtlFile = "raikov.mtl";
                textureFiles = new string[]
                {
                    "ene_ind_makarofu.bmp.png",
                    "ene_spe_boots1.bmp.png",
                    "ene_spe_boots4.bmp.png",
                    "ivn_arm_def.bmp.png",
                    "ivn_belt.bmp.png",
                    "ivn_belt_shl_ovl_alp.bmp.png",
                    "ivn_body_def.bmp.png",
                    "ivn_body_neck_suit.bmp.png",
                    "ivn_body_suit_bh_ovl_alp.bmp.png",
                    "ivn_def_boots_c.bmp.png",
                    "ivn_face_def.bmp.png",
                    "ivn_foot_def.bmp.png",
                    "ivn_hair_base_ovl_alp.bmp.png",
                    "ivn_hair_layer_ovl_alp.bmp.png",
                    "ivn_hand_def_hi.bmp.png",
                    "ivn_hat_def.bmp.png",
                    "ivn_head_belt.bmp.png",
                    "ivn_head_front.bmp.png",
                    "ivn_head_mark.bmp.png",
                    "ivn_mgs3_body_under_suit.bmp.png",
                    "ivn_mtg_alp_ovl.bmp.png",
                    "oce_def_boots2.bmp.png",
                    "sna_def_vr_eye.bmp.png",
                    "sna_mgs3_teeth.bmp.png",
                    "thu_belt_ovl_alp.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Scientist")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Scientist");
                modelFile = "Scientist.obj";
                mtlFile = "Scientist.mtl";
                textureFiles = new string[]
                {
                    "wor_def_armband_alp_ovl.bmp.png",
                    "wor_def_boots1.bmp.png",
                    "wor_def_boots2.bmp.png",
                    "wor_def_boots3.bmp.png",
                    "wor_def_face_e.bmp.png",
                    "wor_def_grass_ovl_alp.bmp.png",
                    "wor_def_hand.bmp.png",
                    "wor_def_leg.bmp.png",
                    "wor_def_pen.bmp.png",
                    "wor_def_shatu.bmp.png",
                    "wor_def_shurts.bmp.png",
                    "wor_def_staffproof.bmp.png",
                    "wor_def_white.bmp.png",
                    "wor_sam_eye_def_a.bmp.png",
                };
            }
            else if (selectedModel == "MGS2 Snake Tanker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Snake Tanker");
                modelFile = "sna_def.obj";
                mtlFile = "sna_def.mtl";
                textureFiles = new string[]
                {
                    "sna_hair3.bmp.png",
                    "sna_hi_bdn01dt.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "sna_hi_meziri.bmp.png",
                    "sna_hi_face01dt.bmp.png",
                    "sna_hi_face_ovl_sub_alp.bmp.png",
                    "sna_body_ss01dt.bmp.png",
                    "sna_body01dt_ovl_sub_alp.bmp.png",
                    "sna_p1dt.bmp.png",
                    "sna_shoul01dt.bmp.png",
                    "sna_shoul01dt_ovl_sub_alp.bmp.png",
                    "sna_arm_ovl_sub_alp.bmp.png",
                    "sna_arm_ss.bmp.png",
                    "sna_hand1dt.bmp.png",
                    "sna_hand2.bmp.png",
                    "sna_leg_ovl_sub_alp.bmp.png",
                    "sna_leg_r01dt.bmp.png",
                    "sna_leg_r01dt_ovl_sub_alp.bmp.png",
                    "sna_leg_ss.bmp.png",
                    "sna_belt_leg.bmp.png",
                    "sna_m9_glip.bmp.png",
                    "sna_foot01dt.bmp.png",
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
                    "rai_face_dt01.bmp.png",
                    "rai_body_01_fix.bmp.png",
                    "rai_body_01_fix_ovl_sub_alp.bmp.png",
                    "rai_body_02_fix.bmp.png",
                    "rai_body_02_fix_ovl_sub_alp.bmp.png",
                    "rai_finger_fix.bmp.png",
                    "rai_finger_fix_ovl_sub_alp.bmp.png",
                    "rai_arm_fix.bmp.png",
                    "rai_arm_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_01_fix.bmp.png",
                    "rai_hand_01_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_02_fix.bmp.png",
                    "rai_hand_02_fix_ovl_sub_alp.bmp.png",
                    "rai_rist_fix.bmp.png",
                    "rai_rist_fix_ovl_sub_alp.bmp.png",
                    "rai_watch_dt01.bmp.png",
                    "rai_watch_dt01_ovl_sub_alp.bmp.png",
                    "rai_watch_dt02.bmp.png",
                    "rai_watch_dt02_ovl_sub_alp.bmp.png",
                    "rai_leg_l_fix.bmp.png",
                    "rai_leg_l_fix_ovl_sub_alp.bmp.png",
                    "rai_leg_r_fix.bmp.png",
                    "rai_leg_r_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_fix.bmp.png",
                    "rai_foot_fix_ovl_sub_alp.bmp.png",
                    "rai_toe01_fix.bmp.png",
                    "rai_toe02_fix.bmp.png",
                    "rai_toe_01_fix_ovl_sub_alp.bmp.png",
                    "rai_toe_02_fix_ovl_sub_alp.bmp.png"

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
            else if (selectedModel == "MGS2 Ames")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ames");
                modelFile = "ric_def_sh.obj";
                mtlFile = "ric_def_sh.mtl";
                textureFiles = new string[]
                {
                    "hos_arm01.bmp.png",
                    "hos_body01.bmp.png",
                    "hos_leg01.bmp.png",
                    "hos_shoe01.bmp.png",
                    "ric_hair01.bmp.png",
                    "ric_hi_eye.bmp.png",
                    "ric_hi_face.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Cardboard Box")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Cardboard Box");
                modelFile = "cardboard.obj";
                mtlFile = "cardboard.mtl";
                textureFiles = new string[]
                {
                    "cbx_a.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Coolant Spray")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Coolant Spray");
                modelFile = "cls_sub.obj";
                mtlFile = "cls_sub.mtl";
                textureFiles = new string[]
                {
                    "cls_02.bmp.png",
                    "cls_belt.bmp.png",
                    "cls_bt1.bmp.png",
                    "cls_btr.bmp.png",
                    "cls_noz.bmp.png",
                    "cls_sb_bt2.bmp.png",
                    "cls_sb_bt3.bmp.png",
                    "cls_sb_dai.bmp.png",
                    "cls_sb_grip.bmp.png",
                    "cls_sb_pi.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Cypher")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Cypher");
                modelFile = "cyp_sh.obj";
                mtlFile = "cyp_sh.mtl";
                textureFiles = new string[]
                {
                    "cyp_body_hg.bmp.png",
                    "cyp_face.bmp.png",
                    "cyp_head.bmp.png",
                    "cyp_propera_alp.bmp.png",
                    "cyp_roter01_alp.bmp.png",
                    "cyp_tank.bmp.png",
                    "cyp_temp_tx.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Directional Microphone")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Directional Microphone");
                modelFile = "dmp_sub.obj";
                mtlFile = "dmp_sub.mtl";
                textureFiles = new string[]
                {
                    "bsn3.bmp.png",
                    "dmp_sb2.bmp.png",
                    "dmp_sb2_box.bmp.png",
                    "dmp_sb_body.bmp.png",
                    "dmp_sb_grip.bmp.png",
                    "dmp_sb_mc.bmp.png",
                    "dmp_sb_mc2.bmp.png",
                    "dmp_sb_pi.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Fatman Bombs")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Fatman Bombs");
                modelFile = "c4_kaitai_a1.obj";
                mtlFile = "c4_kaitai_a1.mtl";
                textureFiles = new string[]
                {
                    "c4_a_antena.bmp.png",
                    "c4_a_beruto2_msk.bmp.png",
                    "c4_a_beruto3_msk.bmp.png",
                    "c4_a_hontai_10.bmp.png",
                    "c4_a_hontai_4.bmp.png",
                    "c4_a_sen1_alp.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Item Box 1")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Item Box 1");
                modelFile = "box_ibox.obj";
                mtlFile = "box_ibox.mtl";
                textureFiles = new string[]
                {
                    "ibox_all1.bmp.png",
                    "ibox_all4.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Item Box 2")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Item Box 2");
                modelFile = "box2_ibox.obj";
                mtlFile = "box2_ibox.mtl";
                textureFiles = new string[]
                {
                    "ibox_all3.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 M4")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 M4");
                modelFile = "m4a_nm.obj";
                mtlFile = "m4a_nm.mtl";
                textureFiles = new string[]
                {
                    "m4a_all_t.bmp.png",
                    "m4a_gl_unit_2_01.bmp.png",
                    "m4a_t_msk.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 M9")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 M9");
                modelFile = "m92_sub.obj";
                mtlFile = "m92_sub.mtl";
                textureFiles = new string[]
                {
                    "m92_jyuukou.bmp.png",
                    "m92_laserpoint_f1.bmp.png",
                    "m92_laserpointer.bmp.png",
                    "m92_pointer.bmp.png",
                    "m92_sb_all.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Marine")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Marine");
                modelFile = "us_def_1.obj";
                mtlFile = "us_def_1.mtl";
                textureFiles = new string[]
                {
                    "sco_boot_soko.bmp.png",
                    "usa_arm01dt.bmp.png",
                    "usa_belt01dt.bmp.png",
                    "usa_belt_back01dt.bmp.png",
                    "usa_boots01dt.bmp.png",
                    "usa_face_c01dt.bmp.png",
                    "usa_leg01dt.bmp.png",
                    "usm_chest01dt.bmp.png",
                    "usm_chest_back01dt.bmp.png",
                    "usm_eri01dt.bmp.png",
                    "usm_t_shirt.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Meryl")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Meryl");
                modelFile = "mrl_def_sh_mt.obj";
                mtlFile = "mrl_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "cam_eyes_ovl_sub_alp.bmp.png",
                    "mrk_hlst000.bmp.png",
                    "mrl_arm_sub_ovl_alp.bmp.png",
                    "mrl_arm_tes002.bmp.png",
                    "mrl_arm_wrist_sub_ovl_alp.bmp.png",
                    "mrl_belt.bmp.png",
                    "mrl_belt_sub_alp_ovl.bmp.png",
                    "mrl_boot_003.bmp.png",
                    "mrl_boot_D_sub_alp_ovl.bmp.png",
                    "mrl_boot_T_0006.bmp.png",
                    "mrl_boot_T_sub_alp_ovl.bmp.png",
                    "mrl_boot_bottom.bmp.png",
                    "mrl_chest.bmp.png",
                    "mrl_combat_knife.bmp.png",
                    "mrl_combat_knife2.bmp.png",
                    "mrl_face001.bmp.png",
                    "mrl_face_sub_ovl_alp.bmp.png",
                    "mrl_hair_d_new4.bmp.png",
                    "mrl_hair_t.bmp.png",
                    "mrl_hair_top_sub_ovl_alp.bmp.png",
                    "mrl_hand_side.bmp.png",
                    "mrl_hand_top.bmp.png",
                    "mrl_hi_eye2.bmp.png",
                    "mrl_hiza_sub_alp_ovl.bmp.png",
                    "mrl_leg_l_sub_ovl_alp.bmp.png",
                    "mrl_leg_tes002.bmp.png",
                    "mrl_leg_tes02_r.bmp.png",
                    "mrl_leg_tes04.bmp.png",
                    "mrl_tatoo.bmp.png",
                    "mrl_tatoo_sub_ovl_alp.bmp.png",
                    "mrl_waist_mag.bmp.png",
                    "mrl_waist_pack.bmp.png",
                    "mrl_wrist.bmp.png",
                    "orp_hand_sam.bmp.png",
                    "orp_hf00_st.bmp.png",
                    "orp_hf01_st.bmp.png"
                    };
            }
            else if (selectedModel == "MGS2 Ocelot")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ocelot");
                modelFile = "rev_plant_sh_mt.obj";
                mtlFile = "rev_plant_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "rev_boot_02dt.bmp.png",
                    "rev_boot_03dt.bmp.png",
                    "rev_bullet_01dt.bmp.png",
                    "rev_bullet_02dt.bmp.png",
                    "rev_face_01dt.bmp.png",
                    "rev_gunbelt_01dt.bmp.png",
                    "rev_hair1.bmp.png",
                    "rev_hair2.bmp.png",
                    "rev_hand_01dt.bmp.png",
                    "rev_holster_01dt.bmp.png",
                    "rev_pla_leg_ovl_sub_alp.bmp.png",
                    "rev_plant_arm_ovl_sub_alp.bmp.png",
                    "rev_plant_arm_ss.bmp.png",
                    "rev_plant_body_ovl_sub_alp.bmp.png",
                    "rev_plant_body_ss.bmp.png",
                    "rev_plant_boot_01dt.bmp.png",
                    "rev_plant_eri.bmp.png",
                    "rev_plant_jacket.bmp.png",
                    "rev_plant_leg_ss.bmp.png",
                    "rev_plant_obi.bmp.png",
                    "rev_plant_shirt.bmp.png",
                    "saa_01dt.bmp.png"
                };
            }
            /*MGS2 Olga Ninja:

Model Data:
org_tng_sh_mt.obj
org_tng_sh_mt.mtl

Texture Names:
orn_arm_dt01.bmp.png
orn_arm_dt01_sub_alp_ovl.bmp.png
orn_arm_dt02.bmp.png
orn_arm_dt02_sub_alp_ovl.bmp.png
orn_arm_dt03.bmp.png
orn_arm_dt03_sub_alp_ovl.bmp.png
orn_arm_dt04.bmp.png
orn_arm_dt04_sub_alp_ovl.bmp.png
orn_backb_dt01_sub_alp_ovl.bmp.png
orn_backb_dt02_sub_alp_ovl.bmp.png
orn_backb_dt03_sub_alp_ovl.bmp.png
orn_backbone_dt01.bmp.png
orn_backbone_dt02.bmp.png
orn_backbone_dt03.bmp.png
orn_body_dt01.bmp.png
orn_body_dt01_sub_alp_ovl.bmp.png
orn_elbow_dt01.bmp.png
orn_elbow_dt01_sub_alp_ovl.bmp.png
orn_finger_dt00.bmp.png
orn_finger_dt00_sub_alp_ovl.bmp.png
orn_finger_dt01.bmp.png
orn_finger_dt02.bmp.png
orn_finger_dt02_sub_alp_ovl.bmp.png
orn_foot_dt00.bmp.png
orn_foot_dt00_sub_alp_ovl.bmp.png
orn_foot_dt01.bmp.png
orn_foot_dt01_sub_alp_ovl.bmp.png
orn_foot_dt02.bmp.png
orn_foot_dt02_sub_alp_ovl.bmp.png
orn_foot_dt03.bmp.png
orn_foot_dt03_sub_alp_ovl.bmp.png
orn_hand_dt00.bmp.png
orn_hand_dt00_sub_alp_ovl.bmp.png
orn_hand_dt01.bmp.png
orn_hand_dt01_sub_alp_ovl.bmp.png
orn_hand_dt02.bmp.png
orn_hand_dt02_sub_alp_ovl.bmp.png
orn_helmet_dt01.bmp.png
orn_helmet_dt01_sub_alp_ovl.bmp.png
orn_helmet_dt02.bmp.png
orn_helmet_dt02_sub_alp_ovl.bmp.png
orn_knee_dt01.bmp.png
orn_knee_dt01_sub_alp_ovl.bmp.png
orn_leg02_sub_alp_ovl.bmp.png
orn_leg_dt00.bmp.png
orn_leg_dt00_sub_alp_ovl.bmp.png
orn_leg_dt01.bmp.png
orn_leg_dt01_sub_alp_ovl.bmp.png
orn_leg_dt02.bmp.png
orn_leg_dt03.bmp.png
orn_leg_dt03_sub_alp_ovl.bmp.png
orn_leg_dt04.bmp.png
orn_leg_dt04_sub_alp_ovl.bmp.png
orn_neck_dt01.bmp.png
orn_neck_sub_alp_ovl.bmp.png
orn_should_dt01.bmp.png
orn_should_dt01_sub_alp_ovl.bmp.png
orn_should_dt02.bmp.png
orn_should_dt02_sub_alp_ovl.bmp.png*/
                // MGS2 Olga Ninja
            else if (selectedModel == "MGS2 Olga Ninja")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Olga Ninja");
                modelFile = "org_tng_sh_mt.obj";
                mtlFile = "org_tng_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "orn_arm_dt01.bmp.png",
                    "orn_arm_dt01_sub_alp_ovl.bmp.png",
                    "orn_arm_dt02.bmp.png",
                    "orn_arm_dt02_sub_alp_ovl.bmp.png",
                    "orn_arm_dt03.bmp.png",
                    "orn_arm_dt03_sub_alp_ovl.bmp.png",
                    "orn_arm_dt04.bmp.png",
                    "orn_arm_dt04_sub_alp_ovl.bmp.png",
                    "orn_backb_dt01_sub_alp_ovl.bmp.png",
                    "orn_backb_dt02_sub_alp_ovl.bmp.png",
                    "orn_backb_dt03_sub_alp_ovl.bmp.png",
                    "orn_backbone_dt01.bmp.png",
                    "orn_backbone_dt02.bmp.png",
                    "orn_backbone_dt03.bmp.png",
                    "orn_body_dt01.bmp.png",
                    "orn_body_dt01_sub_alp_ovl.bmp.png",
                    "orn_elbow_dt01.bmp.png",
                    "orn_elbow_dt01_sub_alp_ovl.bmp.png",
                    "orn_finger_dt00.bmp.png",
                    "orn_finger_dt00_sub_alp_ovl.bmp.png",
                    "orn_finger_dt01.bmp.png",
                    "orn_finger_dt02.bmp.png",
                    "orn_finger_dt02_sub_alp_ovl.bmp.png",
                    "orn_foot_dt00.bmp.png",
                    "orn_foot_dt00_sub_alp_ovl.bmp.png",
                    "orn_foot_dt01.bmp.png",
                    "orn_foot_dt01_sub_alp_ovl.bmp.png",
                    "orn_foot_dt02.bmp.png",
                    "orn_foot_dt02_sub_alp_ovl.bmp.png",
                    "orn_foot_dt03.bmp.png",
                    "orn_foot_dt03_sub_alp_ovl.bmp.png",
                    "orn_hand_dt00.bmp.png",
                    "orn_hand_dt00_sub_alp_ovl.bmp.png",
                    "orn_hand_dt01.bmp.png",
                    "orn_hand_dt01_sub_alp_ovl.bmp.png",
                    "orn_hand_dt02.bmp.png",
                    "orn_hand_dt02_sub_alp_ovl.bmp.png",
                    "orn_helmet_dt01.bmp.png",
                    "orn_helmet_dt01_sub_alp_ovl.bmp.png",
                    "orn_helmet_dt02.bmp.png",
                    "orn_helmet_dt02_sub_alp_ovl.bmp.png",
                    "orn_knee_dt01.bmp.png",
                    "orn_knee_dt01_sub_alp_ovl.bmp.png",
                    "orn_leg02_sub_alp_ovl.bmp.png",
                    "orn_leg_dt00.bmp.png",
                    "orn_leg_dt00_sub_alp_ovl.bmp.png",
                    "orn_leg_dt01.bmp.png",
                    "orn_leg_dt01_sub_alp_ovl.bmp.png",
                    "orn_leg_dt02.bmp.png",
                    "orn_leg_dt03.bmp.png",
                    "orn_leg_dt03_sub_alp_ovl.bmp.png",
                    "orn_leg_dt04.bmp.png",
                    "orn_leg_dt04_sub_alp_ovl.bmp.png",
                    "orn_neck_dt01.bmp.png",
                    "orn_neck_sub_alp_ovl.bmp.png",
                    "orn_should_dt01.bmp.png",
                    "orn_should_dt01_sub_alp_ovl.bmp.png",
                    "orn_should_dt02.bmp.png",
                    "orn_should_dt02_sub_alp_ovl.bmp.png"
                    };
            }
            else if (selectedModel == "MGS2 Olga Plant")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Olga Plant");
                modelFile = "org_plant_sh_mt.obj";
                mtlFile = "org_plant_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "orp__arm_ovl_sub_alp.bmp.png",
                    "orp_arm_moji.bmp.png",
                    "orp_band.bmp.png",
                    "orp_band2.bmp.png",
                    "orp_band3.bmp.png",
                    "orp_boot11.bmp.png",
                    "orp_boot_alp_ovl_sub.bmp.png",
                    "orp_chest_j_meisai_re.bmp.png",
                    "orp_chest_re_s_alp_sub_ovl.bmp.png",
                    "orp_chest_s_re.bmp.png",
                    "orp_eri2_meisai_re.bmp.png",
                    "orp_eri_meisai_re.bmp.png",
                    "orp_eyes_ovl_sub_alp.bmp.png",
                    "orp_face2_sub_alp_ovl.bmp.png",
                    "orp_face_re.bmp.png",
                    "orp_hand_sam.bmp.png",
                    "orp_hand_top.bmp.png",
                    "orp_hf00_st.bmp.png",
                    "orp_hf01_st.bmp.png",
                    "orp_hf02_st.bmp.png",
                    "orp_hi_eye.bmp.png",
                    "orp_kata_meisai_re.bmp.png",
                    "orp_larm_j_meisai_re.bmp.png",
                    "orp_pamtsu2_meisai_re.bmp.png",
                    "orp_pamtsu_meisai_re.bmp.png",
                    "orp_rarm_j_meisai_re.bmp.png",
                    "orp_waist.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Olga Tanker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Olga Tanker");
                modelFile = "org_sgl.obj";
                mtlFile = "org_sgl.mtl";
                textureFiles = new string[]
                {
                    "gbs_p4_fro.bmp.png",
                    "gbs_p4_side.bmp.png",
                    "org_arm3.bmp.png",
                    "org_arm4.bmp.png",
                    "org_arm_2.bmp.png",
                    "org_boot.bmp.png",
                    "org_boot_bt1.bmp.png",
                    "org_chest.bmp.png",
                    "org_hand_all4.bmp.png",
                    "org_leg00.bmp.png",
                    "org_mil_re_face.bmp.png",
                    "org_rapelsate.bmp.png",
                    "org_sknc_all.bmp.png",
                    "org_waist.bmp.png",
                    "org_year.bmp.png",
                    "rad_musen1.bmp.png",
                    "rad_musen2.bmp.png",
                    "rad_musen3.bmp.png",
                    "rad_musen4.bmp.png",
                    "rad_musen5.bmp.png",
                    "ssk_all.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Otacon")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Otacon");
                modelFile = "otc_def_sh_mt.obj";
                mtlFile = "otc_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "ema_glassf_sub_alp_ovl.bmp.png",
                    "fat_hand_sub_alp_ovl.bmp.png",
                    "htl.bmp.png",
                    "otc_arm2.bmp.png",
                    "otc_body3.bmp.png",
                    "otc_eri2.bmp.png",
                    "otc_eye.bmp.png",
                    "otc_eye_ovl_sub_alp.bmp.png",
                    "otc_face.bmp.png",
                    "otc_glass_f.bmp.png",
                    "otc_glasses.bmp.png",
                    "otc_hair_all2.bmp.png",
                    "otc_hand00.bmp.png",
                    "otc_hi_face_sub_ovl_alp.bmp.png",
                    "otc_jeans55.bmp.png",
                    "otc_megane_sub_alp_ovl.bmp.png",
                    "otc_neck.bmp.png",
                    "otc_neck_sub_ovl_alp.bmp.png",
                    "otc_phone.bmp.png",
                    "otc_shues.bmp.png",
                    "otc_tshatsu2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Pliskin")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Pliskin");
                modelFile = "iro_def_sh_mt.obj";
                mtlFile = "iro_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "iro_arm_l.bmp.png",
                    "iro_arm_r.bmp.png",
                    "iro_bag1.bmp.png",
                    "iro_bag2.bmp.png",
                    "iro_bag3.bmp.png",
                    "iro_bag4.bmp.png",
                    "iro_bag5.bmp.png",
                    "iro_bag6.bmp.png",
                    "iro_body.bmp.png",
                    "iro_boots.bmp.png",
                    "iro_hair_base.bmp.png",
                    "iro_hand.bmp.png",
                    "iro_hi_face.bmp.png",
                    "iro_hi_face_ovl_sub_alp.bmp.png",
                    "iro_nail.bmp.png",
                    "iro_phones01.bmp.png",
                    "iro_radio.bmp.png",
                    "iro_skin.bmp.png",
                    "iro_snawear.bmp.png",
                    "iro_snawear_ovl_sub_alp.bmp.png",
                    "sel_belt.bmp.png",
                    "sel_folder.bmp.png",
                    "sel_leg.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png"
                    };
            }
            else if (selectedModel == "MGS2 Raiden Ninja")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Raiden Ninja");
                modelFile = "rai_def_sh_mt_stage_r_vr_b_r.obj";
                mtlFile = "rai_def_sh_mt_stage_r_vr_b_r.mtl";
                textureFiles = new string[]
                {
                    "rai_arm.bmp.png",
                    "rai_arm_fix_ovl_sub_alp.bmp.png",
                    "rai_body.bmp.png",
                    "rai_body_b.bmp.png",
                    "rai_body_b_fix_ovl_sub_alp.bmp.png",
                    "rai_body_fix_ovl_sub_alp.bmp.png",
                    "rai_finger_fix_ovl_sub_alp.bmp.png",
                    "rai_foot.bmp.png",
                    "rai_foot_a.bmp.png",
                    "rai_foot_a_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_b.bmp.png",
                    "rai_foot_b_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_c.bmp.png",
                    "rai_foot_c_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_fix_ovl_sub_alp.bmp.png",
                    "rai_hand.bmp.png",
                    "rai_hand_a.bmp.png",
                    "rai_hand_b.bmp.png",
                    "rai_hand_b_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_c.bmp.png",
                    "rai_hand_c_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_d.bmp.png",
                    "rai_hand_d_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_fix_ovl_sub_alp.bmp.png",
                    "rai_hi_eye2.bmp.png",
                    "rai_hi_face_ss2.bmp.png",
                    "rai_mask_a_ovl_sub_alp.bmp.png",
                    "rai_mask_b.bmp.png",
                    "rai_mask_d.bmp.png",
                    "rai_mask_e.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Raiden Scuba")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Raiden Scuba");
                modelFile = "rai_def_sh_mt_stage_r_plt1_r.obj";
                mtlFile = "rai_def_sh_mt_stage_r_plt1_r.mtl";
                textureFiles = new string[]
                {
                    "rai_arm_fix.bmp.png",
                    "rai_arm_fix_ovl_sub_alp.bmp.png",
                    "rai_body_01_fix.bmp.png",
                    "rai_body_01_fix_ovl_sub_alp.bmp.png",
                    "rai_body_02_fix.bmp.png",
                    "rai_body_02_fix_ovl_sub_alp.bmp.png",
                    "rai_daiver_chin.bmp.png",
                    "rai_diver_eyeline.bmp.png",
                    "rai_diver_head_side.bmp.png",
                    "rai_diver_head_top.bmp.png",
                    "rai_diver_mask.bmp.png",
                    "rai_diver_neck.bmp.png",
                    "rai_diver_tube.bmp.png",
                    "rai_diver_tube_in.bmp.png",
                    "rai_diver_tube_red.bmp.png",
                    "rai_finger_fix.bmp.png",
                    "rai_finger_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_fix.bmp.png",
                    "rai_foot_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_01_fix.bmp.png",
                    "rai_hand_01_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_02_fix.bmp.png",
                    "rai_hand_02_fix_ovl_sub_alp.bmp.png",
                    "rai_hi_eye2.bmp.png",
                    "rai_hi_face_ss2.bmp.png",
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
            /*
             * MGS2 SAA:

Model Data:
saa.obj
saa.mtl

Texture Names:
m92_jyuukou.bmp.png
saa_all_kimu.bmp.png

MGS2 Scott Dolph:

Model Data:
sco_def_light.obj
sco_def_light.mtl

Texture Names:
leg.bmp.png
sco_body.bmp.png
sco_body_eri.bmp.png
sco_body_kubi.bmp.png
sco_boot_soko.bmp.png
sco_boots.bmp.png
sco_boots2.bmp.png
sco_colt_folder.bmp.png
sco_hand_in.bmp.png
sco_hand_in_ovl_sub_alp.bmp.png
sco_hand_out.bmp.png
sco_hand_out_ovl_sub_alp.bmp.png
sco_hi_face_n_ovl_sub_alp.bmp.png
sco_n3_ude_u_ovl_sub_alp.bmp.png
sco_n3_ude_under.bmp.png
sco_ude.bmp.png
sco_watch.bmp.png
sna_hi_eye01dt.bmp.png
sna_hi_eye_ovl_sub_alp.bmp.png
svo_hi_face_n.bmp.png
v_scm_r.bmp.png

MGS2 Seal:

Model Data:
sel_def_sh.obj
sel_def_sh.mtl

Texture Names:
iro_bag1.bmp.png
sel_arm_l.bmp.png
sel_arm_r.bmp.png
sel_bag2.bmp.png
sel_bag3.bmp.png
sel_bag4.bmp.png
sel_belt.bmp.png
sel_body.bmp.png
sel_boots.bmp.png
sel_eyebig_surp.bmp.png
sel_folder.bmp.png
sel_hand.bmp.png
sel_leg.bmp.png
sel_mask.bmp.png*/
            else if (selectedModel == "MGS2 SAA")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 SAA");
                modelFile = "saa.obj";
                mtlFile = "saa.mtl";
                textureFiles = new string[]
                {
                    "m92_jyuukou.bmp.png",
                    "saa_all_kimu.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Scott Dolph")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Scott Dolph");
                modelFile = "sco_def_light.obj";
                mtlFile = "sco_def_light.mtl";
                textureFiles = new string[]
                {
                    "leg.bmp.png",
                    "sco_body.bmp.png",
                    "sco_body_eri.bmp.png",
                    "sco_body_kubi.bmp.png",
                    "sco_boot_soko.bmp.png",
                    "sco_boots.bmp.png",
                    "sco_boots2.bmp.png",
                    "sco_colt_folder.bmp.png",
                    "sco_hand_in.bmp.png",
                    "sco_hand_in_ovl_sub_alp.bmp.png",
                    "sco_hand_out.bmp.png",
                    "sco_hand_out_ovl_sub_alp.bmp.png",
                    "sco_hi_face_n_ovl_sub_alp.bmp.png",
                    "sco_n3_ude_u_ovl_sub_alp.bmp.png",
                    "sco_n3_ude_under.bmp.png",
                    "sco_ude.bmp.png",
                    "sco_watch.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "v_scm_r.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Seal")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Seal");
                modelFile = "sel_def_sh.obj";
                mtlFile = "sel_def_sh.mtl";
                textureFiles = new string[]
                {
                    "iro_bag1.bmp.png",
                    "sel_arm_l.bmp.png",
                    "sel_arm_r.bmp.png",
                    "sel_bag2.bmp.png",
                    "sel_bag3.bmp.png",
                    "sel_bag4.bmp.png",
                    "sel_belt.bmp.png",
                    "sel_body.bmp.png",
                    "sel_boots.bmp.png",
                    "sel_eyebig_surp.bmp.png",
                    "sel_folder.bmp.png",
                    "sel_hand.bmp.png",
                    "sel_leg.bmp.png",
                    "sel_mask.bmp.png"
                    };
            }
            /*
             * MGS2 Snake (MGS1):

Model Data:
sna_oss_sh_mt.obj
sna_oss_sh_mt.mtl

Texture Names:
mrl_waist_pack.bmp.png
sna_foot01dt.bmp.png
sna_hair3.bmp.png
sna_hand1dt.bmp.png
sna_hand2.bmp.png
sna_hi_bdn01dt.bmp.png
sna_hi_eye01dt.bmp.png
sna_hi_eye_ovl_sub_alp.bmp.png
sna_hi_face01dt.bmp.png
sna_hi_face_ovl_sub_alp.bmp.png
sna_hi_meziri.bmp.png
sna_m9_glip.bmp.png
sna_oss_apad.bmp.png
sna_oss_arm_ovl_sub_alp.bmp.png
sna_oss_arm_ss.bmp.png
sna_oss_body.bmp.png
sna_oss_body_ovl_sub_alp.bmp.png
sna_oss_eri.bmp.png
sna_oss_eri_ovl_sub_alp.bmp.png
sna_oss_hip.bmp.png
sna_oss_hip_ovl_sub_alp.bmp.png
sna_oss_hlst_back.bmp.png
sna_oss_hlst_fro.bmp.png
sna_oss_hlst_side.bmp.png
sna_oss_leg_ovl_sub_alp.bmp.png
sna_oss_leg_ss.bmp.png
sna_oss_lpad.bmp.png
sna_oss_neck.bmp.png
sna_oss_oshould_ovl_sub_alp.bmp.png
sna_oss_outshould.bmp.png
sna_oss_rthigh.bmp.png
sna_oss_rthigh_ovl_sub_alp.bmp.png
sna_oss_should.bmp.png
sna_oss_spad.bmp.png
sna_oss_spad_ovl_sub_alp.bmp.png
sna_oss_vbelt.bmp.png
sna_oss_weast.bmp.png
sna_oss_weast_ovl_sub_alp.bmp.png
sna_p1dt.bmp.png
sna_toe01dt.bmp.png
sna_toe02dt.bmp.png*/
            else if (selectedModel == "MGS2 Snake (MGS1)")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Snake (MGS1)");
                modelFile = "sna_oss_sh_mt.obj";
                mtlFile = "sna_oss_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "mrl_waist_pack.bmp.png",
                    "sna_foot01dt.bmp.png",
                    "sna_hair3.bmp.png",
                    "sna_hand1dt.bmp.png",
                    "sna_hand2.bmp.png",
                    "sna_hi_bdn01dt.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "sna_hi_face01dt.bmp.png",
                    "sna_hi_face_ovl_sub_alp.bmp.png",
                    "sna_hi_meziri.bmp.png",
                    "sna_m9_glip.bmp.png",
                    "sna_oss_apad.bmp.png",
                    "sna_oss_arm_ovl_sub_alp.bmp.png",
                    "sna_oss_arm_ss.bmp.png",
                    "sna_oss_body.bmp.png",
                    "sna_oss_body_ovl_sub_alp.bmp.png",
                    "sna_oss_eri.bmp.png",
                    "sna_oss_eri_ovl_sub_alp.bmp.png",
                    "sna_oss_hip.bmp.png",
                    "sna_oss_hip_ovl_sub_alp.bmp.png",
                    "sna_oss_hlst_back.bmp.png",
                    "sna_oss_hlst_fro.bmp.png",
                    "sna_oss_hlst_side.bmp.png",
                    "sna_oss_leg_ovl_sub_alp.bmp.png",
                    "sna_oss_leg_ss.bmp.png",
                    "sna_oss_lpad.bmp.png",
                    "sna_oss_neck.bmp.png",
                    "sna_oss_oshould_ovl_sub_alp.bmp.png",
                    "sna_oss_outshould.bmp.png",
                    "sna_oss_rthigh.bmp.png",
                    "sna_oss_rthigh_ovl_sub_alp.bmp.png",
                    "sna_oss_should.bmp.png",
                    "sna_oss_spad.bmp.png",
                    "sna_oss_spad_ovl_sub_alp.bmp.png",
                    "sna_oss_vbelt.bmp.png",
                    "sna_oss_weast.bmp.png",
                    "sna_oss_weast_ovl_sub_alp.bmp.png",
                    "sna_p1dt.bmp.png",
                    "sna_toe01dt.bmp.png",
                    "sna_toe02dt.bmp.png"
                    };
            }
            /*MGS2 Socom:

Model Data:
scm.obj
scm.mtl

Texture Names:
scm_sb_s.bmp.png
scm_sb_temp.bmp.png

MGS2 Solidus:

Model Data:
sol_def_sh_mt.obj
sol_def_sh_mt.mtl

Texture Names:
jam_hi_tooth_d2_dt.bmp.png
jam_hi_tooth_d3_dt.bmp.png
ptr_tooth_d1.bmp.png
ptr_tooth_d4.bmp.png
ptr_tooth_u1.bmp.png
ptr_tooth_u2.bmp.png
ptr_tooth_u3.bmp.png
ptr_tooth_u4.bmp.png
sna2h_mouin.bmp.png
sna_hi_eye_ovl_sub_alp.bmp.png
sna_hi_tang01dt.bmp.png
sol_arm_dt01.bmp.png
sol_arm_dt01_ovl_sub_alp.bmp.png
sol_arm_dt02.bmp.png
sol_arm_dt02_sub_alp_ovl.bmp.png
sol_belt_dt.bmp.png
sol_body_dt01.bmp.png
sol_body_dt01_ovl_sub_alp.bmp.png
sol_body_dt02.bmp.png
sol_body_dt02_ovl_sub_alp.bmp.png
sol_body_dt03.bmp.png
sol_body_dt03_ovl_sub_alp.bmp.png
sol_body_dt04.bmp.png
sol_body_dt04_ovl_sub_alp.bmp.png
sol_body_dt05.bmp.png
sol_body_dt05_ovl_sub_alp.bmp.png
sol_body_dt06.bmp.png
sol_body_dt06_ovl_sub_alp.bmp.png
sol_foot_dt01.bmp.png
sol_foot_dt01_ovl_sub_alp.bmp.png
sol_foot_dt02.bmp.png
sol_foot_dt02_ovl_sub_alp.bmp.png
sol_hand_dt.bmp.png
sol_hand_dt_ovl_sub_alp.bmp.png
sol_hi_eye_dt01.bmp.png
sol_hi_face_d1_ovl_sub_alp.bmp.png
sol_hi_face_d2_ovl_sub_alp.bmp.png
sol_hi_face_dt01.bmp.png
sol_hi_face_dt02.bmp.png
sol_leg_dt01.bmp.png
sol_leg_dt01_ovl_sub_alp.bmp.png
sol_leg_dt02.bmp.png
sol_leg_dt02_ovl_sub_alp.bmp.png
sol_leg_dt03.bmp.png
sol_leg_dt03_ovl_sub_alp.bmp.png
sol_leg_dt04.bmp.png
sol_leg_dt04_ovl_sub_alp.bmp.png
sol_leg_dt05.bmp.png
sol_leg_dt05_sub_alp_ovl.bmp.png
sol_neck_i_dt.bmp.png
sol_shoulder_dt.bmp.png
sol_shoulder_dt_ovl_sub_alp.bmp.png*/
            else if (selectedModel == "MGS2 Socom")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Socom");
                modelFile = "scm.obj";
                mtlFile = "scm.mtl";
                textureFiles = new string[]
                {
                    "scm_sb_s.bmp.png",
                    "scm_sb_temp.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Solidus")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Solidus");
                modelFile = "sol_def_sh_mt.obj";
                mtlFile = "sol_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "jam_hi_tooth_d2_dt.bmp.png",
                    "jam_hi_tooth_d3_dt.bmp.png",
                    "ptr_tooth_d1.bmp.png",
                    "ptr_tooth_d4.bmp.png",
                    "ptr_tooth_u1.bmp.png",
                    "ptr_tooth_u2.bmp.png",
                    "ptr_tooth_u3.bmp.png",
                    "ptr_tooth_u4.bmp.png",
                    "sna2h_mouin.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "sna_hi_tang01dt.bmp.png",
                    "sol_arm_dt01.bmp.png",
                    "sol_arm_dt01_ovl_sub_alp.bmp.png",
                    "sol_arm_dt02.bmp.png",
                    "sol_arm_dt02_sub_alp_ovl.bmp.png",
                    "sol_belt_dt.bmp.png",
                    "sol_body_dt01.bmp.png",
                    "sol_body_dt01_ovl_sub_alp.bmp.png",
                    "sol_body_dt02.bmp.png",
                    "sol_body_dt02_ovl_sub_alp.bmp.png",
                    "sol_body_dt03.bmp.png",
                    "sol_body_dt03_ovl_sub_alp.bmp.png",
                    "sol_body_dt04.bmp.png",
                    "sol_body_dt04_ovl_sub_alp.bmp.png",
                    "sol_body_dt05.bmp.png",
                    "sol_body_dt05_ovl_sub_alp.bmp.png",
                    "sol_body_dt06.bmp.png",
                    "sol_body_dt06_ovl_sub_alp.bmp.png",
                    "sol_foot_dt01.bmp.png",
                    "sol_foot_dt01_ovl_sub_alp.bmp.png",
                    "sol_foot_dt02.bmp.png",
                    "sol_foot_dt02_ovl_sub_alp.bmp.png",
                    "sol_hand_dt.bmp.png",
                    "sol_hand_dt_ovl_sub_alp.bmp.png",
                    "sol_hi_eye_dt01.bmp.png",
                    "sol_hi_face_d1_ovl_sub_alp.bmp.png",
                    "sol_hi_face_d2_ovl_sub_alp.bmp.png",
                    "sol_hi_face_dt01.bmp.png",
                    "sol_hi_face_dt02.bmp.png",
                    "sol_leg_dt01.bmp.png",
                    "sol_leg_dt01_ovl_sub_alp.bmp.png",
                    "sol_leg_dt02.bmp.png",
                    "sol_leg_dt02_ovl_sub_alp.bmp.png",
                    "sol_leg_dt03.bmp.png",
                    "sol_leg_dt03_ovl_sub_alp.bmp.png",
                    "sol_leg_dt04.bmp.png",
                    "sol_leg_dt04_ovl_sub_alp.bmp.png",
                    "sol_leg_dt05.bmp.png",
                    "sol_leg_dt05_sub_alp_ovl.bmp.png",
                    "sol_neck_i_dt.bmp.png",
                    "sol_shoulder_dt.bmp.png",
                    "sol_shoulder_dt_ovl_sub_alp.bmp.png"
                    };
            }
            /*MGS2 Stillman:

Model Data:
ptr_def_sh_mt.obj
ptr_def_sh_mt.mtl

Texture Names:
pit_arm.bmp.png
pit_arm_markl.bmp.png
pit_arm_markr.bmp.png
pit_arm_ovl_alp_sub.bmp.png
pit_armark_l_alp_ovl_sub.bmp.png
pit_armark_r_alp_ovl_sub.bmp.png
pit_body3_ovl_alp_sub.bmp.png
pit_eye.bmp.png
pit_face2_ovl_sub_alp.bmp.png
pit_food_c.bmp.png
pit_food_c_sub_ovl_alp.bmp.png
pit_food_s.bmp.png
pit_food_s_sub_ovl_alp.bmp.png
pit_legl.bmp.png
pit_neccktie.bmp.png
pit_shatu00.bmp.png
pit_shatu01.bmp.png
pit_shoese.bmp.png
pit_shoese_ovl_sub_alp.bmp.png
ptr_eyes_ovl_sub_alp.bmp.png
ptr_face03.bmp.png
ptr_hand.bmp.png
ptr_hand_sub_alp_ovl.bmp.png
ptr_jaket_nypd.bmp.png

MGS2 Tuxedo Snake:

Model Data:
sna_txd_sh_mt.obj
sna_txd_sh_mt.mtl

Texture Names:
iro_hair_base.bmp.png
iro_hi_face.bmp.png
iro_hi_face_ovl_sub_alp.bmp.png
sna_hi_eye01dt.bmp.png
sna_hi_eye_ovl_sub_alp.bmp.png
sna_txd_arm_fix.bmp.png
sna_txd_arm_ovl_sub_alp.bmp.png
sna_txd_hand.bmp.png
sna_txd_jacket.bmp.png
sna_txd_jacket_fix.bmp.png
sna_txd_jacket_ovl_sub_alp.bmp.png
sna_txd_leg_fix2.bmp.png
sna_txd_leg_ovl_sub_alp.bmp.png
sna_txd_shirt.bmp.png
sna_txd_shoe.bmp.png
sna_txd_vest.bmp.png

MGS2 USP:

Model Data:
usp.obj
usp.mtl

Texture Names:
scm_sb_temp.bmp.png
usp_sb_all.bmp.png
usp_sb_all2.bmp.png*/
            else if (selectedModel == "MGS2 Stillman")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Stillman");
                modelFile = "ptr_def_sh_mt.obj";
                mtlFile = "ptr_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "pit_arm.bmp.png",
                    "pit_arm_markl.bmp.png",
                    "pit_arm_markr.bmp.png",
                    "pit_arm_ovl_alp_sub.bmp.png",
                    "pit_armark_l_alp_ovl_sub.bmp.png",
                    "pit_armark_r_alp_ovl_sub.bmp.png",
                    "pit_body3_ovl_alp_sub.bmp.png",
                    "pit_eye.bmp.png",
                    "pit_face2_ovl_sub_alp.bmp.png",
                    "pit_food_c.bmp.png",
                    "pit_food_c_sub_ovl_alp.bmp.png",
                    "pit_food_s.bmp.png",
                    "pit_food_s_sub_ovl_alp.bmp.png",
                    "pit_legl.bmp.png",
                    "pit_neccktie.bmp.png",
                    "pit_shatu00.bmp.png",
                    "pit_shatu01.bmp.png",
                    "pit_shoese.bmp.png",
                    "pit_shoese_ovl_sub_alp.bmp.png",
                    "ptr_eyes_ovl_sub_alp.bmp.png",
                    "ptr_face03.bmp.png",
                    "ptr_hand.bmp.png",
                    "ptr_hand_sub_alp_ovl.bmp.png",
                    "ptr_jaket_nypd.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Tuxedo Snake")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Tuxedo Snake");
                modelFile = "sna_txd_sh_mt.obj";
                mtlFile = "sna_txd_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "iro_hair_base.bmp.png",
                    "iro_hi_face.bmp.png",
                    "iro_hi_face_ovl_sub_alp.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "sna_txd_arm_fix.bmp.png",
                    "sna_txd_arm_ovl_sub_alp.bmp.png",
                    "sna_txd_hand.bmp.png",
                    "sna_txd_jacket.bmp.png",
                    "sna_txd_jacket_fix.bmp.png",
                    "sna_txd_jacket_ovl_sub_alp.bmp.png",
                    "sna_txd_leg_fix2.bmp.png",
                    "sna_txd_leg_ovl_sub_alp.bmp.png",
                    "sna_txd_shirt.bmp.png",
                    "sna_txd_shoe.bmp.png",
                    "sna_txd_vest.bmp.png"
                    };
            }
            else if (selectedModel == "MGS2 USP")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 USP");
                modelFile = "usp.obj";
                mtlFile = "usp.mtl";
                textureFiles = new string[]
                {
                    "scm_sb_temp.bmp.png",
                    "usp_sb_all.bmp.png",
                    "usp_sb_all2.bmp.png"
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

        private void ShowHelpFaq()
        {
            using (Form faq = new Form())
            {
                faq.Text = "Help & FAQ";
                faq.StartPosition = FormStartPosition.CenterParent;
                faq.FormBorderStyle = FormBorderStyle.FixedDialog;
                faq.MinimizeBox = false;
                faq.MaximizeBox = false;
                faq.ClientSize = new Size(595, 525);

                Label lbl = new Label
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                    Text = BuildFaqText(),
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
                btn.Click += (_, __) => faq.Close();

                faq.Controls.Add(lbl);
                faq.Controls.Add(btn);

                faq.ShowDialog(this);
            }
        }

        private string BuildFaqText() =>
        "\n1. Can I swap models with this tool?\n" +
        "No, you cannot. When model swapping is figured out I will add it in.\n\n" +

        "2. Why isn’t it working when I just click a button?\n" +
        "This app is for users who already know how to replace or edit game\n" +
        "textures. It doesn’t teach modding from scratch. Instead, it handles\n" +
        "the boring stuff: making folders, naming files, and packing everything.\n" +
        "This way you can publish a finished mod much faster.\n\n" +

        "3. Why do I need Python and GIMP?\n" +
        "Python  run scripts I created for GIMP with GIMP's Python-Fu.\n" +
        "GIMP 2.10 is needed to convert your PNG images into a DDS image with mipmaps.\n" +
        "If either one is missing, the converters and creating mods will not work.\n\n" +

        "4. What can the tool do?\n" +
        "- View 3D models from MGS2 and MGS3 along with their textures.\n" +
        "- Pick a PNG, and see how it looks on a 3D model saving time checking ingame.\n" +
        "- Create Mods’—the app builds a ready‑to‑use folder in either\n" +
        "- Convert files between CTXR, DDS & PNG \n\n" +

        "5. I can't figure this out. Can you make me a mod?\n" +
        "Yep I can create a mod for you, but not for free.\n" +
        "Reach out to me on Discord under antibigboss and we can discuss it.\n";

        private void HelpFaqButton_Click(object sender, EventArgs e)
        {
            ShowHelpFaq();
        }
    }
}