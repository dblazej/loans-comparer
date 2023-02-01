namespace dotNetAPI.DTO.VehicleResponse
{
    public class VehiclesResponse
    {
        public VehiclesResponse()
        {
            vehicles = new List<CarDTO>();
        }

        public int vehiclesCount { get; set; }
        public DateTime generatedDate { get; set; }
        public List<CarDTO> vehicles { get; set; }


    }
}
