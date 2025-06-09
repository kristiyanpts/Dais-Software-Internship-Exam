using FinalProject.Services.DTOs.Requests.User.GetUserByUsername;
using FinalProject.Services.DTOs.Responses.User.GetUserByUsername;

namespace FinalProject.Services.Interfaces.User
{
    public interface IUserService
    {
        Task<GetUserByUsernameResponse> GetUserByUsername(GetUserByUsernameRequest request);
    }
}