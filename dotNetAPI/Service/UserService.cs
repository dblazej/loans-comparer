using dotNetAPI.DTO;
using dotNetAPI.Entity;
using dotNetAPI.Repository;
using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;


namespace dotNetAPI.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICarStatusService _carStatusService;

        public UserService(IUserRepository userRepository, ICarStatusService carStatusService)
        {
            _userRepository = userRepository;
            _carStatusService = carStatusService;
        } 

        public bool AddUser(RegisterUserDTO registerUserDTO)
        {
            User user = new User();
            user.MicrosoftID = registerUserDTO.MicrosoftID;
            user.YearOfGettingDriverLicense = registerUserDTO.YearOfGettingDriverLicence;
            user.YearOfBirth = registerUserDTO.YearOfBirth;
            user.City = registerUserDTO.City;
            user.Country = registerUserDTO.Country;
            user.Email = registerUserDTO.Email;

            //user.UserName = registerUserDTO.UserName;
            //user.Password = registerUserDTO.Password;

            return _userRepository.AddUser(user);
        }

        public string AuthenticateUser(string username, string password)
        {
            AuthConfig authConfig = AuthConfig.ReadJsonFromFile("appsettings.json");
            Console.WriteLine($"Authority: {authConfig.Authority}");

            RunAsync().GetAwaiter().GetResult();

            return "asd";
        }

        private static async Task RunAsync()
        {
            AuthConfig authConfig = AuthConfig.ReadJsonFromFile("appsettings.json");

            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder.Create(authConfig.ClientId)
                .WithClientSecret(authConfig.ClientSecret)
                .WithAuthority(new Uri(authConfig.Authority))
                .Build();

            string[] ResourceIds = new string[] {authConfig.ResourceId};

            AuthenticationResult result = null;

            try
            {
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.WriteLine(result.AccessToken);
            }
            catch (MsalClientException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public List<UserDTO> GetAllUsers()
        {
            List<User> users = _userRepository.GetAllUsers();

            List<UserDTO> result = new List<UserDTO>();

            int currentYear = DateTime.Now.Year;

            foreach (User user in users)
            {
                UserDTO userDTO = new UserDTO();

                userDTO.MicrosoftID = user.MicrosoftID;
                userDTO.YearsOfHavingDriverLicence = currentYear - user.YearOfGettingDriverLicense;
                userDTO.YearOfBirth = user.YearOfBirth;
                userDTO.City = user.City;
                userDTO.Country = user.Country;
                userDTO.Email = user.Email;

                List<CarHistoryTimestampDTO> carHistoryTimestampDTOs = _carStatusService.GetHistoryOfRentedCarsTimestamps(user.MicrosoftID);

                userDTO.CurrentlyRentedCount = _carStatusService.CountActuallyRentedCarsFromRentedHistory(carHistoryTimestampDTOs);
                userDTO.OverallRentedCount = _carStatusService.CountOverallRentedCarsFromRentedHistory(carHistoryTimestampDTOs);

                result.Add(userDTO);
            }

            return result;
        }
        public UserDTO GetUserByID(string MicrosoftID)
        {
            User user = _userRepository.GetUserByID(MicrosoftID);

            if(user == null)
                return null;

            UserDTO result = new UserDTO();

            int currentYear = DateTime.Now.Year;

            result.MicrosoftID = user.MicrosoftID;
            result.YearsOfHavingDriverLicence = currentYear - user.YearOfGettingDriverLicense;
            result.YearOfBirth = user.YearOfBirth;
            result.City = user.City;
            result.Country = user.Country;
            result.Email = user.Email;

            List<CarHistoryTimestampDTO> carHistoryTimestampDTOs = _carStatusService.GetHistoryOfRentedCarsTimestamps(user.MicrosoftID);

            result.CurrentlyRentedCount = _carStatusService.CountActuallyRentedCarsFromRentedHistory(carHistoryTimestampDTOs);
            result.OverallRentedCount = _carStatusService.CountOverallRentedCarsFromRentedHistory(carHistoryTimestampDTOs);

            return result;
        }
    }
}
