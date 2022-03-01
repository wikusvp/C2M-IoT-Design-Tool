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
    public partial class NewDesign : Form
    {
        /// <summary>
        /// Form for selecting the type of deck the card design will use
        /// </summary>
        public NewDesign()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Open design form for IoT Cards if that option it chosen and close current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void iotCards_Click(object sender, EventArgs e)
        {
            this.Hide();
            DesignForm designForm = new DesignForm(Design.Decks.IoT);
            designForm.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Open design form for Mobile Application Cards if that option it chosen and close current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mobileCards_Click(object sender, EventArgs e)
        {
            this.Hide();
            DesignForm designForm = new DesignForm(Design.Decks.Mobile);
            designForm.ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Open design form for Conceptual Cards if that option it chosen and close current form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void conceptualCards_Click(object sender, EventArgs e)
        {
            this.Hide();
            DesignForm designForm = new DesignForm(Design.Decks.Conceptual);
            designForm.ShowDialog();
            this.Close();
        }

        private void NewDesign_Load(object sender, EventArgs e)
        {
            
        }
    }
}
