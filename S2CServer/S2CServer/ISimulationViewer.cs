using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface ISimulationViewer
    {
        void NewRound(int roundNum);

        void ShowState(Matrix<Double> space, Matrix<Double> agentSpace);

        void ShowMessage(string msg);

        void CommandExecuted(IAgentCommand command);

        void CommandFailed(IAgentCommand command);

        void Aborted();
    }
}
