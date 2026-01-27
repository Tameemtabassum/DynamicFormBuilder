
namespace DynamicFormBuilder.Models
{
    public class DailyForecastDto
    {
        public DateTime Date { get; set; }
        public double MinTemp { get; set; }
        public double MaxTemp { get; set; }
        public string Condition { get; set; }
    }
}