using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    //TODO: move to Cleaning Agent base class
    public enum Direction
    {
        N = 0, E, W, S
    }

    public class SimpleCleaningAgent : SimpleAgentBase
    {
        public SimpleCleaningAgent(Dictionary<string, string> args) : base(args)
        {
            //do nothing
        }

        public override IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            if (isDirty)
            {
                return new CleanCommand(AgentId, location.Row, location.Column);
            }
            else
            {
                (int r, int c) = NewLocation(location);
                return new MoveToComand(AgentId, r, c);
            }
        }
    }
}
