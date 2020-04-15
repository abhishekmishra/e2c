using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class ScoreKeeper : ISimulationViewer
    {
        public void Aborted()
        {
            throw new NotImplementedException();
        }

        public void CommandExecuted(IAgentCommand command)
        {
            throw new NotImplementedException();
        }

        public void CommandFailed(IAgentCommand command)
        {
            throw new NotImplementedException();
        }

        public void NewRound(int roundNum)
        {
            throw new NotImplementedException();
        }

        public void ShowMessage(string msg)
        {
            throw new NotImplementedException();
        }

        public void ShowState(Matrix<double> space, Matrix<double> agentSpace)
        {
            throw new NotImplementedException();
        }
    }
}
