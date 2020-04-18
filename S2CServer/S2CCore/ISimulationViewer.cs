using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface ISimulationViewer
    {
        void SimStarted(int simNum, string name);

        void SimComplete();

        void SimAborted();

        void ShowState(int simRound, List<IAgentCommand> commands,
            Matrix<Double> space, Matrix<Double> agentSpace);

        void ShowMessage(string msg);
    }
}
