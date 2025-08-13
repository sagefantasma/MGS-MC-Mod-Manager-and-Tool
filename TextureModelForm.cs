using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Font = System.Drawing.Font;
using Image = System.Drawing.Image;
using Application = System.Windows.Forms.Application;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class TextureModelForm : Form
    {
        private ElementHost elementHost;
        private ModelViewerControl modelViewerControl;
        private Panel panelTextures;
        private string currentModelPath;
        private string currentMtlPath;
        private bool _isCustomModel = false;

        /// <summary>
        /// A list of all the predefined models that ANTIBigBoss hand picked <br></br>
        /// A zip of them can be found here: <br></br>
        /// https://github.com/ANTIBigBoss/MGS-MC-Mod-Manager-and-Tool/releases/download/ToolsModelsandTextures/3D.Models.and.Textures.zip
        /// </summary>
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

        /// <summary>
        /// A dictionary for the predefined model names, big thanks to Jacky720 for <br></br>
        /// helping me out with organizing this and removing the old else if mess I created
        /// </summary>
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

        /// <summary>
        /// Filters the model list based on MGS2/MGS3 checkbox selections
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event data</param>
        private void FilterModels(object sender, EventArgs e)
        {
            RefreshModelSelection();
        }


        /// <summary>
        /// Reloads the model selection combo box based on current filters
        /// </summary>
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

        /// <summary>
        /// Sets up the required mod tools and assets by verifying paths and downloading necessary components.
        /// </summary>
        /// <remarks>This method performs the following steps: <list type="bullet">
        /// <item><description>Prompts the user to verify or provide the path to the mod tools if not already
        /// configured.</description></item> <item><description>Downloads the mod tools if they are not already present
        /// at the specified path.</description></item> <item><description>Prompts the user to verify or provide paths
        /// for additional required tools, such as GIMP and Python.</description></item> </list> If any step fails, the
        /// method returns <see langword="false"/> to indicate that the setup process was not completed
        /// successfully.</remarks>
        /// <param name="config">The configuration settings containing paths and options required for setup.</param>
        /// <returns><see langword="true"/> if all required tools and assets are successfully set up; otherwise, <see
        /// langword="false"/>.</returns>
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

        /// <summary>
        /// Ensures that the mod tools folder is properly configured, prompting the user if necessary.
        /// </summary>
        /// <remarks>If the mod tools folder is not already set, the method prompts the user to confirm or
        /// select a folder.  The user can choose to accept the default path, select a new folder, or cancel the
        /// operation.  If the user cancels, the method returns <see langword="false"/>.</remarks>
        /// <param name="config">The configuration settings object containing the mod tools folder path and status.</param>
        /// <returns><see langword="true"/> if the mod tools folder is successfully configured;  otherwise, <see
        /// langword="false"/> if the operation is canceled by the user.</returns>
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

        /// <summary>
        /// Verifies and configures GIMP console path through user prompts <br></br>
        /// It then saves it to the config file stored in the user's documents folder.
        /// </summary>
        /// <param name="config">Application configuration settings</param>
        /// <returns>True if path is valid, False if canceled</returns>
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

        /// <summary>
        /// Checks if a valid Python executable path is configured or available on the system,  and prompts the user to<br></br>
        /// locate or download Python if it is not found. This is needed for running the Python scripts via GIMP's Python-Fu.
        /// </summary>
        /// <remarks>This method first attempts to verify the Python executable path specified in the <br></br>
        /// configuration  or a default "python" command. If no valid Python executable is found, the user is prompted <br></br>
        /// to  either locate the Python executable manually or download it from the official Python website.  If the <br></br>
        /// user provides a valid path, it is saved to the configuration.</remarks>
        /// <param name="cfg">The configuration settings object where the Python executable path is stored.</param>
        /// <returns><see langword="true"/> if a valid Python executable path is found or successfully configured;  otherwise,
        /// <see langword="false"/>.</returns>
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

        /// <summary>
        /// More or less the same as <see cref="CheckAndPromptForPythonPath"/>, but for the GIMP Python script path.<br></br>
        /// There's not much reason this would fail, since the script is included with the mod tools.<br></br>
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Adjusts the size and location of the <see cref="elementHost"/> control (Where the 3D Models are displayed)<br></br>
        /// to occupy the right half of the client area.
        /// </summary>
        /// <remarks>This method positions the <see cref="elementHost"/> control at the midpoint of the <br></br>
        /// client area's width and resizes it to fill the right half of the client area while maintaining the full
        /// height.</remarks>
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

        /// <summary>
        /// Will load whatever pre-defined model is selected in the dropdown along with its textures.<br></br>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadPreDefinedModel_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            panelTextures.Controls.Clear();

            // Flag to allow mods to be create with models not in the predefined list
            _isCustomModel = false;

            string selectedModel = ModelSelectionComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedModel))
            {
                MessageBox.Show("Please select a model from the dropdown.");
                return;
            }

            // Look up model file in dictionary
            if (modelFileNames.TryGetValue(selectedModel, out string modelFile))
            {
                string folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, selectedModel);
                string mtlFile = Path.ChangeExtension(modelFile, ".mtl");

                string modelPath = Path.Combine(folder, modelFile);
                string mtlPath = Path.Combine(folder, mtlFile);

                LoadModelWithTextures(modelPath, mtlPath);
            }
            else
            {
                MessageBox.Show("Model configuration not found.");
            }
        }

        /// <summary>
        /// Handles the click event for changing the texture of a model.
        /// </summary>
        /// <remarks>This method allows the user to select a new texture file via an <see cref="OpenFileDialog"/> <br></br>
        /// and updates the associated model and UI elements with the new texture. <br></br>
        /// The method performs the following actions: <list type="bullet"> <item>Clears the current model from the viewer.</item>
        /// <item>Prompts the user to select a new texture file.</item> <item>Updates the material file to reference the
        /// new texture.</item> <item>Copies the selected texture file to the appropriate location.</item> <item>Updates
        /// the <see cref="PictureBox"/> to display the new texture.</item> <item>Reloads the model with the updated
        /// texture.</item> </list> If an error occurs during the process, an error message is displayed to the
        /// user.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the click event.</param>
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

        /// <summary>
        /// Restores the default texture for a specific model element when triggered by a button click.
        /// </summary>
        /// <remarks>This method updates the texture of the associated model element to its default state
        /// by: <list type="bullet"> <item><description>Clearing the current model from the viewer.</description></item>
        /// <item><description>Updating the material file to reference the default texture.</description></item>
        /// <item><description>Reloading the default texture into the associated <see
        /// cref="PictureBox"/>.</description></item> <item><description>Reloading the model into the
        /// viewer.</description></item> </list> If an error occurs during the process, a message box is displayed with
        /// the error details.</remarks>
        /// <param name="sender">The button that triggered the event. The button's <see cref="Button.Tag"/> property must contain a reference
        /// to the associated <see cref="PictureBox"/>.</param>
        /// <param name="e">The event data associated with the click event.</param>
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

        /// <summary>
        /// Renames a texture reference in a material (.mtl) file by replacing all <br></br> 
        /// occurrences of the old texture name with the new texture name.
        /// </summary>
        /// <remarks>If the specified material file does not exist, the method does nothing. <br></br> 
        /// If the old texture name is not found in the file, no changes are made.</remarks>
        /// <param name="mtlPath">The file path to the material (.mtl) file. Must not be null or empty.</param>
        /// <param name="oldName">The name of the texture to be replaced. Must not be null or empty.</param>
        /// <param name="newName">The new name of the texture to replace the old name. Must not be null or empty.</param>
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

        /// <summary>
        /// Generates a new file path by appending a numeric suffix to the original file name. <br></br>
        /// I created this to avoid file name conflicts and so that I could restore the original texture file name.
        /// </summary>
        /// <param name="originalPath">The original file path for which a new suffixed path is required.</param>
        /// <returns>A new file path with a numeric suffix appended to the file name, ensuring that the resulting path does not
        /// already exist on the file system.</returns>
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
                // If somehow someone gets to 999 I'll be amazed
                if (i > 999)
                    throw new IOException("Couldn't find new suffix name up to _999.");
            }
            return candidate;
        }

        /// <summary>
        /// Removes a numeric suffix from the end of a string, if present.
        /// </summary>
        /// <remarks>A numeric suffix is defined as a sequence of digits following the last underscore <br></br>
        /// ('_') in the string. If no underscore is found, or if the portion after the last underscore is not entirely<br></br>
        /// numeric, the original string is returned unchanged.</remarks>
        /// <param name="fNoExt">The input string to process. This string is expected to potentially contain a numeric suffix separated by an
        /// underscore.</param>
        /// <returns>The input string without the numeric suffix if one is present and valid; otherwise, the original string.</returns>
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

        /// <summary>
        /// Removes any numeric suffix from the file name in the specified path <br></br>
        /// while preserving the directory and file extension.
        /// </summary>
        /// <param name="path">The full file path from which to remove the numeric suffix from the file name.</param>
        /// <returns>A new file path with the numeric suffix removed from the file name. <br></br>
        /// The directory and file extension remain unchanged.</returns>
        private string RemoveSuffix(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileNoExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            fileNoExt = StripNumericSuffix(fileNoExt);
            return Path.Combine(dir, fileNoExt + ext);
        }

        /// <summary>
        /// Restores all material file references in the specified MTL file to their original file names.
        /// </summary>
        /// <remarks>This method reads the specified MTL file, identifies texture file references <br></br>
        /// (lines starting with "map_Kd"), and updates them to use only the original file names, removing any directory paths <br></br>
        /// or suffixes. The updated MTL file is then written back to the same location.</remarks>
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

        /// <summary>
        /// Loads an image from the specified file path without applying any locking mechanisms.
        /// </summary>
        /// <remarks>This method does not lock the file during or after loading. The caller is responsible <br></br>
        /// for ensuring that the file is not modified or deleted while it is being accessed.</remarks>
        /// <param name="path">The file path of the image to load. Must be a valid path to an existing file.</param>
        /// <returns>An <see cref="Image"/> object representing the loaded image, or <see langword="null"/> if the file does not
        /// exist.</returns>
        private Image LoadImageNoLock(string path)
        {
            if (!File.Exists(path))
                return null;
            using (var fs = File.OpenRead(path))
            {
                return Image.FromStream(fs);
            }
        }

        /// <summary>
        /// This method allows the user to choose between converting all CTXR files in a folder <br></br>
        /// or a single CTXR file. If the user selects folder conversion, all CTXR files in the selected folder are <br></br>
        /// converted to PNG format. If the user selects single file conversion, the selected CTXR file is converted to <br></br>
        /// PNG format. This will mostly be helpful with custom models from SeaLouse. <br></br><br></br>
        /// </summary>
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

        /// <summary>
        /// This is the main way creating mods is handled, it will take the PNG<br></br>
        /// and convert it to DDS using GIMP's Python-Fu script, then from there <br></br>
        /// it will convert the DDS to CTXR using the CtxrTool.exe.<br></br>
        /// </summary>
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

        /// <summary>
        /// I decided to keep this option in the rare case of someone solely wanting to convert<br></br>
        /// a PNG to DDS for whatever reason. Might be more useful later on when Delta releases.
        /// </summary>
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

        /// <summary>
        /// The main way to get an image file to be recognized by MGS2/3's Master Collection is to convert DDS files <br></br> 
        /// to CTXR files using the CtxrTool.exe.This is most helpful if someone is working on a singular texture file but <br></br>
        /// batch processing is available like with the PNG and DDS conversion methods.<br></br>
        /// </summary>
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

        /// <summary>
        /// This method is for loading a 3D Model not from the pre-defined list, this will get the most <br></br>
        /// mileage with Jacky's SeaLouse but it can be used for any custom model from anything really.<br></br><br></br>
        /// Pressing the "Create a Mod" button will ask if you're creating a mod for MGS2 or MGS3, <br></br>
        /// so the appropripate file structure/path can be created for the user.
        /// </summary>
        private void btnLoadObj_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "3D Model Files|*.obj;|All Files|*.*" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string modelPath = ofd.FileName;
                    string mtlPath = Path.ChangeExtension(modelPath, ".mtl");

                    // Bool here so we can ask the user if they wanna make an MGS2/3 mod if it's not on the list
                    _isCustomModel = true;

                    if (File.Exists(mtlPath))
                    {
                        LoadModelWithTextures(modelPath, mtlPath);
                    }
                    else
                    {
                        modelViewerControl.LoadModel(modelPath);
                        MessageBox.Show("No MTL file found for textures");
                    }
                }
            }
        }

        /// <summary>
        /// Loads a 3D model and its associated textures, including those referenced in the material file (MTL) and any<br></br>
        /// additional texture files found in the model's directory. This helped solve the secular map files not being loaded <br></br>
        /// in the list along with some textures being loaded multiple times if their mtl file referenced them multiple times.
        /// </summary>
        /// <param name="modelPath">The file path to the 3D model file to be loaded.</param>
        /// <param name="mtlPath">The file path to the material file (MTL) associated with the 3D model.</param>
        private void LoadModelWithTextures(string modelPath, string mtlPath)
        {
            currentModelPath = modelPath;
            currentMtlPath = mtlPath;

            RestoreAllMtlReferencesToOriginal(mtlPath);
            modelViewerControl.LoadModel(modelPath);

            // Get all textures: MTL references + all PNGs in folder
            string folderPath = Path.GetDirectoryName(modelPath);
            var allTextures = GetAllTexturesInFolder(folderPath);
            var mtlTextures = ParseMtlForTextures(mtlPath);

            // Stops us from seeing the same texture multiple times reference more than once in the MTL file
            var combinedTextures = allTextures.Union(mtlTextures, StringComparer.OrdinalIgnoreCase).ToList();

            DisplayTextures(combinedTextures, folderPath);
        }

        /// <summary>
        /// Gets all the PNG files in a folder to help us compare and figure out <br></br>
        /// what we should load in the LoadModelWithTextures method.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private List<string> GetAllTexturesInFolder(string folderPath)
        {
            return Directory.GetFiles(folderPath, "*.png")
                .Select(Path.GetFileName)
                .ToList();
        }

        private List<string> ParseMtlForTextures(string mtlPath)
        {
            var textures = new List<string>();
            string mtlDir = Path.GetDirectoryName(mtlPath);

            foreach (string line in File.ReadLines(mtlPath))
            {
                if (line.Trim().StartsWith("map_Kd", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                    {
                        textures.Add(parts[1]);
                    }
                }
            }
            return textures;
        }

        /// <summary>
        /// Attempts to locate a texture file by checking the specified file path and several fallback paths.
        /// </summary>
        /// <remarks>This method first checks if the provided <paramref name="textureFile"/> exists as-is. <br></br>
        /// If not, it constructs and checks several fallback paths relative to <paramref name="baseDir"/>, including <br></br>
        /// appending a ".png" extension or using only the file name portion of the path. The first valid file path
        /// found is returned.</remarks>
        /// <param name="baseDir">The base directory to use when constructing fallback paths.</param>
        /// <param name="textureFile">The original texture file path, which may include relative paths or require cleaning.</param>
        /// <returns>The full path to the texture file if found; otherwise, <see langword="null"/> if no matching file is
        /// located.</returns>
        private string FindTextureFile(string baseDir, string textureFile)
        {
            string cleanTextureFile = textureFile
                .Replace("\"", "")
                .Replace("\\\\", "\\")
                .Trim();

            if (File.Exists(cleanTextureFile))
            {
                return cleanTextureFile;
            }

            string[] candidates = {
            Path.Combine(baseDir, cleanTextureFile),
            Path.Combine(baseDir, cleanTextureFile + ".png"),
            Path.Combine(baseDir, Path.ChangeExtension(cleanTextureFile, ".png")),
            Path.Combine(baseDir, Path.GetFileName(cleanTextureFile)),
            Path.Combine(baseDir, Path.GetFileName(cleanTextureFile) + ".png")
        };

            return candidates.FirstOrDefault(File.Exists);
        }

        /// <summary>
        /// Displays a list of textures in a panel, including their names, resolutions, and options to change or restore
        /// them.
        /// </summary>
        /// <remarks>Each texture is displayed with its name, resolution, and two buttons: one to change <br></br>
        /// the texture and another to restore it to its default state.  Textures with duplicate names are displayed <br></br>
        /// only once. If a texture cannot be loaded, it is indicated with a placeholder.</remarks>
        /// <param name="textureFiles">A list of texture file names to be displayed.</param>
        /// <param name="folderPath">The folder path where the texture files are located.</param>
        private void DisplayTextures(List<string> textureFiles, string folderPath)
        {
            panelTextures.Controls.Clear();
            int w = 335, h = 127;
            int xPos = panelTextures.ClientSize.Width - w - 40;
            int yPos = 10;
            int spacing = 40;
            int labelHeight = 20;

            // Use HashSet to track which textures we've shown
            var shownTextures = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (string tex in textureFiles)
            {
                string texPath = FindTextureFile(folderPath, tex);
                if (string.IsNullOrEmpty(texPath)) continue;

                // Skip if we've already shown this exact texture to avoid the duplicates issue
                string textureKey = Path.GetFileName(texPath);
                if (shownTextures.Contains(textureKey)) continue;
                shownTextures.Add(textureKey);

                string name = Path.GetFileNameWithoutExtension(texPath);
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
                    Font = new System.Drawing.Font("Arial", 10, FontStyle.Bold),
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

                System.Drawing.Image img = LoadImageNoLock(texPath);
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

        private void BackButton_Click(object sender, EventArgs e)
        {
            ReturnToMainMenu();
        }

        /// <summary>
        /// Handles the creation of a new mod, including gathering user input, setting up the mod folder <br></br>
        /// structure, and processing texture files for compatibility with the game.
        /// </summary>
        /// <remarks>This method prompts the user for mod details such as the mod name, an optional image, <br></br>
        /// and a description.  It determines the target game (Metal Gear Solid 2 or 3) based on user input or <br></br>
        /// predefined model selection,  and creates the necessary folder structure for the mod.   Texture files are <br></br>
        /// processed and converted to the appropriate format for the game, and any changes are  saved to the mod <br></br>
        /// folder. The method ensures that required tools and paths are configured before proceeding.  If no textures <br></br>
        /// are modified, the method will prompt the user to make changes before creating the mod.</remarks>
        /// <param name="sender">The source of the event, typically the button that was clicked.</param>
        /// <param name="e">The event data associated with the button click.</param>
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

            // This is where a custom model is handled and we ask which game it is for
            bool isMgs2;
            if (_isCustomModel)
            {
                var result = MessageBox.Show("Is this mod for Metal Gear Solid 2?", "Game Selection",
                                            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                isMgs2 = (result == DialogResult.Yes);
            }
            else
            {
                string selectedModel = ModelSelectionComboBox.SelectedItem?.ToString() ?? "";
                isMgs2 = selectedModel.StartsWith("MGS2", StringComparison.OrdinalIgnoreCase);
            }

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

            string conversionFolder = Path.Combine(modFolder, "textures", "flatlist", "ovr_stm", "_win");
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

        /// <summary>
        /// Displays a modal dialog box with a prompt message and an input field, allowing the user to enter text.<br></br>
        /// They'd use this to enter mod names, or descriptions for a mod so that the hover events in MGS2/3 Modding Forms <br></br>
        /// Will show a summary and preview image to the user.
        /// </summary>
        /// <param name="prompt">The message displayed to the user in the dialog box.</param>
        /// <param name="title">The title of the dialog box.</param>
        /// <returns>The text entered by the user, trimmed of leading and trailing whitespace. <br></br> 
        /// Returns an empty string if the user cancels the dialog or closes it without providing input.</returns>
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

        /// <summary>
        /// This was basically just to save me time answering the same questions <br></br>
        /// I'd get on Nexus Mods or Discord.
        /// </summary>
        /// <returns></returns>
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