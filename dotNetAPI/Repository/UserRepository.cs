using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CarRentalDbContext _db;

        public UserRepository(CarRentalDbContext db)
        {
            _db = db;
        }
        public bool AddUser(User user)
        {
            if(!IsValidEmail(user.Email)) // walidacja maila
                return false;
            if (_db.User.Where(u => u.MicrosoftID == user.MicrosoftID || u.Email == user.Email ).Any() ) // nie pozwalamy założyć konta o tej samej nazwie lub mailu co istniejące
                return false;

            _db.User.Add(user);
            _db.SaveChanges();
            return true;
        }

        public List<User> GetAllUsers()
        {
            return _db.User.ToList();
        }
        public User GetUserByID(string MicrosoftID)
        {
            return _db.User.Where(user => user.MicrosoftID == MicrosoftID).SingleOrDefault();
        }

        private bool IsValidEmail(string email) // walidacja maila ze Stack'a
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
