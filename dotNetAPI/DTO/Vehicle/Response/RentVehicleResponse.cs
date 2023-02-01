namespace dotNetAPI.DTO.Vehicle.Response
{
    public class RentVehicleResponse
    {
        public string quoteId { get; set; }
        public string rentId { get; set; }
        public DateTime rentAt { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }

    }
}
