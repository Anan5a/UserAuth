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
            Logger.Debug("Starting UserBLL::CreateUser with params:user:{0}", JsonConvert.SerializeObject(newUser));
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
                Logger.Info("User creation failed");
                Logger.Info("End UserBLL::CreateUser");

                return false;
            }
            Logger.Info("User creation successful");
            Logger.Info("End UserBLL::CreateUser");

            return true;
        }
        public static dynamic LoginUser(string connectionString, UserLoginDto loginDto, IUserRepository _userRepository)
        {
            Logger.Debug("Starting UserBLL::LoginUser with params:user:{0}", JsonConvert.SerializeObject(loginDto));

            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Email"] = loginDto.Email;

            BE.User? existingUser = _userRepository.Get(connectionString, condition, includeProperties: "true");
            if (existingUser == null)
            {
                Logger.Info("Login failed");
                Logger.Info("End UserBLL::LoginUser");

                return null;
            }
            if (!PasswordManager.VerifyPassword(loginDto.Password, existingUser.Password))
            {
                Logger.Info("Login failed");
                Logger.Info("End UserBLL::LoginUser");

                return false;
            }
            //valid user

            Logger.Info("Login successful");
            Logger.Info("End UserBLL::LoginUser");

            return existingUser;

        }

        public static User AccountInfo(string connectionString, IUserRepository _userRepository, long userId)
        {
            Logger.Debug("Starting UserBLL::AccountInfo with params:userId:{0}", userId);

            Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>();
            condition["Id"] = userId;

            User? existingUser = _userRepository.Get(connectionString,condition, includeProperties: "true");
            if (existingUser == null)
            {
                Logger.Info("No user account found");
                Logger.Info("End UserBLL::AccountInfo");

                return null;
            }
            Logger.Info("Login successful");
            Logger.Info("End UserBLL::AccountInfo");

            return existingUser;
        }
    }
}
