using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class TextureModelForm : Form
    {
        private readonly string ctxrToolPath = @"C:\Users\ANTIBigBoss\source\repos\TextureDisplayApp\CtxrTool.exe";
        private ElementHost elementHost;
        private ModelViewerControl modelViewerControl;
        private string gruModelPath;
        private string gruMtlPath;

        public TextureModelForm()
        {
            InitializeComponent();

            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.Black;
            this.BackgroundImage = null;
            this.BackgroundImageLayout = ImageLayout.None;
            this.Load += TextureModelForm_Load;
            this.FormClosing += TextureModelForm_FormClosing;

            DdsFilePictureBox.Dock = DockStyle.None;
            DdsFilePictureBox.Location = new Point(0, 0);
            DdsFilePictureBox.Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height);
            DdsFilePictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            DdsFilePictureBox.AllowDrop = true;
            DdsFilePictureBox.DragEnter += DdsFilePictureBox_DragEnter;
            DdsFilePictureBox.DragDrop += DdsFilePictureBox_DragDrop;

            elementHost = new ElementHost
            {
                Name = "elementHost3D",
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            modelViewerControl = new ModelViewerControl();
            elementHost.Child = modelViewerControl;
            this.Controls.Add(DdsFilePictureBox);
            this.Controls.Add(elementHost);
            elementHost.BringToFront();
            this.Resize += TextureModelForm_Resize;
            AdjustElementHostSize();
        }


        private void TextureModelForm_Load(object sender, EventArgs e)
        {
            string baseDir = @"D:\3D Models\MGS3\GRU BOI\GRU Soldier Mod Manager";
            gruModelPath = Path.Combine(baseDir, "ene_defout.obj");
            gruMtlPath = Path.Combine(baseDir, "ene_defout.mtl");

            if (File.Exists(gruMtlPath))
            {
                RestoreAllMtlReferencesToOriginal(gruMtlPath);
            }
        }

        private void TextureModelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            LoggingManager.Instance.Log("User exiting the Mod Manager.\nEnd of log for this session.\n\n");

            if (File.Exists(gruMtlPath))
            {
                RestoreAllMtlReferencesToOriginal(gruMtlPath);
            }
            Application.Exit();
        }

        private void TextureModelForm_Resize(object sender, EventArgs e)
        {
            DdsFilePictureBox.Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height);
            AdjustElementHostSize();
        }
        private void AdjustElementHostSize()
        {
            int halfWidth = this.ClientSize.Width / 2;
            elementHost.Location = new Point(halfWidth, 0);
            elementHost.Size = new Size(halfWidth, this.ClientSize.Height);
        }

        private async void btnConvertCtxrFiles_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "CTXR Files|*.ctxr",
                Multiselect = true
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (string filePath in ofd.FileNames)
                    {
                        try
                        {
                            string ddsPath = await ConvertCtxrToDds(filePath);
                            if (!string.IsNullOrEmpty(ddsPath))
                            {
                                string pngPath = ConvertDdsToPng(ddsPath);
                            }
                        }
                        catch { }
                    }
                    MessageBox.Show("Conversion completed for all selected files.",
                        "Conversion Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private async Task<string> ConvertCtxrToDds(string ctxrFilePath)
        {
            await Task.Delay(100);
            return null;
        }
        private string ConvertDdsToPng(string ddsFilePath)
        {
            return null;
        }

        private void DdsFilePictureBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                if (extension == ".png" || extension == ".dds" || extension == ".ctxr")
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
        }
        private async void DdsFilePictureBox_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string filePath = files[0];
            string ext = Path.GetExtension(filePath).ToLower();

            if (ext == ".png")
            {
                DdsFilePictureBox.Image = Image.FromFile(filePath);
                DdsFilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else if (ext == ".dds")
            {
                string pngPath = ConvertDdsToPng(filePath);
                if (!string.IsNullOrEmpty(pngPath))
                {
                    DdsFilePictureBox.Image = Image.FromFile(pngPath);
                    DdsFilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else if (ext == ".ctxr")
            {
                string ddsPath = await ConvertCtxrToDds(filePath);
                if (!string.IsNullOrEmpty(ddsPath))
                {
                    string pngPath = ConvertDdsToPng(ddsPath);
                    if (!string.IsNullOrEmpty(pngPath))
                    {
                        DdsFilePictureBox.Image = Image.FromFile(pngPath);
                        DdsFilePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            LoggingManager.Instance.Log("Going back to Mod Resources Form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "ModResourcesForm");
            ModResourcesForm modResourcesForm = new ModResourcesForm();
            modResourcesForm.Show();
            this.Hide();
        }
        private void btnLoadObj_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog
            {
                Filter = "3D Model Files|*.fbx;*.obj;*.dae|All Files|*.*"
            })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    modelViewerControl.LoadModel(ofd.FileName);
                }
            }
        }

        private void LoadGruButton_Click(object sender, EventArgs e)
        {
            // Ensure your form can scroll if content exceeds the visible area
            // (Put this in your constructor or right here)
            this.AutoScroll = true;

            // Clear or reload the model as needed:
            modelViewerControl.LoadModel(gruModelPath);

            int xPos = 226;
            int yPos = 57;
            int w = 335;
            int h = 127;
            int spacing = 40;

            string baseDir = Path.GetDirectoryName(gruModelPath);
            string[] textureFiles =
            {
        Path.Combine(baseDir, "ene_def_body.bmp.png"),
        Path.Combine(baseDir, "ene_def_arm.bmp.png"),
        Path.Combine(baseDir, "ene_def_leg.bmp.png"),
        Path.Combine(baseDir, "ene_def_pa-ka.bmp.png")
    };

            foreach (string texPath in textureFiles)
            {
                // PictureBox
                var pb = new PictureBox
                {
                    Location = new Point(xPos, yPos),
                    Size = new Size(w, h),
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = texPath
                };

                var img = LoadImageNoLock(texPath);
                if (img != null)
                    pb.Image = img;
                else
                    pb.BackColor = Color.DarkRed;

                this.Controls.Add(pb);
                pb.BringToFront();

                // "Change Texture" button, shifted 10px to the right
                var btnChange = new Button
                {
                    Text = "Change Texture",
                    Location = new Point(xPos + 10, yPos + h + 5),
                    Size = new Size(100, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50) // or any dark gray you prefer
                };
                btnChange.Click += ChangeTexture_Click;
                this.Controls.Add(btnChange);
                btnChange.BringToFront();

                // "Restore Default" button, also shifted
                var btnRestore = new Button
                {
                    Text = "Restore Default",
                    Location = new Point(xPos + 120, yPos + h + 5),
                    Size = new Size(110, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnRestore.Click += RestoreOneTextureDefault_Click;
                this.Controls.Add(btnRestore);
                btnRestore.BringToFront();

                // Move down
                yPos += h + 30 + spacing;
            }
        }


        private void ChangeTexture_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            PictureBox pb = (PictureBox)btn.Tag;
            string oldTexPath = pb.Tag.ToString();

            modelViewerControl.ClearModel();
            Thread.Sleep(100);

            string newTexPath = GetNextSuffixPath(oldTexPath);

            using (var dlg = new OpenFileDialog { Filter = "PNG Files|*.png|All Files|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        RenameMtlTextureReference(gruMtlPath,
                            Path.GetFileName(oldTexPath),
                            Path.GetFileName(newTexPath));

                        File.Copy(dlg.FileName, newTexPath, overwrite: true);

                        pb.Tag = newTexPath;
                        pb.Image?.Dispose();
                        pb.Image = LoadImageNoLock(newTexPath);

                        modelViewerControl.LoadModel(gruModelPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error changing texture:\n" + ex.Message);
                    }
                }
            }
        }

        private void RestoreOneTextureDefault_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            PictureBox pb = (PictureBox)btn.Tag;
            string newTexPath = pb.Tag.ToString();
            string oldTexPath = RemoveSuffix(newTexPath);

            try
            {
                modelViewerControl.ClearModel();
                Thread.Sleep(100);

                RenameMtlTextureReference(gruMtlPath,
                    Path.GetFileName(newTexPath),
                    Path.GetFileName(oldTexPath));

                pb.Tag = oldTexPath;
                pb.Image?.Dispose();
                pb.Image = LoadImageNoLock(oldTexPath);

                modelViewerControl.LoadModel(gruModelPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring default:\n" + ex.Message);
            }
        }

        private void RenameMtlTextureReference(string mtlPath, string oldName, string newName)
        {
            if (!File.Exists(mtlPath)) return;
            string text = File.ReadAllText(mtlPath);
            if (text.Contains(oldName))
            {
                text = text.Replace(oldName, newName);
                File.WriteAllText(mtlPath, text);
            }
        }

        private string GetNextSuffixPath(string originalPath)
        {
            string dir = Path.GetDirectoryName(originalPath);
            string fNoExt = Path.GetFileNameWithoutExtension(originalPath);
            string ext = Path.GetExtension(originalPath);

            fNoExt = StripNumericSuffix(fNoExt);

            int i = 1;
            string candidate;
            while (true)
            {
                candidate = Path.Combine(dir, fNoExt + "_" + i + ext);
                if (!File.Exists(candidate)) break;
                i++;
                if (i > 999) throw new IOException("Couldn't find new suffix name up to _999.");
            }
            return candidate;
        }

        private string StripNumericSuffix(string fNoExt)
        {
            int idx = fNoExt.LastIndexOf('_');
            if (idx < 0) return fNoExt;

            string suffix = fNoExt.Substring(idx + 1);
            if (suffix.All(char.IsDigit))
            {
                return fNoExt.Substring(0, idx);
            }
            return fNoExt;
        }

        private string RemoveSuffix(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string fileNoExt = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            fileNoExt = StripNumericSuffix(fileNoExt);
            return Path.Combine(dir, fileNoExt + ext);
        }

        private void RestoreAllMtlReferencesToOriginal(string mtlPath)
        {
            string text = File.ReadAllText(mtlPath);
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (line.StartsWith("map_Kd", StringComparison.OrdinalIgnoreCase))
                {
                    string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2)
                    {
                        string oldFile = parts[1];
                        string dir = Path.GetDirectoryName(Path.Combine(Path.GetDirectoryName(mtlPath), oldFile));
                        string fixedPath = RemoveSuffix(Path.Combine(dir, oldFile));
                        string justFileName = Path.GetFileName(fixedPath);

                        lines[i] = parts[0] + " " + justFileName;
                    }
                }
            }
            text = string.Join(Environment.NewLine, lines);
            File.WriteAllText(mtlPath, text);
        }

        private Image LoadImageNoLock(string path)
        {
            if (!File.Exists(path)) return null;
            using (var fs = File.OpenRead(path))
            {
                return Image.FromStream(fs);
            }
        }
    }
}
