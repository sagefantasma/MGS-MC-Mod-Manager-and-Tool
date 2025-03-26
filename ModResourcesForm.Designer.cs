namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class ModResourcesForm
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
            BackButton = new System.Windows.Forms.Button();
            TextureImportExportButton = new System.Windows.Forms.Button();
            button1 = new System.Windows.Forms.Button();
            MGS3GuardRouteTool = new System.Windows.Forms.Button();
            button3 = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // BackButton
            // 
            BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            BackButton.Location = new System.Drawing.Point(245, 55);
            BackButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            BackButton.Name = "BackButton";
            BackButton.Size = new System.Drawing.Size(562, 61);
            BackButton.TabIndex = 7;
            BackButton.Text = "Back";
            BackButton.UseVisualStyleBackColor = true;
            BackButton.Click += BackButton_Click;
            // 
            // TextureImportExportButton
            // 
            TextureImportExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            TextureImportExportButton.Location = new System.Drawing.Point(245, 123);
            TextureImportExportButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            TextureImportExportButton.Name = "TextureImportExportButton";
            TextureImportExportButton.Size = new System.Drawing.Size(562, 61);
            TextureImportExportButton.TabIndex = 9;
            TextureImportExportButton.Text = "MGS2/MGS3 Texture Import/Export";
            TextureImportExportButton.UseVisualStyleBackColor = true;
            TextureImportExportButton.Click += TextureImportExportButton_Click;
            // 
            // button1
            // 
            button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            button1.Location = new System.Drawing.Point(245, 192);
            button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(562, 61);
            button1.TabIndex = 10;
            button1.Text = "MGS2 Guard Route Tool";
            button1.UseVisualStyleBackColor = true;
            // 
            // MGS3GuardRouteTool
            // 
            MGS3GuardRouteTool.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            MGS3GuardRouteTool.Location = new System.Drawing.Point(245, 260);
            MGS3GuardRouteTool.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            MGS3GuardRouteTool.Name = "MGS3GuardRouteTool";
            MGS3GuardRouteTool.Size = new System.Drawing.Size(562, 61);
            MGS3GuardRouteTool.TabIndex = 11;
            MGS3GuardRouteTool.Text = "MGS3 Guard Route Tool";
            MGS3GuardRouteTool.UseVisualStyleBackColor = true;
            MGS3GuardRouteTool.Click += MGS3GuardRouteTool_Click;
            // 
            // button3
            // 
            button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            button3.Location = new System.Drawing.Point(245, 328);
            button3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new System.Drawing.Size(562, 61);
            button3.TabIndex = 12;
            button3.Text = "MGS2 C4 Bomb Location Tool";
            button3.UseVisualStyleBackColor = true;
            // 
            // ModResourcesForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1051, 749);
            Controls.Add(button3);
            Controls.Add(MGS3GuardRouteTool);
            Controls.Add(button1);
            Controls.Add(TextureImportExportButton);
            Controls.Add(BackButton);
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "ModResourcesForm";
            Text = "ModResources";
            Load += ModResources_Load;
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button TextureImportExportButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button MGS3GuardRouteTool;
        private System.Windows.Forms.Button button3;
    }
}