using dotNetAPI.DTO;
using dotNetAPI.Service;
using Microsoft.AspNetCore.Mvc;

namespace dotNetAPI.Controller
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult AddUser(RegisterUserDTO registerUserDTO)
        {
            if(_userService.AddUser(registerUserDTO))
                return Ok();
            return Conflict(); // użytkownik z podanym MicrosoftID lub Email istnieje, nie rejestrujemy
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers() // do sprawdzania
        {
            return Ok(_userService.GetAllUsers());
        }

        [HttpGet("authenticate/{username}/{password}")]
        public string AuthenticateUser(string username, string password)
        {
            return _userService.AuthenticateUser(username, password);
        }

        [HttpGet("get")]
        public ActionResult<UserDTO> GetUser([FromHeader(Name = "id")] string MicrosoftID)
        {
            var user = _userService.GetUserByID(MicrosoftID);
            if (user != null)
                return Ok(user);
            return NotFound();
        }
    }
}
