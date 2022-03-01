using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    /// <summary>
    /// Abstract class containing common methods for all decks of cards
    /// </summary>
    public abstract class Rules
    {
        /// <summary>
        /// Constant for the Responder category string
        /// </summary>
        protected const string RESPONDER = "Responder";
        /// <summary>
        /// Constant for the ActionControl category string
        /// </summary>
        protected const string ACTIONCONTROL = "ActionControl";
        /// <summary>
        /// Constant for the Display category string
        /// </summary>
        protected const string DISPLAY = "Display";

        /// <summary>
        /// Design being changed to a model
        /// </summary>
        public Design design;

        /// <summary>
        /// String containing the formatted XML to be saved
        /// </summary>
        public string xml;

        /// <summary>
        /// Presentation Models for the current design
        /// </summary>
        protected List<PresentationModel> PModels;


        /// <summary>
        /// Assigns the design and creates an empty list for the presentation models
        /// </summary>
        /// <param name="design"></param>
        public Rules(Design design)
        {
            this.design = design;
            PModels = new List<PresentationModel>();
        }

        /// <summary>
        /// Constructs the PModels from the design
        /// </summary>
        public void Transform()
        {
            GeneratePModel();
            xml += "<PIMS>\n  <PresentationModel>\n  <PModel>" + design.CreateTitleCase(design.ApplicationName) + "</PModel>\n\n";
            foreach (PresentationModel pm in PModels)
            {
                xml += pm.GeneratePM();
            }
            xml += "</PresentationModel>\n\n<PIM>\n" + GeneratePIM() + "</PIM>\n\n<PMR>\n" + GeneratePMR() + "</PMR>\n</PIMS>";

        }

        /// <summary>
        /// Creates the PModels and widgets in them from the card design by performing the appropriate rule
        /// for each arrow in the design.
        /// </summary>
        protected abstract void GeneratePModel();

        /// <summary>
        /// Given two cards this uses a switch statament to decide which rule to call
        /// </summary>
        /// <param name="startCard">Card the arrow starts from</param>
        /// <param name="c">Card the arrow is going to</param>
        protected abstract void PerformRule(Card startCard, Card c);

        /// <summary>
        /// Returns all cards that the given card is going to
        /// </summary>
        /// <param name="card">Starting card</param>
        /// <returns>All cards the current card is going to</returns>
        protected List<Card> FindConnectedCardsTo(Card card)
        {
            List<Card> cards = new List<Card>();
            foreach (Arrow arrow in design.arrows)
            {
                if (arrow.From == card)
                    cards.Add(arrow.To);
            }
            return cards;
        }

        /// <summary>
        /// Returns all cards that are pointing towards the given card
        /// </summary>
        /// <param name="card">Starting card</param>
        /// <returns>List of cards that are going to the current card</returns>
        protected List<Card> FindConnectedCardsFrom(Card card)
        {
            List<Card> cards = new List<Card>();
            foreach (Arrow arrow in design.arrows)
            {
                if (arrow.To == card)
                    cards.Add(arrow.From);
            }
            return cards;
        }

        /// <summary>
        /// Finds the cards connected to the given card
        /// </summary>
        /// <param name="card">Starting card</param>
        /// <returns>List of cards that the current card is connected to</returns>
        protected List<Card> FindConnectedCards(Card card)
        {
            List<Card> cards = new List<Card>();
            foreach (Arrow arrow in design.arrows)
            {
                if (arrow.From == card)
                    cards.Add(arrow.To);
                else if (arrow.To == card)
                    cards.Add(arrow.From);
            }
            return cards;
        }

        /// <summary>
        /// Creates the XML for the PIM
        /// </summary>
        /// <returns>XML for PIM as a string</returns>
        private string GeneratePIM()
        {
            string pim = "";
            List<string> ibehsInCurrentPModel = new List<string>();
            foreach (PresentationModel pm in PModels)
            {
                foreach (Widget w in pm.widgets)
                {
                    if (w.I_Behaviours.Count != 0)
                    {
                        ibehsInCurrentPModel.AddRange(w.GetIBehaviourNames());
                    }
                }
                ibehsInCurrentPModel = ibehsInCurrentPModel.Distinct().ToList();
                foreach (string i in ibehsInCurrentPModel)
                {
                    pim += "<transition>\n  <start>" + pm.name + "</start>\n  <end>" + i + "</end>\n  <ibeh>I_"
                               + i + "</ibeh>\n</transition>\n";
                }
                ibehsInCurrentPModel.Clear();
            }

            return pim;
        }

        /// <summary>
        /// Creates XML for the PMR
        /// </summary>
        /// <returns>XML for PMR as a string</returns>
        private string GeneratePMR()
        {
            string pmr = "";
            List<string> S_Behaviours = new List<string>();
            foreach (PresentationModel pm in PModels)
            {
                foreach (Widget w in pm.widgets)
                {
                    S_Behaviours.AddRange(w.S_Behaviours);
                }
            }

            S_Behaviours = S_Behaviours.Distinct().ToList();

            foreach (string s in S_Behaviours)
            {
                pmr += "<rel>\n  <sbeh>" + s + "</sbeh>\n  <sop>" + s.Replace("S_", "") + "</sop>\n</rel>\n";
            }
            return pmr;
        }

        /// <summary>
        /// Divides the PModel placeholder into separate PModels if needed according to S-behaviours
        /// and adds I-behaviours to navigate between them
        /// </summary>
        /// <param name="placeHolder">PModel holding all widgets</param>
        protected void Split(PresentationModel placeHolder)
        {
            // Add the widget that corresponds to the PModel to the PModel and remove from the placeholder
            foreach (PresentationModel pm in PModels)
            {
                Widget widget = placeHolder.widgets.Find(w => w.name.Contains(pm.name));
                pm.AddWidget(widget);
                placeHolder.widgets.Remove(widget);
            }

            // For the remaining widgets in the placeholder PModel, if the widget has any S-behaviours that match
            // the ones in the PModel widget or if the PModel widget is using or used by the widget, it is added
            // to the PModel
            foreach (PresentationModel pm in PModels)
            {
                foreach (Widget w in placeHolder.widgets)
                {
                    Widget pmWidget = pm.widgets.Find(wid => wid.name.Contains(pm.name));
                    if (w.S_Behaviours.Any(b => pmWidget.S_Behaviours.Contains(b)))
                    {
                        pm.AddWidget(w);
                    }
                    else if (pmWidget.S_Behaviours.Any(b => b.Contains(w.name)))
                    {
                        pm.AddWidget(w);
                    } else if (w.S_Behaviours.Any(b => b.Contains(pm.name)))
                    {
                        pm.AddWidget(w);
                    }
                }
            }
            AddNavigation();

        }

        /// <summary>
        /// Add I_Behaviours for each presentation model
        /// </summary>
        private void AddNavigation()
        {
            foreach (PresentationModel pm in PModels)
            {
                foreach (PresentationModel p in PModels) if (p != pm)
                    {
                        Widget navigation = new Widget("Open" + p.name, ACTIONCONTROL);
                        navigation.AddIBehaviour(p.name);
                        pm.widgets.Add(navigation);
                    }
            }
        }

        public List<PresentationModel> GetPModels()
        {
            return PModels;
        }

        public List<Widget> GetWidgets()
        {
            List<Widget> w = new List<Widget>();
            foreach (PresentationModel m in PModels)
            {
                foreach (Widget widget in m.widgets)
                {
                    if (!w.Contains(widget))
                    {
                        w.Add(widget);
                    }
                }
            }
            return w;
        }
    }
}
