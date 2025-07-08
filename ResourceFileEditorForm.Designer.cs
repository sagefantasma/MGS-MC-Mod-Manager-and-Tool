namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class ResourceFileEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            addRemoveResourcesLabel = new System.Windows.Forms.Label();
            stageResourcesLabel = new System.Windows.Forms.Label();
            _stageTreeView = new System.Windows.Forms.TreeView();
            _stageResourcesListBox = new System.Windows.Forms.ListBox();
            _availableResourcesListBox = new System.Windows.Forms.CheckedListBox();
            saveButton = new System.Windows.Forms.Button();
            stageTreeLabel = new System.Windows.Forms.Label();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            copyButton = new System.Windows.Forms.Button();
            pasteButton = new System.Windows.Forms.Button();
            clearClipboardButton = new System.Windows.Forms.Button();
            flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            triTreeBtn = new System.Windows.Forms.Button();
            refreshButton = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.Controls.Add(addRemoveResourcesLabel, 2, 0);
            tableLayoutPanel1.Controls.Add(stageResourcesLabel, 1, 0);
            tableLayoutPanel1.Controls.Add(_stageTreeView, 0, 1);
            tableLayoutPanel1.Controls.Add(_stageResourcesListBox, 1, 1);
            tableLayoutPanel1.Controls.Add(_availableResourcesListBox, 2, 1);
            tableLayoutPanel1.Controls.Add(saveButton, 2, 2);
            tableLayoutPanel1.Controls.Add(stageTreeLabel, 0, 0);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 1, 2);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 2);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 87.5F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.5F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1384, 661);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // addRemoveResourcesLabel
            // 
            addRemoveResourcesLabel.AutoSize = true;
            addRemoveResourcesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            addRemoveResourcesLabel.Location = new System.Drawing.Point(832, 0);
            addRemoveResourcesLabel.Name = "addRemoveResourcesLabel";
            addRemoveResourcesLabel.Size = new System.Drawing.Size(549, 33);
            addRemoveResourcesLabel.TabIndex = 7;
            addRemoveResourcesLabel.Text = "Add/Remove Resources to Selected Stage";
            addRemoveResourcesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // stageResourcesLabel
            // 
            stageResourcesLabel.AutoSize = true;
            stageResourcesLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            stageResourcesLabel.Location = new System.Drawing.Point(279, 0);
            stageResourcesLabel.Name = "stageResourcesLabel";
            stageResourcesLabel.Size = new System.Drawing.Size(547, 33);
            stageResourcesLabel.TabIndex = 6;
            stageResourcesLabel.Text = "Selected Stage's Resources";
            stageResourcesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _stageTreeView
            // 
            _stageTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            _stageTreeView.Location = new System.Drawing.Point(3, 36);
            _stageTreeView.Name = "_stageTreeView";
            _stageTreeView.Size = new System.Drawing.Size(270, 572);
            _stageTreeView.TabIndex = 0;
            _stageTreeView.NodeMouseDoubleClick += StageTreeView_NodeMouseDoubleClicked;
            // 
            // _stageResourcesListBox
            // 
            _stageResourcesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _stageResourcesListBox.FormattingEnabled = true;
            _stageResourcesListBox.ItemHeight = 15;
            _stageResourcesListBox.Location = new System.Drawing.Point(279, 36);
            _stageResourcesListBox.Name = "_stageResourcesListBox";
            _stageResourcesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            _stageResourcesListBox.Size = new System.Drawing.Size(547, 572);
            _stageResourcesListBox.TabIndex = 1;
            // 
            // _availableResourcesListBox
            // 
            _availableResourcesListBox.CheckOnClick = true;
            _availableResourcesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _availableResourcesListBox.FormattingEnabled = true;
            _availableResourcesListBox.Location = new System.Drawing.Point(832, 36);
            _availableResourcesListBox.Name = "_availableResourcesListBox";
            _availableResourcesListBox.Size = new System.Drawing.Size(549, 572);
            _availableResourcesListBox.TabIndex = 2;
            // 
            // saveButton
            // 
            saveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            saveButton.Location = new System.Drawing.Point(832, 614);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(549, 44);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save Selected Resources to Selected Stage";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // stageTreeLabel
            // 
            stageTreeLabel.AutoSize = true;
            stageTreeLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            stageTreeLabel.Location = new System.Drawing.Point(3, 0);
            stageTreeLabel.Name = "stageTreeLabel";
            stageTreeLabel.Size = new System.Drawing.Size(270, 33);
            stageTreeLabel.TabIndex = 5;
            stageTreeLabel.Text = "Stage to Edit";
            stageTreeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(copyButton);
            flowLayoutPanel1.Controls.Add(pasteButton);
            flowLayoutPanel1.Controls.Add(clearClipboardButton);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(279, 614);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(547, 44);
            flowLayoutPanel1.TabIndex = 8;
            // 
            // copyButton
            // 
            copyButton.Location = new System.Drawing.Point(3, 3);
            copyButton.Name = "copyButton";
            copyButton.Size = new System.Drawing.Size(197, 41);
            copyButton.TabIndex = 0;
            copyButton.Text = "Copy Selected Resources To Store";
            copyButton.UseVisualStyleBackColor = true;
            copyButton.Click += copyButton_Click;
            // 
            // pasteButton
            // 
            pasteButton.Enabled = false;
            pasteButton.Location = new System.Drawing.Point(206, 3);
            pasteButton.Name = "pasteButton";
            pasteButton.Size = new System.Drawing.Size(136, 41);
            pasteButton.TabIndex = 1;
            pasteButton.Text = "Paste Stored Resources";
            pasteButton.UseVisualStyleBackColor = true;
            pasteButton.Click += pasteButton_Click;
            // 
            // clearClipboardButton
            // 
            clearClipboardButton.Enabled = false;
            clearClipboardButton.Location = new System.Drawing.Point(348, 3);
            clearClipboardButton.Name = "clearClipboardButton";
            clearClipboardButton.Size = new System.Drawing.Size(75, 41);
            clearClipboardButton.TabIndex = 2;
            clearClipboardButton.Text = "Clear Store";
            clearClipboardButton.UseVisualStyleBackColor = true;
            clearClipboardButton.Click += clearClipboardButton_Click;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(triTreeBtn);
            flowLayoutPanel2.Controls.Add(refreshButton);
            flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel2.Location = new System.Drawing.Point(3, 614);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new System.Drawing.Size(270, 44);
            flowLayoutPanel2.TabIndex = 9;
            // 
            // triTreeBtn
            // 
            triTreeBtn.Location = new System.Drawing.Point(3, 3);
            triTreeBtn.Name = "triTreeBtn";
            triTreeBtn.Size = new System.Drawing.Size(83, 41);
            triTreeBtn.TabIndex = 10;
            triTreeBtn.Text = "Build Tri Tree";
            triTreeBtn.UseVisualStyleBackColor = true;
            // 
            // refreshButton
            // 
            refreshButton.Location = new System.Drawing.Point(92, 3);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new System.Drawing.Size(139, 41);
            refreshButton.TabIndex = 11;
            refreshButton.Text = "Refresh Selected Stage";
            refreshButton.UseVisualStyleBackColor = true;
            refreshButton.Click += refreshButton_Click;
            // 
            // ResourceFileEditorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1384, 661);
            Controls.Add(tableLayoutPanel1);
            Name = "ResourceFileEditorForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "EZ Resource File Editor";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView _stageTreeView;
        private System.Windows.Forms.ListBox _stageResourcesListBox;
        private System.Windows.Forms.CheckedListBox _availableResourcesListBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label addRemoveResourcesLabel;
        private System.Windows.Forms.Label stageResourcesLabel;
        private System.Windows.Forms.Label stageTreeLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button pasteButton;
        private System.Windows.Forms.Button clearClipboardButton;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Button triTreeBtn;
        private System.Windows.Forms.Button refreshButton;
    }
}