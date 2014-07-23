namespace connectionChecker
{
    partial class InsertRecordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InsertRecordForm));
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbLastName = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.lbLastName = new System.Windows.Forms.Label();
            this.btOK = new System.Windows.Forms.Button();
            this.lbPESEL = new System.Windows.Forms.Label();
            this.tbPESEL = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(152, 37);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(141, 20);
            this.tbName.TabIndex = 1;
            this.tbName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbName_KeyPress);
            // 
            // tbLastName
            // 
            this.tbLastName.Location = new System.Drawing.Point(152, 77);
            this.tbLastName.Name = "tbLastName";
            this.tbLastName.Size = new System.Drawing.Size(141, 20);
            this.tbLastName.TabIndex = 2;
            this.tbLastName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbLastName_KeyPress);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(46, 41);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(26, 13);
            this.lbName.TabIndex = 3;
            this.lbName.Text = "Imię";
            // 
            // lbLastName
            // 
            this.lbLastName.AutoSize = true;
            this.lbLastName.Location = new System.Drawing.Point(46, 81);
            this.lbLastName.Name = "lbLastName";
            this.lbLastName.Size = new System.Drawing.Size(53, 13);
            this.lbLastName.TabIndex = 4;
            this.lbLastName.Text = "Nazwisko";
            // 
            // btOK
            // 
            this.btOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btOK.Image = global::connectionChecker.Properties.Resources.add;
            this.btOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btOK.Location = new System.Drawing.Point(209, 168);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(84, 40);
            this.btOK.TabIndex = 0;
            this.btOK.Text = "Dodaj";
            this.btOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // lbPESEL
            // 
            this.lbPESEL.AutoSize = true;
            this.lbPESEL.Location = new System.Drawing.Point(46, 121);
            this.lbPESEL.Name = "lbPESEL";
            this.lbPESEL.Size = new System.Drawing.Size(41, 13);
            this.lbPESEL.TabIndex = 6;
            this.lbPESEL.Text = "PESEL";
            // 
            // tbPESEL
            // 
            this.tbPESEL.Location = new System.Drawing.Point(152, 117);
            this.tbPESEL.Name = "tbPESEL";
            this.tbPESEL.Size = new System.Drawing.Size(141, 20);
            this.tbPESEL.TabIndex = 5;
            // 
            // InsertRecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 220);
            this.Controls.Add(this.lbPESEL);
            this.Controls.Add(this.tbPESEL);
            this.Controls.Add(this.lbLastName);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.tbLastName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.btOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InsertRecordForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dodaj pacjenta";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbLastName;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lbLastName;
        private System.Windows.Forms.Label lbPESEL;
        private System.Windows.Forms.TextBox tbPESEL;
    }
}