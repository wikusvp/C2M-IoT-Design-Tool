using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Drawing.Imaging;

namespace CardsToPModels
{
    /// <summary>
    /// Form for creating the card design
    /// </summary>
    public partial class DesignForm : Form
    {
        /// <summary>
        /// The currently selected card
        /// </summary>
        public Card currentCard;

        /// <summary>
        /// Design object that holds the deck, cards and arrows for the current design
        /// </summary>
        public Design design;

        /// <summary>
        /// Design object that holds the deck, cards and arrows for the design to be added to the undo/redo stack
        /// </summary>
        public Design undoRedoDesign;

        /// <summary>
        /// An object that holds the current and past states of the design on an undo stack 
        /// and future states on a redo stack
        /// </summary>
        public UndoRedo undoRedo;

        /// <summary>
        /// True if the arrow tool is being used and false if not. Default is set to false.
        /// </summary>
        public bool insertArrow = false;

        /// <summary>
        /// True if the design has been saved
        /// </summary>
        public bool saved = true;

        /// <summary>
        /// List of cards that are being connected using the arrow tool
        /// </summary>
        public List<Card> connectionCards = new List<Card>();

        /// <summary>
        /// Currently selected arrow or arrow being added
        /// </summary>
        public Arrow currentArrow;

        /// <summary>
        /// Filename of the current design to use when saving
        /// </summary>
        public string designFileName;

        /// <summary>
        /// True if the arrow tool has been activated by double clicking
        /// </summary>
        public bool doubleClick = false;

        /// <summary>
        /// The (x,y) mouse position of the last MouseDown event.
        /// </summary>
        protected int startX, startY;

        /// <summary>
        /// The (x,y) position of the current card, just before we started dragging it.
        /// </summary>
        protected int currentX, currentY;

        /// <summary>
        /// Type of deck of the current design is using
        /// </summary>
        public Design.Decks deckType;

        /// <summary>
        /// Store a copy of the selected 'other' card
        /// </summary>
        public Card cardCopy;

        /// <summary>
        /// Keep track of the number of times different cards were used
        /// </summary>
        public CardCount cardCount;

        /// <summary>
        /// Keeps the visibility status of the Models layout.
        /// </summary>
        public bool pModelVisible = true;
        public bool  pIMVisible = true;
        public bool pMRVisible = true;

        /// <summary>
        /// Form for creating the card design. Adds the different tool strip menu items for the cards in the current deck.
        /// </summary>
        /// <param name="deckType">Deck being used for the current card design</param>
        public DesignForm(Design.Decks deckType)
        {
            InitializeComponent();
            design = new Design(deckType);
            undoRedo = new UndoRedo();
            cardCount = new CardCount();
            undoRedo.stateChanged(copyDesign());
            output();

            foreach (string cardCategory in design.CardCategories)
            {
                cardToolStripMenuItem.DropDownItems.Add(
                    cardCategory + " Card", 
                    null, 
                    delegate { SelectCardDialog(cardCategory); }
                    );
                cardToolStripButton.DropDownItems.Add(
                    cardCategory + " Card",
                    null,
                    delegate { SelectCardDialog(cardCategory); }
                    );
            }
        }

        /// <summary>
        /// Changes the display of the form when resized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            //titleBox.Width = ActiveForm.Width;                    //Tile box removed
            //applicationTitle.Width = ActiveForm.Width / 2;        //Removed application name text box and replaced with option in menu strip
            SizeChange();
            pictureBox.Size = flowLayoutPanel.Size;
            //disableArrowToolButton.Width = ActiveForm.Width / 5;  //DisableArrowToolButton removed
            //disableArrowToolButton.Location = new Point(ActiveForm.Width - (ActiveForm.Width / 5) - 30, 36);     //DisableArrowToolButton removed
            
        }

        /////////////////////////      INSERT CARDS      /////////////////////////

        /// <summary>
        /// Shows the card selection dialog for a specific type of card
        /// </summary>
        /// <param name="type">Type of card being inserted</param>
        private void SelectCardDialog(string type)
        {
            List<Card> cards = design.Deck.GetCategoryCards(type);
            
            //for (int i = 0; i < cards.Count; i++)
            //{

            //}
            foreach (Card c in design.Cards)
            {
                cards.Remove(c);
            }
            CardSelector cardSelectorForm = new CardSelector(type, cards, design);
            if (cardSelectorForm.ShowDialog() == DialogResult.OK)
            {
                AddCard(cardSelectorForm.selected);
                cards.Remove(cardSelectorForm.selected);
            }
            else if (cardSelectorForm.DialogResult == DialogResult.No)         
            {
                SelectCardDialog(cardSelectorForm.categoryChange);
            }
        }


        /////////////////////////      INSERT ARROW      /////////////////////////
        
