using dotNetAPI.Repository;
using dotNetAPI.Service;
using dotNetAPI.Controller;
using dotNetAPI.DTO;
using dotNetAPI.Entity;
using Xunit;
using Moq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using dotNetAPI.Utils;
using dotNetAPI.Controllers;
using dotNetAPI.DTO.VehicleResponse;
using dotNetAPI.DTO.Vehicle.Request;
using dotNetAPI.DTO.Vehicle.Response;

namespace dotNetAPI.Test
{
    public class CarControllerTest
    {
        [Fact]
        public void TestGetAllCars()
        {
            // Arrange
            var mockCarRepo = new Mock<ICarRepository>();
            mockCarRepo.Setup(repo => repo.GetAllCars())
                .Returns(GetTestCars());

            var carService = new CarService(mockCarRepo.Object, null, null, null, null, null, null, null);
            var controller = new CarController(carService, null);

            // Act
            var result = controller.GetAllCars();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<VehiclesResponse>(list.Value);

            var listCars = list.Value as VehiclesResponse;
            Assert.Equal(29, listCars.vehicles.Count);
            Assert.Equal(29, listCars.vehiclesCount);

        }

        [Fact]
        public void TestAddCar()
        {
            // Arange
            var addCarDTO = new AddCarDTO()
            {
                ImageUrl = "image",
                brandName = "Tesla",
                modelName = "Model S",
                year = 2020,
                enginePower = 670,
                enginePowerType = "HP",
                capacity = 5,
                description = "wow",
                price = 50000,
                currency = "PLN"
            };

            var mockCarRepo = new Mock<ICarRepository>();
            mockCarRepo.Setup(repo => repo.AddCar(It.IsAny<Car>()));

            var carService = new CarService(mockCarRepo.Object, null, null, new PriceCalculator(), null, null, null, null);
            var controller = new CarController(carService, null);

            // Act
            var response = controller.AddCar(addCarDTO);

            // Assert
            Assert.IsType<StatusCodeResult>(response); // zmieniæ na OkResult
        }

