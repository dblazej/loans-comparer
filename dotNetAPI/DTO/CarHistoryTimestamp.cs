namespace dotNetAPI.DTO
{
    public class CarHistoryTimestampDTO
    {
        public string id { get; set; }
        public string ImageUrl { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public bool IsBeingRented { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        public CarHistoryTimestampDTO()
        {

        }
    }
}