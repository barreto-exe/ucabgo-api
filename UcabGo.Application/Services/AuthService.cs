using AutoMapper;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Utils;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.Location.Inputs;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Entities;

namespace UcabGo.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper mapper;
        private readonly IUserService userService;
        private readonly ILocationService locationService;
        private readonly IMailService mailService;
        public AuthService(IMapper mapper, IUserService userService, ILocationService locationService, IMailService mailService)
        {
            this.mapper = mapper;
            this.userService = userService;
            this.locationService = locationService;
            this.mailService = mailService;
        }

        public async Task Register(RegisterInput input)
        {
            var userInput = mapper.Map<User>(input);

            if (await userService.GetByEmail(userInput.Email) != null)
            {
                throw new Exception("USER_ALREADY_EXISTS");
            }

            if (input.Name.Length > 32|| input.LastName.Length > 32) 
            {    
                throw new Exception("REGISTER_FIELD_LENGTH");
            }

            //Create validation guid and send email
            var validationGuid = Guid.NewGuid().ToString();
            userInput.ValidationGuid = validationGuid;

            var newUser = await userService.Create(userInput);

            //Send validation email
            await mailService.SendNewValidationMail(newUser.Email, input.ValidationUrl);

            //Create default UCAB location
            await locationService.Create(new LocationInput()
            {
                Email = newUser.Email,
                Alias = "UCAB Guayana",
                Zone = "Av. Atlántico",
                Detail = "Ciudad Guayana",
                Latitude = 8.29727428f,
                Longitude = -62.71308436f,
                IsHome = false,
            }, isRegistering: true);

            //return await Login(new LoginInput()
            //{
            //    Email = input.Email,
            //    Password = input.Password
            //});
        }

        public async Task<LoginDto> Login(LoginInput input)
        {
            var user = await userService.GetByEmailAndPass(input);
            if (user == null)
            {
                throw new Exception("WRONG_CREDENTIALS");
            }
            if(!user.IsValidated)
            {
                throw new Exception("USER_NOT_VALIDATED");
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

        public async Task<string> ValidateUser(string email, string guid)
        {
            var user = await userService.GetByEmail(email);
            if(user == null || guid == null || user.ValidationGuid != guid || user.IsValidated)
            {
                throw new Exception("NOT_FOUND");
            }

            user.IsValidated = true;
            await userService.Update(user);

            string html = File
                .ReadAllText("Utils/MailSuccess.html")
                .Replace("@User", $"{user.Name}");

            return html;
        }

        public async Task RequestNewValidationGuid(string email, string validationUrl)
        {
            var user = await userService.GetByEmail(email);
            if (user == null || user.IsValidated)
            {
                throw new Exception("NOT_FOUND");
            }

            //Create validation guid and send email
            var validationGuid = Guid.NewGuid().ToString();
            user.ValidationGuid = validationGuid;

            await userService.Update(user);

            //Send validation email
            await mailService.SendNewValidationMail(user.Email, validationUrl);
        }
    }
}
