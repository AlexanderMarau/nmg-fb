using System;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace NHibernateMappingGenerator.Firebird
{
    public partial class FbDataConnectionDialog : Form
    {
        public FbDataConnectionDialog()
        {
            InitializeComponent();
        }

        public string ConnectionString
        {
            get { return this.fbDataConnectionUIControl1.ConnectionString; }
            set { this.fbDataConnectionUIControl1.ConnectionString = value; }
        }

        private void testConnectionButton_Click(object sender, System.EventArgs e)
        {
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				Test();
			}
			catch (Exception ex)
			{
				Cursor.Current = currentCursor;
                ShowError("Connection Error", ex);
				return;
			}
			Cursor.Current = currentCursor;
            ShowMessage("Connected");
		}

        public virtual void Test()
        {
            // If the connection string is empty, don't even bother testing
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("Invalid connection string");
            }

            // Create a connection object
            DbConnection connection = null;
            DbProviderFactory factory = DbProviderFactories.GetFactory(FbProvider.Name);
            Debug.Assert(factory != null);
            connection = factory.CreateConnection();
            Debug.Assert(connection != null);

            // Try to open it
            try
            {
                connection.ConnectionString = ConnectionString;
                connection.Open();
            }
            finally
            {
                connection.Dispose();
            }
        }


        private void ShowMessage(String message)
        {
            // TODO Show in a pretty form
            MessageBox.Show(message, "Connection info", MessageBoxButtons.OK);
        }

        private void ShowError(Exception ex)
        {
            ShowError(null, ex);
        }

        private void ShowError(String message, Exception ex = null)
        {
            // TODO Show in a pretty form
            StringBuilder errormsg = new StringBuilder();
            if (!String.IsNullOrEmpty(message))
            {
                errormsg.Append(message);
            }

            if (ex != null)
            {
                if (errormsg.Length > 0)
                    errormsg.Append("\n");
                errormsg.Append(ex.Message);
            }

            MessageBox.Show(errormsg.ToString(), "Connection error", 
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