        [Fact]
        public void TestGetCarPriceByID()
        {
            // Arrange
            var validCarID = "DE1B8281-8D09-4CEF-EC46-08D9C667C13C";
            var invalidCarID = "DE1B8281-8D09-4CEF-EC46-08D9C667C14D";

            var validVehiclePriceFrontendRequest = new VehiclePriceFrontendRequest()
            {
                MicrosoftId = "1",
                CompanyName = "OUR",
                RentDuration = 10
            };

            var invalidVehiclePriceFrontendRequest = new VehiclePriceFrontendRequest()
            {
                MicrosoftId = "1000000",
                CompanyName = "SOME",
                RentDuration = -10
            };

            var mockValidCarRepository = new Mock<ICarRepository>();
            mockValidCarRepository.Setup(repo => repo.GetCarById(new Guid(validCarID)))
                .Returns(GetTestCars()[1]);

            var mockInvalidCarRepository = new Mock<ICarRepository>();
            mockInvalidCarRepository.Setup(repo => repo.GetCarById(new Guid(invalidCarID)))
                .Returns<Car>(null);

            var mockValidUserRepository = new Mock<IUserRepository>();
            mockValidUserRepository.Setup(repo => repo.GetUserByID(validVehiclePriceFrontendRequest.MicrosoftId))
                .Returns(GetTestUsers()[0]);

            var mockInvalidUserRepository = new Mock<IUserRepository>();
            mockInvalidUserRepository.Setup(repo => repo.GetUserByID(invalidVehiclePriceFrontendRequest.MicrosoftId))
                .Returns<User>(null);

            var mockValidCarStatusRepository = new Mock<ICarStatusRepository>();
            mockValidCarStatusRepository.Setup(repo => repo.GetHistoryOfRentedCars(validVehiclePriceFrontendRequest.MicrosoftId))
                .Returns(GetTestHistoryByID());

            var mockInvalidCarStatusRepository = new Mock<ICarStatusRepository>();
            mockInvalidCarStatusRepository.Setup(repo => repo.GetHistoryOfRentedCars(invalidVehiclePriceFrontendRequest.MicrosoftId))
                .Returns<CarStatus>(null);

            var mockVehicleQuoteRepository = new Mock<IVehicleQuoteRepository>();
            mockVehicleQuoteRepository.Setup(repo => repo.addVehicleQuote(It.IsAny<VehicleQuote>()))
                .Returns(new Guid("372733D4-337B-4212-881D-08D9C669EF60"));


            var validCarStatusService = new CarHistoryService(null, mockValidCarStatusRepository.Object);
            var invalidCarStatusService = new CarHistoryService(null, mockInvalidCarStatusRepository.Object);

            var validUserService = new UserService(mockValidUserRepository.Object, validCarStatusService);
            var invalidUserService = new UserService(mockInvalidUserRepository.Object, invalidCarStatusService);

            var validCarService = new CarService(mockValidCarRepository.Object, null, mockValidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, validCarStatusService);
            var invalidCarIDCarService = new CarService(mockInvalidCarRepository.Object, null, mockValidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, validCarStatusService);
            var invalidUserCarService = new CarService(mockValidCarRepository.Object, null, mockInvalidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, invalidCarStatusService);
            var invalidCarService = new CarService(mockInvalidCarRepository.Object, null, mockInvalidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, invalidCarStatusService);

            var validCarController = new CarController(validCarService, validCarStatusService);
            var invalidUserCarController = new CarController(invalidUserCarService, invalidCarStatusService);
            var invalidCarIDCarController = new CarController(invalidCarIDCarService, validCarStatusService);
            var invalidCarController = new CarController(invalidCarService, invalidCarStatusService);

            // Act
            var validResponse = validCarController.GetCarPriceById(validVehiclePriceFrontendRequest, validCarID);
            var invalidUserResponse = invalidUserCarController.GetCarPriceById(invalidVehiclePriceFrontendRequest, validCarID);
            //var invalidCarIDResponse = invalidCarIDCarController.GetCarPriceById(validVehiclePriceFrontendRequest, invalidCarID);
            var invalidResponse = invalidCarController.GetCarPriceById(invalidVehiclePriceFrontendRequest, invalidCarID);

            // Assert
            Assert.IsType<OkObjectResult>(validResponse.Result);
            Assert.IsType<StatusCodeResult>(invalidUserResponse.Result);
            //Assert.IsType<StatusCodeResult>(invalidCarIDResponse.Result);
            Assert.IsType<StatusCodeResult>(invalidResponse.Result);

            var invalidPriceResponse1 = invalidUserResponse.Result as StatusCodeResult;
            //var invalidPriceResponse2 = invalidCarIDResponse.Result as StatusCodeResult;
            var invalidPriceResponse3 = invalidResponse.Result as StatusCodeResult;
            Assert.Equal(404, invalidPriceResponse1.StatusCode);
            //Assert.Equal(403, invalidPriceResponse2.StatusCode);
            Assert.Equal(404, invalidPriceResponse3.StatusCode);


            var priceResponse = validResponse.Result as OkObjectResult;
            Assert.IsType<VehiclePriceResponse>(priceResponse.Value);

            var vehiclePriceResponse = priceResponse.Value as VehiclePriceResponse;
            Assert.Equal(110, vehiclePriceResponse.price);
        }


