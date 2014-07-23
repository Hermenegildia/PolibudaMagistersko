namespace connectionChecker.AppPreparation
{
    partial class PatientSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PatientSelection));
            this.lbPatients = new System.Windows.Forms.ListBox();
            this.btAdd = new System.Windows.Forms.Button();
            this.btOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbPatients
            // 
            this.lbPatients.FormattingEnabled = true;
            this.lbPatients.Location = new System.Drawing.Point(52, 47);
            this.lbPatients.Name = "lbPatients";
            this.lbPatients.Size = new System.Drawing.Size(237, 225);
            this.lbPatients.TabIndex = 0;
            // 
            // btAdd
            // 
            this.btAdd.Image = global::connectionChecker.Properties.Resources.add;
            this.btAdd.Location = new System.Drawing.Point(332, 122);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(34, 36);
            this.btAdd.TabIndex = 2;
            this.btAdd.UseVisualStyleBackColor = true;
            // 
            // btOk
            // 
            this.btOk.Image = global::connectionChecker.Properties.Resources.accept;
            this.btOk.Location = new System.Drawing.Point(332, 47);
            this.btOk.Name = "btOk";
            this.btOk.Size = new System.Drawing.Size(34, 36);
            this.btOk.TabIndex = 1;
            this.btOk.UseVisualStyleBackColor = true;
            // 
            // PatientSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 353);
            this.Controls.Add(this.btAdd);
            this.Controls.Add(this.btOk);
            this.Controls.Add(this.lbPatients);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PatientSelection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wybór pacjenta";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbPatients;
        private System.Windows.Forms.Button btOk;
        private System.Windows.Forms.Button btAdd;
    }
}