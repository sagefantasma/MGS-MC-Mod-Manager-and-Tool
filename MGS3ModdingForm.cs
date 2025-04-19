using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IOSearchOption = System.IO.SearchOption;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MGS3ModdingForm : Form
    {
        #region Fields & Mapping Dictionaries
        private ConfigSettings config;
        private RichTextBox modInfoRichTextBox;
        private FlowLayoutPanel modListPanel;
        private FileExplorerManager fileExplorerManager;
        private ModListManager modListManager;

        private readonly string[] expectedPaths = new string[]
        {
            "assets/geom/us",
            "assets/gcx/fr/_bp",
            "assets/gcx/gr/_bp",
            "assets/gcx/it/_bp",
            "assets/gcx/sp/_bp",
            "textures/flatlist/_win",
            "textures/flatlist/ovr_eu/_win",
            "textures/flatlist/ovr_PS3/_win",
            "textures/flatlist/ovr_stm/_win",
            "textures/flatlist/ovr_stm/ovr_eu/_win",
            "textures/flatlist/ovr_stm/ctrltype_kbd/_win",
            "textures/flatlist/ovr_stm/ctrltype_kbd/type_a/_win",
            "textures/flatlist/ovr_stm/ctrltype_kbd/type_b/_win",
            "textures/flatlist/ovr_stm/ctrltype_nx",
            "textures/flatlist/ovr_stm/ctrltype_ps4/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps4/ovr_us/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps4/ovr_jp/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/ovr_us/_win",
            "textures/flatlist/ovr_stm/ctrltype_ps5/ovr_jp/_win",
            "textures/flatlist/ovr_stm/ctrltype_stmd/_win",
            "textures/flatlist/ovr_stm/ctrltype_xs/_win",
            "textures/flatlist/ovr_stm/ovr_us/_win",
            "textures/flatlist/ovr_us/_win",
            "us/bgm",
            "us/bgm_2",
            "assets/mtar/fr",
            "assets/mtar/gr",
            "assets/mtar/it",
            "assets/mtar/jp",
            "assets/mtar/sp",
            "assets/mtar/us",
            "assets/gcx/fr/_bp",
            "assets/gcx/gr/_bp",
            "assets/gcx/it/_bp",
            "assets/gcx/sp/_bp"
        };
        #endregion

        #region Constructor & Load Event
        public MGS3ModdingForm()
        {
            InitializeComponent();
            this.FormClosing += MGS3ModdingForm_FormClosing;

            hoverPictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(149, 149, 125)
            };

            modInfoRichTextBox = new RichTextBox
            {
                Multiline = true,
                ReadOnly = true,
                Visible = false,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(149, 149, 125),
                ScrollBars = RichTextBoxScrollBars.None,
                WordWrap = true,
                MaximumSize = new Size(364, 142),
                Size = new Size(364, 142),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold),
                ForeColor = SystemColors.ActiveCaptionText,
                Location = new Point(8, 495)
            };
            this.Controls.Add(modInfoRichTextBox);
            this.Controls.Add(hoverPictureBox);
        }

        private void MGS3ModdingForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
            config = ConfigManager.LoadSettings();

            if (!CheckAndPromptForFolderPaths())
                return;

            string gameInstallPath = config.GamePaths["MGS3"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                gameInstallPath = FindMGS3Installation();
                if (!string.IsNullOrEmpty(gameInstallPath))
                {
                    config.GamePaths["MGS3"] = gameInstallPath;
                    ConfigManager.SaveSettings(config);
                }
            }

            SetupModListPanel();

            fileExplorerManager = new FileExplorerManager(config, this, modListPanel, "MGS3", expectedPaths);
            modListManager = new ModListManager(modListPanel);

            fileExplorerManager.SetupBackupFolders();
            fileExplorerManager.SetupModFolder();
            LoadModsIntoUI();

            if (!config.Backup.MGS3BackupCompleted)
            {
                fileExplorerManager.BackupVanillaFiles(gameInstallPath);
                config.Backup.MGS3BackupCompleted = true;
                ConfigManager.SaveSettings(config);
            }
        }

        private void MGS3ModdingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }
        #endregion

        #region First Time Setup Methods
        private bool CheckAndPromptForFolderPaths()
        {
            if (!config.MGS3VanillaFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Before you can modify the files we need to make a backup of your MGS3 Files.\n\nDo you want to use the default location for the MGS3 Vanilla Files folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MGS3VanillaFolderPath,
                    "Vanilla Files Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    GoBackToMainMenu();
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MGS3VanillaFolderPath,
                        Description = "Select a folder where the 'MGS3 Vanilla Files' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MGS3VanillaFolderPath = Path.Combine(fbd.SelectedPath, "MGS3 Vanilla Files");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MGS3VanillaFolderSet = true;
            }

            if (!config.MGS3ModFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Now we need to set up a location where your mods will be stored.\n\nDo you want to use the default location for the MGS3 Mods folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MGS3ModFolderPath,
                    "MGS3 Mods Folder Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    GoBackToMainMenu();
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MGS3ModFolderPath,
                        Description = "Select a folder where the 'MGS3 Mods' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MGS3ModFolderPath = Path.Combine(fbd.SelectedPath, "MGS3 Mods");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MGS3ModFolderSet = true;
            }
            ConfigManager.SaveSettings(config);
            return true;
        }

        private string FindMGS3Installation()
        {
            string[] commonPaths =
            {
                @"C:\Program Files (x86)\Steam\steamapps\common\MGS3",
                @"A:\SteamLibrary\steamapps\common\MGS3",
                @"B:\SteamLibrary\steamapps\common\MGS3",
                @"D:\SteamLibrary\steamapps\common\MGS3",
                @"E:\SteamLibrary\steamapps\common\MGS3",
                @"F:\SteamLibrary\steamapps\common\MGS3",
                @"G:\SteamLibrary\steamapps\common\MGS3",
            };

            string foundPath = commonPaths.FirstOrDefault(Directory.Exists);
            if (!string.IsNullOrEmpty(foundPath))
            {
                return foundPath;
            }
            else
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the installation folder for MGS3 - Master Collection.";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        return fbd.SelectedPath;
                    }
                    else
                    {
                        GoBackToMainMenu();
                        return null;
                    }
                }
            }
        }

        private void GoBackToMainMenu()
        {
            LoggingManager.Instance.Log("User did not complete folder selection. Returning to Main Menu.\n");
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.FormClosing -= MGS3ModdingForm_FormClosing;
            this.Close();
        }
        #endregion

        #region Mod List GUI Setup
        private void SetupModListPanel()
        {
            int rightMargin = 50;
            int topMargin = 80;
            int bottomMargin = 80;
            int panelWidth = (int)(this.Width / 1.5);
            int panelHeight = this.Height - topMargin - bottomMargin;

            modListPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Size = new Size(panelWidth, panelHeight),
                Location = new Point(this.Width - panelWidth - rightMargin, topMargin),
                BackColor = ColorTranslator.FromHtml("#95957d"),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 10, 20, 10)
            };
            modListPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            Controls.Add(modListPanel);
        }

        private void LoadModsIntoUI()
        {
            foreach (var modName in config.Mods.ActiveMods.Keys.ToList())
            {
                string modPath = Path.Combine(fileExplorerManager.ModFolder, modName);
                if (!Directory.Exists(modPath))
                {
                    config.Mods.ActiveMods[modName] = false;
                    LoggingManager.Instance.Log($"Mod folder for '{modName}' not found during UI refresh. Marking mod as disabled.");
                }
            }
            ConfigManager.SaveSettings(config);

            modListPanel.Controls.Clear();

            string[] modDirectories = Directory.GetDirectories(fileExplorerManager.ModFolder);

            int entryWidth = modListPanel.Width - 20;
            int buttonSpacing = 10;
            int leftMargin = 5;
            int rightMargin = 10;

            foreach (string modPath in modDirectories)
            {
                string modName = Path.GetFileName(modPath);
                bool isEnabled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
                string displayName = ModListManager.WrapText(modName, 50);

                Panel modPanel = new Panel
                {
                    Width = entryWidth,
                    Height = 60,
                    BackColor = ColorTranslator.FromHtml("#95957d"),
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(0, 2, 0, 2)
                };

                Button toggleButton = new Button
                {
                    Text = isEnabled ? "Installed" : "Not Installed",
                    Size = new Size(150, 40),
                    Tag = modName,
                    Font = new Font(Font.FontFamily, Font.Size + 4, FontStyle.Bold),
                    BackColor = isEnabled ? Color.Green : Color.Red,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                toggleButton.FlatAppearance.BorderSize = 0;
                toggleButton.Click += fileExplorerManager.ToggleModState;

                Label modLabel = new Label
                {
                    Text = displayName,
                    AutoSize = false,
                    Font = new Font(Font.FontFamily, Font.Size + 4, FontStyle.Bold),
                    ForeColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 0, 0, 0)
                };

                Button renameButton = new Button
                {
                    Text = "Edit",
                    Size = new Size(100, 40),
                    Tag = modName,
                    Font = new Font(Font.FontFamily, Font.Size + 4, FontStyle.Bold),
                    BackColor = Color.LightBlue,
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat
                };
                renameButton.FlatAppearance.BorderSize = 0;
                renameButton.Click += RenameMod;

                Button deleteButton = new Button
                {
                    Text = "Delete",
                    Size = new Size(80, 40),
                    Tag = modName,
                    Font = new Font(Font.FontFamily, Font.Size + 4, FontStyle.Bold),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                deleteButton.FlatAppearance.BorderSize = 0;
                deleteButton.Click += fileExplorerManager.DeleteMod;

                Button settingsButton = null;

                if (fileExplorerManager.IsMGSHDFixMod(modPath))
                {
                    Button btnSettings = new Button
                    {
                        Text = "HDFix Settings",
                        Size = new Size(160, 40),
                        Tag = modName,
                        Font = new Font(Font.FontFamily, Font.Size + 4, FontStyle.Bold),
                        BackColor = Color.LightBlue,
                        ForeColor = Color.Black,
                        FlatStyle = FlatStyle.Flat
                    };
                    btnSettings.FlatAppearance.BorderSize = 0;
                    btnSettings.Click += (s, e) =>
                    {
                        string iniPath = Path.Combine(modPath, "MGSHDFix.ini");
                        if (!File.Exists(iniPath))
                        {
                            MessageBox.Show("MGSHDFix.ini not found in mod folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        MGSHDFixSettingsForm settingsForm = new MGSHDFixSettingsForm("MGS3", this);
                        settingsForm.ShowDialog();
                        this.ActiveControl = modListPanel;
                    };
                    // Add btnSettings to your container (e.g., modListPanel or another control)
                    settingsButton = btnSettings;
                }


                else
                {
                    var (foundCamos, foundBoxes, foundFaces) = fileExplorerManager.GetRecognizedTextures(modPath);
                    int categoryCount = 0;
                    if (foundCamos.Count > 0) categoryCount++;
                    if (foundBoxes.Count > 0) categoryCount++;
                    if (foundFaces.Count > 0) categoryCount++;

                    if (categoryCount == 1)
                    {
                        if (foundCamos.Count > 0)
                        {
                            if (foundCamos.Count == 1)
                            {
                                settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Camo", modName, (s, e) =>
                                {
                                    MGS3TextureRenamer.ShowSingleCamoSwapDialog(
                                        config,
                                        modName,
                                        modPath,
                                        foundCamos[0],
                                        config.GamePaths["MGS3"],
                                        fileExplorerManager.RestoreVanillaFiles,
                                        fileExplorerManager.ApplyModFiles,
                                        fileExplorerManager.ReplaceOrAppendModInfoLine
                                    );
                                    this.ActiveControl = modListPanel;
                                });
                            }
                            else
                            {
                                settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Camos", modName, (s, e) =>
                                {
                                    MGS3TextureRenamer.ShowMultiCamoSwapDialog(
                                        config,
                                        modName,
                                        modPath,
                                        foundCamos,
                                        config.GamePaths["MGS3"],
                                        fileExplorerManager.RestoreVanillaFiles,
                                        fileExplorerManager.ApplyModFiles,
                                        fileExplorerManager.ReplaceOrAppendModInfoLine
                                    );
                                    this.ActiveControl = modListPanel;
                                });
                            }
                        }
                        else if (foundBoxes.Count > 0)
                        {
                            if (foundBoxes.Count == 1)
                            {
                                settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Box", modName, (s, e) =>
                                {
                                    MGS3TextureRenamer.ShowSingleBoxSwapDialog(
                                        config,
                                        modName,
                                        modPath,
                                        foundBoxes[0],
                                        config.GamePaths["MGS3"],
                                        fileExplorerManager.RestoreVanillaFiles,
                                        fileExplorerManager.ApplyModFiles,
                                        fileExplorerManager.ReplaceOrAppendModInfoLine
                                    );
                                    this.ActiveControl = modListPanel;
                                });
                            }
                            else
                            {
                                settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Boxes", modName, (s, e) =>
                                {
                                    MGS3TextureRenamer.ShowMultiBoxSwapDialog(
                                        config,
                                        modName,
                                        modPath,
                                        foundBoxes,
                                        config.GamePaths["MGS3"],
                                        fileExplorerManager.RestoreVanillaFiles,
                                        fileExplorerManager.ApplyModFiles,
                                        fileExplorerManager.ReplaceOrAppendModInfoLine
                                    );
                                    this.ActiveControl = modListPanel;
                                });
                            }
                        }
                        else
                        {
                            settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Face(s)", modName, (s, e) =>
                            {
                                var faceList = (foundFaces.Count == 1) ? new List<string> { foundFaces[0] } : foundFaces;
                                MGS3TextureRenamer.ShowUnifiedFaceSwapDialog(
                                    config,
                                    modName,
                                    modPath,
                                    faceList,
                                    config.GamePaths["MGS3"],
                                    fileExplorerManager.RestoreVanillaFiles,
                                    fileExplorerManager.ApplyModFiles,
                                    fileExplorerManager.ReplaceOrAppendModInfoLine
                                );
                                this.ActiveControl = modListPanel;
                            });                          
                        }
                    }
                    else if (categoryCount > 1)
                    {
                        settingsButton = MGS3TextureRenamer.CreateChangeButton("Change Textures", modName, (s, e) =>
                        {
                            MGS3TextureRenamer.ShowMultiTextureSwapDialog(
                                config,
                                modName,
                                modPath,
                                foundCamos,
                                foundBoxes,
                                foundFaces,
                                config.GamePaths["MGS3"],
                                fileExplorerManager.RestoreVanillaFiles,
                                fileExplorerManager.ApplyModFiles,
                                fileExplorerManager.ReplaceOrAppendModInfoLine
                            );
                            this.ActiveControl = modListPanel;
                        });
                    }
                }

                toggleButton.Location = new Point(leftMargin, (modPanel.Height - toggleButton.Height) / 2);

                int currentRightX = modPanel.Width - rightMargin;

                currentRightX -= deleteButton.Width;
                deleteButton.Location = new Point(currentRightX, (modPanel.Height - deleteButton.Height) / 2);
                currentRightX -= buttonSpacing;

                currentRightX -= renameButton.Width;
                renameButton.Location = new Point(currentRightX, (modPanel.Height - renameButton.Height) / 2);
                currentRightX -= buttonSpacing;

                if (settingsButton != null)
                {
                    currentRightX -= settingsButton.Width;
                    settingsButton.Location = new Point(currentRightX, (modPanel.Height - settingsButton.Height) / 2);
                    currentRightX -= buttonSpacing;
                }

                int labelX = toggleButton.Right + buttonSpacing;
                int labelWidth = currentRightX - labelX - buttonSpacing;
                modLabel.Location = new Point(labelX, 0);
                modLabel.Size = new Size(labelWidth, modPanel.Height);

                modPanel.Controls.Add(toggleButton);
                modPanel.Controls.Add(modLabel);
                modPanel.Controls.Add(renameButton);
                modPanel.Controls.Add(deleteButton);
                if (settingsButton != null)
                    modPanel.Controls.Add(settingsButton);

                void AttachHoverEvents(Control c)
                {
                    c.MouseEnter += (s, e) => ShowModImage(modName, modPanel);
                    c.MouseLeave += (s, e) => HideModImage();
                }
                AttachHoverEvents(modPanel);
                AttachHoverEvents(toggleButton);
                AttachHoverEvents(modLabel);
                AttachHoverEvents(renameButton);
                AttachHoverEvents(deleteButton);
                if (settingsButton != null) AttachHoverEvents(settingsButton);

                modListPanel.Controls.Add(modPanel);
            }
        }      

        #endregion

        
        #region Hover Methods
        private void ShowModImage(string modName, Control modControl)
        {
            string modImagePath = Path.Combine(fileExplorerManager.ModFolder, modName, "Mod Details", "Mod Image.png");
            string modInfoPath = Path.Combine(fileExplorerManager.ModFolder, modName, "Mod Details", "Mod Info.txt");
            if (File.Exists(modImagePath))
                hoverPictureBox.Image = Image.FromFile(modImagePath);
            else
                hoverPictureBox.Image = null;
            hoverPictureBox.BackColor = Color.FromArgb(149, 149, 125);
            hoverPictureBox.Size = new Size(364, 270);
            hoverPictureBox.Location = new Point(8, 219);
            hoverPictureBox.Visible = true;
            hoverPictureBox.BringToFront();
            ShowModInfo(modInfoPath);
        }

        private void ShowModInfo(string modInfoPath)
        {
            if (File.Exists(modInfoPath))
                modInfoRichTextBox.Text = File.ReadAllText(modInfoPath);
            else
                modInfoRichTextBox.Text = string.Empty;
            modInfoRichTextBox.Location = new Point(8, 495);
            modInfoRichTextBox.Visible = true;
            modInfoRichTextBox.BringToFront();
        }

        private void HideModImage()
        {
            if (hoverPictureBox.Image != null)
            {
                hoverPictureBox.Image.Dispose();
                hoverPictureBox.Image = null;
            }
            hoverPictureBox.Visible = false;
            modInfoRichTextBox.Visible = false;
        }
        #endregion

        #region Mod Renaming and Editing Methods
        private void RenameMod(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null)
                return;

            string oldModName = button.Tag.ToString();
            string oldModPath = Path.Combine(fileExplorerManager.ModFolder, oldModName);
            string newModPath = oldModPath;
            if (MessageBox.Show("Do you want to rename the mod?", "Rename Mod", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string newModName = PromptForModName(oldModName);
                if (string.IsNullOrWhiteSpace(newModName) || newModName == oldModName)
                    return;
                newModPath = Path.Combine(fileExplorerManager.ModFolder, newModName);
                if (Directory.Exists(newModPath))
                {
                    MessageBox.Show($"A mod with the name '{newModName}' already exists.", "Rename Mod", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    Directory.CreateDirectory(newModPath);
                    Directory.CreateDirectory(Path.Combine(newModPath, "Mod Details"));
                    foreach (string dirPath in Directory.GetDirectories(oldModPath, "*", IOSearchOption.AllDirectories))
                    {
                        string newDirPath = dirPath.Replace(oldModPath, newModPath);
                        if (!newDirPath.EndsWith("Mod Details", StringComparison.OrdinalIgnoreCase))
                            Directory.CreateDirectory(newDirPath);
                    }
                    foreach (string filePath in Directory.GetFiles(oldModPath, "*.*", IOSearchOption.AllDirectories))
                    {
                        string newFilePath = filePath.Replace(oldModPath, newModPath);
                        if (!newFilePath.Contains(Path.Combine(newModPath, "Mod Details")))
                            File.Move(filePath, newFilePath);
                    }
                    FileSystem.DeleteDirectory(oldModPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    UpdateModNameInConfig(oldModName, newModName);
                    oldModPath = newModPath;
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show($"Access denied while renaming mod '{oldModName}':\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming mod '{oldModName}':\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            string modDetailsPath = Path.Combine(oldModPath, "Mod Details");
            if (!Directory.Exists(modDetailsPath))
                Directory.CreateDirectory(modDetailsPath);
            if (MessageBox.Show("Do you want to select a new mod image?", "Select Mod Image", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SelectModImage(modDetailsPath);
            if (MessageBox.Show("Do you want to edit the mod description?", "Edit Mod Description", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                EditModDescription(modDetailsPath);
            LoadModsIntoUI();
        }

        private string PromptForModName(string currentName)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 150;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Rename Mod";
                prompt.StartPosition = FormStartPosition.CenterParent;
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter a new name for the mod:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Text = currentName };
                Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 80, DialogResult = DialogResult.OK };
                prompt.AcceptButton = confirmation;
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                return (prompt.ShowDialog() == DialogResult.OK) ? textBox.Text.Trim() : null;
            }
        }

        private void SelectModImage(string modDetailsPath)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title = "Select a Mod Image";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string selectedImagePath = ofd.FileName;
                    string destinationImagePath = Path.Combine(modDetailsPath, "Mod Image.png");
                    File.Copy(selectedImagePath, destinationImagePath, true);
                }
            }
        }

        private void EditModDescription(string modDetailsPath)
        {
            string modInfoPath = Path.Combine(modDetailsPath, "Mod Info.txt");
            using (Form prompt = new Form())
            {
                prompt.Width = 400;
                prompt.Height = 300;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Edit Mod Description";
                prompt.StartPosition = FormStartPosition.CenterParent;
                Label textLabel = new Label() { Left = 20, Top = 20, Text = "Enter a new description for the mod:" };
                TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 340, Height = 150, Multiline = true, ScrollBars = ScrollBars.Vertical };
                if (File.Exists(modInfoPath))
                    textBox.Text = File.ReadAllText(modInfoPath);
                Button confirmation = new Button() { Text = "OK", Left = 280, Width = 80, Top = 220, DialogResult = DialogResult.OK };
                prompt.AcceptButton = confirmation;
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                if (prompt.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(modInfoPath, textBox.Text.Trim());
            }
        }

        private void UpdateModNameInConfig(string oldModName, string newModName)
        {
            bool isActive = config.Mods.ActiveMods.ContainsKey(oldModName) && config.Mods.ActiveMods[oldModName];
            config.Mods.ActiveMods.Remove(oldModName);
            config.Mods.ActiveMods[newModName] = isActive;
            if (config.Mods.ModMappings.ContainsKey(oldModName))
            {
                var mappings = config.Mods.ModMappings[oldModName];
                config.Mods.ModMappings.Remove(oldModName);
                config.Mods.ModMappings[newModName] = mappings;
            }
            if (config.Mods.ReplacedFiles.ContainsKey(oldModName))
            {
                var replacedFiles = config.Mods.ReplacedFiles[oldModName];
                config.Mods.ReplacedFiles.Remove(oldModName);
                config.Mods.ReplacedFiles[newModName] = replacedFiles;
            }
            ConfigManager.SaveSettings(config);
        }
        #endregion

        #region Other Event Handlers
        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from MGS3.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }

        private void RefreshMods_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Refreshing mods list in MGS3 form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MGS3ModdingForm newForm = new MGS3ModdingForm();
            newForm.Show();
            this.Hide();
        }

        private void RebuildVanillaFiles_Click(object sender, EventArgs e)
        {
            string gameInstallPath = config.GamePaths["MGS3"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                MessageBox.Show("Game installation not found. Please set the game path first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            fileExplorerManager.BackupVanillaFiles(gameInstallPath);
            config.Backup.MGS3BackupCompleted = true;
            ConfigManager.SaveSettings(config);

            MessageBox.Show("Vanilla files have been rebuilt successfully.",
                "Rebuild Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MoveVanillaFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MGS3VanillaFolderPath,
                Description = "Select a new location for the MGS3 Vanilla Files folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MGS3 Vanilla Files");
                    try
                    {
                        Directory.Move(config.MGS3VanillaFolderPath, newFolderPath);
                        fileExplorerManager.SetupBackupFolders();
                        config.MGS3VanillaFolderPath = newFolderPath;
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("Vanilla Files folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving Vanilla Files folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void MoveMGS3ModFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MGS3ModFolderPath,
                Description = "Select a new location for the MGS3 Mods folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MGS3 Mods");
                    try
                    {
                        Directory.Move(fileExplorerManager.ModFolder, newFolderPath);
                        config.MGS3ModFolderPath = newFolderPath;
                        fileExplorerManager.SetupModFolder();
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("MGS3 Mods folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving MGS3 Mods folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddMods_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you adding a single mod?",
                "Add Mod(s)", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Cancel)
                return;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = fileExplorerManager.ModFolder,
                Description = (result == DialogResult.Yes) ?
                    "Select the mod folder you want to add." :
                    "Select the folder containing the mods you want to add."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    if (result == DialogResult.Yes)
                        fileExplorerManager.ProcessModFolder(fbd.SelectedPath);
                    else if (result == DialogResult.No)
                    {
                        string[] modDirs = Directory.GetDirectories(fbd.SelectedPath);
                        if (modDirs.Length == 0)
                        {
                            MessageBox.Show("The selected folder does not contain any mod folders.",
                                "No Mods Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        foreach (string modPath in modDirs)
                        {
                            fileExplorerManager.ProcessModFolder(modPath);
                        }
                    }
                    ConfigManager.SaveSettings(config);
                    LoadModsIntoUI();
                }
            }
        }

        private void DownloadModsMGS3_Click(object sender, EventArgs e)
        {
            DownloadForm downloadManager = new DownloadForm();
            downloadManager.ShowDialog();
        }

        private void AudioSwap_Click(object sender, EventArgs e)
        {
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            AudioSwapperForm form6 = new AudioSwapperForm();
            form6.Show();
            this.Hide();
        }
        #endregion
    }
}
