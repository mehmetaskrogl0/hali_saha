namespace halı_saha
{
    partial class Form2
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
            this.lstTakim1 = new System.Windows.Forms.ListBox();
            this.lstTakim2 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lstTakim1
            // 
            this.lstTakim1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lstTakim1.FormattingEnabled = true;
            this.lstTakim1.ItemHeight = 22;
            this.lstTakim1.Location = new System.Drawing.Point(70, 36);
            this.lstTakim1.Name = "lstTakim1";
            this.lstTakim1.Size = new System.Drawing.Size(263, 312);
            this.lstTakim1.TabIndex = 0;
            // 
            // lstTakim2
            // 
            this.lstTakim2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lstTakim2.FormattingEnabled = true;
            this.lstTakim2.ItemHeight = 22;
            this.lstTakim2.Location = new System.Drawing.Point(446, 36);
            this.lstTakim2.Name = "lstTakim2";
            this.lstTakim2.Size = new System.Drawing.Size(263, 290);
            this.lstTakim2.TabIndex = 1;
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(964, 532);
            this.Controls.Add(this.lstTakim2);
            this.Controls.Add(this.lstTakim1);
            this.Name = "Form2";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.Form2_Load_1);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lstTakim1;
        private System.Windows.Forms.ListBox lstTakim2;
    }
}