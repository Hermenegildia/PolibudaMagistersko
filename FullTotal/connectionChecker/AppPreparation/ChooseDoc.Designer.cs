namespace connectionChecker.AppPreparation
{
    partial class ChooseDoc
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
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btPath = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(65, 48);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(182, 20);
            this.tbPath.TabIndex = 0;
            // 
            // btPath
            // 
            this.btPath.Location = new System.Drawing.Point(284, 48);
            this.btPath.Name = "btPath";
            this.btPath.Size = new System.Drawing.Size(45, 23);
            this.btPath.TabIndex = 1;
            this.btPath.Text = "...";
            this.btPath.UseVisualStyleBackColor = true;
            this.btPath.Click += new System.EventHandler(this.btPath_Click);
            // 
            // ChooseDoc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 401);
            this.Controls.Add(this.btPath);
            this.Controls.Add(this.tbPath);
            this.Name = "ChooseDoc";
            this.Text = "ChooseDoc";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btPath;
    }
}