using Microsoft.AspNetCore.Mvc;
using FinalProject.Services.Interfaces.Payment;
using FinalProject.Web.Controllers.Base;
using FinalProject.Services.DTOs.Requests.Payment.GetPaymentsForAuthUser;
using FinalProject.Web.Models.ViewModels.Error;
using FinalProject.Web.Models.ViewModels.Payment.Index;
using FinalProject.Web.Models.ViewModels.Payment;
using FinalProject.Services.DTOs.Requests.UserAccount.GetAccountsForAuthUser;
using FinalProject.Services.Interfaces.UserAccount;
using FinalProject.Web.Models.ViewModels.Home;
using FinalProject.Web.Models.ViewModels.Payment.Create;
using FinalProject.Services.DTOs.Requests.Payment.CreatePayment;
using FinalProject.Services.DTOs.Requests.Payment.SendPaymentById;
using FinalProject.Services.DTOs.Requests.Payment.CancelPaymentById;

namespace FinalProject.Web.Controllers.Payment
{
    [Route("[controller]")]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IUserAccountService _userAccountService;

        public PaymentController(IPaymentService paymentService, IUserAccountService userAccountService)
        {
            _paymentService = paymentService;
            _userAccountService = userAccountService;
        }

        [HttpGet]
        [Route("{accountId?}")]
        public async Task<IActionResult> Index(int? accountId)
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var request = new GetPaymentsForAuthUserRequest()
            {
                User = GetUserSessionData()
            };

            var response = await _paymentService.GetPaymentsForAuthUser(request);

            if (!response.Status)
            {
                var errorViewModel = new ErrorViewModel()
                {
                    RequestId = response.Message,
                };

                return View("Error", errorViewModel);
            }

            var payments = response.Data.Select(x => new PaymentViewModel()
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = x.UserName,
                FromAccountId = x.FromAccountId,
                FromAccountNumber = x.FromAccountNumber,
                ToAccountId = x.ToAccountId,
                ToAccountNumber = x.ToAccountNumber,
                Amount = x.Amount,
                Description = x.Description,
                Status = (PaymentStatus)x.Status,
                CreatedAt = x.CreatedAt
            }).ToList();

            if (accountId.HasValue && accountId.Value != 0)
            {
                payments = payments.Where(x => x.FromAccountId == accountId.Value).ToList();
            }

            var viewModel = new IndexPaymentViewModel()
            {
                Payments = payments
            };

            return View(viewModel);
        }

        [HttpGet]
        [Route("create/{accountId?}")]
        public async Task<IActionResult> Create(int? accountId)
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
                Balance = x.Balance,
            }).ToList();

            var viewModel = new CreatePaymentViewModel()
            {
                Accounts = accounts,
                FromAccountNumber = accountId.HasValue ? accounts.FirstOrDefault(x => x.Id == accountId.Value)?.AccountNumber : null,
            };

            return View(viewModel);
        }

        [HttpPost]
        [Route("create/{accountId?}")]
        public async Task<IActionResult> Create(CreatePaymentViewModel viewModel)
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            if (string.IsNullOrEmpty(viewModel.FromAccountNumber) || string.IsNullOrEmpty(viewModel.ToAccountNumber) || viewModel.Amount <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please fill in all fields");

                var accountsRequest = new GetAccountsForAuthUserRequest()
                {
                    User = GetUserSessionData()
                };

                var accountsResponse = await _userAccountService.GetAccountsForAuthUser(accountsRequest);

                if (!accountsResponse.Status)
                {
                    var errorViewModel = new ErrorViewModel()
                    {
                        RequestId = accountsResponse.Message,
                    };

                    return View("Error", errorViewModel);
                }

                var accounts = accountsResponse.Data.Select(x => new AccountViewModel()
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Balance = x.Balance,
                }).ToList();

                viewModel.Accounts = accounts;

                return View(viewModel);
            }

            var request = new CreatePaymentRequest()
            {
                User = GetUserSessionData(),
                Data = new CreatePaymentDto()
                {
                    FromAccountNumber = viewModel.FromAccountNumber,
                    ToAccountNumber = viewModel.ToAccountNumber,
                    Amount = viewModel.Amount,
                    Description = viewModel.Description,
                }
            };

            var response = await _paymentService.CreatePayment(request);

            if (!response.Status)
            {
                ModelState.AddModelError(string.Empty, response.Message);

                var accountsRequest = new GetAccountsForAuthUserRequest()
                {
                    User = GetUserSessionData()
                };

                var accountsResponse = await _userAccountService.GetAccountsForAuthUser(accountsRequest);

                if (!accountsResponse.Status)

                    if (!accountsResponse.Status)
                    {
                        var errorViewModel = new ErrorViewModel()
                        {
                            RequestId = accountsResponse.Message,
                        };

                        return View("Error", errorViewModel);
                    }

                var accounts = accountsResponse.Data.Select(x => new AccountViewModel()
                {
                    Id = x.Id,
                    AccountNumber = x.AccountNumber,
                    Balance = x.Balance,
                }).ToList();

                viewModel.Accounts = accounts;

                return View(viewModel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("process-payment/{id}")]
        public async Task<IActionResult> ProcessPayment(int id)
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var request = new SendPaymentByIdRequest()
            {
                User = GetUserSessionData(),
                Data = new SendPaymentByIdDto()
                {
                    Id = id,
                }
            };

            var response = await _paymentService.SendPaymentById(request);

            if (!response.Status)
            {
                return View("Error", new ErrorViewModel() { RequestId = response.Message });
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("cancel-payment/{id}")]
        public async Task<IActionResult> CancelPayment(int id)
        {
            if (GetUserId() == 0)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var request = new CancelPaymentByIdRequest()
            {
                User = GetUserSessionData(),
                Data = new CancelPaymentByIdDto()
                {
                    Id = id,
                }
            };

            var response = await _paymentService.CancelPaymentById(request);

            if (!response.Status)
            {
                return View("Error", new ErrorViewModel() { RequestId = response.Message });
            }

            return RedirectToAction("Index");
        }
    }
}