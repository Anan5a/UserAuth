using System.Net.Mail;
using System.Net;

namespace BLL.Services
{

    public class EmailService
    {
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public EmailService(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            Logger.Debug("Starting EmailService::SendMailAsync with params:{0}:{1}:{2}", toEmail,subject,body);


            using (SmtpClient smtpClient = new SmtpClient(_configuration["EmailService:Host"]))
            {
                smtpClient.Port = int.Parse(_configuration["EmailService:Port"]);
                smtpClient.Credentials = new NetworkCredential(_configuration["EmailService:UserName"], _configuration["EmailService:Password"]);
                smtpClient.EnableSsl = true;
                

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_configuration["EmailService:From"]);
                    mailMessage.To.Add(toEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        await smtpClient.SendMailAsync(mailMessage);
                        Logger.Debug("Send mail to:{0} was successful.", toEmail);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Failed to send email:" + ex.Message);
                    }
                }
            }
            Logger.Info("End EmailService::SendMailAsync");
        }
    }


}