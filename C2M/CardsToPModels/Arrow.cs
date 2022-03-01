using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CardsToPModels
{
    /// <summary>
    /// Class to describe the connection between two cards.
    /// </summary>
    public class Arrow
    {
        private Card _from;
        private Card _to;

        /// <summary>
        /// Point where the arrow joins to the from card
        /// </summary>
        private Point _fromPoint;

        /// <summary>
        /// Point where the arrow joins to the to card
        /// </summary>
        private Point _toPoint;

        /// <summary>
        /// Create a new arrow which goes between two cards. This also creates two points to give the x,y position
        /// of both ends of the arrow.
        /// </summary>
        /// <param name="from">Card from</param>
        /// <param name="to">Card to</param>
        public Arrow(Card from, Card to)
        {
            _from = from;
            _to = to;
            _fromPoint = new Point();
            _toPoint = new Point();
        }

        /// <summary>
        /// Card the arrow is coming from
        /// </summary>
        public Card From
        {
            get { return _from; }
        }

        /// <summary>
        /// Card the arrow is going to
        /// </summary>
        public Card To
        {
            get { return _to; }
        }

        /// <summary>
        /// Check if the mouse is on the arrow
        /// </summary>
        /// <param name="x">Cursor x position</param>
        /// <param name="y">Cursor y position</param>
        /// <returns>True if the cursor is on the current arrow</returns>
        public bool IsMouseOn(int x, int y)
        {
            // calculate arrow line equation
            decimal rise = _toPoint.Y - _fromPoint.Y;
            decimal run = _toPoint.X - _fromPoint.X;
            if (run == 0)
            {
                run = _toPoint.X+1 - _fromPoint.X-1;
            }
            decimal gradient = rise / run;

            // find the difference between the calculated y coordinate and the clicked y coordinate
            decimal difference = y - (gradient * (x - _fromPoint.X) + _fromPoint.Y);

            // a range of +-5 is given so the cursor doesn't have to be directly on the arrow
            if (difference >= -5 && difference <= 5){
                Debug.Print("Arrow selected = true");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the current arrow contains a specific card
        /// </summary>
        /// <param name="card">Card to check</param>
        /// <returns>True if the arrow contains that card</returns>
        public bool ArrowContains(Card card)
        {
            if (From == card || To == card)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draw the arrow. 
        /// This will move depending on what region (there are 9) one card is 
        /// (the to card) in relation to the other (the from card)
        /// </summary>
        /// <param name="paper">Graphics object of where the arrow will be drawn</param>
        public void Draw(Graphics paper)
        {
            int region;
            // find the center of the to card
            (int, int) toCenter = (To.Left + Card.WIDTH / 2, To.Top + Card.HEIGHT / 2);

            // Determine the region the to card is in in relation to the from card
            if (toCenter.Item1 < From.Left)
            {
                if (toCenter.Item2 < From.Top)
                {
                    region = 1;
                }else if (toCenter.Item2 > From.Top + Card.HEIGHT)
                {
                    region = 7;
                }else
                {
                    region = 4;
                }

            }else if (toCenter.Item1 > From.Left + Card.WIDTH)
            {
                if (toCenter.Item2 < From.Top)
                {
                    region = 3;
                }
                else if (toCenter.Item2 > From.Top + Card.HEIGHT)
                {
                    region = 9;
                }
                else
                {
                    region = 6;
                }

            }
            else
            {
                if (toCenter.Item2 < From.Top)
                {
                    region = 2;
                }
                else if (toCenter.Item2 > From.Top + Card.HEIGHT)
                {
                    region = 8;
                }
                else
                {
                    region = 5;
                }

            }

            // add an arrow head to the line
            AdjustableArrowCap arrow = new AdjustableArrowCap(5, 5);
            Pen arrowPen = new Pen(Color.Black, 2);
            arrowPen.CustomEndCap = arrow;

            // draw the arrow between the appropriate two points on the cards depending on the region and set the point fields of the arrow
            switch (region)
            {
                case 1:
                    paper.DrawLine(arrowPen, From.Left, From.Top, To.Left + Card.WIDTH, To.Top + Card.HEIGHT);
                    _fromPoint.X = From.Left;
                    _fromPoint.Y = From.Top;
                    _toPoint.X = To.Left + Card.WIDTH;
                    _toPoint.Y = To.Top + Card.HEIGHT;
                    break;
                case 2:
                    paper.DrawLine(arrowPen, From.Left + Card.WIDTH/2, From.Top, To.Left + Card.WIDTH/2, To.Top + Card.HEIGHT);
                    _fromPoint.X = From.Left + Card.WIDTH / 2;
                    _fromPoint.Y = From.Top;
                    _toPoint.X = To.Left + Card.WIDTH / 2;
                    _toPoint.Y = To.Top + Card.HEIGHT;
                    break;
                case 3:
                    paper.DrawLine(arrowPen, From.Left + Card.WIDTH, From.Top, To.Left, To.Top + Card.HEIGHT);
                    _fromPoint.X = From.Left + Card.WIDTH;
                    _fromPoint.Y = From.Top;
                    _toPoint.X = To.Left;
                    _toPoint.Y = To.Top + Card.HEIGHT;
                    break;
                case 4:
                    paper.DrawLine(arrowPen, From.Left, From.Top + Card.HEIGHT/2, To.Left + Card.WIDTH, To.Top + Card.HEIGHT/2);
                    _fromPoint.X = From.Left;
                    _fromPoint.Y = From.Top + Card.HEIGHT / 2;
                    _toPoint.X = To.Left + Card.WIDTH;
                    _toPoint.Y = To.Top + Card.HEIGHT / 2;
                    break;
                case 5:
                    paper.DrawLine(arrowPen, From.Left, From.Top, To.Left, To.Top);
                    _fromPoint.X = From.Left;
                    _fromPoint.Y = From.Top;
                    _toPoint.X = To.Left;
                    _toPoint.Y = To.Top;
                    break;
                case 6:
                    paper.DrawLine(arrowPen, From.Left + Card.WIDTH, From.Top + Card.HEIGHT/2, To.Left, To.Top + Card.HEIGHT/2);
                    _fromPoint.X = From.Left + Card.WIDTH;
                    _fromPoint.Y = From.Top + Card.HEIGHT / 2;
                    _toPoint.X = To.Left;
                    _toPoint.Y = To.Top + Card.HEIGHT / 2;
                    break;
                case 7:
                    paper.DrawLine(arrowPen, From.Left, From.Top + Card.HEIGHT, To.Left + Card.WIDTH, To.Top);
                    _fromPoint.X = From.Left;
                    _fromPoint.Y = From.Top + Card.HEIGHT;
                    _toPoint.X = To.Left + Card.WIDTH;
                    _toPoint.Y = To.Top;
                    break;
                case 8:
                    paper.DrawLine(arrowPen, From.Left + Card.WIDTH/2, From.Top + Card.HEIGHT, To.Left + Card.WIDTH/2, To.Top);
                    _fromPoint.X = From.Left + Card.WIDTH / 2;
                    _fromPoint.Y = From.Top + Card.HEIGHT;
                    _toPoint.X = To.Left + Card.WIDTH / 2;
                    _toPoint.Y = To.Top;
                    break;
                case 9:
                    paper.DrawLine(arrowPen, From.Left + Card.WIDTH, From.Top + Card.HEIGHT, To.Left, To.Top);
                    _fromPoint.X = From.Left + Card.WIDTH;
                    _fromPoint.Y = From.Top + Card.HEIGHT;
                    _toPoint.X = To.Left;
                    _toPoint.Y = To.Top;
                    break;
            }
        }

      

    }
}
