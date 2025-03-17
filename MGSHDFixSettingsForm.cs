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
        private string iniPath;
        // Stores the INI data as: section -> list of IniEntry (each holds key and value)
        private Dictionary<string, List<IniEntry>> iniData;

        // Represents an INI entry.
        public class IniEntry
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        // Constructor accepting just the INI file path.
        public MGSHDFixSettingsForm(string iniPath)
            : this(iniPath, null)
        {
        }

        // Constructor accepting an INI file path and a parent form for theme inheritance.
        public MGSHDFixSettingsForm(string iniPath, Form parent)
        {
            InitializeComponent();
            this.iniPath = iniPath;
            this.Text = "MGSHDFix Settings";
            this.Width = 500;
            this.Height = 600;
            if (parent != null)
            {
                this.Owner = parent;
                this.BackColor = parent.BackColor;
                this.ForeColor = parent.ForeColor;
                this.Font = parent.Font;
            }
            LoadIni();
            BuildUI();
        }

        // Loads the INI file (ignores original comments) into iniData.
        private void LoadIni()
        {
            iniData = new Dictionary<string, List<IniEntry>>(StringComparer.OrdinalIgnoreCase);
            string[] lines = File.ReadAllLines(iniPath);
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
                // Skip comments starting with ;
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

        // Builds the UI using a TableLayoutPanel with two columns.
        private void BuildUI()
        {
            this.Controls.Clear();

            Panel scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            TableLayoutPanel table = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2
            };
            // Left column: fixed width (150 pixels); right column fills remaining width.
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Iterate over each section.
            foreach (var section in iniData)
            {
                // Add section header spanning both columns.
                Label sectionHeader = new Label
                {
                    Text = section.Key,
                    Font = new Font(this.Font, FontStyle.Bold),
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    Padding = new Padding(0, 10, 0, 5)
                };
                table.RowCount++;
                table.Controls.Add(sectionHeader, 0, table.RowCount - 1);
                table.SetColumnSpan(sectionHeader, 2);

                // Add each key/value pair.
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

                    table.RowCount++;
                    table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    table.Controls.Add(keyLabel, 0, table.RowCount - 1);
                    table.Controls.Add(ctrl, 1, table.RowCount - 1);
                }
            }

            Button saveButton = new Button
            {
                Text = "Save",
                Dock = DockStyle.Bottom,
                Height = 40
            };
            saveButton.Click += SaveButton_Click;

            scrollPanel.Controls.Add(table);
            this.Controls.Add(scrollPanel);
            this.Controls.Add(saveButton);
        }

        // Creates a control based on the key.
        private Control CreateControlForKey(string key, string currentValue, string section)
        {
            // For boolean values: use a toggle button.
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
            // For fixed options: use ComboBox.
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
            // For MSXWallType: use a TrackBar (0–6) with a label.
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
            // For Samples: use a TrackBar (0–16) with a label.
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
            // For restricted numeric values (Width, Height, SizeMB).
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
            // For multipliers.
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

        // Save updated settings back to the INI file.
        private void SaveButton_Click(object sender, EventArgs e)
        {
            Panel scrollPanel = this.Controls.OfType<Panel>().FirstOrDefault();
            if (scrollPanel != null)
            {
                TableLayoutPanel table = scrollPanel.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                if (table != null)
                {
                    foreach (Control ctrl in table.Controls)
                    {
                        if (ctrl.Tag is Tuple<string, string> tag)
                        {
                            string section = tag.Item1;
                            string key = tag.Item2;
                            string value = "";
                            if (ctrl is TextBox txt)
                                value = txt.Text;
                            else if (ctrl is Button btn)
                            {
                                // For boolean toggle buttons, ensure lower-case.
                                if (btn.Text.Equals("True", StringComparison.OrdinalIgnoreCase) ||
                                    btn.Text.Equals("False", StringComparison.OrdinalIgnoreCase))
                                    value = btn.Text.ToLowerInvariant();
                                else
                                    value = btn.Text;
                            }
                            else if (ctrl is ComboBox cb)
                                value = cb.SelectedItem?.ToString() ?? "";
                            else if (ctrl is NumericUpDown nud)
                                value = nud.Value.ToString();
                            else if (ctrl is Panel p)
                            {
                                TrackBar tb = p.Controls.OfType<TrackBar>().FirstOrDefault();
                                if (tb != null)
                                    value = tb.Value.ToString();
                            }
                            var entry = iniData[section].FirstOrDefault(item => item.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
                            if (entry != null)
                                entry.Value = value;
                        }
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
            MessageBox.Show("Settings saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
