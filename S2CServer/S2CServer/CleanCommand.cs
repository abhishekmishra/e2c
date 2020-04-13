using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class CleanCommand : BaseCommand
    {
        private int row, col;

        public CleanCommand(int agentId, int row, int col)
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
            return "clean";
        }

        public override string ToString()
        {
            return "Clean [" + row + ", " + col + "]";
        }
    }
}
