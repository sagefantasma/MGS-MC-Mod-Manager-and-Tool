using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MG1andMG2ModdingForm : Form
    {
        #region Fields & Mapping Dictionaries
        private ConfigSettings config;
        private FlowLayoutPanel modListPanel;
        private FileExplorerManager fileExplorerManager;
        private string gameBasePath;
        private string bgm2Dir;
        private bool mgs3Installed = false;
        private string mgs3GameBasePath;
        private string mgs3BgmDir;

        private readonly string[] expectedPaths = new string[]
        {
            "textures/flatlist/_win",
            "us/bgm_2",
        };
        #endregion

        #region Constructor & Load Event
        public MG1andMG2ModdingForm()
        {
            InitializeComponent();
            this.FormClosing += MG1andMG2ModdingForm_FormClosing;
        }

        private void MG1andMG2ModdingForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
            config = ConfigManager.LoadSettings();

            if (!CheckAndPromptForFolderPaths())
                return;

            string gameInstallPath = config.GamePaths["MG1andMG2"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                gameInstallPath = FindMG1andMG2Installation();
                if (!string.IsNullOrEmpty(gameInstallPath))
                {
                    config.GamePaths["MG1andMG2"] = gameInstallPath;
                    ConfigManager.SaveSettings(config);
                }
            }

            fileExplorerManager = new FileExplorerManager(config, this, modListPanel, "MG1andMG2", expectedPaths);
            fileExplorerManager.SetupBackupFolders();
            fileExplorerManager.SetupModFolder();

            if (!config.Backup.MG1andMG2BackupCompleted)
            {
                fileExplorerManager.BackupVanillaFiles(gameInstallPath);
                config.Backup.MG1andMG2BackupCompleted = true;
                ConfigManager.SaveSettings(config);
            }

            dataGridSdtTracks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridSdtTracks.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dataGridSdtTracks.DefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            dataGridSdtTracks.DefaultCellStyle.ForeColor = Color.Black;
            dataGridSdtTracks.DefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.EnableHeadersVisualStyles = false;
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            dataGridSdtTracks.RowHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridSdtTracks.RowHeadersDefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.DataError += DataGridSdtTracks_DataError;
            dataGridSdtTracks.EditingControlShowing += DataGridSdtTracks_EditingControlShowing;
            dataGridSdtTracks.CellContentClick += DataGridSdtTracks_CellContentClick;
            dataGridSdtTracks.AllowUserToAddRows = false;

            if (string.IsNullOrEmpty(config.GamePaths["MG1andMG2"]) || !Directory.Exists(config.GamePaths["MG1andMG2"]))
            {
                MessageBox.Show("MG1/2 game path is not configured or doesn't exist. Please update your settings.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            gameBasePath = Path.Combine(config.GamePaths["MG1andMG2"], "us");
            bgm2Dir = Path.Combine(gameBasePath, "bgm_2");

            if (!Directory.Exists(config.MG1andMG2VanillaFolderPath))
            {
                MessageBox.Show("MG1/2 Vanilla Files folder is not configured or doesn't exist. Please update your settings.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(config.GamePaths["MGS3"]) && Directory.Exists(config.GamePaths["MGS3"]))
            {
                mgs3Installed = true;
                mgs3GameBasePath = Path.Combine(config.GamePaths["MGS3"], "us");
                mgs3BgmDir = Path.Combine(mgs3GameBasePath, "bgm");
                checkboxMGS3.Enabled = true;
            }
            else
            {
                mgs3Installed = false;
                checkboxMGS3.Checked = false;
                checkboxMGS3.Enabled = false;
            }

            InitializeDataGridView();

            checkboxMG1.Checked = checkboxMG2.Checked = true;
            if (mgs3Installed)
                checkboxMGS3.Checked = true;
        }

        private void MG1andMG2ModdingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");
            Application.Exit();
        }
        #endregion

        #region First Time Setup Methods
        private bool CheckAndPromptForFolderPaths()
        {
            if (!config.MG1andMG2VanillaFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Before you can modify the files we need to make a backup of your MG1 and MG2 Files.\n\nDo you want to use the default location for the MG1andMG2 Vanilla Files folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MG1andMG2VanillaFolderPath,
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
                        SelectedPath = config.MG1andMG2VanillaFolderPath,
                        Description = "Select a folder where the 'MG1andMG2 Vanilla Files' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MG1andMG2VanillaFolderPath = Path.Combine(fbd.SelectedPath, "MG1andMG2 Vanilla Files");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MG1andMG2VanillaFolderSet = true;
            }

            if (!config.MG1andMG2ModFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Now we need to set up a location where your mods will be stored.\n\nDo you want to use the default location for the MG1andMG2 Mods folder?\n\nClick 'No' if you'd like to select your own location" +
                    "\nDefault location:\n" + config.MG1andMG2ModFolderPath,
                    "MG1andMG2 Mods Folder Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    GoBackToMainMenu();
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MG1andMG2ModFolderPath,
                        Description = "Select a folder where the 'MG1andMG2 Mods' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MG1andMG2ModFolderPath = Path.Combine(fbd.SelectedPath, "MG1andMG2 Mods");
                        }
                        else
                        {
                            GoBackToMainMenu();
                            return false;
                        }
                    }
                }
                config.MG1andMG2ModFolderSet = true;
            }
            ConfigManager.SaveSettings(config);
            return true;
        }

        private string FindMG1andMG2Installation()
        {
            string[] commonPaths =
            {
                @"C:\Program Files (x86)\Steam\steamapps\common\MG and MG2",
                @"A:\SteamLibrary\steamapps\common\MG and MG2",
                @"B:\SteamLibrary\steamapps\common\MG and MG2",
                @"D:\SteamLibrary\steamapps\common\MG and MG2",
                @"E:\SteamLibrary\steamapps\common\MG and MG2",
                @"F:\SteamLibrary\steamapps\common\MG and MG2",
                @"G:\SteamLibrary\steamapps\common\MG and MG2",
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
                    fbd.Description = "Select the installation folder for MG and MG2 - Master Collection.";
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
            this.FormClosing -= MG1andMG2ModdingForm_FormClosing;
            this.Close();
        }
        #endregion

        #region DataGridView Setup
        private void InitializeDataGridView()
        {
            dataGridSdtTracks.Columns.Clear();
            dataGridSdtTracks.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dataGridSdtTracks.GridColor = Color.Black;
            dataGridSdtTracks.RowHeadersVisible = false;
            dataGridSdtTracks.EnableHeadersVisualStyles = false;
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridSdtTracks.Font.FontFamily, dataGridSdtTracks.Font.Size + 2, FontStyle.Bold);
            dataGridSdtTracks.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn colFile = new DataGridViewTextBoxColumn
            {
                HeaderText = "SDT File",
                Name = "colFile",
                ReadOnly = true,
                Width = 400
            };
            dataGridSdtTracks.Columns.Add(colFile);

            DataGridViewTextBoxColumn colDescription = new DataGridViewTextBoxColumn
            {
                HeaderText = "Track Description",
                Name = "colDescription",
                ReadOnly = true,
                Width = 400
            };
            dataGridSdtTracks.Columns.Add(colDescription);

            DataGridViewComboBoxColumn colReplacement = new DataGridViewComboBoxColumn
            {
                HeaderText = "Replacement Song",
                Name = "colReplacement",
                DataSource = GetFilteredSongs(),
                DisplayMember = "DisplayName",
                ValueMember = "FileName",
                Width = 400
            };
            dataGridSdtTracks.Columns.Add(colReplacement);

            DataGridViewTextBoxColumn colCurrent = new DataGridViewTextBoxColumn
            {
                HeaderText = "Replaced Song",
                Name = "colCurrentReplacement",
                ReadOnly = true,
                Width = 80
            };
            dataGridSdtTracks.Columns.Add(colCurrent);

            DataGridViewButtonColumn colReset = new DataGridViewButtonColumn
            {
                HeaderText = "Reset Song",
                Name = "colReset",
                Text = "Reset",
                UseColumnTextForButtonValue = true,
                Width = 80,
                FlatStyle = FlatStyle.Flat
            };
            dataGridSdtTracks.Columns.Add(colReset);

            List<SongInfo> targetSongs = new List<SongInfo>();
            targetSongs.AddRange(AudioSdtManager.MG1Songs);
            targetSongs.AddRange(AudioSdtManager.MG2Songs);
            foreach (SongInfo song in targetSongs)
            {
                string fullPath = Path.Combine(bgm2Dir, song.FileName);
                if (File.Exists(fullPath))
                {
                    int rowIndex = dataGridSdtTracks.Rows.Add();
                    dataGridSdtTracks.Rows[rowIndex].Cells["colFile"].Value = song.FileName;
                    dataGridSdtTracks.Rows[rowIndex].Cells["colDescription"].Value = song.DisplayName;
                    string currentReplacement = "Vanilla";
                    if (config.AudioReplacements != null && config.AudioReplacements.ContainsKey(song.FileName))
                    {
                        currentReplacement = config.AudioReplacements[song.FileName];
                    }
                    dataGridSdtTracks.Rows[rowIndex].Cells["colCurrentReplacement"].Value = currentReplacement;
                    dataGridSdtTracks.Rows[rowIndex].Cells["colReplacement"].Value = "";
                }
            }
        }

        private List<SongInfo> GetFilteredSongs()
        {
            List<SongInfo> songs = new List<SongInfo>();
            if (checkboxMG1.Checked)
                songs.AddRange(AudioSdtManager.MG1Songs);
            if (checkboxMG2.Checked)
                songs.AddRange(AudioSdtManager.MG2Songs);
            if (checkboxMGS3.Enabled && checkboxMGS3.Checked)
                songs.AddRange(AudioSdtManager.MGS3Songs);
            return songs;
        }

        private void UpdateComboBoxColumn()
        {
            if (dataGridSdtTracks.Columns["colReplacement"] is DataGridViewComboBoxColumn col)
            {
                var filteredSongs = GetFilteredSongs();
                col.DataSource = filteredSongs;
                foreach (DataGridViewRow row in dataGridSdtTracks.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        var cell = row.Cells["colReplacement"];
                        bool valid = false;
                        if (cell.Value != null)
                        {
                            foreach (var song in filteredSongs)
                            {
                                if (song.FileName.Equals(cell.Value.ToString(), StringComparison.OrdinalIgnoreCase))
                                {
                                    valid = true;
                                    break;
                                }
                            }
                        }
                        if (!valid)
                        {
                            cell.Value = "";
                        }
                    }
                }
            }
        }

        private void DataGridSdtTracks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is ArgumentException)
            {
                if (dataGridSdtTracks.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell cell)
                {
                    cell.Value = "";
                }
                e.ThrowException = false;
            }
        }

        private void DataGridSdtTracks_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridSdtTracks.CurrentCell.ColumnIndex == dataGridSdtTracks.Columns["colReplacement"].Index)
            {
                if (e.Control is ComboBox combo)
                {
                    combo.BackColor = Color.FromArgb(156, 156, 124);
                    combo.ForeColor = Color.Black;
                    combo.Font = new Font(combo.Font, FontStyle.Bold);
                }
            }
        }

        private void DataGridSdtTracks_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridSdtTracks.Columns[e.ColumnIndex].Name == "colReset" && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridSdtTracks.Rows[e.RowIndex];
                ResetSongToVanilla(row);
            }
        }

        private void ResetSongToVanilla(DataGridViewRow row)
        {
            string targetFileName = row.Cells["colFile"].Value.ToString();
            string vanillaPath = Path.Combine(config.MG1andMG2VanillaFolderPath, "us", "bgm_2", targetFileName);
            if (File.Exists(vanillaPath))
            {
                string targetPath = Path.Combine(bgm2Dir, targetFileName);
                try
                {
                    if (File.Exists(targetPath))
                        File.Delete(targetPath);
                    File.Copy(vanillaPath, targetPath);
                    row.Cells["colCurrentReplacement"].Value = "Vanilla";
                    if (config.AudioReplacements != null && config.AudioReplacements.ContainsKey(targetFileName))
                    {
                        config.AudioReplacements.Remove(targetFileName);
                        ConfigManager.SaveSettings(config);
                    }
                    MessageBox.Show("Song reset to vanilla.", "Reset Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error resetting file " + targetFileName + ": " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Vanilla file not found for: " + targetFileName, "Reset Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Swap & Restore Logic
        private void swapAudioButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridSdtTracks.Rows)
            {
                if (row.IsNewRow)
                    continue;
                string targetFileName = row.Cells["colFile"].Value.ToString();
                object selectedValue = row.Cells["colReplacement"].Value;

                if (selectedValue != null && !string.IsNullOrEmpty(selectedValue.ToString()))
                {
                    string replacementFileName = selectedValue.ToString();
                    string sourcePath = "";

                    if (checkboxMGS3.Enabled && AudioSdtManager.MGS3Songs.Any(s => s.FileName.Equals(replacementFileName, StringComparison.OrdinalIgnoreCase)))
                    {
                        sourcePath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm", replacementFileName);
                        if (!File.Exists(sourcePath))
                        {
                            sourcePath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm_2", replacementFileName);
                        }
                    }
                    else
                    {
                        sourcePath = Path.Combine(config.MG1andMG2VanillaFolderPath, "us", "bgm_2", replacementFileName);
                    }

                    if (File.Exists(sourcePath))
                    {
                        string targetPath = Path.Combine(bgm2Dir, targetFileName);
                        try
                        {
                            if (File.Exists(targetPath))
                                File.Delete(targetPath);
                            File.Copy(sourcePath, targetPath);
                            row.Cells["colCurrentReplacement"].Value = replacementFileName;
                            if (config.AudioReplacements == null)
                                config.AudioReplacements = new Dictionary<string, string>();
                            config.AudioReplacements[targetFileName] = replacementFileName;
                            ConfigManager.SaveSettings(config);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error replacing file " + targetFileName + ": " + ex.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Replacement file not found in vanilla backup for: " + replacementFileName);
                    }
                }
            }
            MessageBox.Show("Swap complete.");
        }

        private void restoreAudioButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to restore all vanilla audio files? " +
                                                    "This will overwrite the current files.",
                                                    "Confirm Restore", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
                return;
            RestoreAllVanillaAudioFiles();
            foreach (DataGridViewRow row in dataGridSdtTracks.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells["colCurrentReplacement"].Value = "Vanilla";
                }
            }
            if (config.AudioReplacements == null)
                config.AudioReplacements = new Dictionary<string, string>();
            else
                config.AudioReplacements.Clear();
            ConfigManager.SaveSettings(config);
        }

        private void RestoreAllVanillaAudioFiles()
        {
            string folder = "bgm_2";
            string sourceDir = Path.Combine(config.MG1andMG2VanillaFolderPath, "us", folder);
            string targetDir = Path.Combine(gameBasePath, folder);
            if (Directory.Exists(sourceDir))
            {
                DirectoryCopy(sourceDir, targetDir, true);
            }
            else
            {
                MessageBox.Show($"Backup for {folder} not found in the vanilla folder.",
                                "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            MessageBox.Show("Vanilla audio files restored successfully.",
                            "Restore Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Source directory does not exist: " + sourceDirName);
            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);
            foreach (FileInfo file in dir.GetFiles())
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, true);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
        #endregion

        #region Other Event Handlers
        private void checkboxMG1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        private void checkboxMG2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        private void checkboxMGS3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        #region Other Event Handlers

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from MG1andMG2.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MG1andMG2ModdingForm");
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.Show();
            this.Hide();
        }

        private void RebuildVanillaFiles_Click(object sender, EventArgs e)
        {
            string gameInstallPath = config.GamePaths["MG1andMG2"];
            if (string.IsNullOrEmpty(gameInstallPath) || !Directory.Exists(gameInstallPath))
            {
                MessageBox.Show("Game installation not found. Please set the game path first.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            fileExplorerManager.BackupVanillaFiles(gameInstallPath);
            config.Backup.MG1andMG2BackupCompleted = true;
            ConfigManager.SaveSettings(config);

            MessageBox.Show("Vanilla files have been rebuilt successfully.",
                "Rebuild Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MoveVanillaFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MG1andMG2VanillaFolderPath,
                Description = "Select a new location for the MG1andMG2 Vanilla Files folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MG1andMG2 Vanilla Files");
                    try
                    {
                        Directory.Move(config.MG1andMG2VanillaFolderPath, newFolderPath);
                        fileExplorerManager.SetupBackupFolders();
                        config.MG1andMG2VanillaFolderPath = newFolderPath;
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

        private void MoveMg1and2ModFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MG1andMG2ModFolderPath,
                Description = "Select a new location for the MG1andMG2 Mods folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MG1andMG2 Mods");
                    try
                    {
                        Directory.Move(fileExplorerManager.ModFolder, newFolderPath);
                        config.MG1andMG2ModFolderPath = newFolderPath;
                        fileExplorerManager.SetupModFolder();
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("MG1andMG2 Mods folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving MG1andMG2 Mods folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void MGSHDFixForm_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Opening MGSHDFix for MG1 and MG2.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGSHDFixForm");
            MGSHDFixSettingsForm hdFixForm = new MGSHDFixSettingsForm("MG1andMG2", this);
            hdFixForm.Show();
        }
        #endregion
        #endregion
    }
}
