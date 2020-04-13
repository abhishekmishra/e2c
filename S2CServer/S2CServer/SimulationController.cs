using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace S2CCore
{
    /**
     * The task of the simulation controller is to create
     * a new simulation and maintain it's lifecycle.
     *
     * This includes creating the cleaning space, creating agents,
     * keeping score, initializing the display (if any) and also
     * ending the simulation when some goal is reached.
     */
    public class SimulationController
    {

        private SimConfig simConfig;
        List<ICleaningAgent> agents;

        public SimulationController()
        {
            // Load Simulation Configuration
            LoadConfig();

            // Create a Cleaning Space as per configuration
            Console.WriteLine("Created new space to clean -> ");
            var sp = new Space(simConfig.Space.Rows, simConfig.Space.Columns,
                simConfig.Space.WallProbability, simConfig.Space.DirtProbability);

            // Create agents as per configuration
            // and drop them into the space
            agents = new List<ICleaningAgent>();
            int agent = 0;
            int r = 0, c = 0;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    agent = sp.initAgent(i, i);
                    r = c = i;
                    var a = new SimpleCleaningAgent();
                    agents.Add(a);
                    a.SetLocation(r, c);
                    a.id = agent;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                break;
            }

            // Setup a Simulation Viewer
            ISimulationViewer view = new ConsoleSimulationViewer();
            view.ShowState(sp.space, sp.agentSpace);

            // Start Simulation
            // Each agent is allowed one command per round of lifecycle
            // Viewer is updated after every round.
            // Simulation continues till simulation goal is reached.
            if (agent > 0)
            {
                int round = 0;
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
                            a.CommandResult(true, null);
                            view.CommandExecuted(a.id, cmd.ToString());
                        }
                        catch (Exception e)
                        {
                            a.CommandResult(false, e.Message);
                            view.CommandFailed(a.id, cmd.ToString(), e.Message);
                        }
                    }
                    Console.WriteLine();
                    view.ShowState(sp.space, sp.agentSpace);
                    Thread.Sleep(50);
                    round += 1;
                }
            }
        }

        private void LoadConfig()
        {
            // Setup the input
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            simConfig = deserializer.Deserialize<SimConfig>(input);
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
        public SpaceConfig Space;
        public List<AgentConfig> Agents;
    }

    public class SpaceConfig
    {
        public int Rows;
        public int Columns;
        public double DirtProbability;
        public double WallProbability;
    }

    public class AgentConfig
    {
        public string Type;
    }
}
