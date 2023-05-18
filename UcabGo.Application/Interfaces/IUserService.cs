﻿using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> GetById(int id);
        Task<User> GetByEmail(string email);
        Task<User> GetByEmailAndPass(LoginInput login);
        Task<UserDto> Create(User user);
        Task<UserDto> Update(User user);
        Task Delete(int id);
    }
}
