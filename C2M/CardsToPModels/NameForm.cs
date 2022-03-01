using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    public partial class NameForm : Form
    {
        public string applicationName;
        public NameForm()
        {
            InitializeComponent();
            btnOK.DialogResult = DialogResult.OK;
        }


        private void NameForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtAppName_TextChanged(object sender, EventArgs e)
        {
            if (txtAppName.Text.Contains("/") 
                || txtAppName.Text.Contains("\\") 
                || txtAppName.Text.Contains(":") 
                || txtAppName.Text.Contains("*") 
                || txtAppName.Text.Contains("?") 
                || txtAppName.Text.Contains("\"") 
                || txtAppName.Text.Contains("<") 
                || txtAppName.Text.Contains(">") 
                || txtAppName.Text.Contains("|"))
            {
                MessageBox.Show("A file name can't contain any of the following characters: \n" +
                    "\\ / : * ? \" < > |");
                txtAppName.Text = txtAppName.Text.Replace("/", "");
                txtAppName.Text = txtAppName.Text.Replace("\\", "");
                txtAppName.Text = txtAppName.Text.Replace(":", "");
                txtAppName.Text = txtAppName.Text.Replace("*", "");
                txtAppName.Text = txtAppName.Text.Replace("?", "");
                txtAppName.Text = txtAppName.Text.Replace("\"", "");
                txtAppName.Text = txtAppName.Text.Replace("<", "");
                txtAppName.Text = txtAppName.Text.Replace(">", "");
                txtAppName.Text = txtAppName.Text.Replace("|", "");
                txtAppName.SelectionStart = txtAppName.Text.Length;
            }
            
        }
    }
}