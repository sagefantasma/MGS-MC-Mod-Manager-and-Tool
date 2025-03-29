using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class HzxEditor
    {
        // HZX_MAP header (96 bytes)
        public class HZXMap
        {
            public ushort Version;
            public ushort NGroups;
            public ushort NPatrols;
            public ushort NClears;
            public byte[] Pad; // 40 bytes
            public uint PatrolsPtr;
            public uint PointsPtr;
            public uint ClearsPtr;
            public uint CPatrolsPtr;
            public uint CPointsPtr;
            public uint GroupsPtr;
            public byte[] ExtraReserved; // up to 96
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

        // HZX_PAT entry (16 bytes)
        public class HZXPat
        {
            public ushort NPoints;
            public ushort InitPoint;
            public ushort PointsOffset; // offset to HZX_PTP entries
            public long FileOffset;     // absolute offset in file

            public static HZXPat FromBytes(byte[] data, long fileOffset)
            {
                if (data.Length < 16)
                    throw new Exception("Not enough data for HZX_PAT.");

                HZXPat pat = new HZXPat();
                pat.NPoints = BitConverter.ToUInt16(data, 0);
                pat.InitPoint = BitConverter.ToUInt16(data, 2);
                // Bytes 4–7: padding
                pat.PointsOffset = BitConverter.ToUInt16(data, 8);
                pat.FileOffset = fileOffset;
                return pat;
            }
        }

        // HZX_PTP entry (48 bytes) for MGS2:
        // Bytes 0–3: float X
        // Bytes 4–7: float Z
        // Bytes 8–11: float Y
        // Bytes 12–15: float AX
        // Bytes 16–19: float AZ
        // Bytes 20–23: float AY
        // Bytes 24–25: ushort Action
        // Bytes 26–27: ushort Time
        // Bytes 28–29: ushort Dir
        // Bytes 30–31: ushort Move
        // Bytes 32–35: uint Flag
        // Bytes 36–39: uint GroupId
        // Bytes 40–47: 8 bytes Extra
        public class HZXPtp
        {
            public float X, Y, Z;
            public float AX, AY, AZ;
            public ushort Action, Time, Dir, Move;
            public uint Flag, GroupId;
            public byte[] Extra;   // 8 bytes
            public long FileOffset;

            public static HZXPtp FromBytes(byte[] data, long fileOffset)
            {
                if (data.Length < 48)
                    throw new Exception("Not enough data for HZX_PTP.");

                HZXPtp ptp = new HZXPtp();
                // read X, Z, Y, AX, AZ, AY
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

                // MGS2 stores pos as (X, Z, Y) and aim as (AX, AZ, AY).
                ptp.Z = zVal;
                ptp.Y = yVal;
                ptp.AZ = azVal;
                ptp.AY = ayVal;

                ptp.FileOffset = fileOffset;
                return ptp;
            }
        }

        // Editor form using a TableLayoutPanel to ensure the DataGridView is fully visible
        public class GuardRouteEditorForm : Form
        {
            private List<HZXPat> _patEntries;
            private Dictionary<ushort, List<HZXPtp>> _guardRoutes;
            private int _currentIndex = 0;
            private Label lblRouteInfo;
            private Label lblTitle;
            private DataGridView dataGrid;
            private Button btnPrev, btnNext, btnSetAllXYZ, btnSetAllAXYZ, btnSave;
            private TableLayoutPanel mainLayout;
            private string _filePath;

            // We'll store the columns so we can reference them easily
            private int colIdx = 0;     // read-only
            private int colX = 1;
            private int colY = 2;
            private int colZ = 3;
            private int colSnakeXYZ = 4;   // button
            private int colAX = 5;
            private int colAY = 6;
            private int colAZ = 7;
            private int colSnakeA = 8;     // button
            private int colAction = 9;
            private int colMove = 10;
            private int colTime = 11;
            private int colDir = 12;

            public GuardRouteEditorForm(
                List<HZXPat> patEntries,
                Dictionary<ushort, List<HZXPtp>> guardRoutes,
                string filePath)
            {
                _patEntries = patEntries;
                _guardRoutes = guardRoutes;
                _filePath = filePath;

                InitializeComponents();
                LoadCurrentRoute();
            }

            private void InitializeComponents()
            {
                this.Text = "MGS2 Guard Route Editor";
                this.Size = new Size(1100, 700);
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.FromArgb(15, 57, 48);

                // We'll build everything inside a TableLayoutPanel with 4 rows:
                // Row 0: Title label
                // Row 1: Route info label
                // Row 2: DataGridView (fills leftover space)
                // Row 3: bottom button panel
                mainLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromArgb(15, 57, 48),
                    ColumnCount = 1,
                    RowCount = 4
                };
                // Row 2 (the data grid row) should fill the remaining vertical space
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // row 0
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // row 1
                mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f)); // row 2
                mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // row 3
                this.Controls.Add(mainLayout);

                // Title label
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

                // Route info label
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

                // DataGridView in row 2
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
                    // Single-click to edit
                    EditMode = DataGridViewEditMode.EditOnEnter,
                    SelectionMode = DataGridViewSelectionMode.CellSelect
                };

                // Style the default cell style
                dataGrid.DefaultCellStyle.BackColor = Color.FromArgb(15, 57, 48);
                dataGrid.DefaultCellStyle.ForeColor = SystemColors.Control;
                dataGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 80, 70);
                dataGrid.DefaultCellStyle.SelectionForeColor = SystemColors.Control;
                dataGrid.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                // Style column headers
                dataGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 57, 48);
                dataGrid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.Control;
                dataGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(15, 57, 48);
                dataGrid.ColumnHeadersDefaultCellStyle.SelectionForeColor = SystemColors.Control;
                dataGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

                // Double-click selects all text in the editing textbox
                dataGrid.EditingControlShowing += (s, e) =>
                {
                    if (e.Control is TextBox tb)
                    {
                        tb.MouseDoubleClick -= TextBox_MouseDoubleClick;
                        tb.MouseDoubleClick += TextBox_MouseDoubleClick;
                    }
                };

                // Create columns
                // 0: Index
                var col0 = new DataGridViewTextBoxColumn
                {
                    HeaderText = "Idx",
                    ReadOnly = true,
                    Width = 40
                };
                dataGrid.Columns.Add(col0);

                // 1: X
                var col1 = new DataGridViewTextBoxColumn { HeaderText = "X" };
                dataGrid.Columns.Add(col1);

                // 2: Y
                var col2 = new DataGridViewTextBoxColumn { HeaderText = "Y" };
                dataGrid.Columns.Add(col2);

                // 3: Z
                var col3 = new DataGridViewTextBoxColumn { HeaderText = "Z" };
                dataGrid.Columns.Add(col3);

                // 4: "Snake XYZ" button
                var col4 = new DataGridViewButtonColumn
                {
                    HeaderText = "Snake XYZ",
                    Text = "Set",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                };
                dataGrid.Columns.Add(col4);

                // 5: AX
                var col5 = new DataGridViewTextBoxColumn { HeaderText = "AX" };
                dataGrid.Columns.Add(col5);

                // 6: AY
                var col6 = new DataGridViewTextBoxColumn { HeaderText = "AY" };
                dataGrid.Columns.Add(col6);

                // 7: AZ
                var col7 = new DataGridViewTextBoxColumn { HeaderText = "AZ" };
                dataGrid.Columns.Add(col7);

                // 8: "Snake A(X/Y/Z)" button
                var col8 = new DataGridViewButtonColumn
                {
                    HeaderText = "Snake A(X/Y/Z)",
                    Text = "Set",
                    UseColumnTextForButtonValue = true,
                    Width = 110
                };
                dataGrid.Columns.Add(col8);

                // 9: Action
                var col9 = new DataGridViewTextBoxColumn { HeaderText = "Action" };
                dataGrid.Columns.Add(col9);

                // 10: Move
                var col10 = new DataGridViewTextBoxColumn { HeaderText = "Move" };
                dataGrid.Columns.Add(col10);

                // 11: Time
                var col11 = new DataGridViewTextBoxColumn { HeaderText = "Time" };
                dataGrid.Columns.Add(col11);

                // 12: Dir
                var col12 = new DataGridViewTextBoxColumn { HeaderText = "Dir" };
                dataGrid.Columns.Add(col12);

                // Handle row-based button clicks
                dataGrid.CellClick += DataGrid_CellClick;

                // Add the DataGridView to row 2
                mainLayout.Controls.Add(dataGrid, 0, 2);

                // Bottom panel (row 3)
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
                    // For all rows in current route, set X,Y,Z to (8987,2428,2000).
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        dataGrid.Rows[i].Cells[colX].Value = "8987";
                        dataGrid.Rows[i].Cells[colY].Value = "2428";
                        dataGrid.Rows[i].Cells[colZ].Value = "2000";
                    }
                };
                bottomPanel.Controls.Add(btnSetAllXYZ);

                btnSetAllAXYZ = CreateButton("Set All A(X/Y/Z)", 130);
                btnSetAllAXYZ.Click += (s, e) =>
                {
                    // For all rows in current route, set AX,AY,AZ to (8987,2428,2000).
                    for (int i = 0; i < dataGrid.Rows.Count; i++)
                    {
                        dataGrid.Rows[i].Cells[colAX].Value = "8987";
                        dataGrid.Rows[i].Cells[colAY].Value = "2428";
                        dataGrid.Rows[i].Cells[colAZ].Value = "2000";
                    }
                };
                bottomPanel.Controls.Add(btnSetAllAXYZ);

                btnSave = CreateButton("Save All", 100);
                btnSave.Click += (s, e) =>
                {
                    if (!ApplyChangesFromGrid()) return;
                    SaveFile();
                    MessageBox.Show("File saved.", "Success");
                    this.Close();
                };
                bottomPanel.Controls.Add(btnSave);

                // Add the bottom panel to row 3
                mainLayout.Controls.Add(bottomPanel, 0, 3);
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

            // Double-click selects all text in the cell's textbox
            private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                if (sender is TextBox tb)
                {
                    tb.SelectAll();
                }
            }

            // This method updates a single row's position or aim to (8987,2428,2000).
            private void SetSnakeOrRaidenXyz(int rowIndex, bool isAim)
            {
                // Hardcoded for demonstration
                string x = MGS2MemoryManager.ReadMGS2PlayerPositionX();
                string y = MGS2MemoryManager.ReadMGS2PlayerPositionY();
                string z = MGS2MemoryManager.ReadMGS2PlayerPositionZ();

                if (!isAim)
                {
                    // X,Y,Z
                    dataGrid.Rows[rowIndex].Cells[colX].Value = x;
                    dataGrid.Rows[rowIndex].Cells[colY].Value = y;
                    dataGrid.Rows[rowIndex].Cells[colZ].Value = z;
                }
                else
                {
                    // AX,AY,AZ
                    dataGrid.Rows[rowIndex].Cells[colAX].Value = x;
                    dataGrid.Rows[rowIndex].Cells[colAY].Value = y;
                    dataGrid.Rows[rowIndex].Cells[colAZ].Value = z;
                }
            }

            private void DataGrid_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0 || e.RowIndex >= dataGrid.Rows.Count)
                    return;
                // colSnakeXYZ => 4, colSnakeA => 8
                if (e.ColumnIndex == colSnakeXYZ)
                {
                    // row-based Snake XYZ => set X,Y,Z
                    SetSnakeOrRaidenXyz(e.RowIndex, false);
                }
                else if (e.ColumnIndex == colSnakeA)
                {
                    // row-based Snake A(X/Y/Z) => set AX,AY,AZ
                    SetSnakeOrRaidenXyz(e.RowIndex, true);
                }
            }

            private void LoadCurrentRoute()
            {
                if (_currentIndex < 0 || _currentIndex >= _patEntries.Count)
                    return;

                ushort key = _patEntries[_currentIndex].PointsOffset;
                List<HZXPtp> route = _guardRoutes.ContainsKey(key) ? _guardRoutes[key] : new List<HZXPtp>();

                lblRouteInfo.Text = $"Guard Route: {_currentIndex + 1} of {_patEntries.Count} - Points: {route.Count}";

                dataGrid.Rows.Clear();

                for (int i = 0; i < route.Count; i++)
                {
                    var ptp = route[i];
                    // Convert to string for the grid
                    string xStr = ptp.X.ToString("F6");
                    string yStr = ptp.Y.ToString("F6");
                    string zStr = ptp.Z.ToString("F6");
                    string axStr = ptp.AX.ToString("F6");
                    string ayStr = ptp.AY.ToString("F6");
                    string azStr = ptp.AZ.ToString("F6");
                    string actionStr = ptp.Action.ToString("X4");
                    string moveStr = ptp.Move.ToString("X4");
                    string timeStr = ptp.Time.ToString("X4");
                    string dirStr = ptp.Dir.ToString("X4");

                    // Add row
                    dataGrid.Rows.Add(
                        i.ToString(), // colIdx
                        xStr,         // colX
                        yStr,         // colY
                        zStr,         // colZ
                        "Set",        // colSnakeXYZ button
                        axStr,        // colAX
                        ayStr,        // colAY
                        azStr,        // colAZ
                        "Set",        // colSnakeA button
                        actionStr,    // colAction
                        moveStr,      // colMove
                        timeStr,      // colTime
                        dirStr        // colDir
                    );
                }

                // Scroll to top
                if (dataGrid.Rows.Count > 0)
                    dataGrid.FirstDisplayedScrollingRowIndex = 0;
            }

            private bool ApplyChangesFromGrid()
            {
                if (_currentIndex < 0 || _currentIndex >= _patEntries.Count)
                    return true;

                ushort key = _patEntries[_currentIndex].PointsOffset;
                if (!_guardRoutes.ContainsKey(key))
                    return true;

                var route = _guardRoutes[key];
                if (route.Count != dataGrid.Rows.Count)
                    return true; // mismatch or empty

                for (int i = 0; i < route.Count; i++)
                {
                    DataGridViewRow row = dataGrid.Rows[i];
                    try
                    {
                        float xVal = float.Parse((string)row.Cells[colX].Value);
                        float yVal = float.Parse((string)row.Cells[colY].Value);
                        float zVal = float.Parse((string)row.Cells[colZ].Value);

                        float axVal = float.Parse((string)row.Cells[colAX].Value);
                        float ayVal = float.Parse((string)row.Cells[colAY].Value);
                        float azVal = float.Parse((string)row.Cells[colAZ].Value);

                        ushort actionVal = ushort.Parse(((string)row.Cells[colAction].Value).Trim(), System.Globalization.NumberStyles.HexNumber);
                        ushort moveVal = ushort.Parse(((string)row.Cells[colMove].Value).Trim(), System.Globalization.NumberStyles.HexNumber);
                        ushort timeVal = ushort.Parse(((string)row.Cells[colTime].Value).Trim(), System.Globalization.NumberStyles.HexNumber);
                        ushort dirVal = ushort.Parse(((string)row.Cells[colDir].Value).Trim(), System.Globalization.NumberStyles.HexNumber);

                        route[i].X = xVal;
                        route[i].Y = yVal;
                        route[i].Z = zVal;
                        route[i].AX = axVal;
                        route[i].AY = ayVal;
                        route[i].AZ = azVal;
                        route[i].Action = actionVal;
                        route[i].Move = moveVal;
                        route[i].Time = timeVal;
                        route[i].Dir = dirVal;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error parsing row {i}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                return true;
            }

            private void SaveFile()
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\MGS2\assets\hzx\us",
                    Filter = "HZX files (*.hzx)|*.hzx|All files (*.*)|*.*"
                };
                if (ofd.ShowDialog() != DialogResult.OK)
                    return;

                string savePath = ofd.FileName;
                using (FileStream fs = File.Open(savePath, FileMode.Open, FileAccess.ReadWrite))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (var pat in _patEntries)
                    {
                        ushort key = pat.PointsOffset;
                        if (!_guardRoutes.ContainsKey(key))
                            continue;
                        List<HZXPtp> pts = _guardRoutes[key];
                        fs.Seek(pat.PointsOffset, SeekOrigin.Begin);

                        foreach (var ptp in pts)
                        {
                            // Write 6 floats in the same order read: X, Z, Y, AX, AZ, AY
                            bw.Write(ptp.X);
                            bw.Write(ptp.Z);
                            bw.Write(ptp.Y);
                            bw.Write(ptp.AX);
                            bw.Write(ptp.AZ);
                            bw.Write(ptp.AY);

                            // 4 ushort: Action, Time, Dir, Move
                            bw.Write(ptp.Action);
                            bw.Write(ptp.Time);
                            bw.Write(ptp.Dir);
                            bw.Write(ptp.Move);

                            // 2 uint: Flag, GroupId
                            bw.Write(ptp.Flag);
                            bw.Write(ptp.GroupId);

                            // 8 bytes extra
                            bw.Write(ptp.Extra);
                        }
                    }
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
            // Load header
            HZXMap map = HZXMap.Load(filePath);

            // Read HZX_PAT entries
            List<HZXPat> patEntries = new List<HZXPat>();
            using (FileStream fs = File.OpenRead(filePath))
            using (BinaryReader br = new BinaryReader(fs))
            {
                fs.Seek(HZXMap.HeaderSize, SeekOrigin.Begin);
                byte[] patData = br.ReadBytes(0x100);
                int offset = 0;
                while (offset + 16 <= patData.Length)
                {
                    byte[] block = patData.Skip(offset).Take(16).ToArray();
                    HZXPat pat = HZXPat.FromBytes(block, HZXMap.HeaderSize + offset);
                    if (pat.NPoints > 0 && pat.PointsOffset > 0)
                        patEntries.Add(pat);
                    offset += 16;
                }
            }

            // Read HZX_PTP entries
            Dictionary<ushort, List<HZXPtp>> guardRoutes = new Dictionary<ushort, List<HZXPtp>>();
            using (FileStream fs = File.OpenRead(filePath))
            using (BinaryReader br = new BinaryReader(fs))
            {
                foreach (var pat in patEntries)
                {
                    List<HZXPtp> ptpList = new List<HZXPtp>();
                    fs.Seek(pat.PointsOffset, SeekOrigin.Begin);
                    for (int i = 0; i < pat.NPoints; i++)
                    {
                        byte[] ptpData = br.ReadBytes(48);
                        if (ptpData.Length < 48)
                            break;
                        HZXPtp ptp = HZXPtp.FromBytes(ptpData, fs.Position - 48);
                        ptpList.Add(ptp);
                    }
                    guardRoutes[pat.PointsOffset] = ptpList;
                }
            }

            // Show the editor form
            GuardRouteEditorForm form = new GuardRouteEditorForm(patEntries, guardRoutes, filePath);
            form.ShowDialog();
        }
    }
}
