/*
 *  Firebird Data Connection Control
 *  Ivan Masmitjà (2013)
 *  
 * 
 *  based on : 
 *  Visual Studio DDEX Provider for FirebirdClient
 *      
 *  Copyright (c) 2005 Carlos Guzman Alvarez
 *  All Rights Reserved.
 *   
 *  Contributors:
 *    Jiri Cincura (jiri@cincura.net)
 */

using System;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;

namespace NHibernateMappingGenerator.Firebird
{
    public partial class FbDataConnectionUIControl : UserControl
    {
        public FbConnectionProperties ConnectionProperties;

        public string ConnectionString
        {
            get
            {
                if (ConnectionProperties != null)
                    return ConnectionProperties.ConnectionString;

                return "";
            }

            set
            {
                ConnectionProperties = new FbConnectionProperties(value);
                LoadProperties();
            }
        }

        #region · Constructors ·

        public FbDataConnectionUIControl()
        {
            System.Diagnostics.Trace.WriteLine("FbDataConnectionUIControl()");
            ConnectionProperties = new FbConnectionProperties();
            InitializeComponent();
        }

        #endregion

        #region · Methods ·

        public void LoadProperties()
        {
            System.Diagnostics.Trace.WriteLine("FbDataConnectionUIControl::LoadProperties()");

            try
            {
                this.txtDataSource.Text = (string)ConnectionProperties["Data Source"];
                this.txtUserName.Text   = (string)ConnectionProperties["User ID"];
                this.txtDatabase.Text   = (string)ConnectionProperties["Initial Catalog"];
                this.txtPassword.Text   = (string)ConnectionProperties["Password"];
                this.txtRole.Text       = (string)ConnectionProperties["Role"];
                this.cboCharset.Text    = (string)ConnectionProperties["Character Set"];
                if (this.ConnectionProperties.Contains("Port Number"))
                {
                    this.txtPort.Text = ConnectionProperties["Port Number"].ToString();
                }

                if (this.ConnectionProperties.Contains("Dialect"))
                {
                    if (Convert.ToInt32(ConnectionProperties["Dialect"]) == 1)
                    {
                        this.cboDialect.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cboDialect.SelectedIndex = 1;
                    }
                }

                if (this.ConnectionProperties.Contains("Server Type"))
                {
                    if (Convert.ToInt32(ConnectionProperties["Server Type"]) == 0)
                    {
                        this.cboServerType.SelectedIndex = 0;
                    }
                    else
                    {
                        this.cboServerType.SelectedIndex = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        #endregion

        #region · Private Methods ·

        private void SetProperty(string propertyName, object value)
        {
            this.ConnectionProperties[propertyName] = value;
        }

        #endregion

        #region · Event Handlers ·

        private void SetProperty(object sender, EventArgs e)
        {
            if (sender.Equals(this.txtDataSource))
            {
                this.SetProperty("Data Source", this.txtDataSource.Text);
            } 
            else if (sender.Equals(this.txtDatabase))
            {
                this.SetProperty("Initial Catalog", this.txtDatabase.Text);
            }
            else if (sender.Equals(this.txtUserName))
            {
                this.SetProperty("User ID", this.txtUserName.Text);
            }
            else if (sender.Equals(this.txtPassword))
            {
                this.SetProperty("Password", this.txtPassword.Text);
            }
            else if (sender.Equals(this.txtRole))
            {
                this.SetProperty("Role", this.txtRole.Text);
            }
            else if (sender.Equals(this.txtPort))
            {
                if (!String.IsNullOrEmpty(this.txtPort.Text))
                {
                    this.SetProperty("Port Number", Convert.ToInt32(this.txtPort.Text));
                }
            }
            else if (sender.Equals(this.cboCharset))
            {
                this.SetProperty("Character Set", this.cboCharset.Text);
            }
            else if (sender.Equals(this.cboDialect))
            {
                if (!String.IsNullOrEmpty(this.cboDialect.Text))
                {
                    this.SetProperty("Dialect", Convert.ToInt32(this.cboDialect.Text));
                }
            }
            else if (sender.Equals(this.cboServerType))
            {
                if (this.cboServerType.SelectedIndex != -1)
                {
                    this.SetProperty("Server Type", Convert.ToInt32(this.cboServerType.SelectedIndex));
                }
            }
        }

        private void cmdGetFile_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.txtDatabase.Text = this.openFileDialog.FileName;
            }
        }

        #endregion

       
    }
}
