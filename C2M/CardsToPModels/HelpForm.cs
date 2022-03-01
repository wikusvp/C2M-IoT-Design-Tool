using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    public partial class HelpForm : Form
    {
        /// <summary>
        /// Type of deck being used
        /// </summary>
        public Design.Decks deckType;

        /// <summary>
        /// List of descriptions corresponding to the different card design image examples
        /// </summary>
        public List<string> imageDescriptions = new List<string>();

        /// <summary>
        /// List of card example images
        /// </summary>
        public List<Bitmap> images = new List<Bitmap>();

 
        /// <summary>
        /// Current position of the example used to select image and description. Starts at 0.
        /// </summary>
        public int imagePosition = 0;
        
        /// <summary>
        /// Form to show the user the rules for connecting cards with arrows and examples of card designs made using the current
        /// deck type
        /// </summary>
        /// <param name="deckType">The current deck type</param>
        public HelpForm(Design.Decks deckType)
        {
            InitializeComponent();
            this.Text = "Help: " + deckType + " Cards";

            this.deckType = deckType;

            // Help text is stored in a txt file
            ResourceManager resourceManager = Properties.Resources.ResourceManager;
            StreamReader reader;
            string line = "";
            reader = File.OpenText("Help.txt");

            while (!reader.EndOfStream)
            {
                try
                {
                    line = reader.ReadLine();
                    string[] lineArray = line.Split(';');
                    if (lineArray[0] == deckType.ToString())
                    {
                        if (lineArray[1] == "HelpText")
                        {
                            help.Text = string.Join("\n\n", new ArraySegment<string>(lineArray, 2, lineArray.Length - 2));
                        }else if (lineArray[1] == "Image")
                        {
                            images.Add((Bitmap)resourceManager.GetObject(lineArray[2]));
                            imageDescriptions.Add(lineArray[3]);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Error: " + line);
                }

            }
            reader.Close();

            exampleDescription.Text = imageDescriptions[imagePosition];
        }

        /// <summary>
        /// Move to the next example. Change image and description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextButton_Click(object sender, EventArgs e)
        {
            imagePosition++;
            if (imagePosition == images.Count)
                imagePosition = 0;
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Move back to the previous example. Change image and description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backButton_Click(object sender, EventArgs e)
        {
            imagePosition--;
            if (imagePosition < 0)
                imagePosition = 0;
            pictureBox1.Invalidate();
        }

        /// <summary>
        /// Change image and text description
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            pictureBox1.BackgroundImage = images[imagePosition];
            exampleDescription.Text = imageDescriptions[imagePosition];
        }
    }
}
