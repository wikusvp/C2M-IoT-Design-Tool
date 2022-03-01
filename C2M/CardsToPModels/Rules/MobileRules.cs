using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    /// <summary>
    /// Derived class from Rules specific for the mobile application cards
    /// </summary>
    public class MobileRules : Rules
    {
        /// <summary>
        /// Create a new MobileRules object containing the methods for creating a model from 
        /// a card design using the mobile application cards
        /// </summary>
        /// <param name="design">Design that the rules will be used on</param>
        public MobileRules(Design design) : base(design) { }

        /// <summary>
        /// A list of the codes corresponding to the widget cards that are not display widgets:
        /// TextInput, Checkbox and RadioButton
        /// </summary>
        private List<string> notDisplayWidgets = new List<string>() { "W3", "W4", "W9" };

        /// <summary>
        /// Creates the PModels and widgets in them from the card design by performing the appropriate rule
        /// for each arrow in the design.
        /// </summary>
        protected override void GeneratePModel()
        {
            CreatePModels();
            foreach (Arrow a in design.arrows)
            {
                PerformRule(a.From, a.To);
            }
        }

        /// <summary>
        /// Create a new PModel for each view card in the design
        /// </summary>
        private void CreatePModels()
        {
            foreach (Card card in design.Cards) if (card.CardType == "View")
            {
                PModels.Add(new PresentationModel(card.Name));
            }
        }

        /// <summary>
        /// Given two cards this uses a switch statament to decide which rule to call
        /// </summary>
        /// <param name="startCard">Card the arrow starts from</param>
        /// <param name="c">Card the arrow is going to</param>
        protected override void PerformRule(Card startCard, Card c)
        {
            switch (startCard.CardType)
            {
                case "Widget":
                    switch (c.CardType)
                    {
                        case "Widget":
                            MessageBox.Show("Invalid Connection between " + startCard.Name + " and " + c.Name);
                            break;
                        case "View":
                            WtoV(startCard, c);
                            break;
                        case "Gesture":
                            WtoGorR(startCard, c);
                            break;
                        case "Response":
                            WtoGorR(startCard, c);
                            break;
                        case "Service":
                            WtoS(startCard, c);
                            break;
                    }
                    break;
                case "View":
                    switch (c.CardType)
                    {
                        case "Widget":
                            VtoW(startCard, c);
                            break;
                        case "View":
                            VtoV(startCard, c);
                            break;
                        case "Gesture":
                            VtoGorR(startCard, c);
                            break;
                        case "Response":
                            VtoGorR(startCard, c);
                            break;
                        case "Service":
                            VtoS(startCard, c);
                            break;
                    }
                    break;
                case "Gesture":
                    switch (c.CardType)
                    {
                        case "Widget":
                            GorRtoW(startCard, c);
                            break;
                        case "View":
                            GorRtoV(startCard, c);
                            break;
                        case "Gesture":
                            GtoG_RtoR_GtoR_RtoG(startCard, c);
                            break;
                        case "Response":
                            GtoG_RtoR_GtoR_RtoG(startCard, c);
                            break;
                        case "Service":
                            GorRtoS(startCard, c);
                            break;
                    }
                    break;
                case "Response":
                    switch (c.CardType)
                    {
                        case "Widget":
                            GorRtoW(startCard, c);
                            break;
                        case "View":
                            GorRtoV(startCard, c);
                            break;
                        case "Gesture":
                            GtoG_RtoR_GtoR_RtoG(startCard, c);
                            break;
                        case "Response":
                            GtoG_RtoR_GtoR_RtoG(startCard, c);
                            break;
                        case "Service":
                            GorRtoS(startCard, c);
                            break;
                    }
                    break;
                case "Service":
                    switch (c.CardType)
                    {
                        case "Widget":
                            StoW(startCard, c);
                            break;
                        case "View":
                            StoV(startCard, c);
                            break;
                        case "Gesture":
                            StoGorR(startCard, c);
                            break;
                        case "Response":
                            StoGorR(startCard, c);
                            break;
                        case "Service":
                            StoS(startCard, c);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Adds the widget to the specified PModel
        /// </summary>
        /// <param name="w">Widget to be added</param>
        /// <param name="pmName">Name of the PModel</param>
        private void AddToPM(Widget w, string pmName)
        {
            PresentationModel pm = PModels.Find(m => m.name == pmName);
            pm.AddWidget(w);
        }


        /// <summary>
        /// Widget to View: The widget card becomes a widget in the PModel of the view card that it is coming from. 
        /// It has a category of ActionControl, an I-behaviour to navigate to the current view card.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="view"></param>
        private void WtoV(Card widget, Card view)
        {
            Widget w = new Widget(widget.Name, ACTIONCONTROL);
            w.AddIBehaviour(view.Name);
            foreach (Card c in FindConnectedCardsFrom(widget)) if (c.CardType == "View")
            {
                AddToPM(w, c.Name);  
            }
        }

        /// <summary>
        /// Widget to Gesture or Response: A widget for the widget card is added with a 
        /// category of ActionControl and an S-behaviour corresponding with the gesture or response.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="gestureORresponse"></param>
        private void WtoGorR(Card widget, Card gestureORresponse)
        {
            Widget w = new Widget(widget.Name, ACTIONCONTROL);
            w.AddSBehaviour(gestureORresponse.Name);

            foreach (Card c in FindConnectedCardsFrom(widget)) if (c.CardType == "View")
            {
                AddToPM(w, c.Name);
            }
        }

        /// <summary>
        /// View to Widget: A widget is created for the widget card in the view card's PModel. 
        /// The widget is an ActionControl if there is another arrow leaving the card
        /// and it is a  text input, checkbox or radio button card, otherwise it is a Display.
        /// contained in. 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="widget"></param>
        private void VtoW(Card view, Card widget)
        {
            Widget w;
            if (FindConnectedCardsTo(widget).Count != 0 || notDisplayWidgets.Contains(widget.Code))
                w = new Widget(widget.Name, ACTIONCONTROL);
            else
                w = new Widget(widget.Name, DISPLAY);
            AddToPM(w, view.Name);
        }

        /// <summary>
        /// View to View: A widget is added into each with an I-behaviour to navigate between the views. 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="view2"></param>
        private void VtoV(Card view, Card view2)
        {
            Widget w = new Widget("Open" + view2.Name, ACTIONCONTROL);
            w.AddIBehaviour(view2.Name);
            AddToPM(w, view.Name);

            w = new Widget("Open" + view.Name, ACTIONCONTROL);
            w.AddIBehaviour(view.Name);
            AddToPM(w, view2.Name);
        }

        /// <summary>
        /// View to Gesture or Response: Using the gesture or response card as a start, a path is tracked, 
        /// following the direction of the arrows, to find the destination view card. A widget is created 
        /// for this destination view card with a category of ActionControl in the current view card's PModel. 
        /// It has an S-behaviour corresponding with the gesture or response. If there are no destination view 
        /// cards then the widget created uses the name of the current view card instead.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="gestureORresponse"></param>
        private void VtoGorR(Card view, Card gestureORresponse)
        {
            List<Card> destinationCards = FindViewORWidgetDestinationCard(gestureORresponse);
            if (destinationCards.Count == 0)
            {
                Widget w = new Widget(view.Name, RESPONDER, gestureORresponse.Name);
                AddToPM(w, view.Name);
            }
            else
            {
                foreach (Card c in destinationCards)
                {
                    Widget w = new Widget(c.Name, RESPONDER, gestureORresponse.Name);
                    AddToPM(w, view.Name);
                }
            }
        }

        /// <summary>
        /// Gesture or Response to Widget: A widget is added for the widget card with a category of 
        /// Responder and an S-behaviour corresponding with the name of the gesture or response. The 
        /// widget is added to the PModel of the view card from which the gesture or response is connected.
        /// </summary>
        /// <param name="gestureORresponse"></param>
        /// <param name="widget"></param>
        private void GorRtoW(Card gestureORresponse, Card widget)
        {
            Widget w = new Widget(widget.Name, RESPONDER, gestureORresponse.Name);

            foreach (Card c in FindViewORWidgetOriginCard(gestureORresponse)) if (c.CardType == "View")
            {
                AddToPM(w, c.Name);
            }
        }

        /// <summary>
        /// Gesture or Response to View: A widget is added for the view or widget card connected 
        /// before the gesture or response with a category of ActionContol and I-behaviour to navigate to the view.
        /// </summary>
        /// <param name="gestureORresponse"></param>
        /// <param name="view"></param>
        private void GorRtoV(Card gestureORresponse, Card view)
        {
            foreach (Card c in FindViewORWidgetOriginCard(gestureORresponse))
            {
                Widget w = new Widget("Open" + view.Name, ACTIONCONTROL);
                w.AddIBehaviour(view.Name);
                w.AddSBehaviour(gestureORresponse.Name);
                AddToPM(w, c.Name);
            }
        }

        /// <summary>
        /// Gesture to Gesture or Response, Response to Gesture or Response: If there is a view or widget card 
        /// connected to the first gesture or response card an S-behaviour is added for the second gesture or 
        /// response card.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void GtoG_RtoR_GtoR_RtoG(Card start, Card end)
        {
            List<Card> connectedService = FindConnectedCards(start).FindAll(c => c.CardType == "View");
            foreach (Card c in connectedService)
            {
                VtoGorR(c, end);
            }

            List<Card> connectedThing = FindConnectedCards(start).FindAll(c => c.CardType == "Widget");
            foreach (Card c in connectedThing)
            {
                WtoGorR(c, end);
            }

        }

        /// <summary>
        /// Service to View: A widget for the view card is added to the view card’s PModel. 
        /// The widget has an S-behaviour to collect data from the service and has a category of ActionControl.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="view"></param>
        private void StoV(Card service, Card view)
        {
            Widget w = new Widget(view.Name, RESPONDER, "Collect" + service.Name + "Data");
            AddToPM(w, view.Name);
        }

        /// <summary>
        /// Service to Widget: A widget for the widget card is added to the view card’s PModel which it is 
        /// attached to. The widget has an S-behaviour to collect data from the service and has a category of 
        /// ActionControl.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="widget"></param>
        private void StoW(Card service, Card widget)
        {
            Widget w = new Widget(widget.Name, RESPONDER, "Collect" + service.Name + "Data");
            foreach (Card c in FindViewORWidgetOriginCard(widget))
            {
                AddToPM(w, c.Name);
            }
        }

        /// <summary>
        /// View to Service: A widget for the view card is added to the view card’s PModel. The widget has an S-behaviour to 
        /// send data to the service and has a category of Responder.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="service"></param>
        private void VtoS(Card view, Card service)
        {
            Widget w = new Widget(view.Name, ACTIONCONTROL, "SendTo" + service.Name);
            AddToPM(w, view.Name);
        }

        /// <summary>
        /// Widget to Service: A widget for the widget card is added to the view card’s PModel 
        /// which it is attached to. The widget has an S-behaviour to send data to the service and has a 
        /// category of ActionControl.
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="service"></param>
        private void WtoS(Card widget, Card service)
        {
            Widget w = new Widget(widget.Name, ACTIONCONTROL, "SendTo" + service.Name);
            foreach (Card c in FindViewORWidgetOriginCard(widget))
            {
                AddToPM(w, c.Name);
            }
        }

        /// <summary>
        /// If there is a view or widget card with an arrow coming from the gesture or response
        /// card an S-behaviour is added to the view or widget to load the data from the service.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="gestureOrResponse"></param>
        private void StoGorR(Card service, Card gestureOrResponse)
        {
            List<Card> cards = FindViewORWidgetDestinationCard(gestureOrResponse);
            cards.AddRange(FindViewORWidgetOriginCard(gestureOrResponse));
            foreach (Card c in cards)
            {
                Widget w = new Widget(c.Name, ACTIONCONTROL, new string[] { "Collect" + service.Name + "Data", gestureOrResponse.Name });
                if (c.CardType == "View")
                    AddToPM(w, c.Name);
                else
                {
                    foreach (Card c2 in FindConnectedCards(c)) if (c2.CardType == "View")
                    {
                        AddToPM(w, c2.Name);
                    }
                }
            }
        }

        /// <summary>
        /// If there is a view or widget card with an arrow towards the gesture or response card
        /// an S-behaviour is added to the view or widget to send the data to the service.
        /// </summary>
        /// <param name="gestureOrResponse"></param>
        /// <param name="service"></param>
        private void GorRtoS(Card gestureOrResponse, Card service)
        {
            List<Card> cards = FindViewORWidgetDestinationCard(gestureOrResponse);
            cards.AddRange(FindViewORWidgetOriginCard(gestureOrResponse));
            if (cards.Count != 0)
            {
                foreach (Card c in cards)
                {
                    Widget w = new Widget(c.Name, ACTIONCONTROL, new string[] { "SendTo" + service.Name, gestureOrResponse.Name });
                    if (c.CardType == "View")
                        AddToPM(w, c.Name);
                    else
                    {
                        foreach (Card c2 in FindConnectedCards(c)) if (c2.CardType == "View")
                            {
                                AddToPM(w, c2.Name);
                            }
                    }
                }
            }
            
        }

        /// <summary>
        /// Any view or widget cards which are sending data or loading data from one of the
        /// services will now send/load data from both services.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="service2"></param>
        private void StoS(Card service, Card service2)
        {
            List<Card> loadingService = FindViewORWidgetDestinationCard(service);
            loadingService.AddRange(FindViewORWidgetDestinationCard(service2));

            if (loadingService.Count != 0)
            {
                foreach (Card c in loadingService)
                {
                    Widget w = new Widget(c.Name, ACTIONCONTROL, new string[] { "Collect" + service.Name + "Data", "Collect" + service2.Name + "Data" });
                    if (c.CardType == "View")
                        AddToPM(w, c.Name);
                    else
                    {
                        foreach (Card c2 in FindConnectedCards(c)) if (c2.CardType == "View")
                            {
                                AddToPM(w, c2.Name);
                            }
                    }
                }
            }

            List<Card> sendingToService = FindViewORWidgetOriginCard(service);
            sendingToService.AddRange(FindViewORWidgetOriginCard(service2));

            if (sendingToService.Count != 0)
            {
                foreach (Card c in sendingToService)
                {
                    Widget w = new Widget(c.Name, ACTIONCONTROL, new string[] { "SendTo" + service.Name, "SendTo" + service2.Name });
                    if (c.CardType == "View")
                        AddToPM(w, c.Name);
                    else
                    {
                        foreach (Card c2 in FindConnectedCards(c)) if (c2.CardType == "View")
                            {
                                AddToPM(w, c2.Name);
                            }
                    }
                }
            }
        }

        /// <summary>
        /// From the card given, a path is traced back to find which view or widget cards it came from
        /// </summary>
        /// <param name="card">Starting card</param>
        /// <returns>A list of view and widget cards that the given card originates from</returns>
        private List<Card> FindViewORWidgetOriginCard(Card card)
        {
            List<Card> originCards = new List<Card>();
            List<Card> toCheck = new List<Card>();
            foreach (Card c in FindConnectedCardsFrom(card))
            {
                if (c.CardType == "View" || c.CardType == "Widget")
                    originCards.Add(c);
                else
                    toCheck.Add(c);
            }
            if (originCards.Count == 0)
            {
                foreach (Card c in toCheck)
                {
                    originCards.AddRange(FindViewORWidgetOriginCard(c));
                }
            }
            return originCards;

        }

        /// <summary>
        /// From the card given, a path is traced back to find which view or widget cards it is going to
        /// </summary>
        /// <param name="card">Starting card</param>
        /// <returns>A list of view and widget cards that the given card is going to</returns>
        private List<Card> FindViewORWidgetDestinationCard(Card card)
        {
            List<Card> destinationCards = new List<Card>();
            List<Card> toCheck = new List<Card>();
            foreach (Card c in FindConnectedCardsTo(card))
            {
                if (c.CardType == "View" || c.CardType == "Widget")
                    destinationCards.Add(c);
                else
                    toCheck.Add(c);
            }
            if (destinationCards.Count == 0)
            {
                foreach (Card c in toCheck)
                {
                    destinationCards.AddRange(FindViewORWidgetDestinationCard(c));
                }
            }
            return destinationCards;

        }
    }
}
