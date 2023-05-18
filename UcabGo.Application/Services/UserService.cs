using AutoMapper;
using UcabGo.Application.Interfaces;
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
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<User> GetById(int id)
        {
            var users = unitOfWork.UserRepository.GetAll().Result;
            var user = users.Where(x => x.Id == id).FirstOrDefault();
            return user;
        }
        public async Task<User> GetByEmail(string email)
        {
            var user = (await unitOfWork.UserRepository
                .GetAll())
                .FirstOrDefault(x => x.Email == email);
            return user;
        }
        public async Task<User> GetByEmailAndPass(LoginInput login)
        {
            var user = (await unitOfWork.UserRepository
                .GetAll())
                .FirstOrDefault(x => x.Email == login.Email && x.Password == login.Password);
            return user;
        }
        public async Task<UserDto> Create(User user)
        {
            await unitOfWork.UserRepository.Add(user);
            await unitOfWork.SaveChangesAsync();

            var userDb = await GetByEmail(user.Email);

            var userDto = mapper.Map<UserDto>(userDb);
            return userDto;
        }
        public async Task<UserDto> Update(User user)
        {
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            var userDb = await GetByEmail(user.Email);

            var userDto = mapper.Map<UserDto>(userDb);
            return userDto;
        }
        public async Task Delete(int id)
        {
            await unitOfWork.UserRepository.Delete(id);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePhone(PhoneInput input)
        {
            var user = await GetByEmail(input.Email);
            user.Phone = input.Phone;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
