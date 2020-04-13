using MathNet.Numerics.LinearAlgebra;
using S2CCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S2CServices
{
    public class WebSimulationViewer : ISimulationViewer
    {
        private readonly Dictionary<int, List<IAgentCommand>> commands;
        private readonly Dictionary<int, List<string>> messages;
        public int CurrentRound { get; set; }

        private Matrix<double> latestSpace;
        private Matrix<double> latestAgentSpace;

        public WebSimulationViewer()
        {
            commands = new Dictionary<int, List<IAgentCommand>>();
            messages = new Dictionary<int, List<string>>();
        }

        public void CommandExecuted(IAgentCommand command)
        {
            commands[CurrentRound].Add(command);
        }

        public void CommandFailed(IAgentCommand command)
        {
            commands[CurrentRound].Add(command);
        }

        public void NewRound(int roundNum)
        {
            CurrentRound = roundNum;
            commands.Add(CurrentRound, new List<IAgentCommand>());
            messages.Add(CurrentRound, new List<string>());
        }

        public void ShowMessage(string msg)
        {
            messages[CurrentRound].Add(msg);
        }

        public void ShowState(Matrix<double> space, Matrix<double> agentSpace)
        {
            latestSpace = Matrix<double>.Build.DenseOfMatrix(space); ;
            latestAgentSpace = Matrix<double>.Build.DenseOfMatrix(agentSpace);
        }
    }
}
