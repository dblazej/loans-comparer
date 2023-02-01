using dotNetAPI.DTO;
using dotNetAPI.DTO.Vehicle.Request;
using dotNetAPI.DTO.Vehicle.Response;
using dotNetAPI.DTO.VehicleResponse;

namespace dotNetAPI.Service
{
    public interface ICarService
    {
        VehiclesResponse GetAllCars();
        void AddCar(AddCarDTO addCarDTO);
        public VehiclePriceResponse GetCarPriceById(VehiclePriceFrontendRequest request, string id);
        public VehiclePriceResponse GetCarPriceByBrandAndModel(VehiclePriceFrontendRequest request, string brand, string model);
        public RentVehicleResponse RentCar(RentVehicleRequest rentVehicleRequest, string quoteId, string MicrosoftID);
        public void ReturnCar(string rentId);
    }
}
