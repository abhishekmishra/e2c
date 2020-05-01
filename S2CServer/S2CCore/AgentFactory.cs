using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace S2CCore
{
    public class AgentFactory
    {
        private static Assembly _CurrentAssembly = typeof(AgentFactory).Assembly;
        private static readonly Dictionary<string, Type> _AgentTypes
            = new Dictionary<string, Type>()
            {
                { "simple", _CurrentAssembly.GetType("S2CCore.SimpleCleaningAgent") },
                { "simple.bound", _CurrentAssembly.GetType("S2CCore.SimpleBoundCheckAgent") },
                { "simple.boundandwall", _CurrentAssembly.GetType("S2CCore.BoundAndWallCheckAgent") },
                { "graph.visitonce", _CurrentAssembly.GetType("S2CCore.GraphBasedVisitOnceAgent") },
                { "spiral", _CurrentAssembly.GetType("S2CCore.SpiralAgent") },
                { "http", _CurrentAssembly.GetType("S2CCore.HttpAgent") },
            };


        public static List<ICleaningAgent> CreateAgents(List<AgentConfig> agentConfigs)
        {
            List<ICleaningAgent> agents = new List<ICleaningAgent>();
            foreach (var agentConfig in agentConfigs)
            {
                var agentType = agentConfig.Type;
                ICleaningAgent agent = 
                    (ICleaningAgent)Activator.CreateInstance(_AgentTypes[agentType], agentConfig.Params);
                agents.Add(agent);
            }
            return agents;
        }
    }
}
