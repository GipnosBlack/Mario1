namespace Mario1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Mario = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)Mario).BeginInit();
            SuspendLayout();
            // 
            // Mario
            // 
            Mario.Location = new Point(57, 131);
            Mario.Margin = new Padding(3, 2, 3, 2);
            Mario.Name = "Mario";
            Mario.Size = new Size(83, 83);
            Mario.TabIndex = 0;
            Mario.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ActiveCaptionText;
            label1.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.ForeColor = Color.Lime;
            label1.Location = new Point(424, 92);
            label1.Name = "label1";
            label1.Size = new Size(39, 47);
            label1.TabIndex = 2;
            label1.Text = "0";
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            BackColor = Color.DodgerBlue;
            ClientSize = new Size(1370, 749);
            Controls.Add(label1);
            Controls.Add(Mario);
            DoubleBuffered = true;
            ForeColor = SystemColors.ControlLight;
            KeyPreview = true;
            Name = "Form1";
            StartPosition = FormStartPosition.WindowsDefaultBounds;
            Text = "Form1";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            Paint += Form1_Paint;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)Mario).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Mario;
        private Label label1;
    }
}
