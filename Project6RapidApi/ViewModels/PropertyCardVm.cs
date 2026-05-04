namespace Project6RapidApi.ViewModels
{
    public class PropertyCardVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal NightPrice { get; set; }
        public string Currency { get; set; } = "";
        public int Stars { get; set; }
        public double ReviewScore { get; set; }
        public string ReviewScoreWord { get; set; } = "";
        public int ReviewCount { get; set; }
    }
}
