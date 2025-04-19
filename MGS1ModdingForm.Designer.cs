namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class MGS1ModdingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MGS1ModdingForm));
            BackButton = new System.Windows.Forms.Button();
            MoveMGS1ModFolder = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // BackButton
            // 
            BackButton.BackgroundImage = (System.Drawing.Image)resources.GetObject("BackButton.BackgroundImage");
            BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            BackButton.Location = new System.Drawing.Point(3, 12);
            BackButton.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            BackButton.Name = "BackButton";
            BackButton.Size = new System.Drawing.Size(149, 57);
            BackButton.TabIndex = 9;
            BackButton.Text = "Back";
            BackButton.UseVisualStyleBackColor = true;
            BackButton.Click += BackButton_Click;
            // 
            // MoveMGS1ModFolder
            // 
            MoveMGS1ModFolder.BackgroundImage = (System.Drawing.Image)resources.GetObject("MoveMGS1ModFolder.BackgroundImage");
            MoveMGS1ModFolder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            MoveMGS1ModFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            MoveMGS1ModFolder.Location = new System.Drawing.Point(3, 75);
            MoveMGS1ModFolder.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            MoveMGS1ModFolder.Name = "MoveMGS1ModFolder";
            MoveMGS1ModFolder.Size = new System.Drawing.Size(149, 99);
            MoveMGS1ModFolder.TabIndex = 10;
            MoveMGS1ModFolder.Text = "Move MGS1 Mods Folder";
            MoveMGS1ModFolder.UseVisualStyleBackColor = true;
            MoveMGS1ModFolder.Click += MoveMGS1ModFolder_Click;
            // 
            // MGS1ModdingForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(887, 623);
            Controls.Add(MoveMGS1ModFolder);
            Controls.Add(BackButton);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Consolas", 10.2F, System.Drawing.FontStyle.Bold);
            ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            MaximumSize = new System.Drawing.Size(903, 662);
            MinimumSize = new System.Drawing.Size(903, 662);
            Name = "MGS1ModdingForm";
            Text = "Metal Gear Solid 1 - Modding Menu";
            Load += MGS1ModdingForm_Load;
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button MoveMGS1ModFolder;
    }
}