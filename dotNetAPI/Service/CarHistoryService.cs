using dotNetAPI.DTO;
using dotNetAPI.Entity;
using dotNetAPI.Enums;
using dotNetAPI.Repository;

namespace dotNetAPI.Service
{
    public class CarHistoryService : ICarStatusService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICarStatusRepository _carHistoryRepository;

        public CarHistoryService(ICarRepository carRepository,ICarStatusRepository carHistoryRepository)
        {
            _carRepository = carRepository;
            _carHistoryRepository = carHistoryRepository;
        }
        public List<CarHistoryTimestampDTO> GetHistoryOfRentedCarsTimestamps(string MicrosoftID)
        {
            List<CarStatus> carHistoryList = _carHistoryRepository.GetHistoryOfRentedCars(MicrosoftID);

            List<CarHistoryTimestampDTO> carHistoryTimestampDTOs = new List<CarHistoryTimestampDTO>();

            foreach(CarStatus status in carHistoryList)
            {
                CarHistoryTimestampDTO timestamp = new CarHistoryTimestampDTO();
                timestamp.id = status.Car.Id.ToString();
                timestamp.ImageUrl = status.Car.ImageUrl;
                timestamp.Brand = status.Car.Brand;
                timestamp.Model = status.Car.Model;
                timestamp.Year = status.Car.Year;
                timestamp.IsBeingRented = (status.Action == CarStatusEnum.RENTED.ToString()) ? true:false;
                timestamp.Date = status.ActionDateTime;
                timestamp.Note = status.Note;

                carHistoryTimestampDTOs.Add(timestamp);
            }

            carHistoryTimestampDTOs.Sort((x, y) => y.Date.CompareTo(x.Date));

            return carHistoryTimestampDTOs;
        }
        public List<CarHistoryTimestampDTO> GetRentedCarsTimestamps(string MicrosoftID)
        {
            List<CarHistoryTimestampDTO> carHistoryTimestampDTOs = GetHistoryOfRentedCarsTimestamps(MicrosoftID);

            List<CarHistoryTimestampDTO> result = new List<CarHistoryTimestampDTO>();

            //carHistoryTimestampDTOs.Sort((x, y) => x.Date.CompareTo(y.Date));

            var cars = carHistoryTimestampDTOs.Select(x => x.id).Distinct();

            foreach (var car in cars)
            {
                var timestamp = carHistoryTimestampDTOs.Find(x => x.id == car);
                if(timestamp != null && timestamp.IsBeingRented)
                {
                    result.Add(timestamp);
                }
            }
            return result;
        }

        public int CountActuallyRentedCarsFromRentedHistory(List<CarHistoryTimestampDTO> carHistoryTimestampDTOs) // żeby nie wykonywać ponownie tych samych operacji, jedynie zliczenie
        {
            int result = 0;

            var cars = carHistoryTimestampDTOs.Select(x => x.id).Distinct();

            foreach (var car in cars)
            {
                var timestamp = carHistoryTimestampDTOs.Find(x => x.id == car);
                if (timestamp != null && timestamp.IsBeingRented)
                {
                    result++;
                }
            }
            return result;
        }
        public int CountOverallRentedCarsFromRentedHistory(List<CarHistoryTimestampDTO> carHistoryTimestampDTOs)
        {
            int result = 0;

            foreach (var timestamp in carHistoryTimestampDTOs)
            {
                if (timestamp.IsBeingRented)
                {
                    result++;
                }
            }
            return result;
        }

        public List<CarRentalDTO> GetHistoryOfCarRentals(string MicrosoftID)
        {
            var timestampList = GetHistoryOfRentedCarsTimestamps(MicrosoftID);
            var timestampsDictionary = new Dictionary<string, List<CarHistoryTimestampDTO>>();

            var result = new List<CarRentalDTO>();

            foreach (var timestamp in timestampList)
            {
                if (timestampsDictionary.ContainsKey(timestamp.id))
                {
                    timestampsDictionary[timestamp.id].Add(timestamp);
                }
                else
                {
                    var list = new List<CarHistoryTimestampDTO>();
                    list.Add(timestamp);
                    timestampsDictionary.Add(timestamp.id, list);
                }
            }

            foreach(var carID in timestampsDictionary.Keys)
            {
                CarRentalDTO rental = null;
                var carTimestamps = timestampsDictionary[carID];
                foreach (var timestamp in carTimestamps)
                {
                    if(timestamp.IsBeingRented == true)
                    {
                        rental = new CarRentalDTO();
                        rental.CarId = carID;
                        rental.rentDate = timestamp.Date;
                    }
                    else
                    {
                        if (rental == null) continue;
                        if (rental.rentDate == default(DateTime)) continue;
                        rental.returnDate = timestamp.Date;
                        result.Add(rental);
                        rental = new CarRentalDTO();
                    }
                }
            }

            return result.OrderBy(x => x.rentDate).ToList();
        }

