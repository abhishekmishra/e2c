using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    interface ISimulationViewer
    {
        void NewRound(int roundNum);

        void ShowState(Matrix<Double> space, Matrix<Double> agentSpace);

        void ShowMessage(string msg);

        void CommandExecuted(int agentId, string command);

        void CommandFailed(int agentId, string command, string reason);
    }
}
