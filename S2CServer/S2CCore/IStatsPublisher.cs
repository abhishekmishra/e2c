using System;
using System.Collections.Generic;
using System.Text;

namespace S2CCore
{
    public interface IStatsPublisher
    {
        void RegisterListener(IStatsListener statsListener);

        void DeregisterListener(IStatsListener statsListener);

        void PublishStats(int round);
    }
}
