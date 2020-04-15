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
        private Dictionary<int, List<IAgentCommand>> commands;
        private Dictionary<int, List<string>> messages;
        public int CurrentRound { get; set; }

        private Dictionary<int, Matrix<double>> spaceHist;
        private Dictionary<int, Matrix<double>> agentSpaceHist;

        public bool SimAborted { get; set; }

        public WebSimulationViewer()
        {
            Init();
            SimAborted = false;
        }

        private void Init()
        {
            CurrentRound = 0;
            commands = new Dictionary<int, List<IAgentCommand>>();
            messages = new Dictionary<int, List<string>>();
            spaceHist = new Dictionary<int, Matrix<double>>();
            agentSpaceHist = new Dictionary<int, Matrix<double>>();
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
            if(roundNum == 0 && roundNum < CurrentRound)
            {
                Init();
            }
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
            spaceHist.Add(CurrentRound, Matrix<double>.Build.DenseOfMatrix(space));
            agentSpaceHist.Add(CurrentRound, Matrix<double>.Build.DenseOfMatrix(agentSpace));
        }

        public WebSimulationViewState ViewState(int round)
        {
            if (round <= CurrentRound)
            {
                var s = new WebSimulationViewState();
                s.RoundNum = round;
                s.Commands = commands[round];
                s.Messages = messages[round];
                
                var a = spaceHist[round];
                s.SpaceArr = new List<List<double>>();
                for(int i = 0; i < a.RowCount; i++)
                {
                    s.SpaceArr.Add(new List<double>());
                    for (int j = 0; j < a.ColumnCount; j++)
                    {
                        s.SpaceArr[i].Add(a[i, j]);
                    }
                }
                
                a = agentSpaceHist[round];
                s.AgentSpaceArr = new List<List<double>>();
                for (int i = 0; i < a.RowCount; i++)
                {
                    s.AgentSpaceArr.Add(new List<double>());
                    for (int j = 0; j < a.ColumnCount; j++)
                    {
                        s.AgentSpaceArr[i].Add(a[i, j]);
                    }
                }
                return s;
            }else
            {
                return null;
            }
        }

        public void Aborted()
        {
            SimAborted = true;
        }
    }
}
