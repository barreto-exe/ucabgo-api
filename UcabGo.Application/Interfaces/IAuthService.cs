using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginDto> Register(RegisterInput input);
        Task<LoginDto> Login(LoginInput input);
        public static ErrorDto ErrorCodeBuilder(string errorCode)
        {
            return new ErrorDto()
            {
                Code = errorCode,
            };
        }
    }
}