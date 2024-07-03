using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BE.DTOs;
using BE;
using UserAuth.Utility;

namespace UserAuth.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClientHelper _httpClientHelper;
        private readonly string _apiUrl;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public UserController(IConfiguration configuration, HttpClientHelper httpClientHelper)
        {
            _configuration = configuration;
            _httpClientHelper = httpClientHelper;
            _apiUrl = ConfigHelper.GetApiUrl(_configuration);
        }

        public IActionResult Signup()
        {
            Logger.Info("Return signup view");
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Signup([FromForm] UserSignupDto newUser)
        {
            Logger.Debug("Starting UserController::Signup with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(newUser));
            if (!ModelState.IsValid)
            {
                return View("Signup", newUser);
            }
            //log
            //call api
            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Email"] = newUser.Email;
            form["Name"] = newUser.Name;
            form["Password"] = newUser.Password;

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/Signup", form);

            Logger.Debug("API response for signup:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code == 200)
            {
                //signup ok, show user success page,
                ViewBag.SignupOk = true;
                Logger.Info("Signup Ok, Returning view");
                return View();
            }


            ModelState.AddModelError("", "User creation failed. Something catastrophically went wrong!");
            Logger.Info("Signup Error, Returning view with error");
            Logger.Info("End UserController::Signup");
            return View("Signup", newUser);

        }

        public IActionResult Login()
        {
            Logger.Info("Starting UserController::Login view");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Logger.Info("Already logged in, Redirect to account page");
                return RedirectToAction("Account", "User");
            }
            Logger.Info("Return login form");
            Logger.Info("End UserController::Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLoginDto loginDto)
        {
            Logger.Debug("Starting UserController::Login with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(loginDto));
            if (!ModelState.IsValid)
            {
                Logger.Info("Model validation error. Returning errors");
                return View(loginDto);
            }
            //log
            //call api
            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Email"] = loginDto.Email;
            form["Password"] = loginDto.Password;

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/Login", form);
            Logger.Debug("API response for login:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code != 200)
            {
                //signup ok, show user success page,
                ModelState.AddModelError("", result.Message);
                Logger.Info("Error from API,{0}", result.Message);
                Logger.Info("End UserController::Login");
                return View(loginDto);
            }


            //valid user, login using cookie
            User existingUser = result.User;

            var claimsIdentity = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, existingUser.Name),
                    new Claim(ClaimTypes.Email, existingUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };
            HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties).Wait();
            Logger.Info("Login Ok, Redirecting user");
            Logger.Info("End UserController::Login");

            return RedirectToAction("Account", "User");

        }

        [HttpPost]
        public IActionResult Logout()
        {
            Logger.Info("Start UserController::Logout");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                ViewBag.SuccessMessage = "Logout successful!";
                Logger.Info("Logout Ok");
                Logger.Info("End UserController::Logout");

                return RedirectToAction("Index", "Home");
            }
            //otherwise nothing to do :-}
            Logger.Info("Not logged in. Redirecting to /");

            Logger.Info("End UserController::Logout");

            return Redirect("/");
        }

        [Authorize]
        public async Task<IActionResult> Account()
        {
            Logger.Info("Start UserController::Account");

            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            //log
            //call api
            Logger.Info("Requesting data for account id: {0}", userId);

            ApiResponseDto result = await _httpClientHelper.GetAsync<ApiResponseDto>(_apiUrl + $"/api/Account/{userId}");
            Logger.Info("API response for account id:{0}:{1}", userId, Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code != 200)
            {
                //signup ok, show user success page,
                Logger.Info("Api returned error");
                Logger.Info("End UserController::Account");

                return Redirect("/Home/Error");
            }
            User existingUser = result.User;
            var accountViewModel = new AccountViewModel
            {
                Id = existingUser.Id,
                Name = existingUser.Name,
                Email = existingUser.Email,
                CreatedAt = existingUser.CreatedAt,
                ModifiedAt = existingUser.ModifiedAt,
            };
            Logger.Info("Return account view");
            Logger.Info("End UserController::Account");

            return View(accountViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Update()
        {
            Logger.Debug("Starting UserController::Update");

            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            Logger.Debug("Authorized user id:{0}", userId);


            ApiResponseDto result = await _httpClientHelper.GetAsync<ApiResponseDto>(_apiUrl + $"/api/Update/{userId}");

            Logger.Debug("API response for update:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));
            if (result.Code != 200)
            {
                Logger.Info("Get Update user failed, Returning view");
                ModelState.AddModelError("", result.Message);

                return View();
            }

            UserUpdateDto userUpdateDto = new UserUpdateDto
            {
                Id = result.User.Id,
                Name = result.User.Name,
                Email = result.User.Email,
            };

            return View(userUpdateDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] UserUpdateDto existingUser)
        {
            //TODO: Implement update feature
            Logger.Debug("Starting UserController::Update with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(existingUser));
            if (!ModelState.IsValid)
            {
                return View(existingUser);
            }
            //log
            //call api
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Email"] = existingUser.Email;
            form["Name"] = existingUser.Name;
            form["Id"] = userId.ToString();

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/Update", form);

            Logger.Debug("API response for update:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code == 200)
            {
                //signup ok, show user success page,
                Logger.Info("Update Ok, Returning view");
                return RedirectToAction("Account", "User");
            }


            ModelState.AddModelError("", "User update failed. Something catastrophically went wrong!");
            Logger.Info("Update Error, Returning view with error");
            Logger.Info("End UserController::Update");
            return View(existingUser);

        }

        [Authorize]
        public async Task<IActionResult> ChangePassword()
        {
            Logger.Debug("Starting UserController::ChangePassword");

            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            string? userName = User.FindFirst(ClaimTypes.Name)?.Value;

            Logger.Debug("Authorized user id:{0}", userId);




            ChangePasswordDto changePasswordDto = new ChangePasswordDto
            {
                Id = userId,
                Name = userName,
            };
            Logger.Debug("Returning view");

            Logger.Debug("End UserController::ChangePassword");

            return View(changePasswordDto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordDto changePasswordDto)
        {
            //TODO: Implement update feature
            Logger.Debug("Starting UserController::ChangePassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(changePasswordDto));
            if (!ModelState.IsValid)
            {
                return View(changePasswordDto);
            }
            //log
            //call api
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);

            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Password"] = changePasswordDto.Password;
            form["NewPassword"] = changePasswordDto.NewPassword;
            form["ConfirmNewPassword"] = changePasswordDto.ConfirmNewPassword;
            form["Id"] = userId.ToString();

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/UpdatePassword", form);

            Logger.Debug("API response for ChangePassword:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code == 200)
            {
                //signup ok, show user success page,
                Logger.Info("ChangePassword Ok, redirecting to profile");
                return RedirectToAction("Account", "User");
            }


            ModelState.AddModelError("", "User ChangePassword failed. Something catastrophically went wrong!");
            Logger.Info("ChangePassword Error, Returning view with error");
            Logger.Info("End UserController::ChangePassword");
            return View(changePasswordDto);

        }

        public async Task<IActionResult> ForgotPassword()
        {
            Logger.Debug("Starting UserController::ForgotPassword");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Logger.Info("Already logged in, Redirect to account page");
                return RedirectToAction("Account", "User");
            }
            Logger.Debug("Returning view");

            Logger.Debug("End UserController::ForgotPassword");

            return View();
        }

        //ForgotMailSent

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
            //TODO: Implement update feature
            Logger.Debug("Starting UserController::ForgotPassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(forgotPasswordDto));
            if (!ModelState.IsValid)
            {
                return View(forgotPasswordDto);
            }
            //log
            //call api
            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Email"] = forgotPasswordDto.Email;

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/ForgotPassword", form);

            Logger.Debug("API response for ForgotPassword:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code == 200)
            {
                //signup ok, show user success page,
                Logger.Info("ForgotPassword Ok, Returning view");
                ViewBag.ForgotMailSent = true;
                return View();
            }


            ModelState.AddModelError("", "User ForgotPassword failed. Something catastrophically went wrong!");
            Logger.Info("ForgotPassword Error, Returning view with error");
            Logger.Info("End UserController::ForgotPassword");
            return View(forgotPasswordDto);

        }

        [Route("/User/ResetPassword/{token}")]
        public async Task<IActionResult> ResetPassword(string token)
        {
            Logger.Debug("Starting UserController::ResetPassword");
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Logger.Info("Already logged in, Redirect to account page");
                return RedirectToAction("Account", "User");
            }
            Logger.Debug("Returning view");

            Logger.Debug("End UserController::ResetPassword");

            return View(new ResetPasswordDto { Token = token });
        }

        [HttpPost]
        [Route("/User/ResetPassword/{token}")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
        {
            //TODO: Implement update feature
            Logger.Debug("Starting UserController::ResetPassword with param:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(resetPasswordDto));
            if (!ModelState.IsValid)
            {
                Logger.Info("Error in model. Return error page");
                return View(resetPasswordDto);
            }
            //log
            //call api
            Dictionary<string, string> form = new Dictionary<string, string>();
            form["NewPassword"] = resetPasswordDto.NewPassword;
            form["ConfirmNewPassword"] = resetPasswordDto.ConfirmNewPassword;
            form["Token"] = resetPasswordDto.Token;

            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/ResetPassword", form);

            Logger.Debug("API response for ResetPassword:{0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code == 200)
            {
                //signup ok, show user success page,
                Logger.Info("ResetPassword Ok, Returning view");
                ViewBag.ResetOk = true;
                return View();
            }


            ModelState.AddModelError("", "User ResetPassword failed. Something catastrophically went wrong!");
            Logger.Info("ResetPassword Error, Returning view with error");
            Logger.Info("End UserController::ResetPassword");
            return View(resetPasswordDto);

        }


    }
}
