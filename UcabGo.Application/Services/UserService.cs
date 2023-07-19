using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.User.Inputs;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger logger;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<User> GetById(int id)
        {
            var users = unitOfWork.UserRepository.GetAll();
            var user = users.Where(x => x.Id == id).FirstOrDefault();
            return user;
        }
        public async Task<User> GetByEmail(string email)
        {
            var users = unitOfWork.UserRepository.GetAll();
            var user = users.Where(x => x.Email == email).FirstOrDefault();
            return user;
        }
        public async Task<User> GetByEmailAndPass(LoginInput login)
        {
            login.Password = EncodePassword(login.Password);

            var users = unitOfWork.UserRepository.GetAll();
            var user = users.Where(x => x.Email == login.Email && x.Password == login.Password).FirstOrDefault();
            return user;
        }
        public async Task<UserDto> Create(User user)
        {
            user.Password = EncodePassword(user.Password);
            user.Phone = user?.Phone?.FormatPhone();

            await unitOfWork.UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();

            var userDb = await GetByEmail(user.Email);

            var userDto = mapper.Map<UserDto>(userDb);
            return userDto;
        }
        public async Task<UserDto> Update(User user)
        {
            var userDb = await GetByEmail(user.Email);

            bool isPasswordChanged = userDb?.Password != user?.Password;
            userDb.Password = isPasswordChanged ? EncodePassword(user.Password) : userDb.Password;
            userDb.Phone = user?.Phone?.FormatPhone();

            unitOfWork.UserRepository.Update(userDb);
            await unitOfWork.SaveChangesAsync();


            var userDto = mapper.Map<UserDto>(userDb);
            return userDto;
        }
        public async Task Delete(int id)
        {
            await unitOfWork.UserRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<UserDto> UpdatePersonalInfo(UserUpdateInput input)
        {
            var user = mapper.Map<User>(input);
            var userDto = await Update(user);
            return userDto;
        }
        public async Task<UserDto> UpdateWalkingDistance(WalkingInput input)
        {
            if (input.WalkingDistance <= 0)
            {
                throw new Exception("LIMIT_REACHED");
            }
            if (input.WalkingDistance > 10000)
            {
                throw new Exception("LIMIT_REACHED");
            }

            var user = await GetByEmail(input.Email);
            user.WalkingDistance = input.WalkingDistance;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> UpdateProfilePicture(string userEmail, string url, ILogger logger = null)
        {
            var user = await GetByEmail(userEmail);
            user.ProfilePicture = url;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            if (logger != null) logger.LogError($"Profile picture updated for user {userEmail}. URL: {url}");

            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }

        private static string EncodePassword(string password)
        {
            using MD5 md5Hash = MD5.Create();

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder sb = new();
            for (int i = 0; i < data.Length; i++)
            {
                sb.Append(data[i].ToString("x2"));
            }

            return sb.ToString();
        }
    }
}
