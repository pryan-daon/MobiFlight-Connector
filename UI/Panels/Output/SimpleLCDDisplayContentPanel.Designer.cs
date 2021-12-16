namespace MobiFlight.UI.Panels
{
    partial class SimpleLCDDisplayContentPanel
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
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lcdDisplayTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.MinimumSize = new System.Drawing.Size(0, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 170);
            this.label2.TabIndex = 10;
            this.label2.Text = "Text ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MinimumSize = new System.Drawing.Size(0, 80);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(427, 170);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(56, 0);
            this.panel2.MinimumSize = new System.Drawing.Size(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(371, 106);
            this.panel2.TabIndex = 13;
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.BackColor = System.Drawing.Color.Green;
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Padding = new System.Windows.Forms.Padding(5);
            this.panel5.Size = new System.Drawing.Size(152, 52);
            this.panel5.TabIndex = 11;
            // 
            // panel6
            // 
            this.panel6.AutoSize = true;
            this.panel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel6.BackColor = System.Drawing.Color.Black;
            this.panel6.Controls.Add(this.lcdDisplayTextBox);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(5, 5);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(142, 42);
            this.panel6.TabIndex = 5;
            // 
            // lcdDisplayTextBox
            // 
            this.lcdDisplayTextBox.BackColor = System.Drawing.Color.RoyalBlue;
            this.lcdDisplayTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lcdDisplayTextBox.Font = new System.Drawing.Font("Courier New", 10F);
            this.lcdDisplayTextBox.ForeColor = System.Drawing.Color.White;
            this.lcdDisplayTextBox.Location = new System.Drawing.Point(5, 5);
            this.lcdDisplayTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.lcdDisplayTextBox.Multiline = true;
            this.lcdDisplayTextBox.Name = "lcdDisplayTextBox";
            this.lcdDisplayTextBox.Size = new System.Drawing.Size(132, 32);
            this.lcdDisplayTextBox.TabIndex = 1;
            this.lcdDisplayTextBox.Text = ".-=MobiFlight=-.\r\n****Test: $$****";
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(56, 106);
            this.label3.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label3.MinimumSize = new System.Drawing.Size(0, 30);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.label3.Size = new System.Drawing.Size(371, 64);
            this.label3.TabIndex = 0;
            this.label3.Text = "Bei der Ausgabe wird $ durch den aktuellen Wert ersetzt.\r\n\r\nDu kannst weitere Pla" +
    "tzhalter verwenden, die du im FSUIPC Tab definieren kannst.";
            // 
            // SimpleLCDDisplayContentPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "SimpleLCDDisplayContentPanel";
            this.Size = new System.Drawing.Size(427, 180);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        public System.Windows.Forms.TextBox lcdDisplayTextBox;
        private System.Windows.Forms.Label label3;
    }
}
