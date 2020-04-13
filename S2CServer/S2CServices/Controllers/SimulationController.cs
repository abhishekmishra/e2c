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
        private static readonly WebSimulationViewer simViewer = new WebSimulationViewer();
        private readonly Simulation simulation = new Simulation();

        public SimulationController(ILogger<SimulationController> logger)
        {
            _logger = logger;
            simulation.SetView(simViewer);
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("config")]
        public SimConfig GetSimConfig()
        {
            return simulation.SimulationConfig;
        }

        [HttpGet("start")]
        public string StartSim()
        {
            simulation.Run();
            return "started";
        }

        [HttpGet("round")]
        public int GetCurrentRound()
        {
            return simViewer.CurrentRound;
        }
    }
}
