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
            public byte[] ExtraReserved; // 24 bytes
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

        // Updated HZX_PAT entry (16 bytes) – note we now treat the "points" pointer as a 4-byte uint.
        public class HZXPat
        {
            public ushort NPoints;
            public ushort InitPoint;
            public uint PointsPtr;  // pointer to the HZX_PTP entries
            public long FileOffset; // absolute offset in file

            public static HZXPat FromBytes(byte[] data, long fileOffset)
            {
                if (data.Length < 16)
                    throw new Exception("Not enough data for HZX_PAT.");

                HZXPat pat = new HZXPat();
                pat.NPoints = BitConverter.ToUInt16(data, 0);
                pat.InitPoint = BitConverter.ToUInt16(data, 2);
                // Skip 4 bytes padding (bytes 4–7) then read the 4‑byte pointer.
                pat.PointsPtr = BitConverter.ToUInt32(data, 8);
                // The final 4 bytes (bytes 12–15) are ignored (padding)
                pat.FileOffset = fileOffset;
                return pat;
            }
        }

        // HZX_PTP entry (48 bytes) for MGS2:
        // Bytes 0–3: float X, 4–7: float Z, 8–11: float Y,
        // 12–15: float AX, 16–19: float AZ, 20–23: float AY,
        // 24–25: ushort Action, 26–27: ushort Time,
        // 28–29: ushort Dir, 30–31: ushort Move,
        // 32–35: uint Flag, 36–39: uint GroupId,
        // 40–47: 8 bytes Extra
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
                // read X, Z, Y then AX, AZ, AY
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

                // MGS2 stores position as (X, Z, Y) and aim as (AX, AZ, AY).
                ptp.Z = zVal;
                ptp.Y = yVal;
                ptp.AZ = azVal;
                ptp.AY = ayVal;

                ptp.FileOffset = fileOffset;
                return ptp;
            }
        }

        // Editor form (unchanged except for using uint as key instead of ushort)
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

            // Column definitions (unchanged)
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
                Dictionary<uint, List<HZXPtp>> guardRoutes,
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
                    Width = 40
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
                    Width = 110
                };
                dataGrid.Columns.Add(col8);

                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Action" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Move" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Time" });
                dataGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Dir" });

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
                    MessageBox.Show("File saved.", "Success");
                    this.Close();
                };
                bottomPanel.Controls.Add(btnSave);

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

                // Use the new PointsPtr (uint) as the key.
                uint key = _patEntries[_currentIndex].PointsPtr;
                List<HZXPtp> route = _guardRoutes.ContainsKey(key) ? _guardRoutes[key] : new List<HZXPtp>();

                lblRouteInfo.Text = $"Guard Route: {_currentIndex + 1} of {_patEntries.Count} - Points: {route.Count}";

                dataGrid.Rows.Clear();

                for (int i = 0; i < route.Count; i++)
                {
                    var ptp = route[i];
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

                    dataGrid.Rows.Add(
                        i.ToString(),
                        xStr,
                        yStr,
                        zStr,
                        "Set",
                        axStr,
                        ayStr,
                        azStr,
                        "Set",
                        actionStr,
                        moveStr,
                        timeStr,
                        dirStr
                    );
                }

                if (dataGrid.Rows.Count > 0)
                    dataGrid.FirstDisplayedScrollingRowIndex = 0;
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
            }
        }

        // Main method to open and parse the HZX file.
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

            // Read HZX_PAT entries using map.NPatrols * 16 bytes.
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

            // Read HZX_PTP entries for each patrol.
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

            // Show the editor form.
            GuardRouteEditorForm form = new GuardRouteEditorForm(patEntries, guardRoutes, filePath);
            form.ShowDialog();
        }
    }
}
