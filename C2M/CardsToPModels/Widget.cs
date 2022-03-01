using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    /// <summary>
    /// Individual widget with a name, category, list of I-behaviours and a list of S-behaviours
    /// </summary>
    public class Widget
    {
        /// <summary>
        /// Name of the widget
        /// </summary>
        public string name;

        /// <summary>
        /// Category of the widget: Responder, ActionControl or Display
        /// </summary>
        public string category;

        /// <summary>
        /// List of S_Behaviours
        /// </summary>
        public List<string> S_Behaviours;

        /// <summary>
        /// List of I_Behaviours
        /// </summary>
        public List<string> I_Behaviours;

        /// <summary>
        /// Create a new widget with an empty S_Behaviour and I_Behaviour list
        /// </summary>
        /// <param name="name">Name of widget</param>
        /// <param name="category">Category of widget: Responder, ActionControl or Display</param>
        public Widget(string name, string category)
        {
            this.name = name;
            this.category = category;
            S_Behaviours = new List<string>();
            I_Behaviours = new List<string>();
        }

        /// <summary>
        /// Creates a new widget but also adds an S_Behaviour to the new S_Behaviour list
        /// </summary>
        /// <param name="name">Name of widget</param>
        /// <param name="category">Category of widger: Responder, ActionControl or Display</param>
        /// <param name="sBehaviour">Name of S_Behaviour (exclude the S_)</param>
        public Widget(string name, string category, string sBehaviour)
        {
            this.name = name;
            this.category = category;
            S_Behaviours = new List<string>();
            I_Behaviours = new List<string>();
            AddSBehaviour(sBehaviour);
        }

        /// <summary>
        /// Creates a new widget but also adds a list of S_Behaviours to the new S_Behaviour list
        /// </summary>
        /// <param name="name">Name of widget</param>
        /// <param name="category">Category of widger: Responder, ActionControl or Display</param>
        /// <param name="sBehaviours">List of names of S_Behaviours (exclude the S_)</param>
        public Widget(string name, string category, string[] sBehaviours)
        {
            this.name = name;
            this.category = category;
            S_Behaviours = new List<string>();
            I_Behaviours = new List<string>();
            foreach (string behaviour in sBehaviours)
            {
                AddSBehaviour(behaviour);
            }
            
        }

        /// <summary>
        /// Formats a new S_Behaviour and adds to list
        /// </summary>
        /// <param name="behaviour">Name of S_Behaviour (exclude the S_)</param>
        public void AddSBehaviour(string behaviour)
        {
            if (!S_Behaviours.Contains("S_" + behaviour))
             S_Behaviours.Add("S_" + behaviour);
        }

        /// <summary>
        /// Formats a new I_Behaviour and adds to list
        /// </summary>
        /// <param name="behaviour">Name of I_Behaviour (exclude the I_)</param>
        public void AddIBehaviour(string behaviour)
        {
            if (!I_Behaviours.Contains("I_" + behaviour))
                I_Behaviours.Add("I_" + behaviour);
        }

        /// <summary>
        /// Compares the current widget to another and adds any behaviours from the new
        /// widget that are not in the current widget
        /// </summary>
        /// <param name="newWidget">Widget being combined</param>
        public void AddNewBehaviours(Widget newWidget)
        {
            foreach (string sBeh in newWidget.S_Behaviours)
            {
                if (!S_Behaviours.Contains(sBeh))
                {
                    AddSBehaviour(sBeh.Substring(2));
                }
            }
            foreach (string iBeh in newWidget.I_Behaviours)
            {
                if (!I_Behaviours.Contains(iBeh))
                {
                    AddIBehaviour(iBeh.Substring(2));
                }
            }
        }

        /// <summary>
        /// Returns a list of the names of the I_Behaviours without the I_
        /// </summary>
        /// <returns>List of I_Behaviours without the I_</returns>
        public List<string> GetIBehaviourNames()
        {
            List<string> names = new List<string>();
            foreach (string i in I_Behaviours)
            {
                names.Add(i.Replace("I_", ""));
            }
            return names;
        }
    }
}
