namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class TextureModelForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.DdsFilePictureBox = new System.Windows.Forms.PictureBox();
            this.btnLoadObj = new System.Windows.Forms.Button();
            this.LoadGruButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DdsFilePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // BackButton
            // 
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BackButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.BackButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.BackButton.Location = new System.Drawing.Point(12, 12);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(218, 31);
            this.BackButton.TabIndex = 7;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(355, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 25);
            this.label1.TabIndex = 11;
            this.label1.Text = "Textures:";
            // 
            // DdsFilePictureBox
            // 
            this.DdsFilePictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.DdsFilePictureBox.Location = new System.Drawing.Point(12, 152);
            this.DdsFilePictureBox.Name = "DdsFilePictureBox";
            this.DdsFilePictureBox.Size = new System.Drawing.Size(218, 127);
            this.DdsFilePictureBox.TabIndex = 12;
            this.DdsFilePictureBox.TabStop = false;
            // 
            // btnLoadObj
            // 
            this.btnLoadObj.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.btnLoadObj.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnLoadObj.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.btnLoadObj.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLoadObj.Location = new System.Drawing.Point(12, 52);
            this.btnLoadObj.Name = "btnLoadObj";
            this.btnLoadObj.Size = new System.Drawing.Size(218, 31);
            this.btnLoadObj.TabIndex = 13;
            this.btnLoadObj.Text = "Load OBJ Model";
            this.btnLoadObj.UseVisualStyleBackColor = false;
            this.btnLoadObj.Click += new System.EventHandler(this.btnLoadObj_Click);
            // 
            // LoadGruButton
            // 
            this.LoadGruButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.LoadGruButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.LoadGruButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.LoadGruButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.LoadGruButton.Location = new System.Drawing.Point(12, 89);
            this.LoadGruButton.Name = "LoadGruButton";
            this.LoadGruButton.Size = new System.Drawing.Size(218, 31);
            this.LoadGruButton.TabIndex = 14;
            this.LoadGruButton.Text = "Load MGS3 GRU";
            this.LoadGruButton.UseVisualStyleBackColor = false;
            this.LoadGruButton.Click += new System.EventHandler(this.LoadGruButton_Click);
            // 
            // TextureModelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1196, 626);
            this.Controls.Add(this.LoadGruButton);
            this.Controls.Add(this.btnLoadObj);
            this.Controls.Add(this.DdsFilePictureBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BackButton);
            this.DoubleBuffered = true;
            this.Name = "TextureModelForm";
            this.Text = "TextureModelForm";
            ((System.ComponentModel.ISupportInitialize)(this.DdsFilePictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox DdsFilePictureBox;
        private System.Windows.Forms.Button btnLoadObj;
        private System.Windows.Forms.Button LoadGruButton;
    }
}