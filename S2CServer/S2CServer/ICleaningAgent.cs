using System;
namespace S2CServer
{
    public interface ICleaningAgent
    {
        public int id { get; set; }

        void SetLocation(int row, int col);

        void SetLocationDirty(bool isDirty);

        IAgentCommand GetCommand();

        void CommandResult(bool success, string failureReason);
    }
}
