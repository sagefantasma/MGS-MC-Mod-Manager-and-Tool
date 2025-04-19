namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class MGS3ModdingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGS3ModdingForm));
            hoverPictureBox = new System.Windows.Forms.PictureBox();
            AddMods = new System.Windows.Forms.Button();
            MoveMgs3ModFolder = new System.Windows.Forms.Button();
            MoveVanillaFolder = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            RebuildVanillaFiles = new System.Windows.Forms.Button();
            RefreshMods = new System.Windows.Forms.Button();
            DownloadModsMGS3 = new System.Windows.Forms.Button();
            BackButton = new System.Windows.Forms.Button();
            AudioSwap = new System.Windows.Forms.Button();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)hoverPictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // hoverPictureBox
            // 
            hoverPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            hoverPictureBox.Location = new System.Drawing.Point(8, 219);
            hoverPictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            hoverPictureBox.Name = "hoverPictureBox";
            hoverPictureBox.Size = new System.Drawing.Size(364, 270);
            hoverPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            hoverPictureBox.TabIndex = 23;
            hoverPictureBox.TabStop = false;
            hoverPictureBox.Visible = false;
            // 
            // AddMods
            // 
            AddMods.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            AddMods.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            AddMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            AddMods.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            AddMods.Location = new System.Drawing.Point(8, 147);
            AddMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AddMods.Name = "AddMods";
            AddMods.Size = new System.Drawing.Size(364, 35);
            AddMods.TabIndex = 22;
            AddMods.Text = "Add Mods";
            AddMods.UseVisualStyleBackColor = false;
            AddMods.Click += AddMods_Click;
            // 
            // MoveMgs3ModFolder
            // 
            MoveMgs3ModFolder.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            MoveMgs3ModFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            MoveMgs3ModFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            MoveMgs3ModFolder.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            MoveMgs3ModFolder.Location = new System.Drawing.Point(8, 683);
            MoveMgs3ModFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MoveMgs3ModFolder.Name = "MoveMgs3ModFolder";
            MoveMgs3ModFolder.Size = new System.Drawing.Size(364, 36);
            MoveMgs3ModFolder.TabIndex = 21;
            MoveMgs3ModFolder.Text = "Move MGS3 Mods Folder";
            MoveMgs3ModFolder.UseVisualStyleBackColor = false;
            // 
            // MoveVanillaFolder
            // 
            MoveVanillaFolder.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            MoveVanillaFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            MoveVanillaFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            MoveVanillaFolder.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            MoveVanillaFolder.Location = new System.Drawing.Point(8, 643);
            MoveVanillaFolder.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MoveVanillaFolder.Name = "MoveVanillaFolder";
            MoveVanillaFolder.Size = new System.Drawing.Size(364, 36);
            MoveVanillaFolder.TabIndex = 20;
            MoveVanillaFolder.Text = "Move Vanilla Files Folder";
            MoveVanillaFolder.UseVisualStyleBackColor = false;
            MoveVanillaFolder.Click += MoveVanillaFolder_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            label1.Location = new System.Drawing.Point(727, 14);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(102, 25);
            label1.TabIndex = 19;
            label1.Text = "Mod List";
            // 
            // RebuildVanillaFiles
            // 
            RebuildVanillaFiles.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            RebuildVanillaFiles.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            RebuildVanillaFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            RebuildVanillaFiles.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            RebuildVanillaFiles.Location = new System.Drawing.Point(8, 111);
            RebuildVanillaFiles.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RebuildVanillaFiles.Name = "RebuildVanillaFiles";
            RebuildVanillaFiles.Size = new System.Drawing.Size(364, 35);
            RebuildVanillaFiles.TabIndex = 18;
            RebuildVanillaFiles.Text = "Rebuild Vanilla Files";
            RebuildVanillaFiles.UseVisualStyleBackColor = false;
            RebuildVanillaFiles.Click += RebuildVanillaFiles_Click;
            // 
            // RefreshMods
            // 
            RefreshMods.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            RefreshMods.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            RefreshMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            RefreshMods.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            RefreshMods.Location = new System.Drawing.Point(8, 75);
            RefreshMods.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            RefreshMods.Name = "RefreshMods";
            RefreshMods.Size = new System.Drawing.Size(364, 35);
            RefreshMods.TabIndex = 17;
            RefreshMods.Text = "Refresh Mods";
            RefreshMods.UseVisualStyleBackColor = false;
            RefreshMods.Click += RefreshMods_Click;
            // 
            // DownloadModsMGS3
            // 
            DownloadModsMGS3.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            DownloadModsMGS3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            DownloadModsMGS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            DownloadModsMGS3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            DownloadModsMGS3.Location = new System.Drawing.Point(8, 39);
            DownloadModsMGS3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            DownloadModsMGS3.Name = "DownloadModsMGS3";
            DownloadModsMGS3.Size = new System.Drawing.Size(364, 35);
            DownloadModsMGS3.TabIndex = 16;
            DownloadModsMGS3.Text = "Download Mods";
            DownloadModsMGS3.UseVisualStyleBackColor = false;
            DownloadModsMGS3.Click += DownloadModsMGS3_Click;
            // 
            // BackButton
            // 
            BackButton.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            BackButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            BackButton.Location = new System.Drawing.Point(8, 2);
            BackButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BackButton.Name = "BackButton";
            BackButton.Size = new System.Drawing.Size(364, 35);
            BackButton.TabIndex = 15;
            BackButton.Text = "Back";
            BackButton.UseVisualStyleBackColor = false;
            BackButton.Click += BackButton_Click;
            // 
            // AudioSwap
            // 
            AudioSwap.BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            AudioSwap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            AudioSwap.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            AudioSwap.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            AudioSwap.Location = new System.Drawing.Point(8, 182);
            AudioSwap.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            AudioSwap.Name = "AudioSwap";
            AudioSwap.Size = new System.Drawing.Size(364, 35);
            AudioSwap.TabIndex = 24;
            AudioSwap.Text = "Audio Swap";
            AudioSwap.UseVisualStyleBackColor = false;
            AudioSwap.Click += AudioSwap_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            pictureBox1.Location = new System.Drawing.Point(8, 495);
            pictureBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(364, 142);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 25;
            pictureBox1.TabStop = false;
            pictureBox1.Visible = false;
            // 
            // MGS3ModdingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(149, 149, 125);
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(1339, 722);
            Controls.Add(pictureBox1);
            Controls.Add(AudioSwap);
            Controls.Add(hoverPictureBox);
            Controls.Add(AddMods);
            Controls.Add(MoveMgs3ModFolder);
            Controls.Add(MoveVanillaFolder);
            Controls.Add(label1);
            Controls.Add(RebuildVanillaFiles);
            Controls.Add(RefreshMods);
            Controls.Add(DownloadModsMGS3);
            Controls.Add(BackButton);
            DoubleBuffered = true;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "MGS3ModdingForm";
            Text = "Metal Gear Solid 3: Snake Eater - Modding Menu";
            Load += MGS3ModdingForm_Load;
            ((System.ComponentModel.ISupportInitialize)hoverPictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.PictureBox hoverPictureBox;
        private System.Windows.Forms.Button AddMods;
        private System.Windows.Forms.Button MoveMgs3ModFolder;
        private System.Windows.Forms.Button MoveVanillaFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button RebuildVanillaFiles;
        private System.Windows.Forms.Button RefreshMods;
        private System.Windows.Forms.Button DownloadModsMGS3;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button AudioSwap;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}