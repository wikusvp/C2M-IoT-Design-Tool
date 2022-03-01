using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardsToPModels
{
    /// <summary>
    /// An object containing all information for a specific card design
    /// </summary>
    public class Design
    {
        /// <summary>
        /// List of arrows that are part of the design
        /// </summary>
        public List<Arrow> arrows;

        private List<Card> _cards;
        private Deck _deck;
        private string _applicationName;
        private List<Tuple<int, int>> _edgeList;

        /// <summary>
        /// The rules object containing the methods for changing the card design to a model.
        /// </summary>
        public Rules rules;

        /// <summary>
        /// The different decks of cards available
        /// </summary>
        public enum Decks { IoT, Mobile, Conceptual };

        private Decks _deckType;

        private List<string> _cardCategories;

       /// <summary>
       /// Creates a new deck for the specified deck type with an empty list of cards and arrows.
       /// </summary>
       /// <param name="DeckType">The type of cards used in the deck</param>
        [JsonConstructor]
        public Design(Decks deckType)
        {
            _cards = new List<Card>();
            arrows = new List<Arrow>();
            _edgeList = new List<Tuple<int, int>>();
            _cardCategories = new List<string>();
            _deckType = deckType;
            _deck = new Deck(this);
            _deck.CreateDeck();

            switch (_deckType)
            {
                case Decks.IoT:
                    rules = new IoTRules(this);
                    break;
                case Decks.Mobile:
                    rules = new MobileRules(this);
                    break;
                case Decks.Conceptual:
                    rules = new ConceptualRules(this);
                    break;
            }
        }

        /// <summary>
        /// Read and write property for the deck object which contains the cards not currently used in the design
        /// </summary>
        public Deck Deck
        {
            get { return _deck; }
            set { _deck = value; }
        }

        /// <summary>
        /// Read and write property for a list of the different card categories in the deck
        /// </summary>
        public List<string> CardCategories
        {
            get { return _cardCategories; }
            set { _cardCategories = value; }
        }

        /// <summary>
        /// Read and write property for the type of deck
        /// </summary>
        public Decks DeckType
        {
            get { return _deckType; }
            set { _deckType = value; }
        }

        /// <summary>
        /// List of all cards part of the design
        /// </summary>
        public List<Card> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }

        /// <summary>
        /// Name of the application in title case
        /// </summary>
        public string ApplicationName
        {
            get {
                return _applicationName;
            }
            set {
                _applicationName = value;
            }
        }

        /// <summary>
        /// A list of int pairs representing the index of the cards with arrows between them.
        /// First int is the from card, second is the to card
        /// </summary>
        public List<Tuple<int,int>> EdgeList
        {
            get 
            {
                ArrowsToEdgeList();
                return _edgeList;
            }
            set { _edgeList = value; }
        }


        /// <summary>
        /// Draw each card that is part of the design
        /// </summary>
        /// <param name="paper">Graphics object for where the cards are being drawn</param>
        public void DrawCards(Graphics paper)
        {
            foreach (Card card in Cards)
            {
                card.Draw(paper);
            }
        }

        /// <summary>
        /// Draws each arrow between the cards in the design
        /// </summary>
        /// <param name="paper">Graphics object for where to be drawn</param>
        public void DrawArrows(Graphics paper)
        {
            foreach (Arrow arrow in arrows)
            {
                arrow.Draw(paper);
            }
        }

        /// <summary>
        /// Remove all arrows connected to the card
        /// </summary>
        /// <param name="card">Card whose arrows are being removed</param>
        public void RemoveConnectedArrows(Card card)
        {
            for (int i = 0; i < arrows.Count; i++)
            {
                if (arrows[i].ArrowContains(card))
                {
                    arrows.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Convert the arrows to an edge list for saving to a json file
        /// </summary>
        public void ArrowsToEdgeList()
        {
            _edgeList.Clear();
            foreach (Arrow arrow in arrows)
            {
                _edgeList.Add(new Tuple<int, int>(Cards.FindIndex(c => c == arrow.From), Cards.FindIndex(c => c == arrow.To)));
            }
        }

        /// <summary>
        /// Convert the edge list to arrows.
        /// This ensures that the arrows use the cards in the design rather than creating copies of them.
        /// Use when opening from a json file
        /// </summary>
        public void EdgeListToArrows()
        {
            foreach (Tuple<int, int> edge in _edgeList)
            {
                arrows.Add(new Arrow(Cards[edge.Item1], Cards[edge.Item2]));
            }
        }

        /// <summary>
        /// Checks that there is at least two cards and every card has an arrow connected to it
        /// </summary>
        /// <returns>true is design is valid and false if design is invalid</returns>
        public bool CheckCards()
        {
            if (!CheckTitle())
                return false;

            if (Cards.Count == 1)
                return false;

            List<bool> cardCheck = new List<bool>();
            List<bool> arrowCheck = new List<bool>();
            foreach (Card card in Cards)
            {
                foreach (Arrow arrow in arrows)
                {
                    if (card.Equals(arrow.To) || card.Equals(arrow.From))
                    {
                        arrowCheck.Add(true);
                    }
                    else
                    {
                        arrowCheck.Add(false);
                    }
                }
                if (arrowCheck.Contains(true))
                    cardCheck.Add(true);
                else
                    cardCheck.Add(false);
                arrowCheck.Clear();
            }    
            if (cardCheck.Contains(false))
                return false;

            return true;
        }

        /// <summary>
        /// Checks that an application name has been entered
        /// </summary>
        /// <returns>true if there is a name and false if there is not a name</returns>
        public bool CheckTitle()
        {
            if (ApplicationName == null)
                return false;
            return true;
        }

        /// <summary>
        /// Formats the given string in TitleCase
        /// </summary>
        /// <param name="name">String to format</param>
        /// <returns>Formatted string</returns>
        public string CreateTitleCase(string name)
        {
            TextInfo textFormat = new CultureInfo("en-US", false).TextInfo;
            return textFormat.ToTitleCase(name.ToLower()).Replace(" ", "");
        }
    }
}
