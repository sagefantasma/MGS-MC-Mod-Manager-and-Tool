using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class ModelSwapperForm : Form
    {
        internal class MGSModel
        {
            public string LoLoDId { get; set; }
            public string HiLoDId { get; set; }
            public string Name { get; set; }
            public string ResidentStage { get; set; }
            public string Headpiece { get; set; }
            public List<string> LoLoDTextures { get; set; } = new List<string>();
            public List<string> HiLoDTextures { get; set; } = new List<string>();
            public string LoLodKms { get; set; }
            public string LoLoDTri { get; set; }
            public string HiLodKms { get; set; }
            public string HiLoDTri { get; set; }
            //TODO: probably need shadow too
            //TODO: codec?
            //TODO: first person arm?

            public override string ToString()
            {
                return Name;
            }
        }


        private string _gameDirectory;
        private string _backupDirectory;
        private List<MGSModel> ModelsToSwapIn { get; set; } = new List<MGSModel>();
        private List<MGSModel> ModelsToSwapOut { get; set; } = new List<MGSModel>();

        private List<string> GetTexturesFromListOfIds(List<string> idList, string triFile, string newResidentId)
        {
            DirectoryInfo stagesDirectory = new DirectoryInfo(Path.Combine(_gameDirectory, "eu", "stage"));
            FileInfo[] bpAssetsList = stagesDirectory.GetFiles("bp_assets.txt", SearchOption.AllDirectories);
            List<string> textureList = new List<string>();
            foreach (string id in idList)
            {
                //Get resource string for desired asset
                string firstFileContainingId = (from file in bpAssetsList
                                        let fileText = File.ReadAllText(file.FullName)
                                        where fileText.Contains(id, StringComparison.CurrentCultureIgnoreCase)
                                        select file.FullName).FirstOrDefault();
                string lineContainingId = (from line in File.ReadAllLines(firstFileContainingId) 
                                           where line.Contains(id, StringComparison.CurrentCultureIgnoreCase)// && line.Contains(triFile, StringComparison.CurrentCultureIgnoreCase) 
                                           select line).FirstOrDefault();
                //"textures/flatlist/ema_arm_sub_alp_ovl.bmp.ctxr,stage/a16a/cache/ema_arm_sub_alp_ovl.bmp.ctxr,eu/stage/a16a/cache/00535469/000361b9.ctxr

                string assetName = lineContainingId.Split('/', StringSplitOptions.RemoveEmptyEntries)[2].Split(',',StringSplitOptions.None)[0];

                //Create new resource string for asset in proper resident format
                textureList.Add($"textures/flatlist/{assetName},stage/{newResidentId}/resident/{assetName},eu/stage/{newResidentId}/resident/{triFile}/{id.ToLower()}.ctxr");
            }

            return textureList;
        }

        private void BuildModelsToSwapInList()
        {
            //TODO: fix the hardcode to r_plt0 in Tri files
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Emma",
                LoLoDId = "00b35469", //assets/tri/us/ema_def_sh_mt.tri,us/stage/w31d/cache/00b35469.tri,cache/00b35469.tri
                LoLodKms = "assets/kms/us/ema_def_sh_mt.kms,us/stage/XXXX/resident/00b35469.kms,resident/00b35469.kms",
                LoLoDTri = "assets/tri/us/ema_def_mh_mt.tri,us/stage/r_plt0/resident/00535469.tri,resident/00535469.tri", //is this actually a cutscene model instead?
                HiLoDId = "00535469", //assets/tri/us/ema_def_mh_mt.tri,us/stage/w31d/cache/00535469.tri,cache/00535469.tri -- not used
                HiLodKms = "assets/kms/us/ema_def_sh_mt.kms,us/stage/XXXX/resident/00b35469.kms,resident/00b35469.kms", //reusing other LoD
                HiLoDTri = "assets/tri/us/ema_def_sh_mt.tri,us/stage/r_plt0/resident/00b35469.tri,resident/00b35469.tri",
                //assets/tri/us/ema_hair_mh.tri,us/stage/w31d/cache/00ce9e72.tri,cache/00ce9e72.tri -- needed?
                LoLoDTextures = new List<string>
                {
                    "000361B9",
            "00F33158",
            "00995033",
            "001ADA6E",
            "00A901BB",
            "00256123",
            "00B33B34",
            "00B34158",
            "0034CE8A",
            "00CE23D8",
            "00BF280E",
            "00741A88",
            "00980D82",
            "00D42992",
            "0019862E",
            "00980F82",
            "00681CDE",
            "00127870",
            "008B6076",
            "0000E994",
            "00114762",
            "00FAD987",
            "00D672E9",
            "0058B6D5",
            "00415727",
            "007A021C",
            "00ADDBA2",
            "003B9DA8",
            "00917197",
            "000577FC",
            "000E1816",
            "0025614B",
            "006E5D1F"
                },
                HiLoDTextures = new List<string>
                {
                    "000361B9",
            "00F33158",
            "00995033",
            "001ADA6E",
            "00A901BB",
            "00256123",
            "00B33B34",
            "00B34158",
            "00CE23D8",
            "00BF280E",
            "00741A88",
            "00980D82",
            "00D42B43",
            "0034962E",
            "00980F82",
            "00681CDE",
            "00127870",
            "008B6076",
            "00114762",
            "00FAD987",
            "00D672E9",
            "0058B6D5",
            "00415727",
            "007A021C",
            "00ADDBA2",
            "0071E293",
            "003B9DA8",
            "000577FC",
            "000E1816",
            "0025614B",
            "002ED62F",
            "00F87996",
            "00F87997",
            "00F87998",
            "00F87999",
            "00F87BB6",
            "00F87BB7",
            "00F87BB8",
            "00F87BB9",
            "006E5D1F"
                }
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Fortune",
                LoLoDId = "0051376b",
                LoLodKms = "assets/kms/us/for_def_sh_mt.kms,us/stage/XXXX/resident/00b365ad.kms,resident/00b365ad.kms",
                LoLoDTri = "assets/tri/us/for_def.tri,us/stage/r_plt0/resident/0051376b.tri,resident/0051376b.tri",
                HiLoDId = "0004ad70",
                HiLodKms = "assets/kms/us/for_def_sh_mt.kms,us/stage/XXXX/resident/00b365ad.kms,resident/00b365ad.kms", //reusing other LoD
                HiLoDTri = "assets/tri/us/fortune_mh_mt.tri,us/stage/r_plt0/resident/0004ad70.tri,resident/0004ad70.tri",
                LoLoDTextures = new()
                {
                    "00512D12",
            "00517DFE",
            "002007A5",
            "00AF0DCB",
            "001FDB23",
            "00DA8B4C",
            "00D31DDD",
            "006EF8B3",
            "006EF8B4",
            "006FF37B",
            "000D0276",
            "00787367",
            "005EFDFF",
            "00574B69",
            "002816FE",
            "00744D49",
            "0057A390",
            "0028DECE",
            "001BD9F7",
            "00F3CC28",
            "00A347A5",
            "00BF5036",
            "0070DB5E",
            "00CAEC9E",
            "00CAECAF",
            "0074A7CD",
            "0013B767",
            "007D47E7",
            "00F71E00",
            "00B7B37E",
            "007A52EE",
            "002029A5",
            "005174BD",
            "00CC2E02",
            "005178AA"
                },
                HiLoDTextures = new()
                {
                    "00512D12",
            "00517DFE",
            "002007A5",
            "00AF0DCB",
            "001FDB23",
            "00DA8B4C",
            "00D31DDD",
            "006EF8B3",
            "006EF8B4",
            "006FF37B",
            "00138BFD",
            "003001DC",
            "000D0276",
            "005EFDFF",
            "00574B69",
            "002816FE",
            "00744D49",
            "00F3CC28",
            "00A347A5",
            "001AA2F9",
            "00545F55",
            "0054D480",
            "0054D481",
            "0054D482",
            "00BF5036",
            "0070DB5E",
            "00CAEC9E",
            "00CAECAF",
            "0074A7CD",
            "0013B767",
            "007D47E7",
            "00F71E00",
            "00B7B37E",
            "007A52EE",
            "002029A5",
            "005174BD",
            "00CC2E02",
            "00EB6E02",
            "00EBCE02",
            "005178AA",
            "008F00C7"
                }
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "MGS1 Snake"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Ninja Raiden"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Pliskin"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Raiden"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Snake"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Solidus"
            });
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Tuxedo Snake"
            });

        }

        private void BuildModelsToSwapOutList()
        {
            ModelsToSwapOut.Add(new MGSModel {
                Name = "Raiden - Story (r_plt0)",
                LoLodKms = "assets/kms/us/rai_def.kms,us/stage/r_plt0/resident/00c13a4e.kms,resident/00c13a4e.kms",
                HiLodKms = "assets/kms/us/rai_def_sh_mt_stage_r_plt0_r.kms,us/stage/r_plt0/resident/00b41e89.kms,resident/00b41e89.kms",
                Headpiece = "rai_hair_bounding_stage_r_plt0_r.kms",
                ResidentStage = "r_plt0"
            });
            ModelsToSwapOut.Add(new MGSModel { 
                Name = "Snake - Story (r_tnk0)", //TODO: confirm
                LoLodKms = "assets/kms/us/sna_def_stage_r_plt10_r.kms,us/stage/r_tnk0/resident/00413aa8.kms,resident/00413aa8.kms",
                HiLodKms = "assets/kms/us/sna_def_sh_stage_r_plt10_r.kms,us/stage/r_tnk0/resident/0055ab65.kms,resident/0055ab65.kms",
                Headpiece = "sna_bdn1_stage_r_plt_s_r.kms",
                ResidentStage = "r_tnk0"
            });
        }

        public ModelSwapperForm(string gameDirectory, string modToolsDirectory)
        {
            InitializeComponent();
            BuildModelsToSwapInList();
            BuildModelsToSwapOutList();
            modelToSwapInComboBox.Items.AddRange(ModelsToSwapIn.ToArray());
            modelToSwapOutComboBox.Items.AddRange(ModelsToSwapOut.ToArray());
            _gameDirectory = gameDirectory;
            _backupDirectory = Path.Combine(modToolsDirectory, "Model Swap Backups");
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        private void SwapModelIn(MGSModel modelToSwapOut, MGSModel modelToSwapIn)
        {
            DirectoryInfo kmsDirectory = new(Path.Combine(_gameDirectory, "assets", "kms", "us"));
            DirectoryInfo cmdlDirectory = new(Path.Combine(kmsDirectory.FullName, "_win"));
            string loLodFileNameToSwapOut = modelToSwapOut.LoLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
            string hiLodFileNameToSwapOut = modelToSwapOut.HiLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
            FileInfo[] kmsFiles = kmsDirectory.GetFiles();
            FileInfo[] cmdlFiles = cmdlDirectory.GetFiles();

            File.Delete(kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == loLodFileNameToSwapOut).FullName);
            File.Delete(cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == loLodFileNameToSwapOut).FullName);
            File.Delete(kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == hiLodFileNameToSwapOut).FullName);
            File.Delete(cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == hiLodFileNameToSwapOut).FullName);

            string loLodFileNameToSwapIn = modelToSwapIn.LoLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
            string hiLodFileNameToSwapIn = modelToSwapIn.HiLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];

            File.Copy(kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == loLodFileNameToSwapIn).FullName, Path.Combine(kmsDirectory.FullName, $"{loLodFileNameToSwapOut}.kms"));
            File.Copy(cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == loLodFileNameToSwapIn).FullName, Path.Combine(cmdlDirectory.FullName, $"{loLodFileNameToSwapOut}.cmdl"));
            File.Copy(kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == hiLodFileNameToSwapIn).FullName, Path.Combine(kmsDirectory.FullName, $"{hiLodFileNameToSwapOut}.kms"));
            File.Copy(cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension, "") == hiLodFileNameToSwapIn).FullName, Path.Combine(cmdlDirectory.FullName, $"{hiLodFileNameToSwapOut}.cmdl"));
        }

        private void RemoveRaidenHair()
        {
            DirectoryInfo kmsDirectory = new(Path.Combine(_gameDirectory, "assets", "kms", "us"));
            DirectoryInfo cmdlDirectory = new(Path.Combine(kmsDirectory.FullName, "_win"));
            string raidenHair = "rai_hair_bounding_stage_r_plt0_r";
            string nullModel = "null.kms";
            FileInfo[] kmsFiles = kmsDirectory.GetFiles();
            FileInfo[] cmdlFiles = cmdlDirectory.GetFiles();
        }

        private void BackupModel(MGSModel modelToBackup)
        {
            DirectoryInfo backupDirectory = new (_backupDirectory);
            DirectoryInfo kmsDirectory = new (Path.Combine(_gameDirectory, "assets", "kms", "us"));
            DirectoryInfo cmdlDirectory = new (Path.Combine(kmsDirectory.FullName, "_win"));
            string loLodFileName = modelToBackup.LoLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.',StringSplitOptions.RemoveEmptyEntries)[0];
            string hiLodFileName = modelToBackup.HiLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
            FileInfo[] kmsFiles = kmsDirectory.GetFiles();
            FileInfo[] cmdlFiles = cmdlDirectory.GetFiles();

            List<FileInfo> filesToBackup = new() {
                kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension,"") == loLodFileName),
                cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension,"") == loLodFileName),
                kmsFiles.FirstOrDefault(x => x.Name.Replace(x.Extension,"") == hiLodFileName),
                cmdlFiles.FirstOrDefault(x => x.Name.Replace(x.Extension,"") == hiLodFileName)
            };

            FileInfo[] backupFiles = backupDirectory.GetFiles();

            foreach(FileInfo fileToBackup in filesToBackup)
            {
                if(!backupFiles.Any(x=>x.Name == fileToBackup.Name))
                {
                    File.Copy(fileToBackup.FullName, Path.Combine(_backupDirectory, fileToBackup.Name));
                }
            }
        }

        private void RestoreBackupModel(MGSModel model)
        {
            try
            {
                DirectoryInfo backupDirectory = new(_backupDirectory);
                FileInfo[] backupFiles = backupDirectory.GetFiles();
                DirectoryInfo kmsDirectory = new(Path.Combine(_gameDirectory, "assets", "kms", "us"));
                DirectoryInfo cmdlDirectory = new(Path.Combine(kmsDirectory.FullName, "_win"));
                string loLodFileName = model.LoLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
                string hiLodFileName = model.HiLodKms.Split('/', StringSplitOptions.RemoveEmptyEntries)[3].Split(',', StringSplitOptions.RemoveEmptyEntries)[0].Split('.', StringSplitOptions.RemoveEmptyEntries)[0];


                File.Copy(backupFiles.FirstOrDefault(x => x.Name == $"{loLodFileName}.kms").FullName, Path.Combine(kmsDirectory.FullName, $"{loLodFileName}.kms"), true);
                File.Copy(backupFiles.FirstOrDefault(x => x.Name == $"{loLodFileName}.cmdl").FullName, Path.Combine(cmdlDirectory.FullName, $"{loLodFileName}.cmdl"), true);
                File.Copy(backupFiles.FirstOrDefault(x => x.Name == $"{hiLodFileName}.kms").FullName, Path.Combine(kmsDirectory.FullName, $"{hiLodFileName}.kms"), true);
                File.Copy(backupFiles.FirstOrDefault(x => x.Name == $"{hiLodFileName}.cmdl").FullName, Path.Combine(cmdlDirectory.FullName, $"{hiLodFileName}.cmdl"), true);
            }
            catch
            {
                //File not backed up yet, so no need to worry
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: it probably makes the most sense to backup all the models when opening the form for the first time, and then using THAT to swap in.
            MGSModel modelToSwapIn = modelToSwapInComboBox.SelectedItem as MGSModel;
            MGSModel modelToSwapOut = modelToSwapOutComboBox.SelectedItem as MGSModel;
            RestoreBackupModel(modelToSwapOut);
            BackupModel(modelToSwapOut);
            
            List<string> loLodTextureList = GetTexturesFromListOfIds(modelToSwapIn.LoLoDTextures, modelToSwapIn.LoLoDId, modelToSwapOut.ResidentStage);
            List<string> hiLodTextureList = GetTexturesFromListOfIds(modelToSwapIn.HiLoDTextures, modelToSwapIn.HiLoDId, modelToSwapOut.ResidentStage);
            InsertTexturesIntoResidentFile(loLodTextureList, modelToSwapOut.ResidentStage);
            InsertTexturesIntoResidentFile(hiLodTextureList, modelToSwapOut.ResidentStage);

            InsertTriIntoResidentFile(modelToSwapIn.LoLoDTri, modelToSwapOut.ResidentStage); //Are the tris necessary? Let's find out :) - tested it, and yes they are lmao
            InsertTriIntoResidentFile(modelToSwapIn.HiLoDTri, modelToSwapOut.ResidentStage);
            //Swap kms and cmdl names
            SwapModelIn(modelToSwapOut, modelToSwapIn);
            //Raiden loLod: rai_def
            //Raiden hiLoD: rai_def_sh_mt_stage_r_plt0_r
            //can we do codec as well?
            //^this resulted in nothing, xdd -- WAIT NO, I HAVE IT. FUCK YEAH - just needed to correct a cache -> resident and it worked immediately~!
        }

        

        private void InsertTriIntoResidentFile(string newTriFile, string resident)
        {
            DirectoryInfo residentDirectory = new DirectoryInfo(Path.Combine(_gameDirectory, "eu", "stage", resident));
            FileInfo manifest = residentDirectory.GetFiles("manifest.txt").FirstOrDefault();
            string[] manifestContents = File.ReadAllLines(manifest.FullName);
            List<string> triFiles = manifestContents.Where(x => x.Contains(".tri")).ToList();
            List<string> otherAssetsList = manifestContents.Where(x => !x.Contains(".tri")).ToList();
            triFiles.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            otherAssetsList.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            if(!triFiles.Contains(newTriFile))
                triFiles.Add(newTriFile);
            triFiles.Sort();

            string newResidentFileContents = "";
            foreach (string triFile in triFiles)
            {
                newResidentFileContents += $"{triFile.Trim()}\r\r\n";
            }
            foreach(string otherAsset in otherAssetsList)
            {
                newResidentFileContents += $"{otherAsset.Trim()}\r\r\n";
            }

            File.WriteAllText(manifest.FullName, newResidentFileContents);
        }

        private void InsertTexturesIntoResidentFile(List<string> textureList, string resident)
        {
            DirectoryInfo residentDirectory = new DirectoryInfo(Path.Combine(_gameDirectory, "eu", "stage", resident));
            FileInfo bpAssets = residentDirectory.GetFiles("bp_assets.txt").FirstOrDefault();
            string[] bpAssetsContents = File.ReadAllLines(bpAssets.FullName);
            List<string> texturesList = bpAssetsContents.Where(x=>x.Contains(".ctxr")).ToList();
            List<string> otherAssetsList = bpAssetsContents.Where(x => !x.Contains(".ctxr")).ToList();
            texturesList.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            otherAssetsList.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            foreach(string texture in textureList)
            {
                if (!texturesList.Contains(texture))
                {
                    texturesList.Add(texture);
                }
            }
            texturesList.Sort();

            string newResidentFileContents = "";
            foreach (string texture in texturesList)
            {
                newResidentFileContents += $"{texture.Trim()}\r\r\n";
            }
            foreach (string otherAsset in otherAssetsList)
            {
                newResidentFileContents += $"{otherAsset.Trim()}\r\r\n";
            }

            File.WriteAllText(bpAssets.FullName, newResidentFileContents);
        }

        private void modelToSwapOutComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            modelToSwapInComboBox.Enabled = true;
            //TODO: select currently swapped in model in modelToSwapInComboBox
        }
    }
}
