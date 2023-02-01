namespace dotNetAPI.DTO.Vehicle.Request
{
    public class VehiclePriceFrontendRequest
    {
        public VehiclePriceFrontendRequest()
        {
        }
        public string MicrosoftId { get; set; }
        public string CompanyName { get; set; }
        public int RentDuration { get; set; }
    }
}
