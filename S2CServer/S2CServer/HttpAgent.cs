using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class HttpAgent : ICleaningAgent
    {
        public int AgentId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Coords SpaceSize { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void CommandResult(bool success, string failureReason, SimulationErrorCode errorCode, Coords location)
        {
            throw new NotImplementedException();
        }

        public IAgentCommand NextCommand(Coords location, bool isDirty)
        {
            throw new NotImplementedException();
        }
    }
}
