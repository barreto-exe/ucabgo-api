using MailKit.Net.Smtp;
using MimeKit;
using System;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;

namespace UcabGo.Application.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings mailSettings;
        private readonly IUserService userService;
        public MailService(MailSettings mailSettings, IUserService userService)
        {
            this.mailSettings = mailSettings;
            this.userService = userService;
        }

        public bool SendMail(MailData mailData)
        {
            try
            {
                using MimeMessage emailMessage = new();

                MailboxAddress emailFrom = new(mailSettings.SenderName, mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new(mailData.EmailToName, mailData.EmailToId);
                emailMessage.To.Add(emailTo);
                
                emailMessage.Subject = mailData.EmailSubject;

                BodyBuilder emailBodyBuilder = new()
                {
                    HtmlBody = mailData.EmailBody
                };

                emailMessage.Body = emailBodyBuilder.ToMessageBody();

                using SmtpClient mailClient = new();
                
                mailClient.Connect(mailSettings.Server, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                mailClient.Authenticate(mailSettings.UserName, mailSettings.Password);
                mailClient.Send(emailMessage);
                mailClient.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                // Exception Details
                return false;
            }
        }

        public async Task<bool> SendNewValidationMail(string email, string validationUrl)
        {
            var userDb = await userService.GetByEmail(email);
            
            userDb.ValidationGuid = Guid.NewGuid().ToString();
            
            await userService.Update(userDb);

            //Get html format from file in Utils/MailTemplate.html
            string html = (await new HttpClient()
                .GetStringAsync("https://raw.githubusercontent.com/barreto-exe/ucabgo-api/main/UcabGo.Application/Utils/MailTemplate.html"))
                .Replace("@User", $"{userDb.Name}")
                .Replace("@Url", $"{validationUrl}/?ValidationEmail={email}&ValidationGuid={userDb.ValidationGuid}");

            //Send email
            return SendMail(new MailData()
            {
                EmailToId = email,
                EmailToName = userDb.Name,
                EmailSubject = "Registro de usuario",
                EmailBody = html
            });
        }
    }
}
