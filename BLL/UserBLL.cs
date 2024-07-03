using BE.DTOs;
using DAL.IRepository;
using UserAuth.Utility;
using BE;
using Newtonsoft.Json;
using BLL.Services;

namespace BLL
{
    public static class UserBLL
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string _emailTemplate = @"
<html lang='en'>
<head>
    <title>{{subject}}</title>
</head>
<body>
    <h1>{{title}}</h1>
    <p>{{content}}</p>
<h5>Thank you</h5>
<h5>Awesome Team</h5>
</body>
</html>
";


        public static bool CreateUser(string connectionString, UserSignupDto newUser, IUserRepository _userRepository, EmailService _emailService)
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
            int id = _userRepository.Add(connectionString, user);

            if (id == 0)
            {
                Logger.Info("User creation failed");
                Logger.Info("End UserBLL::CreateUser");

                return false;
            }
            Logger.Info("User creation successful");
            Logger.Info("End UserBLL::CreateUser");
            //send email
            string subject = "Welcome to Awesome Website";
            string emailTitle = $"Hi {newUser.Name},";
            string emailContent = $"Your account has been created successfully!<br>Your email address/username is: {newUser.Email}";
            string emailBody = _emailTemplate.Replace("{{subject}}", subject)
                .Replace("{{title}}", emailTitle)
                .Replace("{{content}}", emailContent);

            _emailService.SendEmailAsync(newUser.Email, subject, emailBody);
            //end send email

