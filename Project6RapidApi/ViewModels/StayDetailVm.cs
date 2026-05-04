using Project6RapidApi.Dtos.ReviewDtos;

namespace Project6RapidApi.ViewModels
{
    public class StayDetailVm
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Stars { get; set; }
        public double ReviewScore { get; set; }
        public string ReviewScoreWord { get; set; } = "";
        public int ReviewCount { get; set; }
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; } = "";
        public string CheckinTime { get; set; } = "";
        public string CheckoutTime { get; set; } = "";
        public List<string> GalleryUrls { get; set; } = new();
        public List<string> Facilities { get; set; } = new();
        public string Description { get; set; } = "";
        public string BookingUrl { get; set; } = "";
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Rooms { get; set; }
        public List<string> Highlights { get; set; } = new();
        public List<HotelReviewItemDto> Reviews { get; set; } = new();
    }
}
