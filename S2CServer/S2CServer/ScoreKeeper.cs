using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class ScoreKeeper : ISimulationViewer
    {
        public void CommandExecuted(IAgentCommand command)
        {
            throw new NotImplementedException();
        }

        public void CommandFailed(IAgentCommand command)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public void ShowState(int simRound, List<IAgentCommand> commands, Matrix<double> space, Matrix<double> agentSpace)
        {
            throw new NotImplementedException();
        }

        public void SimAborted()
        {
            throw new NotImplementedException();
        }

        public void SimComplete()
        {
            throw new NotImplementedException();
        }

        public void SimStarted(int simNum, string name)
        {
            throw new NotImplementedException();
        }
    }
}
