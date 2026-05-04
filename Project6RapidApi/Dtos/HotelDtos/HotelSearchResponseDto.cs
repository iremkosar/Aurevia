namespace Project6RapidApi.Dtos.HotelDtos
{
    public class HotelSearchResponseDto
    {
        public bool status {  get; set; }
        public string message { get; set; } = "";
        public HotelSearchDataDto data { get; set; } = new();
    }
    public class HotelSearchDataDto
    {
        public List<HotelSearchItemDto> hotels { get; set; } = new();
    }
    public class HotelSearchItemDto
    {
        public string accessibilityLabel { get; set; } = "";
        public HotelPropertyDto property { get; set; } = new();
    }
    public class HotelPropertyDto
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public int propertyClass { get; set; }
        public int accuratePropertyClass { get; set; }
        public double reviewScore { get; set; }
        public string reviewScoreWord { get; set; } = "";
        public int reviewCount { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string countryCode { get; set; } = "";
        public string currency { get; set; } = "";
        public string checkinDate { get; set; } = "";
        public string checkoutDate { get; set; } = "";
        public List<string> photoUrls { get; set; } = new();
        public HotelCheckinDto checkin { get; set; } = new();
        public HotelCheckoutDto checkout { get; set; } = new();
        public HotelPriceBreakdownDto priceBreakdown { get; set; } = new();
    }
    public class HotelCheckinDto
    {
        public string fromTime { get; set; } = "";
        public string untilTime { get; set; } = "";
    }
    public class HotelCheckoutDto
    {
        public string fromTime { get; set; } = "";
        public string untilTime { get; set; } = "";
    }
    public class HotelPriceBreakdownDto
    {
        public HotelGrossPriceDto grossPrice { get; set; } = new();
    }
    public class HotelGrossPriceDto
    {
        public decimal value { get; set; }
        public string currency { get; set; } = "";
    }
}
