namespace MobiFlight.UI.Panels
{
    partial class UpdatedLCDDisplayPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lcdDisplayContentTypeComboBox = new System.Windows.Forms.ComboBox();
            this.lcdDisplayContentTypeLabel = new System.Windows.Forms.Label();
            this.DisplayComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // lcdDisplayContentTypeComboBox
            // 
            this.lcdDisplayContentTypeComboBox.FormattingEnabled = true;
            this.lcdDisplayContentTypeComboBox.Items.AddRange(new object[] {
            "Simple",
            "Scripted"});
            this.lcdDisplayContentTypeComboBox.Location = new System.Drawing.Point(50, 11);
            this.lcdDisplayContentTypeComboBox.Name = "lcdDisplayContentTypeComboBox";
            this.lcdDisplayContentTypeComboBox.Size = new System.Drawing.Size(133, 21);
            this.lcdDisplayContentTypeComboBox.TabIndex = 14;
            this.lcdDisplayContentTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.lcdDisplayContentTypeComboBox_SelectedIndexChanged);
            // 
            // lcdDisplayContentTypeLabel
            // 
            this.lcdDisplayContentTypeLabel.AutoSize = true;
            this.lcdDisplayContentTypeLabel.Location = new System.Drawing.Point(3, 14);
            this.lcdDisplayContentTypeLabel.Name = "lcdDisplayContentTypeLabel";
            this.lcdDisplayContentTypeLabel.Size = new System.Drawing.Size(44, 13);
            this.lcdDisplayContentTypeLabel.TabIndex = 15;
            this.lcdDisplayContentTypeLabel.Text = "Content";
            // 
            // DisplayComboBox
            // 
            this.DisplayComboBox.FormattingEnabled = true;
            this.DisplayComboBox.Location = new System.Drawing.Point(50, 8);
            this.DisplayComboBox.Name = "DisplayComboBox";
            this.DisplayComboBox.Size = new System.Drawing.Size(133, 21);
            this.DisplayComboBox.TabIndex = 3;
            this.DisplayComboBox.SelectedIndexChanged += new System.EventHandler(this.DisplayComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Display";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.DisplayComboBox);
            this.panel3.Location = new System.Drawing.Point(3, 34);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(419, 39);
            this.panel3.TabIndex = 13;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lcdDisplayContentTypeLabel);
            this.panel1.Controls.Add(this.lcdDisplayContentTypeComboBox);
            this.panel1.Location = new System.Drawing.Point(3, 79);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(419, 45);
            this.panel1.TabIndex = 16;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(920, 28);
            this.panel2.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(3, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(502, 18);
            this.label4.TabIndex = 18;
            this.label4.Text = "Wähle Dein LCD Display und verwende die TextBox um Deine Ausgabe zu definieren.";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label2);
            this.panel4.Location = new System.Drawing.Point(3, 130);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(419, 214);
            this.panel4.TabIndex = 17;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 97);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "No LCD content type selected";
            // 
            // UpdatedLCDDisplayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Name = "UpdatedLCDDisplayPanel";
            this.Size = new System.Drawing.Size(920, 485);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox lcdDisplayContentTypeComboBox;
        private System.Windows.Forms.Label lcdDisplayContentTypeLabel;
        private System.Windows.Forms.ComboBox DisplayComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
    }
}
