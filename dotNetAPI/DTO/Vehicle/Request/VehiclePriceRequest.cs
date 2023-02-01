namespace dotNetAPI.DTO.Vehicle.Request
{
    public class VehiclePriceRequest
    {
        public int age { get; set; }
        public int yearsOfHavingDriverLicense { get; set; }
        public int rentDuration { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public int currentlyRentedCount { get; set; }
        public int overallRenetdCount { get; set; }
        public string companyName { get; set; }
    }
}
