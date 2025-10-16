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
            public List<string> LoLoDTextures { get; set; } = new List<string>();
            public List<string> HiLoDTextures { get; set; } = new List<string>();
            //TODO: probably need shadow too

            public override string ToString()
            {
                return Name;
            }
        }

        private string _gameDirectory;
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
                                           where line.Contains(id, StringComparison.CurrentCultureIgnoreCase) && line.Contains(triFile, StringComparison.CurrentCultureIgnoreCase) 
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
            ModelsToSwapIn.Add(new MGSModel
            {
                Name = "Emma",
                LoLoDId = "00b35469", //assets/tri/us/ema_def_sh_mt.tri,us/stage/w31d/cache/00b35469.tri,cache/00b35469.tri
                HiLoDId = "00535469", //assets/tri/us/ema_def_mh_mt.tri,us/stage/w31d/cache/00535469.tri,cache/00535469.tri
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
            /*
             * Raiden - Story (r_plt0)
Snake - Story (r_tnk0)
             */
            ModelsToSwapOut.Add(new MGSModel { Name = "Raiden - Story (r_plt0)"});
            ModelsToSwapOut.Add(new MGSModel { Name = "Snake - Story (r_tnk0)" });
        }

        public ModelSwapperForm(string gameDirectory)
        {
            InitializeComponent();
            BuildModelsToSwapInList();
            BuildModelsToSwapOutList();
            modelToSwapInComboBox.Items.AddRange(ModelsToSwapIn.ToArray());
            modelToSwapOutComboBox.Items.AddRange(ModelsToSwapOut.ToArray());
            _gameDirectory = gameDirectory;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MGSModel modelToSwapIn = modelToSwapInComboBox.SelectedItem as MGSModel;
            List<string> loLodTextureList = GetTexturesFromListOfIds(modelToSwapIn.LoLoDTextures, modelToSwapIn.LoLoDId, "r_plt0");
            List<string> hiLodTextureList = GetTexturesFromListOfIds(modelToSwapIn.HiLoDTextures, modelToSwapIn.HiLoDId, "r_plt0");
            InsertTexturesIntoResidentFile(loLodTextureList, "r_plt0");
            InsertTexturesIntoResidentFile(hiLodTextureList, "r_plt0");
            //assets/tri/us/rai_tex_mt.tri,us/stage/r_plt0/resident/0031ead1.tri,resident/0031ead1.tri
            InsertTriIntoResidentFile("assets/tri/us/ema_def_mh_mt.tri,us/stage/r_plt0/resident/00535469.tri,resident/00535469.tri", "r_plt0");
            InsertTriIntoResidentFile("assets/tri/us/ema_def_sh_mt.tri,us/stage/r_plt0/resident/00b35469.tri,resident/00b35469.tri", "r_plt0");
            //Swap kms and cmdl names
            //Raiden loLod: rai_def
            //Raiden hiLoD: rai_def_sh_mt_stage_r_plt0_r
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
