using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class AudioSwapperForm : Form
    {
        private ConfigSettings config;
        private string gameBasePath;
        private string bgmDir;
        private string bgm2Dir;

        public AudioSwapperForm()
        {
            InitializeComponent();
            this.FormClosing += AudioSwapperForm_FormClosing;
        }

        private void AudioSwapperForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();

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

            config = ConfigManager.LoadSettings();

            if (string.IsNullOrEmpty(config.GamePaths["MGS3"]) || !Directory.Exists(config.GamePaths["MGS3"]))
            {
                MessageBox.Show("MGS3 game path is not configured or doesn't exist. Please update your settings.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            gameBasePath = Path.Combine(config.GamePaths["MGS3"], "us");
            bgmDir = Path.Combine(gameBasePath, "bgm");
            bgm2Dir = Path.Combine(gameBasePath, "bgm_2");

            if (!Directory.Exists(config.MGS3VanillaFolderPath))
            {
                MessageBox.Show("MGS3 Vanilla Files folder is not configured or doesn't exist. Please update your settings.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            InitializeDataGridView();

            checkboxMGS3.Checked = checkboxMG1.Checked = checkboxMG2.Checked = true;
        }

        private void AudioSwapperForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MGS3 form from the Audio Swapper form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MGS3ModdingForm form6 = new MGS3ModdingForm();
            form6.Show();
            this.Hide();
        }

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

            colReplacement.DefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            colReplacement.DefaultCellStyle.ForeColor = Color.Black;
            colReplacement.DefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.Columns.Add(colReplacement);

            DataGridViewTextBoxColumn colCurrent = new DataGridViewTextBoxColumn
            {
                HeaderText = "Replaced Song",
                Name = "colCurrentReplacement",
                ReadOnly = true,
                Width = 80
            };
            colCurrent.DefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            colCurrent.DefaultCellStyle.ForeColor = Color.Black;
            colCurrent.DefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
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
            colReset.DefaultCellStyle.BackColor = Color.FromArgb(156, 156, 124);
            colReset.DefaultCellStyle.ForeColor = Color.Black;
            colReset.DefaultCellStyle.Font = new Font(dataGridSdtTracks.Font, FontStyle.Bold);
            dataGridSdtTracks.Columns.Add(colReset);

            foreach (SongInfo song in AudioSdtManager.MGS3Songs)
            {
                string fullPath = Path.Combine(bgmDir, song.FileName);
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
            if (checkboxMGS3.Checked)
                songs.AddRange(AudioSdtManager.MGS3Songs);
            if (checkboxMG1.Checked)
                songs.AddRange(AudioSdtManager.MG1Songs);
            if (checkboxMG2.Checked)
                songs.AddRange(AudioSdtManager.MG2Songs);
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
                DataGridViewComboBoxCell cell = dataGridSdtTracks.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                if (cell != null)
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
                ComboBox combo = e.Control as ComboBox;
                if (combo != null)
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
            string vanillaPath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm", targetFileName);
            if (!File.Exists(vanillaPath))
            {
                vanillaPath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm_2", targetFileName);
            }
            if (File.Exists(vanillaPath))
            {
                string targetPath = Path.Combine(bgmDir, targetFileName);
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
                    string sourcePath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm", replacementFileName);

                    if (!File.Exists(sourcePath))
                    {
                        sourcePath = Path.Combine(config.MGS3VanillaFolderPath, "us", "bgm_2", replacementFileName);
                    }
                    if (File.Exists(sourcePath))
                    {
                        string targetPath = Path.Combine(bgmDir, targetFileName);
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
            string[] audioFolders = new string[] { "bgm", "bgm_2" };
            foreach (string folder in audioFolders)
            {
                string sourceDir = Path.Combine(config.MGS3VanillaFolderPath, "us", folder);
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

        private void checkboxMGS3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        private void checkboxMG1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        private void checkboxMG2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateComboBoxColumn();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("User is changing to the MGS3 form from the Audio Swapper form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS3ModdingForm");
            MGS3ModdingForm form6 = new MGS3ModdingForm();
            form6.Show();
            this.Hide();
        }
    }
}
