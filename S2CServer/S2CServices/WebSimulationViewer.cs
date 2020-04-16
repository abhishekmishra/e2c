using MathNet.Numerics.LinearAlgebra;
using S2CCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S2CServices
{
    public class WebSimulationViewer : ISimulationViewer, IStatsListener
    {
        private Dictionary<int, List<IAgentCommand>> commands;
        private Dictionary<int, List<string>> messages;
        public int CurrentRound { get; set; }

        private Dictionary<int, Matrix<double>> spaceHist;
        private Dictionary<int, Matrix<double>> agentSpaceHist;

        public SimState State { get; set; }
        public Dictionary<int, SpaceStatistics> SpaceStatistics { get; set; }
        public Dictionary<int, List<AgentStatistics>> AgentStatistics { get; set; }

        public WebSimulationViewer()
        {
            Init();
            State = SimState.STOPPED;
        }

        private void Init()
        {
            CurrentRound = 0;
            commands = new Dictionary<int, List<IAgentCommand>>();
            messages = new Dictionary<int, List<string>>();
            spaceHist = new Dictionary<int, Matrix<double>>();
            agentSpaceHist = new Dictionary<int, Matrix<double>>();
            SpaceStatistics = new Dictionary<int, SpaceStatistics>();
            AgentStatistics = new Dictionary<int, List<AgentStatistics>>();
        }

        public void ShowMessage(string msg)
        {
            messages[CurrentRound].Add(msg);
        }

        public void ShowState(int simRound, List<IAgentCommand> cmds,
            Matrix<Double> space, Matrix<Double> agentSpace)
        {
            if (simRound == 0 && simRound < CurrentRound)
            {
                Init();
            }
            CurrentRound = simRound;
            List<IAgentCommand> cmd_list = new List<IAgentCommand>(cmds);
            commands.Add(CurrentRound, cmd_list);
            messages.Add(CurrentRound, new List<string>());

            spaceHist.Add(CurrentRound, Matrix<double>.Build.DenseOfMatrix(space));
            agentSpaceHist.Add(CurrentRound, Matrix<double>.Build.DenseOfMatrix(agentSpace));
        }


        public void SimAborted()
        {
            State = SimState.ABORTED;
        }

        public void SimStarted(int simNum, string name)
        {
            State = SimState.RUNNING;
        }

        public void SimComplete()
        {
            State = SimState.STOPPED;
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
                for (int i = 0; i < a.RowCount; i++)
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
                s.SpaceStatistics = SpaceStatistics[round];
                s.AgentStatistics = AgentStatistics[round];
                return s;
            }
            else
            {
                return null;
            }
        }

        public void GetStats(SpaceStatistics spaceStatistics,
            Dictionary<int, AgentStatistics> agentStatistics)
        {
            SpaceStatistics.Add(CurrentRound, spaceStatistics);
            var s = new List<AgentStatistics>(agentStatistics.Values);
            AgentStatistics.Add(CurrentRound, s);
        }
    }
}
