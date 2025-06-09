using FinalProject.Services.Interfaces.Authentication;
using FinalProject.Repository.Interfaces.User;
using FinalProject.Repository.Helpers;
using FinalProject.Services.Helpers;
using FinalProject.Services.DTOs.Responses.User;
using FinalProject.Services.DTOs.Requests.Authentication.Login;
using FinalProject.Services.DTOs.Responses.Authentication.Login;
using FinalProject.Services.DTOs.Responses.Authentication.Register;
using FinalProject.Services.DTOs.Requests.Authentication.Register;

namespace FinalProject.Services.Implementations.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Data.Username) || string.IsNullOrEmpty(request.Data.Password))
            {
                return new LoginResponse()
                {
                    Status = false,
                    Message = "Невалидно потребителско име или парола!"
                };
            }

            var queryParameters = new QueryParameters();
            queryParameters.AddWhere("username", request.Data.Username);

            var user = await _userRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();
            var passwordHash = SecurityHelper.HashPassword(request.Data.Password);

            if (user == null || user.Password != passwordHash)
            {
                return new LoginResponse()
                {
                    Status = false,
                    Message = "Невалидно потребителско име или парола!"
                };
            }

            try
            {
                var userResponse = MapUserToUserResponseDto(user);

                return new LoginResponse()
                {
                    Status = true,
                    Message = "Входът е успешен!",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<RegisterResponse> Register(RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Data.Username) || string.IsNullOrEmpty(request.Data.Password) || string.IsNullOrEmpty(request.Data.FullName) || string.IsNullOrEmpty(request.Data.ConfirmPassword))
            {
                return new RegisterResponse()
                {
                    Status = false,
                    Message = "Всички полета са задължителни!"
                };
            }

            var queryParameters = new QueryParameters();
            queryParameters.AddWhere("username", request.Data.Username);

            var existingUser = await _userRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();
            if (existingUser != null)
            {
                return new RegisterResponse()
                {
                    Status = false,
                    Message = "Потребител с това потребителско име вече съществува!"
                };
            }

            var user = new Models.User()
            {
                Username = request.Data.Username,
                Password = SecurityHelper.HashPassword(request.Data.Password),
                FullName = request.Data.FullName,
            };

            try
            {
                var id = await _userRepository.Create(user);

                user.Id = id;

                var userResponse = MapUserToUserResponseDto(user);

                return new RegisterResponse()
                {
                    Status = true,
                    Message = "Потребителят е регистриран успешно!",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new RegisterResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        private UserResponseDto MapUserToUserResponseDto(Models.User user)
        {
            return new UserResponseDto()
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
            };
        }
    }
}