using FinalProject.Repository.Helpers;
using FinalProject.Repository.Interfaces.Account;
using FinalProject.Repository.Interfaces.Payment;
using FinalProject.Repository.Interfaces.User;
using FinalProject.Repository.Interfaces.UserAccount;
using FinalProject.Services.DTOs.Requests.Payment.CancelPaymentById;
using FinalProject.Services.DTOs.Requests.Payment.CreatePayment;
using FinalProject.Services.DTOs.Requests.Payment.GetPaymentsForAuthUser;
using FinalProject.Services.DTOs.Requests.Payment.SendPaymentById;
using FinalProject.Services.DTOs.Responses.Payment;
using FinalProject.Services.DTOs.Responses.Payment.CancelPaymentById;
using FinalProject.Services.DTOs.Responses.Payment.CreatePayment;
using FinalProject.Services.DTOs.Responses.Payment.GetPaymentsForAuthUser;
using FinalProject.Services.DTOs.Responses.Payment.SendPaymentById;
using FinalProject.Services.Interfaces.Payment;

namespace FinalProject.Services.Implementations.Payment
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountRepository _userAccountRepository;

        public PaymentService(IPaymentRepository paymentRepository, IAccountRepository accountRepository, IUserRepository userRepository, IUserAccountRepository userAccountRepository)
        {
            _paymentRepository = paymentRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _userAccountRepository = userAccountRepository;
        }

        public async Task<GetPaymentsForAuthUserResponse> GetPaymentsForAuthUser(GetPaymentsForAuthUserRequest request)
        {
            if (request == null)
            {
                return new GetPaymentsForAuthUserResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.User == null)
            {
                return new GetPaymentsForAuthUserResponse()
                {
                    Status = false,
                    Message = "Само логнати потребители могат да виждат плащанията си!"
                };
            }

            try
            {
                var queryParameters = new QueryParameters();
                queryParameters.AddWhere("user_id", request.User.Id);

                var payments = await _paymentRepository.RetrieveAll(queryParameters).ToListAsync();

                if (payments.Count == 0)
                {
                    return new GetPaymentsForAuthUserResponse()
                    {
                        Status = true,
                        Message = "Няма плащания за този потребител!",
                        Data = new List<PaymentResponseDto>()
                    };
                }

                var mappedPayments = await Task.WhenAll(payments.Select(MapPaymentToPaymentResponseDto));

                return new GetPaymentsForAuthUserResponse()
                {
                    Status = true,
                    Message = "Плащанията са успешно получени!",
                    Data = mappedPayments
                };
            }

            catch (Exception ex)
            {
                return new GetPaymentsForAuthUserResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request)
        {
            if (request == null)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.User == null)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Само логнати потребители могат да създават плащания!"
                };
            }

            if (request.Data == null)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Данните за плащането са невалидни!"
                };
            }

            if (string.IsNullOrEmpty(request.Data.FromAccountNumber) || string.IsNullOrEmpty(request.Data.ToAccountNumber))
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Сметките не могат да бъдат празни!"
                };
            }

            if (string.IsNullOrEmpty(request.Data.Description))
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Основанието е задължително!"
                };
            }

            if (request.Data.FromAccountNumber == request.Data.ToAccountNumber)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Сметките не могат да бъдат еднакви!"
                };
            }

            if (request.Data.Amount <= 0)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Сума трябва да бъде положителна!"
                };
            }

            if (request.Data.FromAccountNumber.Length != 22 || request.Data.ToAccountNumber.Length != 22)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Сметките трябва да имат 22 символа!"
                };
            }

            if (request.Data.Description.Length > 32)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = "Основанието не може да надвишава 32 символа!"
                };
            }

            try
            {
                var queryParameters = new QueryParameters();
                queryParameters.AddWhere("account_number", request.Data.FromAccountNumber);
                var fromAccount = await _accountRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();

                if (fromAccount == null)
                {
                    return new CreatePaymentResponse()
                    {
                        Status = false,
                        Message = "Избраната сметка не съществува!"
                    };
                }

                queryParameters = new QueryParameters();
                queryParameters.AddWhere("user_id", request.User.Id);
                queryParameters.AddWhere("account_id", fromAccount.Id);
                var doesUserHaveAccessToAccount = await _userAccountRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();

                if (doesUserHaveAccessToAccount == null)
                {
                    return new CreatePaymentResponse()
                    {
                        Status = false,
                        Message = "Нямате достъп до избраната сметка!"
                    };
                }

                queryParameters = new QueryParameters();
                queryParameters.AddWhere("account_number", request.Data.ToAccountNumber);
                var toAccount = await _accountRepository.RetrieveAll(queryParameters).SingleOrDefaultAsync();

                if (toAccount == null)
                {
                    return new CreatePaymentResponse()
                    {
                        Status = false,
                        Message = "Сметката, към която се изпраща плащането, не съществува!"
                    };
                }

                var payment = new Models.Payment()
                {
                    UserId = request.User.Id,
                    FromAccountId = fromAccount.Id,
                    ToAccountId = toAccount.Id,
                    Amount = request.Data.Amount,
                    Description = request.Data.Description,
                    Status = Models.PaymentStatus.PENDING,
                    CreatedAt = DateTime.Now
                };

                var id = await _paymentRepository.Create(payment);

                payment.Id = id;

                var mappedPayment = await MapPaymentToPaymentResponseDto(payment);

                return new CreatePaymentResponse()
                {
                    Status = true,
                    Message = "Плащането е успешно създадено!",
                    Data = mappedPayment
                };
            }

            catch (Exception ex)
            {
                return new CreatePaymentResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<SendPaymentByIdResponse> SendPaymentById(SendPaymentByIdRequest request)
        {
            if (request == null)
            {
                return new SendPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.User == null)
            {
                return new SendPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Само логнати потребители могат да изпращат плащания!"
                };
            }

            if (request.Data == null)
            {
                return new SendPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Данните за плащането са невалидни!"
                };
            }

            if (request.Data.Id <= 0)
            {
                return new SendPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Идентификаторът на плащането е невалиден!"
                };
            }

            try
            {
                var payment = await _paymentRepository.Retrieve(request.Data.Id);

                if (payment == null)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Плащането не съществува!"
                    };
                }

                if (payment.UserId != request.User.Id)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Нямате достъп до това плащание!"
                    };
                }

                if (payment.Status != Models.PaymentStatus.PENDING)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Плащането не е в състояние за изпращане!"
                    };
                }

                var fromAccount = await _accountRepository.Retrieve(payment.FromAccountId);
                var toAccount = await _accountRepository.Retrieve(payment.ToAccountId);

                if (fromAccount == null)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Сметката, от която се изпраща плащането, не съществува!"
                    };
                }

                if (toAccount == null)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Сметката, към която се изпраща плащането, не съществува!"
                    };
                }

                if (fromAccount.Balance < payment.Amount)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Недостатъчен баланс на сметката!"
                    };
                }

                var fromAccountUpdate = new AccountUpdate()
                {
                    Balance = fromAccount.Balance - payment.Amount
                };

                var toAccountUpdate = new AccountUpdate()
                {
                    Balance = toAccount.Balance + payment.Amount
                };

                var fromAccountSuccess = await _accountRepository.Update(fromAccount.Id, fromAccountUpdate);
                if (!fromAccountSuccess)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Неуспешно обновяване на баланса на отправната сметка!"
                    };
                }

                var toAccountSuccess = await _accountRepository.Update(toAccount.Id, toAccountUpdate);
                if (!toAccountSuccess)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Неуспешно обновяване на баланса на получната сметка!"
                    };
                }

                var paymentUpdate = new PaymentUpdate()
                {
                    Status = Models.PaymentStatus.PROCESSED
                };

                var paymentSuccess = await _paymentRepository.Update(payment.Id, paymentUpdate);
                if (!paymentSuccess)
                {
                    return new SendPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Неуспешно обновяване на статуса на плащането!"
                    };
                }

                return new SendPaymentByIdResponse()
                {
                    Status = true,
                    Message = "Плащането е успешно изпратено!",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new SendPaymentByIdResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<CancelPaymentByIdResponse> CancelPaymentById(CancelPaymentByIdRequest request)
        {
            if (request == null)
            {
                return new CancelPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Заявката е невалидна!"
                };
            }

            if (request.User == null)
            {
                return new CancelPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Само логнати потребители могат да отменят плащания!"
                };
            }

            if (request.Data == null)
            {
                return new CancelPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Данните за плащането са невалидни!"
                };
            }

            if (request.Data.Id <= 0)
            {
                return new CancelPaymentByIdResponse()
                {
                    Status = false,
                    Message = "Идентификаторът на плащането е невалиден!"
                };
            }

            try
            {
                var payment = await _paymentRepository.Retrieve(request.Data.Id);

                if (payment == null)
                {
                    return new CancelPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Плащането не съществува!"
                    };
                }

                if (payment.UserId != request.User.Id)
                {
                    return new CancelPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Нямате достъп до това плащание!"
                    };
                }

                if (payment.Status != Models.PaymentStatus.PENDING)
                {
                    return new CancelPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Плащането не е в състояние за отмяна!"
                    };
                }

                var paymentUpdate = new PaymentUpdate()
                {
                    Status = Models.PaymentStatus.CANCELLED
                };

                var paymentSuccess = await _paymentRepository.Update(payment.Id, paymentUpdate);

                if (!paymentSuccess)
                {
                    return new CancelPaymentByIdResponse()
                    {
                        Status = false,
                        Message = "Неуспешно обновяване на статуса на плащането!"
                    };
                }

                return new CancelPaymentByIdResponse()
                {
                    Status = true,
                    Message = "Плащането е отменено успешно!",
                    Data = true
                };
            }
            catch (Exception ex)
            {
                return new CancelPaymentByIdResponse()
                {
                    Status = false,
                    Message = ex.Message
                };
            }
        }

        private async Task<PaymentResponseDto> MapPaymentToPaymentResponseDto(Models.Payment payment)
        {
            var user = await _userRepository.Retrieve(payment.UserId);
            var fromAccount = await _accountRepository.Retrieve(payment.FromAccountId);
            var toAccount = await _accountRepository.Retrieve(payment.ToAccountId);

            if (user == null || fromAccount == null || toAccount == null)
            {
                throw new Exception("Неуспешно получаване на информация за плащането!");
            }

            return new PaymentResponseDto()
            {
                Id = payment.Id,
                UserId = payment.UserId,
                UserName = user.FullName,
                FromAccountId = payment.FromAccountId,
                FromAccountNumber = fromAccount.AccountNumber,
                ToAccountId = payment.ToAccountId,
                ToAccountNumber = toAccount.AccountNumber,
                Amount = payment.Amount,
                Description = payment.Description,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}