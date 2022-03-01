using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsToPModels
{
    public class UndoRedo
    {
        /// <summary>
        /// Creates a stack to store the states of the Design that have already been passed.
        /// </summary>
        private Stack<Design> undoStack = new Stack<Design>();

        /// <summary>
        /// Creates a stack to store the state of the Design that has been undone.
        /// </summary>
        private Stack<Design> redoStack = new Stack<Design>();

        /// <summary>
        /// the current active state of the program
        /// </summary>
        private Design currentState;

        int counter = 0;

        public UndoRedo()
        {
            undoStack = new Stack<Design>();
            redoStack = new Stack<Design>();
        }

        /// <summary>
        /// Clear the contents of the stack
        /// </summary>
        public void clear()
        {
            undoStack.Clear();
            redoStack.Clear();
        }

        /// <summary>
        /// Save the state of the program to the stack
        /// </summary>
        /// <param name="item"></param>
        public void stateChanged(Design item)
        {
            undoStack.Push(currentState);
            currentState = item;
            redoStack.Clear();
        }

        /// <summary>
        /// Undo the last action that was taken
        /// </summary>
        /// <returns></returns>
        public Design undo()
        {
            if (undoStack.Count == 0) return currentState;
            redoStack.Push(currentState);
            currentState = undoStack.Pop();
            return currentState;
        }

        /// <summary>
        /// Redo the undo action
        /// </summary>
        /// <returns></returns>
        public Design redo()
        {
            if (redoStack.Count == 0) return currentState;
            undoStack.Push(currentState);
            currentState = redoStack.Pop();
            return currentState;
        }

        public bool canUndo()
        {
            return undoStack.Count() > 1;
        }

        public bool canRedo()
        {
            return redoStack.Count() > 0;
        }

        /// <summary>
        /// Returns a list of all the items in the undo stack
        /// </summary>
        /// <returns></returns>
        public List<Design> lstUndo()
        {
            List<Design> u = new List<Design>();
            foreach (Design item in undoStack)
            {
                u.Add(item);
            }
            return u;
        }

        /// <summary>
        /// Returns a list of all the items in the redo Stack
        /// </summary>
        /// <returns></returns>
        public List<Design> lstRedo()
        {
            List<Design> r = new List<Design>();
            foreach (Design item in redoStack)
            {
                r.Add(item);
            }
            return r;
        }

        public Design currState()
        {
            return currentState;
        }
    }
}
