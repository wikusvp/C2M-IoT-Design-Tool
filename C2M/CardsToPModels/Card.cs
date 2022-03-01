using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CardsToPModels
{
    public class Card
    {
        /// <summary>
        /// Width of a card
        /// </summary>
        public const int WIDTH = 150;
        /// <summary>
        /// Height of a card
        /// </summary>
        public const int HEIGHT = 190;

        /// <summary>
        /// Height of the label on custom cards
        /// </summary>
        private const int LABEL_HEIGHT = 40;

        private string _cardType;
        private string _name;
        private string _code;
        private Bitmap _image;
        private bool _custom;

        private int _left;
        private int _top;

        /// <summary>
        /// Create a new card with a type, name and image
        /// </summary>
        /// <param name="cardType">Category of the card</param>
        /// <param name="name">Name on card</param>
        /// <param name="code">Code for constructing the image filename</param>
        /// <param name="custom">True if card is a custom card and false if not</param>
        public Card(string cardType, string name, string code, bool custom)
        {
            _cardType = cardType;
            _name = name;
            _code = code;
            ResourceManager resourceManager = Properties.Resources.ResourceManager;
            _image = (Bitmap)resourceManager.GetObject(code);
            _custom = custom;
        }

        /// <summary>
        /// Read only property for the category of the card
        /// </summary>
        public string CardType
        {
            get { return _cardType; }
        }

        /// <summary>
        /// Read and write property for the name on the card
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Read property for the card code
        /// </summary>
        public string Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Read only property for the card image
        /// </summary>
        public Bitmap Image
        {
            get { return _image; }
        }

        /// <summary>
        /// Read and write property for the left position of the top left corner of the card
        /// </summary>
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// Read and write property for the top position of the top left corner of the card
        /// </summary>
        public int Top
        {
            get { return _top; }
            set { _top = value; }
        }

        /// <summary>
        /// Read property for whether the card is custom
        /// </summary>
        public bool Custom
        {
            get { return _custom; }
        }

        /// <summary>
        /// Checks if the current cursor position is on the card
        /// </summary>
        /// <param name="x">Cursor x position</param>
        /// <param name="y">Cursor y position</param>
        /// <returns>True if the cursor is on the current card</returns>
        public bool IsMouseOn(int x, int y)
        {
            if (_left <= x && x < _left + WIDTH
                && _top <= y && y < _top + HEIGHT)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Change left and top positions to current x and y
        /// </summary>
        /// <param name="x">New x position</param>
        /// <param name="y">New y position</param>
        public void MoveTo(int x, int y)
        {
            Left = x;
            Top = y;
        }

        /// <summary>
        /// Draw the card image and if it is a custom card also draw the label
        /// </summary>
        /// <param name="paper">Graphics object for where card is to be drawn</param>
        public void Draw(Graphics paper)
        {
            paper.DrawImage(_image, _left, _top, WIDTH, HEIGHT);

            if (_custom || CardType == "Widget")
            {
                // default color
                Color color = Image.GetPixel(0, 0);

                // draw the rectangle with the chosen color
                paper.FillRectangle(new SolidBrush(color), Left, Top + 40, WIDTH, LABEL_HEIGHT);

                // draw the string using the card name adding spaced between words (since the name is stored in title case)
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                SolidBrush brush;
                if (color.GetBrightness() >= 0.9)
                    brush = new SolidBrush(Color.Black);
                else
                    brush = new SolidBrush(Color.White);

                paper.DrawString(
                    string.Concat(Name.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' '),
                    new Font("Calibri", 10),
                    brush,
                    Left + 75,
                    Top + 60,
                    stringFormat);
            }
        }
    }
}
