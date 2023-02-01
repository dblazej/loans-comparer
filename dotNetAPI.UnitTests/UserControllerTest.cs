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

namespace dotNetAPI.Test
{
    public class UserControllerTest
    {
        [Fact]
        public void TestGetAllUsers()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(repo => repo.GetAllUsers())
                .Returns(GetTestUsers());
            var mockCarStatusRepo = new Mock<ICarStatusRepository>();
            mockCarStatusRepo.Setup(repo => repo.GetHistoryOfRentedCars(It.IsAny<string>()))
                .Returns(GetTestHistory());

            var carStatusService = new CarHistoryService(null, mockCarStatusRepo.Object);
            var userService = new UserService(mockUserRepo.Object, carStatusService);
            var controller = new UserController(userService);

            // Act
            var result = controller.GetAllUsers();

            //Assert
            Assert.IsType<OkObjectResult>(result.Result);

            var list = result.Result as OkObjectResult;
            Assert.IsType<List<UserDTO>>(list.Value);

            var listUsers = list.Value as List<UserDTO>;
            Assert.Equal(2, listUsers.Count);

        }

        [Theory]
        [InlineData("1", "100000000")]
        public void TestGetUser(string validMicrosoftID, string invalidMicrosoftID)
        {
            // Arrange
            var mockValidUserRepo = new Mock<IUserRepository>();
            mockValidUserRepo.Setup(repo => repo.GetUserByID(validMicrosoftID))
                .Returns(GetTestUsers()[0]);
            var mockInvalidUserRepo = new Mock<IUserRepository>();
            mockInvalidUserRepo.Setup(repo => repo.GetUserByID(invalidMicrosoftID))
                .Returns<User>(null);
            var mockCarStatusRepo = new Mock<ICarStatusRepository>();
            mockCarStatusRepo.Setup(repo => repo.GetHistoryOfRentedCars(It.IsAny<string>()))
                .Returns(GetTestHistoryByID());

            var carStatusService = new CarHistoryService(null, mockCarStatusRepo.Object);

            var validUserService = new UserService(mockValidUserRepo.Object, carStatusService);
            var validUserController = new UserController(validUserService);

            var invalidUserService = new UserService(mockInvalidUserRepo.Object, carStatusService);
            var invalidUserController = new UserController(invalidUserService);

            // Act
            var okResult = validUserController.GetUser(validMicrosoftID);
            var notFoundResult = invalidUserController.GetUser(invalidMicrosoftID);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.IsType<NotFoundResult>(notFoundResult.Result);

            var user = okResult.Result as OkObjectResult;

            Assert.IsType<UserDTO>(user.Value);

            var userData = user.Value as UserDTO;
            Assert.Equal(validMicrosoftID, userData.MicrosoftID);
            Assert.Equal(12, userData.YearsOfHavingDriverLicence);
            Assert.Equal(1990, userData.YearOfBirth);
            Assert.Equal("Warsaw", userData.City);
            Assert.Equal("Poland", userData.Country);
            Assert.Equal("test@gmail.com", userData.Email);
            Assert.Equal(1, userData.CurrentlyRentedCount);
            Assert.Equal(2, userData.OverallRentedCount);
        }

        [Fact]
        public void TestAddUser()
        {
            // Arrange
            var validRegisterUserDTO = new RegisterUserDTO()
            {
                MicrosoftID = "3",
                YearOfGettingDriverLicence = 2020,
                YearOfBirth = 2000,
                City = "Pruszków",
                Country = "Poland",
                Email = "valid@email.com"
            };
            var invalidRegisterUserDTO = new RegisterUserDTO()
            {
                MicrosoftID = "1",
                Country = "Poland",
                Email = "invalid"
            };

            var mockValidUserRepo = new Mock<IUserRepository>();
            mockValidUserRepo.Setup(repo => repo.AddUser(It.IsAny<User>()))
                .Returns(true);
            var mockInvalidUserRepo = new Mock<IUserRepository>();
            mockInvalidUserRepo.Setup(repo => repo.AddUser(It.IsAny<User>()))
                .Returns(false);

            var validUserService = new UserService(mockValidUserRepo.Object, null);
            var invalidUserService = new UserService(mockInvalidUserRepo.Object, null);
            var validUserController = new UserController(validUserService);
            var invalidUserController = new UserController(invalidUserService);

            // Act
            var validResponse = validUserController.AddUser(validRegisterUserDTO);
            var invalidResponse = invalidUserController.AddUser(invalidRegisterUserDTO);

            // Assert
            Assert.IsType<OkResult>(validResponse);
            Assert.IsType<ConflictResult>(invalidResponse);

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
    }
}