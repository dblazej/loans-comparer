using System.ComponentModel.DataAnnotations;

namespace dotNetAPI.Entity
{
    public class User
    {
        [Key]
        public string MicrosoftID { get; set; }
        public int YearOfGettingDriverLicense { get; set; } // tak samo jak YearOfBirth
        public int YearOfBirth { get; set; } // chyba sensowniejszym polem jest data urodzenia niż podanie ile ma się lat?
        public string City { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }

        //public string UserName { get; set; }
        //public string Password { get; set; } // musimy zapisywać hasło? Czy OpenId za nas tego nie załatwia?

        public User()
        {
        }
    }

}
