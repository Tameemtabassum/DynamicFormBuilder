
using DynamicFormBuilder.Models;

namespace DynamicFormBuilder.Services.Interfaces
{
    public interface IWeatherService
    {
        CurrentWeatherDto GetCurrentWeather(string city);
        List<DailyForecastDto> GetNext7Days();
        List<DailyForecastDto> GetPrevious7Days();
    }
}
