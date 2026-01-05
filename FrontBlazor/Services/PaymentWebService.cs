using System.Net.Http.Json;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel;
using Shared;
using Shared.DTO;

namespace FrontBlazor.Services;

public class PaymentWebService : IPaymentService
{
    private readonly HttpClient _httpClient;

    public PaymentWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PaymentIntentResponseDTO> CreatePaymentIntentAsync(CreatePaymentIntentDTO request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/payment/create-intent", request);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<PaymentIntentResponseDTO>();
        return result ?? throw new Exception("Failed to create payment intent");
    }

    public async Task<bool> ConfirmPaymentAsync(string paymentIntentId)
    {
        var response = await _httpClient.PostAsync($"api/payment/confirm/{paymentIntentId}", null);
        return response.IsSuccessStatusCode;
    }

    public async Task<PaymentStatusDTO> GetPaymentStatusAsync(string paymentIntentId)
    {
        var response = await _httpClient.GetAsync($"api/payment/status/{paymentIntentId}");
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<PaymentStatusDTO>();
        return result ?? throw new Exception("Failed to get payment status");
    }
}