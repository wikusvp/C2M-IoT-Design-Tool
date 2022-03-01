using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    public partial class CardName : Form
    {
        /// <summary>
        /// Name of the custom card
        /// </summary>
        public string name;

        /// <summary>
        /// Form object to allow user to input the name of a card that has been selected.
        /// Used for all custom cards and widget cards in the mobile application deck.
        /// </summary>
        public CardName()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Convert name to title case and close form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void confirmButton_Click(object sender, EventArgs e)
        {
            TextInfo textFormat = new CultureInfo("en-US", false).TextInfo;
            if (nameTextBox.Text == "")
            {
                return;
            }
            name = textFormat.ToTitleCase(nameTextBox.Text.ToLower()).Replace(" ", "");

            DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
