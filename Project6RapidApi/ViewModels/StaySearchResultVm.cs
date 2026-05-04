namespace Project6RapidApi.ViewModels
{
    public class StaySearchResultVm
    {
        public string City { get; set; } = "";
        public string DestinationLabel { get; set; } = "";
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Rooms { get; set; }
        public List<PropertyCardVm> Properties { get; set; } = new();
    }
}
