using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class CleanCommand : BaseCommand
    {
        public CleanCommand(int agentId, int row, int col)
        {
            Name = "clean";
            AgentId = agentId;
            Location = new Coords(row, col);
        }

        public override string ToString()
        {
            return "Clean [" + Location.Row + ", " + Location.Column + "]";
        }
    }
}
