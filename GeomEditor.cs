using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    internal static class GeomEditor
    {

        private class MapRouteGroup
        {
            public string GroupName { get; }
            public List<List<int>> GuardIndexSets { get; }

            public MapRouteGroup(string groupName, params int[][] indexSets)
            {
                GroupName = groupName;
                GuardIndexSets = indexSets.Select(arr => arr.ToList()).ToList();
            }
        }

        private class MapDefinition
        {
            public string FileName { get; }
            public List<MapRouteGroup> Groups { get; }

            public MapDefinition(string fileName, List<MapRouteGroup> groups)
            {
                FileName = fileName;
                Groups = groups;
            }
        }

        private class GuardHeader
        {
            public uint RouteID;
            public uint PointCount;
            public uint DataOffset;
        }

        private class RoutePoint
        {
            public long FileOffset;
            public float X, Y, Z;
            public float AX, AY, AZ;
            public short Action, Move, Time, Dir;
        }

        private class GuardSet
        {
            public string GroupName;
            public int GuardNumberInGroup;
            public int[] Indices;
        }

        private static readonly List<MapDefinition> s_mapDefinitions = new List<MapDefinition>
        {
            new MapDefinition(
                "v004a.geom", // Dremuchji North - Virtuous Mission
                new List<MapRouteGroup>
                {
                    new("Reinforcements", [41], [42], [43]),
                    new("Very Easy / Easy",[5,6], [7,8], [9,10], [11,13]),
                    new("Normal",[14,15], [16,17], [18,19], [20,22]),
                    new("Hard",[23,24], [25,26], [27,28], [29,31]),
                    new("Extreme / E.Extreme",[32,33], [34,35], [36,37], [38,40])
                }
            ),
            new MapDefinition(
                "c004b.geom", // Dremuchji North - Snake Eater
                new List<MapRouteGroup>
                {
                    new("Running Guard Path", [2, 3], [4,5]),
                    new("Reinforcements", [38], [39], [40]),
                    new("Very Easy / Easy",[10,11], [12,13]),
                    new("Normal",[16/17], [18/19]),
                    new("Hard",[22,23], [24,25], [28,29]),
                    new("Extreme / E.Extreme",[30/31], [32/33], [36/37])
                }
            ),

            new MapDefinition(
                "c005a.geom", // Dolinovodno Rope Bridge - Virtuous Mission & Snake Eater
                new List<MapRouteGroup>
                {
                    new("Reinforcements", [9], [10]),
                    new("All Difficulties",[1,2], [3,4], [5,6], [7,8]),
                }
            ),

            new MapDefinition(
                "v006a.geom", // Rassvet - Virtuous Mission
                new List<MapRouteGroup>
                {
                    new("Reinforcements", [1], [2], [3], [4]),
                    new("Very Easy / Easy",[6,13], [7,14], [8,15], [9,16]),
                    new("Normal",[10,17], [11,18]),
                    new("Hard",[12,19]),
                    new("Extreme / E.Extreme",[27,34], [28,35], [29,36], [30,37], [31,38], [32,39], [33,40])
                }
            ),

            new MapDefinition(
                "s021a.geom", // Bolshaya Past South
                new List<MapRouteGroup>
                {
                    new("Reinforcements", [27], [28], [29]),
                    new("Guard Very Easy / Easy",[9,4], [5,7], [1,2]),
                    new("Guard Normal",[9,10], [11,12], [13,14], [15,16]),
                    new("Guard Hard/Extreme/E.Extreme", [17,18], [19,20], [21,22], [23,24], [25,26]),
                    new("Dogs Very Easy / Easy",[34]),
                    new("Dogs Normal",[34]),
                    new("Dog Hard",[32], [34]),
                    new("Dog Hard/Extreme/E.Extreme",[33]),
                    new("Dogs Extreme/E.Extreme",[31], [32], [33])
                }
            ),

            new MapDefinition(
                "s022a.geom", // Bolshaya Past Base
                new List<MapRouteGroup>
            {
                new("Reinforcements", [31], [32], [33]),
                new("Very Easy/Easy", [41]),
                new("Hard", [45]),
                new("Extreme/E.Extreme", [35], [40], [46]),
                new("Very Easy/Easy + Normal", [39], [43]),
                new("Normal + Hard", [48]),
                new("Hard + Extreme/E.Extreme", [37], [47]),
                new("Normal + Hard + Extreme/E.Extreme", [34]),
                new("Very Easy/Easy + Normal + Hard", [44]),
                new("Very Easy/Easy + Normal + Extreme/E.Extreme", [38]),
                new("Very Easy/Easy + Normal + Hard + Extreme/E.Extreme", [36])
            }
        ),
        };

        public static void EditGeomFileNextPrevGrouped(string geomFilePath)
        {
            if (!TryLoadAllGuardRoutes(geomFilePath,
                out List<GuardHeader> allHeaders,
                out Dictionary<uint, List<RoutePoint>> allGuardPoints))
            {
                MessageBox.Show("Failed to load guard routes.", "Error");
                return;
            }

            string fileName = Path.GetFileName(geomFilePath);
            var mapDef = s_mapDefinitions.FirstOrDefault(md =>
                md.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
            if (mapDef == null)
            {
                MessageBox.Show($"No grouped definition found for {fileName}.", "Info");
                return;
            }

            List<GuardSet> sets = new List<GuardSet>();
            foreach (var group in mapDef.Groups)
            {
                int guardCounter = 1;
                foreach (var indexSet in group.GuardIndexSets)
                {
                    if (indexSet.Count == 0) continue;
                    if (indexSet.Count > 2) continue;

                    sets.Add(new GuardSet
                    {
                        GroupName = group.GroupName,
                        GuardNumberInGroup = guardCounter,
                        Indices = indexSet.ToArray()
                    });
                    guardCounter++;
                }
            }

            if (sets.Count == 0)
            {
                MessageBox.Show("No guard sets found in the definition for this file.", "Info");
                return;
            }

            ShowNextPrevGroupedForm(geomFilePath, allHeaders, allGuardPoints, sets);
        }

        private static void ShowNextPrevGroupedForm(
            string geomFilePath,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints,
            List<GuardSet> sets)
        {
            bool userSavedAll = false;
            int currentIndex = 0;

            Form form = new Form
            {
                Text = "Grouped Editor (Next/Prev)",
                StartPosition = FormStartPosition.CenterScreen,
                Size = new Size(1200, 800),
                BackColor = Color.FromArgb(149, 149, 125)
            };

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                BackColor = Color.FromArgb(149, 149, 125)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            form.Controls.Add(mainLayout);

            Label lblTitle = new Label
            {
                Text = "",
                AutoSize = true,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(149, 149, 125),
                ForeColor = Color.Black
            };
            mainLayout.Controls.Add(lblTitle, 0, 0);

            Panel setPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(149, 149, 125)
            };
            mainLayout.Controls.Add(setPanel, 0, 1);

            FlowLayoutPanel bottomFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            mainLayout.Controls.Add(bottomFlow, 0, 2);

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Width = 100,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnCancel.Click += (s, e) => form.Close();
            bottomFlow.Controls.Add(btnCancel);

            Button btnSaveAll = new Button
            {
                Text = "Save All",
                Width = 100,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnSaveAll.Click += (s, e) =>
            {
                if (!ApplyChangesForSet(setPanel, allHeaders, allGuardPoints, sets[currentIndex]))
                    return;

                using (FileStream fs = File.OpenWrite(geomFilePath))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (var gh in allHeaders)
                    {
                        var rpList = allGuardPoints[gh.RouteID];
                        SaveUpdatedRoutePointsInternal(rpList, fs, bw);
                    }
                }
                userSavedAll = true;
                form.Close();
            };
            bottomFlow.Controls.Add(btnSaveAll);

            Button btnSetAllXYZ = new Button
            {
                Text = "Set All X/Y/Z",
                Width = 120,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnSetAllXYZ.Click += (s, e) =>
            {
                if (!ApplyChangesForSet(setPanel, allHeaders, allGuardPoints, sets[currentIndex]))
                    return;

                float[] snakePos = GetSnakePosition();
                if (snakePos == null)
                {
                    MessageBox.Show("Failed to get Snake's position.\nMGS3 may not be running.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                foreach (var gh in allHeaders)
                {
                    if (!allGuardPoints.ContainsKey(gh.RouteID)) continue;
                    foreach (var rp in allGuardPoints[gh.RouteID])
                    {
                        rp.X = snakePos[0];
                        rp.Y = snakePos[1];
                        rp.Z = snakePos[2];
                    }
                }
                RebuildSetPage(setPanel, lblTitle, sets, currentIndex, allHeaders, allGuardPoints);
            };
            bottomFlow.Controls.Add(btnSetAllXYZ);

            Button btnSetAllAXYZ = new Button
            {
                Text = "Set All A(X/Y/Z)",
                Width = 120,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnSetAllAXYZ.Click += (s, e) =>
            {
                if (!ApplyChangesForSet(setPanel, allHeaders, allGuardPoints, sets[currentIndex]))
                    return;

                float[] snakePos = GetSnakePosition();
                if (snakePos == null)
                {
                    MessageBox.Show("Failed to get Snake's position.\nMGS3 may not be running.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                foreach (var gh in allHeaders)
                {
                    if (!allGuardPoints.ContainsKey(gh.RouteID)) continue;
                    foreach (var rp in allGuardPoints[gh.RouteID])
                    {
                        rp.AX = snakePos[0];
                        rp.AY = snakePos[1];
                        rp.AZ = snakePos[2];
                    }
                }
                RebuildSetPage(setPanel, lblTitle, sets, currentIndex, allHeaders, allGuardPoints);
            };
            bottomFlow.Controls.Add(btnSetAllAXYZ);

            Button btnNext = new Button
            {
                Text = "Next >",
                Width = 80,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnNext.Click += (s, e) =>
            {
                if (currentIndex < sets.Count - 1)
                {
                    if (!ApplyChangesForSet(setPanel, allHeaders, allGuardPoints, sets[currentIndex]))
                        return;
                    currentIndex++;
                    RebuildSetPage(setPanel, lblTitle, sets, currentIndex, allHeaders, allGuardPoints);
                }
            };
            bottomFlow.Controls.Add(btnNext);

            Button btnPrev = new Button
            {
                Text = "< Prev",
                Width = 80,
                FlatStyle = FlatStyle.Popup,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125)
            };
            btnPrev.Click += (s, e) =>
            {
                if (currentIndex > 0)
                {
                    if (!ApplyChangesForSet(setPanel, allHeaders, allGuardPoints, sets[currentIndex]))
                        return;
                    currentIndex--;
                    RebuildSetPage(setPanel, lblTitle, sets, currentIndex, allHeaders, allGuardPoints);
                }
            };
            bottomFlow.Controls.Add(btnPrev);

            RebuildSetPage(setPanel, lblTitle, sets, currentIndex, allHeaders, allGuardPoints);
            form.ShowDialog();
        }

        private static void RebuildSetPage(
            Panel container,
            Label lblTitle,
            List<GuardSet> sets,
            int index,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints)
        {
            container.Controls.Clear();
            GuardSet set = sets[index];
            lblTitle.Text = $"{set.GroupName}\nGuard #{set.GuardNumberInGroup} | (Set {index + 1} / {sets.Count})";

            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(149, 149, 125)
            };
            container.Controls.Add(mainPanel);

            if (set.Indices.Length == 2)
            {
                Panel normalPanel = BuildSingleIndexPanel(set.Indices[0], "Normal", allHeaders, allGuardPoints);
                normalPanel.Location = new Point(0, 0);
                mainPanel.Controls.Add(normalPanel);

                Panel cautionPanel = BuildSingleIndexPanel(set.Indices[1], "Caution", allHeaders, allGuardPoints);
                cautionPanel.Location = new Point(0, normalPanel.Bottom + 10);
                mainPanel.Controls.Add(cautionPanel);
            }
            else
            {
                Panel singlePanel = BuildSingleIndexPanel(set.Indices[0], "(Single)", allHeaders, allGuardPoints);
                singlePanel.Location = new Point(0, 0);
                mainPanel.Controls.Add(singlePanel);
            }
        }

        private static Panel BuildSingleIndexPanel(
            int index1Based,
            string labelName,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints)
        {
            Panel outerPanel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(1100, 250),
                AutoScroll = true,
                BackColor = Color.FromArgb(149, 149, 125)
            };

            int actualIndex = index1Based - 1;
            if (actualIndex < 0 || actualIndex >= allHeaders.Count)
            {
                Label lblNoData = new Label
                {
                    Text = $"(No data for guard index={index1Based})",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(5, 5),
                    Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                    BackColor = Color.FromArgb(149, 149, 125)
                };
                outerPanel.Controls.Add(lblNoData);
                return outerPanel;
            }

            GuardHeader gh = allHeaders[actualIndex];
            string text = $"{labelName} (Index={index1Based}) => RouteID=0x{gh.RouteID:X8}, Points={gh.PointCount}";

            Label lblHeader = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5),
                BackColor = Color.FromArgb(149, 149, 125),
                ForeColor = Color.Black
            };
            outerPanel.Controls.Add(lblHeader);

            if (!allGuardPoints.ContainsKey(gh.RouteID) || gh.PointCount == 0)
            {
                Label lblNoPoints = new Label
                {
                    Text = "(No route points?)",
                    ForeColor = Color.Red,
                    AutoSize = true,
                    Location = new Point(5, lblHeader.Bottom + 10),
                    Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                    BackColor = Color.FromArgb(149, 149, 125)
                };
                outerPanel.Controls.Add(lblNoPoints);
                return outerPanel;
            }

            var routePoints = allGuardPoints[gh.RouteID];

            TableLayoutPanel tbl = new TableLayoutPanel
            {
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single,
                ColumnCount = 12,
                RowCount = routePoints.Count + 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Location = new Point(5, lblHeader.Bottom + 10),
                BackColor = Color.FromArgb(149, 149, 125)
            };

            for (int i = 0; i < tbl.RowCount; i++)
            {
                tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            for (int i = 0; i < tbl.ColumnCount; i++)
            {
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80f));
            }

            tbl.Controls.Add(HeaderLabel("Idx"), 0, 0);
            tbl.Controls.Add(HeaderLabel("X"), 1, 0);
            tbl.Controls.Add(HeaderLabel("Y"), 2, 0);
            tbl.Controls.Add(HeaderLabel("Z"), 3, 0);
            tbl.Controls.Add(HeaderLabel("Aim X"), 4, 0);
            tbl.Controls.Add(HeaderLabel("Aim Y"), 5, 0);
            tbl.Controls.Add(HeaderLabel("Aim Z"), 6, 0);
            tbl.Controls.Add(HeaderLabel("Set to Snake's XYZ"), 7, 0);
            tbl.Controls.Add(HeaderLabel("Action"), 8, 0);
            tbl.Controls.Add(HeaderLabel("Movement"), 9, 0);
            tbl.Controls.Add(HeaderLabel("Time"), 10, 0);
            tbl.Controls.Add(HeaderLabel("Direction"), 11, 0);

            for (int i = 0; i < routePoints.Count; i++)
            {
                RoutePoint rp = routePoints[i];
                int rowIdx = i + 1;

                tbl.Controls.Add(CreateCenterLabel(i.ToString()), 0, rowIdx);

                TextBox txtX = new TextBox
                {
                    Text = rp.X.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtX_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtX, 1, rowIdx);

                TextBox txtY = new TextBox
                {
                    Text = rp.Y.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtY_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtY, 2, rowIdx);

                TextBox txtZ = new TextBox
                {
                    Text = rp.Z.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtZ_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtZ, 3, rowIdx);

                TextBox txtAX = new TextBox
                {
                    Text = rp.AX.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtAX_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtAX, 4, rowIdx);

                TextBox txtAY = new TextBox
                {
                    Text = rp.AY.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtAY_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtAY, 5, rowIdx);

                TextBox txtAZ = new TextBox
                {
                    Text = rp.AZ.ToString("F6"),
                    Dock = DockStyle.Fill,
                    Name = $"txtAZ_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtAZ, 6, rowIdx);

                Button btnSetToSnake = new Button
                {
                    Text = "Set XYZ",
                    Dock = DockStyle.Fill,
                    FlatStyle = FlatStyle.Popup,
                    Font = new Font("Segoe UI", 8.25f, FontStyle.Bold),
                    BackColor = Color.FromArgb(149, 149, 125)
                };
                btnSetToSnake.Click += (sender, e) =>
                {
                    float[] snakePos = GetSnakePosition();
                    if (snakePos == null)
                    {
                        MessageBox.Show("Failed to get Snake's position.\nMGS3 may not be running.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    txtX.Text = snakePos[0].ToString("F6");
                    txtY.Text = snakePos[1].ToString("F6");
                    txtZ.Text = snakePos[2].ToString("F6");
                };
                tbl.Controls.Add(btnSetToSnake, 7, rowIdx);

                byte[] actBytes = BitConverter.GetBytes(rp.Action);
                TextBox txtAction = new TextBox
                {
                    Text = $"{actBytes[0]:X2} {actBytes[1]:X2}",
                    Dock = DockStyle.Fill,
                    Name = $"txtAction_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtAction, 8, rowIdx);

                byte[] movBytes = BitConverter.GetBytes(rp.Move);
                TextBox txtMove = new TextBox
                {
                    Text = $"{movBytes[0]:X2} {movBytes[1]:X2}",
                    Dock = DockStyle.Fill,
                    Name = $"txtMove_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtMove, 9, rowIdx);

                byte[] timeBytes = BitConverter.GetBytes(rp.Time);
                TextBox txtTime = new TextBox
                {
                    Text = $"{timeBytes[0]:X2} {timeBytes[1]:X2}",
                    Dock = DockStyle.Fill,
                    Name = $"txtTime_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtTime, 10, rowIdx);

                byte[] dirBytes = BitConverter.GetBytes(rp.Dir);
                TextBox txtDir = new TextBox
                {
                    Text = $"{dirBytes[0]:X2} {dirBytes[1]:X2}",
                    Dock = DockStyle.Fill,
                    Name = $"txtDir_{index1Based}_{i}"
                };
                tbl.Controls.Add(txtDir, 11, rowIdx);
            }

            outerPanel.Controls.Add(tbl);
            return outerPanel;
        }

        private static bool ApplyChangesForSet(
            Panel containerPanel,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints,
            GuardSet set)
        {
            foreach (Control ctl in containerPanel.Controls)
            {
                if (!ApplyAllChanges(ctl, allHeaders, allGuardPoints))
                    return false;
            }
            return true;
        }

        private static bool ApplyAllChanges(
            Control parent,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TableLayoutPanel tbl)
                {
                    if (!ApplyChangesFromOneTable(tbl, allHeaders, allGuardPoints))
                        return false;
                }
                if (c.HasChildren)
                {
                    if (!ApplyAllChanges(c, allHeaders, allGuardPoints))
                        return false;
                }
            }
            return true;
        }

        private static bool ApplyChangesFromOneTable(
            TableLayoutPanel table,
            List<GuardHeader> allHeaders,
            Dictionary<uint, List<RoutePoint>> allGuardPoints)
        {
            foreach (TextBox txt in table.Controls.OfType<TextBox>())
            {
                string[] parts = txt.Name.Split('_');
                if (parts.Length < 3) continue;
                string fieldType = parts[0];
                if (!int.TryParse(parts[1], out int guardIndex1Based)) continue;
                if (!int.TryParse(parts[2], out int ptIndex)) continue;
                int actualIndex = guardIndex1Based - 1;
                if (actualIndex < 0 || actualIndex >= allHeaders.Count) continue;
                GuardHeader gh = allHeaders[actualIndex];
                if (!allGuardPoints.ContainsKey(gh.RouteID)) continue;
                var routePoints = allGuardPoints[gh.RouteID];
                if (ptIndex < 0 || ptIndex >= routePoints.Count) continue;
                RoutePoint rp = routePoints[ptIndex];

                switch (fieldType)
                {
                    case "txtX":
                    case "txtY":
                    case "txtZ":
                    case "txtAX":
                    case "txtAY":
                    case "txtAZ":
                        if (!float.TryParse(txt.Text, out float fVal))
                        {
                            MessageBox.Show($"Invalid float in {txt.Name}.", "Error");
                            return false;
                        }
                        if (fieldType == "txtX") rp.X = fVal;
                        if (fieldType == "txtY") rp.Y = fVal;
                        if (fieldType == "txtZ") rp.Z = fVal;
                        if (fieldType == "txtAX") rp.AX = fVal;
                        if (fieldType == "txtAY") rp.AY = fVal;
                        if (fieldType == "txtAZ") rp.AZ = fVal;
                        break;

                    case "txtAction":
                    case "txtMove":
                    case "txtTime":
                    case "txtDir":
                        if (!TryParseShortHex(txt.Text, out short sVal))
                        {
                            MessageBox.Show($"Invalid short-hex in {txt.Name}.", "Error");
                            return false;
                        }
                        if (fieldType == "txtAction") rp.Action = sVal;
                        if (fieldType == "txtMove") rp.Move = sVal;
                        if (fieldType == "txtTime") rp.Time = sVal;
                        if (fieldType == "txtDir") rp.Dir = sVal;
                        break;
                }
            }
            return true;
        }

        private static void SaveUpdatedRoutePointsInternal(
            List<RoutePoint> routePoints,
            FileStream fs,
            BinaryWriter bw)
        {
            foreach (var rp in routePoints)
            {
                fs.Seek(rp.FileOffset, SeekOrigin.Begin);
                bw.BaseStream.Seek(8, SeekOrigin.Current);
                bw.Write(rp.X);
                bw.Write(rp.Y);
                bw.Write(rp.Z);
                bw.Write(rp.AX);
                bw.Write(rp.AY);
                bw.Write(rp.AZ);
                bw.Write(BitConverter.GetBytes(rp.Action));
                bw.Write(BitConverter.GetBytes(rp.Move));
                bw.Write(BitConverter.GetBytes(rp.Time));
                bw.Write(BitConverter.GetBytes(rp.Dir));
                bw.BaseStream.Seek(8, SeekOrigin.Current);
            }
        }

        private static bool TryParseShortHex(string hexString, out short value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(hexString)) return false;
            string[] parts = hexString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2) return false;
            if (!byte.TryParse(parts[0], NumberStyles.HexNumber, null, out byte b0) ||
                !byte.TryParse(parts[1], NumberStyles.HexNumber, null, out byte b1))
            {
                return false;
            }
            value = BitConverter.ToInt16(new byte[] { b0, b1 }, 0);
            return true;
        }

        private static uint ReadUIntBigEndian(byte[] data, int offset)
        {
            byte[] temp = new byte[4];
            Array.Copy(data, offset, temp, 0, 4);
            Array.Reverse(temp);
            return BitConverter.ToUInt32(temp, 0);
        }

        private static bool TryLoadAllGuardRoutes(
            string geomFilePath,
            out List<GuardHeader> guardHeaders,
            out Dictionary<uint, List<RoutePoint>> allGuardPoints)
        {
            guardHeaders = new List<GuardHeader>();
            allGuardPoints = new Dictionary<uint, List<RoutePoint>>();

            try
            {
                using (FileStream fs = File.OpenRead(geomFilePath))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] rawHeader = br.ReadBytes(112);
                    if (rawHeader.Length < 112) return false;
                    uint chunkCount = BitConverter.ToUInt32(rawHeader, 0x08);
                    if (chunkCount > 8) return false;
                    int chunkEntryStart = 0x20;
                    int chunkEntrySize = 16;
                    uint routesOffset = 0;
                    for (int i = 0; i < chunkCount; i++)
                    {
                        int off = chunkEntryStart + (i * chunkEntrySize);
                        if (off + 16 > rawHeader.Length) break;
                        ushort chunkType = BitConverter.ToUInt16(rawHeader, off);
                        uint chunkOffsetVal = BitConverter.ToUInt32(rawHeader, off + 8);
                        if (chunkType == 0x0007)
                            routesOffset = chunkOffsetVal;
                    }
                    if (routesOffset == 0) return false;
                    fs.Seek(routesOffset, SeekOrigin.Begin);
                    byte[] terminator = new byte[]
                    {
                        0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x80,
                        0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00,
                        0x00,0x00,0x00,0x00, 0x00,0x00,0x00,0x00
                    };
                    while (true)
                    {
                        if (fs.Position + 32 > fs.Length) break;
                        byte[] routeEntry = br.ReadBytes(32);
                        if (routeEntry.Length < 32) break;
                        if (routeEntry.SequenceEqual(terminator)) break;
                        uint routeID = ReadUIntBigEndian(routeEntry, 0);
                        uint pointCount = BitConverter.ToUInt32(routeEntry, 8);
                        uint dataOffset = BitConverter.ToUInt32(routeEntry, 16);
                        if (routeID != 0 || pointCount != 0)
                        {
                            guardHeaders.Add(new GuardHeader
                            {
                                RouteID = routeID,
                                PointCount = pointCount,
                                DataOffset = dataOffset
                            });
                        }
                    }
                }
                if (guardHeaders.Count == 0) return false;
                foreach (var gh in guardHeaders)
                {
                    var points = LoadRoutePoints(geomFilePath, gh);
                    allGuardPoints[gh.RouteID] = points;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static List<RoutePoint> LoadRoutePoints(string geomFilePath, GuardHeader gh)
        {
            List<RoutePoint> points = new List<RoutePoint>();
            if (gh.PointCount == 0) return points;
            using (FileStream fs = File.OpenRead(geomFilePath))
            using (BinaryReader br = new BinaryReader(fs))
            {
                fs.Seek(gh.DataOffset, SeekOrigin.Begin);
                for (int i = 0; i < gh.PointCount; i++)
                {
                    long fileOffset = fs.Position;
                    byte[] routePointBytes = br.ReadBytes(48);
                    if (routePointBytes.Length < 48) break;
                    float x = BitConverter.ToSingle(routePointBytes, 0x08);
                    float y = BitConverter.ToSingle(routePointBytes, 0x0C);
                    float z = BitConverter.ToSingle(routePointBytes, 0x10);
                    float ax = BitConverter.ToSingle(routePointBytes, 0x14);
                    float ay = BitConverter.ToSingle(routePointBytes, 0x18);
                    float az = BitConverter.ToSingle(routePointBytes, 0x1C);
                    short actionVal = BitConverter.ToInt16(routePointBytes, 0x20);
                    short moveVal = BitConverter.ToInt16(routePointBytes, 0x22);
                    short timeVal = BitConverter.ToInt16(routePointBytes, 0x24);
                    short dirVal = BitConverter.ToInt16(routePointBytes, 0x26);
                    points.Add(new RoutePoint
                    {
                        FileOffset = fileOffset,
                        X = x,
                        Y = y,
                        Z = z,
                        AX = ax,
                        AY = ay,
                        AZ = az,
                        Action = actionVal,
                        Move = moveVal,
                        Time = timeVal,
                        Dir = dirVal
                    });
                }
            }
            return points;
        }

        public static bool HasGroupedDefinition(string geomFilePath)
        {
            string fileName = Path.GetFileName(geomFilePath);
            return s_mapDefinitions.Any(md =>
                md.FileName.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        private static Label HeaderLabel(string text)
        {
            return new Label
            {
                Text = text,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(149, 149, 125),
                ForeColor = Color.Black
            };
        }

        private static Label CreateCenterLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Bold),
                BackColor = Color.FromArgb(149, 149, 125),
                ForeColor = Color.Black
            };
        }

        private static float[] GetSnakePosition()
        {
            IntPtr processHandle = MGS3MemoryManager.OpenGameProcess(MGS3MemoryManager.GetMGS3Process());
            if (processHandle != IntPtr.Zero)
            {
                try
                {
                    return MGS3MemoryManager.Instance.ReadSnakePosition(processHandle);
                }
                finally
                {
                    MGS3MemoryManager.NativeMethods.CloseHandle(processHandle);
                }
            }
            return null;
        }
    }
}
