using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace S2CCore
{
    public enum SimState
    {
        STOPPED = 0, RUNNING, ABORTED
    }

    /**
     * The task of the simulation class is to create
     * a new simulation and maintain it's lifecycle.
     *
     * This includes creating the cleaning space, creating agents,
     * keeping score, initializing the display (if any) and also
     * ending the simulation when some goal is reached.
     */
    public class Simulation : IStatsPublisher
    {

        public SimConfig SimulationConfig { get; private set; }
        private List<ISimulationViewer> Views;
        private Space space;
        private int round { get; set; }
        private List<ICleaningAgent> agents;
        public SimState State { get; private set; }
        public bool Abort { get; set; }
        private ScoreKeeper statsPublisher;

        public Simulation()
        {
            // Load Default Simulation Configuration
            LoadConfig();
            _Init();
        }

        public Simulation(SimConfig cfg)
        {
            SimulationConfig = cfg;
            _Init();
        }

        private void _Init()
        {
            statsPublisher = new ScoreKeeper();
            Views = new List<ISimulationViewer>();
            AddView(statsPublisher);
        }

        public void Prepare()
        {
            Abort = false;
            State = SimState.STOPPED;

            // Create a Cleaning Space as per configuration
            Console.WriteLine("Created new space to clean -> ");
            space = new Space(SimulationConfig.Space.Rows, SimulationConfig.Space.Columns,
                SimulationConfig.Space.WallProbability, SimulationConfig.Space.DirtProbability);

            // Create agents as per configuration
            // and drop them into the space
            agents = AgentFactory.CreateAgents(SimulationConfig.Agents);
            foreach (var a in agents)
            {
                int agentId = 0;
                agentId = space.InitAgent();
                a.AgentId = agentId;
                a.SpaceSize = new Coords(SimulationConfig.Space.Rows, SimulationConfig.Space.Columns);
            }
        }

        public void AddView(ISimulationViewer v)
        {
            Views.Add(v);
        }

        public void Run()
        {
            if (State == SimState.STOPPED)
            {
                Prepare();
                Task.Run(() => _Run());
            }
            else
            {
                throw new SimulationException("Simulation is not stopped.", 
                    SimulationErrorCode.SIM_ERR_SIM_RUNNING, null);
            }
        }

        public void RunForeground()
        {
            if (State == SimState.STOPPED)
            {
                Prepare();
                _Run();
            }
            else
            {
                throw new SimulationException("Simulation is not stopped.", 
                    SimulationErrorCode.SIM_ERR_SIM_RUNNING, null);
            }
        }

        private void _Run()
        {
            round = 0;
            State = SimState.RUNNING;

            foreach (var view in Views)
            {
                view.ShowState(round, new List<IAgentCommand>(), space.space, space.agentSpace);
            }
            PublishStats(round);
            foreach (var view in Views)
            {
                view.SimStarted(0, null);
            }

            // Start Simulation
            // Each agent is allowed one command per round of lifecycle
            // Viewer is updated after every round.
            // Simulation continues till simulation goal is reached.
            if (agents.Count > 0)
            {
                round = 1;
                while (space.HasDirty())
                {
                    if (Abort)
                    {
                        Abort = false;
                        State = SimState.ABORTED;
                        break;
                    }

                    List<IAgentCommand> commands = new List<IAgentCommand>();
                    foreach (var a in agents)
                    {
                        (int row, int col) = space.WhereAmI(a.AgentId);
                        bool dirty = space.IsDirty(row, col);
                        var cmd = a.NextCommand(new Coords(row, col), dirty);
                        try
                        {
                            if (cmd is CleanCommand)
                            {
                                Coords c = cmd.Location;
                                space.Clean(a.AgentId, c.Row, c.Column);
                            }
                            else if (cmd is MoveToCommand)
                            {
                                Coords c = cmd.Location;
                                space.MoveAgent(a.AgentId, c.Row, c.Column);
                            }
                            cmd.Status = true;
                            cmd.FailureReason = null;
                            commands.Add(cmd);
                            a.CommandResult(true, null, 
                                SimulationErrorCode.SIM_ERR_SUCCESS, cmd.Location);
                        }
                        catch (SimulationException e)
                        {
                            cmd.Status = false;
                            cmd.FailureReason = e.Message;
                            commands.Add(cmd);
                            a.CommandResult(false, e.Message, e.ErrorCode, e.Location);
                        }
                        catch (Exception e)
                        {
                            cmd.Status = false;
                            cmd.FailureReason = e.Message;
                            commands.Add(cmd);
                            a.CommandResult(false, e.Message, 
                                SimulationErrorCode.SIM_ERR_UNKNOWN, cmd.Location);
                        }
                    }
                    foreach (var view in Views)
                    {
                        view.ShowState(round, commands, space.space, space.agentSpace);
                    }
                    PublishStats(round);
                    round += 1;
                }
            }

            foreach (var view in Views)
            {
                view.SimComplete();
            }

            Prepare();
        }

        private void LoadConfig()
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            SimulationConfig = JsonSerializer.Deserialize<SimConfig>(Document, options);
        }

        public void RegisterListener(IStatsListener statsListener)
        {
            statsPublisher.RegisterListener(statsListener);
        }

        public void DeregisterListener(IStatsListener statsListener)
        {
            statsPublisher.DeregisterListener(statsListener);
        }

        public void PublishStats(int round)
        {
            statsPublisher.PublishStats(round);
        }

        private const string Document = @"
            {
                ""space"": {
                    ""rows"": 10,
                    ""columns"": 20,
                    ""dirtProbability"": 0.3,
                    ""wallProbability"": 0.1
                },
                ""agents"": [
                    {
                        ""type"": ""http"",
                        ""params"": {
                            ""url"": ""http://127.0.0.1:5000/""
                        }
                    },
                ]
            }
        ";

                    //        {
                    //    ""type"": ""simple"",
                    //    ""params"": {}
                    //},
                    //{
                    //    ""type"": ""simple.bound"",
                    //    ""params"": {}
                    //},
                    //{
                    //    ""type"": ""simple.bound"",
                    //    ""params"": {}
                    //},

        //{
        //    ""type"": ""simple-bound-check"",
        //    ""params"": {
        //        ""blah"": ""bluh""
        //    }
        //}
    }

    public class SimConfig
    {
        public SpaceConfig Space { get; set; }
        public List<AgentConfig> Agents { get; set; }
    }

    public class SpaceConfig
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public double DirtProbability { get; set; }
        public double WallProbability { get; set; }
    }

    public class AgentConfig
    {
        public string Type { get; set; }
        public Dictionary<string, string> Params { get; set; }
    }
}
