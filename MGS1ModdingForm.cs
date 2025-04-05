using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MGS1ModdingForm : Form
    {
        private ConfigSettings config;
        private Dictionary<string, List<IniEntry>> iniData;
        private string iniPath;
        private string gameIniPath; 

        public class IniEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private Panel modListPanel;
        private TableLayoutPanel settingsTable;

        public MGS1ModdingForm()
        {
            InitializeComponent();
            this.Load += MGS1ModdingForm_Load;
            this.FormClosing += MGS1ModdingForm_FormClosing;
        }

        private async void MGS1ModdingForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
            this.BackColor = ColorTranslator.FromHtml("#52a472");
            this.Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
            this.ForeColor = Color.Black;

            config = ConfigManager.LoadSettings();

            if (!config.MGS1ModFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Please choose a location for the MGS1 Mods folder.\n\n" +
                    "Click 'Yes' to use the default location or 'No' to select your own.",
                    "MGS1 Mods Folder Location", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (res == DialogResult.Cancel)
                {
                    new MainMenuForm().Show();
                    this.Close();
                    return;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.MGS1ModFolderPath,
                        Description = "Select a folder where the 'MGS1 Mods' folder will be created."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.MGS1ModFolderPath = Path.Combine(fbd.SelectedPath, "MGS1 Mods");
                        }
                        else
                        {
                            new MainMenuForm().Show();
                            this.Close();
                            return;
                        }
                    }
                }
                config.MGS1ModFolderSet = true;
                ConfigManager.SaveSettings(config);
            }

            string modFolder = config.MGS1ModFolderPath;
            if (!Directory.Exists(modFolder))
            {
                Directory.CreateDirectory(modFolder);
                LoggingManager.Instance.Log($"Created MGS1 Mods folder: {modFolder}");
            }

            iniPath = Path.Combine(modFolder, "MGSM2Fix.ini");

            DownloadManager dm = new DownloadManager();
            if (!File.Exists(iniPath))
            {
                await dm.EnsureMGSM2FixDownloaded(modFolder);
            }

            string gameDir = config.GamePaths.ContainsKey("MGS1") ? config.GamePaths["MGS1"] : "";
            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir))
            {
                gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\MGS1";
                if (!Directory.Exists(gameDir))
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        Description = "Select your MGS1 installation folder."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            gameDir = fbd.SelectedPath;
                        }
                        else
                        {
                            MessageBox.Show("MGS1 game directory not selected. Cannot proceed.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                config.GamePaths["MGS1"] = gameDir;
                ConfigManager.SaveSettings(config);
            }

            gameIniPath = Path.Combine(gameDir, "MGSM2Fix.ini");

            if (!File.Exists(gameIniPath))
            {
                File.Copy(iniPath, gameIniPath, false);
            }

            LoadIni(iniPath);
            BuildSettingsUI();
        }

        private void LoadIni(string filePath)
        {
            iniData = new Dictionary<string, List<IniEntry>>(StringComparer.OrdinalIgnoreCase);
            string[] lines = File.ReadAllLines(filePath);
            string currentSection = "General";

            foreach (string line in lines)
            {
                string trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed))
                    continue;
                if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                {
                    currentSection = trimmed.Substring(1, trimmed.Length - 2).Trim();
                    if (!iniData.ContainsKey(currentSection))
                        iniData[currentSection] = new List<IniEntry>();
                    continue;
                }
                if (trimmed.StartsWith(";"))
                    continue;

                int idx = trimmed.IndexOf('=');
                if (idx > 0)
                {
                    string key = trimmed.Substring(0, idx).Trim();
                    string value = trimmed.Substring(idx + 1).Trim();
                    if (!iniData.ContainsKey(currentSection))
                        iniData[currentSection] = new List<IniEntry>();
                    iniData[currentSection].Add(new IniEntry { Key = key, Value = value });
                }
            }
        }

        private void BuildSettingsUI()
        {
            if (modListPanel == null)
            {
                modListPanel = new Panel
                {
                    Location = new Point(224, 16),
                    Size = new Size(739, 575),
                    BackColor = ColorTranslator.FromHtml("#52a472"),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left
                };
                this.Controls.Add(modListPanel);
            }
            modListPanel.Controls.Clear();

            Panel containerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = this.BackColor
            };

            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = this.BackColor
            };

            settingsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                BackColor = this.BackColor
            };
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            settingsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            foreach (var section in iniData)
            {
                Label sectionHeader = new Label
                {
                    Text = section.Key,
                    Font = new Font(this.Font, FontStyle.Bold),
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(0, 10, 0, 5)
                };
                settingsTable.RowCount++;
                settingsTable.Controls.Add(sectionHeader, 0, settingsTable.RowCount - 1);
                settingsTable.SetColumnSpan(sectionHeader, 2);

                foreach (var entry in section.Value)
                {
                    Label keyLabel = new Label
                    {
                        Text = entry.Key,
                        Font = new Font(this.Font, FontStyle.Bold),
                        AutoSize = false,
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    Control ctrl = CreateControlForKey(entry.Key, entry.Value, section.Key);
                    ctrl.Dock = DockStyle.Fill;

                    settingsTable.RowCount++;
                    settingsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    settingsTable.Controls.Add(keyLabel, 0, settingsTable.RowCount - 1);
                    settingsTable.Controls.Add(ctrl, 1, settingsTable.RowCount - 1);
                }
            }

            scrollPanel.Controls.Add(settingsTable);
            containerPanel.Controls.Add(scrollPanel);

            Button saveButton = new Button
            {
                Text = "Save",
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = this.Font,
                BackColor = Color.LightBlue,
                ForeColor = Color.Black
            };
            saveButton.Click += SaveButton_Click;
            containerPanel.Controls.Add(saveButton);

            modListPanel.Controls.Add(containerPanel);
        }

        private Control CreateControlForKey(string key, string currentValue, string section)
        {
            if (currentValue.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                currentValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                Button btn = new Button
                {
                    Text = currentValue.Equals("true", StringComparison.OrdinalIgnoreCase) ? "true" : "false",
                    BackColor = currentValue.Equals("true", StringComparison.OrdinalIgnoreCase) ? Color.Green : Color.Red,
                    ForeColor = Color.White,
                    Tag = new Tuple<string, string>(section, key)
                };
                btn.Click += (s, e) =>
                {
                    Button b = s as Button;
                    if (b.Text.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        b.Text = "false";
                        b.BackColor = Color.Red;
                    }
                    else
                    {
                        b.Text = "true";
                        b.BackColor = Color.Green;
                    }
                };
                return btn;
            }
            else if (key.Equals("Width", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("Height", StringComparison.OrdinalIgnoreCase))
            {
                NumericUpDown nud = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 10000,
                    Tag = new Tuple<string, string>(section, key)
                };
                if (int.TryParse(currentValue, out int num))
                    nud.Value = num;
                return nud;
            }
            else
            {
                TextBox txt = new TextBox
                {
                    Text = currentValue,
                    Tag = new Tuple<string, string>(section, key)
                };
                return txt;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (Control ctrl in settingsTable.Controls)
            {
                if (ctrl.Tag is Tuple<string, string> tag)
                {
                    string section = tag.Item1;
                    string key = tag.Item2;
                    string value = "";

                    if (ctrl is TextBox txt)
                    {
                        value = txt.Text;
                    }
                    else if (ctrl is Button btn)
                    {
                        value = btn.Text.ToLower();
                    }
                    else if (ctrl is NumericUpDown nud)
                    {
                        value = nud.Value.ToString();
                    }
                    else if (ctrl is ComboBox cb)
                    {
                        value = cb.SelectedItem?.ToString() ?? "";
                    }
                    else if (ctrl is Panel p)
                    {
                        TrackBar tb = p.Controls.OfType<TrackBar>().FirstOrDefault();
                        if (tb != null)
                            value = tb.Value.ToString();
                    }

                    var entry = iniData[section].FirstOrDefault(ei => ei.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (entry != null)
                    {
                        entry.Value = value;
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(iniPath))
            {
                foreach (var section in iniData)
                {
                    writer.WriteLine("[" + section.Key + "]");
                    foreach (var entry in section.Value)
                    {
                        writer.WriteLine($"{entry.Key} = {entry.Value}");
                    }
                    writer.WriteLine();
                }
            }

            if (!string.IsNullOrEmpty(gameIniPath))
            {
                File.Copy(iniPath, gameIniPath, true);
            }

            MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MGS1ModdingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("MGS1ModdingForm closing.");
            Application.Exit();
        }

        private void MoveMGS1ModFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog
            {
                SelectedPath = config.MGS1ModFolderPath,
                Description = "Select a new location for the MGS1 Mods folder."
            })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string newFolderPath = Path.Combine(fbd.SelectedPath, "MGS1 Mods");
                    try
                    {
                        Directory.Move(config.MGS1ModFolderPath, newFolderPath);
                        config.MGS1ModFolderPath = newFolderPath;
                        ConfigManager.SaveSettings(config);
                        MessageBox.Show("MGS1 Mods folder moved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error moving MGS1 Mods folder:\n" + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Main Menu from MGS1.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "MGS1ModdingForm");
            new MainMenuForm().Show();
            this.Hide();
        }
    }
}
