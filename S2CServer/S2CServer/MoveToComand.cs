using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class MoveToComand: BaseCommand
    {
        private int row, col;

        public MoveToComand(int agentId, int row, int col)
        {
            AgentId = agentId;
            this.row = row;
            this.col = col;
        }

        public override (int row, int col) GetLocation()
        {
            return (row, col);
        }

        public override string GetName()
        {
            return "moveto";
        }

        public override string ToString()
        {
            return "Move To [" + row + ", " + col + "]";
        }
    }
}