        [Fact]
        public void TestGetCarPriceByBrandAndModel()
        {
            // Arrange
            var validCarBrand = "BMW";
            var validCarModel = "320d";

            var invalidCarBrand = "invalid";
            var invalidCarModel = "invalid";

            var validVehiclePriceFrontendRequest = new VehiclePriceFrontendRequest()
            {
                MicrosoftId = "1",
                CompanyName = "OUR",
                RentDuration = 10
            };

            var invalidVehiclePriceFrontendRequest = new VehiclePriceFrontendRequest()
            {
                MicrosoftId = "1000000",
                CompanyName = "SOME",
                RentDuration = -10
            };

            var mockValidCarRepository = new Mock<ICarRepository>();
            mockValidCarRepository.Setup(repo => repo.GetCarByBrandAndModel(validCarBrand, validCarModel))
                .Returns(GetTestCars()[1]);

            var mockInvalidCarRepository = new Mock<ICarRepository>();
            mockInvalidCarRepository.Setup(repo => repo.GetCarByBrandAndModel(invalidCarBrand, invalidCarModel))
                .Returns<Car>(null);

            var mockValidUserRepository = new Mock<IUserRepository>();
            mockValidUserRepository.Setup(repo => repo.GetUserByID(validVehiclePriceFrontendRequest.MicrosoftId))
                .Returns(GetTestUsers()[0]);

            var mockInvalidUserRepository = new Mock<IUserRepository>();
            mockInvalidUserRepository.Setup(repo => repo.GetUserByID(invalidVehiclePriceFrontendRequest.MicrosoftId))
                .Returns<User>(null);

            var mockValidCarStatusRepository = new Mock<ICarStatusRepository>();
            mockValidCarStatusRepository.Setup(repo => repo.GetHistoryOfRentedCars(validVehiclePriceFrontendRequest.MicrosoftId))
                .Returns(GetTestHistoryByID());

            var mockInvalidCarStatusRepository = new Mock<ICarStatusRepository>();
            mockInvalidCarStatusRepository.Setup(repo => repo.GetHistoryOfRentedCars(invalidVehiclePriceFrontendRequest.MicrosoftId))
                .Returns<CarStatus>(null);

            var mockVehicleQuoteRepository = new Mock<IVehicleQuoteRepository>();
            mockVehicleQuoteRepository.Setup(repo => repo.addVehicleQuote(It.IsAny<VehicleQuote>()))
                .Returns(new Guid("372733D4-337B-4212-881D-08D9C669EF60"));


            var validCarStatusService = new CarHistoryService(null, mockValidCarStatusRepository.Object);
            var invalidCarStatusService = new CarHistoryService(null, mockInvalidCarStatusRepository.Object);

            var validUserService = new UserService(mockValidUserRepository.Object, validCarStatusService);
            var invalidUserService = new UserService(mockInvalidUserRepository.Object, invalidCarStatusService);

            var validCarService = new CarService(mockValidCarRepository.Object, null, mockValidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, validCarStatusService);
            var invalidCarIDCarService = new CarService(mockInvalidCarRepository.Object, null, mockValidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, validCarStatusService);
            var invalidUserCarService = new CarService(mockValidCarRepository.Object, null, mockInvalidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, invalidCarStatusService);
            var invalidCarService = new CarService(mockInvalidCarRepository.Object, null, mockInvalidCarStatusRepository.Object, new PriceCalculator(), mockVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, invalidCarStatusService);

            var validCarController = new CarController(validCarService, validCarStatusService);
            var invalidUserCarController = new CarController(invalidUserCarService, invalidCarStatusService);
            var invalidCarIDCarController = new CarController(invalidCarIDCarService, validCarStatusService);
            var invalidCarController = new CarController(invalidCarService, invalidCarStatusService);

            // Act
            var validResponse = validCarController.GetCarPriceByBrandAndModel(validVehiclePriceFrontendRequest, validCarBrand, validCarModel);
            var invalidUserResponse = invalidUserCarController.GetCarPriceByBrandAndModel(invalidVehiclePriceFrontendRequest, validCarBrand, validCarModel);
            //var invalidCarIDResponse = invalidCarIDCarController.GetCarPriceByBrandAndModel(validVehiclePriceFrontendRequest, invalidCarBrand, invalidCarModel);
            var invalidResponse = invalidCarController.GetCarPriceByBrandAndModel(invalidVehiclePriceFrontendRequest, invalidCarBrand, invalidCarModel);

            // Assert
            Assert.IsType<OkObjectResult>(validResponse.Result);
            Assert.IsType<StatusCodeResult>(invalidUserResponse.Result);
            //Assert.IsType<StatusCodeResult>(invalidCarIDResponse.Result);
            Assert.IsType<StatusCodeResult>(invalidResponse.Result);

            var invalidPriceResponse1 = invalidUserResponse.Result as StatusCodeResult;
            //var invalidPriceResponse2 = invalidCarIDResponse.Result as StatusCodeResult;
            var invalidPriceResponse3 = invalidResponse.Result as StatusCodeResult;
            Assert.Equal(404, invalidPriceResponse1.StatusCode);
            //Assert.Equal(403, invalidPriceResponse2.StatusCode);
            Assert.Equal(404, invalidPriceResponse3.StatusCode);


            var priceResponse = validResponse.Result as OkObjectResult;
            Assert.IsType<VehiclePriceResponse>(priceResponse.Value);

            var vehiclePriceResponse = priceResponse.Value as VehiclePriceResponse;
            Assert.Equal(110, vehiclePriceResponse.price);
        }

