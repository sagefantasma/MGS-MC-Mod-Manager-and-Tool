using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public class ModListManager
    {
        public FlowLayoutPanel ModListPanel { get; private set; }
        public Color PanelBackgroundColor { get; set; }
        public int RightMargin { get; set; } = 50;
        public int TopMargin { get; set; } = 80;
        public int BottomMargin { get; set; } = 80;
        public int ButtonSpacing { get; set; } = 10;
        public int LeftMargin { get; set; } = 5;
        public int RightButtonMargin { get; set; } = 10;
        public int ModPanelHeight { get; set; } = 60;
        public string FontFamily { get; set; } = "Microsoft Sans Serif";
        public float FontSize { get; set; } = 10f;

        private Control parentControl;

        public ModListManager(Control parent, int formWidth, int formHeight, Color panelBackColor)
        {
            parentControl = parent;
            PanelBackgroundColor = panelBackColor;
            int panelWidth = (int)(formWidth / 1.5);
            int panelHeight = formHeight - TopMargin - BottomMargin;
            ModListPanel = new FlowLayoutPanel
            {
                AutoScroll = true,
                Size = new Size(panelWidth, panelHeight),
                Location = new Point(formWidth - panelWidth - RightMargin, TopMargin),
                BackColor = PanelBackgroundColor,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 10, 20, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            parentControl.Controls.Add(ModListPanel);
        }

        public ModListManager(FlowLayoutPanel existingPanel)
        {
            ModListPanel = existingPanel;
        }

        public void ClearMods() => ModListPanel.Controls.Clear();

        public class ModItem
        {
            public string ModName { get; set; }
            public string ModPath { get; set; }
            public bool IsEnabled { get; set; }
            public bool IsHDfix { get; set; }
        }

        public delegate void ModAction(string modName);

        private void AttachHoverEvents(Control control, string modName, Action<string, Control> onHoverEnter, Action onHoverLeave)
        {
            control.MouseEnter += (s, e) => onHoverEnter(modName, control);
            control.MouseLeave += (s, e) => onHoverLeave();
            foreach (Control child in control.Controls)
            {
                AttachHoverEvents(child, modName, onHoverEnter, onHoverLeave);
            }
        }

        public void LoadMods(
            List<ModItem> mods,
            ModAction onToggle,
            ModAction onRename,
            ModAction onDelete,
            ModAction onSettings,
            Action<string, Control> onHoverEnter,
            Action onHoverLeave)
        {
            ClearMods();
            int entryWidth = ModListPanel.Width - 20;
            foreach (var mod in mods)
            {
                Panel modPanel = CreateModPanel(mod, entryWidth, onToggle, onRename, onDelete, onSettings);
                AttachHoverEvents(modPanel, mod.ModName, onHoverEnter, onHoverLeave);
                ModListPanel.Controls.Add(modPanel);
            }
        }

        private Panel CreateModPanel(ModItem mod, int entryWidth, ModAction onToggle, ModAction onRename, ModAction onDelete, ModAction onSettings)
        {
            Panel modPanel = new Panel
            {
                Width = entryWidth,
                Height = ModPanelHeight,
                BackColor = PanelBackgroundColor,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 2, 0, 2)
            };

            Button toggleButton = new Button
            {
                Text = mod.IsEnabled ? "Installed" : "Not Installed",
                Size = new Size(150, 40),
                Tag = mod.ModName,
                Font = new Font(FontFamily, FontSize + 4, FontStyle.Bold),
                BackColor = mod.IsEnabled ? Color.Green : Color.Red,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            toggleButton.FlatAppearance.BorderSize = 0;
            toggleButton.Click += (s, e) => onToggle?.Invoke(mod.ModName);

            Label modLabel = new Label
            {
                Text = WrapText(mod.ModName, 50),
                AutoSize = false,
                Font = new Font(FontFamily, FontSize + 4, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(5, 0, 0, 0)
            };

            Button renameButton = new Button
            {
                Text = "Edit",
                Size = new Size(100, 40),
                Tag = mod.ModName,
                Font = new Font(FontFamily, FontSize + 4, FontStyle.Bold),
                BackColor = Color.LightBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            renameButton.FlatAppearance.BorderSize = 0;
            renameButton.Click += (s, e) => onRename?.Invoke(mod.ModName);

            Button deleteButton = new Button
            {
                Text = "Delete",
                Size = new Size(80, 40),
                Tag = mod.ModName,
                Font = new Font(FontFamily, FontSize + 4, FontStyle.Bold),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.Click += (s, e) => onDelete?.Invoke(mod.ModName);

            Button settingsButton = null;
            if (mod.IsHDfix && onSettings != null)
            {
                settingsButton = new Button
                {
                    Text = "HDFix Settings",
                    Size = new Size(160, 40),
                    Tag = mod.ModName,
                    Font = new Font(FontFamily, FontSize + 4, FontStyle.Bold),
                    BackColor = Color.LightBlue,
                    ForeColor = Color.Black,
                    FlatStyle = FlatStyle.Flat
                };
                settingsButton.FlatAppearance.BorderSize = 0;
                settingsButton.Click += (s, e) => onSettings?.Invoke(mod.ModName);
            }

            toggleButton.Location = new Point(LeftMargin, (modPanel.Height - toggleButton.Height) / 2);
            int currentRightX = modPanel.Width - RightButtonMargin;
            currentRightX -= deleteButton.Width;
            deleteButton.Location = new Point(currentRightX, (modPanel.Height - deleteButton.Height) / 2);
            currentRightX -= ButtonSpacing;
            currentRightX -= renameButton.Width;
            renameButton.Location = new Point(currentRightX, (modPanel.Height - renameButton.Height) / 2);
            if (settingsButton != null)
            {
                currentRightX -= ButtonSpacing;
                currentRightX -= settingsButton.Width;
                settingsButton.Location = new Point(currentRightX, (modPanel.Height - settingsButton.Height) / 2);
            }
            int labelX = toggleButton.Right + ButtonSpacing;
            int labelWidth = currentRightX - labelX - ButtonSpacing;
            modLabel.Location = new Point(labelX, 0);
            modLabel.Size = new Size(labelWidth, modPanel.Height);

            modPanel.Controls.Add(toggleButton);
            modPanel.Controls.Add(modLabel);
            modPanel.Controls.Add(renameButton);
            modPanel.Controls.Add(deleteButton);
            if (settingsButton != null)
                modPanel.Controls.Add(settingsButton);

            return modPanel;
        }

        public static string WrapText(string text, int maxChars)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            var words = text.Split(new char[] { ' ' }, StringSplitOptions.None);
            List<string> lines = new List<string>();
            string currentLine = "";
            foreach (var word in words)
            {
                if (string.IsNullOrEmpty(currentLine))
                {
                    if (word.Length > maxChars)
                    {
                        currentLine = word.Substring(0, maxChars);
                        lines.Add(currentLine);
                        currentLine = word.Substring(maxChars);
                    }
                    else
                    {
                        currentLine = word;
                    }
                }
                else if (currentLine.Length + 1 + word.Length <= maxChars)
                {
                    currentLine += " " + word;
                }
                else
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
            }
            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
            return string.Join(Environment.NewLine, lines);
        }
    }
}
