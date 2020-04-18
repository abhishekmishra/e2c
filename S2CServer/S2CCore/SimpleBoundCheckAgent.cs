using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class SimpleBoundCheckAgent : SimpleAgentBase
    {
        public SimpleBoundCheckAgent(Dictionary<string, string> args) : base(args)
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
                int r, c;
                do
                {
                    (r, c) = NewLocation(location);
                } while (r < 0 || r >= SpaceSize.Row
                        || c < 0 || c >= SpaceSize.Column);
                return new MoveToCommand(AgentId, r, c);
            }
        }
    }
}