        public List<CarRentalDTO> GetCarRentals(string MicrosoftID)
        {
            var timestampList = GetRentedCarsTimestamps(MicrosoftID);
            var timestampsDictionary = new Dictionary<string, List<CarHistoryTimestampDTO>>();

            var result = new List<CarRentalDTO>();

            foreach (var timestamp in timestampList)
            {
                if (timestampsDictionary.ContainsKey(timestamp.id))
                {
                    timestampsDictionary[timestamp.id].Add(timestamp);
                }
                else
                {
                    var list = new List<CarHistoryTimestampDTO>();
                    list.Add(timestamp);
                    timestampsDictionary.Add(timestamp.id, list);
                }
            }

            foreach (var carID in timestampsDictionary.Keys)
            {
                CarRentalDTO rental = null;
                var carTimestamps = timestampsDictionary[carID];
                foreach (var timestamp in carTimestamps)
                {
                    if (timestamp.IsBeingRented == true)
                    {
                        rental = new CarRentalDTO();
                        rental.CarId = carID;
                        rental.rentDate = timestamp.Date;
                        rental.returnDate = _carRepository.GetCarById(Guid.Parse(carID)).ReturnDate;
                        result.Add(rental);
                    }
                }
            }

            return result.OrderBy(x => x.rentDate).ToList();
        }


        private Dictionary<Guid, List<CarStatus>> CreateCarStatusDictionaryKeyedByCarId(List<CarStatus> statusList)
        {
            var timestampsDictionary = new Dictionary<Guid, List<CarStatus>>();

            foreach (var timestamp in statusList)
            {
                if (timestampsDictionary.ContainsKey(timestamp.Car.Id))
                {
                    timestampsDictionary[timestamp.Car.Id].Add(timestamp);
                }
                else
                {
                    var list = new List<CarStatus>();
                    list.Add(timestamp);
                    timestampsDictionary.Add(timestamp.Car.Id, list);
                }
            }
            return timestampsDictionary;
        }

        public List<CarRentalAdminDTO> GetHistoryOfAllCarRentals()
        {
            var timestampList = _carHistoryRepository.GetHistoryOfAllRentedCars();

            var timestampsDictionary = CreateCarStatusDictionaryKeyedByCarId(timestampList);
            var result = new List<CarRentalAdminDTO>();

            foreach (var carID in timestampsDictionary.Keys)
            {
                CarRentalAdminDTO rental = null;
                var carTimestamps = timestampsDictionary[carID];
                foreach (var timestamp in carTimestamps)
                {
                    if (timestamp.Action == CarStatusEnum.RENTED.ToString())
                    {
                        rental = new CarRentalAdminDTO();
                        rental.CarId = carID.ToString();
                        rental.RentId = timestamp.Id.ToString();
                        rental.rentDate = timestamp.ActionDateTime;
                    }
                    else
                    {
                        if (rental == null) continue;
                        if (rental.rentDate == default(DateTime)) continue;
                        rental.returnDate = timestamp.ActionDateTime;
                        result.Add(rental);
                        rental = new CarRentalAdminDTO();
                    }
                }
            }

            return result.OrderBy(x => x.rentDate).ToList();
        }

        public List<CarRentalAdminDTO> GetAllCarRentals()
        {
            var timestampList = _carHistoryRepository.GetHistoryOfAllRentedCars();

            var timestampsDictionary = CreateCarStatusDictionaryKeyedByCarId(timestampList);
            var result = new List<CarRentalAdminDTO>();


            foreach (var carID in timestampsDictionary.Keys)
            {
                var carTimestamps = timestampsDictionary[carID];
                
                var last = carTimestamps.OrderBy(x => x.ActionDateTime).Last();
                if(last.Action == CarStatusEnum.RENTED.ToString())
                {
                    CarRentalAdminDTO rental = new CarRentalAdminDTO()
                    {
                        CarId = carID.ToString(),
                        rentDate = last.ActionDateTime,
                        RentId = last.Id.ToString()
                    };
                    result.Add(rental);
                }
            }

            return result;
        }
    }
}
