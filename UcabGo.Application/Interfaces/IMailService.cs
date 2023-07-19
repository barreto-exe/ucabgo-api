using UcabGo.Application.Utils;

namespace UcabGo.Application.Interfaces
{
    public interface IMailService
    {
        bool SendMail(MailData mailData);
        Task<bool> SendNewValidationMail(string email, string validationUrl);
    }
}