        /// <summary>
        /// Enable arrow tool for inserting arrows. Change cursor and make disable button visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void arrowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (insertArrow)
            {
                insertArrow = false;
                ActiveForm.Cursor = Cursors.Default;
                connectionCards.Clear();
                currentArrow = null;
                //disableArrowToolButton.Visible = false;       //DisableArrowToolButton removed
                pictureBox.Invalidate();
            }
            else
            {
                insertArrow = true;
                ActiveForm.Cursor = Cursors.Hand;
                //disableArrowToolButton.Visible = true;        //DisableArrowToolButton removed
                this.Invalidate();
            }
            
        }


        /////////////////////////      EXPORT      /////////////////////////
        
        /// <summary>
        /// Export the design as an XML file. 
        /// Show message if an error occurs, the design is invalid or if the file is successfully created.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportAsXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Removes all 'other' type cards before export
            List<Card> temp = new List<Card>();
            foreach (Card c in design.Cards)
            {
                if (c.CardType == "Other")
                {
                    temp.Add(c);
                }
            }
            foreach (Card c in temp)
            {
                if (design.Cards.Contains(c))
                {
                    design.Cards.Remove(c);
                }
            }

            if (design.CheckCards())
            {
                saveFileDialog.Filter = "XML files|*.xml";
                saveFileDialog.FileName = design.CreateTitleCase(design.ApplicationName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        design.rules.Transform();
                        StreamWriter writer;
                        writer = File.CreateText(saveFileDialog.FileName);
                        writer.WriteLine(design.rules.xml);
                        

                        writer.Close();

                        MessageBox.Show(
                        "PModels, PIM and PMR successfully created.",
                        "Export Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    }
                    catch
                    {
                         MessageBox.Show(
                        "There was an error exporting your design.",
                        "Export Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show(
                    "Cannot export design as it is invalid.\nPlease ensure that you have filled in the application name and that all cards are connected using at least one arrow.",
                    "Invalid Design",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            //Add 'other' type cards back into the design
            foreach (Card c in temp)
            {
                design.Cards.Add(c);
            }
            
        }

        /// <summary>
        /// Captures an image of the picture box and allows user to save this. Filename is filled in according to the
        /// application name. If no application name is provided, the user is alerted to fill this in before proceeding.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportDesignAsJPGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (design.CheckTitle())
            {
                saveFileDialog.Filter = "JPG files|*.jpg";
                saveFileDialog.FileName = design.CreateTitleCase(design.ApplicationName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Bitmap designBitmap = new Bitmap(pictureBox.Width, pictureBox.Height);
                    pictureBox.DrawToBitmap(designBitmap, new Rectangle(0, 0, pictureBox.Width, pictureBox.Height));
                    designBitmap.Save(Path.GetFullPath(saveFileDialog.FileName), ImageFormat.Jpeg);
                }
            }
            else
            {
                MessageBox.Show(
                    "Please ensure that you have filled in the application name.",
                    "Application Name Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

        }


        /////////////////////////      FILE MENU - new, open, save and save as      /////////////////////////

        /// <summary>
        /// Closes the design form and reopens the form for the user to select the type of cards beign used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewDesign form = new NewDesign();
            form.ShowDialog();
            this.Close();
        }


        /// <summary>
        /// Open an existing design
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
            {
                if (SaveDialog(sender))
                {
                    saved = true;
                    this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
                }
            }
            StreamReader reader;
            string jsonString;

            openFileDialog.Filter = "JSON files|*.json";
            openFileDialog.FileName = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                reader = File.OpenText(openFileDialog.FileName);
                jsonString = reader.ReadToEnd();

                designFileName = openFileDialog.FileName;
                saveToolStripMenuItem.Enabled = true;
                saveToolStripButton.Enabled = true;

                reader.Close();
                design = JsonSerializer.Deserialize<Design>(jsonString);
                
                this.Hide();
                DesignForm form = new DesignForm(design.DeckType);

                form.design = design;
                form.design.EdgeListToArrows();
                //form.applicationTitle.Text = string.Concat(design.ApplicationName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                form.pictureBox.Invalidate();

                form.arrowToolStripMenuItem.Enabled = true;
                form.arrowToolStripButton.Enabled = true;
                form.saveToolStripMenuItem.Enabled = true;
                form.saveToolStripButton.Enabled = true;
                form.designFileName = openFileDialog.FileName;
                Debug.Print(openFileDialog.FileName);
                form.design.Cards = design.Cards;
                form.undoRedo.stateChanged(copyDesign());
                form.ShowDialog();
                this.Close();
            }
        }

        /// <summary>
        /// Overrides the design saved as the current filename.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(design, options);

            StreamWriter writer;
            writer = File.CreateText(designFileName);
            writer.WriteLine(jsonString);

            writer.Close();
            saved = true;
            this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }

        /// <summary>
        /// Saves the design as a JSON file as long as the application name field is not blank.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsDesignToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();

        }

        private void Save()
        {
            if (design.CheckTitle())
            {
                saveFileDialog.Filter = "JSON files|*.json";
                saveFileDialog.FileName = design.CreateTitleCase(design.ApplicationName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    cardCount.WriteFile();
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };

                    string jsonString = JsonSerializer.Serialize(design, options);

                    StreamWriter writer;
                    writer = File.CreateText(saveFileDialog.FileName);
                    writer.WriteLine(jsonString);

                    designFileName = saveFileDialog.FileName;
                    saveToolStripMenuItem.Enabled = true;
                    saveToolStripButton.Enabled = true;

                    writer.Close();
                    saved = true;
                    this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
                }
            }
            else
            {
                MessageBox.Show(
                    "Please ensure that you have filled in the application name.",
                    "Application Name Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /////////////////////////      HELP MENU     /////////////////////////

        /// <summary>
        /// Open arrow placement help form for the current deck type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HelpForm arrowHelpForm = new HelpForm(design.DeckType);
            arrowHelpForm.ShowDialog();
        }


        /////////////////////////      MOUSE EVENTS     /////////////////////////

        /// <summary>
        /// Moves the currently selected card or changes the cursor if no card is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            // Move currently selected card
            if (startX >= 0 && startY >= 0 && currentCard != null)
            {
                currentCard.MoveTo(currentX + (e.X - startX), currentY + (e.Y - startY));
                pictureBox.Refresh();
                saved = false;
                this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
                ResizePictureBox(currentCard);
            }
            else
            {
                // set arrow type depending on it if is over a card and if the arrow tool is chosen
                foreach (Card card in design.Cards)
                {
                    if (card.IsMouseOn(e.X, e.Y))
                    {
                        try
                        {
                            if (DesignForm.ActiveForm != null)
                            {
                                // cursor is over a card so sizeall cursor is used if arrow tool is not selected.
                                // if the arrow tool is selected then the hand cursor is used
                                DesignForm.ActiveForm.Cursor = insertArrow ? Cursors.Hand : Cursors.SizeAll;
                            }
                            this.Invalidate();
                            return;
                        }
                         catch (NullReferenceException)
                        {
                            Debug.WriteLine("Error with changing cursor");
                            return;
                        }
                    }
                }
                try
                {
                    if (DesignForm.ActiveForm != null)
                    {
                        // cursor is not over a card so the default cursor is used unless the arrow tool is selected
                        DesignForm.ActiveForm.Cursor = insertArrow ? Cursors.Hand : Cursors.Default;
                        this.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error with changing cursor");
                }
            }
            
        }

        private void ResizePictureBox(Card card)
        {
            if (card.Left + 150 >= pictureBox.Width)
            {
                pictureBox.Width += 10;
            }
            if (card.Top + 190 >= pictureBox.Height)
            {
                pictureBox.Height += 10;
            }
        }

        /// <summary>
        /// Finished moving the card
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            startX = -1;
            startY = -1;
            currentX = 0;
            currentY = 0;
        }

        /// <summary>
        /// Set the current card to the card that the mouse is over when mouse is pressed down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Card card in design.Cards)
            {
                if (card.IsMouseOn(e.X, e.Y))
                {
                    currentCard = card;
                    // start dragging the current card around
                    startX = e.X;
                    startY = e.Y;
                    currentX = currentCard.Left;
                    currentY = currentCard.Top;
                    pictureBox.Refresh();
                }
            }

        }

        /// <summary>
        /// Either add cards to the connectionCards list to add an arrow if arrow tool is selected
        /// or if right click show the appropriate context menu for the card or arrow clicked over
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            bool cardSelected = false;              //Saves whether a card or an empty space was clicked 
            if (insertArrow)
            {
                foreach (Card card in design.Cards)
                {
                    if (card.IsMouseOn(e.X, e.Y))
                    {
                        // remove card if it is already in the list
                        if (connectionCards.Contains(card))
                        {
                            connectionCards.Remove(card);
                        }
                        // add card if it isn't already in the list
                        else
                        {
                            connectionCards.Add(card);
                        }
                        cardSelected = true;
                        pictureBox.Invalidate();
                    }
                    else
                    {
                        cardSelected = false;
                    }


                    // create arrow if the to and from cards are selected
                    if (connectionCards.Count == 2)
                    {
                        currentArrow = new Arrow(connectionCards[0], connectionCards[1]);
                        design.arrows.Add(currentArrow);

                        connectionCards.Clear();
                        currentCard = null;
                        pictureBox.Invalidate();
                        if (doubleClick)
                        saved = false;
                        this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
                        undoRedo.stateChanged(copyDesign());
                        output();
                    }
                    if (doubleClick)
                    {
                        doubleClick = false;
                        insertArrow = false;
                        ActiveForm.Cursor = Cursors.Default;
                        currentCard = null;
                        deletetoolStripButton.Enabled = false;
                        deleteCardToolStripMenuItem.Enabled = false;
                        cutToolStripMenuItem.Enabled = false;
                        copyToolStripButton.Enabled = false;
                        copyToolStripMenuItem.Enabled = false;
                    }
                }
                if (!cardSelected)          //All cards are deselected if an empty space on the picturebox is clicked.
                {
                        connectionCards.Clear();
                        pictureBox.Invalidate();
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                foreach (Card card in design.Cards)
                {
                    if (card.IsMouseOn(e.X, e.Y))
                    {
                        // show custom card context menu
                        currentCard = card;
                        if (card.Custom)
                        {
                            contextMenuCustom.Show(Cursor.Position);
                            return;
                        }
                        // show normal card context menu
                        else
                        {
                            contextMenuNormal.Show(Cursor.Position);
                            return;
                        }
                    }
                }

                foreach (Arrow arrow in design.arrows)
                {
                    // show arrow context menu
                    if (arrow.IsMouseOn(e.X, e.Y))
                    {
                        currentArrow = arrow;
                        contextMenuArrow.Show(Cursor.Position);
                    }
                }

            }
            else if (currentArrow != null)
            {
                foreach (Arrow arrow in design.arrows)
                {
                    if (arrow == currentArrow)
                    {
                            currentArrow = null;
                            deleteCardToolStripMenuItem.Enabled = true;
                            deletetoolStripButton.Enabled = true;
                    }
                }
                pictureBox.Invalidate();
            }
        }


        /////////////////////////      CONTEXT MENUS      /////////////////////////
       
        /// <summary>
        /// Remove normal card context menu button
        /// Remove any connected arrows, return card to deck and remove card from design
        /// If there are no longer 2 cards disable the arrow tool in the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            connectionCards.Clear();
            doubleClick = false;
            design.RemoveConnectedArrows(currentCard);
            //design.Deck.ReturnCard(currentCard);
            design.Cards.Remove(currentCard);
            undoRedo.stateChanged(copyDesign());
            output();

            currentCard = null;
            ActiveForm.Cursor = Cursors.Default;
            doubleClick = false;
            insertArrow = false;
            pictureBox.Invalidate();

            if (design.Cards.Count < 2)
            {
                arrowToolStripMenuItem.Enabled = false;
                arrowToolStripButton.Enabled = false;
            }
            saved = false;
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
            deleteCardToolStripMenuItem.Enabled = false;
            deletetoolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
            copyToolStripButton.Enabled = false;
            cutToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// Remove custom card context menu button
        /// Remove any connected arrows, return card to deck and remove card from design
        /// If there are no longer 2 cards disable the arrow tool in the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeCardToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            design.RemoveConnectedArrows(currentCard);
            design.Cards.Remove(currentCard);
            undoRedo.stateChanged(copyDesign());
            output();

            currentCard = null;
            ActiveForm.Cursor = Cursors.Default;
            pictureBox.Invalidate();

            if (design.Cards.Count < 2)
            {
                arrowToolStripMenuItem.Enabled = false;
                arrowToolStripButton.Enabled = false;
            }
            saved = false;
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }

        /// <summary>
        /// Change name custom card context menu
        /// Open the form for naming the custom card and then change the name of the card if OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CardName customCardForm = new CardName();
            if (customCardForm.ShowDialog() == DialogResult.OK)
            {
                currentCard.Name = customCardForm.name;
                undoRedo.stateChanged(copyDesign());
                output();
            }
            saved = false;
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }

        /// <summary>
        /// Remove arrow context menu button
        /// Remove the currently selected arrow from the design
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeArrowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            design.arrows.Remove(currentArrow);
            currentArrow = null;
            undoRedo.stateChanged(design);
            output();

            ActiveForm.Cursor = Cursors.Default;
            pictureBox.Invalidate();
            saved = false;
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }


        /////////////////////////       OTHER CONTROLS      /////////////////////////
        
        /// <summary>
        /// Turns off the arrow tool
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disableArrowToolButton_Click(object sender, EventArgs e)
        {
            insertArrow = false;
            ActiveForm.Cursor = Cursors.Default;
            connectionCards.Clear();
            currentArrow = null;
            //disableArrowToolButton.Visible = false;       //DisableArrowToolButton removed
            pictureBox.Invalidate();
        }

        /// <summary>
        /// Set the application name of the design (in title case) whenever the application name is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applicationTitle_TextChanged(object sender, EventArgs e)
        {
            //design.ApplicationName = applicationTitle.Text;
        }

        private void DesignForm_Load(object sender, EventArgs e)
        {
            SizeChange();
            pictureBox.Size = flowLayoutPanel.Size;
            NameApplication();
            this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
            DesignForm_MaximizedBoundsChanged(sender, e);
            //this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Set the name of the application 
        /// </summary>
        private void NameApplication()
        {
            NameForm nameDialog = new NameForm();
            nameDialog.txtAppName.Text = design.ApplicationName;
            Debug.Print("design.application name" +design.ApplicationName);
            // Show the nameDialog as a modal dialog and determine if DialogResult = OK.
            if (nameDialog.ShowDialog(this) == DialogResult.OK)
            {

                // Read the contents of testDialog's TextBox.
                design.ApplicationName = nameDialog.txtAppName.Text;
            }
            else
            {
                design.ApplicationName = "";
            }
            nameDialog.Dispose();
            if (design.ApplicationName == "")
            {
                design.ApplicationName = "Untitled";
            }
        }

        private void renameApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NameApplication();
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
            saved = false;
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string jsonString = JsonSerializer.Serialize(design, options);

            StreamWriter writer;
            writer = File.CreateText(designFileName);
            writer.WriteLine(jsonString);

            writer.Close();
            saved = true;
            this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }

        private void arrowToolStripButton_Click(object sender, EventArgs e)
        {
            if (insertArrow)
            {
                ActiveForm.Cursor = Cursors.Default;
                connectionCards.Clear();
                currentArrow = null;
                //disableArrowToolButton.Visible = false;   //DisableArrowToolButton removed
                pictureBox.Invalidate();
                insertArrow = false;
            }
            else
            {
                insertArrow = true;
                ActiveForm.Cursor = Cursors.Hand;
                //disableArrowToolButton.Visible = true;        //DisableArrowToolButton
                //removed
                this.Invalidate();
            }        
        }


        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!insertArrow)
            {
                foreach (Card card in design.Cards)
                {
                    if (card.IsMouseOn(e.X, e.Y))
                    {
                        insertArrow = true;
                        ActiveForm.Cursor = Cursors.Hand;
                        deleteCardToolStripMenuItem.Enabled = true;
                        deletetoolStripButton.Enabled = true;
                        cutToolStripMenuItem.Enabled = true;
                        if (currentCard.Custom)
                        {
                            copyToolStripButton.Enabled = true;
                            copyToolStripMenuItem.Enabled = true;
                        }
                        this.Invalidate();
                        doubleClick = true;
                        currentCard = card;
                        // remove card if it is already in the list
                        if (connectionCards.Contains(card))
                        {
                            connectionCards.Remove(card);
                        }
                        // add card if it isn't already in the list
                        else
                        {
                            connectionCards.Add(card);
                        }
                        pictureBox.Invalidate();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the design as a JSON file as long as the application name field is not blank.
        /// <param name="e"></param>
        private void saveAllToolStripButton_Click(object sender, EventArgs e)
        {
            if (design.CheckTitle())
            {
                saveFileDialog.Filter = "JSON files|*.json";
                saveFileDialog.FileName = design.CreateTitleCase(design.ApplicationName);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    };

                    string jsonString = JsonSerializer.Serialize(design, options);

                    StreamWriter writer;
                    writer = File.CreateText(saveFileDialog.FileName);
                    writer.WriteLine(jsonString);

                    designFileName = saveFileDialog.FileName;
                    saveToolStripMenuItem.Enabled = true;
                    saveToolStripButton.Enabled = true;

                    writer.Close();
                    saved = true;
                    this.Text = design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
                }
            }
            else
            {
                MessageBox.Show(
                    "Please ensure that you have filled in the application name.",
                    "Application Name Missing",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }

        private bool SaveDialog(object sender)
        {
            SaveConfirmation save = new SaveConfirmation(design.ApplicationName);
            if (save.ShowDialog(this) == DialogResult.OK)
            {
                Save();
            }
            else if (save.DialogResult == DialogResult.Cancel)
            {
                save.Dispose();
                return false;
            }
            save.Dispose();
            return true;
        }

        private void DesignForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
            {
                if (!SaveDialog(sender)){
                    e.Cancel = true;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            exportAsXMLToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            exportDesignAsJPGToolStripMenuItem_Click(sender, e);
        }

        private void DesignForm_MaximizedBoundsChanged(object sender, EventArgs e)
        {
            SizeChange();
        }

        private void SizeChange()
        {
            if (ActiveForm != null)
            {
                tabControl.Location = new Point(toolStrip1.Width, menuStrip1.Height);
                flowLayoutPanel.Location = new Point(0,0);
                pictureBox.Location = new Point(0, 0);
                tabControl.Height = ActiveForm.Height - menuStrip1.Height - ActiveForm.MainMenuStrip.Height;
                tabControl.Width = ActiveForm.Width - toolStrip1.Width;
                flowLayoutPanel.Height = C2M.Height - 10;
                flowLayoutPanel.Width = C2M.Width - 10;
                UpdateModelsLayout();
            }
        }

        private void DesignForm_MaximumSizeChanged(object sender, EventArgs e)
        {
            SizeChange();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoRedo.canUndo())
            {
                undoRedoDesign = new Design(deckType);
                undoRedoDesign = undoRedo.undo();
                design.ApplicationName = undoRedoDesign.ApplicationName;
                design.Cards.Clear();
                design.arrows.Clear();
                foreach (Card item in undoRedoDesign.Cards)
                {
                    design.Cards.Add(item);
                }
                foreach (Arrow item in undoRedoDesign.arrows)
                {
                    design.arrows.Add(item);
                }
                output();
                redoToolStripMenuItem.Enabled = true;
                pictureBox.Invalidate();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoRedo.canRedo())
            {
                undoRedoDesign = new Design(deckType);
                undoRedoDesign = undoRedo.redo();
                design.ApplicationName = undoRedoDesign.ApplicationName;
                design.Cards.Clear();
                design.arrows.Clear();
                foreach (Card item in undoRedoDesign.Cards)
                {
                    design.Cards.Add(item);
                }
                foreach (Arrow item in undoRedoDesign.arrows)
                {
                    design.arrows.Add(item);
                }
                output();
                pictureBox.Invalidate();
            }
            else
            {
                redoToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Draw the design onto the picture box and draw a yellow rectangle over selected cards when adding arrows
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            design.DrawCards(e.Graphics);
            design.DrawArrows(e.Graphics);

            foreach (Card card in connectionCards)
            {
               e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(70, 255, 255, 0)), card.Left, card.Top, Card.WIDTH, Card.HEIGHT);
            }
        }

        /////////////////////////      OTHER METHODS      /////////////////////////
        
        /// <summary>
        /// Place card onto the form and add to design. 
        /// If there are at least two cards then enable the arrow tool in the menu
        /// </summary>
        /// <param name="card"></param>
        private void AddCard(Card card)
        {
            card.MoveTo(pictureBox.Width / 2 - Card.WIDTH / 2, pictureBox.Height / 2 - Card.HEIGHT / 2);
            design.Cards.Add(card);
            if (card.CardType != "Other")
            {
                cardCount.CountCard(card);
            }
            
            undoRedo.stateChanged(copyDesign());
            output();
            redoToolStripMenuItem.Enabled = false;
            pictureBox.Invalidate();

            if (design.Cards.Count >= 2)
            {
                arrowToolStripMenuItem.Enabled = true;
                arrowToolStripButton.Enabled = true;
            }
            saved = false;
            this.Text = "*" + design.ApplicationName + " - C2M - " + design.DeckType + " Cards";
        }

        private void deletetoolStripButton_Click(object sender, EventArgs e)
        {
            removeCardToolStripMenuItem_Click(sender, e);
            removeArrowToolStripMenuItem_Click(sender, e);
        }

        private void deleteCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            removeCardToolStripMenuItem_Click(sender, e);
            removeArrowToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem_Click(sender, e);
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentCard != null)
            {
                if (currentCard.Custom)
                {
                    cardCopy = new Card(currentCard.CardType, currentCard.Name, currentCard.Code, true);
                }
            }
            copyToolStripButton.Enabled = false;
            copyToolStripMenuItem.Enabled = false;
            pasteToolStripMenuItem.Enabled = true;
            connectionCards.Clear();
            pictureBox.Invalidate();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cardCopy != null)
            {
                Card newCard = new Card(cardCopy.CardType, cardCopy.Name, cardCopy.Code, cardCopy.Custom);
                design.Cards.Add(newCard);
                undoRedo.stateChanged(copyDesign());
                if (!cardCopy.Custom)
                {
                    cardCopy = null;
                    pasteToolStripMenuItem.Enabled = false;
                }
            }
            pictureBox.Invalidate();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentCard != null)
            {
                cardCopy = currentCard;
                removeCardToolStripMenuItem_Click(sender, e);
                undoRedo.stateChanged(copyDesign());
            }
            connectionCards.Clear();
            pictureBox.Invalidate();
            pasteToolStripMenuItem.Enabled = true;
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            Model_Output();
            if (tabControl.SelectedTab == C2M)
            {
                cardToolStripButton.Visible = true;
                deletetoolStripButton.Visible = true;
                arrowToolStripButton.Visible = true;
                copyToolStripButton.Visible = true;
                editToolStripMenuItem.Visible = true;
                insertToolStripMenuItem.Visible = true;
                helpToolStripMenuItem.Visible = true;
                maximizetoolStripDropDownButton.Visible = false;
                viewToolStripMenuItem.Visible = false;
            }
            else if (tabControl.SelectedTab == Models)
            {
                cardToolStripButton.Visible = false;
                deletetoolStripButton.Visible = false;
                arrowToolStripButton.Visible = false;
                copyToolStripButton.Visible = false;
                editToolStripMenuItem.Visible = false;
                insertToolStripMenuItem.Visible = false;
                helpToolStripMenuItem.Visible = false;
                maximizetoolStripDropDownButton.Visible = true;
                viewToolStripMenuItem.Visible = true;
                UpdateModelsLayout();
            }
        }

        private void UpdateModelsLayout()
        {
            if (pModelVisible)
            {
                panelPModel.Height = Models.Height - 20;
                if (!pMRVisible && !pIMVisible)
                {
                    panelPModel.Width = Models.Width - 20;
                }
                else
                {
                    panelPModel.Width = (Models.Width / 2) - 10;
                    if (pIMVisible)
                    {
                        panelPIM.Top = 5;
                        panelPIM.Left = Models.Width / 2;
                        panelPIM.Width = (Models.Width / 2) - 20;
                        if (!pMRVisible)
                        {
                            panelPIM.Height = Models.Height - 20;
                        }
                        else
                        {
                            panelPIM.Height = (Models.Height / 2) - 10;
                            panelPMR.Left = Models.Width / 2;
                            panelPMR.Top = Models.Height / 2;
                            panelPMR.Width = (Models.Width / 2) - 20;
                            panelPMR.Height = (Models.Height / 2) - 15;
                        }
                    }
                    else
                    {
                        panelPMR.Top = 5;
                        panelPMR.Left = Models.Width / 2;
                        panelPMR.Width = (Models.Width / 2) - 20;
                        panelPMR.Height = Models.Height - 15;
                    }
                }
            }
            else
            {
                if (pIMVisible)
                {
                    panelPIM.Top = 5;
                    panelPIM.Left = 5;
                    panelPIM.Width = Models.Width - 20;
                    if (!pMRVisible)
                    {
                        panelPIM.Height = Models.Height - 20;
                    }
                    else
                    {
                        panelPIM.Height = (Models.Height / 2) - 10;
                        panelPMR.Left = 5;
                        panelPMR.Top = Models.Height / 2;
                        panelPMR.Width = Models.Width - 20;
                        panelPMR.Height = (Models.Height / 2) - 20;
                    }
                }
                else
                {
                    panelPMR.Top = 5;
                    panelPMR.Left = 5;
                    panelPMR.Width = Models.Width - 20;
                    panelPMR.Height = Models.Height - 20;
                }
            }
        }

        private void undoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (undoRedo.canUndo())
            {
                Design curr = undoRedo.undo();
                design.arrows.Clear();
                design.Cards.Clear();
                design.ApplicationName = curr.ApplicationName;
                foreach (Card item in curr.Cards)
                {
                    design.Cards.Add(item);
                }
                foreach (Arrow item in curr.arrows)
                {
                    design.arrows.Add(item);
                }
                redoToolStripMenuItem.Enabled = true;
            }
            pictureBox.Refresh();
        }

        private void redoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (undoRedo.canRedo())
            {
                Design curr = undoRedo.redo();
                design.arrows.Clear();
                design.Cards.Clear();
                design.ApplicationName = curr.ApplicationName;
                foreach (Card item in curr.Cards)
                {
                    design.Cards.Add(item);
                }
                foreach (Arrow item in curr.arrows)
                {
                    design.arrows.Add(item);
                }
            }
            pictureBox.Refresh();
        }

        private void buttomPModelMinimize_Click(object sender, EventArgs e)
        {
            pModelVisible = false;
            panelPModel.Visible = false;
            UpdateModelsLayout();
            pModelToolStripMenuItem.Visible = true;
            pModelToolStripMenuItem1.Visible = true;
        }

        private void toolStripDropDownButton1_VisibleChanged(object sender, EventArgs e)
        {
            
        }

        private void buttomPIMMinimize_Click(object sender, EventArgs e)
        {
            pIMVisible = false;
            panelPIM.Visible = false;
            UpdateModelsLayout();
            pIMToolStripMenuItem.Visible = true;
            pIMToolStripMenuItem1.Visible = true;
        }

        private void buttomPMRMinimize_Click(object sender, EventArgs e)
        {
            pMRVisible = false;
            panelPMR.Visible = false;
            UpdateModelsLayout();
            pMRToolStripMenuItem.Visible = true;
            pModelToolStripMenuItem1.Visible = true;
        }

        private void pModelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pModelVisible = true;
            panelPModel.Visible = true;
            UpdateModelsLayout();
            pModelToolStripMenuItem.Visible = false;
        }

        private void pIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pIMVisible = true;
            panelPIM.Visible = true;
            UpdateModelsLayout();
            pIMToolStripMenuItem.Visible = false;
        }

        private void pMRToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pMRVisible = true;
            panelPMR.Visible = true;
            UpdateModelsLayout();
            pMRToolStripMenuItem.Visible = false;
        }

        /// <summary>
        /// Display program information in the output box
        /// </summary>
        private void output()
        {
            Debug.Print("--------------------------------------------------------------------------");
            Debug.Print("Cards");
            string lstUndo = "Undo: ";
            foreach (Design item in undoRedo.lstUndo())
            {
                if (item != null)
                {
                    string list = null;
                    foreach (Card c in item.Cards)
                    {
                        list = list + c.Name + "," ;

                    }
                    if (list != null) lstUndo = lstUndo + "\n" + list;
                }
            }
            Debug.Print(lstUndo);

            string lstRedo = "Redo: ";
            foreach (Design item in undoRedo.lstRedo())
            {
                if (item != null)
                {
                    string list = null;
                    foreach (Card c in item.Cards)
                    {
                        list = list + c.Name + ","; 
                    }
                    if (list != null) lstRedo = lstRedo + "\n" + list;
                }
            }
            Debug.Print(lstRedo);

            string lstCurr = "Curr: ";
            if (undoRedo.currState() != null)
            {
                string list = null;
                foreach (Card c in design.Cards)
                {
                    list = list + c.Name + ",";
                }
                if (list != null) lstCurr = lstCurr + "\n" + list;
            }
            Debug.Print(lstCurr + "\n");
            Debug.Print("Arrows:");
            lstUndo = "Undo: ";
            foreach (Design item in undoRedo.lstUndo())
            {
                if (item != null) 
                {
                    string list = null;
                    foreach (Arrow c in item.arrows)
                    {
                        list = list + "(" + c.From.Name + ":" + c.To.Name + "),";

                    }
                    if (list != null) lstUndo = lstUndo + "\n" + list;
                }
            }
            Debug.Print(lstUndo);

            lstRedo = "Redo: ";
            foreach (Design item in undoRedo.lstRedo())
            {
                if (item != null)
                {
                    string list = null;
                    foreach (Arrow c in item.arrows)
                    {
                        list = list + "(" + c.From.Name + ":" + c.To.Name + "),";
                    }
                    if (list != null) lstRedo = lstRedo + "\n" + list;
                }
            }
            Debug.Print(lstRedo);

            lstCurr = "Curr: ";
            if (undoRedo.currState() != null)
            {
                string list = null;
                foreach (Arrow c in design.arrows)
                {
                    list = list + "(" + c.From.Name + ":" + c.To.Name + "),";
                }
                if (list != null) lstCurr = lstCurr + "\n" + list;
            }
            Debug.Print(lstCurr);
        }

        /// <summary>
        /// Creates a new design to save the contents of the main design sor seperate use.
        /// </summary>
        /// <returns></returns>
        private Design copyDesign()
        {
            undoRedoDesign = new Design(deckType);
            undoRedoDesign.arrows.Clear();
            undoRedoDesign.Cards.Clear();
            undoRedoDesign.ApplicationName = design.ApplicationName;
            foreach (Card item in design.Cards)
            {
                undoRedoDesign.Cards.Add(item);
            }
            foreach (Arrow item in design.arrows)
            {
                undoRedoDesign.arrows.Add(item);
            }
            return undoRedoDesign;
        }

        private void Model_Output()
        {
            design.rules.Transform();
            List<PresentationModel> pModels = design.rules.GetPModels();
            List<Widget> widgets = design.rules.GetWidgets();
            string output = "PModel\t\t";
            foreach (PresentationModel m in pModels)
            {
                output += m.name + " ";
            }
            output += design.ApplicationName + "\nWidgetName\t";
            foreach (Widget w in widgets)
            {
                output += w.name + " ";
            }
            output += "\nCategory\t\t";
            foreach (Widget w in widgets)
            {
                if (!output.Contains(w.category))
                {
                    output += w.category + " ";
                }
            }
            output += "\nBehaviour\t";
            foreach (Widget w in widgets)
            {
                foreach (string s in w.S_Behaviours)
                {
                    if (!output.Contains(s)) output += s + " "; 
                    
                }
                foreach (string i in w.I_Behaviours)
                {
                    if (!output.Contains(i)) output += i + " ";
                }
            }
            output += "\n\n" + design.ApplicationName + " is ";
            for (int i = 0; i < pModels.Count; i++)
            {
                if (pModels.Last().name == pModels[i].name)
                {
                    output += pModels[i].name + "\n\n";
                }
                else
                {
                    output += pModels[i].name + " : ";
                }
            }
            foreach (PresentationModel p in pModels)
            {
                output += p.name + " is\n";
                foreach (Widget w in p.widgets)
                {
                    output += "(" + w.name + ", " + w.category + ", (";
                    if (w.category == "Responder")
                    {
                        for (int i = 0; i < w.S_Behaviours.Count; i++)
                        {
                            if (w.S_Behaviours.Last() == w.S_Behaviours[i])
                            {
                                output += w.S_Behaviours[i] + "))\n";
                            }
                            else
                            {
                                output += w.S_Behaviours[i] + ", ";
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < w.I_Behaviours.Count; i++)
                        {
                            if (w.I_Behaviours.Last() == w.I_Behaviours[i])
                            {
                                output += w.I_Behaviours[i] + "))\n";
                            }
                            else
                            {
                                output += w.I_Behaviours[i] + ", ";
                            }
                        }
                    }
                }
                output += "\n";
            }
            string pmr = "";
            List<string> S_Behaviours = new List<string>();
            foreach (PresentationModel pm in pModels)
            {
                foreach (Widget w in pm.widgets)
                {
                    S_Behaviours.AddRange(w.S_Behaviours);
                }
            }

            S_Behaviours = S_Behaviours.Distinct().ToList();
            pmr += "Presentation Model Relation (PMR)\n";
            foreach (string s in S_Behaviours)
            {
                pmr += "\t" + s + " -> " + s.Replace("S_", "") + "\n";
            }

            pModelRichTextBox.Clear();
            pModelRichTextBox.Text = output;
            pMRRichTextBox.Clear();
            pMRRichTextBox.Text = pmr;
        }
    }
}
