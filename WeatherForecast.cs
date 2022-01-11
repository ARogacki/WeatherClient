using System;

namespace WeatherClient
{
    public class WeatherForecast
    {
        public DateTime? Date { get; set; }
        public double? Temp { get; set; }
        public double? Pressure { get; set; }
        public double? Humidity { get; set; }
        public double? Precipitation { get; set; }
        public double? Wind_speed { get; set; }
        public double? Wind_direction { get; set; }
    }
}
