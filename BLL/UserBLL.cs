using BE.DTOs;
using IRepository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using UserAuth.Utility;
using BE;

namespace BLL
{
    public static class UserBLL
    {

        public static bool CreateUser(string connectionString, UserSignupDto newUser, IUserRepository _userRepository)
        {

            BE.User user = new BE.User
            {
                Name = newUser.Name,
                Email = newUser.Email,
                Password = PasswordManager.HashPassword(newUser.Password),
                CreatedAt = DateTime.Now,
                ModifiedAt = DateTime.Now,
            };
            int id = _userRepository.Add(connectionString,user);

            if (id == 0)
            {
                return false;
            }
            return true;
        }
        public static dynamic LoginUser(string connectionString, UserLoginDto loginDto, IUserRepository _userRepository)
        {

            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Email"] = loginDto.Email;

            BE.User? existingUser = _userRepository.Get(connectionString, condition, includeProperties: "true");
            if (existingUser == null)
            {
                return null;
            }
            if (!PasswordManager.VerifyPassword(loginDto.Password, existingUser.Password))
            {
                return false;
            }
            //valid user


            return existingUser;

        }

        public static User AccountInfo(string connectionString, IUserRepository _userRepository, long userId)
        {
            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = userId;

            User? existingUser = _userRepository.Get(connectionString,condition, includeProperties: "true");
            if (existingUser == null)
            {
                return null;
            }
            
            return existingUser;
        }
    }
}
