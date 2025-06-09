using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.User;
using FinalProject.Services.DTOs.Requests.User.GetUserByEmail;
using FinalProject.Services.DTOs.Responses.User;
using FinalProject.Services.DTOs.Responses.User.GetUserByEmail;
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

        // TODO: Currently not used. If it won't be used, remove it, and if there aren't any methods left, remove the service.
        public async Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request)
        {
            if (request == null)
            {
                return new GetUserByEmailResponse()
                {
                    Status = false,
                    Message = "Request is null!"
                };
            }

            if (request.Data == null)
            {
                return new GetUserByEmailResponse()
                {
                    Status = false,
                    Message = "Data is null!"
                };
            }

            try
            {
                var queryParameters = new QueryParameters();
                queryParameters.AddWhere("email", request.Data.Email);

                var user = await _userRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();

                if (user == null)
                {
                    return new GetUserByEmailResponse()
                    {
                        Status = false,
                        Message = "User not found!"
                    };
                }

                var userResponse = await MapUserToUserResponseDto(user);

                return new GetUserByEmailResponse()
                {
                    Status = true,
                    Message = "User retrieved successfully!",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new GetUserByEmailResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        // TODO: Change to non-async method, if no fields are needed to be mapped.
        private async Task<UserResponseDto> MapUserToUserResponseDto(Models.User user)
        {
            return new UserResponseDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };
        }
    }
}