namespace DrapPanel
{
    partial class Form5
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableElement1 = new DrapPanel.tableElement();
            this.tableElement2 = new DrapPanel.tableElement();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(593, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(126, 596);
            this.panel1.TabIndex = 0;
            // 
            // tableElement1
            // 
            this.tableElement1.Location = new System.Drawing.Point(54, 46);
            this.tableElement1.Name = "tableElement1";
            this.tableElement1.Size = new System.Drawing.Size(147, 199);
            this.tableElement1.TabIndex = 1;
            // 
            // tableElement2
            // 
            this.tableElement2.Location = new System.Drawing.Point(252, 34);
            this.tableElement2.Name = "tableElement2";
            this.tableElement2.Size = new System.Drawing.Size(147, 199);
            this.tableElement2.TabIndex = 2;
            // 
            // Form5
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 611);
            this.Controls.Add(this.tableElement2);
            this.Controls.Add(this.tableElement1);
            this.Controls.Add(this.panel1);
            this.Name = "Form5";
            this.Text = "Form5";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private tableElement tableElement1;
        private tableElement tableElement2;
    }
}