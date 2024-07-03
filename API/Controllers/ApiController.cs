using Microsoft.AspNetCore.Mvc;
using API.Utility;
using DAL.IRepository;
using BE.DTOs;
using BE;
using BLL.Services;

namespace API.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly string _connectionString;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly EmailService _emailService;
        


        public ApiController(IUserRepository userRepository, IPasswordResetRepository passwordResetRepository, IConfiguration configuration, EmailService emailService)
        {
            _userRepository = userRepository;
            _passwordResetRepository = passwordResetRepository;
            _connectionString = ConfigHelper.GetConnectionString(configuration);
            _emailService = emailService;

        }

        [HttpPost]
        [Route("Signup")]
        public async Task<IActionResult> Signup([FromForm] UserSignupDto newUser)
        {
            Logger.Debug("Starting ApiController::Signup with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(newUser));
            bool createUserStatus = BLL.UserBLL.CreateUser(_connectionString, newUser, _userRepository, _emailService);

            if (createUserStatus)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::Signup");

                


                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                });
            }
            Logger.Info("Return Error");
            Logger.Info("End ApiController::Signup");

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
            Logger.Debug("Start ApiController::Login with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(loginDto));

            var loginUser = BLL.UserBLL.LoginUser(_connectionString, loginDto, _userRepository);
            if (loginUser == null)
            {
                Logger.Info("Return Error");
                Logger.Info("End ApiController::Login");

                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "No user found.",
                });
            }
            if (loginUser is false)
            {
                Logger.Info("Return Error");
                Logger.Info("End ApiController::Login");

                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "Invalid user or password.",
                });
            }
            Logger.Info("Return Ok");
            Logger.Info("End ApiController::Login");

            return Ok(new ApiResponseDto
            {
                Code = 200,
                Message = "Success",
                User = loginUser as BE.User
            });

        }

        [Route("Account/{userId}")]
        public IActionResult Account(long userId)
        {
            Logger.Debug("Start ApiController::Account with userId: {0}", userId);

            var userAccount = BLL.UserBLL.AccountInfo(_connectionString, _userRepository, userId);
            if (userAccount == null)
            {
                Logger.Info("Return Error");
                Logger.Info("End ApiController::Account");

                return Ok(new ApiResponseDto
                {
                    Code = 100,
                    Message = "No user found.",
                });
            }
            Logger.Info("Return Ok");
            Logger.Info("End ApiController::Login");

            return Ok(new ApiResponseDto
            {
                Code = 200,
                Message = "Success",
                User = userAccount
            });
        }

        [Route("Update/{userId}")]
        public IActionResult Update(long userId)
        {
            Logger.Debug("Starting ApiController::Update with userId:{0}", userId);
            User? user = BLL.UserBLL.GetUser(_connectionString, userId, _userRepository);
            if (user != null)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::Update");
                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                    User = user
                });
            }

            Logger.Info("Return Error");
            Logger.Info("End ApiController::Update");

            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User get failed.",
            });

        }

        [HttpPost]
        [Route("Update")]
        public IActionResult Update([FromForm] UserUpdateDto existingUser)
        {
            Logger.Debug("Starting ApiController::Update with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(existingUser));
            User? updatedUser = BLL.UserBLL.UpdateUser(_connectionString, existingUser, _userRepository);

            if (updatedUser != null)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::Update");
                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                    User = updatedUser
                });
            }
            Logger.Info("Return Error");
            Logger.Info("End ApiController::Update");

            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User update failed.",
            });
        }
        [HttpPost]
        [Route("UpdatePassword")]
        public IActionResult UpdatePassword([FromForm] ChangePasswordDto changePasswordDto)
        {
            Logger.Debug("Starting ApiController::UpdatePassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(changePasswordDto));
            User? updatedUser = BLL.UserBLL.UpdateUserPassword(_connectionString, changePasswordDto, _userRepository);

            if (updatedUser != null)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::UpdatePassword");
                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                    User = updatedUser
                });
            }
            Logger.Info("Return Error");
            Logger.Info("End ApiController::UpdatePassword");

            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User UpdatePassword failed.",
            });
        }
        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
            Logger.Debug("Starting ApiController::ForgotPassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(forgotPasswordDto));
            
            User? updatedUser = BLL.UserBLL.ForgotPassword(_connectionString, forgotPasswordDto, _userRepository, _passwordResetRepository, _emailService);

            if (updatedUser != null)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::ForgotPassword");
                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                    User = updatedUser
                });
            }
            Logger.Info("Return Error");
            Logger.Info("End ApiController::ForgotPassword");

            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User ForgotPassword failed.",
            });
        }
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
        {
            Logger.Debug("Starting ApiController::ResetPassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(resetPasswordDto));
            User? updatedUser = BLL.UserBLL.ResetUserPassword(_connectionString, resetPasswordDto, _userRepository, _passwordResetRepository);

            if (updatedUser != null)
            {
                Logger.Info("Return Ok");
                Logger.Info("End ApiController::UpdatePassword");
                return Ok(new ApiResponseDto
                {
                    Code = 200,
                    Message = "Success",
                    User = updatedUser
                });
            }
            Logger.Info("Return Error");
            Logger.Info("End ApiController::UpdatePassword");

            return Ok(new ApiResponseDto
            {
                Code = 100,
                Message = "User UpdatePassword failed.",
            });
        }

    }
}
