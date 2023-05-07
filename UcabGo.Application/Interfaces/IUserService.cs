using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetById(int id);
        Task<UserDto> GetByEmail(string email);
        Task<UserDto> GetByEmailAndPass(LoginInput login);
        Task<UserDto> Create(User user);
        Task<UserDto> Update(User user);
        Task Delete(int id);

    }
}
