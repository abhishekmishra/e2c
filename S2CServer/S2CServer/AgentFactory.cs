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
                { "simple", _CurrentAssembly.GetType("S2Core.SimpleCleaningAgent") } 
            };
        public AgentFactory()
        {
        }
    }
}
