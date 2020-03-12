using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTracing;

namespace JaegerDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITracer tracer;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,ITracer tracer)
        {
            _logger = logger;
            this.tracer = tracer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get()
        {
            var rng = new Random();
            using (IScope scope = tracer.BuildSpan("waitingForValues").StartActive(finishSpanOnDispose: true))
            {
                await Task.Delay(1000);
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
                    .ToArray();
            }
        }
    }
}