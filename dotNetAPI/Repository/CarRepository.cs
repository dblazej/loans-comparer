using dotNetAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace dotNetAPI.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly CarRentalDbContext _db;

        public CarRepository(CarRentalDbContext db)
        {
            _db = db;
        }

        public List<Car> GetAllCars()
        {
            return _db.Car.ToList();
        }

        public void AddCar(Car car)
        {
            _db.Car.Add(car);
            _db.SaveChanges();
        }

        public Car GetCarById(Guid id)
        {
            return _db.Car.Where(car => car.Id == id).SingleOrDefault();
        }

        public Car GetCarByBrandAndModel(string brand, string model)
        {
            return _db.Car.Where(car => car.Brand == brand && car.Model == model).FirstOrDefault();
        }

        public void UpdateCar(Car car)
        {
            _db.Car.Update(car);
            _db.SaveChanges();
        }
    }
}
