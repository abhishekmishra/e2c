using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace S2CCore
{
    public enum SimState
    {
        STOPPED = 0, RUNNING, STOPPING
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
        private ISimulationViewer view;
        private Space sp;
        private List<ICleaningAgent> agents;
        public SimState State { get; private set; }

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
            State = SimState.STOPPED;

            // Create a Cleaning Space as per configuration
            Console.WriteLine("Created new space to clean -> ");
            sp = new Space(SimulationConfig.Space.Rows, SimulationConfig.Space.Columns,
                SimulationConfig.Space.WallProbability, SimulationConfig.Space.DirtProbability);

            // Create agents as per configuration
            // and drop them into the space
            agents = new List<ICleaningAgent>();
            int agentId = 0;
            int r = 0, c = 0;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    agentId = sp.initAgent(i, i);
                    r = c = i;
                    var a = new SimpleCleaningAgent();
                    agents.Add(a);
                    a.SetLocation(r, c);
                    a.id = agentId;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                break;
            }
        }

        public void SetView(ISimulationViewer v)
        {
            view = v;
        }

        private void _InitView()
        {
            // Setup a Console Simulation Viewer, if no View exists
            if (view == null)
            {
                view = new ConsoleSimulationViewer();
            }
            view.NewRound(0);
            view.ShowState(sp.space, sp.agentSpace);
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

        private void _Run()
        {
            State = SimState.RUNNING;

            _InitView();

            // Start Simulation
            // Each agent is allowed one command per round of lifecycle
            // Viewer is updated after every round.
            // Simulation continues till simulation goal is reached.
            if (agents.Count > 0)
            {
                int round = 1;
                while (sp.hasDirty())
                {
                    view.NewRound(round);
                    foreach (var a in agents)
                    {
                        (int row, int col) = sp.whereAmI(a.id);
                        bool dirty = sp.isDirty(row, col);
                        a.SetLocation(row, col);
                        a.SetLocationDirty(dirty);
                        var cmd = a.GetCommand();
                        try
                        {
                            if (cmd is CleanCommand)
                            {
                                (int rx, int cx) = cmd.GetLocation();
                                sp.clean(a.id, rx, cx);
                            }
                            else if (cmd is MoveToComand)
                            {
                                (int rx, int cx) = cmd.GetLocation();
                                sp.moveAgent(a.id, rx, cx);
                            }
                            cmd.Status = true;
                            cmd.FailureReason = null;
                            a.CommandResult(true, null);
                            view.CommandExecuted(cmd);
                        }
                        catch (Exception e)
                        {
                            cmd.Status = false;
                            cmd.FailureReason = e.Message;
                            a.CommandResult(false, e.Message);
                            view.CommandFailed(cmd);
                        }
                    }
                    //Console.WriteLine();
                    view.ShowState(sp.space, sp.agentSpace);
                    round += 1;
                }
            }

            _Init();
        }

        private void LoadConfig()
        {
            // Setup the input
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            SimulationConfig = deserializer.Deserialize<SimConfig>(input);
        }

        private const string Document = @"---
            space:
                rows: 4
                columns: 4
                dirtProbability: 0.3
                wallProbability: 0.2

            agents:
                - type: simple
...";
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
    }
}
