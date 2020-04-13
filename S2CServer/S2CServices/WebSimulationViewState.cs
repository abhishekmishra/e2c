using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using S2CCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S2CServices
{
    public class WebSimulationViewState
    {
        public int RoundNum { get; set; }
        public List<string> Messages { get; set; }
        public List<List<double>> SpaceArr { get; set; }
        public List<List<double>> AgentSpaceArr { get; set; }
        public List<IAgentCommand> Commands { get; set; }
    }
}
