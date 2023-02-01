using dotNetAPI.DTO;
using dotNetAPI.Entity;
using dotNetAPI.Repository;

namespace dotNetAPI.Service
{
    public interface ICarStatusService
    {
        List<CarHistoryTimestampDTO> GetHistoryOfRentedCarsTimestamps(string MicrosoftID);
        List<CarHistoryTimestampDTO> GetRentedCarsTimestamps(string MicrosoftID);
        int CountActuallyRentedCarsFromRentedHistory(List<CarHistoryTimestampDTO> carHistoryTimestampDTOs);
        int CountOverallRentedCarsFromRentedHistory(List <CarHistoryTimestampDTO> carHistoryTimestamps);

        List<CarRentalDTO> GetHistoryOfCarRentals(string MicrosoftID);
        List<CarRentalDTO> GetCarRentals(string MicrosoftID);
        List<CarRentalAdminDTO> GetHistoryOfAllCarRentals();
        List<CarRentalAdminDTO> GetAllCarRentals();

    }
}
