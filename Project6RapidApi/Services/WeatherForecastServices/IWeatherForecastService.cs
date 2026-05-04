using Project6RapidApi.Dtos.WeatherDtos;

namespace Project6RapidApi.Services.WeatherForecastServices
{
    public interface IWeatherForecastService
    {
        Task<WeatherResponseDto> GetWeatherAsync();
    }
}
