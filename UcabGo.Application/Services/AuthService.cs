using AutoMapper;
using System.Runtime.CompilerServices;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.Destination.Inputs;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly IDestinationService destinationService;
        private readonly ILocationService locationService;
        public AuthService(IMapper mapper, IUserService userService, IDestinationService destinationService, ILocationService locationService)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.destinationService = destinationService;
            this.locationService = locationService;
        }

        public async Task<LoginDto> Register(RegisterInput input)
        {
            var userInput = mapper.Map<User>(input);

            if (await userService.GetByEmail(userInput.Email) != null)
            {
                throw new Exception("USER_ALREADY_EXISTS");
            }

            var newUser = await userService.Create(userInput);

            //Create default UCAB destination for drivers
            await destinationService.Create(new DestinationInput()
            {
                Email = newUser.Email,
                Alias = "UCAB Guayana",
                Zone = "UCAB Guayana",
                Detail = "UCAB Guayana",
                Latitude = 8.2970305f,
                Longitude = -62.7179975f,
            }, isRegistering: true);

            //Create default UCAB location for passangers
            await locationService.Create(new LocationInput()
            {
                Email = newUser.Email,
                Alias = "UCAB Guayana",
                Zone = "UCAB Guayana",
                Detail = "UCAB Guayana",
                Latitude = 8.2970305f,
                Longitude = -62.7179975f,
                IsHome = false,
            }, isRegistering: true);

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
                throw new Exception("WRONG_CREDENTIALS");
            }

            var userDto = mapper.Map<UserDto>(user);

            return new LoginDto()
            {
                User = userDto,
                Token = userDto.ObtainToken(),
            };
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            var user = await userService.GetByEmailAndPass(new LoginInput()
            {
                Email = input.Email,
                Password = input.OldPassword
            });

            if (user == null)
            {
                throw new Exception("USER_NOT_FOUND");
            }

            user.Password = input.NewPassword;
            await userService.Update(user);
        }
    }
}
