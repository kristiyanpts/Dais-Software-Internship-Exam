using FinalProject.Services.DTOs.Requests.UserAccount.GetAccountsForAuthUser;
using FinalProject.Services.DTOs.Responses.UserAccount.GetAccountsForAuthUser;

namespace FinalProject.Services.Interfaces.UserAccount
{
    public interface IUserAccountService
    {
        Task<GetAccountsForAuthUserResponse> GetAccountsForAuthUser(GetAccountsForAuthUserRequest request);
    }
}