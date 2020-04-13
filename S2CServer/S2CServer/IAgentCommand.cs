using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface IAgentCommand
    {
        int AgentId { get; set; }

        string GetName();

        (int row, int col) GetLocation();

        bool Status { get; set; }

        string FailureReason { get; set; }
    }
}
