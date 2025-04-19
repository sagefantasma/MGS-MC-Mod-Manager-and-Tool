using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class MGSHDFixSettingsForm : Form
    {
        private ConfigSettings config;
        private Dictionary<string, List<IniEntry>> iniData;
        private string modFolder;
        private string modIniPath;
        private string modAsiPath;
        private string modReadmePath;
        private string modUltimateASILicensePath;
        private string modWinhttpDllPath;
        private string modWininetDllPath;
        private string gameDir;
        private string targetGame;

        public class IniEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        private Panel modListPanel;
        private TableLayoutPanel settingsTable;

        public MGSHDFixSettingsForm(string targetGame, Form parent = null)
        {
            InitializeComponent();
            this.targetGame = targetGame;
            if (parent != null)
            {
                this.Owner = parent;
                this.BackColor = parent.BackColor;
                this.ForeColor = parent.ForeColor;
                this.Font = parent.Font;
            }
            this.Load += MGSHDFixSettingsForm_Load;
        }

        private async void MGSHDFixSettingsForm_Load(object sender, EventArgs e)
        {
            this.Location = GuiManager.GetLastFormLocation();
            config = ConfigManager.LoadSettings();

            string baseModFolder = "";
            switch (targetGame)
            {
                case "MG1andMG2":
                    baseModFolder = config.MG1andMG2ModFolderPath;
                    gameDir = config.GamePaths.ContainsKey("MG1andMG2") ? config.GamePaths["MG1andMG2"] : "";
                    break;
                case "MGS2":
                    baseModFolder = config.MGS2ModFolderPath;
                    gameDir = config.GamePaths.ContainsKey("MGS2") ? config.GamePaths["MGS2"] : "";
                    break;
                case "MGS3":
                    baseModFolder = config.MGS3ModFolderPath;
                    gameDir = config.GamePaths.ContainsKey("MGS3") ? config.GamePaths["MGS3"] : "";
                    break;
                default:
                    MessageBox.Show("Invalid game specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
            }

            if (!Directory.Exists(baseModFolder))
            {
                Directory.CreateDirectory(baseModFolder);
                LoggingManager.Instance.Log($"Created base mod folder: {baseModFolder}");
            }

            modFolder = Path.Combine(baseModFolder, "MGSHDFix");
            if (!Directory.Exists(modFolder))
            {
                Directory.CreateDirectory(modFolder);
                LoggingManager.Instance.Log($"Created MGSHDFix mod folder: {modFolder}");
            }

            modIniPath = Path.Combine(modFolder, "MGSHDFix.ini");
            modAsiPath = Path.Combine(modFolder, "MGSHDFix.asi");
            modReadmePath = Path.Combine(modFolder, "README.md");
            modUltimateASILicensePath = Path.Combine(modFolder, "UltimateASILoader_LICENSE.md");
            modWinhttpDllPath = Path.Combine(modFolder, "winhttp.dll");
            modWininetDllPath = Path.Combine(modFolder, "wininet.dll");

            DownloadManager dm = new DownloadManager();
            if (!File.Exists(modIniPath))
            {
                await dm.EnsureMGSHDFixDownloaded(modFolder);
            }

            if (string.IsNullOrEmpty(gameDir) || !Directory.Exists(gameDir))
            {
                switch (targetGame)
                {
                    case "MG1andMG2":
                        gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\MG1andMG2";
                        break;
                    case "MGS2":
                        gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\MGS2";
                        break;
                    case "MGS3":
                        gameDir = @"C:\Program Files (x86)\Steam\steamapps\common\MGS3";
                        break;
                }
                if (!Directory.Exists(gameDir))
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        Description = "Select your game installation folder."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            gameDir = fbd.SelectedPath;
                        }
                        else
                        {
                            MessageBox.Show("Game directory not selected. Cannot proceed.",
                                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
                config.GamePaths[targetGame] = gameDir;
                ConfigManager.SaveSettings(config);
            }

            SyncFilesToGameDirectory();
            LoadIni(modIniPath);
            BuildSettingsUI();
        }

        private void SyncFilesToGameDirectory()
        {
            if (string.IsNullOrEmpty(gameDir))
                return;

            try
            {
                File.Copy(modIniPath, Path.Combine(gameDir, "MGSHDFix.ini"), true);
                File.Copy(modAsiPath, Path.Combine(gameDir, "MGSHDFix.asi"), true);
                File.Copy(modReadmePath, Path.Combine(gameDir, "README.md"), true);
                File.Copy(modUltimateASILicensePath, Path.Combine(gameDir, "UltimateASILoader_LICENSE.md"), true);
                File.Copy(modWinhttpDllPath, Path.Combine(gameDir, "winhttp.dll"), true);
                File.Copy(modWininetDllPath, Path.Combine(gameDir, "wininet.dll"), true);
            }
            catch (Exception ex)
            {
                LoggingManager.Instance.Log($"Error syncing files to game directory: {ex.Message}");
            }
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
                int left = (int)(this.ClientSize.Width * 0.01);
                int top = (int)(this.ClientSize.Height * 0.03);
                int width = (int)(this.ClientSize.Width * 0.98);
                int height = (int)(this.ClientSize.Height * 0.92);

                modListPanel = new Panel
                {
                    Location = new Point(left, top),
                    Size = new Size(width, height),
                    BackColor = this.BackColor,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
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
            else if (key.Equals("CtrlType", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox cb = new ComboBox { Tag = new Tuple<string, string>(section, key) };
                cb.Items.AddRange(new string[] { "PS5", "PS4", "XBOX", "NX", "STMD", "KBD" });
                cb.SelectedItem = currentValue;
                return cb;
            }
            else if (key.Equals("Language", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox cb = new ComboBox { Tag = new Tuple<string, string>(section, key) };
                cb.Items.AddRange(new string[] { "EN", "JP", "FR", "GR", "IT", "PR", "SP", "DU", "RU" });
                cb.SelectedItem = currentValue;
                return cb;
            }
            else if (key.Equals("Region", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox cb = new ComboBox { Tag = new Tuple<string, string>(section, key) };
                cb.Items.AddRange(new string[] { "US", "JP", "EU" });
                cb.SelectedItem = currentValue;
                return cb;
            }
            else if (key.Equals("MSXGame", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox cb = new ComboBox { Tag = new Tuple<string, string>(section, key) };
                cb.Items.AddRange(new string[] { "MG1", "MG2" });
                cb.SelectedItem = currentValue;
                return cb;
            }
            else if (key.Equals("MSXWallAlign", StringComparison.OrdinalIgnoreCase))
            {
                ComboBox cb = new ComboBox { Tag = new Tuple<string, string>(section, key) };
                cb.Items.AddRange(new string[] { "L", "R", "C" });
                cb.SelectedItem = currentValue;
                return cb;
            }
            else if (key.Equals("MSXWallType", StringComparison.OrdinalIgnoreCase))
            {
                Panel p = new Panel { Height = 40 };
                TrackBar tb = new TrackBar
                {
                    Minimum = 0,
                    Maximum = 6,
                    TickStyle = TickStyle.None,
                    Dock = DockStyle.Fill,
                    Tag = new Tuple<string, string>(section, key)
                };
                if (int.TryParse(currentValue, out int val))
                    tb.Value = val;
                Label lbl = new Label
                {
                    Text = tb.Value.ToString(),
                    Width = 40,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Right,
                    Tag = new Tuple<string, string>(section, key)
                };
                tb.Scroll += (s, e) => { lbl.Text = tb.Value.ToString(); };
                p.Controls.Add(tb);
                p.Controls.Add(lbl);
                return p;
            }
            else if (key.Equals("Samples", StringComparison.OrdinalIgnoreCase))
            {
                Panel p = new Panel { Height = 40 };
                TrackBar tb = new TrackBar
                {
                    Minimum = 0,
                    Maximum = 16,
                    TickStyle = TickStyle.None,
                    Dock = DockStyle.Fill,
                    Tag = new Tuple<string, string>(section, key)
                };
                if (int.TryParse(currentValue, out int val))
                    tb.Value = val;
                Label lbl = new Label
                {
                    Text = tb.Value.ToString(),
                    Width = 40,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Right,
                    Tag = new Tuple<string, string>(section, key)
                };
                tb.Scroll += (s, e) => { lbl.Text = tb.Value.ToString(); };
                p.Controls.Add(tb);
                p.Controls.Add(lbl);
                return p;
            }
            else if (key.Equals("Width", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("Height", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("SizeMB", StringComparison.OrdinalIgnoreCase))
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
            else if (key.Equals("X Multiplier", StringComparison.OrdinalIgnoreCase) ||
                     key.Equals("Y Multiplier", StringComparison.OrdinalIgnoreCase))
            {
                NumericUpDown nud = new NumericUpDown
                {
                    Minimum = 0,
                    Maximum = 100,
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
                        value = txt.Text;
                    else if (ctrl is Button btn)
                        value = btn.Text.ToLower();
                    else if (ctrl is NumericUpDown nud)
                        value = nud.Value.ToString();
                    else if (ctrl is ComboBox cb)
                        value = cb.SelectedItem?.ToString() ?? "";
                    else if (ctrl is Panel p)
                    {
                        TrackBar tb = p.Controls.OfType<TrackBar>().FirstOrDefault();
                        if (tb != null)
                            value = tb.Value.ToString();
                    }

                    var entry = iniData[section].FirstOrDefault(ei => ei.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                    if (entry != null)
                        entry.Value = value;
                }
            }

            using (StreamWriter writer = new StreamWriter(modIniPath))
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
            SyncFilesToGameDirectory();

            MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}