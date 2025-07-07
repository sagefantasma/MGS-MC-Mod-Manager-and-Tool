using Newtonsoft.Json;
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
using static ANTIBigBoss_MGS_Mod_Manager.ResourceFileEditor;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    public partial class ResourceFileEditorForm : Form
    {
        private string _mainPath;
        private Stage _activeStage;
        private List<IResource> masterResourceList;
        private string _masterResourceListCachedFile = "masterResourceList.json";

        public ResourceFileEditorForm(string path)
        {
            _mainPath = path;
            ResourceFileEditor.BaseDirectory = _mainPath;
            InitializeComponent();

            _availableResourcesListBox.DisplayMember = "Name";
            _availableResourcesListBox.Enabled = false;
            string stageDirectory = Path.Combine(_mainPath, "eu", "stage");
            DirectoryInfo stageDirectoryInfo = new DirectoryInfo(stageDirectory);
            foreach (DirectoryInfo directoryInfo in stageDirectoryInfo.GetDirectories())
            {
                _stageTreeView.Nodes.Add(directoryInfo.Name);
            }

            PopulateAvailableResources();
        }

        private void PopulateAvailableResources()
        {
            if (File.Exists(_masterResourceListCachedFile))
            {
                string cachedContents = File.ReadAllText(_masterResourceListCachedFile);
                //masterResourceList = System.Text.Json.JsonSerializer.Deserialize<List<IResource>>(cachedContents);
                masterResourceList = JsonConvert.DeserializeObject<List<IResource>>(cachedContents, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto});
            }
            else
            {
                List<Stage> allStages = new List<Stage>();
                foreach (TreeNode node in _stageTreeView.Nodes)
                {
                    allStages.Add(new Stage(node.Text));
                }

                masterResourceList = new List<IResource>();
                foreach (Stage stage in allStages)
                {
                    foreach (IResource resource in stage.ResourceList)
                    {
                        if (resource is Ctxr)
                        {
                            List<Ctxr> masterCtxr = masterResourceList.Where(x => x is Ctxr).Cast<Ctxr>().ToList();
                            if (!masterCtxr.Any(x => x.Name == resource.Name && x.ID == resource.ID && x.IDMappedTo == (resource as Ctxr).IDMappedTo))
                            {
                                masterResourceList.Add(resource);
                            }
                        }
                        else if (!masterResourceList.Any(x => x.Name == resource.Name && x.ID == resource.ID))
                        {
                            masterResourceList.Add(resource);
                        }
                    }
                }

                masterResourceList.Sort(new CompareResourceNames());

                //File.WriteAllText(_masterResourceListCachedFile, System.Text.Json.JsonSerializer.Serialize(masterResourceList));
                File.WriteAllText(_masterResourceListCachedFile, JsonConvert.SerializeObject(masterResourceList, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
            }

            foreach (IResource masterResource in masterResourceList)
            {
                //if (!masterResource.Path.Contains("cmdl"))
                _availableResourcesListBox.Items.Add(masterResource);
            }
        }

        private class CompareResourceNames : IComparer<IResource>
        {
            public int Compare(IResource x, IResource y)
            {
                return x.Name.CompareTo(y.Name);
            }
        }

        private class CompareResourcePaths : IComparer<IResource>
        {
            public int Compare(IResource x, IResource y)
            {
                return x.Path.CompareTo(y.Path);
            }
        }

        private void OpenStage(object sender, TreeNodeMouseClickEventArgs e)
        {
            _stageResourcesListBox.Items.Clear();
            for (int i = 0; i < _availableResourcesListBox.Items.Count; i++)
            {
                _availableResourcesListBox.SetItemChecked(i, false);
            }
            TreeNode senderNode = (sender as TreeView).SelectedNode;
            _activeStage = new Stage(senderNode.Text);

            foreach (IResource resource in _activeStage.ResourceList)
            {
                //if (!resource.Path.Contains("cmdl"))
                //{
                _stageResourcesListBox.Items.Add(resource.Name);
                int index = _availableResourcesListBox.FindString(resource.Name); //TODO: this needs to be more specific
                //index = _availableResourcesListBox.Items.Cast<IResource>().First(resource)
                IResource masterVersion;
                if (resource is Ctxr)
                {
                    masterVersion = masterResourceList.Where(x => x is Ctxr).Cast<Ctxr>().First(y => y.ID == resource.ID && y.IDMappedTo == (resource as Ctxr).IDMappedTo);
                }
                else
                {
                    masterVersion = masterResourceList.First(x => x.Name == resource.Name && x.ID == resource.ID);
                }
                index = masterResourceList.IndexOf(masterVersion);
                //index = _availableResourcesListBox.Items.IndexOf(resource);
                _availableResourcesListBox.SetItemChecked(index, true);
                //}
            }
            _availableResourcesListBox.Enabled = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            //not changing anything is resulting in different results... y?
            List<byte> bpAssetsContents = new List<byte>();
            List<byte> manifestContents = new List<byte>();
            List<IResource> checkedResources = _availableResourcesListBox.CheckedItems.Cast<IResource>().ToList();
            //checkedResources.Sort(new CompareResourcePaths());
            List<IResource> manifestResources = new List<IResource>();
            List<IResource> bpAssetsResources = new List<IResource>();
            foreach (IResource checkedResource in checkedResources)
            {
                if (checkedResource.Path.Contains("ctxr") || checkedResource.Path.Contains("cmdl"))
                {
                    bpAssetsResources.Add(checkedResource);
                }
                else
                {
                    manifestResources.Add(checkedResource);
                }
            }

            manifestResources = SortManifestResources(manifestResources);
            bpAssetsResources = SortBpAssetsResources(bpAssetsResources);

            foreach(IResource bpAssetsResource in bpAssetsResources)
            {
                string modifiedPath = ModifyPath(bpAssetsResource.Path);

                bpAssetsContents.AddRange(Encoding.UTF8.GetBytes(modifiedPath));
                bpAssetsContents.AddRange(Encoding.UTF8.GetBytes("\r\r\n"));
            }
            foreach (IResource manifestResource in manifestResources)
            {
                string modifiedPath = ModifyPath(manifestResource.Path);

                manifestContents.AddRange(Encoding.UTF8.GetBytes(modifiedPath));
                manifestContents.AddRange(Encoding.UTF8.GetBytes("\r\r\n"));
            }

            File.WriteAllBytes($"{_activeStage.Name}bp_assets.txt",bpAssetsContents.ToArray());
            File.WriteAllBytes($"{_activeStage.Name}manifest.txt",manifestContents.ToArray());
        }

        private List<IResource> SortBpAssetsResources(List<IResource> resources)
        {
            List<IResource> textures = resources.FindAll(resource => resource.Path.Contains(".ctxr"));
            textures.Sort(new CompareResourceNames());
            List <IResource> evmCmdl = resources.FindAll(resource => resource.Path.Contains("/evm/"));
            evmCmdl.Sort(new CompareResourceNames());
            List<IResource> kmsCmdl = resources.FindAll(resource => resource.Path.Contains("/kms/"));
            kmsCmdl.Sort(new CompareResourceNames());

            resources.Clear();
            resources.AddRange(textures);
            resources.AddRange(evmCmdl);
            resources.AddRange(kmsCmdl);
            return resources;
        }

        private List<IResource> SortManifestResources(List<IResource> resources)
        {
            //seems like the order might not matter(within blocks), which is interesting and good to know! Still works without reversing
            List<IResource> triFiles = resources.FindAll(resource => resource.Path.Contains("/tri/"));
            triFiles.Sort(new CompareResourceNames());
            List<IResource> hzxFiles = resources.FindAll(resource => resource.Path.Contains("/hzx/"));
            hzxFiles.Sort(new CompareResourceNames());
            List<IResource> varFiles = resources.FindAll(resource => resource.Path.Contains("/var/"));
            varFiles.Sort(new CompareResourceNames());
            List<IResource> sarFiles = resources.FindAll(resource => resource.Path.Contains("/sar/"));
            sarFiles.Sort(new CompareResourceNames());
            List<IResource> rowFiles = resources.FindAll(resource => resource.Path.Contains("/row/"));
            rowFiles.Sort(new CompareResourceNames());
            //rowFiles.Reverse();
            List<IResource> o2dFiles = resources.FindAll(resource => resource.Path.Contains("/o2d/"));
            o2dFiles.Sort(new CompareResourceNames());
            List<IResource> marFiles = resources.FindAll(resource => resource.Path.Contains("/mar/"));
            marFiles.Sort(new CompareResourceNames());
            List<IResource> lt2Files = resources.FindAll(resource => resource.Path.Contains("/lt2/"));
            lt2Files.Sort(new CompareResourceNames());
            List<IResource> kmsFiles = resources.FindAll(resource => resource.Path.Contains("/kms/"));
            kmsFiles.Sort(new CompareResourceNames());
            //kmsFiles.Reverse();
            List<IResource> farFiles = resources.FindAll(resource => resource.Path.Contains("/far/"));
            farFiles.Sort(new CompareResourceNames());
            List<IResource> evmFiles = resources.FindAll(resource => resource.Path.Contains("/evm/"));
            evmFiles.Sort(new CompareResourceNames());
            List<IResource> cv2Files = resources.FindAll(resource => resource.Path.Contains("/cv2/"));
            cv2Files.Sort(new CompareResourceNames());
            //cv2Files.Reverse();
            List<IResource> anmFiles = resources.FindAll(resource => resource.Path.Contains("/anm/"));
            anmFiles.Sort(new CompareResourceNames());
            List<IResource> gcxFiles = resources.FindAll(resource => resource.Path.Contains("/gcx/"));
            gcxFiles.Sort(new CompareResourceNames());

            resources.Clear();
            resources.AddRange(triFiles);
            resources.AddRange(hzxFiles);
            resources.AddRange(varFiles);
            resources.AddRange(sarFiles);
            resources.AddRange(rowFiles);
            resources.AddRange(o2dFiles);
            resources.AddRange(marFiles);
            resources.AddRange(lt2Files);
            resources.AddRange(kmsFiles);
            resources.AddRange(farFiles);
            resources.AddRange(evmFiles);
            resources.AddRange(cv2Files);
            resources.AddRange(anmFiles);
            resources.AddRange(gcxFiles);

            return resources;
        }

        private string ModifyPath(string inputPath)
        {
            string[] pathParts = inputPath.Split(',');
            string secondPath = pathParts[1];
            List<string> secondPathParts = secondPath.Split("/").ToList();
            int stagePart = secondPathParts.IndexOf("stage");
            string oldStageCode = secondPathParts[stagePart + 1];
            return inputPath.Replace($"/{oldStageCode}/", $"/{_activeStage.Name}/");
        }
    }
}
