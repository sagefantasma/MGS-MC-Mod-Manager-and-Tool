using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class TextureModelForm : Form
    {
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
            this.Load += TextureModelForm_Load;
            this.FormClosing += TextureModelForm_FormClosing;

            panelTextures = new Panel
            {
                Name = "panelTextures",
                AutoScroll = true,
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width / 2, this.ClientSize.Height),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 30)
            };
            this.Controls.Add(panelTextures);

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
            AdjustElementHostSize();
        }

        private async void TextureModelForm_Load(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();

            string gruAssetsFolder = Path.Combine(config.Assets.ModelsAndTexturesFolder, config.Assets.FolderMapping["GRU"]);

            gruModelPath = Path.Combine(gruAssetsFolder, "ene_defout.obj");
            gruMtlPath = Path.Combine(gruAssetsFolder, "ene_defout.mtl");

            if (File.Exists(gruMtlPath))
            {
                RestoreAllMtlReferencesToOriginal(gruMtlPath);
            }

            this.BeginInvoke(new Action(async () =>
            {
                if (!CheckAndPromptForModToolsPath(config))
                {
                    ReturnToMainMenu();
                    this.Hide();
                    return;
                }
                DownloadManager dm = new DownloadManager();
                try
                {
                    await dm.EnsureModToolsDownloaded(config.ModToolsPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading mod tools: " + ex.Message);
                    ReturnToMainMenu();
                    this.Hide();
                    return;
                }
            }));
        }

        private bool CheckAndPromptForModToolsPath(ConfigSettings config)
        {
            if (!config.ModToolsFolderSet)
            {
                DialogResult res = MessageBox.Show(
                    "Before using the modding tools, we need to set up a folder where the required tools will be stored.\n\n" +
                    "Do you want to use the default location?\n\nDefault location:\n" + config.ModToolsPath,
                    "Modding Tools Folder Location \n\n After a location is picked the tools and files will be downloaded and extracted into that folder.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (res == DialogResult.Cancel)
                {
                    return false;
                }
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.ModToolsPath,
                        Description = "Select a folder where 'MGS Modding Tools' will be stored."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                        {
                            config.ModToolsPath = Path.Combine(fbd.SelectedPath, "MGS Modding Tools");
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                config.ModToolsFolderSet = true;
                ConfigManager.SaveSettings(config);
            }
            return true;
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

        
        private void AdjustElementHostSize()
        {
            int halfWidth = this.ClientSize.Width / 2;
            elementHost.Location = new Point(halfWidth, 0);
            elementHost.Size = new Size(halfWidth, this.ClientSize.Height);
        }     

        private void BackButton_Click(object sender, EventArgs e)
        {
            ReturnToMainMenu();
        }

        private void ReturnToMainMenu()
        {
            LoggingManager.Instance.Log("Going back to Main Menu from Texture and 3D Model form.\n");
            GuiManager.UpdateLastFormLocation(this.Location);
            GuiManager.LogFormLocation(this, "TextureModelForm");
            MainMenuForm mainMenuForm = new MainMenuForm();
            mainMenuForm.Show();
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
            modelViewerControl.LoadModel(gruModelPath);
            panelTextures.Controls.Clear();

            int w = 335;
            int h = 127;
            int xPos = panelTextures.ClientSize.Width - w - 10;
            int yPos = 10;
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

                panelTextures.Controls.Add(pb);

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

        private async void PngToCtxr_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();

            if (!CheckAndPromptForModToolsPath(config))
            {
                MessageBox.Show("Mod tools folder setup was cancelled.");
                return;
            }

            DownloadManager dm = new DownloadManager();
            await dm.EnsureModToolsDownloaded(config.ModToolsPath);

            string texconvExe = Path.Combine(config.ModToolsPath, "texconv.exe");

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select a PNG file";
                ofd.Filter = "PNG Files|*.png";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string pngPath = ofd.FileName;
                    try
                    {
                        CtxrConverter.PngToCtxr(config.ModToolsPath, texconvExe, pngPath);
                        MessageBox.Show("PNG successfully converted to CTXR in the same folder as the PNG.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error converting PNG to CTXR: {ex.Message}");
                    }
                }
            }
        }

        private async void PngToDds_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            if (!CheckAndPromptForModToolsPath(config))
            {
                MessageBox.Show("Mod tools folder setup was cancelled.");
                return;
            }

            DownloadManager dm = new DownloadManager();
            await dm.EnsureModToolsDownloaded(config.ModToolsPath);
            string texconvExe = Path.Combine(config.ModToolsPath, "texconv.exe");
            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "PNG Files|*.png" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string inputPng = ofd.FileName;
                    string outputDds = Path.ChangeExtension(inputPng, ".dds");
                    try
                    {
                        CtxrConverter.PngToDdsWithTexconv(texconvExe, inputPng, outputDds);
                        MessageBox.Show($"DDS created:\n{outputDds}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error converting PNG to DDS: {ex.Message}");
                    }
                }
            }
        }

        private void DdsToCtxr_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            if (!CheckAndPromptForModToolsPath(config))
            {
                MessageBox.Show("Mod tools folder setup was cancelled.");
                return;
            }

            string modToolsPath = config.ModToolsPath;
            string ctxrToolExe = Path.Combine(modToolsPath, "CtxrTool.exe");

            using (OpenFileDialog ofd = new OpenFileDialog { Filter = "DDS Files|*.dds" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string ddsPath = ofd.FileName;
                    try
                    {
                        CtxrConverter.DdsToCtxr(ddsPath, ctxrToolExe);
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