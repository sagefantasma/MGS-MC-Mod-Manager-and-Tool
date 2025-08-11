using Assimp;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        private List<string> getTexturesFromMtl(string mtlPath)
        {
            string mtlContent = File.ReadAllText(mtlPath);
            string[] mtlLines = mtlContent.Replace("\r", "").Split("\n");
            List<string> ret = new();
            foreach (string line in mtlLines)
            {
                if (!line.StartsWith("map_Kd")) continue;
                ret.Add(line.Substring(7));
            }
            return ret;
        }

        private readonly Dictionary<string, string> modelFileNames = new() {
            { "MGS3 Snake SE", "MGS3 Snake.obj" },
            { "MGS3 Snake Sneaking Suit", "Snake.obj" },
            { "MGS3 Tanya (Eva)", "TanyaEva.obj" },
            { "MGS3 GRU", "ene_defout.obj" },
            { "MGS3 KGB", "ene_kgb.obj" },
            { "MGS3 Ocelot Unit", "ene_spe.obj" },
            { "MGS3 Officer", "ene_ind.obj" },
            { "MGS3 Raikov", "raikov.obj" },
            { "MGS3 Assistant", "MGS3Assistant.obj" },
            { "MGS3 Boss Mantle", "MGS3 Boss Mantle.obj" },
            { "MGS3 Boss Sneaking Suit", "MGS3 Boss Sneaking Suit.obj" },
            { "MGS3 Boss VM", "MGS3 Boss - Virtuous Mission.obj" },
            { "MGS3 Cameraman A", "MGS3 Cameraman A.obj" },
            { "MGS3 Cameraman B", "MGS3 Cameraman B.obj" },
            { "MGS3 Chopper Worker", "MGS3 Chopper Worker.obj" },
            { "MGS3 CIA Director", "MGS3 CIA Director.obj" },
            { "MGS3 Enemy Bike", "MGS3 Enemy Biker.obj" },
            { "MGS3 Enemy Johnny", "MGS3 Johnny.obj" },
            { "MGS3 Enemy Platform", "MGS3 Hovercraft Enemy.obj" },
            { "MGS3 Enemy Pyro", "MGS3 Flamethrower Enemy.obj" },
            { "MGS3 EVA Half Naked", "MGS3 Eva Half Naked.obj" },
            { "MGS3 EVA Injured", "MGS3 Eva Injured.obj" },
            { "MGS3 EVA Jumpsuit", "MGS3 Eva Jumpsuit.obj" },
            { "MGS3 EVA Jumpsuit Jacket", "MGS3 Eva Jumpsuit Jacket.obj" },
            { "MGS3 EVA Naked", "MGS3 Eva Naked.obj" },
            { "MGS3 Granin", "MGS3 Granin.obj" },
            { "MGS3 Granin Dead", "MGS3 Granin Dead.obj" },
            { "MGS3 Maintenance Worker", "MGS3 Maintenance Worker.obj" },
            { "MGS3 Major Ocelot", "MGS3 Major Ocelot.obj" },
            { "MGS3 Major Zero", "MGS3 Major Zero.obj" },
            { "MGS3 Major Zero Headphones", "MGS3 Major Zero Headphones.obj" },
            { "MGS3 Major Zero Suit", "MGS3 Major Zero Suit.obj" },
            { "MGS3 MiG Pilot", "MGS3 MiG Pilot.obj" },
            { "MGS3 Paramedic", "MGS3 Paramedic.obj" },
            { "MGS3 Paramedic Headphones", "MGS3 Paramedic Headphones.obj" },
            { "MGS3 Paramedic Suit", "MGS3 Paramedic Suit.obj" },
            { "MGS3 Pilot", "MGS3 Pilot.obj" },
            { "MGS3 President", "MGS3 President.obj" },
            { "MGS3 Raikov Naked", "MGS3 Raikov Naked.obj" },
            { "MGS3 Scientist Dead", "MGS3 Scientist Dead.obj" },
            { "MGS3 Secretary of Defense", "MGS3 Secretary of Defense.obj" },
            { "MGS3 Sigint", "MGS3 Sigint.obj" },
            { "MGS3 Snake Halo Jump", "MGS3 Snake Halo.obj" },
            { "MGS3 Snake Maintenance", "MGS3 Snake Maintenance.obj" },
            { "MGS3 Snake Naked", "MGS3 Snake Naked.obj" },
            { "MGS3 Snake Naked Eyepatch", "MGS3 Snake Naked Eyepatch.obj" },
            { "MGS3 Snake Scientist", "MGS3 Snake Scientist.obj" },
            { "MGS3 Snake SE Eyepatch", "MGS3 Snake SE Eyepatch.obj" },
            { "MGS3 Snake SE Injured", "MGS3 Snake SE Injured.obj" },
            { "MGS3 Snake Suit Endgame", "MGS3 Snake Epiloge Suit.obj" },
            { "MGS3 Snake Torture Room", "MGS3 Snake Torture.obj" },
            { "MGS3 Snake Torture Room Bag", "MGS3 Snake Torture Bag.obj" },
            { "MGS3 Snake Tuxedo", "MGS3 Snake Tuxedo.obj" },
            { "MGS3 Snake VM Injured", "MGS3 Snake VM Injured.obj" },
            { "MGS3 Sokolov Coat", "MGS3 Sokolov Coat.obj" },
            { "MGS3 Sokolov Scientist", "MGS3 Sokolov Scientist.obj" },
            { "MGS3 The End", "MGS3 The End.obj" },
            { "MGS3 The Fear", "MGS3 The Fear.obj" },
            { "MGS3 The Fury Helmet", "MGS3 The Fury Helmet.obj" },
            { "MGS3 The Fury No Helmet", "MGS3 The Fury No Helmet.obj" },
            { "MGS3 The Pain Mask", "MGS3 The Pain Mask.obj" },
            { "MGS3 The Pain No Mask", "MGS3 The Pain.obj" },
            { "MGS3 The Sorrow Bleeding Eyes", "MGS3 The Sorrow Bleeding Eyes.obj" },
            { "MGS3 The Sorrow Main", "MGS3 The Sorrow.obj" },
            { "MGS3 The Sorrow Parka", "MGS3 The Sorrow Parka.obj" },
            { "MGS3 VIP A", "MGS3 VIP A.obj" },
            { "MGS3 VIP B", "MGS3 VIP B.obj" },
            { "MGS3 VIP C", "MGS3 VIP C.obj" },
            { "MGS3 Volgin Coat", "MGS3 Volgin Coat.obj" },
            { "MGS3 Volgin Coat (Ammo)", "MGS3 Volgin Coat Ammo.obj" },
            { "MGS3 Volgin No Coat", "MGS3 Volgin No Coat.obj" },
            { "MGS3 Volgin No Coat (Ammo)", "MGS3 Volgin No Coat Ammo.obj" },
            { "MGS3 Book", "MGS3 Book.obj" },
            { "MGS3 Bucket", "MGS3 Bucket.obj" },
            { "MGS3 Camera A", "MGS3 Camera A.obj" },
            { "MGS3 Camera B", "MGS3 Camera B.obj" },
            { "MGS3 Cardboard Box A", "MGS3 CBox A.obj" },
            { "MGS3 Cardboard Box B", "MGS3 CBox B.obj" },
            { "MGS3 Cardboard Box C", "MGS3 CBox C.obj" },
            { "MGS3 Cigar", "MGS3 Cigar.obj" },
            { "MGS3 Comic", "MGS3 Comic.obj" },
            { "MGS3 Croc Cap", "MGS3 Croc Cap.obj" },
            { "MGS3 Directional Mic", "MGS3 D Mic.obj" },
            { "MGS3 Flask", "MGS3 Flask.obj" },
            { "MGS3 Item Belt", "MGS3 Item Belt.obj" },
            { "MGS3 Johnny Picture", "MGS3 Johnny Picture.obj" },
            { "MGS3 Kerotan", "MGS3 Kerotan.obj" },
            { "MGS3 Microfilm", "MGS3 Microfilm.obj" },
            { "MGS3 Mousetrap", "MGS3 Mousetrap.obj" },
            { "MGS3 Night Vision Goggles", "MGS3 NVG.obj" },
            { "MGS3 Radio", "MGS3 Radio.obj" },
            { "MGS3 Philosopher's Legacy", "MGS3 Philosopher's Legacy.obj" },
            { "MGS3 Raikov Picture", "MGS3 Raikov Picture.obj" },
            { "MGS3 Roast Fish", "MGS3 Roast Fish.obj" },
            { "MGS3 Roast Snake", "MGS3 Roast Snake.obj" },
            { "MGS3 Scope", "MGS3 Scope.obj" },
            { "MGS3 Sokolov Picture", "MGS3 Sokolov Picture.obj" },
            { "MGS3 Spy Radio Broken", "MGS3 Spy Radio Broken.obj" },
            { "MGS3 Spy Radio Closed", "MGS3 Spy Radio Closed.obj" },
            { "MGS3 Spy Radio Open", "MGS3 Spy Radio Open.obj" },
            { "MGS3 Suitcase EVA", "MGS3 Suitcase Eva.obj" },
            { "MGS3 Suitcase Snake", "MGS3 Suitcase Snake.obj" },
            { "MGS3 Tape", "MGS3 Tape.obj" },
            { "MGS3 Tape Recorder", "MGS3 Tape Recorder.obj" },
            { "MGS3 Thermal Goggles", "MGS3 Thermal Goggles.obj" },
            { "MGS3 Torch", "MGS3 Torch.obj" },
            { "MGS3 Transmitter", "MGS3 Transmitter.obj" },
            { "MGS3 Wine Glass", "MGS3 Wine Glass.obj" },
            { "MGS2 Snake Tanker", "sna_def.obj" },
            { "MGS2 Raiden", "rai_def_mt.obj" },
            { "MGS2 Tanker Guards", "gbs_def.obj" },
            { "MGS2 Tanker Backup", "gba_def_mt.obj" },
            { "MGS2 Big Shell Guards", "gps_def_mt.obj" },
            { "MGS2 Big Shell Backup", "htc_def_mt.obj" },
            { "MGS2 NYPD", "NYPD (Armed).obj" },
            { "MGS2 Genome", "gno_def.obj" },
            { "MGS2 Genome Mecha", "gno_meca_mt.obj" },
            { "MGS2 Fortune", "for_def_sh_mt.obj" },
            { "MGS2 Emma", "ema_def_sh_mt.obj" },
            { "MGS2 Vamp Naked", "vmp_naked_sh.obj" },
            { "MGS2 Ocelot Tanker", "rev_stand_test.obj" },
            { "MGS2 Ray Prototype", "ray_def_mt.obj" },
            { "MGS2 Ray Cockpit", "ray_cockpit.obj" },
            { "MGS2 Naked Raiden", "rai_naked_sh.obj" },
            { "MGS2 Gun Cypher", "gcyp.obj" },
            { "MGS2 Ray Mass Produced", "pdray_def_mt.obj" },
            { "MGS2 Ames", "ric_def_sh.obj" },
            { "MGS2 Cardboard Box", "cardboard.obj" },
            { "MGS2 Coolant Spray", "cls_sub.obj" },
            { "MGS2 Cypher", "cyp_sh.obj" },
            { "MGS2 Directional Microphone", "dmp_sub.obj" },
            { "MGS2 Fatman", "fat_def_sh_mt.obj" },
            { "MGS2 Fatman Bombs", "c4_kaitai_a1.obj" },
            { "MGS2 Item Box 1", "box_ibox.obj" },
            { "MGS2 Item Box 2", "box2_ibox.obj" },
            { "MGS2 M4", "m4a_nm.obj" },
            { "MGS2 M9", "m92_sub.obj" },
            { "MGS2 Marine", "us_def_1.obj" },
            { "MGS2 Meryl", "mrl_def_sh_mt.obj" },
            { "MGS2 Ocelot", "rev_plant_sh_mt.obj" },
            { "MGS2 Olga Ninja", "org_tng_sh_mt.obj" },
            { "MGS2 Olga Plant", "org_plant_sh_mt.obj" },
            { "MGS2 Olga Tanker", "org_sgl.obj" },
            { "MGS2 Otacon", "otc_def_sh_mt.obj" },
            { "MGS2 Pliskin", "iro_def_sh_mt.obj" },
            { "MGS2 President", "jam_def_sh_mt.obj" },
            { "MGS2 Raiden Ninja", "rai_def_sh_mt_stage_r_vr_b_r.obj" },
            { "MGS2 Raiden Scuba", "rai_def_sh_mt_stage_r_plt1_r.obj" },
            { "MGS2 SAA", "saa.obj" },
            { "MGS2 Scott Dolph", "sco_def_light.obj" },
            { "MGS2 Seal", "sel_def_sh.obj" },
            { "MGS2 Snake (MGS1)", "sna_oss_sh_mt.obj" },
            { "MGS2 Socom", "scm.obj" },
            { "MGS2 Solidus", "sol_def_sh_mt.obj" },
            { "MGS2 Stillman", "ptr_def_sh_mt.obj" },
            { "MGS2 Tuxedo Snake", "sna_txd_sh_mt.obj" },
            { "MGS2 USP", "usp.obj" },
        };

        private void MakeTexturePanel(int w, int h, int xPos, int yPos, int labelHeight, string texPath, string name)
        {
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

            string folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, selectedModel);
            string modelFile = this.modelFileNames[selectedModel];
            string mtlFile = modelFile.Replace("obj", "mtl");
            List<string> textureFiles = this.getTexturesFromMtl(Path.Combine(folder, mtlFile));

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
            int yPos = 10, spacing = 70;
            int labelHeight = 20;

            foreach (string tex in textureFiles)
            {
                string texPath = Path.Combine(folderPath, tex);

                string name = Path.GetFileNameWithoutExtension(tex);

                MakeTexturePanel(w, h, xPos, yPos, labelHeight, texPath, name);

                yPos += labelHeight + h + spacing;
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
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;
                
                modelViewerControl.LoadModel(ofd.FileName);

                int w = 335, h = 127;
                int xPos = panelTextures.ClientSize.Width - w - 40;
                int yPos = 10, spacing = 70;
                int labelHeight = 20;

                List<string> textureFiles = this.getTexturesFromMtl(ofd.FileName.Replace(".obj", ".mtl"));
                string folderPath = Path.GetDirectoryName(ofd.FileName);

                foreach (string tex in textureFiles)
                {
                    string name = Path.GetFileNameWithoutExtension(tex);
                    string texPath = Path.Combine(folderPath, tex);
                    /* // Commented: Out of scope (external renaming tools)
                    if (Regex.IsMatch(tex, "^[0-9a-f]{6}.png$"))
                    {
                        // Noesis extracts textures from the .tri file, not the .ctxr.
                        // If a file appears to be ripped by Noesis, it must be renamed to mod.
                        // Fortunately, we have a lookup of every .ctxr name (I think).
                        string newTex = this.ctxrLookup[tex] ?? "00" + tex;
                        if (newTex != tex)
                        {
                            string newPath = Path.Combine(Path.GetDirectoryName(texPath), newTex);
                            File.Copy(texPath, newPath, true);
                            texPath = newPath;
                            name = Path.GetFileNameWithoutExtension(newTex);
                        }

                    }
                    */

                    MakeTexturePanel(w, h, xPos, yPos, labelHeight, texPath, name);

                    yPos += labelHeight + h + spacing;
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