using Microsoft.AspNetCore.Mvc;
using API.Utility;
using DAL.IRepository;
using BE.DTOs;

namespace API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly string _connectionString;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ApiController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _connectionString = ConfigHelper.GetConnectionString(configuration);
        }

        [HttpPost]
        [Route("Signup")]
        public IActionResult Signup([FromForm] UserSignupDto newUser)
        {
            Logger.Info("Signup request got data: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(newUser));
            bool createUserStatus = BLL.UserBLL.CreateUser(_connectionString, newUser, _userRepository);

            if (createUserStatus)
            {
                return Ok(new ApiResponseDto
                {
                    Code=200,
                    Message="Success",
                });
            }
            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User creation failed.",
            });
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromForm] UserLoginDto loginDto)
        {
            Logger.Info("Login request got data: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(loginDto));

            var loginUser = BLL.UserBLL.LoginUser(_connectionString, loginDto, _userRepository);
            if (loginUser == null) 
            {
                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "No user found.",
                });
            }
            if (loginUser is false)
            {
                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "Invalid user or password.",
                });
            }

            return Ok(new ApiResponseDto
            {
                Code = 200,
                Message = "Success",
                User= loginUser as BE.User
            });

        }

        [Route("Account/{userId}")]
         public IActionResult Account(long userId)
        {
            Logger.Info("Account request got ID: {0}", userId);

            var userAccount = BLL.UserBLL.AccountInfo(_connectionString, _userRepository, userId);
            if (userAccount == null)
            {
                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "No user found.",
                });
            }

            return Ok(new ApiResponseDto
            {
                Code = 200,
                Message = "Success",
                User = userAccount
            });
        }


    }
}
