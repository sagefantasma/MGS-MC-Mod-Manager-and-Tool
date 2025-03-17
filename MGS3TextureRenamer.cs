using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using IOSearchOption = System.IO.SearchOption;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public static class MGS3TextureRenamer
    {
        #region Static Mapping Dictionaries

        public static readonly Dictionary<string, string> CamoMappings = new Dictionary<string, string>()
    {
    { "sna_def_olive.bmp", "Olive Drab" },
    { "sna_def_olive.bmp_fddc17abd35f6bd4bcfe075aa87fcd3d", "Tiger Stripe" },
    { "sna_def_olive.bmp_c9b5db71e3bb16a2462c4ad6d16643a6", "Leaf" },
    { "sna_def_olive.bmp_9af6aaef5b609776c8e303d94ab315b0", "Tree Bark" },
    { "sna_def_olive.bmp_1e8fc08d5b630dfe69f34753272d8373", "Choco Chip" },
    { "sna_def_olive.bmp_d1074941f11b44c7bc252191a3af0061", "Splitter" },
    { "sna_def_olive.bmp_6b97ee6a2a4b329361fa5a0d882cd8aa", "Raindrop" },
    { "sna_def_olive.bmp_8a7087c7c87848f2d0a0aa81a5398d63", "Squares" },
    { "sna_def_olive.bmp_b38b6218ff8fa62af178fe6218ed1e0a", "Water" },
    { "sna_def_olive.bmp_4fe34e9fb94333e54a96678c83539a6a", "Black" },
    { "sna_def_olive.bmp_e4294d43ca30e584d679b3175ebea12f", "Snow" },
    { "sna_def_olive.bmp_66e33477ee309989a8edabf630709b1f", "Hornet Stripe" },
    { "sna_def_olive.bmp_80b56de647570ad2aea1f3d87ce70d27", "Spider" },
    { "sna_def_olive.bmp_8376e598d8c57af4a0eb4f3fd60a016f", "Moss" },
    { "sna_def_olive.bmp_fb7533906ae772bfe0412dad5c8f1b2c", "Fire" },
    { "sna_def_olive.bmp_c24ea2e0e48eee993d85e497dc482559", "Spirit" },
    { "sna_def_olive.bmp_6944dfbf2b7396ecd6c6b4eae01573c9", "Cold War" },
    { "sna_def_olive.bmp_4ed6ab75380ef132068c07def42791b2", "Snake" },
    { "sna_def_olive.bmp_57799a742ee247d19fecd7117747eb4c", "GA-KO" },
    { "sna_def_olive.bmp_5cde5a23f56142e48a6daea0749d27b6", "Desert Tiger" },
    { "sna_def_olive.bmp_c93f85099829dd9ea31bea8b0ed00ceb", "DPM" },
    { "sna_def_olive.bmp_3143cadcb73aac1bbd968a2b5d153f86", "Flecktarn" },
    { "sna_def_olive.bmp_3613882f043e61b306da380d95f00e56", "Auscam" },
    { "sna_def_olive.bmp_69a080e9a1fbf01b5815160fd59db213", "Animals" },
    { "sna_def_olive.bmp_6ad901c21d969ba685fb19576d166134", "Fly" },
    { "sna_def_olive.bmp_250c64d61d6d43eebe785ba570084310", "Banana" },
    { "00765dfa-sna_def_olive~cd4b56738f492778c2200038dc1286dd", "Anubis" },
    { "00765dfa-sna_def_olive~86d6763dde8ecfc17f0757fd8e4ef306", "Barracuda" },
    { "00765dfa-sna_def_olive~e13e0de15eb8f17eb8b7cdcc907a8c73", "Urban Tiger" },
    { "00765dfa-sna_def_olive~5b3d1598f27e95be7e3c4707df16c674", "Chameleon" },
    { "00765dfa-sna_def_olive~25834abebf04d0988daad5bf2794029f", "Dododo" },
    { "00765dfa-sna_def_olive~d911c904e6c6ec906da9f684a40302be", "East Germany" },
    { "00765dfa-sna_def_olive~bf83de4607fa1858fee936ce855f1bf2", "Festival" },
    { "00765dfa-sna_def_olive~a921b3a31002786d9ab96ce815775207", "Flower" },
    { "sna_def_olive.bmp_429f87cee87483c5dfd68a8483db6a88", "Grenade" },
    { "00765dfa-sna_def_olive~226720d60bf32248d4b2ff4d91332ff5", "KLMK" },
    { "sna_def_olive.bmp_34ca265f5efeb803459e00e7bba3ec76", "Mummy" },
    { "00765dfa-sna_def_olive~3899b0451a9bf3a066c06607da896be5", "Night Desert" },
    { "00765dfa-sna_def_olive~0063d2755986c53e88ca8238dcc91517", "Rainbow" },
    { "00765dfa-sna_def_olive~595bccc37f7fd4e109580710c4e05ace", "Rock" },
    { "00765dfa-sna_def_olive~11b6c529be02c7be258feb2b9c0d765b", "Santa" },
    { "00765dfa-sna_def_olive~aa50c1cc5d6b4a104c9f98591f5fe392", "Swamp" },
    { "00765dfa-sna_def_olive~9d26662c63d25efb0d45047f2138adde", "Soviet Woodland" },
    { "00765dfa-sna_def_olive~e5992dad6ebeda010e8d35d02c628c98", "St.Valentines" },
    { "00765dfa-sna_def_olive~d4d384d014eaf51ba3c47999300d13b6", "West Germany" },
    { "00765dfa-sna_def_olive~fe6f8be43a9b674bd5151a36957fbfa8", "Watersnake" },
    };

        public static readonly Dictionary<string, string> FaceMappingsWithBandana = new Dictionary<string, string>()
    {
    { "0003a157.img_938598d4a616f43eebbcf7870bb3a641", "Black With Bandana" },
    { "0003a157.img", "Brown With Bandana" },
    { "0003a157.img_1ba470027dacd7c10adf661610dc04e1", "Desert With Bandana" },
    { "0003a157.img_1ece3f27cf607206241327a91b57c89d", "France With Bandana" },
    { "0003a157.img_8a1bf389847c5277575f65a87d707776", "Germany With Bandana" },
    { "0003a157.img_2be35633bee586cebc75a438f0e6e4e2", "Green With Bandana" },
    { "0003a157.img_e24452b395efe665c10dd879c9efd64", "Infinity With Bandana" },
    { "0003a157.img_42c8b0bc2f3455093d90a25729ed91b4", "Italy With Bandana" },
    { "0003a157.img_f327b39ad21edb1d0512ebe14c8315c5", "Japan With Bandana" },
    { "0003a157.img_4ca343a31ddc4dfa5528cda5ec7f111b", "Kabuki With Bandana" },
    { "sna_face_def.bmp", "No Paint With Bandana" },
    { "0003a157.img_bcf56bfe3db15abfc750268e7f5174c7", "Oyama With Bandana" },
    { "0003a157.img_4f9b6c4f12bbb06f5abff743fcd1085c", "Snow With Bandana" },
    { "0003a157.img_68d061fb998272c35600430d51659598", "Spain With Bandana" },
    { "0003a157.img_335003ac512d622e8ff81548074823df", "Splitter With Bandana" },
    { "0003a157.img_185cf90c2eaa2eef3aeb870ec4834671", "Soviet With Bandana" },
    { "0003a157.img_cb75a58f0c068f44d888ce45e65894af", "Sweden With Bandana" },
    { "0003a157.img_f3a959460eba4c009e7c4033581abb86", "UK With Bandana" },
    { "0003a157.img_8ce501972e9b7908be5585cdfc2be0a4", "USA With Bandana" },
    { "0003a157.img_4b9c199f7524b4531a399cdcae08fec1", "Water With Bandana" },
    { "0003a157.img_072734442395aa77b4dc73a300e14635", "Woodland With Bandana" },
    { "0003a157.img_079c21f87c2396e85d5f7c82637e7352", "Zombie With Bandana" },
    };

        public static readonly Dictionary<string, string> FaceMappingsNoBandana = new Dictionary<string, string>()
    {
    { "0003a157.img_f86a2cbae5c969c4f37a6ef62ff21901", "Black No Bandana" },
    { "0003a157.img_fe7cce545d819c784fd506cca4c9d7d8", "Brown No Bandana" },
    { "0003a157.img_89fb46309dec3e73cf0af435d75612bc", "Desert No Bandana" },
    { "0003a157.img_f0a023edf77daaaa485f569e28b23488", "France No Bandana" },
    { "0003a157.img_e4856e14c95d0d7d93a4ef11546df433", "Germany No Bandana" },
    { "0003a157.img_4344dd0f107c82e1ab241f4f84165e45", "Green No Bandana" },
    { "0003a157.img_f338b1aa17146d40502eb55896857452", "Infinity No Bandana" },
    { "0003a157.img_8e47400085fdb32a4b0d7074929174f7", "Italy No Bandana" },
    { "0003a157.img_70d31f246558c4f580b0374defb6bdb2", "Japan No Bandana" },
    { "0003a157.img_3175e6874d78a2e57eb432e68c004751", "Kabuki No Bandana" },
    { "sna_face_def.bmp_bbe58170874ef112ad7f8269143d4430", "No Paint No Bandana" },
    { "0003a157.img_e88c0cb5c7f3449120f1f1f62980cdf0", "Oyama No Bandana" },
    { "0003a157.img_b8e21cf55f0d808db1b31817da1b8d12", "Snow No Bandana" },
    { "0003a157.img_b5f0a7ccc931259672c395e02daf5c4a", "Spain No Bandana" },
    { "0003a157.img_f4d589d12767497bfeb21185f0ec710b", "Splitter No Bandana" },
    { "0003a157.img_03f17ff9395846ea5bf936552c3b988a", "Soviet No Bandana" },
    { "0003a157.img_5667dcc8ac21297c981a8ec40fbe8c19", "Sweden No Bandana" },
    { "0003a157.img_c5891bd5da126da6f5e76224cbcd00b0", "UK No Bandana" },
    { "0003a157.img_40340683d6a287e2af39d3760e2162ee", "USA No Bandana" },
    { "0003a157.img_5aab7ee976ca7144ad954ec48d7f93a4", "Water No Bandana" },
    { "0003a157.img_7f44ac4b5710b0a4e618d21d87b63de1", "Woodland No Bandana" },
    { "0003a157.img_56df4e5f35dee2941e00cbdcacd04fd1", "Zombie No Bandana" },
    };

        public static readonly Dictionary<string, string> BoxMappings = new Dictionary<string, string>()
        {
        { "cbox_a.bmp", "Cardboard Box A" },
        { "cbox_b.bmp", "Cardboard Box B" },
        { "cbox_c.bmp", "Cardboard Box C" },
        };
        public static readonly Dictionary<string, string> ReverseCamoMappings =
            CamoMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static readonly Dictionary<string, string> ReverseFaceMappingsWithBandana =
    FaceMappingsWithBandana.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static readonly Dictionary<string, string> FaceMappings =
            FaceMappingsWithBandana
            .Concat(FaceMappingsNoBandana)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static readonly Dictionary<string, string> ReverseFaceMappings =
            FaceMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        public static readonly Dictionary<string, string> ReverseFaceMappingsNoBandana =
            FaceMappingsNoBandana.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);


        public static readonly Dictionary<string, string> ReverseBoxMappings =
            BoxMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        #endregion

        #region Public Methods (Dialogs)

        public static void ShowSingleCamoSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            string originalSingleCamoFile,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            if (!TryGetSingleCamoInModFolder(modPath, out string currentSingleCamoFile))
            {
                MessageBox.Show("Could not find the current camo file in the mod folder.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileNameNoExt = Path.GetFileNameWithoutExtension(currentSingleCamoFile);
            string currentCamoFriendlyName = CamoMappings.TryGetValue(fileNameNoExt, out string friendly)
                                                ? friendly : fileNameNoExt;

            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);

            Form dialog = new Form
            {
                Width = 400,
                Height = 200,
                Text = "Change Camo",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Label label = new Label
            {
                Text = $"Currently Replacing: {currentCamoFriendlyName}",
                Left = 20,
                Top = 20,
                AutoSize = true,
                ForeColor = Color.Black,
                BackColor = themeColor,
                Font = dialogFont
            };
            dialog.Controls.Add(label);

            ComboBox combo = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor
            };
            combo.Items.AddRange(CamoMappings.Values.ToArray());
            combo.SelectedItem = currentCamoFriendlyName;
            dialog.Controls.Add(combo);

            Button okButton = new Button
            {
                Text = "OK",
                Left = 280,
                Top = 120,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 170,
                Top = 120,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string newFriendlyName = combo.SelectedItem.ToString();
            if (newFriendlyName == currentCamoFriendlyName)
                return;

            if (!ReverseCamoMappings.TryGetValue(newFriendlyName, out string newFileNameNoExt))
            {
                MessageBox.Show($"Could not find an internal name for {newFriendlyName}.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            string folder = Path.GetDirectoryName(currentSingleCamoFile);
            string extension = Path.GetExtension(currentSingleCamoFile);
            string newFullPath = Path.Combine(folder, newFileNameNoExt + extension);
            if (File.Exists(newFullPath))
            {
                try { File.Delete(newFullPath); }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting existing file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                File.Move(currentSingleCamoFile, newFullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renaming camo:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string oldFileName = Path.GetFileName(currentSingleCamoFile);
            string newFileName = newFileNameNoExt + extension;
            if (config.Mods.ModMappings.ContainsKey(modName))
            {
                foreach (var mapping in config.Mods.ModMappings[modName])
                {
                    if (Path.GetFileName(mapping.ModFile).Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        mapping.ModFile = newFileName;
                        mapping.TargetPath = newFileName;
                    }
                }
            }
            if (config.Mods.ReplacedFiles.ContainsKey(modName))
            {
                for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                {
                    if (config.Mods.ReplacedFiles[modName][i].Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        config.Mods.ReplacedFiles[modName][i] = newFileName;
                    }
                }
            }
            ConfigManager.SaveSettings(config);

            string modDetailsPath = Path.Combine(modPath, "Mod Details");
            if (!Directory.Exists(modDetailsPath))
                Directory.CreateDirectory(modDetailsPath);
            string modInfoPath = Path.Combine(modDetailsPath, "Mod Info.txt");
            string infoLine = $"This mod is currently replacing the {newFriendlyName} Camo";
            if (!File.Exists(modInfoPath))
                File.WriteAllText(modInfoPath, infoLine);
            else
                ReplaceOrAppendModInfoLine(modInfoPath, infoLine);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show($"Camo changed to {newFriendlyName}.\n" +
                (wasInstalled ? "Mod was reinstalled to apply changes." : "Changes will apply next time you install this mod."),
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowMultiBoxSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            List<string> boxFiles,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);

            Form dialog = new Form
            {
                Width = 500,
                Height = 400,
                Text = "Change Boxes",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250,
                AutoScroll = true,
                BackColor = themeColor
            };
            dialog.Controls.Add(panel);

            List<(string filePath, ComboBox combo, string currentFriendly)> rows = new List<(string, ComboBox, string)>();
            int y = 10;
            foreach (string file in boxFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string currentFriendly = BoxMappings.ContainsKey(fileNameNoExt) ? BoxMappings[fileNameNoExt] : fileNameNoExt;

                Label lbl = new Label
                {
                    Text = $"Current: {currentFriendly}",
                    Left = 10,
                    Top = y,
                    Width = 200,
                    ForeColor = Color.Black,
                    BackColor = themeColor,
                    Font = dialogFont
                };
                panel.Controls.Add(lbl);

                ComboBox combo = new ComboBox
                {
                    Left = 220,
                    Top = y,
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dialogFont,
                    ForeColor = Color.Black,
                    BackColor = themeColor
                };
                combo.Items.AddRange(BoxMappings.Values.ToArray());
                combo.SelectedItem = currentFriendly;
                panel.Controls.Add(combo);

                rows.Add((file, combo, currentFriendly));
                y += 30;
            }
            dialog.Controls.Add(panel);

            Button okButton = new Button
            {
                Text = "OK",
                Left = dialog.ClientSize.Width - 220,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = dialog.ClientSize.Width - 110,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            foreach (var row in rows)
            {
                string newFriendlyName = row.combo.SelectedItem.ToString();
                if (newFriendlyName == row.currentFriendly)
                    continue;
                if (!ReverseBoxMappings.TryGetValue(newFriendlyName, out string newFileNameNoExt))
                {
                    MessageBox.Show($"Could not find an internal name for {newFriendlyName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                string currentFilePath = row.filePath;
                string folder = Path.GetDirectoryName(currentFilePath);
                string extension = Path.GetExtension(currentFilePath);
                string newFullPath = Path.Combine(folder, newFileNameNoExt + extension);

                if (File.Exists(newFullPath))
                {
                    try { File.Delete(newFullPath); }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting existing file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                try
                {
                    File.Move(currentFilePath, newFullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming box:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                string oldFileName = Path.GetFileName(currentFilePath);
                string newFileName = newFileNameNoExt + extension;
                if (config.Mods.ModMappings.ContainsKey(modName))
                {
                    foreach (var mapping in config.Mods.ModMappings[modName])
                    {
                        if (mapping.ModFile.Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            mapping.ModFile = newFileName;
                            mapping.TargetPath = newFileName;
                        }
                    }
                }
                if (config.Mods.ReplacedFiles.ContainsKey(modName))
                {
                    for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                    {
                        if (config.Mods.ReplacedFiles[modName][i].Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            config.Mods.ReplacedFiles[modName][i] = newFileName;
                        }
                    }
                }
            }
            ConfigManager.SaveSettings(config);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show("Boxes updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowMultiCamoSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            List<string> camoFiles,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);
            Form dialog = new Form
            {
                Width = 500,
                Height = 400,
                Text = "Change Camos",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 300,
                AutoScroll = true,
                BackColor = themeColor
            };
            dialog.Controls.Add(panel);

            var rows = new List<(string filePath, ComboBox combo, string currentFriendly)>();
            int y = 10;
            foreach (string file in camoFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string currentFriendly = CamoMappings.TryGetValue(fileNameNoExt, out string friendly)
                    ? friendly : fileNameNoExt;

                Label lbl = new Label
                {
                    Text = $"Current: {currentFriendly}",
                    Left = 10,
                    Top = y,
                    Width = 200,
                    ForeColor = Color.Black,
                    BackColor = themeColor,
                    Font = dialogFont
                };
                panel.Controls.Add(lbl);

                ComboBox combo = new ComboBox
                {
                    Left = 220,
                    Top = y,
                    Width = 220,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dialogFont,
                    ForeColor = Color.Black,
                    BackColor = themeColor
                };
                combo.Items.AddRange(CamoMappings.Values.ToArray());
                combo.SelectedItem = currentFriendly;
                panel.Controls.Add(combo);

                rows.Add((file, combo, currentFriendly));
                y += 35;
            }

            Button okButton = new Button
            {
                Text = "OK",
                Left = dialog.ClientSize.Width - 220,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = dialog.ClientSize.Width - 110,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var row in rows)
            {
                string chosenFriendly = row.combo.SelectedItem.ToString();
                if (chosenFriendly == row.currentFriendly)
                    continue;

                if (!ReverseCamoMappings.TryGetValue(chosenFriendly, out string newFileNameNoExt))
                {
                    MessageBox.Show($"Could not find an internal name for {chosenFriendly}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                string currentFilePath = row.filePath;
                string folder = Path.GetDirectoryName(currentFilePath);
                string extension = Path.GetExtension(currentFilePath);
                string newFullPath = Path.Combine(folder, newFileNameNoExt + extension);

                if (File.Exists(newFullPath))
                {
                    try { File.Delete(newFullPath); }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting existing file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }

                try
                {
                    File.Move(currentFilePath, newFullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming camo file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                string oldFileName = Path.GetFileName(currentFilePath);
                string newFileName = newFileNameNoExt + extension;
                if (config.Mods.ModMappings.ContainsKey(modName))
                {
                    foreach (var mapping in config.Mods.ModMappings[modName])
                    {
                        if (Path.GetFileName(mapping.ModFile).Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            string prefix = mapping.ModFile.Substring(0, mapping.ModFile.Length - oldFileName.Length);
                            mapping.ModFile = prefix + newFileName;
                            mapping.TargetPath = prefix + newFileName;
                        }
                    }
                }
                if (config.Mods.ReplacedFiles.ContainsKey(modName))
                {
                    for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                    {
                        if (Path.GetFileName(config.Mods.ReplacedFiles[modName][i]).Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            string prefix = config.Mods.ReplacedFiles[modName][i].Substring(0, config.Mods.ReplacedFiles[modName][i].Length - oldFileName.Length);
                            config.Mods.ReplacedFiles[modName][i] = prefix + newFileName;
                        }
                    }
                }
            }

            ConfigManager.SaveSettings(config);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show("Camos updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowSingleBoxSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            string originalSingleBoxFile,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            if (!TryGetSingleBoxInModFolder(modPath, out string currentSingleBoxFile))
            {
                MessageBox.Show("Could not find the current box file in the mod folder.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string fileNameNoExt = Path.GetFileNameWithoutExtension(currentSingleBoxFile);
            string currentBoxFriendlyName = BoxMappings.ContainsKey(fileNameNoExt) ? BoxMappings[fileNameNoExt] : fileNameNoExt;

            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);

            Form dialog = new Form
            {
                Width = 400,
                Height = 200,
                Text = "Change Box",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Label label = new Label
            {
                Text = $"Currently Replacing: {currentBoxFriendlyName}",
                Left = 20,
                Top = 20,
                AutoSize = true,
                ForeColor = Color.Black,
                BackColor = themeColor,
                Font = dialogFont
            };
            dialog.Controls.Add(label);

            ComboBox combo = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor
            };
            combo.Items.AddRange(BoxMappings.Values.ToArray());
            combo.SelectedItem = currentBoxFriendlyName;
            dialog.Controls.Add(combo);

            Button okButton = new Button
            {
                Text = "OK",
                Left = 280,
                Top = 120,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 170,
                Top = 120,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string newFriendlyName = combo.SelectedItem.ToString();
            if (newFriendlyName == currentBoxFriendlyName)
                return;

            if (!ReverseBoxMappings.TryGetValue(newFriendlyName, out string newFileNameNoExt))
            {
                MessageBox.Show($"Could not find an internal name for {newFriendlyName}.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            string folder = Path.GetDirectoryName(currentSingleBoxFile);
            string extension = Path.GetExtension(currentSingleBoxFile);
            string newFullPath = Path.Combine(folder, newFileNameNoExt + extension);
            if (File.Exists(newFullPath))
            {
                try { File.Delete(newFullPath); }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting existing file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            try
            {
                File.Move(currentSingleBoxFile, newFullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renaming box:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string oldFileName = Path.GetFileName(currentSingleBoxFile);
            string newFileName = newFileNameNoExt + extension;
            if (config.Mods.ModMappings.ContainsKey(modName))
            {
                foreach (var mapping in config.Mods.ModMappings[modName])
                {
                    if (mapping.ModFile.EndsWith(oldFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = mapping.ModFile.Substring(0, mapping.ModFile.Length - oldFileName.Length);
                        mapping.ModFile = prefix + newFileName;
                        mapping.TargetPath = prefix + newFileName;
                    }
                }
            }
            if (config.Mods.ReplacedFiles.ContainsKey(modName))
            {
                for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                {
                    if (config.Mods.ReplacedFiles[modName][i].EndsWith(oldFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = config.Mods.ReplacedFiles[modName][i].Substring(0, config.Mods.ReplacedFiles[modName][i].Length - oldFileName.Length);
                        config.Mods.ReplacedFiles[modName][i] = prefix + newFileName;
                    }
                }
            }
            ConfigManager.SaveSettings(config);

            string modDetailsPath = Path.Combine(modPath, "Mod Details");
            if (!Directory.Exists(modDetailsPath))
                Directory.CreateDirectory(modDetailsPath);
            string modInfoPath = Path.Combine(modDetailsPath, "Mod Info.txt");
            string infoLine = $"This mod is currently replacing the {newFriendlyName} Box";
            if (!File.Exists(modInfoPath))
                File.WriteAllText(modInfoPath, infoLine);
            else
                ReplaceOrAppendModInfoLine(modInfoPath, infoLine);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show($"Box changed to {newFriendlyName}.\n" +
                (wasInstalled ? "Mod was reinstalled to apply changes." : "Changes will apply next time you install this mod."),
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void ShowUnifiedFaceSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            List<string> faceFiles,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            if (!TryGetFaceVariantsInModFolder(modPath, out string currentFaceWithFile, out string currentFaceNoFile))
            {
                MessageBox.Show("Could not uniquely determine the face files in the mod folder.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string keyWith = Path.GetFileNameWithoutExtension(currentFaceWithFile);
            string keyNo = Path.GetFileNameWithoutExtension(currentFaceNoFile);
            string currentWithFriendly = FaceMappingsWithBandana.TryGetValue(keyWith, out string f1) ? f1 : keyWith;
            string currentNoFriendly = FaceMappingsNoBandana.TryGetValue(keyNo, out string f2) ? f2 : keyNo;

            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);

            Form dialog = new Form
            {
                Width = 450,
                Height = 300,
                Text = "Change Face (Unified)",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Label lblWith = new Label
            {
                Text = $"Currently (With Bandana): {currentWithFriendly}",
                Left = 20,
                Top = 20,
                AutoSize = true,
                ForeColor = Color.Black,
                BackColor = themeColor,
                Font = dialogFont
            };
            dialog.Controls.Add(lblWith);

            ComboBox comboWith = new ComboBox
            {
                Left = 20,
                Top = 50,
                Width = 380,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor
            };
            var withOptions = FaceMappingsWithBandana.Values.OrderBy(x => x).ToArray();
            comboWith.Items.AddRange(withOptions);
            comboWith.SelectedItem = currentWithFriendly;
            dialog.Controls.Add(comboWith);

            Label lblNo = new Label
            {
                Text = $"Currently (No Bandana): {currentNoFriendly}",
                Left = 20,
                Top = 100,
                AutoSize = true,
                ForeColor = Color.Black,
                BackColor = themeColor,
                Font = dialogFont
            };
            dialog.Controls.Add(lblNo);

            ComboBox comboNo = new ComboBox
            {
                Left = 20,
                Top = 130,
                Width = 380,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor
            };
            var noOptions = FaceMappingsNoBandana.Values.OrderBy(x => x).ToArray();
            comboNo.Items.AddRange(noOptions);
            comboNo.SelectedItem = currentNoFriendly;
            dialog.Controls.Add(comboNo);

            Button okButton = new Button
            {
                Text = "OK",
                Left = 300,
                Top = 200,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = 190,
                Top = 200,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            string chosenWith = comboWith.SelectedItem.ToString();
            string chosenNo = comboNo.SelectedItem.ToString();

            if (!ReverseFaceMappingsWithBandana.TryGetValue(chosenWith, out string newInternalWith))
            {
                MessageBox.Show($"Could not find an internal name for {chosenWith}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!ReverseFaceMappingsNoBandana.TryGetValue(chosenNo, out string newInternalNo))
            {
                MessageBox.Show($"Could not find an internal name for {chosenNo}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            string folderWith = Path.GetDirectoryName(currentFaceWithFile);
            string folderNo = Path.GetDirectoryName(currentFaceNoFile);
            string extWith = Path.GetExtension(currentFaceWithFile);
            string extNo = Path.GetExtension(currentFaceNoFile);
            string newFaceWithFile = Path.Combine(folderWith, newInternalWith + extWith);
            string newFaceNoFile = Path.Combine(folderNo, newInternalNo + extNo);

            if (File.Exists(newFaceWithFile))
            {
                try { File.Delete(newFaceWithFile); } catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            }
            if (File.Exists(newFaceNoFile))
            {
                try { File.Delete(newFaceNoFile); } catch (Exception ex) { MessageBox.Show(ex.Message); return; }
            }
            try
            {
                File.Move(currentFaceWithFile, newFaceWithFile);
                File.Move(currentFaceNoFile, newFaceNoFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error renaming face files:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string oldFaceWith = Path.GetFileName(currentFaceWithFile);
            string oldFaceNo = Path.GetFileName(currentFaceNoFile);
            string newFaceWith = newInternalWith + extWith;
            string newFaceNo = newInternalNo + extNo;

            if (config.Mods.ModMappings.ContainsKey(modName))
            {
                foreach (var mapping in config.Mods.ModMappings[modName])
                {
                    if (mapping.ModFile.EndsWith(oldFaceWith, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = mapping.ModFile.Substring(0, mapping.ModFile.Length - oldFaceWith.Length);
                        mapping.ModFile = prefix + newFaceWith;
                        mapping.TargetPath = prefix + newFaceWith;
                    }
                    if (mapping.ModFile.EndsWith(oldFaceNo, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = mapping.ModFile.Substring(0, mapping.ModFile.Length - oldFaceNo.Length);
                        mapping.ModFile = prefix + newFaceNo;
                        mapping.TargetPath = prefix + newFaceNo;
                    }
                }
            }
            if (config.Mods.ReplacedFiles.ContainsKey(modName))
            {
                for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                {
                    if (config.Mods.ReplacedFiles[modName][i].EndsWith(oldFaceWith, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = config.Mods.ReplacedFiles[modName][i].Substring(0, config.Mods.ReplacedFiles[modName][i].Length - oldFaceWith.Length);
                        config.Mods.ReplacedFiles[modName][i] = prefix + newFaceWith;
                    }
                    if (config.Mods.ReplacedFiles[modName][i].EndsWith(oldFaceNo, StringComparison.OrdinalIgnoreCase))
                    {
                        string prefix = config.Mods.ReplacedFiles[modName][i].Substring(0, config.Mods.ReplacedFiles[modName][i].Length - oldFaceNo.Length);
                        config.Mods.ReplacedFiles[modName][i] = prefix + newFaceNo;
                    }
                }
            }
            ConfigManager.SaveSettings(config);

            string modDetailsPath = Path.Combine(modPath, "Mod Details");
            if (!Directory.Exists(modDetailsPath))
                Directory.CreateDirectory(modDetailsPath);
            string modInfoPath = Path.Combine(modDetailsPath, "Mod Info.txt");
            string infoLine = $"This mod is currently replacing the {chosenWith} / {chosenNo} Face variants";
            if (!File.Exists(modInfoPath))
                File.WriteAllText(modInfoPath, infoLine);
            else
                ReplaceOrAppendModInfoLine(modInfoPath, infoLine);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show($"Face variants updated.\n" +
                (wasInstalled ? "Mod was reinstalled to apply changes." : "Changes will apply next time you install this mod."),
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        public static void ShowMultiTextureSwapDialog(
            ConfigSettings config,
            string modName,
            string modPath,
            List<string> camos,
            List<string> boxes,
            List<string> faces,
            string gameInstallPath,
            Action<string, string> RestoreVanillaFiles,
            Action<string, string> ApplyModFiles,
            Action<string, string> ReplaceOrAppendModInfoLine)
        {
            bool wasInstalled = config.Mods.ActiveMods.ContainsKey(modName) && config.Mods.ActiveMods[modName];
            if (wasInstalled)
            {
                RestoreVanillaFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = false;
                ConfigManager.SaveSettings(config);
            }

            Font dialogFont = new Font("Microsoft Sans Serif", 12, FontStyle.Bold);
            Color themeColor = Color.FromArgb(149, 149, 125);

            Form dialog = new Form
            {
                Width = 600,
                Height = 500,
                Text = "Change Textures",
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = themeColor,
                Font = dialogFont
            };

            Panel panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 350,
                AutoScroll = true,
                BackColor = themeColor
            };
            dialog.Controls.Add(panel);

            var rows = new List<(string filePath, string category, ComboBox combo, string oldFriendly)>();
            int y = 10;
            foreach (string file in camos)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string currentFriendly = CamoMappings.ContainsKey(fileNameNoExt)
                    ? CamoMappings[fileNameNoExt]
                    : fileNameNoExt;
                Label lbl = new Label
                {
                    Text = $"Camo: {currentFriendly}",
                    Left = 10,
                    Top = y,
                    Width = 200,
                    ForeColor = Color.Black,
                    BackColor = themeColor,
                    Font = dialogFont
                };
                panel.Controls.Add(lbl);
                ComboBox combo = new ComboBox
                {
                    Left = 220,
                    Top = y,
                    Width = 300,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dialogFont,
                    ForeColor = Color.Black,
                    BackColor = themeColor
                };
                combo.Items.AddRange(CamoMappings.Values.ToArray());
                combo.SelectedItem = currentFriendly;
                panel.Controls.Add(combo);
                rows.Add((file, "camo", combo, currentFriendly));
                y += 30;
            }
            foreach (string file in boxes)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string currentFriendly = BoxMappings.ContainsKey(fileNameNoExt)
                    ? BoxMappings[fileNameNoExt]
                    : fileNameNoExt;
                Label lbl = new Label
                {
                    Text = $"Box: {currentFriendly}",
                    Left = 10,
                    Top = y,
                    Width = 200,
                    ForeColor = Color.Black,
                    BackColor = themeColor,
                    Font = dialogFont
                };
                panel.Controls.Add(lbl);
                ComboBox combo = new ComboBox
                {
                    Left = 220,
                    Top = y,
                    Width = 300,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dialogFont,
                    ForeColor = Color.Black,
                    BackColor = themeColor
                };
                combo.Items.AddRange(BoxMappings.Values.ToArray());
                combo.SelectedItem = currentFriendly;
                panel.Controls.Add(combo);
                rows.Add((file, "box", combo, currentFriendly));
                y += 30;
            }
            foreach (string file in faces)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                string currentFriendly = FaceMappings.ContainsKey(fileNameNoExt)
                    ? FaceMappings[fileNameNoExt]
                    : fileNameNoExt;
                Label lbl = new Label
                {
                    Text = $"Face: {currentFriendly}",
                    Left = 10,
                    Top = y,
                    Width = 200,
                    ForeColor = Color.Black,
                    BackColor = themeColor,
                    Font = dialogFont
                };
                panel.Controls.Add(lbl);
                ComboBox combo = new ComboBox
                {
                    Left = 220,
                    Top = y,
                    Width = 300,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = dialogFont,
                    ForeColor = Color.Black,
                    BackColor = themeColor
                };
                combo.Items.AddRange(FaceMappings.Values.ToArray());
                combo.SelectedItem = currentFriendly;
                panel.Controls.Add(combo);
                rows.Add((file, "face", combo, currentFriendly));
                y += 30;
            }

            Button okButton = new Button
            {
                Text = "OK",
                Left = dialog.ClientSize.Width - 220,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.OK,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            okButton.FlatAppearance.BorderSize = 2;
            okButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(okButton);
            dialog.AcceptButton = okButton;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Left = dialog.ClientSize.Width - 110,
                Top = dialog.ClientSize.Height - 60,
                Width = 100,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                Font = dialogFont,
                ForeColor = Color.Black,
                BackColor = themeColor,
                FlatStyle = FlatStyle.Flat
            };
            cancelButton.FlatAppearance.BorderSize = 2;
            cancelButton.FlatAppearance.BorderColor = Color.Black;
            dialog.Controls.Add(cancelButton);
            dialog.CancelButton = cancelButton;

            if (dialog.ShowDialog() != DialogResult.OK)
                return;

            foreach (var row in rows)
            {
                string newFriendlyName = row.combo.SelectedItem.ToString();
                if (newFriendlyName == row.oldFriendly)
                    continue;

                string newFileNameNoExt = null;
                if (row.category == "camo")
                {
                    if (!ReverseCamoMappings.TryGetValue(newFriendlyName, out newFileNameNoExt))
                    {
                        MessageBox.Show($"Could not find an internal name for {newFriendlyName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                else if (row.category == "box")
                {
                    if (!ReverseBoxMappings.TryGetValue(newFriendlyName, out newFileNameNoExt))
                    {
                        MessageBox.Show($"Could not find an internal name for {newFriendlyName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                else
                {
                    if (!ReverseFaceMappings.TryGetValue(newFriendlyName, out newFileNameNoExt))
                    {
                        MessageBox.Show($"Could not find an internal name for {newFriendlyName}.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }

                string currentFilePath = row.filePath;
                string folder = Path.GetDirectoryName(currentFilePath);
                string extension = Path.GetExtension(currentFilePath);
                string newFullPath = Path.Combine(folder, newFileNameNoExt + extension);

                if (File.Exists(newFullPath))
                {
                    try { File.Delete(newFullPath); }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting existing file:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }
                }
                try
                {
                    File.Move(currentFilePath, newFullPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error renaming {row.category}:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                string oldFileName = Path.GetFileName(currentFilePath);
                string newFileName = newFileNameNoExt + extension;
                if (config.Mods.ModMappings.ContainsKey(modName))
                {
                    foreach (var mapping in config.Mods.ModMappings[modName])
                    {
                        if (Path.GetFileName(mapping.ModFile).Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            string prefix = mapping.ModFile.Substring(0, mapping.ModFile.Length - oldFileName.Length);
                            mapping.ModFile = prefix + newFileName;
                            mapping.TargetPath = prefix + newFileName;
                        }
                    }
                }
                if (config.Mods.ReplacedFiles.ContainsKey(modName))
                {
                    for (int i = 0; i < config.Mods.ReplacedFiles[modName].Count; i++)
                    {
                        if (Path.GetFileName(config.Mods.ReplacedFiles[modName][i]).Equals(oldFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            string prefix = config.Mods.ReplacedFiles[modName][i].Substring(0, config.Mods.ReplacedFiles[modName][i].Length - oldFileName.Length);
                            config.Mods.ReplacedFiles[modName][i] = prefix + newFileName;
                        }
                    }
                }
            }
            ConfigManager.SaveSettings(config);

            if (wasInstalled)
            {
                ApplyModFiles(modPath, gameInstallPath);
                config.Mods.ActiveMods[modName] = true;
                ConfigManager.SaveSettings(config);
            }

            MessageBox.Show("Textures updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Helper Methods

        public static Button CreateChangeButton(string text, string modName, EventHandler onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(160, 40),
                Tag = modName,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Bold),
                BackColor = Color.LightBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            return btn;
        }

        public static void ReplaceOrAppendModInfoLine(string modInfoPath, string newLine)
        {
            var lines = File.ReadAllLines(modInfoPath).ToList();
            bool replaced = false;
            string prefix = "This mod is currently replacing the ";

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith(prefix))
                {
                    lines[i] = newLine;
                    replaced = true;
                    break;
                }
            }

            if (!replaced)
            {
                lines.Add(newLine);
            }
            File.WriteAllLines(modInfoPath, lines);
        }

        public static bool TryGetSingleCamoInModFolder(string modPath, out string singleCamoFile)
        {
            singleCamoFile = null;

            List<string> matchingFiles = new List<string>();

            var ctxrFiles = Directory.GetFiles(modPath, "*.ctxr", IOSearchOption.AllDirectories);
            foreach (string file in ctxrFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (CamoMappings.ContainsKey(fileNameNoExt))
                {
                    matchingFiles.Add(file);
                    if (matchingFiles.Count > 1)
                        break;
                }
            }

            if (matchingFiles.Count == 1)
            {
                singleCamoFile = matchingFiles[0];
                return true;
            }
            return false;
        }

        public static bool TryGetMultipleCamosInModFolder(string modPath, out List<string> camoFiles)
        {
            camoFiles = new List<string>();
            var ctxrFiles = Directory.GetFiles(modPath, "*.ctxr", IOSearchOption.AllDirectories);
            foreach (string file in ctxrFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (CamoMappings.ContainsKey(fileNameNoExt))
                {
                    camoFiles.Add(file);
                }
            }
            return (camoFiles.Count > 1);
        }


        public static bool TryGetSingleBoxInModFolder(string modPath, out string singleBoxFile)
        {
            singleBoxFile = null;
            List<string> matchingFiles = new List<string>();
            var ctxrFiles = Directory.GetFiles(modPath, "*.ctxr", IOSearchOption.AllDirectories);
            foreach (string file in ctxrFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (BoxMappings.ContainsKey(fileNameNoExt))
                {
                    matchingFiles.Add(file);
                    if (matchingFiles.Count > 1)
                        break;
                }
            }
            if (matchingFiles.Count == 1)
            {
                singleBoxFile = matchingFiles[0];
                return true;
            }
            return false;
        }

        public static bool TryGetFaceVariantsInModFolder(string modPath, out string faceWithBandanaFile, out string faceNoBandanaFile)
        {
            faceWithBandanaFile = null;
            faceNoBandanaFile = null;
            var ctxrFiles = Directory.GetFiles(modPath, "*.ctxr", IOSearchOption.AllDirectories);
            foreach (var file in ctxrFiles)
            {
                string fileNameNoExt = Path.GetFileNameWithoutExtension(file);
                if (FaceMappingsWithBandana.ContainsKey(fileNameNoExt) && faceWithBandanaFile == null)
                {
                    faceWithBandanaFile = file;
                }
                else if (FaceMappingsNoBandana.ContainsKey(fileNameNoExt) && faceNoBandanaFile == null)
                {
                    faceNoBandanaFile = file;
                }
                if (faceWithBandanaFile != null && faceNoBandanaFile != null)
                    break;
            }
            return (faceWithBandanaFile != null && faceNoBandanaFile != null);
        }

        #endregion
        

    }
}
