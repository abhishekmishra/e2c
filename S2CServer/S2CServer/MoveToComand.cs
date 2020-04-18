using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class MoveToCommand: BaseCommand
    {
        public MoveToCommand(int agentId, int row, int col)
        {
            Name = "moveto";
            AgentId = agentId;
            Location = new Coords(row, col);
        }

        public override string ToString()
        {
            return "Move To [" + Location.Row + ", " + Location.Column + "]";
        }
    }
}
