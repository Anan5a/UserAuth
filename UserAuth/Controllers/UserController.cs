using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BE.DTOs;
using BE;
using UserAuth.Utility;
using Microsoft.SqlServer.Server;

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
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Signup([FromForm] UserSignupDto newUser)
        {
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

            Logger.Info("API request data for signup: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(form));
            
            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/Signup", form);
            
            Logger.Info("API response for signup: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));
            
            if (result.Code == 200)
            {
                //signup ok, show user success page,
                ViewBag.SignupOk = true;
                return View();
            }


            ModelState.AddModelError("", "User creation failed. Something catastrophically went wrong!");
            return View("Signup", newUser);

        }

        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Account", "User");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            //log
            //call api
            Dictionary<string, string> form = new Dictionary<string, string>();
            form["Email"] = loginDto.Email;
            form["Password"] = loginDto.Password;

            Logger.Info("API request data for login: ", Newtonsoft.Json.JsonConvert.SerializeObject(form));


            ApiResponseDto result = await _httpClientHelper.PostAsync<ApiResponseDto>(_apiUrl + "/api/Login", form);
            Logger.Info("API response for login: {0}", Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code != 200)
            {
                //signup ok, show user success page,
                ModelState.AddModelError("", result.Message);
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

            return RedirectToAction("Account", "User");

        }

        [HttpPost]
        public IActionResult Logout()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                ViewBag.SuccessMessage = "Logout successful!";
                return RedirectToAction("Index", "Home");
            }
            //otherwise nothing to do :-}
            return Redirect("/");
        }

        [Authorize]
        public async Task<IActionResult> Account()
        {
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            //log
            //call api
            Logger.Info("API request data for account id: {0}", userId);

            ApiResponseDto result = await _httpClientHelper.GetAsync<ApiResponseDto>(_apiUrl + $"/api/Account/{userId}");
            Logger.Info("API response for account id: {0}|{1}", userId, Newtonsoft.Json.JsonConvert.SerializeObject(result));

            if (result.Code != 200)
            {
                //signup ok, show user success page,
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
            return View(accountViewModel);
        }

    }
}
