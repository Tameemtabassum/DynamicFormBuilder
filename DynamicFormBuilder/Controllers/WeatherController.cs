using DynamicFormBuilder.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DynamicFormBuilder.Controllers
{
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public IActionResult Index()
        {
            var model = new
            {
                Current = _weatherService.GetCurrentWeather("Dhaka"),
                Next7Days = _weatherService.GetNext7Days(),
                Previous7Days = _weatherService.GetPrevious7Days()
            };

            return View(model);
        }
    }

}
