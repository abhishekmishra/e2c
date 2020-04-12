using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace S2CServer
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
        public SimulationController()
        {
            LoadConfig();

            var sp = new Space(10, 10, 0.0, 0.3);
            int agent = 0;
            int r = 0, c = 0;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    agent = sp.initAgent(i, i);
                    r = c = i;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                break;
            }
            sp.printSpace();
            if (agent > 0)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        sp.moveAgent(agent, r, c + i);
                        bool cleaned = sp.clean(agent, r, c + i);
                        if (cleaned)
                        {
                            Console.WriteLine("Cleaned [" + r + ", " + (c + i) + "]");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine();
            sp.printSpace();

        }

        private static void LoadConfig()
        {
            // Setup the input
            var input = new StringReader(Document);

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var simConfig = deserializer.Deserialize<SimConfig>(input);
            Console.WriteLine(simConfig.Space.Rows);
        }

        private const string Document = @"---
            space:
                rows: 10
                columns: 10
                dirtProbability: 0.3
                wallProbability: 0.0

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
