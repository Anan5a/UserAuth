using DataAccess.IRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
using Models;
using System.Security.Claims;
using UserAuth.Utility;
using Utility;
using UserAuth.Models;

namespace UserAuth.Controllers
{
    public class UserController : Controller
    {
        private readonly PasswordManager _passwordManager;
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserController(IUserRepository userRepository, PasswordManager passwordManager, IConfiguration configuration)
        {
            _passwordManager = passwordManager;
            _userRepository = userRepository;
            _configuration = configuration;

        }
        [Route("User/Signup")]
        [Route("User")]
        public IActionResult Index()
        {
            return View("Signup");
        }


        [HttpPost]
        [Route("User/Signup")]
        [Route("User")]
        public IActionResult Index([FromForm] UserSignupDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return View("Signup", newUser);
            }

            User user = new User
            {
                Name = newUser.Name,
                Email = newUser.Email,
                Password = _passwordManager.HashPassword(newUser.Password),
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
            };
            int id = _userRepository.Add(user);

            if (id == 0)
            {
                ModelState.AddModelError("", "User creation failed. Something catastrophically went wrong!");
                return View("Signup", newUser);
            }

            return RedirectToAction("Login");
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
        public IActionResult Login([FromForm] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }

            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Email"] = loginDto.Email;

            User? existingUser = _userRepository.Get(condition, includeProperties: "true");
            if (existingUser == null)
            {
                ModelState.AddModelError("", "No user was found");
                return View(loginDto);
            }
            if (!_passwordManager.VerifyPassword(loginDto.Password, existingUser.Password))
            {
                ModelState.AddModelError("", "Authentication failed. Invalid username or password");
                return View(loginDto);
            }
            //valid user, login using cookie

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
        public IActionResult Account()
        {
            long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId);
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = userId;

            User? existingUser = _userRepository.Get(condition, includeProperties: "true");
            if (existingUser == null)
            {

                return Redirect("/Home/Error");
            }
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