        [Fact]
        public void TestRentCar()
        {
            // Arrange
            var validQuoteID = "372733D4-337B-4212-881D-08D9C669EF60";
            var invalidQuoteID = "372733D4-337B-4212-881D-08D9C669EF21";

            var validMicrosoftID = "1";
            var invalidMicrosoftID = "1000000";

            var rentVehicleRequest = new RentVehicleRequest()
            {
                startDate = DateTime.Now
            };

            var mockValidVehicleQuoteRepository = new Mock<IVehicleQuoteRepository>();
            mockValidVehicleQuoteRepository.Setup(repo => repo.getVehicleQuoteById(new Guid(validQuoteID)))
                .Returns(GetTestVehicleQuote());

            var mockInvalidVehicleQuoteRepository = new Mock<IVehicleQuoteRepository>();
            mockInvalidVehicleQuoteRepository.Setup(repo => repo.getVehicleQuoteById(new Guid(invalidQuoteID)))
                .Returns<VehicleQuote>(null);

            var mockCarRepository = new Mock<ICarRepository>();
            mockCarRepository.Setup(repo => repo.UpdateCar(It.IsAny<Car>()));

            var mockValidUserRepository = new Mock<IUserRepository>();
            mockValidUserRepository.Setup(repo => repo.GetUserByID(validMicrosoftID))
                .Returns(GetTestUsers()[0]);

            var mockInvalidUserRepository = new Mock<IUserRepository>();
            mockValidUserRepository.Setup(repo => repo.GetUserByID(invalidMicrosoftID))
                .Returns<User>(null);

            var mockCarStatusRepository = new Mock<ICarStatusRepository>();
            mockCarStatusRepository.Setup(repo => repo.addCarStatus(It.IsAny<CarStatus>()))
                .Returns(new Guid("5D4491B6-CF12-4BD1-215F-08D9DA9DE54C"));

            var validCarService = new CarService(mockCarRepository.Object, null, mockCarStatusRepository.Object, new PriceCalculator(), mockValidVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, null);
            var invalidUserCarService = new CarService(mockCarRepository.Object, null, mockCarStatusRepository.Object, new PriceCalculator(), mockValidVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, null);
            var invalidQuoteCarService = new CarService(mockCarRepository.Object, null, mockCarStatusRepository.Object, new PriceCalculator(), mockInvalidVehicleQuoteRepository.Object, null, mockValidUserRepository.Object, null);
            var invalidCarService = new CarService(mockCarRepository.Object, null, mockCarStatusRepository.Object, new PriceCalculator(), mockInvalidVehicleQuoteRepository.Object, null, mockInvalidUserRepository.Object, null);

            var validCarController = new CarController(validCarService, null);
            var invalidUserCarController = new CarController(invalidUserCarService, null);
            var invalidQuoteCarController = new CarController(invalidQuoteCarService, null);
            var invalidCarController = new CarController(invalidCarService, null);

            // Act
            var validResponse = validCarController.RentCar(rentVehicleRequest, validQuoteID, validMicrosoftID);
            var invalidUserResponse = invalidUserCarController.RentCar(rentVehicleRequest, validQuoteID, invalidMicrosoftID);
            var invalidQuoteResponse = invalidQuoteCarController.RentCar(rentVehicleRequest, invalidQuoteID, validMicrosoftID);
            var invalidResponse = invalidCarController.RentCar(rentVehicleRequest, invalidQuoteID, invalidMicrosoftID);

            // Assert
            Assert.IsType<OkObjectResult>(validResponse.Result);
            Assert.IsType<StatusCodeResult>(invalidUserResponse.Result);
            //Assert.IsType<StatusCodeResult>(invalidQuoteResponse.Result);
            //Assert.IsType<StatusCodeResult>(invalidResponse.Result);

            var invalidRentResponse1 = invalidUserResponse.Result as StatusCodeResult;
            //var invalidRentResponse2 = invalidQuoteResponse.Result as StatusCodeResult;
            //var invalidRentResponse3 = invalidResponse.Result as StatusCodeResult;
            Assert.Equal(404, invalidRentResponse1.StatusCode);
            //Assert.Equal(403, invalidRentResponse2.StatusCode);
            //Assert.Equal(403, invalidRentResponse3.StatusCode);

            var rentResponse = validResponse.Result as OkObjectResult;
            Assert.IsType<RentVehicleResponse>(rentResponse.Value);

            var rentVehicleResponse = rentResponse.Value as RentVehicleResponse;
            Assert.Equal("372733D4-337B-4212-881D-08D9C669EF60", rentVehicleResponse.quoteId);
            Assert.Equal("5d4491b6-cf12-4bd1-215f-08d9da9de54c", rentVehicleResponse.rentId);
            Assert.Equal(rentVehicleRequest.startDate, rentVehicleResponse.rentAt);
            Assert.Equal(rentVehicleRequest.startDate, rentVehicleResponse.startDate);


        }

