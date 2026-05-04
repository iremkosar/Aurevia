namespace Project6RapidApi.Dtos.WeatherDtos
{
    public class WeatherResponseDto
    {
        public string name { get; set; } = "";
        public int cod { get; set; }
        public WeatherCoordDto coord { get; set; } = new();
        public List<WeatherDescDto> weather { get; set; } = new();
        public WeatherMainDto main { get; set; } = new();
        public WeatherWindDto wind { get; set; } = new();
        public WeatherSysDto sys { get; set; } = new();
        public int visibility { get; set; }
    }

    public class WeatherCoordDto
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }

    public class WeatherDescDto
    {
        public int id { get; set; }
        public string main { get; set; } = "";
        public string description { get; set; } = "";
        public string icon { get; set; } = "";
    }

    public class WeatherMainDto
    {
        public double temp { get; set; }
        public double feels_like { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public int pressure { get; set; }
        public int humidity { get; set; }
    }

    public class WeatherWindDto
    {
        public double speed { get; set; }
        public int deg { get; set; }
        public double gust { get; set; }
    }

    public class WeatherSysDto
    {
        public string country { get; set; } = "";
        public long sunrise { get; set; }
        public long sunset { get; set; }
    }
}