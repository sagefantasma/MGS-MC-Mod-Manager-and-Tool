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
            List<Stage> allStages = new List<Stage>();
            foreach (TreeNode node in _stageTreeView.Nodes)
            {
                allStages.Add(new Stage(node.Text));
            }

            List<IResource> masterResourceList = new List<IResource>();
            foreach (Stage stage in allStages)
            {
                foreach (IResource resource in stage.ResourceList)
                {
                    if (!masterResourceList.Any(x => x.Name == resource.Name))
                    {
                        masterResourceList.Add(resource);
                    }
                }
            }

            masterResourceList.Sort(new CompareResources());

            foreach (IResource masterResource in masterResourceList)
            {
                if (!masterResource.Path.Contains("cmdl"))
                    _availableResourcesListBox.Items.Add(masterResource);
            }
        }

        private class CompareResources : IComparer<IResource>
        {
            public int Compare(IResource x, IResource y)
            {
                return x.Name.CompareTo(y.Name);
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
            Stage stage = new Stage(senderNode.Text);
            foreach (IResource resource in stage.ResourceList)
            {
                if (!resource.Path.Contains("cmdl"))
                {
                    _stageResourcesListBox.Items.Add(resource.Name);
                    int index = _availableResourcesListBox.FindString(resource.Name);
                    _availableResourcesListBox.SetItemChecked(index, true);
                }
            }
            _availableResourcesListBox.Enabled = true;
        }
    }
}
