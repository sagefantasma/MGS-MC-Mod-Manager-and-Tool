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
            _stageTreeView = new System.Windows.Forms.TreeView();
            _stageResourcesListBox = new System.Windows.Forms.ListBox();
            _availableResourcesListBox = new System.Windows.Forms.CheckedListBox();
            saveButton = new System.Windows.Forms.Button();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.Color.LightSteelBlue;
            tableLayoutPanel1.ColumnCount = 3;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            tableLayoutPanel1.Controls.Add(_stageTreeView, 0, 0);
            tableLayoutPanel1.Controls.Add(_stageResourcesListBox, 1, 0);
            tableLayoutPanel1.Controls.Add(_availableResourcesListBox, 2, 0);
            tableLayoutPanel1.Controls.Add(saveButton, 2, 1);
            tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(1084, 661);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // _stageTreeView
            // 
            _stageTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            _stageTreeView.Location = new System.Drawing.Point(3, 3);
            _stageTreeView.Name = "_stageTreeView";
            _stageTreeView.Size = new System.Drawing.Size(210, 588);
            _stageTreeView.TabIndex = 0;
            _stageTreeView.NodeMouseDoubleClick += OpenStage;
            // 
            // _stageResourcesListBox
            // 
            _stageResourcesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _stageResourcesListBox.FormattingEnabled = true;
            _stageResourcesListBox.ItemHeight = 15;
            _stageResourcesListBox.Location = new System.Drawing.Point(219, 3);
            _stageResourcesListBox.Name = "_stageResourcesListBox";
            _stageResourcesListBox.Size = new System.Drawing.Size(427, 588);
            _stageResourcesListBox.TabIndex = 1;
            // 
            // _availableResourcesListBox
            // 
            _availableResourcesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            _availableResourcesListBox.FormattingEnabled = true;
            _availableResourcesListBox.Location = new System.Drawing.Point(652, 3);
            _availableResourcesListBox.Name = "_availableResourcesListBox";
            _availableResourcesListBox.Size = new System.Drawing.Size(429, 588);
            _availableResourcesListBox.TabIndex = 2;
            // 
            // saveButton
            // 
            saveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            saveButton.Location = new System.Drawing.Point(652, 597);
            saveButton.Name = "saveButton";
            saveButton.Size = new System.Drawing.Size(429, 61);
            saveButton.TabIndex = 3;
            saveButton.Text = "Save Resources";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // ResourceFileEditorForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1084, 661);
            Controls.Add(tableLayoutPanel1);
            Name = "ResourceFileEditorForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "EZ Resource File Editor";
            tableLayoutPanel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TreeView _stageTreeView;
        private System.Windows.Forms.ListBox _stageResourcesListBox;
        private System.Windows.Forms.CheckedListBox _availableResourcesListBox;
        private System.Windows.Forms.Button saveButton;
    }
}