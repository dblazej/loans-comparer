using dotNetAPI.Entity;
using Microsoft.EntityFrameworkCore;

namespace dotNetAPI.Repository
{
    public interface ICarStatusRepository
    {
        List<CarStatus> GetHistoryOfRentedCars(string MicrosoftID);
        Guid addCarStatus(CarStatus carStatus);
        CarStatus getCarHistoryById(Guid id);
        public List<CarStatus> GetHistoryOfAllRentedCars();
    }
}
