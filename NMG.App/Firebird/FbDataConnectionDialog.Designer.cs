namespace NHibernateMappingGenerator.Firebird
{
    partial class FbDataConnectionDialog
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
            this.fbDataConnectionUIControl1 = new NHibernateMappingGenerator.Firebird.FbDataConnectionUIControl();
            this.acceptButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.testConnectionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // fbDataConnectionUIControl1
            // 
            this.fbDataConnectionUIControl1.ConnectionString = "";
            this.fbDataConnectionUIControl1.Location = new System.Drawing.Point(9, 9);
            this.fbDataConnectionUIControl1.Margin = new System.Windows.Forms.Padding(0);
            this.fbDataConnectionUIControl1.MinimumSize = new System.Drawing.Size(457, 198);
            this.fbDataConnectionUIControl1.Name = "fbDataConnectionUIControl1";
            this.fbDataConnectionUIControl1.Size = new System.Drawing.Size(457, 198);
            this.fbDataConnectionUIControl1.TabIndex = 0;
            // 
            // acceptButton
            // 
            this.acceptButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.acceptButton.AutoSize = true;
            this.acceptButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.acceptButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.acceptButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.acceptButton.Location = new System.Drawing.Point(309, 219);
            this.acceptButton.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.acceptButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.acceptButton.Name = "acceptButton";
            this.acceptButton.Size = new System.Drawing.Size(75, 23);
            this.acceptButton.TabIndex = 9;
            this.acceptButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.AutoSize = true;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cancelButton.Location = new System.Drawing.Point(390, 219);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cancelButton.MinimumSize = new System.Drawing.Size(75, 23);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            // 
            // testConnectionButton
            // 
            this.testConnectionButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.testConnectionButton.AutoSize = true;
            this.testConnectionButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.testConnectionButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.testConnectionButton.Location = new System.Drawing.Point(12, 219);
            this.testConnectionButton.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
            this.testConnectionButton.MinimumSize = new System.Drawing.Size(101, 23);
            this.testConnectionButton.Name = "testConnectionButton";
            this.testConnectionButton.Size = new System.Drawing.Size(101, 23);
            this.testConnectionButton.TabIndex = 11;
            this.testConnectionButton.Text = "&Test Connection";
            this.testConnectionButton.Click += new System.EventHandler(this.testConnectionButton_Click);
            // 
            // FbDataConnectionDialog
            // 
            this.AcceptButton = this.acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(474, 254);
            this.Controls.Add(this.acceptButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.testConnectionButton);
            this.Controls.Add(this.fbDataConnectionUIControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FbDataConnectionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Firebird Connection Dialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FbDataConnectionUIControl fbDataConnectionUIControl1;
        private System.Windows.Forms.Button acceptButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button testConnectionButton;
    }
}