using dotNetAPI.Entity;
using dotNetAPI.Enums;
using Microsoft.EntityFrameworkCore;

namespace dotNetAPI.Repository
{
    public class CarStatusRepository : ICarStatusRepository
    {
        private readonly CarRentalDbContext _db;

        public CarStatusRepository(CarRentalDbContext db)
        {
            _db = db;
        }

        public List<CarStatus> GetHistoryOfAllRentedCars()
        {
            return _db.CarStatus.Include(h => h.Car).ToList();
        }
        public List<CarStatus> GetHistoryOfRentedCars(string MicrosoftID)
        {
            return _db.CarStatus.Include(h => h.Car).Where(h => h.PerformedBy == MicrosoftID).ToList();
        }

        public Guid addCarStatus(CarStatus carStatus)
        {
            _db.CarStatus.Add(carStatus);
            _db.SaveChanges();

            return carStatus.Id;
        }

        public CarStatus getCarHistoryById(Guid id)
        {
            try
            {
                return _db.CarStatus
                .Where(carStatus => carStatus.Id == id)
                .Include(carStatus => carStatus.Car)
                .Single();
            }
            catch (InvalidOperationException ex)
            {
                return null;
            }
        }
    }
}
