namespace connectionChecker
{
    partial class StartupWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartupWindow));
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbTitle = new System.Windows.Forms.Label();
            this.lbWait = new System.Windows.Forms.Label();
            this.btLogIn = new System.Windows.Forms.Button();
            this.tbLogin = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lbLogin = new System.Windows.Forms.Label();
            this.lbPassword = new System.Windows.Forms.Label();
            this.lbInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(29, 297);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(304, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // lbTitle
            // 
            this.lbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTitle.AutoSize = true;
            this.lbTitle.BackColor = System.Drawing.Color.Transparent;
            this.lbTitle.Font = new System.Drawing.Font("Calibri", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbTitle.Location = new System.Drawing.Point(60, 35);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(243, 23);
            this.lbTitle.TabIndex = 1;
            this.lbTitle.Text = "Trwa uruchamianie programu.";
            this.lbTitle.Visible = false;
            // 
            // lbWait
            // 
            this.lbWait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbWait.AutoSize = true;
            this.lbWait.BackColor = System.Drawing.Color.Transparent;
            this.lbWait.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.lbWait.Location = new System.Drawing.Point(115, 84);
            this.lbWait.Name = "lbWait";
            this.lbWait.Size = new System.Drawing.Size(132, 23);
            this.lbWait.TabIndex = 2;
            this.lbWait.Text = "Proszę czekać...";
            this.lbWait.Visible = false;
            // 
            // btLogIn
            // 
            this.btLogIn.Location = new System.Drawing.Point(144, 218);
            this.btLogIn.Name = "btLogIn";
            this.btLogIn.Size = new System.Drawing.Size(75, 23);
            this.btLogIn.TabIndex = 3;
            this.btLogIn.Text = "Zaloguj";
            this.btLogIn.UseVisualStyleBackColor = true;
            this.btLogIn.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbLogin
            // 
            this.tbLogin.Location = new System.Drawing.Point(135, 138);
            this.tbLogin.Name = "tbLogin";
            this.tbLogin.Size = new System.Drawing.Size(198, 20);
            this.tbLogin.TabIndex = 4;
            this.tbLogin.Text = "sa";
            this.tbLogin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbLogin_KeyPress);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(135, 178);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(198, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.Text = "mojeHaslo123";
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lbLogin
            // 
            this.lbLogin.AutoSize = true;
            this.lbLogin.Location = new System.Drawing.Point(42, 142);
            this.lbLogin.Name = "lbLogin";
            this.lbLogin.Size = new System.Drawing.Size(33, 13);
            this.lbLogin.TabIndex = 6;
            this.lbLogin.Text = "Login";
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(42, 182);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(36, 13);
            this.lbPassword.TabIndex = 7;
            this.lbPassword.Text = "Hasło";
            // 
            // lbInfo
            // 
            this.lbInfo.AutoSize = true;
            this.lbInfo.Font = new System.Drawing.Font("Calibri", 14.25F);
            this.lbInfo.Location = new System.Drawing.Point(46, 62);
            this.lbInfo.Name = "lbInfo";
            this.lbInfo.Size = new System.Drawing.Size(270, 23);
            this.lbInfo.TabIndex = 8;
            this.lbInfo.Text = "Podaj dane logowania do serwera";
            // 
            // StartupWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 360);
            this.Controls.Add(this.lbInfo);
            this.Controls.Add(this.lbPassword);
            this.Controls.Add(this.lbLogin);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbLogin);
            this.Controls.Add(this.btLogIn);
            this.Controls.Add(this.lbWait);
            this.Controls.Add(this.lbTitle);
            this.Controls.Add(this.progressBar1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StartupWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KiMedi";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Label lbWait;
        private System.Windows.Forms.Button btLogIn;
        private System.Windows.Forms.TextBox tbLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lbLogin;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.Label lbInfo;
    }
}