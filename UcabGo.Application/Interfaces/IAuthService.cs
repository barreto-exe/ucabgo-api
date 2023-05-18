using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Application.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Register a user and returns a LoginDto with the user and a token.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserExistsException"></exception>
        Task<LoginDto> Register(RegisterInput input);
        /// <summary>
        /// Login a user and returns a LoginDto with the user and a token.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        Task<LoginDto> Login(LoginInput input);
        /// <summary>
        /// Change the password of a user.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        Task ChangePassword(ChangePasswordInput input);
    }
}