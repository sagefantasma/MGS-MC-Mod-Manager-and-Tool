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
        private Panel panelTextures;

        public TextureModelForm()
        {
            InitializeComponent();

            this.MinimumSize = new Size(800, 600);
            this.BackColor = Color.Black;
            this.BackgroundImage = null;
            this.BackgroundImageLayout = ImageLayout.None;

            // We fix the .mtl references on load & close
            this.Load += TextureModelForm_Load;
            this.FormClosing += TextureModelForm_FormClosing;

            // 1) Create a scrollable panel on the left half
            panelTextures = new Panel
            {
                Name = "panelTextures",
                AutoScroll = true,
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30) // a dark gray if you want
            };
            this.Controls.Add(panelTextures);

            // 2) If you still want a DdsFilePictureBox somewhere, do so – but let's remove it
            //    if it was overshadowing the panel or wasn't needed.
            //    Otherwise, keep reading below for alternative placements.

            // 3) The Helix 3D viewer on the right side
            elementHost = new ElementHost
            {
                Name = "elementHost3D",
                Dock = DockStyle.None,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };
            modelViewerControl = new ModelViewerControl();
            elementHost.Child = modelViewerControl;
            this.Controls.Add(elementHost);
            elementHost.BringToFront();

            // Adjust layout on resize
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
            // Load the 3D model first
            modelViewerControl.LoadModel(gruModelPath);

            // Clear old controls, in case user clicked again
            panelTextures.Controls.Clear();

            // The size of each PictureBox
            int w = 335;
            int h = 127;

            // We want to place them near the right edge of the panel
            // (10 px from the panel’s right border)
            int xPos = panelTextures.ClientSize.Width - w - 10;
            int yPos = 10;
            int spacing = 40;

            // The texture files
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
                // 1) Create the PictureBox
                var pb = new PictureBox
                {
                    Location = new Point(xPos, yPos),
                    Size = new Size(w, h),
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Tag = texPath
                };

                // Load the texture (no file lock)
                var img = LoadImageNoLock(texPath);
                if (img != null)
                    pb.Image = img;
                else
                    pb.BackColor = Color.DarkRed;

                // Add to the panel
                panelTextures.Controls.Add(pb);

                // 2) "Change Texture" Button below the PictureBox
                var btnChange = new Button
                {
                    Text = "Change Texture",
                    Location = new Point(xPos, yPos + h + 5),
                    Size = new Size(100, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnChange.Click += ChangeTexture_Click;
                panelTextures.Controls.Add(btnChange);

                // 3) "Restore Default" Button next to it
                var btnRestore = new Button
                {
                    Text = "Restore Default",
                    Location = new Point(xPos + 110, yPos + h + 5),
                    Size = new Size(110, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnRestore.Click += RestoreOneTextureDefault_Click;
                panelTextures.Controls.Add(btnRestore);

                // 4) Move yPos down for the next group
                // PictureBox height + button row (30) + some spacing
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

        private void CtxrToPng_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a CTXR file";
                ofd.Filter = "CTXR Files|*.ctxr";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ctxrPath = ofd.FileName;
                    // build a .png path next to the .ctxr
                    string pngPath = Path.ChangeExtension(ctxrPath, ".png");

                    try
                    {
                        CtxrConverter.CtxrToPng(ctxrPath, pngPath);
                        MessageBox.Show($"Converted:\n{ctxrPath}\n→\n{pngPath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void PngToCtxr_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a PNG file";
                ofd.Filter = "PNG Files|*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string pngPath = ofd.FileName;
                    // same folder, same base name, .ctxr extension
                    string ctxrPath = Path.ChangeExtension(pngPath, ".ctxr");

                    try
                    {
                        CtxrConverter.PngToCtxr(pngPath, ctxrPath);
                        MessageBox.Show($"Converted:\n{pngPath}\n→\n{ctxrPath}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void PngToDds_Click(object sender, EventArgs e)
        {
            // Hard-coded texconv path for testing:
            string texconvExe = @"C:\Users\Mitch\Downloads\texconv.exe";

            using (var ofd = new OpenFileDialog { Filter = "PNG Files|*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string inputPng = ofd.FileName;
                    // For example, just replace extension with .dds
                    string outputDds = Path.ChangeExtension(inputPng, ".dds");

                    try
                    {
                        CtxrConverter.PngToDdsWithTexconv2024(texconvExe, inputPng, outputDds);
                        MessageBox.Show($"DDS created:\n{outputDds}");
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void DdsToCtxr_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "DDS Files|*.dds" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ddsPath = ofd.FileName;
                    try
                    {
                        // Call the updated method which runs CtxrTool.exe in the DDS file's folder.
                        CtxrConverter.DdsToCtxr(ddsPath);
                        MessageBox.Show("CTXR created successfully in the same folder as the DDS file.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error converting DDS to CTXR: {ex.Message}");
                    }
                }
            }
        }
    }
}