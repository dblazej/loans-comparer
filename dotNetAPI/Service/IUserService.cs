using dotNetAPI.DTO;

namespace dotNetAPI.Service
{
    public interface IUserService
    {
        bool AddUser(RegisterUserDTO registerUserDTO);
        List<UserDTO> GetAllUsers();
        UserDTO GetUserByID(string MicrosoftID);
        string AuthenticateUser(string username, string password);
    }
}
