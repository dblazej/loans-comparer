using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public interface IVehicleQuoteRepository
    {
        Guid addVehicleQuote(VehicleQuote vehicleQuote);
        VehicleQuote getVehicleQuoteById(Guid id);
    }
}
