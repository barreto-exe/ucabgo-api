using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<UserDto> GetById(int id)
        {
            var users = unitOfWork.UserRepository.GetAll().Result;
            var user = users.Where(x => x.Id == id).FirstOrDefault();
            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> GetByEmail(string email)
        {
            var user = (await unitOfWork.UserRepository
                .GetAll())
                .FirstOrDefault(x => x.Email == email);
            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> GetByEmailAndPass(LoginInput login)
        {
            var user = (await unitOfWork.UserRepository
                .GetAll())
                .FirstOrDefault(x => x.Email == login.Email && x.Password == login.Password);
            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> Create(User user)
        {
            await unitOfWork.UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();
            return await GetByEmail(user.Email);
        }
        public Task<UserDto> Update(User user)
        {
            throw new NotImplementedException();
        }
        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
