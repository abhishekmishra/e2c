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
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<SimulationController> _logger;
        private readonly WebSimulationViewer _simViewer;
        private readonly Simulation _sim;

        public SimulationController(ILogger<SimulationController> logger, WebSimulationViewer simViewer, Simulation sim)
        {
            _logger = logger;
            _simViewer = simViewer;
            _sim = sim;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        [HttpGet("config")]
        public SimConfig GetSimConfig()
        {
            return _sim.SimulationConfig;
        }

        [HttpGet("start")]
        public string StartSim()
        {
            if (_sim.State == SimState.STOPPED)
            {
                _sim.Run();
                return "started";
            } else
            {
                return "error: already running";
            }
        }

        [HttpGet("roundnum")]
        public int GetCurrentRoundNum()
        {
            return _simViewer.CurrentRound;
        }

        [HttpGet("round/{round}")]
        public WebSimulationViewState GetRound(int round)
        {
            return _simViewer.ViewState(round);
        }

    }
}
