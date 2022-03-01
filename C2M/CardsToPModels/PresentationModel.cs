using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    /// <summary>
    /// Name and list of widgets in a PModel
    /// </summary>
    public class PresentationModel
    {
        /// <summary>
        /// List of widgets in the presentation model
        /// </summary>
        public List<Widget> widgets;

        /// <summary>
        /// Name of the presentation model
        /// </summary>
        public string name;


        /// <summary>
        /// Create a new presentation model with an empty list of widgets
        /// </summary>
        /// <param name="name">Name of the Presentation Model</param>
        public PresentationModel(string name)
        {
            this.name = name;
            widgets = new List<Widget>();
        }

        /// <summary>
        /// Adds a widget to the PModel if it does not already exist. If it already exists, any new behaviours in the given widget
        /// are added to the existing widget.
        /// </summary>
        /// <param name="widget">Widget to be added</param>
        public void AddWidget(Widget widget)
        {
            Widget widgetSearch;
            widgetSearch = widgets.Find(w => w.name == widget.name);
            if (widgetSearch == null)
                widgets.Add(widget);
            else
            {
                widgetSearch.AddNewBehaviours(widget);
            }
        }

        /// <summary>
        /// Generate the Presentation Model XML
        /// </summary>
        /// <returns>Presentation Model XML</returns>
        public string GeneratePM()
        {
            string xml = "<cpmodel>\n  <cpname>" + name + "</cpname>\n";
            foreach (Widget w in widgets)
            {
                xml += "<widget>\n  <name>" + w.name + "</name>\n  <cat>" + w.category + "</cat>\n";
                foreach (string s in w.S_Behaviours)
                {
                    xml += "<beh>" + s + "</beh>\n";
                }
                foreach (string i in w.I_Behaviours)
                {
                    xml += "<beh>" + i + "</beh>\n";
                }
                xml += "</widget>\n";
            }
            xml += "</cpmodel>\n\n";
            return xml;
        }
    }
}
