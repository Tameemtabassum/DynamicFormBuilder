using DynamicFormBuilder.Models;
using DynamicFormBuilder.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace DynamicFormBuilder.Services.Implementations
{
    public class WeatherService : IWeatherService
    {
        public CurrentWeatherDto GetCurrentWeather(string city)
        {
            return new CurrentWeatherDto
            {
                City = city,
                Temperature = 25.5,
                Condition = "Sunny",
                Date = DateTime.Now
            };
        }

        public List<DailyForecastDto> GetNext7Days()
        {
            return GenerateDays(1, 7);
        }

        public List<DailyForecastDto> GetPrevious7Days()
        {
            return GenerateDays(-7, -1);
        }

        private List<DailyForecastDto> GenerateDays(int start, int end)
        {
            var list = new List<DailyForecastDto>();

            for (int i = start; i <= end; i++)
            {
                list.Add(new DailyForecastDto
                {
                    Date = DateTime.Today.AddDays(i),
                    MinTemp = 22 + i % 3,
                    MaxTemp = 30 + i % 4,
                    Condition = "Cloudy"
                });
            }

            return list;
        }
    }
}



