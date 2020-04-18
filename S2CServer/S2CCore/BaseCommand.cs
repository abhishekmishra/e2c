using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public abstract class BaseCommand : IAgentCommand
    {
        public int AgentId { get; set; }
        public bool Status { get; set; }
        public string FailureReason { get; set; }
        public string Name { get ; set ; }
        public Coords Location { get ; set ; }
    }
}