            return true;
        }

        public static User? UpdateUser(string connectionString, UserUpdateDto existingUser, IUserRepository _userRepository)
        {
            Logger.Debug("Starting UserBLL::UpdateUser with params:user:{0}", JsonConvert.SerializeObject(existingUser));
            //try authenticating the user password
            User? user = _userRepository.Get(connectionString, new Dictionary<string, dynamic> { { "Id", existingUser.Id } });

            if (user == null)
            {
                return null;
            }

            //now we can update, user provided valid password

            BE.User user2 = new BE.User
            {
                Id = (long)existingUser.Id,
                Name = existingUser.Name,
                Email = existingUser.Email,
                ModifiedAt = DateTime.Now,
            };
            User? updatedUser = _userRepository.Update(connectionString, user2);

            if (updatedUser == null)
            {
                Logger.Info("User update failed");
                Logger.Info("End UserBLL::UpdateUser");

                return null;
            }
            Logger.Info("User update successful");
            Logger.Info("End UserBLL::UpdateUser");

            return updatedUser;
        }
        public static User? UpdateUserPassword(string connectionString, ChangePasswordDto changePasswordDto, IUserRepository _userRepository, bool isResetting = false)
        {
            Logger.Debug("Starting UserBLL::UpdateUserPassword with params:user:{0}", JsonConvert.SerializeObject(changePasswordDto));
            //try authenticating the user password
            User? user = _userRepository.Get(connectionString, new Dictionary<string, dynamic> { { "Id", changePasswordDto.Id } });

            if (user == null)
            {
                return null;
            }

            if (!isResetting)
            {
                if (!PasswordManager.VerifyPassword(changePasswordDto.Password, user.Password))
                {
                    //invalid password
                    return null;
                }
            }
            //now we can update, user provided valid password

            BE.User user2 = new BE.User
            {
                Id = user.Id,
                ModifiedAt = DateTime.Now,
            };
            if (changePasswordDto.NewPassword != null)
            {
                user2.Password = PasswordManager.HashPassword(changePasswordDto.NewPassword);
            }
            User? updatedUser = _userRepository.Update(connectionString, user2);

            if (updatedUser == null)
            {
                Logger.Info("User UpdateUserPassword failed");
                Logger.Info("End UserBLL::UpdateUserPassword");

                return null;
            }
            Logger.Info("User update successful");
            Logger.Info("End UserBLL::UpdateUserPassword");

            return updatedUser;
        }
        public static User? ForgotPassword(string connectionString, ForgotPasswordDto forgotPasswordDto, IUserRepository _userRepository, IPasswordResetRepository _passwordResetRepository, EmailService _emailService)
        {
            Logger.Debug("Starting UserBLL::ForgotPassword with params:user:{0}", JsonConvert.SerializeObject(forgotPasswordDto));
            //try authenticating the user password
            User? user = _userRepository.Get(connectionString, new Dictionary<string, dynamic> { { "Email", forgotPasswordDto.Email } });

            if (user == null)
            {
                return null;
            }

            //generate a random token
            PasswordResetEntity passwordResetEntity = new PasswordResetEntity
            {
                Token = RandomStringGenerator.GenerateRandomString(64),
                UserId = user.Id,
                ExpiresAt = DateTime.Now.AddHours(6),//valid for 6 hours
            };


            _passwordResetRepository.Add(connectionString, passwordResetEntity);

            //send email
            string subject = "Reset your password";
            string emailTitle = $"Hi {user.Name},";
            string emailContent = $"A password reset request was received<br>Follow this link to reset your password: <a href='https://localhost:7158/User/ResetPassword/{passwordResetEntity.Token}'>https://localhost:7158/User/ResetPassword/{passwordResetEntity.Token}</a>"
                + "<br>This link is valid for 6 hours.";
            string emailBody = _emailTemplate.Replace("{{subject}}", subject)
                .Replace("{{title}}", emailTitle)
                .Replace("{{content}}", emailContent);

            _emailService.SendEmailAsync(user.Email, subject, emailBody);




            Logger.Info("User reset link send successful");
            Logger.Info("End UserBLL::ForgotPassword");

            return user;
        }

        public static User? GetUser(string connectionString, long userId, IUserRepository _userRepository)
        {
            Logger.Debug("Starting UserBLL::GetUser with params:user:{0}", userId);

            User? user = _userRepository.Get(connectionString, new Dictionary<string, dynamic> { { "Id", userId } });

            if (user == null)
            {
                Logger.Info("User get failed");
                Logger.Info("End UserBLL::GetUser");

                return null;
            }
            Logger.Info("User get successful");
            Logger.Info("End UserBLL::GetUser");

            return user;
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

            User? existingUser = _userRepository.Get(connectionString, condition, includeProperties: "true");
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
        public static User? ResetUserPassword(string connectionString, ResetPasswordDto resetPasswordDto, IUserRepository _userRepository, IPasswordResetRepository _passwordResetRepository, bool isResetting = false)
        {
            Logger.Debug("Starting UserBLL::ResetUserPassword with params:user:{0}", JsonConvert.SerializeObject(resetPasswordDto));
            //try authenticating the reset link

            PasswordResetEntity? passwordResetEntity = _passwordResetRepository.Get(connectionString, new Dictionary<string, dynamic> { { "Token", resetPasswordDto.Token } }, includeProperties: "yes");

            if (passwordResetEntity == null)
            {
                Logger.Info("User ResetUserPassword failed, Token not found.");
                Logger.Info("End UserBLL::ResetUserPassword");
                return null;
            }
            //Remove from database immediately
            _passwordResetRepository.Remove(connectionString, (int)passwordResetEntity.Id);

            //check validity

            if (passwordResetEntity.ExpiresAt < DateTime.Now)
            {
                Logger.Info("User ResetUserPassword failed, Token expired");
                Logger.Info("End UserBLL::ResetUserPassword");
                return null;
            }

            BE.User user2 = new BE.User
            {
                Id = passwordResetEntity.UserId,
                ModifiedAt = DateTime.Now,
            };
            if (resetPasswordDto.NewPassword != null)
            {
                user2.Password = PasswordManager.HashPassword(resetPasswordDto.NewPassword);
            }
            User? updatedUser = _userRepository.Update(connectionString, user2);

            if (updatedUser == null)
            {
                Logger.Info("User ResetUserPassword failed");
                Logger.Info("End UserBLL::ResetUserPassword");

                return null;
            }




            Logger.Info("ResetUserPassword successful");
            Logger.Info("End UserBLL::ResetUserPassword");

            return updatedUser;
        }
    }
}
