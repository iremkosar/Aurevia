namespace Project6RapidApi.Dtos.HotelDtos
{
    public class HotelDetailResponseDto
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public HotelDetailDataDto data { get; set; } = new();
    }
    public class HotelDetailDataDto
    {
        public int hotel_id { get; set; }
        public string hotel_name { get; set; } = "";
        public string url { get; set; } = "";
        public int review_nr { get; set; }
        public string arrival_date { get; set; } = "";
        public string departure_date { get; set; } = "";
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string address { get; set; } = "";
        public string city { get; set; } = "";
        public string district { get; set; } = "";
        public string country_trans { get; set; } = "";
        public string currency_code { get; set; } = "";
        public string zip { get; set; } = "";
        public List<string> family_facilities { get; set; } = new();
        public List<HotelHighlightDto> property_highlight_strip { get; set; } = new();
        public HotelFacilitiesBlockDto facilities_block { get; set; } = new();
        public List<HotelTopBenefitDto> top_ufi_benefits { get; set; } = new();
        public HotelRawDataDto rawData { get; set; } = new();
        public Dictionary<string, RoomListRoomDto> rooms { get; set; } = new();
        public List<HotelBlockDto> block { get; set; } = new();
        public HotelProductPriceDto composite_price_breakdown { get; set; } = new();
    }
    public class RoomListRoomDto
    {
        public string description { get; set; } = "";
        public List<RoomListPhotoDto> photos { get; set; } = new();
        public List<RoomHighlightDto> highlights { get; set; } = new();
    }

    public class RoomListPhotoDto
    {
        public string url_max1280 { get; set; } = "";
        public string url_max750 { get; set; } = "";
        public string url_original { get; set; } = "";
    }

    public class RoomHighlightDto
    {
        public string translated_name { get; set; } = "";
        public string icon { get; set; } = "";
    }
    public class HotelHighlightDto
    {
        public string name { get; set; } = "";
    }

    public class HotelFacilitiesBlockDto
    {
        public string name { get; set; } = "";
        public List<HotelFacilityDto> facilities { get; set; } = new();
    }

    public class HotelFacilityDto
    {
        public string name { get; set; } = "";
        public string icon { get; set; } = "";
    }

    public class HotelTopBenefitDto
    {
        public string translated_name { get; set; } = "";
        public string icon { get; set; } = "";
    }

    public class HotelRawDataDto
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public double reviewScore { get; set; }
        public string reviewScoreWord { get; set; } = "";
        public int reviewCount { get; set; }
        public int propertyClass { get; set; }
        public int accuratePropertyClass { get; set; }
        public List<string> photoUrls { get; set; } = new();
        public HotelRawCheckinDto checkin { get; set; } = new();
        public HotelRawCheckoutDto checkout { get; set; } = new();
        public HotelRawPriceBreakdownDto priceBreakdown { get; set; } = new();
    }

    public class HotelRawCheckinDto
    {
        public string fromTime { get; set; } = "";
        public string untilTime { get; set; } = "";
    }

    public class HotelRawCheckoutDto
    {
        public string fromTime { get; set; } = "";
        public string untilTime { get; set; } = "";
    }

    public class HotelRawPriceBreakdownDto
    {
        public HotelRawGrossPriceDto grossPrice { get; set; } = new();
    }

    public class HotelRawGrossPriceDto
    {
        public decimal value { get; set; }
        public string currency { get; set; } = "";
    }

    public class HotelBlockDto
    {
        public HotelProductPriceDto product_price_breakdown { get; set; } = new();
    }

    public class HotelProductPriceDto
    {
        public HotelRawGrossPriceDto gross_amount_per_night { get; set; } = new();
    }

}
