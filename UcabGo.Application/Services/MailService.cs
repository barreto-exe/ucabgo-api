using MailKit.Net.Smtp;
using MimeKit;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;

namespace UcabGo.Application.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings mailSettings;
        public MailService(MailSettings mailSettings)
        {
            this.mailSettings = mailSettings;
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
                    TextBody = mailData.EmailBody
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
    }
}
