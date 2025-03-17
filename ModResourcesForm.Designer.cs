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
            this.BackButton = new System.Windows.Forms.Button();
            this.bakebutton = new System.Windows.Forms.Button();
            this.TextureImportExportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BackButton
            // 
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.BackButton.Location = new System.Drawing.Point(210, 48);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(482, 53);
            this.BackButton.TabIndex = 7;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // bakebutton
            // 
            this.bakebutton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.bakebutton.Location = new System.Drawing.Point(210, 107);
            this.bakebutton.Name = "bakebutton";
            this.bakebutton.Size = new System.Drawing.Size(482, 53);
            this.bakebutton.TabIndex = 8;
            this.bakebutton.Text = "Bake Textures";
            this.bakebutton.UseVisualStyleBackColor = true;
            this.bakebutton.Click += new System.EventHandler(this.bakebutton_Click);
            // 
            // TextureImportExportButton
            // 
            this.TextureImportExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.TextureImportExportButton.Location = new System.Drawing.Point(210, 166);
            this.TextureImportExportButton.Name = "TextureImportExportButton";
            this.TextureImportExportButton.Size = new System.Drawing.Size(482, 53);
            this.TextureImportExportButton.TabIndex = 9;
            this.TextureImportExportButton.Text = "MGS2/MGS3 Texture Import/Export";
            this.TextureImportExportButton.UseVisualStyleBackColor = true;
            this.TextureImportExportButton.Click += new System.EventHandler(this.TextureImportExportButton_Click);
            // 
            // ModResourcesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(901, 649);
            this.Controls.Add(this.TextureImportExportButton);
            this.Controls.Add(this.bakebutton);
            this.Controls.Add(this.BackButton);
            this.Name = "ModResourcesForm";
            this.Text = "ModResources";
            this.Load += new System.EventHandler(this.ModResources_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button bakebutton;
        private System.Windows.Forms.Button TextureImportExportButton;
    }
}