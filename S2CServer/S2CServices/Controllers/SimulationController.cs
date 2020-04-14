using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using S2CCore;

namespace S2CServices.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SimulationController : ControllerBase
    {
        private readonly ILogger<SimulationController> _logger;
        private readonly SimulationManager _mgr;

        public SimulationController(ILogger<SimulationController> logger,
            SimulationManager mgr)
        {
            _logger = logger;
            _mgr = mgr;
        }

        [HttpGet("new/default")]
        public int NewSim()
        {
            return _mgr.NewSim(null);
        }

        [HttpPost("new")]
        public int NewSim(SimConfig cfg)
        {
            return _mgr.NewSim(cfg);
        }

        [HttpGet("{id}/config")]
        public SimConfig GetSimConfig(int id)
        {
            return _mgr.GetSimConfig(id);
        }

        [HttpGet("{id}/start")]
        public string StartSim(int id)
        {
            try
            {
                _mgr.StartSim(id);
                return "started";
            } catch(Exception e)
            {
                return "error: " + e.Message;
            }
        }

        [HttpGet("{id}/roundnum")]
        public int GetCurrentRoundNum(int id)
        {
            return _mgr.GetView(id).CurrentRound;
        }

        [HttpGet("{id}/round/{round}")]
        public WebSimulationViewState GetRound(int id, int round)
        {
            return _mgr.GetView(id).ViewState(round);
        }

    }
}
