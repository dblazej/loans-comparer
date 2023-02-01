using dotNetAPI.DTO;
using dotNetAPI.Entity;

namespace dotNetAPI.Repository
{
    public interface IUserRepository
    {
        bool AddUser(User user);
        List<User> GetAllUsers();
        User GetUserByID(string MicrosoftID);
    }
}
