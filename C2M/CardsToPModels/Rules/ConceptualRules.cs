using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    /// <summary>
    /// Derived class from Rules specific for the conceptual cards
    /// </summary>
    public class ConceptualRules : Rules
    {

        /// <summary>
        /// Placeholder PModel to hold widgets before it is split
        /// </summary>
        private PresentationModel placeHolder;

        /// <summary>
        /// Create a new ConceptualRules object containing the methods for creating a model from 
        /// a card design using the conceptual cards
        /// </summary>
        /// <param name="design">Design that the rules will be used on</param>
        public ConceptualRules(Design design) : base(design) { }

        /// <summary>
        /// Creates the PModels and widgets in them from the card design by performing the appropriate rule
        /// for each arrow in the design.
        /// </summary>
        protected override void GeneratePModel()
        {
            placeHolder = new PresentationModel("placeHolder");

            CreatePModels();

            foreach (Arrow a in design.arrows)
            {
                PerformRule(a.From, a.To);
            }


            //split placeHolder PModel into separate PModels
            Split(placeHolder);
        }

        /// <summary>
        /// Create a new PModel for each input and output card in the design
        /// </summary>
        private void CreatePModels()
        {
            foreach (Card card in design.Cards) 
            {
                if (card.CardType == "Input" || card.CardType == "Output")
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
                case "Input":
                    switch (c.CardType)
                    {
                        case "Input":
                            ItoI(startCard, c);
                            break;
                        case "Sensor":
                            ItoSE(startCard, c);
                            break;
                        case "Output":
                            ItoO(startCard, c);
                            break;
                        case "Storage":
                            ItoST(startCard, c);
                            break;
                    }
                    break;
                case "Sensor":
                    switch (c.CardType)
                    {
                        case "Input":
                            SEtoI(startCard, c);
                            break;
                        case "Sensor":
                            SEtoSE(startCard, c);
                            break;
                        case "Output":
                            SEtoO(startCard, c);
                            break;
                        case "Storge":
                            SEtoST(startCard, c);
                            break;
                    }
                    break;
                case "Output":
                    switch (c.CardType)
                    {
                        case "Input":
                            OtoI(startCard, c);
                            break;
                        case "Sensor":
                            OtoSE(startCard, c);
                            break;
                        case "Output":
                            OtoO(startCard, c);
                            break;
                        case "Storage":
                            OtoST(startCard, c);
                            break;
                    }
                    break;
                case "Storage":
                    switch (c.CardType)
                    {
                        case "Input":
                            STtoI(startCard, c);
                            break;
                        case "Sensor":
                            STtoSE(startCard, c);
                            break;
                        case "Output":
                            STtoO(startCard, c);
                            break;
                        case "Storage":
                            STtoST(startCard, c);
                            break;
                    }
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Input to Sensor: A widget is created for both the input and the sensor with a category of Responder for the
        /// sensor and ActionControl for the input. 
        /// The sensor widget has an S-behaviour to detect what is being collected and load the input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sensor"></param>
        private void ItoSE(Card input, Card sensor)
        {
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL));
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, new string[] { "Detect" + sensor.Name, "Load" + input.Name + "Input"}));
        }

        /// <summary>
        /// Input to Output: A widget for both the input and output is created. The input has a category of 
        /// ActionControl and the output has a category of Display. The output widget has an S-behaviour to display 
        /// the input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void ItoO(Card input, Card output)
        {
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL));
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY, "Display" + input.Name + "Input"));
        }

        /// <summary>
        /// Input to Storage: A widget for the input is created with a category of ActionControl and 
        /// an S-behaviour to store the data according to where the storage card specifies.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="storage"></param>
        private void ItoST(Card input, Card storage)
        {
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL, "StoreIn" + storage.Name));
        }

        /// <summary>
        /// Input to Input: A widget for both the inputs is created with a category of ActionControl. 
        /// The second input widget has an S-behaviour to load the first input.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="input2"></param>
        private void ItoI(Card input, Card input2)
        {
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL));
            placeHolder.AddWidget(new Widget(input2.Name + "Input", ACTIONCONTROL, "Load" + input.Name + "Input"));
        }

        /// <summary>
        /// Sensor to Sensor:  A widget is created for both sensors with a category of Responder. 
        /// The sensors both have an S-behaviour to detect what is being collected. The second sensor 
        /// has an S-behaviour to load the data from the previous sensor.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="sensor2"></param>
        private void SEtoSE(Card sensor, Card sensor2)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER,"Detect" + sensor.Name));
            placeHolder.AddWidget(new Widget(sensor2.Name + "Sensor", RESPONDER, new string[] { "Detect" + sensor2.Name, "Load" + sensor.Name + "SensorData" }));
        }

        /// <summary>
        /// Sensor to Output:  A widget is created for both the sensor and output. The sensor is a Responder 
        /// and the output is a Display. The output widget also has an S-behaviour to display the sensor data. 
        /// The sensor widget has an S-behaviour to detect what is being collected.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="output"></param>
        private void SEtoO(Card sensor, Card output)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, "Detect" + sensor.Name));
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY, "Display" + sensor.Name + "SensorData"));
        }

        /// <summary>
        /// Sensor or Storage: A widget for the sensor is created with a category of Responder and an 
        /// S-behaviour to store the data according to where the storage card specifies. The sensor also 
        /// has an S-behaviour to detect what is being collected.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="storage"></param>
        private void SEtoST(Card sensor, Card storage)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, new string[] { "Detect" + sensor.Name, "StoreIn" + storage.Name }));
        }

        /// <summary>
        /// Sensor or Input: A widget is created for both the input and the sensor with a category of Responder for the
        /// sensor and ActionControl for the input. 
        /// The sensor widget has an S-behaviour to detect what is being collected. The input has an S-behaviour 
        /// to load the sensor data.
        /// </summary>
        /// <param name="sensor"></param>
        /// <param name="input"></param>
        private void SEtoI(Card sensor, Card input)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, "Detect" + sensor.Name));
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL, "Load" + sensor.Name + "SensorData"));
        }

        /// <summary>
        /// Output to Output: A widget for both outputs is created with a category of Display. 
        /// The second output has an S-behaviour to display the first output.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="output2"></param>
        private void OtoO(Card output, Card output2)
        {
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY));
            placeHolder.AddWidget(new Widget(output2.Name + "Output", DISPLAY, "Display" + output.Name + "Output"));
        }

        /// <summary>
        /// Output to Sensor:  A widget is created for both the output with a category of Display and the 
        /// sensor with a category of Responder. The sensor widget has an S-behaviour to detect what is being 
        /// collected and load the output.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="sensor"></param>
        private void OtoSE(Card output, Card sensor)
        {
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY));
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, new string[] { "Detect" + sensor.Name, "Load" + output.Name + "Output"}));
        }

        /// <summary>
        /// Output to Storage: A widget for the output is created with a category of Display and an 
        /// S-behaviour to store the data according to where the storage card specifies.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="storage"></param>
        private void OtoST(Card output, Card storage)
        {
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY, "StoreIn" + storage.Name));  
        }

        /// <summary>
        /// Output to Input: A widget is created for both the output with a category of Display and the 
        /// input with a category of ActionControl. The input has an S-behaviour to load the output data.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="input"></param>
        private void OtoI(Card output, Card input)
        {
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY));
            placeHolder.AddWidget(new Widget(input.Name + "Input", ACTIONCONTROL, "Load" + output.Name + "Output"));
        }

        /// <summary>
        /// Storage to Input: A widget for the input card with a category of Responder. It has an S-behaviour to load the 
        /// storage data.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="input"></param>
        private void STtoI(Card storage, Card input)
        {
            placeHolder.AddWidget(new Widget(input.Name + "Input", RESPONDER, "Load" + storage.Name + "Data"));
        }

        /// <summary>
        /// Storage to Output: A widget for the output card with a category of Display. It has an S-behaviour to 
        /// load the storage data.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="output"></param>
        private void STtoO(Card storage, Card output)
        {
            placeHolder.AddWidget(new Widget(output.Name + "Output", DISPLAY, "Load" + storage.Name + "Data"));
        }

        /// <summary>
        /// Storage to Sensor: A widget for the sensor card with a category of Responder. It has an S-behaviour to 
        /// load the storage data and an S-behaviour to detect what is being collected.
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="sensor"></param>
        private void STtoSE(Card storage, Card sensor)
        {
            placeHolder.AddWidget(new Widget(sensor.Name + "Sensor", RESPONDER, new string[] { "Detect" + sensor.Name, "Load" + storage.Name + "Data" }));
        }

        /// <summary>
        /// If there is an input, output or sensor card attached to the first storage 
        /// card an S-behaviour to move data from the first storage to the second storage 
        /// is added to the input, output or sensor widget. If there is an input, output or 
        /// sensor card attached to the second storage card an S-behaviour to load data from 
        /// the first storage is added to the input, output or sensor widget.
        /// </summary>
        /// <param name="storage1"></param>
        /// <param name="storage2"></param>
        private void STtoST(Card storage1, Card storage2)
        {
            List<Card> connectedStorage1 = FindConnectedCards(storage1);
            List<Card> connectedStorage2 = FindConnectedCards(storage2);
            if (connectedStorage1.Count != 0)
            {
                foreach (Card c in connectedStorage1) if (c.CardType != "Storage")
                {
                    placeHolder.AddWidget(new Widget(c.Name + c.CardType, ACTIONCONTROL, "Move" + storage1.Name + "To" + storage2.Name));
                }
            }

            if (connectedStorage2.Count != 0)
            {
                foreach (Card c in connectedStorage2) if (c.CardType != "Storage")
                    {
                    placeHolder.AddWidget(new Widget(c.Name + c.CardType, ACTIONCONTROL, "Load" + storage1.Name + "Data"));
                }
            }
        }
    }

}