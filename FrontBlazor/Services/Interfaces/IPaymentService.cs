using FrontBlazor.ViewModel;
using Shared;
using Shared.DTO;
using PaymentIntentResponseDTO = Shared.DTO.PaymentIntentResponseDTO;

namespace FrontBlazor.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResponseDTO> CreatePaymentIntentAsync(CreatePaymentIntentDTO request);
    Task<bool> ConfirmPaymentAsync(string paymentIntentId);
    Task<PaymentStatusDTO> GetPaymentStatusAsync(string paymentIntentId);
}