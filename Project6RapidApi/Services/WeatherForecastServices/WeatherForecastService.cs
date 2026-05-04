using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.WeatherDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.WeatherForecastServices
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;

        public WeatherForecastService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<WeatherResponseDto> GetWeatherAsync()
        {
            var client=_httpClientFactory.CreateClient();        
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://open-weather13.p.rapidapi.com/city?city=istanbul&lang=TR"),
                Headers =
                {
                    { "x-rapidapi-key", _apiSettings.Key },
                    { "x-rapidapi-host", "open-weather13.p.rapidapi.com" },
                },
                };
            using var response=await client.SendAsync(request);            
            response.EnsureSuccessStatusCode();
            var json=await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<WeatherResponseDto>(json)!;
        }
    }
}
