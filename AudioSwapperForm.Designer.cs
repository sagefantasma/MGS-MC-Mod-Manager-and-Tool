using System.Drawing;
using System.Windows.Forms;

namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class AudioSwapperForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioSwapperForm));
            this.swapAudioButton = new System.Windows.Forms.Button();
            this.restoreAudioButton = new System.Windows.Forms.Button();
            this.checkboxMGS3 = new System.Windows.Forms.CheckBox();
            this.checkboxMG1 = new System.Windows.Forms.CheckBox();
            this.checkboxMG2 = new System.Windows.Forms.CheckBox();
            this.dataGridSdtTracks = new System.Windows.Forms.DataGridView();
            this.BackButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSdtTracks)).BeginInit();
            this.SuspendLayout();
            // 
            // swapAudioButton
            // 
            this.swapAudioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.swapAudioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.swapAudioButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.swapAudioButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.swapAudioButton.Location = new System.Drawing.Point(986, 452);
            this.swapAudioButton.Name = "swapAudioButton";
            this.swapAudioButton.Size = new System.Drawing.Size(162, 70);
            this.swapAudioButton.TabIndex = 0;
            this.swapAudioButton.Text = "Swap Audio";
            this.swapAudioButton.UseVisualStyleBackColor = false;
            this.swapAudioButton.Click += new System.EventHandler(this.swapAudioButton_Click);
            // 
            // restoreAudioButton
            // 
            this.restoreAudioButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.restoreAudioButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.restoreAudioButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.restoreAudioButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.restoreAudioButton.Location = new System.Drawing.Point(986, 528);
            this.restoreAudioButton.Name = "restoreAudioButton";
            this.restoreAudioButton.Size = new System.Drawing.Size(162, 70);
            this.restoreAudioButton.TabIndex = 1;
            this.restoreAudioButton.Text = "Restore Audio to Defaults";
            this.restoreAudioButton.UseVisualStyleBackColor = false;
            this.restoreAudioButton.Click += new System.EventHandler(this.restoreAudioButton_Click);
            // 
            // checkboxMGS3
            // 
            this.checkboxMGS3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.checkboxMGS3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkboxMGS3.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkboxMGS3.Location = new System.Drawing.Point(986, 88);
            this.checkboxMGS3.Name = "checkboxMGS3";
            this.checkboxMGS3.Size = new System.Drawing.Size(162, 32);
            this.checkboxMGS3.TabIndex = 738;
            this.checkboxMGS3.Text = "Show MGS3 Tracks";
            this.checkboxMGS3.UseVisualStyleBackColor = false;
            this.checkboxMGS3.CheckedChanged += new System.EventHandler(this.checkboxMGS3_CheckedChanged);
            // 
            // checkboxMG1
            // 
            this.checkboxMG1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.checkboxMG1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkboxMG1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkboxMG1.Location = new System.Drawing.Point(986, 12);
            this.checkboxMG1.Name = "checkboxMG1";
            this.checkboxMG1.Size = new System.Drawing.Size(162, 32);
            this.checkboxMG1.TabIndex = 739;
            this.checkboxMG1.Text = "Show MG1 Tracks";
            this.checkboxMG1.UseVisualStyleBackColor = false;
            this.checkboxMG1.CheckedChanged += new System.EventHandler(this.checkboxMG1_CheckedChanged);
            // 
            // checkboxMG2
            // 
            this.checkboxMG2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.checkboxMG2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkboxMG2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.checkboxMG2.Location = new System.Drawing.Point(986, 50);
            this.checkboxMG2.Name = "checkboxMG2";
            this.checkboxMG2.Size = new System.Drawing.Size(162, 32);
            this.checkboxMG2.TabIndex = 740;
            this.checkboxMG2.Text = "Show MG2 Tracks";
            this.checkboxMG2.UseVisualStyleBackColor = false;
            this.checkboxMG2.CheckedChanged += new System.EventHandler(this.checkboxMG2_CheckedChanged);
            // 
            // dataGridSdtTracks
            // 
            this.dataGridSdtTracks.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.dataGridSdtTracks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridSdtTracks.Location = new System.Drawing.Point(9, 50);
            this.dataGridSdtTracks.Name = "dataGridSdtTracks";
            this.dataGridSdtTracks.RowTemplate.Height = 25;
            this.dataGridSdtTracks.Size = new System.Drawing.Size(971, 551);
            this.dataGridSdtTracks.TabIndex = 741;
            // 
            // BackButton
            // 
            this.BackButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(156)))), ((int)(((byte)(124)))));
            this.BackButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BackButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.BackButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.BackButton.Location = new System.Drawing.Point(9, 12);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(162, 32);
            this.BackButton.TabIndex = 742;
            this.BackButton.Text = "Back";
            this.BackButton.UseVisualStyleBackColor = false;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // AudioSwapperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1160, 613);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.dataGridSdtTracks);
            this.Controls.Add(this.checkboxMG2);
            this.Controls.Add(this.checkboxMG1);
            this.Controls.Add(this.checkboxMGS3);
            this.Controls.Add(this.restoreAudioButton);
            this.Controls.Add(this.swapAudioButton);
            this.Name = "AudioSwapperForm";
            this.Text = "MGS3 Audio Swapper";
            this.Load += new System.EventHandler(this.AudioSwapperForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridSdtTracks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button swapAudioButton;
        private Button restoreAudioButton;
        private CheckBox checkboxMGS3;
        private CheckBox checkboxMG1;
        private CheckBox checkboxMG2;
        private DataGridView dataGridSdtTracks;
        private Button BackButton;
    }
}