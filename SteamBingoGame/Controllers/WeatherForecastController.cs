using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace SteamBingoGame.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [EnableCors("CorsPolicy")]
        [HttpGet(Name = "GetWeatherForecast")]
        public Lobby Get(int id)
        {
            return new Lobby(2);
        }
    }
}