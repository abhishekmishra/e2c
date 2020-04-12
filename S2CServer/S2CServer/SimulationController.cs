using System;
using System.IO;
using YamlDotNet.RepresentationModel;

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
            // Setup the input
            var input = new StringReader(Document);

            // Load the stream
            var yaml = new YamlStream();
            yaml.Load(input);

            // Examine the stream
            var mapping =
                (YamlMappingNode)yaml.Documents[0].RootNode;

            foreach (var entry in mapping.Children)
            {
                Console.WriteLine(((YamlScalarNode)entry.Key).Value);
            }

            // List all the items
            var agents = (YamlSequenceNode)mapping.Children[new YamlScalarNode("agents")];
            foreach (YamlMappingNode agent in agents)
            {
                Console.WriteLine(
                    "{0}",
                    agent.Children[new YamlScalarNode("type")]);
            }
        }


        private const string Document = @"---
            space:
                nrow: 10
                ncol: 10
                dirt_prob: 0.3
                wall_prob: 0.0

            agents:
                - type: simple
...";
    }
}
