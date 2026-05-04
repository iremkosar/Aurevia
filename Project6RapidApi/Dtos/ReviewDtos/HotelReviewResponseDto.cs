namespace Project6RapidApi.Dtos.ReviewDtos
{
    public class HotelReviewResponseDto
    {
        public bool status { get; set; }
        public HotelReviewDataDto data { get; set; } = new();
    }

    public class HotelReviewDataDto
    {
        public int count { get; set; }
        public List<HotelReviewItemDto> result { get; set; } = new();
    }

    public class HotelReviewItemDto
    {
        public string title { get; set; } = "";
        public string pros { get; set; } = "";
        public string cons { get; set; } = "";
        public double average_score { get; set; }
        public string date { get; set; } = "";
        public HotelReviewAuthorDto author { get; set; } = new();
    }

    public class HotelReviewAuthorDto
    {
        public string name { get; set; } = "";
        public string type_string { get; set; } = "";
        public string avatar { get; set; } = "";
        public string countrycode { get; set; } = "";
    }
}