namespace dotNetAPI.DTO.Vehicle.Response
{
    public class VehiclePriceResponse
    {
        public float price { get; set; }
        public string currency { get; set; }
        public DateTime generatedAt { get; set; }
        public DateTime expiredAt { get; set; }
        public string quotaId { get; set; }
    }
}
