using AutoMapper;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;
using UcabGo.Core.Interfaces;

namespace UcabGo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserService userService;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userService = userService;
        }

        public async Task<LoginDto> Register(RegisterInput input)
        {
            var userInput = mapper.Map<User>(input);

            if (await userService.GetByEmail(userInput.Email) != null)
            {
                throw new UserExistsException();
            }

            await userService.Create(userInput);

            return await Login(new LoginInput()
            {
                Email = input.Email,
                Password = input.Password
            });
        }

        public async Task<LoginDto> Login(LoginInput input)
        {
            var user = await userService.GetByEmailAndPass(input);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return new LoginDto()
            {
                User = user,
                Token = user.ObtainToken(),
            };
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            var userDto = await userService.GetByEmailAndPass(new LoginInput()
            {
                Email = input.Email,
                Password = input.OldPassword
            });

            if (userDto == null)
            {
                throw new UserNotFoundException();
            }

            var user = mapper.Map<User>(userDto);
            user.Password = input.NewPassword;

            await userService.Update(user); 
        }
    }
}
