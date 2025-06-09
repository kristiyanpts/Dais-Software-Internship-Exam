using FinalProject.Services.DTOs.Requests.User.GetUserByEmail;
using FinalProject.Services.DTOs.Responses.User.GetUserByEmail;

namespace FinalProject.Services.Interfaces.User
{
    public interface IUserService
    {
        Task<GetUserByEmailResponse> GetUserByEmail(GetUserByEmailRequest request);
        //Task<CreateUserResponse> CreateUserAsync(CreateUserRequest request);
    }
}