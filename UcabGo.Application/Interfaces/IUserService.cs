﻿using Microsoft.Extensions.Logging;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.User.Inputs;
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

        Task<UserDto> UpdatePersonalInfo(UserUpdateInput input);
        Task<UserDto> UpdateWalkingDistance(WalkingInput input);
        Task<UserDto> UpdateProfilePicture(string userId, string url, ILogger logger = null);
    }
}
