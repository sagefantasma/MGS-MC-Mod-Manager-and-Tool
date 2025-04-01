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
        private string currentModelPath;
        private string currentMtlPath;

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

            ModelSelectionComboBox.Items.Clear();
            ModelSelectionComboBox.Items.Add("MGS3 Snake SE");
            ModelSelectionComboBox.Items.Add("MGS3 GRU");
            ModelSelectionComboBox.Items.Add("MGS3 KGB");
            ModelSelectionComboBox.Items.Add("MGS3 Ocelot Unit");
            ModelSelectionComboBox.Items.Add("MGS3 Officer");
            ModelSelectionComboBox.Items.Add("MGS2 Snake Tanker");
            ModelSelectionComboBox.Items.Add("MGS2 Raiden");
            ModelSelectionComboBox.Items.Add("MGS2 Tanker Guards");
            ModelSelectionComboBox.Items.Add("MGS2 Big Shell Guards");
            ModelSelectionComboBox.Items.Add("MGS2 NYPD");
            ModelSelectionComboBox.SelectedIndex = 0;

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
                    "Modding Tools Folder Location",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (res == DialogResult.Cancel)
                    return false;
                else if (res == DialogResult.No)
                {
                    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
                    {
                        SelectedPath = config.ModToolsPath,
                        Description = "Select a folder where 'MGS Modding Tools' will be stored."
                    })
                    {
                        if (fbd.ShowDialog() == DialogResult.OK)
                            config.ModToolsPath = Path.Combine(fbd.SelectedPath, "MGS Modding Tools");
                        else
                            return false;
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
                RestoreAllMtlReferencesToOriginal(gruMtlPath);
            Application.Exit();
        }

        private void AdjustElementHostSize()
        {
            int halfWidth = this.ClientSize.Width / 2;
            elementHost.Location = new Point(halfWidth, 0);
            elementHost.Size = new Size(halfWidth, this.ClientSize.Height);
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

        private void LoadGruButton_Click(object sender, EventArgs e)
        {
            ConfigSettings config = ConfigManager.LoadSettings();
            panelTextures.Controls.Clear();

            string selectedModel = ModelSelectionComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedModel))
            {
                MessageBox.Show("Please select a model from the dropdown.");
                return;
            }

            string folder = string.Empty;
            string modelFile = string.Empty;
            string mtlFile = string.Empty;
            string[] textureFiles = null;

            if (selectedModel == "MGS3 Snake SE")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Snake SE");
                modelFile = "MGS3 Snake.obj";
                mtlFile = "MGS3 Snake.mtl";
                textureFiles = new string[]
                {
                    "sna_def_olive.bmp.png",
                    "sna_def_hand.bmp.png",
                    "cord_ovl_alp.bmp.png",
                    "cqcc_tex02.bmp.png",
                    "cqck_tex02.bmp.png",
                    "sna_bandana_def.bmp.png",
                    "sna_bk.bmp.png",
                    "sna_def_hair_base.bmp.png",
                    "sna_def_hair_front_ovl_alp.bmp.png",                                     
                    "sna_def_vr_eye.bmp.png",
                    "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430.png",
                    "sna_foot_sp.bmp.png",
                    "sna_foot_vr.bmp.png",
                    "sna_hair_back_ovl_alp.bmp.png",
                    "sna_hair_front_ovl_alp.bmp.png",
                    "sna_hair_layer_ovl_alp.bmp.png",
                    "sna_item_hm.bmp.png",
                    "sna_mgs3_antena.bmp.png",
                    "sna_mgs3_arm.bmp.png",
                    "sna_mgs3_belt.bmp_77380cc45d92a52a1e8da9f59a6ea891.png",
                    "sna_mgs3_belt_side.bmp.png",
                    "sna_mgs3_gh.bmp.png",
                    "sna_mgs3_gun_hol.bmp.png",
                    "sna_mgs3_halo_tape.bmp.png",
                    "sna_mgs3_hh.bmp.png",
                    "sna_mgs3_musen.bmp.png",
                    "sna_mgs3_naked_belt.bmp_387276427ee88d88dbacbd0ae1f73fd7.png",
                    "sna_mgs3_teeth.bmp.png",
                    "sna_mgs3_wl_op.bmp_c4cd1b877fd963681314270df67dbdf8.png",
                    "sna_mgs3_wpl.bmp_c8270b421a11c1d3172eaaa68ef98ee7.png",
                    "sna_mtg_ovl_alp.bmp.png",
                    "sna_snif_def.bmp.png",
                    "svknf_grip.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 GRU")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 GRU");
                modelFile = "ene_defout.obj";
                mtlFile = "ene_defout.mtl";
                textureFiles = new string[]
                {
                    "ene_def_headtop.bmp.png",
                    "ene_def_headunder.bmp.png",
                    "ene_def_neck.bmp.png",
                    "ene_def_eye.bmp.png",
                    "ene_def_pa-ka.bmp.png",
                    "ene_def_body.bmp.png",
                    "ene_def_arm.bmp.png",
                    "ene_def_leg.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 KGB")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 KGB");
                modelFile = "ene_kgb.obj";
                mtlFile = "ene_kgb.mtl";
                textureFiles = new string[]
                {
                    "e_grene-do.bmp.png",
                    "e_musenki.bmp.png",
                    "e_musenki_belt.bmp.png",
                    "ene_flame_boots3.bmp.png",
                    "ene_flame_boots4.bmp.png",
                    "ene_kgb_bag.bmp.png",
                    "ene_kgb_best.bmp.png",
                    "ene_kgb_body.bmp.png",
                    "ene_kgb_boots1.bmp.png",
                    "ene_kgb_boots2.bmp.png",
                    "ene_kgb_eye_open.bmp.png",
                    "ene_kgb_face.bmp.png",
                    "ene_kgb_hand.bmp.png",
                    "ene_kgb_hankachi.bmp.png",
                    "ene_kgb_pa-ka.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Ocelot Unit")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Ocelot Unit");
                modelFile = "ene_spe.obj";
                mtlFile = "ene_spe.mtl";
                textureFiles = new string[]
                {
                    "oce_def_bere.bmp.png",
                    "ene_spe_head.bmp.png",
                    "ene_spe_eye_open.bmp.png",
                    "ene_spe_body.bmp.png",
                    "ene_spe_hand.bmp.png",
                    "ene_spe_body_belt_alp_ovl.bmp.png",
                    "oce_def_body_belt.bmp.png",
                    "ene_spe_dr_magpouch.bmp.png",
                    "ene_spe_dr_magpouch2.bmp.png",
                    "oce_def_grene-do.bmp.png",
                    "ene_spe_stboxmagpouch.bmp.png",
                    "oce_def_makarofu.bmp.png",
                    "ene_spe_sspo-ti.bmp.png",
                    "ene_spe_mizu.bmp.png",
                    "ene_spe_boots1.bmp.png",
                    "ene_spe_boots2_ovl_sub_alp.bmp.png",
                    "ene_spe_boots3.bmp.png",
                    "ene_spe_boots4.bmp.png"
                };
            }
            else if (selectedModel == "MGS3 Officer")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS3 Officer");
                modelFile = "ene_ind.obj";
                mtlFile = "ene_ind.mtl";
                textureFiles = new string[]
                {
                    "ene_ind_head_mark.bmp.png",
                    "ene_ind_head_belt.bmp.png",
                    "ene_ind_hat_def.bmp.png",
                    "ene_ind_head_front.bmp.png",
                    "ene_ind_face_a.bmp.png",
                    "ene_ind_eye_open_b.bmp.png",
                    "ene_ind_body.bmp.png",
                    "ene_ind_body_belt.bmp.png",
                    "ene_ind_body_belt_alp_ovl.bmp.png",
                    "ene_ind_hand.bmp.png",
                    "ene_ind_makarofu.bmp.png",
                    "ene_def_akmagpouch_bag.bmp.png",
                    "e_sspo-ti.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Snake Tanker")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Snake Tanker");
                modelFile = "sna_def.obj";
                mtlFile = "sna_def.mtl";
                textureFiles = new string[]
                {
                    "sna_arm_ovl_sub_alp.bmp.png",
                    "sna_arm_ss.bmp.png",
                    "sna_belt_leg.bmp.png",
                    "sna_body01dt_ovl_sub_alp.bmp.png",
                    "sna_body_ss01dt.bmp.png",
                    "sna_foot01dt.bmp.png",
                    "sna_hair3.bmp.png",
                    "sna_hand1dt.bmp.png",
                    "sna_hand2.bmp.png",
                    "sna_hi_bdn01dt.bmp.png",
                    "sna_hi_eye01dt.bmp.png",
                    "sna_hi_eye_ovl_sub_alp.bmp.png",
                    "sna_hi_face01dt.bmp.png",
                    "sna_hi_face_ovl_sub_alp.bmp.png",
                    "sna_hi_meziri.bmp.png",
                    "sna_leg_ovl_sub_alp.bmp.png",
                    "sna_leg_r01dt.bmp.png",
                    "sna_leg_r01dt_ovl_sub_alp.bmp.png",
                    "sna_leg_ss.bmp.png",
                    "sna_m9_glip.bmp.png",
                    "sna_p1dt.bmp.png",
                    "sna_shoul01dt.bmp.png",
                    "sna_shoul01dt_ovl_sub_alp.bmp.png",
                    "sna_toe01dt.bmp.png",
                    "sna_toe02dt.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Raiden")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Raiden");
                modelFile = "rai_def_mt.obj";
                mtlFile = "rai_def_mt.mtl";
                textureFiles = new string[]
                {
                    "rai_arm_fix.bmp.png",
                    "rai_arm_fix_ovl_sub_alp.bmp.png",
                    "rai_body_01_fix.bmp.png",
                    "rai_body_01_fix_ovl_sub_alp.bmp.png",
                    "rai_body_02_fix.bmp.png",
                    "rai_body_02_fix_ovl_sub_alp.bmp.png",
                    "rai_face_dt01.bmp.png",
                    "rai_finger_fix.bmp.png",
                    "rai_finger_fix_ovl_sub_alp.bmp.png",
                    "rai_foot_fix.bmp.png",
                    "rai_foot_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_01_fix.bmp.png",
                    "rai_hand_01_fix_ovl_sub_alp.bmp.png",
                    "rai_hand_02_fix.bmp.png",
                    "rai_hand_02_fix_ovl_sub_alp.bmp.png",
                    "rai_leg_l_fix.bmp.png",
                    "rai_leg_l_fix_ovl_sub_alp.bmp.png",
                    "rai_leg_r_fix.bmp.png",
                    "rai_leg_r_fix_ovl_sub_alp.bmp.png",
                    "rai_rist_fix.bmp.png",
                    "rai_rist_fix_ovl_sub_alp.bmp.png",
                    "rai_toe01_fix.bmp.png",
                    "rai_toe02_fix.bmp.png",
                    "rai_toe_01_fix_ovl_sub_alp.bmp.png",
                    "rai_toe_02_fix_ovl_sub_alp.bmp.png",
                    "rai_watch_dt01.bmp.png",
                    "rai_watch_dt01_ovl_sub_alp.bmp.png",
                    "rai_watch_dt02.bmp.png",
                    "rai_watch_dt02_ovl_sub_alp.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Tanker Guards")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Tanker Guards");
                modelFile = "gbs_def.obj";
                mtlFile = "gbs_def.mtl";
                textureFiles = new string[]
                {
                    "gbs_head_top.bmp.png",
                    "gbs_head_side.bmp.png",
                    "gbs_head_back.bmp.png",
                    "gbs_eye_open.bmp.png",
                    "gbs_eri.bmp.png",
                    "gbs_body_ss.bmp.png",
                    "gbs_arm_ss.bmp.png",
                    "gbs_wrist.bmp.png",
                    "gbs_p2_fro.bmp.png",
                    "gbs_p2_side.bmp.png",
                    "gbs_belt.bmp.png",
                    "gbs_leg_ss.bmp.png",
                    "gbs_boot_ss.bmp.png",
                    "gbs_boot_bot1.bmp.png",
                    "gbs_boot_bot2.bmp.png",
                    "gbs_toe2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 Big Shell Guards")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 Big Shell Guards");
                modelFile = "gps_def_mt.obj";
                mtlFile = "gps_def_mt.mtl";
                textureFiles = new string[]
                {
                    "gps_mask.bmp.png",
                    "gps_eye_def.bmp.png",
                    "gps_eri.bmp.png",
                    "gps_arm_ss2.bmp.png",
                    "gps_body_ss.bmp.png",
                    "gps_leg_ss.bmp.png",
                    "gps_poket.bmp.png",
                    "gps_poket2.bmp.png",
                    "gps_boot_ss.bmp.png",
                    "gps_toe2.bmp.png"
                };
            }
            else if (selectedModel == "MGS2 NYPD")
            {
                folder = Path.Combine(config.Assets.ModelsAndTexturesFolder, "MGS2 NYPD");
                modelFile = "NYPD (Armed).obj";
                mtlFile = "NYPD (Armed).mtl";
                textureFiles = new string[]
                {
                    "nyp_armd_body.bmp.png",
                    "nyp_armd_face.bmp.png",
                    "nyp_armd_helmet.bmp.png",
                    "nyp_armd_police.bmp.png",
                    "nypo_arm.bmp.png",
                    "nypo_emblem.bmp.png",
                    "nypo_folder.bmp.png",
                    "nypo_hand.bmp.png",
                    "nypo_leg.bmp.png",
                    "nypo_shoe.bmp.png",
                    "nypo_unit.bmp.png"
                };
            }
            else
            {
                MessageBox.Show("Unknown model selected.");
                return;
            }

            string folderPath = folder;
            currentModelPath = Path.Combine(folderPath, modelFile);
            currentMtlPath = Path.Combine(folderPath, mtlFile);
            if (File.Exists(currentMtlPath))
            {
                RestoreAllMtlReferencesToOriginal(currentMtlPath);
            }
            modelViewerControl.LoadModel(currentModelPath);

            int w = 335, h = 127, xPos = panelTextures.ClientSize.Width - w - 10 - 30, yPos = 10, spacing = 40;
            int labelHeight = 20;
            foreach (string tex in textureFiles)
            {
                string texPath = Path.Combine(folderPath, tex);

                Label lbl = new Label
                {
                    Text = Path.GetFileNameWithoutExtension(tex),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(30, 30, 30),
                    Location = new Point(xPos, yPos),
                    Size = new Size(w, labelHeight)
                };
                panelTextures.Controls.Add(lbl);

                PictureBox pb = new PictureBox
                {
                    Location = new Point(xPos, yPos + labelHeight),
                    Size = new Size(w, h),
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = texPath
                };

                Image img = LoadImageNoLock(texPath);
                if (img != null)
                {
                    if (img.Width < pb.Width && img.Height < pb.Height)
                    {
                        pb.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        pb.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    pb.Image = img;
                }
                else
                {
                    pb.BackColor = Color.DarkRed;
                }
                panelTextures.Controls.Add(pb);

                Button btnChange = new Button
                {
                    Text = "Change Texture",
                    Location = new Point(xPos, yPos + labelHeight + h + 5),
                    Size = new Size(100, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnChange.Click += ChangeTexture_Click;
                panelTextures.Controls.Add(btnChange);

                Button btnRestore = new Button
                {
                    Text = "Restore Default",
                    Location = new Point(xPos + 110, yPos + labelHeight + h + 5),
                    Size = new Size(110, 30),
                    Tag = pb,
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(50, 50, 50)
                };
                btnRestore.Click += RestoreOneTextureDefault_Click;
                panelTextures.Controls.Add(btnRestore);

                yPos += labelHeight + h + 30 + spacing;
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

            using (OpenFileDialog dlg = new OpenFileDialog { Filter = "PNG Files|*.png|All Files|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        RenameMtlTextureReference(currentMtlPath,
                            Path.GetFileName(oldTexPath),
                            Path.GetFileName(newTexPath));

                        File.Copy(dlg.FileName, newTexPath, true);

                        pb.Tag = newTexPath;
                        pb.Image?.Dispose();
                        pb.Image = LoadImageNoLock(newTexPath);

                        modelViewerControl.LoadModel(currentModelPath);
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

                RenameMtlTextureReference(currentMtlPath,
                    Path.GetFileName(newTexPath),
                    Path.GetFileName(oldTexPath));

                pb.Tag = oldTexPath;
                pb.Image?.Dispose();
                pb.Image = LoadImageNoLock(oldTexPath);

                modelViewerControl.LoadModel(currentModelPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error restoring default:\n" + ex.Message);
            }
        }

        private void RenameMtlTextureReference(string mtlPath, string oldName, string newName)
        {
            if (!File.Exists(mtlPath))
                return;
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
                if (!File.Exists(candidate))
                    break;
                i++;
                if (i > 999)
                    throw new IOException("Couldn't find new suffix name up to _999.");
            }
            return candidate;
        }

        private string StripNumericSuffix(string fNoExt)
        {
            int idx = fNoExt.LastIndexOf('_');
            if (idx < 0)
                return fNoExt;
            string suffix = fNoExt.Substring(idx + 1);
            if (suffix.All(char.IsDigit))
                return fNoExt.Substring(0, idx);
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
            if (!File.Exists(path))
                return null;
            using (var fs = File.OpenRead(path))
            {
                return Image.FromStream(fs);
            }
        }

        private void CtxrToPng_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing CTXR files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.ctxr");
                        foreach (string ctxrPath in files)
                        {
                            string pngPath = Path.ChangeExtension(ctxrPath, ".png");
                            try
                            {
                                CtxrConverter.CtxrToPng(ctxrPath, pngPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {ctxrPath}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
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

            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing PNG files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.png");
                        foreach (string pngPath in files)
                        {
                            try
                            {
                                CtxrConverter.PngToCtxr(config.ModToolsPath, texconvExe, pngPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {pngPath}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
            {
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

            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing PNG files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.png");
                        foreach (string inputPng in files)
                        {
                            string outputDds = Path.ChangeExtension(inputPng, ".dds");
                            try
                            {
                                CtxrConverter.PngToDdsWithTexconv(texconvExe, inputPng, outputDds);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {inputPng}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
            {
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

            DialogResult dr = MessageBox.Show(
                "Convert full folder? (Yes = Folder conversion, No = Single file conversion)",
                "Conversion Mode",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Cancel)
                return;

            if (dr == DialogResult.Yes)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.Description = "Select the folder containing DDS files";
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = fbd.SelectedPath;
                        string[] files = Directory.GetFiles(folderPath, "*.dds");
                        foreach (string ddsPath in files)
                        {
                            try
                            {
                                CtxrConverter.DdsToCtxr(ddsPath, ctxrToolExe);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error converting {ddsPath}: {ex.Message}");
                            }
                        }
                        MessageBox.Show($"Converted {files.Length} files in folder:\n{folderPath}");
                    }
                }
            }
            else
            {
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

        private void BackButton_Click(object sender, EventArgs e)
        {
            ReturnToMainMenu();
        }
    }
}
