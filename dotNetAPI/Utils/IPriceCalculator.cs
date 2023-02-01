using dotNetAPI.DTO.Vehicle.Request;

namespace dotNetAPI.Utils
{
    public interface IPriceCalculator
    {
        public float calculatePrice(VehiclePriceRequest vehiclePriceRequest, int basePrice);
        public int convertDecimalPriceToInt(float price);
    }
}
