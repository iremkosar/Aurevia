using Project6RapidApi.Dtos.ReviewDtos;

namespace Project6RapidApi.Services.ReviewServices
{
    public interface IHotelReviewService
    {
        Task<List<HotelReviewItemDto>> GetReviewsAsync(int hotelId);
    }
}
