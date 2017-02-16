namespace SuperOffice.DevNet.Online.WinForm
{
    partial class Form1
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
            this.btLogin = new System.Windows.Forms.Button();
            this._environmentLogin = new System.Windows.Forms.ComboBox();
            this.btDoStuff = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._stuff = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._applicationId = new System.Windows.Forms.TextBox();
            this._applicationToken = new System.Windows.Forms.TextBox();
            this._netServerUrl = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this._claims = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btLogin
            // 
            this.btLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLogin.Location = new System.Drawing.Point(335, 64);
            this.btLogin.Name = "btLogin";
            this.btLogin.Size = new System.Drawing.Size(75, 23);
            this.btLogin.TabIndex = 0;
            this.btLogin.Text = "Login";
            this.btLogin.UseVisualStyleBackColor = true;
            this.btLogin.Click += new System.EventHandler(this.btLogin_Click);
            // 
            // _environmentLogin
            // 
            this._environmentLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._environmentLogin.FormattingEnabled = true;
            this._environmentLogin.Items.AddRange(new object[] {
            "https://sod.superoffice.com/login/",
            "https://qaonline.superoffice.com/login/",
            "https://online.superoffice.com/online/"});
            this._environmentLogin.Location = new System.Drawing.Point(12, 66);
            this._environmentLogin.Name = "_environmentLogin";
            this._environmentLogin.Size = new System.Drawing.Size(317, 21);
            this._environmentLogin.TabIndex = 1;
            this._environmentLogin.Text = "https://sod.superoffice.com/login/";
            // 
            // btDoStuff
            // 
            this.btDoStuff.Location = new System.Drawing.Point(6, 19);
            this.btDoStuff.Name = "btDoStuff";
            this.btDoStuff.Size = new System.Drawing.Size(75, 23);
            this.btDoStuff.TabIndex = 2;
            this.btDoStuff.Text = "Do Stuff!";
            this.btDoStuff.UseVisualStyleBackColor = true;
            this.btDoStuff.Click += new System.EventHandler(this.btDoStuff_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this._stuff);
            this.groupBox1.Controls.Add(this.btDoStuff);
            this.groupBox1.Location = new System.Drawing.Point(12, 244);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(398, 176);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Do stuff";
            // 
            // _stuff
            // 
            this._stuff.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._stuff.Location = new System.Drawing.Point(7, 49);
            this._stuff.Multiline = true;
            this._stuff.Name = "_stuff";
            this._stuff.ReadOnly = true;
            this._stuff.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._stuff.Size = new System.Drawing.Size(385, 121);
            this._stuff.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Application Identifier (Public)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(133, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Application Token (Secret)";
            // 
            // _applicationId
            // 
            this._applicationId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._applicationId.Location = new System.Drawing.Point(158, 12);
            this._applicationId.Name = "_applicationId";
            this._applicationId.Size = new System.Drawing.Size(252, 20);
            this._applicationId.TabIndex = 6;
            // 
            // _applicationToken
            // 
            this._applicationToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._applicationToken.Location = new System.Drawing.Point(158, 38);
            this._applicationToken.Name = "_applicationToken";
            this._applicationToken.Size = new System.Drawing.Size(252, 20);
            this._applicationToken.TabIndex = 7;
            // 
            // _netServerUrl
            // 
            this._netServerUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._netServerUrl.Location = new System.Drawing.Point(98, 93);
            this._netServerUrl.Name = "_netServerUrl";
            this._netServerUrl.ReadOnly = true;
            this._netServerUrl.Size = new System.Drawing.Size(312, 20);
            this._netServerUrl.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "NetServer URL";
            // 
            // _claims
            // 
            this._claims.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._claims.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3});
            this._claims.Location = new System.Drawing.Point(12, 119);
            this._claims.Name = "_claims";
            this._claims.Size = new System.Drawing.Size(398, 119);
            this._claims.TabIndex = 10;
            this._claims.UseCompatibleStateImageBehavior = false;
            this._claims.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Claim";
            this.columnHeader1.Width = 103;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Resource";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 432);
            this.Controls.Add(this._claims);
            this.Controls.Add(this._netServerUrl);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._applicationToken);
            this.Controls.Add(this._applicationId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._environmentLogin);
            this.Controls.Add(this.btLogin);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btLogin;
        private System.Windows.Forms.ComboBox _environmentLogin;
        private System.Windows.Forms.Button btDoStuff;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox _stuff;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox _applicationId;
        private System.Windows.Forms.TextBox _applicationToken;
        private System.Windows.Forms.TextBox _netServerUrl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView _claims;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}

