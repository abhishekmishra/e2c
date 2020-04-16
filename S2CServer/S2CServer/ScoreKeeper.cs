using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public class SpaceStatistics
    {
        public Coords Size { get; set; }
        public int Dirty { get; set; }
        public int Wall { get; set; }
    }

    public class AgentStatistics
    {
        public int AgentId { get; set; }
        public int Commands { get; set; } = 0;
        public int CommandSuccess { get; set; } = 0;
        public int Moves { get; set; } = 0;
        public int MoveSuccess { get; set; } = 0;
        public int Clean { get; set; } = 0;
        public int CleanSuccess { get; set; } = 0;
        public double Efficiency { get; set; } = 0;
        public double ErrorRate { get; set; } = 0;
    }
    public class ScoreKeeper : ISimulationViewer, IStatsPublisher
    {
        public SpaceStatistics SpaceStats { get; set; }
        public Dictionary<int, AgentStatistics> AgentStats { get; set; }

        private readonly List<IStatsListener> StatsListeners;

        public ScoreKeeper()
        {
            SpaceStats = new SpaceStatistics();
            AgentStats = new Dictionary<int, AgentStatistics>();
            StatsListeners = new List<IStatsListener>();
        }

        public void ShowMessage(string msg)
        {
            // do nothing
        }

        public void ShowState(int simRound, List<IAgentCommand> commands, Matrix<double> space, Matrix<double> agentSpace)
        {
            if (simRound == 0)
            {
                SpaceStats.Size = new Coords(space.RowCount, space.ColumnCount);
                SpaceStats.Dirty = 0;
                for (int i = 0; i < space.RowCount; i++)
                {
                    for (int j = 0; j < space.ColumnCount; j++)
                    {
                        if (space[i, j] == Space.DIRTY)
                        {
                            SpaceStats.Dirty += 1;
                        }
                        if (space[i, j] == Space.WALL)
                        {
                            SpaceStats.Wall += 1;
                        }
                    }
                }
            }

            foreach (var command in commands)
            {
                if (!AgentStats.ContainsKey(command.AgentId))
                {
                    var agentStats = new AgentStatistics();
                    agentStats.AgentId = command.AgentId;
                    AgentStats[command.AgentId] = agentStats;
                }

                var astats = AgentStats[command.AgentId];
                astats.Commands += 1;
                if (command.Status)
                {
                    astats.CommandSuccess += 1;
                }
                astats.ErrorRate = Math.Round(
                    (1 - ((astats.CommandSuccess * 1.0) / astats.Commands)) * 100.0, 2);

                if (command.Name == "clean")
                {
                    astats.Clean += 1;
                    if (command.Status)
                    {
                        astats.CleanSuccess += 1;
                    }
                }

                if (command.Name == "moveto")
                {
                    astats.Moves += 1;
                    if (command.Status)
                    {
                        astats.MoveSuccess += 1;
                    }
                }

                astats.Efficiency = Math.Round(
                    (astats.CleanSuccess * 100.0) / astats.Commands, 2);
            }

            if (simRound > 0)
            {
                foreach (var listener in StatsListeners)
                {
                    listener.GetStats(SpaceStats, AgentStats);
                }
            }
        }

        public void SimAborted()
        {
            // do nothing
        }

        public void SimComplete()
        {
            // do nothing
        }

        public void SimStarted(int simNum, string name)
        {
            // do nothing
        }

        public void RegisterListener(IStatsListener statsListener)
        {
            if (!StatsListeners.Contains(statsListener))
            {
                StatsListeners.Add(statsListener);
            }
        }

        public void DeregisterListener(IStatsListener statsListener)
        {
            if (StatsListeners.Contains(statsListener))
            {
                StatsListeners.Remove(statsListener);
            }
        }
    }
}
