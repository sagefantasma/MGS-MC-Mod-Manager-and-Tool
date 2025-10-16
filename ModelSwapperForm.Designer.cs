namespace ANTIBigBoss_MGS_Mod_Manager
{
    partial class ModelSwapperForm
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
            modelToSwapOutComboBox = new System.Windows.Forms.ComboBox();
            modelToSwapLabel = new System.Windows.Forms.Label();
            modelToSwapInLabel = new System.Windows.Forms.Label();
            modelToSwapInComboBox = new System.Windows.Forms.ComboBox();
            swapInNewModelButton = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // modelToSwapOutComboBox
            // 
            modelToSwapOutComboBox.FormattingEnabled = true;
            modelToSwapOutComboBox.Location = new System.Drawing.Point(54, 102);
            modelToSwapOutComboBox.Name = "modelToSwapOutComboBox";
            modelToSwapOutComboBox.Size = new System.Drawing.Size(225, 23);
            modelToSwapOutComboBox.TabIndex = 0;
            modelToSwapOutComboBox.SelectedIndexChanged += modelToSwapOutComboBox_SelectedIndexChanged;
            // 
            // modelToSwapLabel
            // 
            modelToSwapLabel.AutoSize = true;
            modelToSwapLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            modelToSwapLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            modelToSwapLabel.Location = new System.Drawing.Point(54, 67);
            modelToSwapLabel.Name = "modelToSwapLabel";
            modelToSwapLabel.Size = new System.Drawing.Size(225, 32);
            modelToSwapLabel.TabIndex = 1;
            modelToSwapLabel.Text = "Model To Swap Out";
            // 
            // modelToSwapInLabel
            // 
            modelToSwapInLabel.AutoSize = true;
            modelToSwapInLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            modelToSwapInLabel.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            modelToSwapInLabel.Location = new System.Drawing.Point(332, 67);
            modelToSwapInLabel.Name = "modelToSwapInLabel";
            modelToSwapInLabel.Size = new System.Drawing.Size(205, 32);
            modelToSwapInLabel.TabIndex = 3;
            modelToSwapInLabel.Text = "Model To Swap In";
            // 
            // modelToSwapInComboBox
            // 
            modelToSwapInComboBox.Enabled = false;
            modelToSwapInComboBox.FormattingEnabled = true;
            modelToSwapInComboBox.Location = new System.Drawing.Point(332, 102);
            modelToSwapInComboBox.Name = "modelToSwapInComboBox";
            modelToSwapInComboBox.Size = new System.Drawing.Size(205, 23);
            modelToSwapInComboBox.TabIndex = 2;
            // 
            // swapInNewModelButton
            // 
            swapInNewModelButton.Location = new System.Drawing.Point(240, 185);
            swapInNewModelButton.Name = "swapInNewModelButton";
            swapInNewModelButton.Size = new System.Drawing.Size(131, 23);
            swapInNewModelButton.TabIndex = 4;
            swapInNewModelButton.Text = "Swap In New Model";
            swapInNewModelButton.UseVisualStyleBackColor = true;
            swapInNewModelButton.Click += button1_Click;
            // 
            // ModelSwapperForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(51, 51, 51);
            ClientSize = new System.Drawing.Size(800, 450);
            Controls.Add(swapInNewModelButton);
            Controls.Add(modelToSwapInLabel);
            Controls.Add(modelToSwapInComboBox);
            Controls.Add(modelToSwapLabel);
            Controls.Add(modelToSwapOutComboBox);
            Name = "ModelSwapperForm";
            Text = "ModelSwapperForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ComboBox modelToSwapOutComboBox;
        private System.Windows.Forms.Label modelToSwapLabel;
        private System.Windows.Forms.Label modelToSwapInLabel;
        private System.Windows.Forms.ComboBox modelToSwapInComboBox;
        private System.Windows.Forms.Button swapInNewModelButton;
    }
}