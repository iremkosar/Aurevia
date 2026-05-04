using Newtonsoft.Json;
using Project6RapidApi.ViewModels;
using Project6RapidApi.Dtos.HotelDtos;
using Project6RapidApi.Services.RoomListServices;
using Project6RapidApi.Settings;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Project6RapidApi.Services.ReviewServices;

namespace Project6RapidApi.Services.HotelServices
{
    public class HotelDetailService : IHotelDetailService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRoomListService _roomListService;
        private readonly RapidApiSettings _apiSettings;
        private readonly IMemoryCache _cache;
        private readonly IHotelReviewService _reviewService;

        public HotelDetailService(IHttpClientFactory httpClientFactory, IRoomListService roomListService, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings, IHotelReviewService reviewService)
        {
            _httpClientFactory = httpClientFactory;
            _roomListService = roomListService;
            _cache = cache;
            _apiSettings = apiSettings.Value;
            _reviewService = reviewService;
        }

        public async Task<StayDetailVm> GetHotelDetailAsync(
            int hotelId,
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
                $"hotel_id={hotelId}",
                $"arrival_date={checkin:yyyy-MM-dd}",
                $"departure_date={checkout:yyyy-MM-dd}",
                $"adults={adults}",
                $"room_qty={rooms}",
                "units=metric",
                "temperature_unit=c",
                "languagecode=en-us",
                "currency_code=USD",
            };
            if (children > 0)
                query.Add($"children_age={Uri.EscapeDataString(childrenAge)}");

            var url = "https://booking-com15.p.rapidapi.com/api/v1/hotels/getHotelDetails?" + string.Join("&", query);

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

            using var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<HotelDetailResponseDto>(json);
            var d = result.data;
            var raw = d.rawData;

            var pricePerNight = d.composite_price_breakdown?.gross_amount_per_night?.value ?? 0;
            var currency = d.composite_price_breakdown?.gross_amount_per_night?.currency ?? "USD";

            var facilities = d.facilities_block.facilities
                .Select(f => f.name)
                .ToList();

            var highlights = d.property_highlight_strip
                .Select(h => h.name)
                .ToList();

            // Önce oda fotoğraflarını dene, yoksa hotel fotoğraflarını kullan
            var roomPhotos = await _roomListService.GetRoomPhotosAsync(hotelId, checkin, checkout, adults, rooms);

            var galleryUrls = roomPhotos.Any()
                ? roomPhotos
                : raw.photoUrls.Select(u => u.Replace("square60", "max500")).ToList();

            var reviews = await _reviewService.GetReviewsAsync(hotelId);

            return new StayDetailVm
            {
                HotelId = d.hotel_id,
                Name = d.hotel_name,
                Address = d.address,
                City = d.city,
                Country = d.country_trans,
                Latitude = d.latitude,
                Longitude = d.longitude,
                Stars = raw.accuratePropertyClass > 0
                    ? raw.accuratePropertyClass
                    : raw.propertyClass,
                ReviewScore = raw.reviewScore,
                ReviewScoreWord = raw.reviewScoreWord,
                ReviewCount = d.review_nr,
                PricePerNight = pricePerNight,
                Currency = currency,
                CheckinTime = raw.checkin.fromTime,
                CheckoutTime = raw.checkout.untilTime,
                GalleryUrls = galleryUrls,
                Facilities = facilities,
                Highlights = highlights,
                BookingUrl = d.url,
                Checkin = checkin,
                Checkout = checkout,
                Adults = adults,
                Children = children,
                Rooms = rooms,
                Reviews = reviews,
            };
        }
    }
}