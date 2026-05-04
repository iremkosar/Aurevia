using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Project6RapidApi.Dtos.RoomListDtos;
using Project6RapidApi.Settings;

namespace Project6RapidApi.Services.RoomListServices;

public class RoomListService : IRoomListService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly RapidApiSettings _apiSettings;
    private readonly IMemoryCache _cache;

    public RoomListService(IHttpClientFactory httpClientFactory, IMemoryCache cache,
    IOptions<RapidApiSettings> apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _apiSettings = apiSettings.Value;
    }

    public async Task<List<string>> GetRoomPhotosAsync(int hotelId, DateTime checkin, DateTime checkout, int adults, int roomQty)
    {
        var photos = new List<string>();

        try
        {
            var client = _httpClientFactory.CreateClient();
            var checkinStr = checkin.ToString("yyyy-MM-dd");
            var checkoutStr = checkout.ToString("yyyy-MM-dd");

            var url = $"https://booking-com15.p.rapidapi.com/api/v1/hotels/getRoomList" +
                      $"?hotel_id={hotelId}" +
                      $"&arrival_date={checkinStr}" +
                      $"&departure_date={checkoutStr}" +
                      $"&adults={adults}" +
                      $"&room_qty={roomQty}" +
                      $"&currency_code=USD" +
                      $"&languagecode=en-us";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("x-rapidapi-key", _apiSettings.Key);
            request.Headers.Add("x-rapidapi-host", "booking-com15.p.rapidapi.com");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode) return photos;

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<RoomListResponseDto>(json, options);

            if (result?.data?.rooms == null) return photos;

            var seen = new HashSet<string>();
            foreach (var room in result.data.rooms.Values)
            {
                foreach (var photo in room.photos)
                {
                    var url2 = !string.IsNullOrEmpty(photo.url_max1280) ? photo.url_max1280
                             : !string.IsNullOrEmpty(photo.url_max750) ? photo.url_max750
                             : photo.url_original;

                    if (!string.IsNullOrEmpty(url2) && seen.Add(url2))
                        photos.Add(url2);
                }
            }
        }
        catch { /* sessizce boş döndür */ }

        return photos;
    }
}