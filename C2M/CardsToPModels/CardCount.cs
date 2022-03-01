using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    public class CardCount
    {
        /// <summary>
        /// List of all the cards
        /// </summary>
        private List<string> names = new List<string>();

        /// <summary>
        /// Number of cards corresponding to the names
        /// </summary>
        private List<int> count = new List<int>();

        /// <summary>
        /// Whetyer the card is a custom card or not 
        /// </summary>
        private List<bool> custom = new List<bool>();

        /// <summary>
        /// Create a new counter to keep track of how often cards are used.
        /// </summary>
        public CardCount()
        {
            ReadFile();
        }

        /// <summary>
        /// Adds the card to the list of cards that has been used
        /// </summary>
        /// <param name="card"></param>
        public void CountCard(Card card)
        {
            if (names.Contains(card.Name))
            {
                int pos = names.IndexOf(card.Name);
                count[pos]++;
            }
            else
            {
                names.Add(card.Name);
                count.Add(1);
                custom.Add(card.Custom);
            }
        }

        /// <summary>
        /// Read the CSV file and write the data to lists
        /// </summary>
        private void ReadFile()
        {
            string[] items = new string[3];
            StreamReader inputfile;
            try
            {
                inputfile = File.OpenText("CardCount.csv");
                string line = inputfile.ReadLine();
                while (!inputfile.EndOfStream)
                {
                    line = inputfile.ReadLine();
                    items = line.Split(',');
                    names.Add(items[0].ToString());
                    count.Add(int.Parse(items[1]));
                    custom.Add(Boolean.Parse(items[2]));
                }
                inputfile.Close();
            }
            catch (Exception)
            {
                Debug.Print("CSV file empty");
            }
        }

        /// <summary>
        /// Updates the CSV file.
        /// </summary>
        public void WriteFile()
        {
            StreamWriter file = new StreamWriter("CardCount.csv");
            file.WriteLine("Name,Count,Custom");
            for (int i = 0; i < names.Count; i++)
            {
                file.WriteLine(names[i] + "," + count[i].ToString() + "," + custom[i].ToString());
            }
            file.Flush();
            file.Close();
        }
    }
}
