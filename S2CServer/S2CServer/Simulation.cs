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
    public class Simulation
    {

        public SimConfig SimulationConfig { get; private set; }
        private List<ISimulationViewer> Views = new List<ISimulationViewer>();
        private Space sp;
        private List<ICleaningAgent> agents;
        public SimState State { get; private set; }
        public bool Abort { get; set; }

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
            Abort = false;
            State = SimState.STOPPED;

            // Create a Cleaning Space as per configuration
            Console.WriteLine("Created new space to clean -> ");
            sp = new Space(SimulationConfig.Space.Rows, SimulationConfig.Space.Columns,
                SimulationConfig.Space.WallProbability, SimulationConfig.Space.DirtProbability);

            // Create agents as per configuration
            // and drop them into the space
            agents = AgentFactory.CreateAgents(SimulationConfig.Agents);
            foreach (var a in agents)
            {
                int agentId = 0;
                int r = 0, c = 0;
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        agentId = sp.initAgent(i, i);
                        r = c = i;
                        a.AgentId = agentId;
                        a.SpaceSize = new Coords(SimulationConfig.Space.Rows, SimulationConfig.Space.Columns);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        continue;
                    }
                    break;
                }
            }
        }

        public void AddView(ISimulationViewer v)
        {
            Views.Add(v);
        }

        private void _InitView()
        {
            // Setup a Console Simulation Viewer, if no View exists
            if (Views.Count == 0)
            {
                AddView(new ConsoleSimulationViewer());
            }
            foreach (var view in Views)
            {
                view.ShowState(0, new List<IAgentCommand>(), sp.space, sp.agentSpace);
            }
        }

        public void Run()
        {
            if (State == SimState.STOPPED)
            {
                Task.Run(() => _Run());
            }
            else
            {
                throw new ArgumentException("Simulation is not stopped.");
            }
        }

        public void RunForeground()
        {
            if (State == SimState.STOPPED)
            {
                _Run();
            }
        }

        private void _Run()
        {
            State = SimState.RUNNING;

            _InitView();
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
                int round = 1;
                while (sp.hasDirty())
                {
                    if(Abort)
                    {
                        Abort = false;
                        State = SimState.ABORTED;
                        break;
                    }

                    List<IAgentCommand> commands = new List<IAgentCommand>();
                    foreach (var a in agents)
                    {
                        (int row, int col) = sp.whereAmI(a.AgentId);
                        bool dirty = sp.isDirty(row, col);
                        var cmd = a.NextCommand(new Coords(row, col), dirty);
                        try
                        {
                            if (cmd is CleanCommand)
                            {
                                Coords c = cmd.Location;
                                sp.clean(a.AgentId, c.Row, c.Column);
                            }
                            else if (cmd is MoveToComand)
                            {
                                Coords c = cmd.Location;
                                sp.moveAgent(a.AgentId, c.Row, c.Column);
                            }
                            cmd.Status = true;
                            cmd.FailureReason = null;
                            commands.Add(cmd);
                            a.CommandResult(true, null);
                        }
                        catch (Exception e)
                        {
                            cmd.Status = false;
                            cmd.FailureReason = e.Message;
                            commands.Add(cmd);
                            a.CommandResult(false, e.Message);
                        }
                    }
                    foreach (var view in Views)
                    {
                        view.ShowState(round, commands, sp.space, sp.agentSpace);
                    }
                    round += 1;
                }
            }

            foreach (var view in Views)
            {
                view.SimComplete();
            }

            _Init();
        }

        //private void LoadConfig()
        //{
        //    json
        //    // Setup the input
        //    var input = new StringReader(Document);

        //    var deserializer = new DeserializerBuilder()
        //        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        //        .Build();

        //    SimulationConfig = deserializer.Deserialize<SimConfig>(input);
        //}

        //        private const string Document = @"---
        //            space:
        //                rows: 4
        //                columns: 4
        //                dirtProbability: 0.3
        //                wallProbability: 0.2

        //            agents:
        //                - type: simple
        //...";

        private void LoadConfig()
        {
            var options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true
            };

            SimulationConfig = JsonSerializer.Deserialize<SimConfig>(Document, options);
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
                        ""type"": ""simple"",
                        ""params"": {}
                    },
                    {
                        ""type"": ""simple"",
                        ""params"": {}
                    },
                ]
            }
        ";
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
