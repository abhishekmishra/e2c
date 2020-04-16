using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface IStatsListener
    {
        void GetStats(SpaceStatistics spaceStatistics, Dictionary<int, AgentStatistics> agentStatistics);
    }
}
