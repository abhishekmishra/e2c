using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class MoveToComand: IAgentCommand
    {
        private int row, col;

        public MoveToComand(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public (int row, int col) GetLocation()
        {
            return (row, col);
        }

        public string GetName()
        {
            return "moveto";
        }

        public override string ToString()
        {
            return "Move To [" + row + ", " + col + "]";
        }
    }
}
