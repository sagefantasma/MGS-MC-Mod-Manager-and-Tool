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
            swapAudioButton = new Button();
            restoreAudioButton = new Button();
            checkboxMGS3 = new CheckBox();
            checkboxMG1 = new CheckBox();
            checkboxMG2 = new CheckBox();
            dataGridSdtTracks = new DataGridView();
            BackButton = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridSdtTracks).BeginInit();
            SuspendLayout();
            // 
            // swapAudioButton
            // 
            swapAudioButton.BackColor = Color.FromArgb(156, 156, 124);
            swapAudioButton.BackgroundImageLayout = ImageLayout.Stretch;
            swapAudioButton.FlatStyle = FlatStyle.Popup;
            swapAudioButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            swapAudioButton.Location = new Point(1150, 522);
            swapAudioButton.Margin = new Padding(4, 3, 4, 3);
            swapAudioButton.Name = "swapAudioButton";
            swapAudioButton.Size = new Size(189, 81);
            swapAudioButton.TabIndex = 0;
            swapAudioButton.Text = "Swap Audio";
            swapAudioButton.UseVisualStyleBackColor = false;
            swapAudioButton.Click += swapAudioButton_Click;
            // 
            // restoreAudioButton
            // 
            restoreAudioButton.BackColor = Color.FromArgb(156, 156, 124);
            restoreAudioButton.BackgroundImageLayout = ImageLayout.Stretch;
            restoreAudioButton.FlatStyle = FlatStyle.Popup;
            restoreAudioButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            restoreAudioButton.Location = new Point(1150, 609);
            restoreAudioButton.Margin = new Padding(4, 3, 4, 3);
            restoreAudioButton.Name = "restoreAudioButton";
            restoreAudioButton.Size = new Size(189, 81);
            restoreAudioButton.TabIndex = 1;
            restoreAudioButton.Text = "Restore Audio to Defaults";
            restoreAudioButton.UseVisualStyleBackColor = false;
            restoreAudioButton.Click += restoreAudioButton_Click;
            // 
            // checkboxMGS3
            // 
            checkboxMGS3.BackColor = Color.FromArgb(156, 156, 124);
            checkboxMGS3.FlatStyle = FlatStyle.Flat;
            checkboxMGS3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            checkboxMGS3.Location = new Point(1150, 102);
            checkboxMGS3.Margin = new Padding(4, 3, 4, 3);
            checkboxMGS3.Name = "checkboxMGS3";
            checkboxMGS3.Size = new Size(189, 37);
            checkboxMGS3.TabIndex = 738;
            checkboxMGS3.Text = "Show MGS3 Tracks";
            checkboxMGS3.UseVisualStyleBackColor = false;
            checkboxMGS3.CheckedChanged += checkboxMGS3_CheckedChanged;
            // 
            // checkboxMG1
            // 
            checkboxMG1.BackColor = Color.FromArgb(156, 156, 124);
            checkboxMG1.FlatStyle = FlatStyle.Flat;
            checkboxMG1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            checkboxMG1.Location = new Point(1150, 14);
            checkboxMG1.Margin = new Padding(4, 3, 4, 3);
            checkboxMG1.Name = "checkboxMG1";
            checkboxMG1.Size = new Size(189, 37);
            checkboxMG1.TabIndex = 739;
            checkboxMG1.Text = "Show MG1 Tracks";
            checkboxMG1.UseVisualStyleBackColor = false;
            checkboxMG1.CheckedChanged += checkboxMG1_CheckedChanged;
            // 
            // checkboxMG2
            // 
            checkboxMG2.BackColor = Color.FromArgb(156, 156, 124);
            checkboxMG2.FlatStyle = FlatStyle.Flat;
            checkboxMG2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold);
            checkboxMG2.Location = new Point(1150, 58);
            checkboxMG2.Margin = new Padding(4, 3, 4, 3);
            checkboxMG2.Name = "checkboxMG2";
            checkboxMG2.Size = new Size(189, 37);
            checkboxMG2.TabIndex = 740;
            checkboxMG2.Text = "Show MG2 Tracks";
            checkboxMG2.UseVisualStyleBackColor = false;
            checkboxMG2.CheckedChanged += checkboxMG2_CheckedChanged;
            // 
            // dataGridSdtTracks
            // 
            dataGridSdtTracks.BackgroundColor = Color.FromArgb(156, 156, 124);
            dataGridSdtTracks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridSdtTracks.Location = new Point(10, 58);
            dataGridSdtTracks.Margin = new Padding(4, 3, 4, 3);
            dataGridSdtTracks.Name = "dataGridSdtTracks";
            dataGridSdtTracks.Size = new Size(1133, 636);
            dataGridSdtTracks.TabIndex = 741;
            // 
            // BackButton
            // 
            BackButton.BackColor = Color.FromArgb(156, 156, 124);
            BackButton.BackgroundImageLayout = ImageLayout.Stretch;
            BackButton.FlatStyle = FlatStyle.Popup;
            BackButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            BackButton.Location = new Point(10, 14);
            BackButton.Margin = new Padding(4, 3, 4, 3);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(189, 37);
            BackButton.TabIndex = 742;
            BackButton.Text = "Back";
            BackButton.UseVisualStyleBackColor = false;
            BackButton.Click += BackButton_Click;
            // 
            // AudioSwapperForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1353, 707);
            Controls.Add(BackButton);
            Controls.Add(dataGridSdtTracks);
            Controls.Add(checkboxMG2);
            Controls.Add(checkboxMG1);
            Controls.Add(checkboxMGS3);
            Controls.Add(restoreAudioButton);
            Controls.Add(swapAudioButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "AudioSwapperForm";
            Text = "MGS3 Audio Swapper";
            Load += AudioSwapperForm_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridSdtTracks).EndInit();
            ResumeLayout(false);
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