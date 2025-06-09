using FinalProject.Services.DTOs.Requests.Payment.CancelPaymentById;
using FinalProject.Services.DTOs.Requests.Payment.CreatePayment;
using FinalProject.Services.DTOs.Requests.Payment.GetPaymentsForAuthUser;
using FinalProject.Services.DTOs.Requests.Payment.SendPaymentById;
using FinalProject.Services.DTOs.Responses.Payment.CancelPaymentById;
using FinalProject.Services.DTOs.Responses.Payment.CreatePayment;
using FinalProject.Services.DTOs.Responses.Payment.GetPaymentsForAuthUser;
using FinalProject.Services.DTOs.Responses.Payment.SendPaymentById;

namespace FinalProject.Services.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<GetPaymentsForAuthUserResponse> GetPaymentsForAuthUser(GetPaymentsForAuthUserRequest request);
        Task<CreatePaymentResponse> CreatePayment(CreatePaymentRequest request);
        Task<SendPaymentByIdResponse> SendPaymentById(SendPaymentByIdRequest request);
        Task<CancelPaymentByIdResponse> CancelPaymentById(CancelPaymentByIdRequest request);
    }
}