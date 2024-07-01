using BE.DTOs;
using DAL.IRepository;
using UserAuth.Utility;
using BE;
using Newtonsoft.Json;

namespace BLL
{
    public static class UserBLL
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static bool CreateUser(string connectionString, UserSignupDto newUser, IUserRepository _userRepository)
        {
            Logger.Info("Add new user to database: {0}", JsonConvert.SerializeObject(newUser));
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
            Logger.Info("Login user info: {0}", JsonConvert.SerializeObject(loginDto));

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
            Logger.Info("Account info fro ID: {0}, {1}",userId, JsonConvert.SerializeObject(existingUser));

            if (existingUser == null)
            {
                return null;
            }
            
            return existingUser;
        }
    }
}
