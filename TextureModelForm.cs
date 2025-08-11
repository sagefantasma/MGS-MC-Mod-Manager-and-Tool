using Assimp;
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
        // MGS3 Character Models
        "MGS3 Assistant","MGS3 Boss Mantle","MGS3 Boss Sneaking Suit","MGS3 Boss VM","MGS3 Cameraman A","MGS3 Cameraman B","MGS3 Chopper Worker","MGS3 CIA Director","MGS3 Enemy Bike","MGS3 Enemy Johnny","MGS3 Enemy Platform","MGS3 Enemy Pyro","MGS3 EVA Half Naked","MGS3 EVA Injured","MGS3 EVA Jumpsuit","MGS3 EVA Jumpsuit Jacket","MGS3 EVA Naked","MGS3 Granin","MGS3 Granin Dead","MGS3 GRU","MGS3 KGB","MGS3 Maintenance Worker","MGS3 Major Ocelot","MGS3 Major Zero","MGS3 Major Zero Headphones","MGS3 Major Zero Suit","MGS3 MiG Pilot","MGS3 Ocelot Unit","MGS3 Officer","MGS3 Paramedic","MGS3 Paramedic Headphones","MGS3 Paramedic Suit","MGS3 Pilot","MGS3 President","MGS3 Raikov","MGS3 Raikov Naked","MGS3 Scientist","MGS3 Scientist Dead","MGS3 Secretary of Defense","MGS3 Sigint","MGS3 Snake Halo Jump","MGS3 Snake Maintenance","MGS3 Snake Naked","MGS3 Snake Naked Eyepatch","MGS3 Snake Scientist","MGS3 Snake SE","MGS3 Snake SE Eyepatch","MGS3 Snake SE Injured","MGS3 Snake Sneaking Suit","MGS3 Snake Suit Endgame","MGS3 Snake Torture Room","MGS3 Snake Torture Room Bag","MGS3 Snake Tuxedo","MGS3 Snake VM Injured","MGS3 Sokolov Coat","MGS3 Sokolov Scientist","MGS3 Tanya (Eva)","MGS3 The End","MGS3 The Fear","MGS3 The Fury Helmet","MGS3 The Fury No Helmet","MGS3 The Pain Mask","MGS3 The Pain No Mask","MGS3 The Sorrow Bleeding Eyes","MGS3 The Sorrow Main","MGS3 The Sorrow Parka","MGS3 VIP A","MGS3 VIP B","MGS3 VIP C","MGS3 Volgin Coat","MGS3 Volgin Coat (Ammo)","MGS3 Volgin No Coat","MGS3 Volgin No Coat (Ammo)",

        // MGS3 Weapon/Item Models
        "MGS3 Book","MGS3 Bucket","MGS3 Camera A","MGS3 Camera B","MGS3 Cardboard Box A","MGS3 Cardboard Box B","MGS3 Cardboard Box C","MGS3 Cigar","MGS3 Comic","MGS3 Croc Cap","MGS3 Directional Mic","MGS3 Flask","MGS3 Item Belt","MGS3 Johnny Picture","MGS3 Kerotan","MGS3 Microfilm","MGS3 Mousetrap","MGS3 Night Vision Goggles","MGS3 Philosopher's Legacy","MGS3 Radio","MGS3 Raikov Picture","MGS3 Roast Fish","MGS3 Roast Snake","MGS3 Scope","MGS3 Sokolov Picture","MGS3 Spy Radio Broken","MGS3 Spy Radio Closed","MGS3 Spy Radio Open","MGS3 Suitcase EVA","MGS3 Suitcase Snake","MGS3 Tape","MGS3 Tape Recorder","MGS3 Thermal Goggles","MGS3 Torch","MGS3 Transmitter","MGS3 Wine Glass",

        // MGS2 Character Models
        "MGS2 Snake Tanker", "MGS2 Pliskin", "MGS2 Tuxedo Snake", "MGS2 Snake (MGS1)", "MGS2 Raiden", "MGS2 Raiden Ninja", "MGS2 Raiden Scuba", "MGS2 Naked Raiden", "MGS2 Tanker Guards", "MGS2 Tanker Backup", "MGS2 Big Shell Guards", "MGS2 Big Shell Backup", "MGS2 Cypher", "MGS2 Gun Cypher", "MGS2 Ames", "MGS2 Emma", "MGS2 Fatman", "MGS2 Fortune", "MGS2 Genome", "MGS2 Genome Mecha", "MGS2 Marine", "MGS2 Meryl", "MGS2 Ocelot", "MGS2 Ocelot Tanker",  "MGS2 Olga Ninja", "MGS2 Olga Plant", "MGS2 Olga Tanker", "MGS2 Otacon", "MGS2 President", "MGS2 Scott Dolph", "MGS2 Seal", "MGS2 Solidus", "MGS2 Stillman", "MGS2 Vamp Naked",
        
        // MGS2 Weapon/Item Models
        "MGS2 Fatman Bombs", "MGS2 Directional Microphone", "MGS2 Item Box 1", "MGS2 Item Box 2", "MGS2 M4", "MGS2 M9", "MGS2 Coolant Spray", "MGS2 Socom", "MGS2 SAA", "MGS2 USP", "MGS2 Ray Prototype", "MGS2 Ray Cockpit", "MGS2 Ray Mass Produced",

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
            else if (selectedModel == "MGS3 Assistant")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Assistant");
                modelFile = "MGS3Assistant.obj";
                mtlFile = "MGS3Assistant.mtl";
                textureFiles = new string[]
                {
        "ast_arm.bmp.png",
        "ast_body.bmp.png",
        "ast_eye_new.bmp.png",
        "ast_face.bmp.png",
        "ast_foot.bmp.png",
        "ast_grass_flame.bmp.png",
        "ast_hair_ovl_alp.bmp.png",
        "ast_idcard.bmp.png",
        "ast_neck.bmp.png",
        "med_heel.bmp.png",
        "sec_grass_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Boss Mantle")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Boss Mantle");
                modelFile = "MGS3 Boss Mantle.obj";
                mtlFile = "MGS3 Boss Mantle.mtl";
                textureFiles = new string[]
                {
        "bos_def_eye_new.bmp.png",
        "bos_def_teeth.bmp.png",
        "bos_eyelashes_ovl_alp.bmp.png",
        "bos_face_ss.bmp.png",
        "bos_hair_tail_all_ovl_alp.bmp.png",
        "bos_item_hair_ss_string_.bmp.png",
        "bos_item_shoelace_ovl_alp.bmp.png",
        "bos_mant_col.bmp.png",
        "bos_mant_hood.bmp.png",
        "bos_ss_arm.bmp.png",
        "bos_ss_belt_ovl_alp.bmp.png",
        "bos_ss_body.bmp.png",
        "bos_ss_boots.bmp.png",
        "bos_ss_hihand_dmo.bmp.png",
        "bos_ss_hook.bmp.png",
        "bos_ss_knee.bmp.png",
        "bos_ss_leg_ibl.bmp.png",
        "bos_ss_leg_net.bmp.png",
        "bos_ss_shld.bmp.png",
        "bos_ss_side.bmp.png",
        "bos_ss_side_belt.bmp.png",
        "bos_ss_uarm.bmp.png",
        "rendermap.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Boss Sneaking Suit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Boss Sneaking Suit");
                modelFile = "MGS3 Boss Sneaking Suit.obj";
                mtlFile = "MGS3 Boss Sneaking Suit.mtl";
                textureFiles = new string[]
                {
        "bos_def_eye_new.bmp.png",
        "bos_def_teeth.bmp.png",
        "bos_eyelashes_ovl_alp.bmp.png",
        "bos_face_ss.bmp.png",
        "bos_hair_tail_all_ovl_alp.bmp.png",
        "bos_item_hair_ss_string_.bmp.png",
        "bos_item_shoelace_ovl_alp.bmp.png",
        "bos_rope_ovl_alp.bmp.png",
        "bos_ss_arm.bmp.png",
        "bos_ss_belt_ovl_alp.bmp.png",
        "bos_ss_body.bmp.png",
        "bos_ss_boots.bmp.png",
        "bos_ss_hihand_dmo.bmp.png",
        "bos_ss_hook.bmp.png",
        "bos_ss_knee.bmp.png",
        "bos_ss_leg_ibl.bmp.png",
        "bos_ss_leg_net.bmp.png",
        "bos_ss_shld.bmp.png",
        "bos_ss_side.bmp.png",
        "bos_ss_side_belt.bmp.png",
        "bos_ss_uarm.bmp.png",
        "cqcc_tex02.bmp.png",
        "rendermap.bmp.png",
                };
            }

            else if (selectedModel == "MGS3 Boss VM")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Boss VM");
                modelFile = "MGS3 Boss - Virtuous Mission.obj";
                mtlFile = "MGS3 Boss - Virtuous Mission.mtl";
                textureFiles = new string[]
                {
        "bos_def_eye_new.bmp.png",
        "bos_def_teeth.bmp.png",
        "bos_eyelashes_ovl_alp.bmp.png",
        "bos_face_ss.bmp.png",
        "bos_vr_arm.bmp.png",
        "bos_vr_body.bmp.png",
        "bos_vr_dmo_hand.bmp.png",
        "bos_vr_hair_back_ovl_alp.bmp.png",
        "bos_vr_leg.bmp.png",
        "bos_vr_uarm.bmp.png",
        "cord_ovl_alp.bmp.png",
        "cqcc_tex02.bmp.png",
        "cqck_tex02.bmp.png",
        "rend_bandana.bmp.png",
        "rendermap.bmp.png",
        "sna_bk.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_belt.bmp_77380cc45d92a52a1e8da9f59a6ea891.png",
        "sna_mgs3_gh.bmp.png",
        "sna_mgs3_gun_hol.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_belt.bmp_387276427ee88d88dbacbd0ae1f73fd7.png",
        "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
        "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
                };
            }
            else if (selectedModel == "MGS3 Cameraman A")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cameraman A");
                modelFile = "MGS3 Cameraman A.obj";
                mtlFile = "MGS3 Cameraman A.mtl";
                textureFiles = new string[]
                {
        "cme_a_eye.bmp.png",
        "cme_a_face.bmp.png",
        "cme_a_hand.bmp.png",
        "cme_a_idcard.bmp.png",
        "cme_a_nekutai.bmp.png",
        "cme_a_shurts.bmp.png",
        "cme_suits.bmp.png",
        "zro_suit_dmo_boots_new.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Cameraman B")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cameraman B");
                modelFile = "MGS3 Cameraman B.obj";
                mtlFile = "MGS3 Cameraman B.mtl";
                textureFiles = new string[]
                {
        "cme_b_eye.bmp.png",
        "cme_b_hand.bmp.png",
        "cme_b_head.bmp.png",
        "cme_b_idcard.bmp.png",
        "cme_b_shurts.bmp.png",
        "cme_b_suits.bmp.png",
        "zro_suit_dmo_boots_new.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Chopper Worker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Chopper Worker");
                modelFile = "MGS3 Chopper Worker.obj";
                mtlFile = "MGS3 Chopper Worker.mtl";
                textureFiles = new string[]
                {
        "gpi_arm_def.bmp.png",
        "gpi_eyeb.bmp.png",
        "gpi_face_def.bmp.png",
        "gpi_facea.bmp.png",
        "gpi_faceb.bmp.png",
        "gpi_foot_def.bmp.png",
        "gpi_foot_f.bmp.png",
        "gpi_foot_r.bmp.png",
        "gpi_foot_u.bmp.png",
        "gpi_hairb.bmp.png",
        "gpi_hand_def.bmp.png",
        "gpi_headphone.bmp.png",
        "gpi_hpline_ovl__alp.bmp.png",
        "gpi_income.bmp.png",
        "gpi_tshirt.bmp.png",
        "gpi_ubody.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 CIA Director")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 CIA Director");
                modelFile = "MGS3 CIA Director.obj";
                mtlFile = "MGS3 CIA Director.mtl";
                textureFiles = new string[]
                {
        "cia_body.bmp.png",
        "cia_eye.bmp.png",
        "cia_face.bmp.png",
        "cia_glass_ovl_alp.bmp.png",
        "cia_glass_parts.bmp.png",
        "cia_hair_ovl_alp.bmp.png",
        "cia_hand_low.bmp.png",
        "cia_me.bmp.png",
        "cia_nekutai.bmp.png",
        "cia_shats.bmp.png",
        "cia_teeth.bmp.png",
        "ene_spe_boots3.bmp.png",
        "ene_spe_boots4.bmp.png",
        "oce_def_boots2.bmp.png",
        "rus_button_ovl_alp.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Enemy Bike")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Enemy Bike");
                modelFile = "MGS3 Enemy Biker.obj";
                mtlFile = "MGS3 Enemy Biker.mtl";
                textureFiles = new string[]
                {
        "e_grene-do.bmp.png",
        "e_magpo-ti.bmp.png",
        "e_map.bmp.png",
        "e_map_belt.bmp.png",
        "ene_rider_arm.bmp.png",
        "ene_rider_body.bmp.png",
        "ene_rider_boots1.bmp.png",
        "ene_rider_boots2_test.bmp.png",
        "ene_rider_boots3_test.bmp.png",
        "ene_rider_boots4_test.bmp.png",
        "ene_rider_face_mask.bmp.png",
        "ene_rider_goggle_alp_ovl.bmp.png",
        "ene_rider_hand.bmp.png",
        "ene_rider_helmet.bmp.png",
        "ene_rider_leg2_test.bmp.png",
        "ene_rider_leg_test.bmp.png",
        "ene_rider_neck_open.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Enemy Johnny")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Enemy Johnny");
                modelFile = "MGS3 Johnny.obj";
                mtlFile = "MGS3 Johnny.mtl";
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
        "ene_def_eye.bmp.png",
        "ene_def_eye_close.bmp.png",
        "ene_def_eye_mu.bmp.png",
        "ene_def_eye_open.bmp.png",
        "ene_def_eye_pain.bmp.png",
        "ene_def_eye_white.bmp_6c3d7f31526aabd0f04de0a45771023f.png",
        "ene_def_headtop.bmp.png",
        "ene_def_headunder.bmp.png",
        "ene_def_leg.bmp.png",
        "ene_def_neck.bmp.png",
        "ene_def_pa-ka.bmp.png",
        "ene_flame_boots3.bmp.png",
        "ene_flame_boots4.bmp.png",
        "ene_jonny_armband.bmp.png",
        "ene_jonny_ovl_alp.bmp.png",
        "ene_kgb_boots1.bmp.png",
        "ene_kgb_boots2.bmp.png",
        "ene_kgb_hand.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Enemy Platform")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Enemy Platform");
                modelFile = "MGS3 Hovercraft Enemy.obj";
                mtlFile = "MGS3 Hovercraft Enemy.mtl";
                textureFiles = new string[]
                {
        "e_grene-do.bmp.png",
        "ene_fph_best.bmp_aa3b29848739a7ae82da7bc7f83e468a.png",
        "ene_fph_body.bmp_dcc5d84c43203080a9394bc7619c7dcf.png",
        "ene_fph_eye.bmp.png",
        "ene_fph_face.bmp.png",
        "ene_fph_foot_ed.bmp.png",
        "ene_fph_hand.bmp.png",
        "ene_fph_helm_ovl_alp.bmp.png",
        "ene_fph_met.bmp.png",
        "ene_fph_met_kanagu.bmp.png",
        "ene_fph_west_belt_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Enemy Pyro")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Enemy Pyro");
                modelFile = "MGS3 Flamethrower Enemy.obj";
                mtlFile = "MGS3 Flamethrower Enemy.mtl";
                textureFiles = new string[]
                {
        "ene_flame_belt_alp_ovl.bmp.png",
        "ene_flame_body.bmp.png",
        "ene_flame_boots1.bmp.png",
        "ene_flame_boots2.bmp.png",
        "ene_flame_boots3.bmp.png",
        "ene_flame_boots4.bmp.png",
        "ene_flame_eye.bmp.png",
        "ene_flame_hand.bmp.png",
        "ene_flame_lends_ovl_alp.bmp.png",
        "ene_flame_mask.bmp.png",
        "ene_flame_tank_alp_ovl.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 EVA Half Naked")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 EVA Half Naked");
                modelFile = "MGS3 Eva Half Naked.obj";
                mtlFile = "MGS3 Eva Half Naked.mtl";
                textureFiles = new string[]
                {
        "eve_def_body_fix.bmp.png",
        "eve_def_boot.bmp.png",
        "eve_def_teeth.bmp.png",
        "eve_def_zipper.bmp.png",
        "eve_eye.bmp.png",
        "eve_eyelashes_ovl_alp.bmp.png",
        "eve_naked_arm_new.bmp.png",
        "eve_naked_body_fix.bmp.png",
        "eve_naked_face_fix.bmp.png",
        "eve_naked_hand_new.bmp.png",
        "eve_naked_under_fix.bmp.png",
        "eve_nhair_fro1_ovl_alp.bmp.png",
        "eve_nhair_fro2_ovl_alp.bmp.png",
        "eve_nhair_long_ovl_alp.bmp.png",
        "eve_nhair_shade_ovl_alp.bmp.png",
        "eve_nhair_top_ovl_alp.bmp.png",
        "eve_suit_waist.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 EVA Injured")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 EVA Injured");
                modelFile = "MGS3 Eva Injured.obj";
                mtlFile = "MGS3 Eva Injured.mtl";
                textureFiles = new string[]
                {
        "eve_def_belt_ovl_alp.bmp.png",
        "eve_def_body_damage_fix.bmp.png",
        "eve_def_boot.bmp.png",
        "eve_def_bp.bmp.png",
        "eve_def_bra.bmp.png",
        "eve_def_face_fix_new.bmp.png",
        "eve_def_goggles.bmp.png",
        "eve_def_goggles_ovl_alp.bmp.png",
        "eve_def_hand.bmp.png",
        "eve_def_skin_fix.bmp.png",
        "eve_def_teeth.bmp.png",
        "eve_def_watch.bmp.png",
        "eve_def_zipper.bmp.png",
        "eve_eye_new.bmp.png",
        "eve_eyelashes_finish_ovl_alp.bmp.png",
        "eve_nhair_fro1_ovl_alp.bmp.png",
        "eve_nhair_fro2_ovl_alp.bmp.png",
        "eve_nhair_long_ovl_alp.bmp.png",
        "eve_nhair_shade_ovl_alp.bmp.png",
        "eve_nhair_top_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 EVA Jumpsuit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 EVA Jumpsuit");
                modelFile = "MGS3 Eva Jumpsuit.obj";
                mtlFile = "MGS3 Eva Jumpsuit.mtl";
                textureFiles = new string[]
                {
        "eve_def_belt_ovl_alp.bmp.png",
        "eve_def_body_fix.bmp.png",
        "eve_def_boot.bmp.png",
        "eve_def_bp.bmp.png",
        "eve_def_bra.bmp.png",
        "eve_def_face_fix_new.bmp.png",
        "eve_def_goggles.bmp.png",
        "eve_def_goggles_ovl_alp.bmp.png",
        "eve_def_hand.bmp.png",
        "eve_def_skin_fix.bmp.png",
        "eve_def_teeth.bmp.png",
        "eve_def_watch.bmp.png",
        "eve_def_zipper.bmp.png",
        "eve_eye_new.bmp.png",
        "eve_eyelashes_finish_ovl_alp.bmp.png",
        "eve_nhair_fro1_ovl_alp.bmp.png",
        "eve_nhair_fro2_ovl_alp.bmp.png",
        "eve_nhair_long_ovl_alp.bmp.png",
        "eve_nhair_shade_ovl_alp.bmp.png",
        "eve_nhair_top_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 EVA Jumpsuit Jacket")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 EVA Jumpsuit Jacket");
                modelFile = "MGS3 Eva Jumpsuit Jacket.obj";
                mtlFile = "MGS3 Eva Jumpsuit Jacket.mtl";
                textureFiles = new string[]
                {
        "eve_def_belt_ovl_alp.bmp.png",
        "eve_def_boot.bmp.png",
        "eve_def_bp.bmp.png",
        "eve_def_bra.bmp.png",
        "eve_def_face_fix_new.bmp.png",
        "eve_def_goggles.bmp.png",
        "eve_def_goggles_ovl_alp.bmp.png",
        "eve_def_hand.bmp.png",
        "eve_def_skin_fix.bmp.png",
        "eve_def_teeth.bmp.png",
        "eve_def_zipper.bmp.png",
        "eve_eye_new.bmp.png",
        "eve_eyelashes_finish_ovl_alp.bmp.png",
        "eve_nhair_fro1_ovl_alp.bmp.png",
        "eve_nhair_fro2_ovl_alp.bmp.png",
        "eve_nhair_long_ovl_alp.bmp.png",
        "eve_nhair_shade_ovl_alp.bmp.png",
        "eve_nhair_top_ovl_alp.bmp.png",
        "eve_rider_belt.bmp.png",
        "eve_rider_buckle.bmp.png",
        "eve_rider_collar.bmp.png",
        "eve_riders_arm.bmp.png",
        "eve_riders_body.bmp.png",
        "eve_riders_button_ovl_alp.bmp.png",
        "eve_riders_under_fix.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 EVA Naked")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 EVA Naked");
                modelFile = "MGS3 Eva Naked.obj";
                mtlFile = "MGS3 Eva Naked.mtl";
                textureFiles = new string[]
                {
        "eve_def_teeth.bmp.png",
        "eve_eye.bmp.png",
        "eve_eyelashes_ovl_alp.bmp.png",
        "eve_naked_arm_new.bmp.png",
        "eve_naked_body_fix.bmp.png",
        "eve_naked_face_fix.bmp.png",
        "eve_naked_foot_new.bmp.png",
        "eve_naked_hand_new.bmp.png",
        "eve_naked_leg_new.bmp.png",
        "eve_naked_pants.bmp.png",
        "eve_naked_under_fix.bmp.png",
        "eve_nhair_fro1_ovl_alp.bmp.png",
        "eve_nhair_fro2_ovl_alp.bmp.png",
        "eve_nhair_long_ovl_alp.bmp.png",
        "eve_nhair_shade_ovl_alp.bmp.png",
        "eve_nhair_top_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Granin")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Granin");
                modelFile = "MGS3 Granin.obj";
                mtlFile = "MGS3 Granin.mtl";
                textureFiles = new string[]
                {
        "gra_dead_hair_ovl_alp.bmp.png",
        "gra_dead_matuge_alp_ovl.bmp.png",
        "gra_def_arm.bmp.png",
        "gra_def_body.bmp.png",
        "gra_def_body_vest.bmp.png",
        "gra_def_botan2_ovl_alp.bmp.png",
        "gra_def_botan_ovl_alp.bmp.png",
        "gra_def_eye.bmp.png",
        "gra_def_face.bmp.png",
        "gra_def_foot.bmp.png",
        "gra_def_foot_s.bmp.png",
        "gra_def_hand.bmp.png",
        "gra_def_medal_a.bmp.png",
        "gra_def_medal_b.bmp.png",
        "gra_def_medal_c.bmp.png",
        "gra_def_medal_rening.bmp.png",
        "gra_def_nekutai.bmp.png",
        "gra_def_shatu.bmp.png",
        "sna_mgs3_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Granin Dead")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Granin Dead");
                modelFile = "MGS3 Granin Dead.obj";
                mtlFile = "MGS3 Granin Dead.mtl";
                textureFiles = new string[]
                {
        "gra_dead_arm.bmp.png",
        "gra_dead_body.bmp.png",
        "gra_dead_body_vest.bmp.png",
        "gra_dead_botan2_ovl_alp.bmp.png",
        "gra_dead_botan_ovl_alp.bmp.png",
        "gra_dead_eye.bmp.png",
        "gra_dead_face.bmp.png",
        "gra_dead_foot.bmp.png",
        "gra_dead_foot_s.bmp.png",
        "gra_dead_foot_s2.bmp.png",
        "gra_dead_foot_s2cap.bmp.png",
        "gra_dead_hair_ovl_alp.bmp.png",
        "gra_dead_hand.bmp.png",
        "gra_dead_hasshinki.bmp.png",
        "gra_dead_matuge_alp_ovl.bmp.png",
        "gra_dead_medal_a.bmp.png",
        "gra_dead_medal_b.bmp.png",
        "gra_dead_medal_c.bmp.png",
        "gra_dead_medal_rening.bmp.png",
        "gra_dead_nekutai.bmp.png",
        "gra_dead_shatu.bmp.png",
        "gra_dead_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Maintenance Worker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Maintenance Worker");
                modelFile = "MGS3 Maintenance Worker.obj";
                mtlFile = "MGS3 Maintenance Worker.mtl";
                textureFiles = new string[]
                {
        "gpi_foot_f.bmp.png",
        "gpi_foot_r.bmp.png",
        "gpi_foot_u.bmp.png",
        "mnt_arm_def.bmp.png",
        "mnt_body_def.bmp.png",
        "mnt_def_eye_mu_c.bmp.png",
        "mnt_def_face_c.bmp.png",
        "mnt_foot_def.bmp.png",
        "mnt_hand_def.bmp.png",
        "mnt_shl_def.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Major Ocelot")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Major Ocelot");
                modelFile = "MGS3 Major Ocelot.obj";
                mtlFile = "MGS3 Major Ocelot.mtl";
                textureFiles = new string[]
                {
        "e_akm.bmp.png",
        "oce_body_belt_alp_ovl.bmp.png",
        "oce_def_bere.bmp.png",
        "oce_def_body.bmp.png",
        "oce_def_body_belt.bmp.png",
        "oce_def_boots1.bmp.png",
        "oce_def_boots2.bmp.png",
        "oce_def_boots3.bmp.png",
        "oce_def_boots4.bmp.png",
        "oce_def_boots5.bmp.png",
        "oce_def_boots5_2_alp_ovl.bmp.png",
        "oce_def_bullet1.bmp.png",
        "oce_def_bullet2.bmp.png",
        "oce_def_eye.bmp.png",
        "oce_def_face.bmp.png",
        "oce_def_grene-do.bmp.png",
        "oce_def_gun.bmp.png",
        "oce_def_gunbelt.bmp.png",
        "oce_def_hand.bmp.png",
        "oce_def_holster.bmp.png",
        "oce_def_mafura.bmp.png",
        "oce_def_makarofu.bmp.png",
        "oce_def_matuge_alp_ovl.bmp.png",
        "oce_def_pin_alp_ovl.bmp.png",
        "oce_def_teeth.bmp.png",
        "oce_def_wanshou.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Major Zero")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Major Zero");
                modelFile = "MGS3 Major Zero.obj";
                mtlFile = "MGS3 Major Zero.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "zro_def_arm.bmp.png",
        "zro_def_body.bmp.png",
        "zro_def_boot.bmp.png",
        "zro_def_eri.bmp_15527f701d8cd045dc161c08e82d4c5e.png",
        "zro_def_eri_bk.bmp.png",
        "zro_def_eye.bmp.png",
        "zro_def_face.bmp.png",
        "zro_def_hair_back.bmp.png",
        "zro_def_hair_fro_ovl_alp.bmp.png",
        "zro_def_hand.bmp.png",
        "zro_def_leg.bmp.png",
        "zro_def_tai.bmp.png",
        "zro_def_waist.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Major Zero Headphones")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Major Zero Headphones");
                modelFile = "MGS3 Major Zero Headphones.obj";
                mtlFile = "MGS3 Major Zero Headphones.mtl";
                textureFiles = new string[]
                {
        "gpi_headphone.bmp.png",
        "gpi_hpline_ovl__alp.bmp.png",
        "gpi_income.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "zro_def_arm.bmp.png",
        "zro_def_body.bmp.png",
        "zro_def_boot.bmp.png",
        "zro_def_eri.bmp_15527f701d8cd045dc161c08e82d4c5e.png",
        "zro_def_eri_bk.bmp.png",
        "zro_def_eye.bmp.png",
        "zro_def_face.bmp.png",
        "zro_def_hair_back.bmp.png",
        "zro_def_hair_fro_ovl_alp.bmp.png",
        "zro_def_hand.bmp.png",
        "zro_def_leg.bmp.png",
        "zro_def_tai.bmp.png",
        "zro_def_waist.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Major Zero Suit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Major Zero Suit");
                modelFile = "MGS3 Major Zero Suit.obj";
                mtlFile = "MGS3 Major Zero Suit.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "zro_def_eye.bmp.png",
        "zro_def_face.bmp.png",
        "zro_def_hair_back.bmp.png",
        "zro_def_hair_fro_ovl_alp.bmp.png",
        "zro_def_hand.bmp.png",
        "zro_suit_bottun_ovl_alp.bmp.png",
        "zro_suit_dmo_boots_new.bmp.png",
        "zro_suit_dmo_idcard.bmp.png",
        "zro_suit_dmo_shurts.bmp.png",
        "zro_suit_dmo_suits.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 MiG Pilot")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 MiG Pilot");
                modelFile = "MGS3 MiG Pilot.obj";
                mtlFile = "MGS3 MiG Pilot.mtl";
                textureFiles = new string[]
                {
        "mig_arm.bmp.png",
        "mig_body.bmp.png",
        "mig_body_leg.bmp.png",
        "mig_body_parts1.bmp.png",
        "mig_body_parts2_ovl_alp.bmp.png",
        "mig_face.bmp.png",
        "mig_foot.bmp.png",
        "mig_foot_r.bmp.png",
        "mig_hand.bmp.png",
        "mig_helm_ovl_alp.bmp.png",
        "mig_met.bmp.png",
        "mig_over_helm_ovl_alp.bmp.png",
        "mig_parts01_ovl_alp.bmp.png",
        "mig_tube.bmp.png",
        "mig_tube1.bmp.png",
        "mig_tube2.bmp.png",
        "sig_foot_u.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Paramedic")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Paramedic");
                modelFile = "MGS3 Paramedic.obj";
                mtlFile = "MGS3 Paramedic.mtl";
                textureFiles = new string[]
                {
        "med_arm.bmp.png",
        "med_best.bmp.png",
        "med_boots.bmp.png",
        "med_boots_himo_ovl_alp.bmp.png",
        "med_bottun_ovl_alp.bmp.png",
        "med_coat.bmp.png",
        "med_eye.bmp.png",
        "med_eye_brow_ovl_alp.bmp.png",
        "med_face.bmp.png",
        "med_foot.bmp.png",
        "med_hair.bmp.png",
        "med_hair_front_ovl_alp.bmp.png",
        "med_hand.bmp.png",
        "med_jipper.bmp.png",
        "med_midle_hair_cal_ovl_alp.bmp.png",
        "med_skart.bmp.png",
        "med_suits.bmp.png",
        "med_tai_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Paramedic Headphones")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Paramedic Headphones");
                modelFile = "MGS3 Paramedic Headphones.obj";
                mtlFile = "MGS3 Paramedic Headphones.mtl";
                textureFiles = new string[]
                {
        "gpi_headphone.bmp.png",
        "gpi_hpline_ovl__alp.bmp.png",
        "gpi_income.bmp.png",
        "med_arm.bmp.png",
        "med_best.bmp.png",
        "med_boots.bmp.png",
        "med_boots_himo_ovl_alp.bmp.png",
        "med_bottun_ovl_alp.bmp.png",
        "med_coat.bmp.png",
        "med_eye.bmp.png",
        "med_eye_brow_ovl_alp.bmp.png",
        "med_face.bmp.png",
        "med_foot.bmp.png",
        "med_hair.bmp.png",
        "med_hair_front_ovl_alp.bmp.png",
        "med_hand.bmp.png",
        "med_jipper.bmp.png",
        "med_midle_hair_cal_ovl_alp.bmp.png",
        "med_skart.bmp.png",
        "med_suits.bmp.png",
        "med_tai_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Paramedic Suit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Paramedic Suit");
                modelFile = "MGS3 Paramedic Suit.obj";
                mtlFile = "MGS3 Paramedic Suit.mtl";
                textureFiles = new string[]
                {
        "med_arm_ws.bmp.png",
        "med_best_ws.bmp.png",
        "med_bottun_suits_ovl_alp.bmp.png",
        "med_eye.bmp.png",
        "med_eye_brow_ovl_alp.bmp.png",
        "med_face.bmp.png",
        "med_foot.bmp.png",
        "med_hair.bmp.png",
        "med_hair_front_ovl_alp.bmp.png",
        "med_hand_ws.bmp.png",
        "med_heel.bmp.png",
        "med_idcard.bmp.png",
        "med_midle_hair_cal_ovl_alp.bmp.png",
        "med_skarf.bmp.png",
        "med_skarf_tai.bmp.png",
        "med_skart_ws.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Pilot")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Pilot");
                modelFile = "MGS3 Pilot.obj";
                mtlFile = "MGS3 Pilot.mtl";
                textureFiles = new string[]
                {
        "lms_arm.bmp.png",
        "lms_arm_skin.bmp.png",
        "lms_body.bmp.png",
        "lms_face.bmp.png",
        "lms_foot.bmp.png",
        "lms_halo_mask_demo_ovl_alp.bmp.png",
        "lms_hand.bmp.png",
        "lms_helm_ovl_alp.bmp.png",
        "lms_met.bmp.png",
        "lms_met_white.bmp.png",
        "lms_parts01_ovl_alp.bmp.png",
        "lms_parts02a_ovl_alp.bmp.png",
        "lms_parts02b.bmp.png",
        "sna_mgs3_antena.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 President")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 President");
                modelFile = "MGS3 President.obj";
                mtlFile = "MGS3 President.mtl";
                textureFiles = new string[]
                {
        "pre_boots.bmp.png",
        "pre_bottun_suits_ovl_alp.bmp.png",
        "pre_eye.bmp.png",
        "pre_face.bmp.png",
        "pre_hand.bmp.png",
        "pre_head_back.bmp.png",
        "pre_shurts.bmp.png",
        "pre_suits.bmp.png",
        "pre_tai.bmp.png",
        "pre_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Raikov Naked")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Raikov Naked");
                modelFile = "MGS3 Raikov Naked.obj";
                mtlFile = "MGS3 Raikov Naked.mtl";
                textureFiles = new string[]
                {
        "ivn_briefs.bmp.png",
        "ivn_briefs_point_ovl_alp.bmp.png",
        "ivn_face_def.bmp.png",
        "ivn_hair_base.bmp.png",
        "ivn_hair_base_ovl_alp.bmp.png",
        "ivn_hair_front_ovl_alp.bmp.png",
        "ivn_hair_layer_ovl_alp.bmp.png",
        "ivn_kin.bmp.png",
        "ivn_mtg_alp_ovl.bmp.png",
        "ivn_naked_body.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_mgs3_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Scientist Dead")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Scientist Dead");
                modelFile = "MGS3 Scientist Dead.obj";
                mtlFile = "MGS3 Scientist Dead.mtl";
                textureFiles = new string[]
                {
        "wor_def_armband_alp_ovl.bmp.png",
        "wor_def_grass_ovl_alp.bmp.png",
        "wor_pendulum_staffproof.bmp.png",
        "wor_suspend_boots3.bmp.png",
        "wor_suspend_eye.bmp.png",
        "wor_suspend_face.bmp.png",
        "wor_suspend_facedam_ovl_alp.bmp.png",
        "wor_suspend_foot1.bmp.png",
        "wor_suspend_foot2.bmp.png",
        "wor_suspend_hand.bmp.png",
        "wor_suspend_leg.bmp.png",
        "wor_suspend_shurts.bmp.png",
        "wor_suspend_white.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Secretary of Defense")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Secretary of Defense");
                modelFile = "MGS3 Secretary of Defense.obj";
                mtlFile = "MGS3 Secretary of Defense.mtl";
                textureFiles = new string[]
                {
        "sec_boots_new.bmp.png",
        "sec_bottun_suits_ovl_alp.bmp.png",
        "sec_eye.bmp.png",
        "sec_face.bmp.png",
        "sec_grass_ovl_alp.bmp.png",
        "sec_hair_back.bmp.png",
        "sec_hand.bmp.png",
        "sec_shurts.bmp.png",
        "sec_suits.bmp.png",
        "sec_tai.bmp.png",
        "sec_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Sigint")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Sigint");
                modelFile = "MGS3 Sigint.obj";
                mtlFile = "MGS3 Sigint.mtl";
                textureFiles = new string[]
                {
        "sig_body_chest.bmp.png",
        "sig_body_leg.bmp.png",
        "sig_botton_ovl_alp.bmp.png",
        "sig_cap.bmp.png",
        "sig_cap_logo_ovl_alp.bmp.png",
        "sig_cloth.bmp.png",
        "sig_eye.bmp.png",
        "sig_face.bmp.png",
        "sig_foot_f.bmp.png",
        "sig_foot_r.bmp.png",
        "sig_foot_u.bmp.png",
        "sig_hand_low.bmp.png",
        "sig_idcard.bmp.png",
        "sig_me.bmp.png",
        "sig_mgs3_teeth.bmp.png",
        "sig_mtg_ovl_alp.bmp.png",
        "sig_paeker.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Halo Jump")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Halo Jump");
                modelFile = "MGS3 Snake Halo.obj";
                mtlFile = "MGS3 Snake Halo.mtl";
                textureFiles = new string[]
                {
        "00765dfa-sna_def_olive~0063d2755986c53e88ca8238dcc91517.png",
        "cord_ovl_alp.bmp.png",
        "cqcc_tex02.bmp.png",
        "cqck_tex02.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_halo_def.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_halo_alt.bmp.png",
        "sna_halo_arm_def.bmp.png",
        "sna_halo_chut_open.bmp.png",
        "sna_halo_hand.bmp.png",
        "sna_halo_hips_op.bmp.png",
        "sna_halo_para_close.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_arm.bmp.png",
        "sna_mgs3_belt.bmp_77380cc45d92a52a1e8da9f59a6ea891.png",
        "sna_mgs3_belt_side.bmp.png",
        "sna_mgs3_halo_belt_op.bmp.png",
        "sna_mgs3_halo_chut.bmp.png",
        "sna_mgs3_halo_front_op.bmp.png",
        "sna_mgs3_halo_head.bmp.png",
        "sna_mgs3_halo_head_side.bmp.png",
        "sna_mgs3_halo_head_top.bmp.png",
        "sna_mgs3_halo_lens_bld095_ovl_alp.bmp.png",
        "sna_mgs3_halo_mask_demo.bmp.png",
        "sna_mgs3_halo_mbelt_alp_ovl.bmp_538690604392088e5d11997a4d489646.png",
        "sna_mgs3_halo_shl_op.bmp.png",
        "sna_mgs3_halo_tape.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_belt.bmp_387276427ee88d88dbacbd0ae1f73fd7.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_snif_def.bmp.png",
        "svknf_grip.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Maintenance")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Maintenance");
                modelFile = "MGS3 Snake Maintenance.obj";
                mtlFile = "MGS3 Snake Maintenance.mtl";
                textureFiles = new string[]
                {
        "sna_bandana_def.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hair_front_ovl_alp.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
        "sna_hair_back_ovl_alp.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_gantai.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mnt_arm_def.bmp.png",
        "sna_mnt_body_def.bmp.png",
        "sna_mnt_body_front.bmp.png",
        "sna_mnt_boots.bmp.png",
        "sna_mnt_foot_def.bmp.png",
        "sna_mnt_hand.bmp.png",
        "sna_mnt_neck.bmp.png",
        "sna_mnt_rogo_ovl_alp.bmp.png",
        "sna_mnt_shl_def.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Naked")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Naked");
                modelFile = "MGS3 Snake Naked.obj";
                mtlFile = "MGS3 Snake Naked.mtl";
                textureFiles = new string[]
                {
        "sna_def_vr_eye.bmp.png",
        "sna_face_vr_def_hi.bmp.png",
        "sna_flirt_kizu_ovl_alp.bmp.png",
        "sna_flirt_wound_ovl_alp.bmp.png",
        "sna_foot_naked_flirt.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_hair_base.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_body_flirt.bmp.png",
        "sna_naked_hand_flirt.bmp.png",
        "sna_vr_hair_back_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Naked Eyepatch")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Naked Eyepatch");
                modelFile = "MGS3 Snake Naked Eyepatch.obj";
                mtlFile = "MGS3 Snake Naked Eyepatch.mtl";
                textureFiles = new string[]
                {
        "sna_def_vr_eye.bmp.png",
        "sna_face_vr_def_hi.bmp.png",
        "sna_flirt_kizu_ovl_alp.bmp.png",
        "sna_flirt_wound_ovl_alp.bmp.png",
        "sna_foot_naked_flirt.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_hair_base.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_gantai.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_body_flirt.bmp.png",
        "sna_naked_hand_flirt.bmp.png",
        "sna_vr_hair_back_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Scientist")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Scientist");
                modelFile = "MGS3 Snake Scientist.obj";
                mtlFile = "MGS3 Snake Scientist.mtl";
                textureFiles = new string[]
                {
        "sna_bandana_def.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hair_front_ovl_alp.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
        "sna_hair_back_ovl_alp.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_wor_button_ovl_alp.bmp.png",
        "sna_wor_def_shatu.bmp.png",
        "sna_wor_def_staffproof.bmp.png",
        "sna_wor_def_white.bmp.png",
        "sna_wor_glass.bmp.png",
        "sna_wor_hand_hi.bmp.png",
        "sna_wor_white_armband_alp_ovl.bmp.png",
        "wor_def_boots1.bmp.png",
        "wor_def_boots2.bmp.png",
        "wor_def_boots3.bmp.png",
        "wor_def_leg.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake SE Eyepatch")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake SE Eyepatch");
                modelFile = "MGS3 Snake SE Eyepatch.obj";
                mtlFile = "MGS3 Snake SE Eyepatch.mtl";
                textureFiles = new string[]
                {
        "00765dfa-sna_def_olive~0063d2755986c53e88ca8238dcc91517.png",
        "cord_ovl_alp.bmp.png",
        "cqcc_tex02.bmp.png",
        "cqck_tex02.bmp.png",
        "sna_bandana_def.bmp.png",
        "sna_bk.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hair_front_ovl_alp.bmp.png",
        "sna_def_hand.bmp.png",
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
        "sna_mgs3_gantai.bmp.png",
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
        "svknf_grip.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake SE Injured")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake SE Injured");
                modelFile = "MGS3 Snake SE Injured.obj";
                mtlFile = "MGS3 Snake SE Injured.mtl";
                textureFiles = new string[]
                {
        "sna_bandana_def.bmp.png",
        "sna_cell_kizu_ovl_alp.bmp.png",
        "sna_cell_wound_ovl_alp.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hair_front_ovl_alp.bmp.png",
        "sna_def_hand.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_cell.bmp.png",
        "sna_foot_naked_tor.bmp_73f755c5eb2ba5efb9fbd155062db075.png",
        "sna_foot_saa_wound_ovl_alp.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_hair_back_ovl_alp.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_bandage_arml.bmp.png",
        "sna_mgs3_bandage_body.bmp.png",
        "sna_mgs3_belt_side.bmp.png",
        "sna_mgs3_belt_tor.bmp.png",
        "sna_mgs3_gantai.bmp.png",
        "sna_mgs3_halo_tape.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_belt_tor.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_body_dam.bmp.png",
        "sna_naked_cord_ovl_alp.bmp.png",
        "sna_snif_def.bmp.png",
        "svknf_grip.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Suit Endgame")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Suit Endgame");
                modelFile = "MGS3 Snake Epiloge Suit.obj";
                mtlFile = "MGS3 Snake Epiloge Suit.mtl";
                textureFiles = new string[]
                {
        "cia_eye.bmp.png",
        "pre_item_medal_body.bmp.png",
        "sna_eye_sdw_ovl_alp.bmp.png",
        "sna_face_ed_def.bmp.png",
        "sna_hair_base.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_arm_suit.bmp.png",
        "sna_mgs3_ber_suit.bmp.png",
        "sna_mgs3_body_neck_suit.bmp.png",
        "sna_mgs3_body_ovl_alp.bmp.png",
        "sna_mgs3_body_suit.bmp.png",
        "sna_mgs3_body_suit_bh_ovl_alp.bmp.png",
        "sna_mgs3_body_suit_pok.bmp.png",
        "sna_mgs3_body_under_suit.bmp.png",
        "sna_mgs3_foot_suit.bmp.png",
        "sna_mgs3_gantai.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_suit_fox_ovl_alp.bmp.png",
        "sna_suit_hand.bmp.png",
        "sna_suit_tears.bmp.png",
        "sna_tears_ovl_alp.bmp.png",
        "sna_vr_hair_back_ovl_alp.bmp.png",
        "sor_foot_def.bmp.png",
        "sor_foot_under.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Torture Room")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Torture Room");
                modelFile = "MGS3 Snake Torture.obj";
                mtlFile = "MGS3 Snake Torture.mtl";
                textureFiles = new string[]
                {
        "sna_bandana_def.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hair_front_ovl_alp.bmp.png",
        "sna_def_hand.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_dam_eye_ovl_alp.bmp.png",
        "sna_face_dam_max.bmp.png",
        "sna_foot_naked_tor.bmp_73f755c5eb2ba5efb9fbd155062db075.png",
        "sna_foot_saa_wound_ovl_alp.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_hair_back_ovl_alp.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_incon_ovl_alp.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_belt_side.bmp.png",
        "sna_mgs3_belt_tor.bmp.png",
        "sna_mgs3_halo_tape.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_belt_tor.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_body_dam_max.bmp.png",
        "sna_snif_def.bmp.png",
        "sna_vinyl_wound_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Torture Room Bag")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Torture Room Bag");
                modelFile = "MGS3 Snake Torture Bag.obj";
                mtlFile = "MGS3 Snake Torture Bag.mtl";
                textureFiles = new string[]
                {
        "sna_bandana_def.bmp.png",
        "sna_def_hair_base.bmp.png",
        "sna_def_hand.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_dam_eye_ovl_alp.bmp.png",
        "sna_face_dam_max.bmp.png",
        "sna_foot_naked_tor.bmp_73f755c5eb2ba5efb9fbd155062db075.png",
        "sna_foot_saa_wound_ovl_alp.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_incon_ovl_alp.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_belt_side.bmp.png",
        "sna_mgs3_belt_tor.bmp.png",
        "sna_mgs3_halo_tape.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_belt_tor.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_body_dam_max.bmp.png",
        "sna_snif_def.bmp.png",
        "sna_vinyl_def_ovl_alp.bmp.png",
        "sna_vinyl_water_ovl_alp.bmp.png",
        "sna_vinyl_wound_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake Tuxedo")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake Tuxedo");
                modelFile = "MGS3 Snake Tuxedo.obj";
                mtlFile = "MGS3 Snake Tuxedo.mtl";
                textureFiles = new string[]
                {
        "sna_def_vr_eye.bmp.png",
        "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
        "sna_hair_base.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_mgs3_gantai.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mgs3_txd_arm.bmp.png",
        "sna_mgs3_txd_button_ovl_alp.bmp.png",
        "sna_mgs3_txd_collar.bmp.png",
        "sna_mgs3_txd_hand.bmp.png",
        "sna_mgs3_txd_jacket.bmp.png",
        "sna_mgs3_txd_leg.bmp.png",
        "sna_mgs3_txd_shirt.bmp.png",
        "sna_mgs3_txd_shoe.bmp.png",
        "sna_mgs3_txd_vest.bmp.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_vr_hair_back_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Snake VM Injured")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake VM Injured");
                modelFile = "MGS3 Snake VM Injured.obj";
                mtlFile = "MGS3 Snake VM Injured.mtl";
                textureFiles = new string[]
                {
        "cqcc_tex02.bmp.png",
        "cqck_tex02.bmp.png",
        "sna_bk.bmp.png",
        "sna_def_hand.bmp.png",
        "sna_def_vr_eye.bmp.png",
        "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
        "sna_foot_naked_tor.bmp.png",
        "sna_foot_sp.bmp.png",
        "sna_foot_vr.bmp.png",
        "sna_hair_base.bmp.png",
        "sna_hair_front_ovl_alp.bmp.png",
        "sna_hair_layer_ovl_alp.bmp.png",
        "sna_item_hm.bmp.png",
        "sna_mgs3_antena.bmp.png",
        "sna_mgs3_bandage_arml.bmp.png",
        "sna_mgs3_bandage_body.bmp.png",
        "sna_mgs3_belt_side.bmp.png",
        "sna_mgs3_gh.bmp.png",
        "sna_mgs3_gun_hol.bmp.png",
        "sna_mgs3_halo_tape.bmp.png",
        "sna_mgs3_hh.bmp.png",
        "sna_mgs3_musen.bmp.png",
        "sna_mgs3_naked_body.bmp.png",
        "sna_mgs3_teeth.bmp.png",
        "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
        "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
        "sna_mtg_ovl_alp.bmp.png",
        "sna_naked_belt_waist.bmp.png",
        "sna_naked_cord_ovl_alp.bmp.png",
        "sna_snif_def.bmp.png",
        "sna_vr_hair_back_ovl_alp.bmp.png",
        "svknf_grip.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Sokolov Coat")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Sokolov Coat");
                modelFile = "MGS3 Sokolov Coat.obj";
                mtlFile = "MGS3 Sokolov Coat.mtl";
                textureFiles = new string[]
                {
        "sok_coat_body.bmp.png",
        "sok_coat_coat.bmp.png",
        "sok_coat_eye.bmp.png",
        "sok_coat_face.bmp.png",
        "sok_coat_hair_alp_ovl.bmp.png",
        "sok_coat_lupe2_ovl_alp.bmp.png",
        "sok_coat_lupe_ovl_alp.bmp.png",
        "sok_coat_lupe_pin.bmp.png",
        "sok_coat_matuge_alp_ovl.bmp.png",
        "sok_coat_nekutai.bmp.png",
        "sok_coat_shatu.bmp.png",
        "sok_coat_teeth.bmp.png",
        "wor_def_boots1.bmp.png",
        "wor_def_boots2.bmp.png",
        "wor_def_boots3.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Sokolov Scientist")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Sokolov Scientist");
                modelFile = "MGS3 Sokolov Scientist.obj";
                mtlFile = "MGS3 Sokolov Scientist.mtl";
                textureFiles = new string[]
                {
        "sna_wor_def_white.bmp.png",
        "sna_wor_white_armband_alp_ovl.bmp.png",
        "sok_coat_eye.bmp.png",
        "sok_coat_face.bmp.png",
        "sok_coat_hair_alp_ovl.bmp.png",
        "sok_coat_lupe2_ovl_alp.bmp.png",
        "sok_coat_lupe_ovl_alp.bmp.png",
        "sok_coat_lupe_pin.bmp.png",
        "sok_coat_matuge_alp_ovl.bmp.png",
        "sok_coat_nekutai.bmp.png",
        "sok_coat_shatu.bmp.png",
        "sok_coat_teeth.bmp.png",
        "sok_white_body.bmp.png",
        "sok_white_botan_ovl_alp.bmp.png",
        "sok_white_hand.bmp.png",
        "sok_white_staffproof.bmp.png",
        "wor_def_boots1.bmp.png",
        "wor_def_boots2.bmp.png",
        "wor_def_boots3.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The End")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The End");
                modelFile = "MGS3 The End.obj";
                mtlFile = "MGS3 The End.mtl";
                textureFiles = new string[]
                {
        "end_def_hand.bmp.png",
        "end_ela01_alp_ovl.bmp.png",
        "end_item_bk.bmp.png",
        "end_item_bk_side.bmp.png",
        "end_item_case_def.bmp.png",
        "end_item_hp_coad.bmp.png",
        "end_item_hp_def.bmp.png",
        "end_item_hp_ma.bmp.png",
        "end_item_hp_mb.bmp.png",
        "end_item_hp_mc.bmp.png",
        "end_item_hp_md.bmp.png",
        "end_item_hp_me.bmp.png",
        "end_mgs3_arm_def.bmp.png",
        "end_mgs3_belt.bmp.png",
        "end_mgs3_belt_bak.bmp.png",
        "end_mgs3_belt_def.bmp.png",
        "end_mgs3_belt_shl.bmp.png",
        "end_mgs3_body_def.bmp.png",
        "end_mgs3_body_gr_ovl_alp.bmp.png",
        "end_mgs3_eye.bmp.png",
        "end_mgs3_eye_side.bmp.png",
        "end_mgs3_face_def.bmp.png",
        "end_mgs3_face_young.bmp.png",
        "end_mgs3_foot.bmp.png",
        "end_mgs3_foot_def.bmp.png",
        "end_mgs3_foot_under.bmp.png",
        "end_mgs3_gun_belt.bmp.png",
        "end_mgs3_hair_f_ovl_alp.bmp.png",
        "end_mgs3_neck_def.bmp.png",
        "end_mgs3_wpl.bmp.png",
        "end_young_hand.bmp.png",
        "sna_mgs3_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Fear")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Fear");
                modelFile = "MGS3 The Fear.obj";
                mtlFile = "MGS3 The Fear.mtl";
                textureFiles = new string[]
                {
        "fea_arm.bmp.png",
        "fea_backpack.bmp.png",
        "fea_body_fix.bmp.png",
        "fea_case.bmp.png",
        "fea_def_boot.bmp.png",
        "fea_def_dmo_hand.bmp.png",
        "fea_def_dmo_hand_belt.bmp.png",
        "fea_def_teeth.bmp.png",
        "fea_eye.bmp.png",
        "fea_face_fix.bmp_7f1d3c13eea0ad4841af24e2b2561093.png",
        "fea_hair_back_fix_ovl_alp.bmp.png",
        "fea_hair_top_ovl_alp.bmp.png",
        "fea_hair_top_white_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Fury Helmet")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Fury Helmet");
                modelFile = "MGS3 The Fury Helmet.obj";
                mtlFile = "MGS3 The Fury Helmet.mtl";
                textureFiles = new string[]
                {
        "fur_arm_belt_ovl_alp.bmp.png",
        "fur_comp_pipe_ovl_alp.bmp.png",
        "fur_def_body.bmp_8a379e3e4ddc3ceb28bb3ab6ace95b83.png",
        "fur_def_hand.bmp.png",
        "fur_def_met.bmp_428e0e39b43a929f8ebd937b960ebbbf.png",
        "fur_def_met_display_ovl_alp.bmp_d26cd6082b60e4c7e4fdfa55632ea7cb.png",
        "fur_def_met_displayside_ovl_alp.bmp.png",
        "fur_def_tank_display_ovl_alp.bmp.png",
        "fur_eye.bmp.png",
        "fur_eyebrow_ovl_alp.bmp.png",
        "fur_face.bmp.png",
        "fur_face_musk.bmp.png",
        "fur_foot_marking_ovl_alp.bmp.png",
        "fur_glip.bmp.png",
        "fur_grass_met_ovl_alp.bmp.png",
        "fur_jet_rock_ovl_alp.bmp.png",
        "fur_met_pipejoint_ovl_alp.bmp_3059de9d448b96755e3e20c84d1fa64a.png",
        "fur_nosle.bmp_a54a906e32934f75db0ad015e52da6cb.png",
        "fur_pipe.bmp_d9ed02a7b93e413fa860fac86f45ab3c.png",
        "fur_tank.bmp_cfaaca9251f07ff7ce1a40dbb02cac47.png",
        "fur_tank_dekal_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Fury No Helmet")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Fury No Helmet");
                modelFile = "MGS3 The Fury No Helmet.obj";
                mtlFile = "MGS3 The Fury No Helmet.mtl";
                textureFiles = new string[]
                {
        "fur_arm_belt_ovl_alp.bmp.png",
        "fur_comp_pipe_ovl_alp.bmp.png",
        "fur_def_body.bmp_8a379e3e4ddc3ceb28bb3ab6ace95b83.png",
        "fur_def_hand.bmp.png",
        "fur_def_met.bmp_428e0e39b43a929f8ebd937b960ebbbf.png",
        "fur_def_met_displayside_ovl_alp.bmp.png",
        "fur_def_tank_display_ovl_alp.bmp.png",
        "fur_eye.bmp.png",
        "fur_eyebrow_ovl_alp.bmp.png",
        "fur_face.bmp.png",
        "fur_face_musk.bmp.png",
        "fur_foot_marking_ovl_alp.bmp.png",
        "fur_glip.bmp.png",
        "fur_jet_rock_ovl_alp.bmp.png",
        "fur_met_pipejoint_ovl_alp.bmp_3059de9d448b96755e3e20c84d1fa64a.png",
        "fur_nosle.bmp_a54a906e32934f75db0ad015e52da6cb.png",
        "fur_pipe.bmp_d9ed02a7b93e413fa860fac86f45ab3c.png",
        "fur_tank.bmp_cfaaca9251f07ff7ce1a40dbb02cac47.png",
        "fur_tank_dekal_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Pain Mask")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Pain Mask");
                modelFile = "MGS3 The Pain Mask.obj";
                mtlFile = "MGS3 The Pain Mask.mtl";
                textureFiles = new string[]
                {
        "pai_def_body.bmp.png",
        "pai_def_boots.bmp.png",
        "pai_def_bp.bmp.png",
        "pai_def_bp_belt.bmp.png",
        "pai_def_bug.bmp.png",
        "pai_def_eye_hi.bmp.png",
        "pai_def_face.bmp.png",
        "pai_def_hand_hi.bmp.png",
        "pai_def_mask.bmp.png",
        "pai_def_mask_h_ovl_alp.bmp.png",
        "pai_def_matuge_alp_ovl.bmp.png",
        "pai_def_vest.bmp.png",
        "sna_mgs3_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Pain No Mask")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Pain No Mask");
                modelFile = "MGS3 The Pain.obj";
                mtlFile = "MGS3 The Pain.mtl";
                textureFiles = new string[]
                {
        "pai_def_body.bmp.png",
        "pai_def_boots.bmp.png",
        "pai_def_bp.bmp.png",
        "pai_def_bp_belt.bmp.png",
        "pai_def_bug.bmp.png",
        "pai_def_eye_hi.bmp.png",
        "pai_def_face.bmp.png",
        "pai_def_hand_hi.bmp.png",
        "pai_def_matuge_alp_ovl.bmp.png",
        "pai_def_vest.bmp.png",
        "sna_mgs3_teeth.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Sorrow Bleeding Eyes")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Sorrow Bleeding Eyes");
                modelFile = "MGS3 The Sorrow Bleeding Eyes.obj";
                mtlFile = "MGS3 The Sorrow Bleeding Eyes.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "sor_blood_tears_left_ovl_alp.bmp.png",
        "sor_foot_def.bmp.png",
        "sor_foot_under.bmp.png",
        "sor_hair_back_ovl_alp.bmp.png",
        "sor_hair_front_ovl_alp.bmp.png",
        "sor_hand_def.bmp.png",
        "sor_hi_ela01_alp_ovl.bmp.png",
        "sor_item_glass_break.bmp.png",
        "sor_item_glass_def.bmp.png",
        "sor_item_glass_def_ovl_alp.bmp.png",
        "sor_item_glass_jnt.bmp.png",
        "sor_item_glass_ref_ovl_alp.bmp.png",
        "sor_item_hol_def.bmp.png",
        "sor_mgs3_belt.bmp.png",
        "sor_mgs3_body_over.bmp.png",
        "sor_mgs3_body_shld.bmp.png",
        "sor_mgs3_body_under.bmp.png",
        "sor_mgs3_eye_hi.bmp.png",
        "sor_mgs3_face_def.bmp.png",
        "sor_mgs3_foot_goast.bmp.png",
        "sor_mgs3_gun.bmp.png",
        "sor_mgs3_gun_belt.bmp.png",
        "sor_mgs3_gun_belt_fuck.bmp.png",
        "sor_mgs3_gun_belt_left.bmp.png",
        "sor_mgs3_gun_belt_shl.bmp.png",
        "sor_mgs3_gun_grip.bmp.png",
        "sor_mgs3_wpl.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Sorrow Main")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Sorrow Main");
                modelFile = "MGS3 The Sorrow.obj";
                mtlFile = "MGS3 The Sorrow.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "sor_foot_def.bmp.png",
        "sor_foot_under.bmp.png",
        "sor_hair_back_ovl_alp.bmp.png",
        "sor_hair_front_ovl_alp.bmp.png",
        "sor_hand_def.bmp.png",
        "sor_hi_ela01_alp_ovl.bmp.png",
        "sor_item_glass_def.bmp.png",
        "sor_item_glass_def_ovl_alp.bmp.png",
        "sor_item_glass_jnt.bmp.png",
        "sor_item_hol_def.bmp.png",
        "sor_mgs3_belt.bmp.png",
        "sor_mgs3_body_over.bmp.png",
        "sor_mgs3_body_shld.bmp.png",
        "sor_mgs3_body_under.bmp.png",
        "sor_mgs3_eye_hi.bmp.png",
        "sor_mgs3_face_def.bmp.png",
        "sor_mgs3_foot_goast.bmp.png",
        "sor_mgs3_gun.bmp.png",
        "sor_mgs3_gun_belt.bmp.png",
        "sor_mgs3_gun_belt_fuck.bmp.png",
        "sor_mgs3_gun_belt_left.bmp.png",
        "sor_mgs3_gun_belt_shl.bmp.png",
        "sor_mgs3_gun_grip.bmp.png",
        "sor_mgs3_wpl.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 The Sorrow Parka")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 The Sorrow Parka");
                modelFile = "MGS3 The Sorrow Parka.obj";
                mtlFile = "MGS3 The Sorrow Parka.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "sor_foot_def.bmp.png",
        "sor_foot_under.bmp.png",
        "sor_hand_def.bmp.png",
        "sor_hi_ela01_alp_ovl.bmp.png",
        "sor_item_glass_jnt_park.bmp.png",
        "sor_item_glass_park.bmp.png",
        "sor_item_glass_park_ovl_alp.bmp.png",
        "sor_mgs3_arm_park.bmp.png",
        "sor_mgs3_body_over_park.bmp.png",
        "sor_mgs3_body_park.bmp.png",
        "sor_mgs3_body_under.bmp.png",
        "sor_mgs3_eye_hi.bmp.png",
        "sor_mgs3_face_park.bmp.png",
        "sor_mgs3_foot_goast.bmp.png",
        "sor_mgs3_head_park.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 VIP A")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 VIP A");
                modelFile = "MGS3 VIP A.obj";
                mtlFile = "MGS3 VIP A.mtl";
                textureFiles = new string[]
                {
        "rus_arm_def.bmp.png",
        "rus_body_def.bmp.png",
        "rus_body_neck.bmp.png",
        "rus_body_under.bmp.png",
        "rus_body_vest.bmp.png",
        "rus_button_ovl_alp.bmp.png",
        "rus_eye.bmp.png",
        "rus_face.bmp.png",
        "rus_foot_def.bmp.png",
        "rus_foot_s.bmp.png",
        "rus_hand_def.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 VIP B")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 VIP B");
                modelFile = "MGS3 VIP B.obj";
                mtlFile = "MGS3 VIP B.mtl";
                textureFiles = new string[]
                {
        "ene_spe_boots3.bmp.png",
        "ene_spe_boots4.bmp.png",
        "oce_def_boots2.bmp.png",
        "rus_button_ovl_alp.bmp.png",
        "vip_b_body.bmp.png",
        "vip_b_eye.bmp.png",
        "vip_b_face.bmp.png",
        "vip_b_hand.bmp.png",
        "vip_b_nekutai.bmp.png",
        "vip_b_shats.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 VIP C")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 VIP C");
                modelFile = "MGS3 VIP C.obj";
                mtlFile = "MGS3 VIP C.mtl";
                textureFiles = new string[]
                {
        "ene_spe_boots3.bmp.png",
        "ene_spe_boots4.bmp.png",
        "oce_def_boots2.bmp.png",
        "rus_button_ovl_alp.bmp.png",
        "vip_c_body.bmp.png",
        "vip_c_eye.bmp.png",
        "vip_c_face.bmp.png",
        "vip_c_hand.bmp.png",
        "vip_c_nekutai.bmp.png",
        "vip_c_shats.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Volgin Coat")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Volgin Coat");
                modelFile = "MGS3 Volgin Coat.obj";
                mtlFile = "MGS3 Volgin Coat.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "thu_belt_ovl_alp.bmp.png",
        "thu_coat_foot_sole.bmp.png",
        "thu_coat_toe.bmp.png",
        "thu_def_eye.bmp.png",
        "thu_makarofu.bmp.png",
        "thu_mgs3_coat_def.bmp.png",
        "thu_mgs3_coat_hand_def.bmp.png",
        "thu_mgs3_face_def.bmp.png",
        "thu_mgs3_knee_def.bmp.png",
        "thu_mgs3_leg_def.bmp.png",
        "thu_mgs3_pin_ovl_alp.bmp.png",
        "thu_mtg_alp_ovl.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Volgin Coat (Ammo)")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Volgin Coat (Ammo)");
                modelFile = "MGS3 Volgin Coat Ammo.obj";
                mtlFile = "MGS3 Volgin Coat Ammo.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "thu_belt_ovl_alp.bmp.png",
        "thu_coat_foot_sole.bmp.png",
        "thu_coat_toe.bmp.png",
        "thu_def_eye.bmp.png",
        "thu_makarofu.bmp.png",
        "thu_mgs3_coat_def.bmp.png",
        "thu_mgs3_coat_hand_def.bmp.png",
        "thu_mgs3_face_def.bmp.png",
        "thu_mgs3_knee_def.bmp.png",
        "thu_mgs3_leg_def.bmp.png",
        "thu_mgs3_pin_ovl_alp.bmp.png",
        "thu_mtg_alp_ovl.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Volgin No Coat")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Volgin No Coat");
                modelFile = "MGS3 Volgin No Coat.obj";
                mtlFile = "MGS3 Volgin No Coat.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "thu_body_def.bmp.png",
        "thu_def_eye.bmp.png",
        "thu_mgs3_arm_def.bmp.png",
        "thu_mgs3_arm_jnt.bmp.png",
        "thu_mgs3_body_pit_ovl_alp.bmp.png",
        "thu_mgs3_body_rog_ovl_alp.bmp.png",
        "thu_mgs3_face_def.bmp.png",
        "thu_mgs3_knee_def.bmp.png",
        "thu_mgs3_leg_def.bmp.png",
        "thu_mtg_alp_ovl.bmp.png",
        "thu_pal_ovl_alp.bmp.png",
        "thu_pal_top.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Volgin No Coat (Ammo)")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Volgin No Coat (Ammo)");
                modelFile = "MGS3 Volgin No Coat Ammo.obj";
                mtlFile = "MGS3 Volgin No Coat Ammo.mtl";
                textureFiles = new string[]
                {
        "sna_mgs3_teeth.bmp.png",
        "thu_body_def.bmp.png",
        "thu_def_eye.bmp.png",
        "thu_mgs3_arm_def.bmp.png",
        "thu_mgs3_arm_jnt.bmp.png",
        "thu_mgs3_body_pit_ovl_alp.bmp.png",
        "thu_mgs3_body_rog_ovl_alp.bmp.png",
        "thu_mgs3_bull.bmp.png",
        "thu_mgs3_face_def.bmp.png",
        "thu_mgs3_knee_def.bmp.png",
        "thu_mgs3_leg_def.bmp.png",
        "thu_mtg_alp_ovl.bmp.png",
        "thu_pal_ovl_alp.bmp.png",
        "thu_pal_top.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Book")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Book");
                modelFile = "MGS3 Book.obj";
                mtlFile = "MGS3 Book.mtl";
                textureFiles = new string[]
                {
        "sub_maga.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Bucket")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Bucket");
                modelFile = "MGS3 Bucket.obj";
                mtlFile = "MGS3 Bucket.mtl";
                textureFiles = new string[]
                {
        "baketu_01.bmp.png",
        "baketu_02.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Camera A")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Camera A");
                modelFile = "MGS3 Camera A.obj";
                mtlFile = "MGS3 Camera A.mtl";
                textureFiles = new string[]
                {
        "cme_a_item.bmp.png",
        "cme_a_item_fill_ovl_alp.bmp.png",
        "cme_a_item_parts.bmp.png",
        "cme_a_item_renz.bmp.png",
        "cme_a_item_tr_renz02_ovl_alp.bmp.png",
        "cme_b_item_renz_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Camera B")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Camera B");
                modelFile = "MGS3 Camera B.obj";
                mtlFile = "MGS3 Camera B.mtl";
                textureFiles = new string[]
                {
        "cme_a_item_fill_ovl_alp.bmp.png",
        "cme_b_item_body.bmp.png",
        "cme_b_item_renz_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Cardboard Box A")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cardboard Box A");
                modelFile = "MGS3 CBox A.obj";
                mtlFile = "MGS3 CBox A.mtl";
                textureFiles = new string[]
                {
        "cbox_a.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Cardboard Box B")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cardboard Box B");
                modelFile = "MGS3 CBox B.obj";
                mtlFile = "MGS3 CBox B.mtl";
                textureFiles = new string[]
                {
        "cbox_b.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Cardboard Box C")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cardboard Box C");
                modelFile = "MGS3 CBox C.obj";
                mtlFile = "MGS3 CBox C.mtl";
                textureFiles = new string[]
                {
        "cbox_c.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Cigar")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Cigar");
                modelFile = "MGS3 Cigar.obj";
                mtlFile = "MGS3 Cigar.mtl";
                textureFiles = new string[]
                {
        "sga_cigar01.bmp.png",
        "sga_cigar02_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Comic")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Comic");
                modelFile = "MGS3 Comic.obj";
                mtlFile = "MGS3 Comic.mtl";
                textureFiles = new string[]
                {
        "comic_1.bmp.png",
        "comic_2.bmp.png",
        "comic_3.bmp.png",
        "comic_4.bmp.png",
        "it_magazine_side.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Croc Cap")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Croc Cap");
                modelFile = "MGS3 Croc Cap.obj";
                mtlFile = "MGS3 Croc Cap.mtl";
                textureFiles = new string[]
                {
        "gav_eye.bmp.png",
        "gavial_cap.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Directional Mic")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Directional Mic");
                modelFile = "MGS3 D Mic.obj";
                mtlFile = "MGS3 D Mic.mtl";
                textureFiles = new string[]
                {
        "mic_a.bmp.png",
        "mic_pura_alp.trans.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Flask")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Flask");
                modelFile = "MGS3 Flask.obj";
                mtlFile = "MGS3 Flask.mtl";
                textureFiles = new string[]
                {
        "item_gr_bottle_00.bmp.png",
        "item_gr_bottle_01.bmp.png",
        "item_gr_bottle_02.bmp.png",
        "item_gr_bottle_03.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Item Belt")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Item Belt");
                modelFile = "MGS3 Item Belt.obj";
                mtlFile = "MGS3 Item Belt.mtl";
                textureFiles = new string[]
                {
        "sna_belt_bp.bmp.png",
        "sna_bk.bmp.png",
        "sna_mgs3_belt.bmp_77380cc45d92a52a1e8da9f59a6ea891.png",
        "sna_mgs3_gh.bmp.png",
        "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
        "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
                };
            }
            else if (selectedModel == "MGS3 Johnny Picture")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Johnny Picture");
                modelFile = "MGS3 Johnny Picture.obj";
                mtlFile = "MGS3 Johnny Picture.mtl";
                textureFiles = new string[]
                {
        "family_picture_ura_musen.bmp.png",
        "jonny_familiy_01.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Kerotan")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Kerotan");
                modelFile = "MGS3 Kerotan.obj";
                mtlFile = "MGS3 Kerotan.mtl";
                textureFiles = new string[]
                {
        "kerotan_hi.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Microfilm")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Microfilm");
                modelFile = "MGS3 Microfilm.obj";
                mtlFile = "MGS3 Microfilm.mtl";
                textureFiles = new string[]
                {
        "microfilm_film_side.bmp.png",
        "microfilm_film_top.bmp.png",
        "microfilm_tex.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Mousetrap")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Mousetrap");
                modelFile = "MGS3 Mousetrap.obj";
                mtlFile = "MGS3 Mousetrap.mtl";
                textureFiles = new string[]
                {
        "rattrap_tex.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Night Vision Goggles")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Night Vision Goggles");
                modelFile = "MGS3 NVG.obj";
                mtlFile = "MGS3 NVG.mtl";
                textureFiles = new string[]
                {
        "kyano_test_alp_ovl.bmp_d42d43ff6989ac02dd4f9a6b473f7bb6.png",
        "night_goggle.bmp.png",
        "night_goggle_lenz.bmp.png",
        "night_goggle_lenzback.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Radio")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Radio");
                modelFile = "MGS3 Radio.obj";
                mtlFile = "MGS3 Radio.mtl";
                textureFiles = new string[]
                {
        "radio_end.bmp.png",
        "radio_glass_ovl_alp.bmp.png",
        "radio_mark_ovl_alp.bmp.png",
        "radio_moji.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Philosopher's Legacy")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Philosopher's Legacy");
                modelFile = "MGS3 Philosopher's Legacy.obj";
                mtlFile = "MGS3 Philosopher's Legacy.mtl";
                textureFiles = new string[]
                {
        "micro_film_cor01_b.bmp.png",
        "micro_film_cor01_c.bmp.png",
        "micro_film_cor02.bmp.png",
        "micro_film_cor03.bmp.png",
        "micro_film_cor04.bmp.png",
        "micro_film_film01.bmp.png",
        "micro_film_film02.bmp.png",
        "micro_film_film03.bmp.png",
        "micro_film_film04.bmp.png",
        "micro_film_panel_alp.trans.bmp.png",
        "micro_film_side.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Raikov Picture")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Raikov Picture");
                modelFile = "MGS3 Raikov Picture.obj";
                mtlFile = "MGS3 Raikov Picture.mtl";
                textureFiles = new string[]
                {
        "homo_picture_01.bmp.png",
        "picture_ura_01.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Roast Fish")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Roast Fish");
                modelFile = "MGS3 Roast Fish.obj";
                mtlFile = "MGS3 Roast Fish.mtl";
                textureFiles = new string[]
                {
        "sna_item_roastfish_dmo_body.bmp.png",
        "sna_item_roastfish_dmo_niku.bmp.png",
        "sna_item_roastfish_dmo_niku_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Roast Snake")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Roast Snake");
                modelFile = "MGS3 Roast Snake.obj";
                mtlFile = "MGS3 Roast Snake.mtl";
                textureFiles = new string[]
                {
        "sna_item_roastviper_dmo_body.bmp.png",
        "sna_item_roastviper_dmo_mouse.bmp.png",
        "sna_item_roastviper_dmo_niku_ovl_alp.bmp.png",
        "sna_item_roastviper_dmo_wood.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Scope")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Scope");
                modelFile = "MGS3 Scope.obj";
                mtlFile = "MGS3 Scope.mtl";
                textureFiles = new string[]
                {
        "lens_a.bmp.png",
        "m3_a.bmp.png",
        "m3_cover.bmp.png",
        "tr_renz_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Sokolov Picture")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Sokolov Picture");
                modelFile = "MGS3 Sokolov Picture.obj";
                mtlFile = "MGS3 Sokolov Picture.mtl";
                textureFiles = new string[]
                {
        "picture_ura_01.bmp.png",
        "sokorof_family_01.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Spy Radio Broken")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Spy Radio Broken");
                modelFile = "MGS3 Spy Radio Broken.obj";
                mtlFile = "MGS3 Spy Radio Broken.mtl";
                textureFiles = new string[]
                {
        "radi_break_bottan.bmp.png",
        "radi_break_ita.bmp.png",
        "radi_break_tetsu.bmp.png",
        "spy_case_break.bmp.png",
        "spy_knob_brk.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Spy Radio Closed")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Spy Radio Closed");
                modelFile = "MGS3 Spy Radio Closed.obj";
                mtlFile = "MGS3 Spy Radio Closed.mtl";
                textureFiles = new string[]
                {
        "radio_2.bmp.png",
        "radio_4.bmp.png",
        "radio_5.bmp.png",
        "spy_case_a.bmp.png",
        "spy_knob_brk.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Spy Radio Open")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Spy Radio Open");
                modelFile = "MGS3 Spy Radio Open.obj";
                mtlFile = "MGS3 Spy Radio Open.mtl";
                textureFiles = new string[]
                {
        "radio_2.bmp.png",
        "radio_4.bmp.png",
        "radio_5.bmp.png",
        "spy_case_a.bmp.png",
        "spy_knob_brk.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Suitcase EVA")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Suitcase EVA");
                modelFile = "MGS3 Suitcase Eva.obj";
                mtlFile = "MGS3 Suitcase Eva.mtl";
                textureFiles = new string[]
                {
        "radio_2.bmp.png",
        "radio_4.bmp.png",
        "spy_case_a.bmp.png",
        "spy_knob_brk.bmp.png",
        "suit_eva_under.bmp.png",
        "suit_eva_up.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Suitcase Snake")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Suitcase Snake");
                modelFile = "MGS3 Suitcase Snake.obj";
                mtlFile = "MGS3 Suitcase Snake.mtl";
                textureFiles = new string[]
                {
        "suitcase_tex01.bmp.png",
        "suitcase_tex02.bmp.png",
        "suitcase_tex03.bmp.png",
        "suitscase_knob.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Tape")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Tape");
                modelFile = "MGS3 Tape.obj";
                mtlFile = "MGS3 Tape.mtl";
                textureFiles = new string[]
                {
        "reco_sitch.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Tape Recorder")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Tape Recorder");
                modelFile = "MGS3 Tape Recorder.obj";
                mtlFile = "MGS3 Tape Recorder.mtl";
                textureFiles = new string[]
                {
        "button.bmp.png",
        "reco_body_a.bmp.png",
        "reco_body_a_ovl_alp.bmp.png",
        "reco_sitch.bmp.png",
        "rogo_alp_ovl.bmp.png",
        "tape_01_anm.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Thermal Goggles")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Thermal Goggles");
                modelFile = "MGS3 Thermal Goggles.obj";
                mtlFile = "MGS3 Thermal Goggles.mtl";
                textureFiles = new string[]
                {
        "kyano_test_alp_ovl.bmp_d42d43ff6989ac02dd4f9a6b473f7bb6.png",
        "night_goggle.bmp.png",
        "night_goggle_lenz.bmp.png",
        "night_goggle_lenzback.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Torch")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Torch");
                modelFile = "MGS3 Torch.obj";
                mtlFile = "MGS3 Torch.mtl";
                textureFiles = new string[]
                {
        "torch_cloth.bmp.png",
        "torch_kai.bmp.png",
        "torch_wire.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Transmitter")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Transmitter");
                modelFile = "MGS3 Transmitter.obj";
                mtlFile = "MGS3 Transmitter.mtl";
                textureFiles = new string[]
                {
        "beacon_body.bmp.png",
        "cap.bmp.png",
        "capgrass_ovl_alp.bmp.png",
                };
            }
            else if (selectedModel == "MGS3 Wine Glass")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Wine Glass");
                modelFile = "MGS3 Wine Glass.obj";
                mtlFile = "MGS3 Wine Glass.mtl";
                textureFiles = new string[]
                {
        "demo_glass_alp_ovl.bmp_cf36144c527daa7c237269f9eac6c95c.png",
        "wine_liq_5half.bmp.png",
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
            else if (selectedModel == "MGS2 Tanker Backup")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Tanker Backup");
                modelFile = "gba_def_mt.obj";
                mtlFile = "gba_def_mt.mtl";
                textureFiles = new string[]
                {
        "gba_arm.bmp.png",
        "gba_boot02.bmp.png",
        "gba_chest06.bmp.png",
        "gba_chest07.bmp.png",
        "gba_chest_belt.bmp.png",
        "gba_chest_poket.bmp.png",
        "gba_chest_poket2.bmp.png",
        "gba_chest_waki.bmp.png",
        "gba_head_chin.bmp.png",
        "gba_head_inside.bmp.png",
        "gba_head_side.bmp.png",
        "gba_head_side02.bmp.png",
        "gba_head_top.bmp.png",
        "gba_radio_bottom.bmp.png",
        "gba_radio_front.bmp.png",
        "gba_radio_side.bmp.png",
        "gba_radio_top.bmp.png",
        "gba_toe.bmp.png",
        "gbs_body_ss.bmp.png",
        "gbs_boot_bot1.bmp.png",
        "gbs_boot_bot2.bmp.png",
        "gbs_eye_def.bmp.png",
        "gbs_face.bmp.png",
        "gbs_leg_ss.bmp.png",
        "gbs_wrist.bmp.png",
        "grs_black.bmp.png",
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
                    "gps_wrist.bmp.png",
                    "gps_body_ss.bmp.png",
                    "gps_leg_ss.bmp.png",
                    "gps_poket.bmp.png",
                    "gps_poket2.bmp.png",
                    "gps_boot_ss.bmp.png",
                    "gps_toe2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Big Shell Backup")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Big Shell Backup");
                modelFile = "htc_def_mt.obj";
                mtlFile = "htc_def_mt.mtl";
                textureFiles = new string[]
                {
        "htc_arm_uc.bmp.png",
        "htc_belt_n2.bmp.png",
        "htc_belt_uc.bmp.png",
        "htc_body_b_uc.bmp.png",
        "htc_body_uc.bmp.png",
        "htc_face_uc.bmp.png",
        "htc_gun_fro.bmp.png",
        "htc_gun_side.bmp.png",
        "htc_gun_top.bmp.png",
        "htc_hand_i_uc.bmp.png",
        "htc_hand_o_uc.bmp.png",
        "htc_helmet_b_uc.bmp.png",
        "htc_helmet_b_uc_ovl_alp_sub.bmp.png",
        "htc_helmet_i_uc.bmp.png",
        "htc_helmet_uc.bmp.png",
        "htc_helmet_uc_ovl_alp_sub.bmp.png",
        "htc_leg_uc.bmp.png",
        "htc_light_uc.bmp.png",
        "htc_light_uc_ovl_sub_alp.bmp.png",
        "htc_neck_i_uc.bmp.png",
        "htc_neck_o_uc.bmp.png",
        "htc_porch_uc.bmp.png",
        "htc_shoes_uc.bmp.png",
        "htc_shoulder_uc.bmp.png",
        "htc_shoulder_uc_ovl_sub_alp.bmp.png",
        "htc_under_pad_uc.bmp.png",
        "sna_m9_glip.bmp.png",
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

            else if (selectedModel == "MGS2 Genome")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Genome");
                modelFile = "gno_def.obj";
                mtlFile = "gno_def.mtl";
                textureFiles = new string[]
                {
                "gen_arm.bmp.png",
                "gen_bag_bl.bmp.png",
                "gen_bag_br.bmp.png",
                "gen_bag_fl.bmp.png",
                "gen_bag_fr.bmp.png",
                "gen_body.bmp.png",
                "gen_boot_heel.bmp.png",
                "gen_boot_hunder.bmp.png",
                "gen_boot_toe.bmp.png",
                "gen_boot_tunder.bmp.png",
                "gen_eri_omote_fix.bmp.png",
                "gen_eri_ura.bmp.png",
                "gen_eye_fix.bmp.png",
                "gen_hand.bmp.png",
                "gen_hand_u.bmp.png",
                "gen_head.bmp.png",
                "gen_leg_fix.bmp.png",
                "gen_skirt_fix.bmp.png",
                "gen_wrist.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Genome Mecha")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Genome Mecha");
                modelFile = "gno_meca_mt.obj";
                mtlFile = "gno_meca_mt.mtl";
                textureFiles = new string[]
                {
        "gen_bag_bl.bmp.png",
        "gen_bag_br.bmp.png",
        "gen_bag_fl.bmp.png",
        "gen_bag_fr.bmp.png",
        "gen_boot_heel.bmp.png",
        "gen_boot_hunder.bmp.png",
        "gen_boot_toe.bmp.png",
        "gen_boot_tunder.bmp.png",
        "gen_eri_omote_fix.bmp.png",
        "gen_eri_ura.bmp.png",
        "gen_eye_red.bmp.png",
        "gen_hand.bmp.png",
        "gen_hand_u.bmp.png",
        "gen_meca_lite.bmp.png",
        "gen_mecaarm_fix.bmp.png",
        "gen_mecaarm_sub_ovl_alp.bmp.png",
        "gen_mecabody.bmp.png",
        "gen_mecabody_sub_ovl_alp.bmp.png",
        "gen_mecahead_fix.bmp.png",
        "gen_mecahead_sub_ovl_alp.bmp.png",
        "gen_mecaleg_fix.bmp.png",
        "gen_mecaleg_sub_ovl_alp.bmp.png",
        "gen_mecaskirt_fix.bmp.png",
        "gen_mecaskirt_sub_ovl_alp.bmp.png",
        "gen_wrist.bmp.png",
        "gno_neckin.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Fortune")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Fortune");
                modelFile = "for_def_sh_mt.obj";
                mtlFile = "for_def_sh_mt.mtl";
                textureFiles = new string[]
                {
        "for_arm.bmp.png",
        "for_belt_shoulder.bmp.png",
        "for_belt_weist.bmp.png",
        "for_body_ovl_sub_alp.bmp.png",
        "for_body_ss.bmp.png",
        "for_boot_bot1.bmp.png",
        "for_boot_bot2.bmp.png",
        "for_coat_sling.bmp.png",
        "for_eye_ovl_sub_alp.bmp.png",
        "for_foot.bmp.png",
        "for_hair_back.bmp.png",
        "for_hand.bmp.png",
        "for_hand2.bmp.png",
        "for_hi_eye.bmp.png",
        "for_hi_face_ss.bmp.png",
        "for_hlst_back.bmp.png",
        "for_hlst_fro.bmp.png",
        "for_hlst_side.bmp.png",
        "for_leg_ovl_sub_alp.bmp.png",
        "for_leg_ss.bmp.png",
        "for_m9_glip.bmp.png",
        "for_sox_ovl_sub_alp.bmp.png",
        "for_toe.bmp.png",
        "ros_hi_tang01dt.bmp.png",
        "ros_hi_teeth_d.bmp.png",
        "ros_hi_teeth_u.bmp.png",
        "sna_shadow.bmp.png",
        "vmp_naked_udewa.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Emma")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Emma");
                modelFile = "ema_def_sh_mt.obj";
                mtlFile = "ema_def_sh_mt.mtl";
                textureFiles = new string[]
                {
        "ema_arm00.bmp.png",
        "ema_arm_sub_alp_ovl.bmp.png",
        "ema_arm_t.bmp.png",
        "ema_chest.bmp.png",
        "ema_chest_sub_alp_ovl.bmp.png",
        "ema_eri_t.bmp.png",
        "ema_eye_dt.bmp.png",
        "ema_eye_dt_ovl_sub_alp.bmp.png",
        "ema_face_dt.bmp.png",
        "ema_face_dt_alp_sub_ovl.bmp.png",
        "ema_hair_side_dt.bmp.png",
        "ema_hair_top_dt.bmp.png",
        "ema_hand02.bmp.png",
        "ema_hand_sub_alp_ovl.bmp.png",
        "ema_hashi.bmp.png",
        "ema_leg.bmp.png",
        "ema_leg_sub_alp_ovl.bmp.png",
        "ema_legl_sub_alp_ovl_new.bmp.png",
        "ema_legr000.bmp.png",
        "ema_shue_under.bmp.png",
        "ema_shueback.bmp.png",
        "ema_shueside.bmp.png",
        "ema_shuesunder.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Vamp Naked")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Vamp Naked");
                modelFile = "vmp_naked_sh.obj";
                mtlFile = "vmp_naked_sh.mtl";
                textureFiles = new string[]
                {
        "for_belt_shoulder.bmp.png",
        "vmp_belt_weist.bmp.png",
        "vmp_boot_bot1.bmp.png",
        "vmp_boot_bot2.bmp.png",
        "vmp_bp.bmp.png",
        "vmp_foot.bmp.png",
        "vmp_hair_back.bmp.png",
        "vmp_hand1.bmp.png",
        "vmp_hand2.bmp.png",
        "vmp_hi_eye.bmp.png",
        "vmp_hi_face_dead.bmp.png",
        "vmp_holder.bmp.png",
        "vmp_leg_ss.bmp.png",
        "vmp_naked_arm_ss.bmp.png",
        "vmp_naked_body.bmp.png",
        "vmp_naked_rarm_ss.bmp.png",
        "vmp_naked_udewa.bmp.png",
        "vmp_sknife.bmp.png",
        "vmp_toe.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Ocelot Tanker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ocelot Tanker");
                modelFile = "rev_stand_test.obj";
                mtlFile = "rev_stand_test.mtl";
                textureFiles = new string[]
                {
        "gbs_eri.bmp.png",
        "rev_blk.bmp.png",
        "rev_body_ss.bmp.png",
        "rev_boot_ss.bmp.png",
        "rev_coat_arm01dt.bmp.png",
        "rev_coat_body01dt.bmp.png",
        "rev_coat_body02dt.bmp.png",
        "rev_coat_neck01dt.bmp.png",
        "rev_coat_shoulder01dt.bmp.png",
        "rev_face_01dt.bmp.png",
        "rev_face_ss.bmp.png",
        "rev_gunbelt_u1.bmp.png",
        "rev_hair1.bmp.png",
        "rev_hair2.bmp.png",
        "rev_hand1.bmp.png",
        "rev_hand2.bmp.png",
        "rev_kanagu.bmp.png",
        "rev_leg_ss.bmp.png",
        "rev_saapack.bmp.png",
        "rev_spur1_ovl_alp.bmp.png",
        "rev_tama.bmp.png",
        "rev_tama_top.bmp.png",
        "rev_toe.bmp.png",
        "saa_all.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Ray Prototype")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ray Prototype");
                modelFile = "ray_def_mt.obj";
                mtlFile = "ray_def_mt.mtl";
                textureFiles = new string[]
                {
        "pdray_armpit_meca1.bmp.png",
        "pdray_b_pack_t1_sub_alp_ovl.bmp.png",
        "pdray_chin1_u_sub_alp_ovl.bmp.png",
        "pdray_chin2_u_sub_alp_ovl.bmp.png",
        "pdray_face_b1_sub_alp_ovl.bmp.png",
        "pdray_foot_s.bmp.png",
        "pdray_leg_s1_sub_ovl_alp.bmp.png",
        "ray_arm_t1.bmp.png",
        "ray_arm_t1_sub_alp_ovl.bmp.png",
        "ray_arm_u1.bmp.png",
        "ray_arm_u1s.bmp.png",
        "ray_arm_u_finall.bmp.png",
        "ray_armpit_meca2.bmp.png",
        "ray_armpit_t1.bmp.png",
        "ray_armpit_u1.bmp.png",
        "ray_backpack_t1.bmp.png",
        "ray_body_s1.bmp.png",
        "ray_cheek_f1.bmp.png",
        "ray_cheek_inside1.bmp.png",
        "ray_chest_f1.bmp.png",
        "ray_chin1_u.bmp.png",
        "ray_chin2_u.bmp.png",
        "ray_chin_t1.bmp.png",
        "ray_col1.bmp.png",
        "ray_eye_pera.bmp.png",
        "ray_face_b1.bmp.png",
        "ray_face_s1.bmp.png",
        "ray_face_s1_sub_alp_ovl.bmp.png",
        "ray_face_s2.bmp.png",
        "ray_face_s2_sub_alp_ovl.bmp.png",
        "ray_face_t1.bmp.png",
        "ray_face_t1_sub_alp_ovl.bmp.png",
        "ray_forearm_t1.bmp.png",
        "ray_hand_u1.bmp.png",
        "ray_hip_f1.bmp.png",
        "ray_leg1_s1.bmp.png",
        "ray_leg1_s1_sub_alp_ovl.bmp.png",
        "ray_leg2_s1.bmp.png",
        "ray_lip_out.bmp.png",
        "ray_lip_out_sub_alp_ovl.bmp.png",
        "ray_mouth_temptex.bmp.png",
        "ray_muzzle.bmp.png",
        "ray_muzzle_iner.bmp.png",
        "ray_neck_circle.bmp.png",
        "ray_shoulder_circle.bmp.png",
        "ray_shoulder_f1.bmp.png",
        "ray_tail_s1x.bmp.png",
        "ray_tail_s1x_sub_alp_ovl.bmp.png",
        "ray_tail_s2.bmp.png",
        "ray_temptex1.bmp.png",
        "ray_thigh_b1.bmp.png",
        "ray_thigh_b1_sub_alp_ovl.bmp.png",
        "ray_thigh_circle.bmp.png",
        "ray_thigh_s1.bmp.png",
        "ray_thigh_t1.bmp.png",
        "ray_thigh_t1_sub_ovl_alp.bmp.png",
        "ray_thigh_u1.bmp.png",
        "ray_tooth.bmp.png",
        "ray_waist_b1x.bmp.png",
        "ray_waist_b1x_sub_alp_ovl.bmp.png",
        "ray_waist_s1.bmp.png",
        "ray_waist_s1_sub_alp_ovl.bmp.png",
        "w00b_dammy_sub_ovl.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Ray Cockpit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ray Cockpit");
                modelFile = "ray_cockpit.obj";
                mtlFile = "ray_cockpit.mtl";
                textureFiles = new string[]
                {
        "raycp_arm_s1.bmp.png",
        "raycp_arm_t1.bmp.png",
        "raycp_backbox_f1.bmp.png",
        "raycp_backwall.bmp.png",
        "raycp_bmirror_b1.bmp.png",
        "raycp_bmirror_f1.bmp.png",
        "raycp_ceil1.bmp.png",
        "raycp_ceil_lt_alp_decal_ovl.bmp.png",
        "raycp_chestmount_t1.bmp.png",
        "raycp_chestmount_t2.bmp.png",
        "raycp_cheststay_s1.bmp.png",
        "raycp_cheststay_t1.bmp.png",
        "raycp_conpane1.bmp.png",
        "raycp_earcyl_f1.bmp.png",
        "raycp_eyebox_s1.bmp.png",
        "raycp_eyebox_s2.bmp.png",
        "raycp_floor1.bmp.png",
        "raycp_floor2.bmp.png",
        "raycp_footp_stay_s1.bmp.png",
        "raycp_grey.bmp.png",
        "raycp_grip.bmp.png",
        "raycp_hatch_u1.bmp.png",
        "raycp_hatchstay_s1.bmp.png",
        "raycp_hd_cable1.bmp.png",
        "raycp_hd_glass.bmp.png",
        "raycp_hd_t1.bmp.png",
        "raycp_hd_u1.bmp.png",
        "raycp_largecable1.bmp.png",
        "raycp_seat_f1.bmp.png",
        "raycp_seat_s1.bmp.png",
        "raycp_seatbackbox_t1.bmp.png",
        "raycp_shaft_s1.bmp.png",
        "raycp_shldmount_s1.bmp.png",
        "raycp_shldmount_t1.bmp.png",
        "raycp_shouldbox.bmp.png",
        "raycp_sidebox_t1.bmp.png",
        "raycp_smallcable1.bmp.png",
        "raycp_touchp_stay_t1.bmp.png",
        "raycp_touchpanel_f1.bmp.png",
        "raycp_touchpanel_u1.bmp.png",
        "raycp_wall_danmen.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Ray Cockpit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ray Cockpit");
                modelFile = "ray_cockpit.obj";
                mtlFile = "ray_cockpit.mtl";
                textureFiles = new string[]
                {
        "raycp_arm_s1.bmp.png",
        "raycp_arm_t1.bmp.png",
        "raycp_backbox_f1.bmp.png",
        "raycp_backwall.bmp.png",
        "raycp_bmirror_b1.bmp.png",
        "raycp_bmirror_f1.bmp.png",
        "raycp_ceil1.bmp.png",
        "raycp_ceil_lt_alp_decal_ovl.bmp.png",
        "raycp_chestmount_t1.bmp.png",
        "raycp_chestmount_t2.bmp.png",
        "raycp_cheststay_s1.bmp.png",
        "raycp_cheststay_t1.bmp.png",
        "raycp_conpane1.bmp.png",
        "raycp_earcyl_f1.bmp.png",
        "raycp_eyebox_s1.bmp.png",
        "raycp_eyebox_s2.bmp.png",
        "raycp_floor1.bmp.png",
        "raycp_floor2.bmp.png",
        "raycp_footp_stay_s1.bmp.png",
        "raycp_grey.bmp.png",
        "raycp_grip.bmp.png",
        "raycp_hatch_u1.bmp.png",
        "raycp_hatchstay_s1.bmp.png",
        "raycp_hd_cable1.bmp.png",
        "raycp_hd_glass.bmp.png",
        "raycp_hd_t1.bmp.png",
        "raycp_hd_u1.bmp.png",
        "raycp_largecable1.bmp.png",
        "raycp_seat_f1.bmp.png",
        "raycp_seat_s1.bmp.png",
        "raycp_seatbackbox_t1.bmp.png",
        "raycp_shaft_s1.bmp.png",
        "raycp_shldmount_s1.bmp.png",
        "raycp_shldmount_t1.bmp.png",
        "raycp_shouldbox.bmp.png",
        "raycp_sidebox_t1.bmp.png",
        "raycp_smallcable1.bmp.png",
        "raycp_touchp_stay_t1.bmp.png",
        "raycp_touchpanel_f1.bmp.png",
        "raycp_touchpanel_u1.bmp.png",
        "raycp_wall_danmen.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Naked Raiden")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Naked Raiden");
                modelFile = "rai_naked_sh.obj";
                mtlFile = "rai_naked_sh.mtl";
                textureFiles = new string[]
                {
        "rai_hi_eye2.bmp.png",
        "rai_hi_face_ss2.bmp.png",
        "rai_naked_arm_ovl_sub_alp.bmp.png",
        "rai_naked_arm_ss.bmp.png",
        "rai_naked_body.bmp.png",
        "rai_naked_body_ovl_sub_alp.bmp.png",
        "rai_naked_foot1.bmp.png",
        "rai_naked_foot2.bmp.png",
        "rai_naked_hand1.bmp.png",
        "rai_naked_hand2.bmp.png",
        "rai_naked_leg.bmp.png",
        "rai_naked_leg_ovl_sub_alp.bmp.png",
        "rai_naked_toe.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Gun Cypher")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Gun Cypher");
                modelFile = "gcyp.obj";
                mtlFile = "gcyp.mtl";
                textureFiles = new string[]
                {
        "gcyp_amo_f1.bmp.png",
        "gcyp_amo_s1.bmp.png",
        "gcyp_blade_alp.bmp.png",
        "gcyp_body1.bmp.png",
        "gcyp_body_u1.bmp.png",
        "gcyp_eff_blade_alp.bmp.png",
        "gcyp_fook_s1.bmp.png",
        "gcyp_gun_s1.bmp.png",
        "gcyp_gun_s2.bmp.png",
        "gcyp_leg_s1.bmp.png",
        "gcyp_randing_s1.bmp.png",
        "gcyp_rifle_cnt.bmp.png",
        "gcyp_rifle_stay.bmp.png",
        "gcyp_roter_cnt_alp.bmp.png",
        "gcyp_sight_f1.bmp.png",
        "gcyp_sight_s1.bmp.png",
                };
            }

            else if (selectedModel == "MGS2 Ray Mass Produced")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Ray Mass Produced");
                modelFile = "pdray_def_mt.obj";
                mtlFile = "pdray_def_mt.mtl";
                textureFiles = new string[]
                {
        "pdray_arm_t1_small.bmp.png",
        "pdray_arm_t1_sub_alp_ovl.bmp.png",
        "pdray_arm_u1.bmp.png",
        "pdray_arm_u1s.bmp.png",
        "pdray_arm_u_finall.bmp.png",
        "pdray_armpit_meca1.bmp.png",
        "pdray_armpit_meca2.bmp.png",
        "pdray_armpit_t1.bmp.png",
        "pdray_armpit_u1.bmp.png",
        "pdray_b_pack_t1_sub_alp_ovl.bmp.png",
        "pdray_backpack_t1.bmp.png",
        "pdray_body_s1.bmp.png",
        "pdray_canopy.bmp.png",
        "pdray_canopy_sub_ovl_alp.bmp.png",
        "pdray_cheek_f1.bmp.png",
        "pdray_cheek_inside1.bmp.png",
        "pdray_chest_f1.bmp.png",
        "pdray_chin1_u.bmp.png",
        "pdray_chin1_u_sub_alp_ovl.bmp.png",
        "pdray_chin2_u.bmp.png",
        "pdray_chin2_u_sub_alp_ovl.bmp.png",
        "pdray_chin_t1.bmp.png",
        "pdray_eye_pera.bmp.png",
        "pdray_face_b1.bmp.png",
        "pdray_face_b1_sub_alp_ovl.bmp.png",
        "pdray_face_s1.bmp.png",
        "pdray_face_s1_sub_alp_ovl.bmp.png",
        "pdray_face_s2.bmp.png",
        "pdray_face_s2_sub_alp_ovl.bmp.png",
        "pdray_face_t1.bmp.png",
        "pdray_face_t1_sub_alp_ovl.bmp.png",
        "pdray_foot_s.bmp.png",
        "pdray_forearm_t1_small.bmp.png",
        "pdray_hand_u1.bmp.png",
        "pdray_hip_f1.bmp.png",
        "pdray_leg1_s1.bmp.png",
        "pdray_leg1_s1_sub_alp_ovl.bmp.png",
        "pdray_leg2_s1.bmp.png",
        "pdray_leg_s1_sub_ovl_alp.bmp.png",
        "pdray_lip_out.bmp.png",
        "pdray_lip_out_sub_alp_ovl.bmp.png",
        "pdray_mouth_temptex.bmp.png",
        "pdray_muzzle.bmp.png",
        "pdray_muzzle_iner.bmp.png",
        "pdray_neck_circle.bmp.png",
        "pdray_shoulder_circle.bmp.png",
        "pdray_shoulder_f1.bmp.png",
        "pdray_tail_s1x.bmp.png",
        "pdray_tail_s1x_sub_alp_ovl.bmp.png",
        "pdray_temptex1.bmp.png",
        "pdray_thigh_b1.bmp.png",
        "pdray_thigh_b1_sub_alp_ovl.bmp.png",
        "pdray_thigh_cir_sub_alp_ovl.bmp.png",
        "pdray_thigh_circle.bmp.png",
        "pdray_thigh_s1.bmp.png",
        "pdray_thigh_t1.bmp.png",
        "pdray_thigh_t1_sub_ovl_alp.bmp.png",
        "pdray_thigh_u1.bmp.png",
        "pdray_tooth.bmp.png",
        "pdray_waist_b1x.bmp.png",
        "pdray_waist_b1x_sub_alp_ovl.bmp.png",
        "pdray_waist_s1.bmp.png",
        "pdray_waist_s1_sub_alp_ovl.bmp.png",
        "pdray_waist_s2.bmp.png",
        "pdray_waist_s2_sub_alp_ovl.bmp.png",
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
            else if (selectedModel == "MGS2 Fatman")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Fatman");
                modelFile = "fat_def_sh_mt.obj";
                mtlFile = "fat_def_sh_mt.mtl";
                textureFiles = new string[]
                {
                    "cam_mouin.bmp.png",
                    "fat_allarm_st2.bmp.png",
                    "fat_allleg.bmp.png",
                    "fat_allleg_sub_ovl_alp.bmp.png",
                    "fat_arm_belt.bmp.png",
                    "fat_arm_sub_ovl_alp.bmp.png",
                    "fat_belt.bmp.png",
                    "fat_body3_sub_ovl_alp.bmp.png",
                    "fat_calf.bmp.png",
                    "fat_calf_sub_alp_ovl.bmp.png",
                    "fat_ep0_sub_ovl_alp.bmp.png",
                    "fat_ep_re.bmp.png",
                    "fat_eri_back.bmp.png",
                    "fat_eri_back_sub_alp_ovl.bmp.png",
                    "fat_eri_front.bmp.png",
        "fat_eri_front_sub_alp_ovl.bmp.png",
        "fat_eri_side.bmp.png",
        "fat_eri_side_in.bmp.png",
        "fat_eri_side_sub_alp_ovl.bmp.png",
        "fat_eye_re.bmp.png",
        "fat_eyes_ovl_sub_alp.bmp.png",
        "fat_face_re.bmp.png",
        "fat_glg_hlst.bmp.png",
        "fat_glg_hlst_side.bmp.png",
        "fat_hand2.bmp.png",
        "fat_hand_sub_alp_ovl.bmp.png",
        "fat_in.bmp.png",
        "fat_in_eri.bmp.png",
        "fat_in_eri_tome.bmp.png",
        "fat_larm_sub_alp_ovl.bmp.png",
        "fat_poket00_re.bmp.png",
        "fat_poket01.bmp.png",
        "fat_radio.bmp.png",
        "fat_rolar.bmp.png",
        "fat_rolar_flame.bmp.png",
        "fat_rolar_hoir.bmp.png",
        "fat_shues00_sub_ovl_alp.bmp.png",
        "fat_suit_re.bmp.png",
        "fat_suits_kat.bmp.png",
        "fat_suits_kat_sub_alp_ovl.bmp.png",
        "fat_waist.bmp.png",
        "ptr_tooth_d1.bmp.png",
        "ptr_tooth_d2.bmp.png",
        "ptr_tooth_d3.bmp.png",
        "ptr_tooth_d4.bmp.png",
        "ptr_tooth_u1.bmp.png",
        "ptr_tooth_u2.bmp.png",
        "ptr_tooth_u3.bmp.png",
        "ptr_tooth_u4.bmp.png",
        "sna_hi_tang01dt.bmp.png",
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
            else if (selectedModel == "MGS2 President")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 President");
                modelFile = "jam_def_sh_mt.obj";
                mtlFile = "jam_def_sh_mt.mtl";
                textureFiles = new string[]
                {
        "jam_arm_dt01.bmp.png",
        "jam_arm_dt01_ovl_sub_alp.bmp.png",
        "jam_belt_dt01.bmp.png",
        "jam_finger_dt01.bmp.png",
        "jam_finger_dt01_ovl_sub_alp.bmp.png",
        "jam_hair_dt01.bmp.png",
        "jam_hair_dt01_ovl_sub_alp.bmp.png",
        "jam_hand_dt01.bmp.png",
        "jam_hand_dt01_ovl_sub_alp.bmp.png",
        "jam_hi_eye_dt01.bmp.png",
        "jam_hi_eye_dt01_ovl_sub_alp.bmp.png",
        "jam_hi_face_d1_ovl_sub_alp.bmp.png",
        "jam_hi_face_dt01.bmp.png",
        "jam_neck_dt01.bmp.png",
        "jam_neck_dt01_ovl_sub_alp.bmp.png",
        "jam_pants_dt01.bmp.png",
        "jam_shirt_dt01.bmp.png",
        "jam_shirt_dt02.bmp.png",
        "jam_shirts_shadow.bmp.png",
        "jam_shoes_dt01.bmp.png",
        "jam_tie.bmp.png",
        "jam_watch_dt01.bmp.png",
        "jam_watch_dt01_ovl_sub_alp.bmp.png",
        "jam_watch_dt02.bmp.png",
        "jam_watch_dt02_ovl_sub_alp.bmp.png",
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

            var changedBoxes = panelTextures.Controls
                .OfType<PictureBox>()
                .Where(pb =>
                {
                    string currentPath = pb.Tag as string;
                    string originalPath = RemoveSuffix(currentPath);
                    return !string.Equals(currentPath, originalPath, StringComparison.OrdinalIgnoreCase);
                })
                .ToList();

            if (changedBoxes.Count == 0)
            {
                MessageBox.Show(
                    "You haven't changed any textures.\nPlease change at least one texture before creating a mod.",
                    "No Textures Changed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

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
                Maximum = changedBoxes.Count,
                Value = 0
            };
            progressForm.Controls.Add(bar);
            progressForm.Show(this);

            foreach (var pb in changedBoxes)
            {
                string pngPath = pb.Tag as string;
                string ddsPath = Path.ChangeExtension(pngPath, ".dds");
                bool noMip = MipMapManager.ShouldSkipMipmaps(pngPath);
                string pythonArgs = $"\"{gimpScript}\" \"{pngPath}\" \"{ddsPath}\"";
                if (noMip) pythonArgs += " --no-mipmaps";

                try
                {
                    var psi = new ProcessStartInfo(pythonExe, pythonArgs)
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