namespace Project6RapidApi.Dtos.DestinationDtos
{
    public class DestinationResponseDto
    {
        public List<DestinationItemDto> data { get; set; } = new();
    }
    public class DestinationItemDto
    {
        public string dest_id { get; set; } = "";
        public string dest_type { get; set; } = "";
        public string label { get; set; } = "";
        public string name { get; set; } = "";
        public string city_name { get; set; } = "";
        public string country { get; set; } = "";
        public string region { get; set; } = "";
        public string type { get; set; } = "";
        public double latitude { get; set; }
        public double longitude { get; set; } 
        public int nr_hotels { get; set; }
        public string image_url { get; set; } = "";

    }
}
