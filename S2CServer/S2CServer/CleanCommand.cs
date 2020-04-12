using System;
using System.Collections.Generic;
using System.Text;

namespace S2CServer
{
    public class CleanCommand : IAgentCommand
    {
        private int row, col;

        public CleanCommand(int row, int col)
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
            return "clean";
        }

        public override string ToString()
        {
            return "Clean [" + row + ", " + col + "]";
        }
    }
}
