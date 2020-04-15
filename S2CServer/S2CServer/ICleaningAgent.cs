using System;
namespace S2CCore
{
    public interface ICleaningAgent
    {
        public int AgentId { get; set; }

        public Coords SpaceSize { get; set; }

        IAgentCommand NextCommand(Coords location, bool isDirty);

        void CommandResult(bool success, string failureReason);
    }
}
