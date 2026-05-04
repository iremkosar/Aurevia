namespace Project6RapidApi.Dtos.RoomListDtos
{
    public class RoomListResponseDto
    {
        public bool status { get; set; }
        public RoomListDataDto? data { get; set; }
    }

    public class RoomListDataDto
    {
        public Dictionary<string, RoomListRoomDto> rooms { get; set; } = new();
    }

    public class RoomListRoomDto
    {
        public string description { get; set; } = "";
        public List<RoomListPhotoDto> photos { get; set; } = new();
    }

    public class RoomListPhotoDto
    {
        public string url_max1280 { get; set; } = "";
        public string url_max750 { get; set; } = "";
        public string url_original { get; set; } = "";
    }
}
