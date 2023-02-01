using dotNetAPI.DTO;
using dotNetAPI.DTO.Vehicle.Request;
using dotNetAPI.DTO.Vehicle.Response;
using dotNetAPI.DTO.VehicleResponse;
using dotNetAPI.Entity;
using dotNetAPI.Enums;
using dotNetAPI.Exceptions;
using dotNetAPI.Repository;
using dotNetAPI.Utils;

namespace dotNetAPI.Service
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICarStatusRepository _carStatusRepository;
        private readonly IUserRepository _userRepository;

        private readonly ICarStatusService _carStatusService;

        private readonly IPriceCalculator _priceCalculator;
        private readonly IVehicleQuoteRepository _vehicleQuoteRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly BasicModel _basicModel;
        private readonly MailSender _mailSender;

        public CarService(ICarRepository carRepository, ICompanyRepository companyRepository, ICarStatusRepository carStatusRepository, IPriceCalculator priceCalculator, IVehicleQuoteRepository vehicleQuoteRepository, IHttpClientFactory httpClientFactory, IUserRepository userRepository, ICarStatusService carStatusService)
        {
            _carRepository = carRepository;
            _companyRepository = companyRepository;
            _carStatusRepository = carStatusRepository;
            _userRepository = userRepository;
            _carStatusService = carStatusService;
            _priceCalculator = priceCalculator;
            _vehicleQuoteRepository = vehicleQuoteRepository;
            _httpClientFactory = httpClientFactory;
            _basicModel = new BasicModel(httpClientFactory, "https://mini.rentcar.api.snet.com.pl");
            _mailSender = new MailSender();
        }

        public VehiclesResponse GetAllCars()
        {
            VehiclesResponse response = new VehiclesResponse();
            List<Car> cars = _carRepository.GetAllCars();
            int count = 0;
            response.vehicles = new List<CarDTO>();
            foreach (Car car in cars)
            {
                CarDTO carDTO = new CarDTO();
                carDTO.id = car.Id.ToString();
                carDTO.brandName = car.Brand;
                carDTO.modelName = car.Model;
                carDTO.year = car.Year;
                carDTO.enginePower = car.Year;
                carDTO.enginePowerType = car.EnginePowerType;
                carDTO.capacity = car.Capacity;
                carDTO.description = car.Description;
                carDTO.ImageUrl = car.ImageUrl;
                carDTO.currency = car.Currency;
                carDTO.company = CompaniesEnum.OUR.ToString();

                response.vehicles.Add(carDTO);
                count++;
            }

            response.vehiclesCount = count;
            response.generatedDate = DateTime.Now;

            appendCarsFromBossAPI(response);

            return response;
        }

        private void appendCarsFromBossAPI(VehiclesResponse response)
        {
            VehiclesResponse responseBoss = _basicModel.getVehicles();
            responseBoss.vehicles.ForEach(vehicle =>
            {
                vehicle.company = CompaniesEnum.BOSS.ToString();
            });
            response.vehicles.AddRange(responseBoss.vehicles);
            response.vehiclesCount = response.vehiclesCount + responseBoss.vehiclesCount;
        }


        public void AddCar(AddCarDTO addCarDTO)
        {
            Car car = new Car();
            car.Brand = addCarDTO.brandName;
            car.Model = addCarDTO.modelName;
            car.Year = addCarDTO.year;
            car.Description = addCarDTO.description;
            car.BasePrice = _priceCalculator.convertDecimalPriceToInt(addCarDTO.price);
            car.EnginePower = addCarDTO.enginePower;
            car.EnginePowerType = addCarDTO.enginePowerType;
            car.ImageUrl = addCarDTO.ImageUrl;
            car.Capacity = addCarDTO.capacity;
            car.Currency = addCarDTO.currency;
            car.IsRented = false;

        _carRepository.AddCar(car);
        }

        public VehiclePriceResponse GetCarPriceById(VehiclePriceFrontendRequest frontendRequest, string id)
        {
            var vehiclePriceRequest = GenerateVehiclePriceRequestFromUserData(frontendRequest);

            if (vehiclePriceRequest.companyName.Equals(CompaniesEnum.OUR.ToString()))
            {
                Car car = _carRepository.GetCarById(Guid.Parse(id));
                if(car == null) throw new CarDoesntExistException();

                return prepareVehiclePriceResponse(vehiclePriceRequest, car);
            }
            else
            {
                return _basicModel.getPriceById(vehiclePriceRequest, id);

            }
        }

        public VehiclePriceResponse GetCarPriceByBrandAndModel(VehiclePriceFrontendRequest frontendRequest, string brand, string model)
        {
            var vehiclePriceRequest = GenerateVehiclePriceRequestFromUserData(frontendRequest);

            if (vehiclePriceRequest.companyName.Equals(CompaniesEnum.OUR.ToString()))
            {
                Car car = _carRepository.GetCarByBrandAndModel(brand, model);

                return prepareVehiclePriceResponse(vehiclePriceRequest, car);
            }
            else
            {
                return _basicModel.getPriceByBrandAndModel(vehiclePriceRequest, brand, model);
            }
        }

        public RentVehicleResponse RentCar(RentVehicleRequest rentVehicleRequest, string quoteId, string MicrosoftID)
        {
            VehicleQuote vehicleQuote = _vehicleQuoteRepository.getVehicleQuoteById(Guid.Parse(quoteId));

            if (vehicleQuote != null)
            {
                DateTime currentDateTime = rentVehicleRequest.startDate;
                DateTime returnDateTime = currentDateTime.AddDays(vehicleQuote.rentDuration);

                if (vehicleQuote.car.IsRented)
                {
                    throw new CarIsRentedException();
                }

                Guid rentId = addCarStatus(vehicleQuote.car, currentDateTime, MicrosoftID);

                vehicleQuote.car.IsRented = true;
                vehicleQuote.car.ReturnDate = returnDateTime;
                _carRepository.UpdateCar(vehicleQuote.car);

                RentVehicleResponse rentVehicleResponse = new RentVehicleResponse();
                rentVehicleResponse.quoteId = quoteId;
                rentVehicleResponse.rentId = rentId.ToString();
                rentVehicleResponse.rentAt = currentDateTime;
                rentVehicleResponse.startDate = currentDateTime;
                rentVehicleResponse.endDate = returnDateTime;

                return rentVehicleResponse;
            }
            else
            {
                return _basicModel.RentCar(rentVehicleRequest, quoteId);
            }
        }

        public async void ReturnCar(string rentId)
        {
            CarStatus carStatus = _carStatusRepository.getCarHistoryById(Guid.Parse(rentId));
            User user = _userRepository.GetUserByID(carStatus.PerformedBy);

            if (carStatus != null)
            {
                if (!carStatus.Car.IsRented)
                {
                    throw new ArgumentException();
                }

                Car car = carStatus.Car;
                car.IsRented = false;
                _carRepository.UpdateCar(car);

                CarStatus returnCarStatus = new CarStatus();
                returnCarStatus.Car = car; ;
                returnCarStatus.Action = CarStatusEnum.RETURNED.ToString();
                returnCarStatus.ActionDateTime = DateTime.Now;
                returnCarStatus.PerformedBy = "hardcoded";
                returnCarStatus.Note = "hardcoded";

                _carStatusRepository.addCarStatus(returnCarStatus);
                await _mailSender.SendCarReturnMail(user.Email, car.Id.ToString());
            }
            else{
                _basicModel.ReturnCar(rentId);
            }
        }

        private VehiclePriceResponse prepareVehiclePriceResponse(VehiclePriceRequest vehiclePriceRequest, Car car)
        {
            VehiclePriceResponse vehiclePriceResponse = new VehiclePriceResponse();

            VehicleQuote vehicleQuote = new VehicleQuote();

            int basePrice = car.BasePrice;
            float price = _priceCalculator.calculatePrice(vehiclePriceRequest, basePrice);
            DateTime currentDateTime = DateTime.Now;
            DateTime expirationDateTime = DateTime.Now.AddHours(1);

            vehicleQuote.car = car;
            vehicleQuote.currency = car.Currency;
            vehicleQuote.price = price;
            vehicleQuote.expiredAt = expirationDateTime;
            vehicleQuote.generatedAt = currentDateTime;
            vehicleQuote.rentDuration = vehiclePriceRequest.rentDuration;

            string quoteId = _vehicleQuoteRepository.addVehicleQuote(vehicleQuote).ToString();

            vehiclePriceResponse.price = price;
            vehiclePriceResponse.currency = car.Currency;
            vehiclePriceResponse.generatedAt = currentDateTime;
            vehiclePriceResponse.expiredAt = expirationDateTime;
            vehiclePriceResponse.quotaId = quoteId;

            return vehiclePriceResponse;
        }

        private Guid addCarStatus(Car car, DateTime currentDateTime, string MicrosoftID)
        {
            CarStatus carHistory = new CarStatus();

            carHistory.Car = car;
            carHistory.Action = CarStatusEnum.RENTED.ToString();
            carHistory.ActionDateTime = currentDateTime;
            carHistory.PerformedBy = MicrosoftID;
            carHistory.Note = "";

            return _carStatusRepository.addCarStatus(carHistory);
        }

        private bool UserFilledHisData(User user)
        {
            if (user.YearOfBirth == default(int)) return false;  // chyba sensowniejszym polem jest data urodzenia niż podanie ile ma się lat?
            if (user.City == string.Empty) return false;
            if(user.Country == string.Empty) return false;
            if (user.Email == string.Empty) return false;
            return true;
        }

        private VehiclePriceRequest CreateVehiclePriceRequest(VehiclePriceFrontendRequest request, User user)
        {
            List<CarHistoryTimestampDTO> carHistoryTimestampDTOs = _carStatusService.GetHistoryOfRentedCarsTimestamps(user.MicrosoftID);

            var currentlyRentedCount = _carStatusService.CountActuallyRentedCarsFromRentedHistory(carHistoryTimestampDTOs);
            var overallRentedCount = _carStatusService.CountOverallRentedCarsFromRentedHistory(carHistoryTimestampDTOs);

            var vehiclePriceRequest = new VehiclePriceRequest()
            {
                age = DateTime.Now.Year - user.YearOfBirth,
                city = user.City,
                country = user.Country,
                currentlyRentedCount = currentlyRentedCount,
                overallRenetdCount = overallRentedCount,
                companyName = request.CompanyName,
                rentDuration = request.RentDuration,
                yearsOfHavingDriverLicense = DateTime.Now.Year - user.YearOfGettingDriverLicense
            };

            return vehiclePriceRequest;
        }

        private VehiclePriceRequest GenerateVehiclePriceRequestFromUserData(VehiclePriceFrontendRequest frontendRequest)
        {
            var user = _userRepository.GetUserByID(frontendRequest.MicrosoftId);
            if (user == null) throw new ArgumentException();
            if (!UserFilledHisData(user)) throw new ArgumentException();
            return CreateVehiclePriceRequest(frontendRequest, user);
        }
    }
}
