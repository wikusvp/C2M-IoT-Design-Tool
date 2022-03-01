using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    /// <summary>
    /// A Deck object contains a list of card objects for a specific type of deck
    /// </summary>
    public class Deck
    {
        private List<Card> _cards;
        private string _deckNamesFile;
        private Design _design;

        /// <summary>
        /// Create a new deck object, empty constructor is used for JSON desterialization
        /// </summary>
        public Deck() { }

        /// <summary>
        /// Depending on the type of deck, the appropriate text file is chosen which contains the information
        /// for each card. An empty list of cards is created.
        /// </summary>
        /// <param name="design">The design the deck is for</param>
        public Deck(Design design)
        {
            switch (design.DeckType)
            {
                case Design.Decks.IoT:
                    _deckNamesFile = "IoTNames.txt";
                    break;
                case Design.Decks.Mobile:
                    _deckNamesFile = "MobileNames.txt";
                    break;
                case Design.Decks.Conceptual:
                    _deckNamesFile = "ConceptualNames.txt";
                    break;
            }
            _cards = new List<Card>();
            _design = design;
        }

        /// <summary>
        /// Read and write property for the list of cards in the deck
        /// </summary>
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }


        /// <summary>
        /// Creates a new deck of cards from the file containing the card information for that type of deck
        /// </summary>
        public void CreateDeck()
        {
            ResourceManager resourceManager = Properties.Resources.ResourceManager;
            StreamReader reader;
            string line = "";
            reader = File.OpenText(_deckNamesFile);

            while (!reader.EndOfStream)
            {
                try
                {
                    line = reader.ReadLine();
                    string[] lineArray = line.Split(',');
                    _cards.Add(new Card(lineArray[2], lineArray[0], lineArray[1], false));
                    if (!_design.CardCategories.Contains(lineArray[2]))
                    {
                        _design.CardCategories.Add(lineArray[2]);
                    }
                }
                catch
                {
                    Console.WriteLine("Error: " + line);
                }

            }
            reader.Close();
        }

        /// <summary>
        /// Return a card to the deck
        /// </summary>
        /// <param name="card">Card to return</param>
        public void ReturnCard(Card card)
        {
            Cards.Add(card);
        }

        /// <summary>
        /// Gets all of the cards of a specific type from the deck
        /// </summary>
        /// <param name="type">Type of card being selected</param>
        /// <returns>A list of cards of the specified type</returns>
        public List<Card> GetCategoryCards(string type)
        {
            List<Card> categoryCards = new List<Card>();
            foreach (Card c in Cards)
            {
                if (c.CardType == type)
                    categoryCards.Add(c);
            }
            return categoryCards;
        }
    }
}
