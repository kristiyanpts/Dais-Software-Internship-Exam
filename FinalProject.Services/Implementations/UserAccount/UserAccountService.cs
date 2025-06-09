using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.Account;
using FinalProject.Repository.Interfaces.UserAccount;
using FinalProject.Services.DTOs.Requests.UserAccount.GetAccountsForAuthUser;
using FinalProject.Services.DTOs.Responses.UserAccount;
using FinalProject.Services.DTOs.Responses.UserAccount.GetAccountsForAuthUser;
using FinalProject.Services.Interfaces.UserAccount;

namespace FinalProject.Services.Implementations.UserAccount
{
    public class UserAccountService : IUserAccountService
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IAccountRepository _accountRepository;

        public UserAccountService(IUserAccountRepository userAccountRepository, IAccountRepository accountRepository)
        {
            _userAccountRepository = userAccountRepository;
            _accountRepository = accountRepository;
        }


        public async Task<GetAccountsForAuthUserResponse> GetAccountsForAuthUser(GetAccountsForAuthUserRequest request)
        {
            if (request == null)
            {
                return new GetAccountsForAuthUserResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.User == null)
            {
                return new GetAccountsForAuthUserResponse()
                {
                    Status = false,
                    Message = "Само логнати потребители могат да виждат сметките си!"
                };
            }

            try
            {
                var queryParameters = new QueryParameters();
                queryParameters.AddWhere("user_id", request.User.Id);

                var accountIds = await _userAccountRepository.RetrieveAll(queryParameters).Select(userAccount => userAccount.AccountId).ToListAsync();

                if (accountIds.Count == 0)
                {
                    return new GetAccountsForAuthUserResponse()
                    {
                        Status = true,
                        Message = "Няма сметки за този потребител!",
                        Data = new List<UserAccountResponseDto>()
                    };
                }

                var accounts = new List<Models.Account>();

                foreach (var accountId in accountIds)
                {
                    var account = await _accountRepository.Retrieve(accountId);

                    if (account != null)
                    {
                        accounts.Add(account);
                    }
                }

                if (accounts.Count == 0)
                {
                    return new GetAccountsForAuthUserResponse()
                    {
                        Status = true,
                        Message = "Няма сметки за този потребител!",
                        Data = new List<UserAccountResponseDto>()
                    };
                }

                var mappedAccounts = accounts.Select(MapAccountToUserAccountResponseDto).ToList();

                return new GetAccountsForAuthUserResponse()
                {
                    Status = true,
                    Message = "Сметките са успешно получени!",
                    Data = mappedAccounts
                };
            }

            catch (Exception ex)
            {
                return new GetAccountsForAuthUserResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        private UserAccountResponseDto MapAccountToUserAccountResponseDto(Models.Account account)
        {
            return new UserAccountResponseDto()
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                Balance = account.Balance
            };
        }
    }
}