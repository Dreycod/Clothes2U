using System.Net.Http.Json;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Stripe.Climate;

namespace FrontBlazor.Services;

public class OrderWebService : IOrderService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderService> _logger;

    public OrderWebService(HttpClient httpClient, ILogger<OrderService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO order)
    {
        try
        {
            _logger.LogInformation("Creating order for annonce {AnnonceId}", order.AnnonceId);
            
            var response = await _httpClient.PostAsJsonAsync("api/order", order);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OrderDTO>();
            
            _logger.LogInformation("✅ Order created: {OrderId}", result?.CommandeId);
            
            return result ?? throw new Exception("Failed to create order");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error creating order");
            throw;
        }
    }

    public async Task<List<OrderDTO>> GetUserOrdersAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/order/user/{userId}");
            response.EnsureSuccessStatusCode();
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDTO>>();
            return orders ?? new List<OrderDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting user orders for user {UserId}", userId);
            return new List<OrderDTO>();
        }
    }

    public async Task<List<OrderDTO>> GetSellerOrdersAsync(int sellerId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/order/seller/{sellerId}");
            response.EnsureSuccessStatusCode();
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDTO>>();
            return orders ?? new List<OrderDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting seller orders for seller {SellerId}", sellerId);
            return new List<OrderDTO>();
        }
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/order/{orderId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            return await response.Content.ReadFromJsonAsync<OrderDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting order {OrderId}", orderId);
            return null;
        }
    }

    public async Task<OrderDTO?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/order/payment-intent/{paymentIntentId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            return await response.Content.ReadFromJsonAsync<OrderDTO>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting order by payment intent {PaymentIntentId}", paymentIntentId);
            return null;
        }
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO statusUpdate)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"api/order/{orderId}/status", statusUpdate);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating order status for {OrderId}", orderId);
            return false;
        }
    }

    public async Task<OrderStatsDTO> GetUserOrderStatsAsync(int userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"api/order/user/{userId}/stats");
            response.EnsureSuccessStatusCode();
            
            var stats = await response.Content.ReadFromJsonAsync<OrderStatsDTO>();
            return stats ?? new OrderStatsDTO();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting order stats for user {UserId}", userId);
            return new OrderStatsDTO();
        }
    }
}