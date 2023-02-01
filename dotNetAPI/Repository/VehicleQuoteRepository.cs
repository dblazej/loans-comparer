using dotNetAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace dotNetAPI.Repository
{
    public class VehicleQuoteRepository : IVehicleQuoteRepository
    {
        private readonly CarRentalDbContext _db;

        public VehicleQuoteRepository(CarRentalDbContext db)
        {
            _db = db;
        }
        public Guid addVehicleQuote(VehicleQuote vehicleQuote)
        {
            _db.VehicleQuote.Add(vehicleQuote);
            _db.SaveChanges();

            return vehicleQuote.id;
        }

        public VehicleQuote getVehicleQuoteById(Guid id)
        {
            try
            {
                var vehicleQuote = _db.VehicleQuote
                    .Where(vehicleQuote => vehicleQuote.id == id)
                    .Include(vehicleQuote => vehicleQuote.car)
                    .Single();
                return vehicleQuote;
            }
            catch(InvalidOperationException ex)
            {
                return null;
            }
        }
    }
}
