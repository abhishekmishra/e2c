using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using S2CCore;

namespace S2CServices
{
    public class SimulationManager
    {
        private readonly ILogger<SimulationManager> _logger;
        private Mutex _simsMutex = new Mutex();
        private List<Simulation> _sims;
        private List<WebSimulationViewer> _views;
        private int _count;

        public SimulationManager(ILogger<SimulationManager> logger)
        {
            _logger = logger;
            _logger.LogInformation("Creating new sims and views dictionary.");
            _simsMutex.WaitOne();

            _count = -1;
            _sims = new List<Simulation>();
            _views = new List<WebSimulationViewer>();

            _simsMutex.ReleaseMutex();
        }

        public int NumSims()
        {
            return _count + 1;
        }

        public int NewSim(SimConfig cfg)
        {
            _simsMutex.WaitOne();
            _count = _count + 1;
            _logger.LogInformation("Creating new sim id#{1}", _count);

            Simulation s;
            if (cfg == null)
            {
                s = new Simulation();
            } else
            {
                s = new Simulation(cfg);
            }

            WebSimulationViewer w = new WebSimulationViewer();
            s.AddView(w);

            _sims.Add(s);
            _views.Add(w);
            _simsMutex.ReleaseMutex();
            return _count;
        }

        public WebSimulationViewer GetView(int count)
        {
            if(count <= _count)
            {
                return _views[count];
            }
            return null;
        }

        public SimConfig GetSimConfig(int count)
        {
            if (count <= _count)
            {
                return _sims[count].SimulationConfig;
            }
            return null;
        }

        public void StartSim(int count)
        {
            _simsMutex.WaitOne();
            if (count <= _count)
            {
                var s = _sims[count];
                if (s.State == SimState.STOPPED)
                {
                    s.Run();
                }
                else
                {
                    _simsMutex.ReleaseMutex();
                    throw new ArgumentException("Sim is already running.");
                }
                _logger.LogInformation("Started run for id#{1}", count);
            } else
            {
                _logger.LogError("No such sim id#{1}", count);
            }
            _simsMutex.ReleaseMutex();
        }

        public void AbortSim(int count)
        {
            _simsMutex.WaitOne();
            if (count <= _count)
            {
                var s = _sims[count];
                s.Abort = true;
            }
            _simsMutex.ReleaseMutex();
        }
    }
}
