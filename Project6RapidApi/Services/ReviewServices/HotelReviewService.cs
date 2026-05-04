using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Project6RapidApi.Dtos.ReviewDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.ReviewServices
{
    public class HotelReviewService : IHotelReviewService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;

        public HotelReviewService(IHttpClientFactory httpClientFactory, IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _apiSettings = apiSettings.Value;
        }

        public async Task<List<HotelReviewItemDto>> GetReviewsAsync(int hotelId)
        {
            try
            {
                var url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/getHotelReviews" +
           $"?hotel_id={hotelId}" +
           $"&sort_option_id=sort_most_relevant" +
           $"&page_number=1" +
           $"&languagecode=en-us";

                var client = _httpClientFactory.CreateClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("x-rapidapi-key", _apiSettings.Key);
                request.Headers.Add("x-rapidapi-host", "booking-com15.p.rapidapi.com");

                var response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new();

                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<HotelReviewResponseDto>(json);
                return result?.data?.result ?? new();
            }
            catch { return new(); }
        }
    }
}