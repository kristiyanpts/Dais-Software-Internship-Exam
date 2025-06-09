using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.User;
using FinalProject.Services.DTOs.Requests.User.GetUserByUsername;
using FinalProject.Services.DTOs.Responses.User;
using FinalProject.Services.DTOs.Responses.User.GetUserByUsername;
using FinalProject.Services.Interfaces.User;

namespace FinalProject.Services.Implementations.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserByUsernameResponse> GetUserByUsername(GetUserByUsernameRequest request)
        {
            if (request == null)
            {
                return new GetUserByUsernameResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.Data == null)
            {
                return new GetUserByUsernameResponse()
                {
                    Status = false,
                    Message = "Данните са невалидни!"
                };
            }

            try
            {
                var queryParameters = new QueryParameters();
                queryParameters.AddWhere("username", request.Data.Username);

                var user = await _userRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();

                if (user == null)
                {
                    return new GetUserByUsernameResponse()
                    {
                        Status = false,
                        Message = "Потребителят не е намерен!"
                    };
                }

                var userResponse = MapUserToUserResponseDto(user);

                return new GetUserByUsernameResponse()
                {
                    Status = true,
                    Message = "Потребителят е намерен успешно!",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new GetUserByUsernameResponse()
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