namespace dotNetAPI.DTO
{
    public class RegisterUserDTO
    {
        public string MicrosoftID { get; set; }
        public int YearOfGettingDriverLicence { get; set; } // tak samo jak z YearOfBirth
        public int YearOfBirth { get; set; } // chyba sensowniejszym polem jest data urodzenia niż podanie ile ma się lat?
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        //public string UserName { get; set; }
        //public string Password { get; set; }

        public RegisterUserDTO()
        {
        }
    }
}
