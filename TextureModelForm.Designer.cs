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
            this.btnLoadObj = new System.Windows.Forms.Button();
            this.LoadGruButton = new System.Windows.Forms.Button();
            this.DdsFilePictureBox = new System.Windows.Forms.PictureBox();
            this.CtxrToPng = new System.Windows.Forms.Button();
            this.PngToCtxr = new System.Windows.Forms.Button();
            this.PngToDds = new System.Windows.Forms.Button();
            this.DdsToCtxr = new System.Windows.Forms.Button();
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
            // DdsFilePictureBox
            // 
            this.DdsFilePictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.DdsFilePictureBox.Location = new System.Drawing.Point(966, 12);
            this.DdsFilePictureBox.Name = "DdsFilePictureBox";
            this.DdsFilePictureBox.Size = new System.Drawing.Size(218, 127);
            this.DdsFilePictureBox.TabIndex = 12;
            this.DdsFilePictureBox.TabStop = false;
            // 
            // CtxrToPng
            // 
            this.CtxrToPng.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.CtxrToPng.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.CtxrToPng.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.CtxrToPng.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CtxrToPng.Location = new System.Drawing.Point(12, 126);
            this.CtxrToPng.Name = "CtxrToPng";
            this.CtxrToPng.Size = new System.Drawing.Size(218, 31);
            this.CtxrToPng.TabIndex = 15;
            this.CtxrToPng.Text = "CTXR -> PNG";
            this.CtxrToPng.UseVisualStyleBackColor = false;
            this.CtxrToPng.Click += new System.EventHandler(this.CtxrToPng_Click);
            // 
            // PngToCtxr
            // 
            this.PngToCtxr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.PngToCtxr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PngToCtxr.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.PngToCtxr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.PngToCtxr.Location = new System.Drawing.Point(12, 163);
            this.PngToCtxr.Name = "PngToCtxr";
            this.PngToCtxr.Size = new System.Drawing.Size(218, 31);
            this.PngToCtxr.TabIndex = 16;
            this.PngToCtxr.Text = "PNG -> CTXR";
            this.PngToCtxr.UseVisualStyleBackColor = false;
            this.PngToCtxr.Click += new System.EventHandler(this.PngToCtxr_Click);
            // 
            // PngToDds
            // 
            this.PngToDds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.PngToDds.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.PngToDds.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.PngToDds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.PngToDds.Location = new System.Drawing.Point(12, 200);
            this.PngToDds.Name = "PngToDds";
            this.PngToDds.Size = new System.Drawing.Size(218, 31);
            this.PngToDds.TabIndex = 17;
            this.PngToDds.Text = "PNG -> DDS";
            this.PngToDds.UseVisualStyleBackColor = false;
            this.PngToDds.Click += new System.EventHandler(this.PngToDds_Click);
            // 
            // DdsToCtxr
            // 
            this.DdsToCtxr.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.DdsToCtxr.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.DdsToCtxr.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold);
            this.DdsToCtxr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DdsToCtxr.Location = new System.Drawing.Point(12, 237);
            this.DdsToCtxr.Name = "DdsToCtxr";
            this.DdsToCtxr.Size = new System.Drawing.Size(218, 31);
            this.DdsToCtxr.TabIndex = 18;
            this.DdsToCtxr.Text = "DDS -> CTXR";
            this.DdsToCtxr.UseVisualStyleBackColor = false;
            this.DdsToCtxr.Click += new System.EventHandler(this.DdsToCtxr_Click);
            // 
            // TextureModelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1196, 626);
            this.Controls.Add(this.DdsToCtxr);
            this.Controls.Add(this.PngToDds);
            this.Controls.Add(this.PngToCtxr);
            this.Controls.Add(this.CtxrToPng);
            this.Controls.Add(this.LoadGruButton);
            this.Controls.Add(this.btnLoadObj);
            this.Controls.Add(this.DdsFilePictureBox);
            this.Controls.Add(this.BackButton);
            this.DoubleBuffered = true;
            this.Name = "TextureModelForm";
            this.Text = "TextureModelForm";
            ((System.ComponentModel.ISupportInitialize)(this.DdsFilePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button btnLoadObj;
        private System.Windows.Forms.Button LoadGruButton;
        private System.Windows.Forms.PictureBox DdsFilePictureBox;
        private System.Windows.Forms.Button CtxrToPng;
        private System.Windows.Forms.Button PngToCtxr;
        private System.Windows.Forms.Button PngToDds;
        private System.Windows.Forms.Button DdsToCtxr;
    }
}