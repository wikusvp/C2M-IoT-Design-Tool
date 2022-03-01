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
    public partial class CardSelector : Form
    {
        /// <summary>
        /// Card type being selected
        /// </summary>
        public string cardType;

        /// <summary>
        /// The selected Category to change to
        /// </summary>
        public string categoryChange;

        /// <summary>
        /// Cards to select from
        /// </summary>
        public List<Card> cards;

        /// <summary>
        /// Selected card
        /// </summary>
        public Card selected;

        /// <summary>
        /// Row of mouse click
        /// </summary>
        public int row;

        /// <summary>
        /// Column of mouse click
        /// </summary>
        public int col;

        /// <summary>
        /// Gap between cards when drawn
        /// </summary>
        public const int GAP = 10;

        /// <summary>
        /// Form object for selecting a new card
        /// </summary>
        /// <param name="cardType">Type of card being inserted</param>
        /// <param name="cards">Cards to pick from</param>
        /// <param name="design">Design object that holds the deck, cards and arrows for the current design</param>
        public CardSelector(string cardType, List<Card> cards, Design design)
        {
            InitializeComponent();
            // set the form label to the type of card being inserted
            TypeOfCard.Text = cardType.ToString();
            this.Text = "Insert " + cardType.ToString() + " Card";

            this.cardType = cardType;
            this.cards = cards;

            pictureBox.Height = (Card.HEIGHT + GAP) * (int)Math.Ceiling((decimal)cards.Count / 5) - GAP;

            foreach (string cardCategory in design.CardCategories)
            {
                changeCategoryToolStripMenuItem.DropDownItems.Add(
                    cardCategory + " Card",
                    null,
                    delegate { SelectCardDialog(cardCategory); }
                    );
            }
        }

        /// <summary>
        /// Shows the card selection dialog for a specific type of card
        /// </summary>
        /// <param name="type">Type of card being inserted</param>
        private void SelectCardDialog(string type)
        {
            categoryChange = type;
            DialogResult = DialogResult.No;
            this.Close();
        }

        /// <summary>
        /// Draw the cards in the box in rows and columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            int x = 0;
            int y = 0;
            int i = 0;
            foreach (Card card in cards)
            {
                e.Graphics.DrawImage(card.Image, x, y, Card.WIDTH, Card.HEIGHT);
                x += (Card.WIDTH + GAP);
                i++;
                if(i % 5 == 0)
                {
                    y += (Card.HEIGHT + GAP);
                    x = 0;
                }
            }
        }

        /// <summary>
        /// Find the selected card based on the rows and columns
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            row = e.Y / (Card.HEIGHT + GAP);
            col = e.X / (Card.WIDTH + GAP);
            int pos = row * 5 + col;

            if (pos < cards.Count)
            {
                selected = cards[pos];
                DrawSelection();
                selectedCard.Text = "Selected Card : " + selected.Name;
                //addButton.Enabled = true;                                     //Replaced by immediately selecting the card
                add();
            }
        }


        /// <summary>
        /// Redraw selection if scrolled using the scroll bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_Scroll(object sender, ScrollEventArgs e)
        {
            if(selected != null)
            {
                DrawSelection();
            }
        }

        /// <summary>
        /// Redraw selection if scrolled using the mouse wheel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panel1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (selected != null)
            {
                DrawSelection();
            }
        }

        /// <summary>
        /// Draw a yellow rectangle over the selected card
        /// </summary>
        private void DrawSelection()
        {
            pictureBox.Refresh();
            Graphics paper = pictureBox.CreateGraphics();
            paper.FillRectangle(new SolidBrush(Color.FromArgb(70, 255, 255, 0)), col * (Card.WIDTH + GAP), row * (Card.HEIGHT + GAP), Card.WIDTH, Card.HEIGHT);
        }

        /// <summary>
        /// Close form. If the selected card is a custom card then open the form to select the name (CustomCard)
        /// </summary>
        private void add()
        {
            if (selected.Name.Contains("Custom") || selected.CardType == "Widget")
            {
                CardName customCardForm = new CardName();
                string code = selected.Code;
                if (customCardForm.ShowDialog() == DialogResult.OK)
                {
                    selected = new Card(cardType, customCardForm.name, code, true);
                    DialogResult = DialogResult.OK;
                    this.Close();
                }

            }
            else
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            
        }

        private void CardSelector_Load(object sender, EventArgs e)
        {

        }
    }
}
