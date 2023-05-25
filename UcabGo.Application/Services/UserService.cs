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
            var users = unitOfWork.UserRepository.GetAll();
            var user = users.Where(x => x.Email == login.Email && x.Password == login.Password).FirstOrDefault();
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

        public async Task<UserDto> UpdatePhone(PhoneInput input)
        {
            var user = await GetByEmail(input.Email);
            user.Phone = input.Phone;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
        public async Task<UserDto> UpdateWalkingDistance(WalkingInput input)
        {
            if(input.WalkingDistance < 0)
            {
                throw new Exception("NEGATIVE_NUMBER");
            }

            var user = await GetByEmail(input.Email);
            user.WalkingDistance = input.WalkingDistance;
            unitOfWork.UserRepository.Update(user);
            await unitOfWork.SaveChangesAsync();

            var userDto = mapper.Map<UserDto>(user);
            return userDto;
        }
    }
}
