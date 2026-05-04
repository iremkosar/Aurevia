using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.DestinationDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.Destination
{
    public class DestinationService : IDestinationService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;

        public DestinationService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<DestinationResponseDto> SearchDestinationAsync(string query)
        {

            var url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/searchDestination?query={Uri.EscapeDataString(query)}";
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "x-rapidapi-key", _apiSettings.Key },
                     { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                },
            };
            using var response=await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json=await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DestinationResponseDto>(json)!;          
        }
    }
}
