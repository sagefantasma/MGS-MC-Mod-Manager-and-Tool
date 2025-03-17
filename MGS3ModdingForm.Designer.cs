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
            this.hoverPictureBox = new System.Windows.Forms.PictureBox();
            this.AddMods = new System.Windows.Forms.Button();
            this.MoveMgs3ModFolder = new System.Windows.Forms.Button();
            this.MoveVanillaFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.RebuildVanillaFiles = new System.Windows.Forms.Button();
            this.RefreshMods = new System.Windows.Forms.Button();
            this.DownloadModsMGS3 = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.AudioSwap = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.hoverPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // hoverPictureBox
            // 
            this.hoverPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.hoverPictureBox.Location = new System.Drawing.Point(7, 190);
            this.hoverPictureBox.Name = "hoverPictureBox";
            this.hoverPictureBox.Size = new System.Drawing.Size(312, 265);
            this.hoverPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.hoverPictureBox.TabIndex = 23;
            this.hoverPictureBox.TabStop = false;
            this.hoverPictureBox.Visible = false;
            // 
            // AddMods
            // 
            this.AddMods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.AddMods.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AddMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.AddMods.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.AddMods.Location = new System.Drawing.Point(7, 127);
            this.AddMods.Name = "AddMods";
            this.AddMods.Size = new System.Drawing.Size(312, 30);
            this.AddMods.TabIndex = 22;
            this.AddMods.Text = "Add Mods";
            this.AddMods.UseVisualStyleBackColor = false;
            this.AddMods.Click += new System.EventHandler(this.AddMods_Click);
            // 
            // MoveMgs3ModFolder
            // 
            this.MoveMgs3ModFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.MoveMgs3ModFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.MoveMgs3ModFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.MoveMgs3ModFolder.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MoveMgs3ModFolder.Location = new System.Drawing.Point(7, 592);
            this.MoveMgs3ModFolder.Name = "MoveMgs3ModFolder";
            this.MoveMgs3ModFolder.Size = new System.Drawing.Size(312, 31);
            this.MoveMgs3ModFolder.TabIndex = 21;
            this.MoveMgs3ModFolder.Text = "Move MGS3 Mods Folder";
            this.MoveMgs3ModFolder.UseVisualStyleBackColor = false;
            // 
            // MoveVanillaFolder
            // 
            this.MoveVanillaFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.MoveVanillaFolder.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.MoveVanillaFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.MoveVanillaFolder.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.MoveVanillaFolder.Location = new System.Drawing.Point(7, 557);
            this.MoveVanillaFolder.Name = "MoveVanillaFolder";
            this.MoveVanillaFolder.Size = new System.Drawing.Size(312, 31);
            this.MoveVanillaFolder.TabIndex = 20;
            this.MoveVanillaFolder.Text = "Move Vanilla Files Folder";
            this.MoveVanillaFolder.UseVisualStyleBackColor = false;
            this.MoveVanillaFolder.Click += new System.EventHandler(this.MoveVanillaFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label1.Location = new System.Drawing.Point(623, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 25);
            this.label1.TabIndex = 19;
            this.label1.Text = "Mod List";
            // 
            // RebuildVanillaFiles
            // 
            this.RebuildVanillaFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.RebuildVanillaFiles.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RebuildVanillaFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.RebuildVanillaFiles.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.RebuildVanillaFiles.Location = new System.Drawing.Point(7, 96);
            this.RebuildVanillaFiles.Name = "RebuildVanillaFiles";
            this.RebuildVanillaFiles.Size = new System.Drawing.Size(312, 30);
            this.RebuildVanillaFiles.TabIndex = 18;
            this.RebuildVanillaFiles.Text = "Rebuild Vanilla Files";
            this.RebuildVanillaFiles.UseVisualStyleBackColor = false;
            this.RebuildVanillaFiles.Click += new System.EventHandler(this.RebuildVanillaFiles_Click);
            // 
            // RefreshMods
            // 
            this.RefreshMods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.RefreshMods.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.RefreshMods.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.RefreshMods.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.RefreshMods.Location = new System.Drawing.Point(7, 65);
            this.RefreshMods.Name = "RefreshMods";
            this.RefreshMods.Size = new System.Drawing.Size(312, 30);
            this.RefreshMods.TabIndex = 17;
            this.RefreshMods.Text = "Refresh Mods";
            this.RefreshMods.UseVisualStyleBackColor = false;
            this.RefreshMods.Click += new System.EventHandler(this.RefreshMods_Click);
            // 
            // DownloadModsMGS3
            // 
            this.DownloadModsMGS3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.DownloadModsMGS3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DownloadModsMGS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.DownloadModsMGS3.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.DownloadModsMGS3.Location = new System.Drawing.Point(7, 34);
            this.DownloadModsMGS3.Name = "DownloadModsMGS3";
            this.DownloadModsMGS3.Size = new System.Drawing.Size(312, 30);
            this.DownloadModsMGS3.TabIndex = 16;
            this.DownloadModsMGS3.Text = "Download Mods";
            this.DownloadModsMGS3.UseVisualStyleBackColor = false;
            this.DownloadModsMGS3.Click += new System.EventHandler(this.DownloadModsMGS3_Click);
            // 
            // BackButton
            // 
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.BackButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackButton.Location = new System.Drawing.Point(7, 2);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(312, 30);
            this.BackButton.TabIndex = 15;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // AudioSwap
            // 
            this.AudioSwap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.AudioSwap.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.AudioSwap.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.AudioSwap.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.AudioSwap.Location = new System.Drawing.Point(7, 158);
            this.AudioSwap.Name = "AudioSwap";
            this.AudioSwap.Size = new System.Drawing.Size(312, 30);
            this.AudioSwap.TabIndex = 24;
            this.AudioSwap.Text = "Audio Swap";
            this.AudioSwap.UseVisualStyleBackColor = false;
            this.AudioSwap.Click += new System.EventHandler(this.AudioSwap_Click);
            // 
            // MGS3ModdingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(149)))), ((int)(((byte)(149)))), ((int)(((byte)(125)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1148, 626);
            this.Controls.Add(this.AudioSwap);
            this.Controls.Add(this.hoverPictureBox);
            this.Controls.Add(this.AddMods);
            this.Controls.Add(this.MoveMgs3ModFolder);
            this.Controls.Add(this.MoveVanillaFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.RebuildVanillaFiles);
            this.Controls.Add(this.RefreshMods);
            this.Controls.Add(this.DownloadModsMGS3);
            this.Controls.Add(this.BackButton);
            this.DoubleBuffered = true;
            this.Name = "MGS3ModdingForm";
            this.Text = "Metal Gear Solid 3: Snake Eater - Modding Menu";
            this.Load += new System.EventHandler(this.MGS3ModdingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.hoverPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}