        private List<User> GetTestUsers()
        {
            var users = new List<User>();
            users.Add(new User()
            {
                MicrosoftID = "1",
                YearOfGettingDriverLicense = 2010,
                YearOfBirth = 1990,
                City = "Warsaw",
                Country = "Poland",
                Email = "test@gmail.com"
            });
            users.Add(new User()
            {
                MicrosoftID = "2",
                YearOfGettingDriverLicense = 2019,
                YearOfBirth = 1999,
                City = "Warsaw",
                Country = "Poland",
                Email = "testmail@gmail.com"
            });
            return users;
        }
        private List<Car> GetTestCars()
        {
            var cars = new List<Car>();
            Guid carId = Guid.NewGuid();
            cars.Add(new Car()
            {
                Id = new Guid("72FA1B09-6D29-4322-EC45-08D9C667C13C"),
                ImageUrl = "image1",
                Brand = "BMW",
                Model = "320d",
                Year = 2014,
                Description = "kolor czarny",
                BasePrice = 10000,
                EnginePower = 184,
                EnginePowerType = "HP",
                Capacity = 5,
                Currency = "PLN",
                IsRented = false,
                ReturnDate = DateTime.Now

            }); ;
            cars.Add(new Car()
            {
                Id = new Guid("DE1B8281-8D09-4CEF-EC46-08D9C667C13C"),
                ImageUrl = "image2",
                Brand = "BMW",
                Model = "320d",
                Year = 2015,
                Description = "kolor srebrny",
                BasePrice = 11000,
                EnginePower = 184,
                EnginePowerType = "HP",
                Capacity = 5,
                Currency = "PLN",
                IsRented = true,
                ReturnDate = DateTime.Now.AddDays(10)

            });
            cars.Add(new Car()
            {
                Id = new Guid("A076E09B-8122-4D01-0D55-08D9C6C598DD"),
                ImageUrl = "image3",
                Brand = "BMW",
                Model = "320d",
                Year = 2016,
                Description = "kolor srebrny",
                BasePrice = 12000,
                EnginePower = 184,
                EnginePowerType = "HP",
                Capacity = 5,
                Currency = "PLN",
                IsRented = true,
                ReturnDate = DateTime.Now.AddDays(8)

            });
            return cars;
        }
        private List<CarStatus> GetTestHistoryByID()
        {
            var history = new List<CarStatus>();
            history.Add(new CarStatus()
            {
                Id = new Guid("BA7FCBC1-8A79-47D6-1BE5-08D9C6C632C4"),
                Car = GetTestCars()[0],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-10),
                PerformedBy = "1",
                Note = ""
            });
            history.Add(new CarStatus()
            {
                Id = new Guid("BEB60F58-1010-4830-7BFB-08D9C6C6E34D"),
                Car = GetTestCars()[0],
                Action = "RETURNED",
                ActionDateTime = DateTime.Now.AddDays(-5),
                PerformedBy = "1",
                Note = ""
            });
            /*history.Add(new CarStatus()
            {
                Id = new Guid("23DBA8EC-5D83-4AF7-0EAA-08D9D397DA0F"),
                Car = GetTestCars()[1],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-2),
                PerformedBy = "2",
                Note = ""
            });*/
            history.Add(new CarStatus()
            {
                Id = new Guid("B38D3409-4C48-4AE9-4E6A-08D9DA87AF1B"),
                Car = GetTestCars()[2],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-2),
                PerformedBy = "1",
                Note = ""
            });
            return history;
        }

        private List<CarStatus> GetTestHistory()
        {
            var history = new List<CarStatus>();
            history.Add(new CarStatus()
            {
                Id = new Guid("BA7FCBC1-8A79-47D6-1BE5-08D9C6C632C4"),
                Car = GetTestCars()[0],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-10),
                PerformedBy = "1",
                Note = ""
            });
            history.Add(new CarStatus()
            {
                Id = new Guid("BEB60F58-1010-4830-7BFB-08D9C6C6E34D"),
                Car = GetTestCars()[0],
                Action = "RETURNED",
                ActionDateTime = DateTime.Now.AddDays(-5),
                PerformedBy = "1",
                Note = ""
            });
            history.Add(new CarStatus()
            {
                Id = new Guid("23DBA8EC-5D83-4AF7-0EAA-08D9D397DA0F"),
                Car = GetTestCars()[1],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-2),
                PerformedBy = "2",
                Note = ""
            });
            history.Add(new CarStatus()
            {
                Id = new Guid("B38D3409-4C48-4AE9-4E6A-08D9DA87AF1B"),
                Car = GetTestCars()[2],
                Action = "RENTED",
                ActionDateTime = DateTime.Now.AddDays(-2),
                PerformedBy = "1",
                Note = ""
            });
            return history;
        }

        private VehicleQuote GetTestVehicleQuote()
        {
            var quote = new VehicleQuote()
            {
                id = new Guid("372733D4-337B-4212-881D-08D9C669EF60"),
                car = GetTestCars()[0],
                price = 110,
                currency = "PLN",
                generatedAt = DateTime.Now.AddMinutes(-1),
                expiredAt = DateTime.Now.AddMinutes(-1).AddHours(1),
                rentDuration = 10
            };
            return quote;
        }
    }
}