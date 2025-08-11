using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class HzxEditor
    {
        public class HZXMap
        {
            public ushort Version;
            public ushort NGroups;
            public ushort NPatrols;
            public ushort NClears;
            public byte[] Pad;
            public uint PatrolsPtr;
            public uint PointsPtr;
            public uint ClearsPtr;
            public uint CPatrolsPtr;
            public uint CPointsPtr;
            public uint GroupsPtr;
            public byte[] ExtraReserved;
            public const int HeaderSize = 96;

            public static HZXMap Load(string filePath)
            {
                using (FileStream fs = File.OpenRead(filePath))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    byte[] header = br.ReadBytes(HeaderSize);
                    if (header.Length < HeaderSize)
                        throw new Exception("File too small for HZX header.");

                    HZXMap map = new HZXMap();
                    map.Version = BitConverter.ToUInt16(header, 0);
                    map.NGroups = BitConverter.ToUInt16(header, 2);
                    map.NPatrols = BitConverter.ToUInt16(header, 4);
                    map.NClears = BitConverter.ToUInt16(header, 6);
                    map.Pad = new byte[40];
                    Array.Copy(header, 8, map.Pad, 0, 40);
                    map.PatrolsPtr = BitConverter.ToUInt32(header, 48);
                    map.PointsPtr = BitConverter.ToUInt32(header, 52);
                    map.ClearsPtr = BitConverter.ToUInt32(header, 56);
                    map.CPatrolsPtr = BitConverter.ToUInt32(header, 60);
                    map.CPointsPtr = BitConverter.ToUInt32(header, 64);
                    map.GroupsPtr = BitConverter.ToUInt32(header, 68);
                    map.ExtraReserved = new byte[24];
                    Array.Copy(header, 72, map.ExtraReserved, 0, 24);

                    return map;
                }
            }
        }

        

        public class HZXPat
        {
            public ushort NPoints;
            public ushort InitPoint;
            public uint PointsPtr;
            public long FileOffset;

            public static HZXPat FromBytes(byte[] data, long fileOffset)
            {
                if (data.Length < 16)
                    throw new Exception("Not enough data for HZX_PAT.");

                HZXPat pat = new HZXPat();
                pat.NPoints = BitConverter.ToUInt16(data, 0);
                pat.InitPoint = BitConverter.ToUInt16(data, 2);
                pat.PointsPtr = BitConverter.ToUInt32(data, 8);
                pat.FileOffset = fileOffset;
                return pat;
            }
        }

        public class HZXPtp
        {
            public float X, Y, Z;
            public float AX, AY, AZ;
            public ushort Action, Time, Dir, Move;
            public uint Flag, GroupId;
            public byte[] Extra;
            public long FileOffset;

            public static HZXPtp FromBytes(byte[] data, long fileOffset)
            {
                if (data.Length < 48)
                    throw new Exception("Not enough data for HZX_PTP.");

                HZXPtp ptp = new HZXPtp();
                ptp.X = BitConverter.ToSingle(data, 0);
                float zVal = BitConverter.ToSingle(data, 4);
                float yVal = BitConverter.ToSingle(data, 8);
                ptp.AX = BitConverter.ToSingle(data, 12);
                float azVal = BitConverter.ToSingle(data, 16);
                float ayVal = BitConverter.ToSingle(data, 20);
                ptp.Action = BitConverter.ToUInt16(data, 24);
                ptp.Time = BitConverter.ToUInt16(data, 26);
                ptp.Dir = BitConverter.ToUInt16(data, 28);
                ptp.Move = BitConverter.ToUInt16(data, 30);
                ptp.Flag = BitConverter.ToUInt32(data, 32);
                ptp.GroupId = BitConverter.ToUInt32(data, 36);

                ptp.Extra = new byte[8];
                Array.Copy(data, 40, ptp.Extra, 0, 8);

                ptp.Z = zVal;
                ptp.Y = yVal;
                ptp.AZ = azVal;
                ptp.AY = ayVal;

                ptp.FileOffset = fileOffset;
                return ptp;
            }
        }

        public class GuardRouteEditorForm : Form
        {
            private List<HZXPat> _patEntries;
            private Dictionary<uint, List<HZXPtp>> _guardRoutes;
            private int _currentIndex = 0;
            private Label lblRouteInfo;
            private Label lblTitle;
            private DataGridView dataGrid;
            private Button btnPrev, btnNext, btnSetAllXYZ, btnSetAllAXYZ, btnSave;
            private TableLayoutPanel mainLayout;
            private string _filePath;

            private int colIdx = 0;
            private int colX = 1;
            private int colY = 2;
            private int colZ = 3;
            private int colSnakeXYZ = 4;
            private int colAX = 5;
            private int colAY = 6;
            private int colAZ = 7;
            private int colSnakeA = 8;
            private int colAction = 9;
            private int colMove = 10;
            private int colTime = 11;
            private int colDir = 12;

            public GuardRouteEditorForm(
                List<HZXPat> patEntries,
                Dictionary<uint, List<HZXPtp>> guardRoutes,
                string filePath)
            {
                _patEntries = patEntries;
                _guardRoutes = guardRoutes;
                _filePath = filePath;

                InitializeComponents();
                LoadCurrentRoute();
            }

            private static readonly Dictionary<string, ushort> DirectionMap = new Dictionary<string, ushort>
            {
            {"North", 0x0800},
            {"South", 0x0000},
            {"East", 0x0400},
            {"West", 0x0C00},
            {"North East", 0x0600},
            {"North West", 0x0A00},
            {"South East", 0x0200},
            {"South West", 0x0E00}
            };

            private static readonly Dictionary<string, ushort> ActionMap = new Dictionary<string, ushort>
            {
            {"Normal patrol", 0x0000},
            {"Yawns", 0x0001},
            {"Stretches", 0x0002},
            {"Falls Asleep", 0x0003},
            {"Binos at AX AY AZ", 0x0004},
            {"Looks left and right", 0x0008},
            {"Looks at ground", 0x000E},
            {"Aims Gun + Waves hand", 0x0011},
            {"Aims Gun at point", 0x0012},
            {"Cautious Left Walk", 0x0014},
            {"Cautious Right Walk", 0x0015},
            {"Jogs", 0x0016},
            {"Uses Radio", 0x0017},
            {"Pee animation", 0x0018},
            {"Runs with Gun", 0x0019},
            {"Leans right", 0x001A},
            {"Leans left", 0x001B},
            {"Rolls right", 0x001C},
            {"Rolls left", 0x001D},
            {"Walks backwards pointing gun", 0x001F}
            };

            private void InitializeComponents()
            {
                this.Text = "MGS2 Guard Route Editor";
                this.Size = new Size(1100, 700);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.FromArgb(15, 57, 48);

                mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(15, 57, 48),
                    ColumnCount = 1,
                    RowCount = 4
                };
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                this.Controls.Add(mainLayout);

                lblTitle = new Label
                {
                    Text = "MGS2 Guard Route Editor",
                    Height = 25,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = SystemColors.Control,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(15, 57, 48)
                };
                mainLayout.Controls.Add(lblTitle, 0, 0);

                lblRouteInfo = new Label
                {
                    Height = 25,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = SystemColors.Control,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(15, 57, 48)
                };
                mainLayout.Controls.Add(lblRouteInfo, 0, 1);

                dataGrid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = Color.FromArgb(15, 57, 48),
                    BorderStyle = BorderStyle.None,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    EditMode = DataGridViewEditMode.EditOnEnter,
                    SelectionMode = DataGridViewSelectionMode.CellSelect

                };

                dataGrid.EditingControlShowing += (s, e) =>
                {
                    if (e.Control is ComboBox combo && dataGrid.CurrentCell.ColumnIndex == colDir)
                    {
                        combo.DropDownStyle = ComboBoxStyle.DropDown;
                    }
                };

                dataGrid.DefaultCellStyle.BackColor = Color.FromArgb(15, 57, 48);
                dataGrid.DefaultCellStyle.ForeColor = SystemColors.Control;
                dataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 80, 70);
                dataGrid.DefaultCellStyle.SelectionForeColor = SystemColors.Control;
                dataGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 57, 48);
                dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.Control;
                dataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 57, 48);
                dataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.Control;
                dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                dataGrid.EditingControlShowing += (s, e) =>
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.MouseDoubleClick -= TextBox_MouseDoubleClick;
                        tb.MouseDoubleClick += TextBox_MouseDoubleClick;
                    }
                };

                var col0 = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Idx",
                    ReadOnly = true,
                    Width = 5
                };
                dataGrid.Columns.Add(col0);

                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "X" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Y" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Z" });

                var col4 = new DataGridViewButtonColumn
                {
                    HeaderText = "Snake XYZ",
                    Text = "Set",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                };
                dataGrid.Columns.Add(col4);

                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "AX" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "AY" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "AZ" });

                var col8 = new DataGridViewButtonColumn
                {
                    HeaderText = "Snake A(X/Y/Z)",
                    Text = "Set",
                    UseColumnTextForButtonValue = true,
                    Width = 100
                };
                dataGrid.Columns.Add(col8);

                var actionColumn = new DataGridViewComboBoxColumn
                {
                    HeaderText = "Action",
                    Name = "Action",
                    FlatStyle = FlatStyle.Flat,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoComplete = true,
                    Width = 200
                };

                actionColumn.Items.AddRange(ActionMap.Keys.ToArray());
                actionColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                actionColumn.DropDownWidth = 200;
                actionColumn.AutoComplete = true;

                dataGrid.Columns.Add(actionColumn);

                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Move" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Time" });
                var dirColumn = new DataGridViewComboBoxColumn
                {
                    HeaderText = "Direction",
                    Name = "Dir",
                    FlatStyle = FlatStyle.Flat,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing,
                    AutoComplete = true,
                    Width = 200
                };

                // Add direction names first
                dirColumn.Items.AddRange(DirectionMap.Keys.ToArray());

                // Make the combo box editable
                dirColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                dirColumn.DropDownWidth = 150;
                dirColumn.AutoComplete = true;

                dataGrid.Columns.Add(dirColumn);

                // Add these event handlers
                dataGrid.EditingControlShowing += DataGrid_EditingControlShowing;
                dataGrid.CellEndEdit += DataGrid_CellEndEdit;
                dataGrid.CellValidating += DataGrid_CellValidating;

                dataGrid.CellClick += DataGrid_CellClick;

                mainLayout.Controls.Add(dataGrid, 0, 2);

                FlowLayoutPanel bottomPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Height = 60,
                    Padding = new Padding(10),
                    BackColor = Color.FromArgb(15, 57, 48)
                };

                btnPrev = CreateButton("< Prev", 80);
                btnPrev.Click += (s, e) =>
                {
                    if (_currentIndex > 0)
                    {
                        if (!ApplyChangesFromGrid()) return;
                        _currentIndex--;
                        LoadCurrentRoute();
                    }
                };
                bottomPanel.Controls.Add(btnPrev);

                btnNext = CreateButton("Next >", 80);
                btnNext.Click += (s, e) =>
                {
                    if (_currentIndex < _patEntries.Count - 1)
                    {
                        if (!ApplyChangesFromGrid()) return;
                        _currentIndex++;
                        LoadCurrentRoute();
                    }
                };
                bottomPanel.Controls.Add(btnNext);

                btnSetAllXYZ = CreateButton("Set All X/Y/Z", 120);
                btnSetAllXYZ.Click += (s, e) =>
                {
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        SetSnakeOrRaidenXyz(i, false);
                    }
                };
                bottomPanel.Controls.Add(btnSetAllXYZ);

                btnSetAllAXYZ = CreateButton("Set All A(X/Y/Z)", 130);
                btnSetAllAXYZ.Click += (s, e) =>
                {
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        SetSnakeOrRaidenXyz(i, true);
                    }
                };
                bottomPanel.Controls.Add(btnSetAllAXYZ);


                btnSave = CreateButton("Save All", 100);
                btnSave.Click += (s, e) =>
                {
                    if (!ApplyChangesFromGrid()) return;
                    SaveFile();
                    // Removed the closing line since I found it more of a hinderance that it closed while testing new guard routes
                    // this.Close();
                };
                bottomPanel.Controls.Add(btnSave);

                mainLayout.Controls.Add(bottomPanel, 0, 3);
            }

            private void DataGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
            {
                if (dataGrid.CurrentCell.ColumnIndex == colDir && e.Control is ComboBox dirCombo)
                {
                    dirCombo.DropDownStyle = ComboBoxStyle.DropDown;
                    dirCombo.Validating += ComboDirection_Validating;
                }
                else if (dataGrid.CurrentCell.ColumnIndex == colAction && e.Control is ComboBox actionCombo)
                {
                    actionCombo.DropDownStyle = ComboBoxStyle.DropDown;
                    actionCombo.Validating += ComboAction_Validating;
                }
            }

            private void ComboDirection_Validating(object sender, CancelEventArgs e)
            {
                if (sender is ComboBox combo && dataGrid.CurrentCell.ColumnIndex == colDir)
                {
                    string input = combo.Text;
                    ushort value;

                    if (DirectionMap.ContainsKey(input))
                    {
                        return;
                    }

                    if (ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    {
                        string hexValue = value.ToString("X4");
                        if (!combo.Items.Contains(hexValue))
                        {
                            combo.Items.Add(hexValue);
                        }
                        combo.Text = hexValue;
                    }
                    else
                    {
                        MessageBox.Show("Invalid direction. Must be a direction name or hexadecimal value.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }

            private void ComboAction_Validating(object sender, CancelEventArgs e)
            {
                if (sender is ComboBox combo && dataGrid.CurrentCell.ColumnIndex == colAction)
                {
                    string input = combo.Text;
                    ushort value;

                    if (ActionMap.ContainsKey(input))
                    {
                        return;
                    }

                    if (ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    {
                        string hexValue = value.ToString("X4");
                        if (!combo.Items.Contains(hexValue))
                        {
                            combo.Items.Add(hexValue);
                        }
                        combo.Text = hexValue;
                    }
                    else
                    {
                        MessageBox.Show("Invalid action. Must be an action name or hexadecimal value.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }

            private void DataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                if (e.ColumnIndex == colDir)
                {
                    var cell = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                    string currentValue = cell.Value?.ToString();

                    if (!string.IsNullOrEmpty(currentValue) && !cell.Items.Contains(currentValue))
                    {
                        cell.Items.Add(currentValue);
                    }
                }
                else if (e.ColumnIndex == colAction)
                {
                    var cell = dataGrid.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell;
                    string currentValue = cell.Value?.ToString();

                    if (!string.IsNullOrEmpty(currentValue) && !cell.Items.Contains(currentValue))
                    {
                        cell.Items.Add(currentValue);
                    }
                }
            }

            private void DataGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
            {
                if (e.ColumnIndex == colDir)
                {
                    string input = e.FormattedValue.ToString();
                    ushort value;

                    if (!DirectionMap.ContainsKey(input) &&
                        !ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    {
                        MessageBox.Show("Invalid direction. Must be a direction name or hexadecimal value.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
                else if (e.ColumnIndex == colAction)
                {
                    string input = e.FormattedValue.ToString();
                    ushort value;

                    if (!ActionMap.ContainsKey(input) &&
                        !ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    {
                        MessageBox.Show("Invalid action. Must be an action name or hexadecimal value.", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        e.Cancel = true;
                    }
                }
            }
            private Button CreateButton(string text, int width)
            {
                return new Button
                {
                    Text = text,
                    Width = width,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    FlatStyle = FlatStyle.Popup,
                    BackColor = Color.FromArgb(15, 57, 48),
                    ForeColor = SystemColors.Control,
                    Margin = new Padding(5)
                };
            }

            private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                if (sender is TextBox tb)
                {
                    tb.SelectAll();
                }
            }

            private void SetSnakeOrRaidenXyz(int rowIndex, bool isAim)
            {
                string x = MGS2MemoryManager.ReadMGS2PlayerPositionX();
                string y = MGS2MemoryManager.ReadMGS2PlayerPositionY();
                string z = MGS2MemoryManager.ReadMGS2PlayerPositionZ();

                if (!isAim)
                {
                    dataGrid.Rows[rowIndex].Cells[colX].Value = x;
                    dataGrid.Rows[rowIndex].Cells[colY].Value = y;
                    dataGrid.Rows[rowIndex].Cells[colZ].Value = z;
                }
                else
                {
                    dataGrid.Rows[rowIndex].Cells[colAX].Value = x;
                    dataGrid.Rows[rowIndex].Cells[colAY].Value = y;
                    dataGrid.Rows[rowIndex].Cells[colAZ].Value = z;
                }
            }

            private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0 || e.RowIndex >= dataGrid.Rows.Count)
                    return;
                if (e.ColumnIndex == colSnakeXYZ)
                {
                    SetSnakeOrRaidenXyz(e.RowIndex, false);
                }
                else if (e.ColumnIndex == colSnakeA)
                {
                    SetSnakeOrRaidenXyz(e.RowIndex, true);
                }
            }

            private void LoadCurrentRoute()
            {
                if (_currentIndex < 0 || _currentIndex >= _patEntries.Count)
                    return;

                uint key = _patEntries[_currentIndex].PointsPtr;
                List<HZXPtp> route = _guardRoutes.ContainsKey(key) ? _guardRoutes[key] : new List<HZXPtp>();

                lblRouteInfo.Text = $"Guard Route: {_currentIndex} of {_patEntries.Count - 1} - Points: {route.Count}";
                dataGrid.Rows.Clear();

                for (int i = 0; i < route.Count; i++)
                {
                    var ptp = route[i];
                    var row = new DataGridViewRow();
                    row.CreateCells(dataGrid);

                    row.Cells[colIdx].Value = i.ToString();
                    row.Cells[colX].Value = ptp.X.ToString("F6");
                    row.Cells[colY].Value = ptp.Y.ToString("F6");
                    row.Cells[colZ].Value = ptp.Z.ToString("F6");
                    row.Cells[colSnakeXYZ].Value = "Set";
                    row.Cells[colAX].Value = ptp.AX.ToString("F6");
                    row.Cells[colAY].Value = ptp.AY.ToString("F6");
                    row.Cells[colAZ].Value = ptp.AZ.ToString("F6");
                    row.Cells[colSnakeA].Value = "Set";
                    row.Cells[colMove].Value = ptp.Move.ToString("X4");
                    row.Cells[colTime].Value = ptp.Time.ToString("X4");

                    var dirCell = (DataGridViewComboBoxCell)row.Cells[colDir];
                    var actionCell = (DataGridViewComboBoxCell)row.Cells[colAction];

                    string dirDisplayValue = GetDisplayValueForDirection(ptp.Dir, dirCell);
                    string actionDisplayValue = GetDisplayValueForAction(ptp.Action, actionCell);

                    dirCell.Value = dirDisplayValue;
                    actionCell.Value = actionDisplayValue;

                    dataGrid.Rows.Add(row);
                }

                if (dataGrid.Rows.Count > 0)
                    dataGrid.FirstDisplayedScrollingRowIndex = 0;
            }

            private string GetDisplayValueForDirection(ushort value, DataGridViewComboBoxCell cell)
            {
                var knownDir = DirectionMap.FirstOrDefault(x => x.Value == value);
                if (!string.IsNullOrEmpty(knownDir.Key))
                {
                    return knownDir.Key;
                }

                string hexValue = value.ToString("X4");
                if (!cell.Items.Contains(hexValue))
                {
                    cell.Items.Add(hexValue);
                }
                return hexValue;
            }

            private string GetDisplayValueForAction(ushort value, DataGridViewComboBoxCell cell)
            {
                var knownAction = ActionMap.FirstOrDefault(x => x.Value == value);
                if (!string.IsNullOrEmpty(knownAction.Key))
                {
                    return knownAction.Key;
                }

                string hexValue = value.ToString("X4");
                if (!cell.Items.Contains(hexValue))
                {
                    cell.Items.Add(hexValue);
                }
                return hexValue;
            }

            private bool ApplyChangesFromGrid()
            {
                if (_currentIndex < 0 || _currentIndex >= _patEntries.Count)
                    return true;

                uint key = _patEntries[_currentIndex].PointsPtr;
                if (!_guardRoutes.ContainsKey(key))
                    return true;

                var route = _guardRoutes[key];
                if (route.Count != dataGrid.Rows.Count)
                    return true;

                for (int i = 0; i < route.Count; i++)
                {
                    DataGridViewRow row = dataGrid.Rows[i];
                    try
                    {
                        route[i].Dir = ParseDirectionValue(row.Cells[colDir].Value?.ToString());
                        route[i].Action = ParseActionValue(row.Cells[colAction].Value?.ToString());

                        route[i].X = float.Parse(row.Cells[colX].Value?.ToString() ?? "0");
                        route[i].Y = float.Parse(row.Cells[colY].Value?.ToString() ?? "0");
                        route[i].Z = float.Parse(row.Cells[colZ].Value?.ToString() ?? "0");
                        route[i].AX = float.Parse(row.Cells[colAX].Value?.ToString() ?? "0");
                        route[i].AY = float.Parse(row.Cells[colAY].Value?.ToString() ?? "0");
                        route[i].AZ = float.Parse(row.Cells[colAZ].Value?.ToString() ?? "0");
                        route[i].Move = ushort.Parse(row.Cells[colMove].Value?.ToString() ?? "0", NumberStyles.HexNumber);
                        route[i].Time = ushort.Parse(row.Cells[colTime].Value?.ToString() ?? "0", NumberStyles.HexNumber);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error in row {i}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }

            private ushort ParseDirectionValue(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return 0;

                if (DirectionMap.TryGetValue(input, out ushort value))
                    return value;

                if (ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    return value;

                throw new FormatException($"Invalid direction value: {input}");
            }

            private ushort ParseActionValue(string input)
            {
                if (string.IsNullOrEmpty(input))
                    return 0;

                if (ActionMap.TryGetValue(input, out ushort value))
                    return value;

                if (ushort.TryParse(input, NumberStyles.HexNumber, null, out value))
                    return value;

                throw new FormatException($"Invalid action value: {input}");
            }

            private void SaveFile()
            {
                if (string.IsNullOrEmpty(_filePath))
                {
                    MessageBox.Show("No file path specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Backup file so the user doesn't need to go to the backed up vanilla files to restore the original file
                string backupPath = _filePath + ".bak";
                try
                {
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);
                    File.Copy(_filePath, backupPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to create backup:\n{ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                try
                {
                    using (FileStream fs = File.Open(_filePath, FileMode.Open, FileAccess.ReadWrite))
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        foreach (var pat in _patEntries)
                        {
                            uint key = pat.PointsPtr;
                            if (!_guardRoutes.ContainsKey(key))
                                continue;

                            List<HZXPtp> pts = _guardRoutes[key];
                            fs.Seek(pat.PointsPtr, SeekOrigin.Begin);

                            foreach (var ptp in pts)
                            {
                                bw.Write(ptp.X);
                                bw.Write(ptp.Z);
                                bw.Write(ptp.Y);
                                bw.Write(ptp.AX);
                                bw.Write(ptp.AZ);
                                bw.Write(ptp.AY);
                                bw.Write(ptp.Action);
                                bw.Write(ptp.Time);
                                bw.Write(ptp.Dir);
                                bw.Write(ptp.Move);
                                bw.Write(ptp.Flag);
                                bw.Write(ptp.GroupId);
                                bw.Write(ptp.Extra);
                            }
                        }
                    }
                    MessageBox.Show("File saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

            public static void EditHzxFile()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\MGS2\assets\hzx\us",
                Filter = "HZX files (*.hzx)|*.hzx|All files (*.*)|*.*"
            };
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string filePath = ofd.FileName;
            HZXMap map = HZXMap.Load(filePath);

            List<HZXPat> patEntries = new List<HZXPat>();
            using (FileStream fs = File.OpenRead(filePath))
            using (BinaryReader br = new BinaryReader(fs))
            {
                fs.Seek(HZXMap.HeaderSize, SeekOrigin.Begin);
                int patEntrySize = 16;
                byte[] patData = br.ReadBytes(map.NPatrols * patEntrySize);
                int offset = 0;
                for (int i = 0; i < map.NPatrols; i++)
                {
                    byte[] block = patData.Skip(offset).Take(patEntrySize).ToArray();
                    HZXPat pat = HZXPat.FromBytes(block, HZXMap.HeaderSize + offset);
                    if (pat.NPoints > 0 && pat.PointsPtr > 0)
                        patEntries.Add(pat);
                    offset += patEntrySize;
                }
            }

            Dictionary<uint, List<HZXPtp>> guardRoutes = new Dictionary<uint, List<HZXPtp>>();
            using (FileStream fs = File.OpenRead(filePath))
            using (BinaryReader br = new BinaryReader(fs))
            {
                foreach (var pat in patEntries)
                {
                    List<HZXPtp> ptpList = new List<HZXPtp>();
                    fs.Seek(pat.PointsPtr, SeekOrigin.Begin);
                    for (int i = 0; i < pat.NPoints; i++)
                    {
                        byte[] ptpData = br.ReadBytes(48);
                        if (ptpData.Length < 48)
                            break;
                        HZXPtp ptp = HZXPtp.FromBytes(ptpData, fs.Position - 48);
                        ptpList.Add(ptp);
                    }
                    guardRoutes[pat.PointsPtr] = ptpList;
                }
            }

            GuardRouteEditorForm form = new GuardRouteEditorForm(patEntries, guardRoutes, filePath);
            form.ShowDialog();
        }
    }
}
