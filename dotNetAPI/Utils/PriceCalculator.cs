using dotNetAPI.DTO.Vehicle.Request;

namespace dotNetAPI.Utils
{
    public class PriceCalculator : IPriceCalculator
    {
        public float calculatePrice(VehiclePriceRequest vehiclePriceRequest, int basePrice)
        {
            float price = (float)basePrice / 100;
            return price;
        }

        public int convertDecimalPriceToInt(float price)
        {
            int convertedPrice = (int)(price * 100);
            return convertedPrice;
        }

    }
}
