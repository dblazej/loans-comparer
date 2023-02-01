using dotNetAPI.DTO;
using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public interface ICarRepository
    {
        List<Car> GetAllCars();
        void AddCar(Car car);
        Car GetCarById(Guid Id);
        Car GetCarByBrandAndModel(string brand, string model);
        void UpdateCar(Car car);
    }
}
