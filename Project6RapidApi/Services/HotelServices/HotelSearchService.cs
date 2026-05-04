using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Project6RapidApi.Dtos.HotelDtos;
using Project6RapidApi.Settings;
using Project6RapidApi.ViewModels;

namespace Project6RapidApi.Services.HotelServices
{
    public class HotelSearchService : IHotelSearchService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;

        public HotelSearchService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _apiSettings = apiSettings.Value;
        }

        public async Task<List<PropertyCardVm>> SearchHotelsAsync(
            string destId,
            string searchType,
            DateTime checkin,
            DateTime checkout,
            int adults,
            int children,
            int rooms)
        {
            var childrenAge = children > 0
                ? string.Join(",", Enumerable.Repeat(10, children))
                : "";

            var query = new List<string>
            {
                $"dest_id={destId}",
                 $"search_type={searchType}",
                $"arrival_date={checkin:yyyy-MM-dd}",
                $"departure_date={checkout:yyyy-MM-dd}",
                $"adults={adults}",
                $"room_qty={rooms}",
                "page_number=1",
                "units=metric",
                "temperature_unit=c",
                "languagecode=en-us",
                "currency_code=USD",
            };
            if (children > 0)
                query.Add($"children_age={Uri.EscapeDataString(childrenAge)}");

            var url = "https://booking-com15.p.rapidapi.com/api/v1/hotels/searchHotels?" + string.Join("&", query);
            var client=_httpClientFactory.CreateClient();
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
            var result = JsonConvert.DeserializeObject<HotelSearchResponseDto>(json);

            var nights = Math.Max(1, (checkout - checkin).Days);
            var cards = new List<PropertyCardVm>();

            foreach (var item in result.data.hotels)
            {
                var p = item.property;
                var price = p.priceBreakdown.grossPrice.value;
                var nightPrice = Math.Round(price / nights, 2);
                var stars = p.accuratePropertyClass > 0
                    ? p.accuratePropertyClass
                    : p.propertyClass;
                var photo = p.photoUrls.Count > 0
                    ? p.photoUrls[0].Replace("square60", "max500")
                    : "";
                cards.Add(new PropertyCardVm 
                { 
                    Id = p.id,
                    Name = p.name,
                    ImageUrl = photo,
                    NightPrice = nightPrice,
                    Currency = p.priceBreakdown.grossPrice.currency,
                    Stars = stars,
                    ReviewScore = p.reviewScore,
                    ReviewScoreWord = p.reviewScoreWord,
                    ReviewCount = p.reviewCount,
                });
            }

            return cards;
        }
        }
}
