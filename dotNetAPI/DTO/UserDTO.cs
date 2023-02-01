namespace dotNetAPI.DTO
{
    public class UserDTO
    {
        public string MicrosoftID { get; set; }
        public int YearsOfHavingDriverLicence { get; set; }
        public int YearOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public int CurrentlyRentedCount { get; set; }
        public int OverallRentedCount { get; set; }

        //public string UserName { get; set; }
        //public string Password { get; set; }

        public UserDTO()
        {
        }
    }
}
