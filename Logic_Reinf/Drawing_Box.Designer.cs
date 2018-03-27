namespace Logic_Reinf
{
    partial class Drawing_Box
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
            this.pb_visual = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_visual)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_visual
            // 
            this.pb_visual.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pb_visual.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_visual.Location = new System.Drawing.Point(14, 21);
            this.pb_visual.Margin = new System.Windows.Forms.Padding(5);
            this.pb_visual.Name = "pb_visual";
            this.pb_visual.Size = new System.Drawing.Size(392, 395);
            this.pb_visual.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pb_visual.TabIndex = 0;
            this.pb_visual.TabStop = false;
            this.pb_visual.Click += new System.EventHandler(this.pb_visual_Click);
            this.pb_visual.Paint += new System.Windows.Forms.PaintEventHandler(this.pb_visual_Paint);
            // 
            // Drawing_Box
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 430);
            this.Controls.Add(this.pb_visual);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Drawing_Box";
            this.Text = "Drawing Box";
            ((System.ComponentModel.ISupportInitialize)(this.pb_visual)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_visual;
    }
}