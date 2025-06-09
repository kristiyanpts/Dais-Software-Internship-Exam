using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FinalProject.Web.Models.ViewModels.Error;
using FinalProject.Web.Controllers.Base;
using FinalProject.Services.Interfaces.UserAccount;
using FinalProject.Services.DTOs.Requests.UserAccount.GetAccountsForAuthUser;
using FinalProject.Web.Models.ViewModels.Home.Index;
using FinalProject.Web.Models.ViewModels.Home;

namespace FinalProject.Web.Controllers.Home
{
    public class HomeController : BaseController
    {
        private readonly IUserAccountService _userAccountService;

        public HomeController(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public async Task<IActionResult> Index()
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var request = new GetAccountsForAuthUserRequest()
            {
                User = GetUserSessionData()
            };

            var response = await _userAccountService.GetAccountsForAuthUser(request);

            if (!response.Status)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = response.Message,
                };

                return View("Error", errorViewModel);
            }

            var accounts = response.Data.Select(x => new AccountViewModel()
            {
                Id = x.Id,
                AccountNumber = x.AccountNumber,
                Balance = x.Balance
            }).ToList();

            var viewModel = new IndexHomeViewModel()
            {
                Accounts = accounts
            };

            return View(viewModel);
        }
    }
}
