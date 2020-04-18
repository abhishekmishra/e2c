using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface IAgentCommand
    {
        int AgentId { get; set; }

        string Name { get; set; }

        Coords Location { get; set; }

        bool Status { get; set; }

        string FailureReason { get; set; }
    }
}
