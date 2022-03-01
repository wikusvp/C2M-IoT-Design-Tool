using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    /// <summary>
    /// Derived class from Rules specific for the IoT cards
    /// </summary>
    public class IoTRules : Rules
    {
        /// <summary>
        /// Placeholder PModel to hold widgets before it is split
        /// </summary>
        private PresentationModel placeHolder;

        /// <summary>
        /// Create a new IoTRules object containing the methods for creating a model from 
        /// a card design using the IoT cards
        /// </summary>
        /// <param name="design">Design that the rules will be used on</param>
        public IoTRules(Design design) : base(design) { }

        /// <summary>
        /// Creates the PModels and widgets in them from the card design by performing the appropriate rule
        /// for each arrow in the design.
        /// </summary>
        protected override void GeneratePModel()
        {
            placeHolder = new PresentationModel("Placeholder");
            CreatePModels();

            foreach (Arrow a in design.arrows)
            {
                PerformRule(a.From, a.To);
            }

            //split placeholder PModel into separate PModels
            Split(placeHolder);
        }

        /// <summary>
        /// Create a new PModel for each service card with at least one arrow towards it. If not such cards can be found
        /// then create a PModel for each thing card with at least one arrow towards it.
        /// </summary>
        private void CreatePModels()
        {
            // checking for valid service card
            foreach (Card card in design.Cards) if (card.CardType == "Service")
            {
                foreach (Arrow arrow in design.arrows)
                {
                    if (arrow.To == card)
                    {
                        if (PModels.Find(p => p.name == card.Name) == null)
                        {
                            PModels.Add(new PresentationModel(card.Name));
                            break;
                        }
                    }
                }
            }

            // if no valid service card is found check for valid thing card
            if (PModels.Count == 0)
            {
                foreach (Card card in design.Cards) if (card.CardType == "Thing")
                {
                    foreach (Arrow arrow in design.arrows)
                    {
                        if (arrow.To == card)
                        {
                            if (PModels.Find(p => p.name == card.Name) == null)
                            { 
                                PModels.Add(new PresentationModel(card.Name));
                            }
                        }
                    }
                }
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
                case "Thing":
                    switch (c.CardType)
                    {
                        case "Thing":
                            TtoT(startCard, c);
                            break;
                        case "Service":
                            TtoS(startCard, c);
                            break;
                        case "Action":
                            TtoA(startCard, c);
                            break;
                        case "Feedback":
                            TtoF(startCard, c);
                            break;
                        case "Sensor":
                            TtoSe(startCard, c);
                            break;
                    }
                    break;
                case "Service":
                    switch (c.CardType)
                    {
                        case "Thing":
                            StoT(startCard, c);
                            break;
                        case "Service":
                            StoS(startCard, c);
                            break;
                        case "Action":
                            StoAorF(startCard, c);
                            break;
                        case "Feedback":
                            StoAorF(startCard, c);
                            break;
                        case "Sensor":
                            StoSe(startCard, c);
                            break;

                    }
                    break;
                case "Action":
                    switch (c.CardType)
                    {
                        case "Thing":
                            AorFtoT(startCard, c);
                            break;
                        case "Service":
                            AorFtoS(startCard, c);
                            break;
                        case "Action":
                            AtoA_FtoF_AtoF_FtoA(startCard, c);
                            break;
                        case "Feedback":
                            AtoA_FtoF_AtoF_FtoA(startCard, c);
                            break;
                        case "Sensor":
                            AorFtoSe(startCard, c);
                            break;
                    }
                    break;
                case "Feedback":
                    switch (c.CardType)
                    {
                        case "Thing":
                            AorFtoT(startCard, c);
                            break;
                        case "Service":
                            AorFtoS(startCard, c);
                            break;
                        case "Action":
                            AtoA_FtoF_AtoF_FtoA(startCard, c);
                            break;
                        case "Feedback":
                            AtoA_FtoF_AtoF_FtoA(startCard, c);
                            break;
                        case "Sensor":
                            AorFtoSe(startCard, c);
                            break;
                    }
                    break;
                case "Sensor":
                    switch (c.CardType)
                    {
                        case "Thing":
                            SetoT(startCard, c);
                            break;
                        case "Service":
                            SetoS(startCard, c);
                            break;
                        case "Action":
                            SetoA(startCard, c);
                            break;
                        case "Feedback":
                            SetoF(startCard, c);
                            break;
                        case "Sensor":
                            SetoSe(startCard, c);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Thing to Service: A widget is added for the thing card and the service card. They are both Responders. The
        /// service widget has two S-behaviours, one to collect the service data and
        /// one to load the thing data.The thing has an S-behaviour to collect the
        /// thing data.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="service"></param>
        private void TtoS(Card thing, Card service)
        {
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER, "Collect" + thing.Name + "Data"));
            placeHolder.AddWidget(new Widget(service.Name, RESPONDER, new string[] { "Collect" + service.Name + "Data", "Load" + thing.Name + "Data" }));
        }

        /// <summary>
        /// Thing to Thing: A widget is added for both thing cards. Both things are Responders. The second thing widget
        /// has an S-behaviour to collect data from the first thing.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="thing2"></param>
        private void TtoT(Card thing, Card thing2)
        {
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER));
            placeHolder.AddWidget(new Widget(thing2.Name, RESPONDER, "Collect" + thing.Name + "Data"));
        }

        /// <summary>
        /// Thing to Action: A new widget for the thing card is added with a name correcponding to the thing
        /// and followed with "Interaction" and aa category of ActionControl 
        /// An S-behaviour for the action is also added.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="action"></param>
        private void TtoA(Card thing, Card action)
        {
            placeHolder.AddWidget(new Widget(thing.Name + "Interaction", ACTIONCONTROL, action.Name));
        }

        /// <summary>
        /// Thing to Feedback: A widget for the thing card is added with a category of Responder 
        /// and an S-behaviour corresponding with the action or feedback.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="feedback"></param>
        private void TtoF(Card thing, Card feedback)
        {
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER, feedback.Name));
        }

        /// <summary>
        /// Service to Thing: A widget is created for both the service and thing cards. The service is
        /// an ActionControl and the thing is a Responder.The service has an Sbehaviour to collect the service data and thing widget has an S-behaviour
        /// to load the service data.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="thing"></param>
        private void StoT(Card service, Card thing)
        {
            placeHolder.AddWidget(new Widget(service.Name, ACTIONCONTROL, "Collect" + service.Name + "Data"));
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER, "Collect" + service.Name + "Data"));
        }

        /// <summary>
        /// Service to Service: A widget for the second service is created with a category of Responder.
        /// It has two S-behaviours, one to load the data of the first service and one
        /// to collect the data for the second service.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="service2"></param>
        private void StoS(Card service, Card service2)
        {
            placeHolder.AddWidget(new Widget(service2.Name, RESPONDER, new string[] { "Collect" + service2.Name + "Data", "Load" + service.Name + "Data" }));
        }

        /// <summary>
        /// Service to Action or Feedback: A widget is created for the service card with a category of ActionControl. 
        /// It has two S-behaviours, one to load the service data and another corresponding with the action or 
        /// feedback.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="actionORfeedback"></param>
        private void StoAorF(Card service, Card actionORfeedback)
        {
            placeHolder.AddWidget(new Widget(service.Name, ACTIONCONTROL, new string[] { "Collect" + service.Name + "Data", actionORfeedback.Name }));
        }

        /// <summary>
        /// Action or Feedback to Thing: A widget is added for the service with a category of Responder and an Sbehaviour to load the service data. It also has an additional S-behaviour
        /// corresponding to the action or feedback
        /// </summary>
        /// <param name="actionORfeedback"></param>
        /// <param name="thing"></param>
        private void AorFtoT(Card actionORfeedback, Card thing)
        {
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER, actionORfeedback.Name));
        }

        /// <summary>
        /// Action or Feedback to Service: A widget is created for the service card with a category of ActionControl. 
        /// It has two S-behaviours, one to collect the service data and another
        ///corresponding with the action or feedback.
        /// </summary>
        /// <param name="actionORfeedback"></param>
        /// <param name="service"></param>
        private void AorFtoS(Card actionORfeedback, Card service)
        {
            placeHolder.AddWidget(new Widget(service.Name, RESPONDER, new string[] { "Collect" + service.Name + "Data", actionORfeedback.Name }));
        }

        /// <summary>
        /// Action to Feedback or Action, Feedback to Feedback or Action: If there is a service card 
        /// connected to the first action or feedback card then a widget for the service is created with 
        /// two S-behaviours, to load the service data and corresponding with the second action or feedback 
        /// card. If there is a thing card connected to the first action or feedback card then a widget is 
        /// created for the thing and an S-behaviour is added for the second action or feedback card.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void AtoA_FtoF_AtoF_FtoA(Card start, Card end)
        {
            List<Card> connectedService = FindConnectedCards(start).FindAll(c => c.CardType == "Service");
            foreach (Card c in connectedService)
            {
                StoAorF(c, end);
            }

            List<Card> connectedThing = FindConnectedCards(start).FindAll(c => c.CardType == "Thing");
            foreach (Card c in connectedThing)
            {
                if (c.CardType == "Action")
                    TtoA(c, end);
                else
                    TtoF(c, end);
            }
        }

        /// <summary>
        /// A widget is added for the sensor card and the service card. They are both Responders. 
        /// The service widget has two S-behaviours, one to collect the service data and one to load the sensor data. 
        /// The sensor has an S-behaviour to collect the sensor data.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="service"></param>
        private void SetoS(Card sensor, Card service)
        {
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER, "Collect" + sensor.Name + "Data"));
            placeHolder.AddWidget(new Widget(service.Name, RESPONDER, new string[] { "Collect" + service.Name + "Data", "Load" + sensor.Name + "Data" }));
        }

        /// <summary>
        /// Sensor to Sensor: A widget is added for both sensor cards. Both sensors are Responders. The second sensor widget
        /// has an S-behaviour to collect data from the first sensor.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sensor2"></param>
        private void SetoSe(Card sensor, Card sensor2)
        {
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER));
            placeHolder.AddWidget(new Widget(sensor2.Name, RESPONDER, "Collect" + sensor.Name + "Data"));
        }

        /// <summary>
        /// Sensor to Thing: A widget is added for both the sensor and the thing cards. Both are Responders. The thing widget
        /// has an S-behaviour to collect data from the sensor.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="thing"></param>
        private void SetoT(Card sensor, Card thing)
        {
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER));
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER, "Collect" + sensor.Name + "Data"));
        }

        /// <summary>
        /// Sensor to Action: A new widget for the sensor card is added with a name correcponding to the sensor
        /// and followed with "Interaction" and aa category of ActionControl 
        /// An S-behaviour for the action is also added.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="action"></param>
        private void SetoA(Card sensor, Card action)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Interaction", ACTIONCONTROL, action.Name));
        }

        /// <summary>
        /// Sensor to Feedback: A widget for the sensor card is added with a category of Responder 
        /// and an S-behaviour corresponding with the action or feedback.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="feedback"></param>
        private void SetoF(Card sensor, Card feedback)
        {
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER, feedback.Name));
        }

        /// <summary>
        /// Thing to Sensor: A widget is added for both the thing and the sensor cards. Both are Responders. The sensor widget
        /// has an S-behaviour to collect data from the thing.
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="sensor"></param>
        private void TtoSe(Card thing, Card sensor)
        {
            placeHolder.AddWidget(new Widget(thing.Name, RESPONDER));
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER, "Collect" + thing.Name + "Data"));
        }

        /// <summary>
        /// Service to Sensor: A widget is created for both the service and sensor cards. The service is
        /// an ActionControl and the thing is a Responder.The service has an Sbehaviour to collect the service data and sensor widget has an S-behaviour
        /// to load the service data.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="sensor"></param>
        private void StoSe(Card service, Card sensor)
        {
            placeHolder.AddWidget(new Widget(service.Name, ACTIONCONTROL, "Collect" + service.Name + "Data"));
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER, "Collect" + service.Name + "Data"));
        }

        /// <summary>
        /// Action or Feedback to Sensor: A widget is added for the service with a category of Responder and an S-behaviour to load the service data. It also has an additional S-behaviour
        /// corresponding to the action or feedback
        /// </summary>
        /// <param name="actionORfeedback"></param>
        /// <param name="sensor"></param>
        private void AorFtoSe(Card actionORfeedback, Card sensor)
        {
            placeHolder.AddWidget(new Widget(sensor.Name, RESPONDER, actionORfeedback.Name));
        }
    }
}