using System.Net.Http.Json;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO;
using Stripe.Climate;

namespace FrontBlazor.Services;

public class OrderWebService :BaseGenericService, IOrderService
{
    private readonly HttpClient _httpClient;

    public OrderWebService(HttpClient httpClient) : base(httpClient){}

    public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO order)
    {
        try
        {
            //_logger.LogInformation("Creating order for annonce {AnnonceId}", order.AnnonceId);
            
            var response = await PostWithCredentialsAsync("order", JsonContent.Create(order));
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<OrderDTO>();
            
            //_logger.LogInformation("✅ Order created: {OrderId}", result?.CommandeId);
            
            return result ?? throw new Exception("Failed to create order");
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "❌ Error creating order");
            throw;
        }
    }

    public async Task<List<OrderDTO>> GetUserOrdersAsync()
    {
        try
        {
            var response = await GetWithCredentialsAsync($"order/user");
            response.EnsureSuccessStatusCode();
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDTO>>();
            return orders ?? new List<OrderDTO>();
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "❌ Error getting user orders for user {UserId}", userId);
            Console.WriteLine($"Ne peut pas GetOrdersAsync {ex.Message}");
            return null;
        }
    }

    public async Task<List<OrderDTO>> GetSellerOrdersAsync()
    {
        try
        {
            var response = await GetWithCredentialsAsync($"order/seller");
            response.EnsureSuccessStatusCode();
            
            var orders = await response.Content.ReadFromJsonAsync<List<OrderDTO>>();
            return orders ?? new List<OrderDTO>();
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "❌ Error getting seller orders for seller {SellerId}", sellerId);
            Console.WriteLine($"Ne peut pas GetOrdersAsync {ex.Message}");
            return new List<OrderDTO>();
        }
    }

    public async Task<OrderDTO?> GetOrderByIdAsync(int orderId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"order/{orderId}");
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            
            return await response.Content.ReadFromJsonAsync<OrderDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetOrderByIdAsync {ex.Message}");
            return null;
        }
    }

    public async Task<OrderDTO?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"order/payment-intent/{paymentIntentId}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<OrderDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"peymentintent faile {ex.Message}");
            return null;
        }
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO statusUpdate)
    {
        try
        {
            var response = await PutWithCredentialsAsync($"order/{orderId}/status", JsonContent.Create(statusUpdate));
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "❌ Error updating order status for {OrderId}", orderId);
            Console.WriteLine($"UpdateOrderStatusAsync {ex.Message}");
            return false;
        }
    }

    public async Task<OrderStatsDTO> GetUserOrderStatsAsync(int userId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"order/user/{userId}/stats");
            response.EnsureSuccessStatusCode();
            
            var stats = await response.Content.ReadFromJsonAsync<OrderStatsDTO>();
            return stats ?? new OrderStatsDTO();
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex, "❌ Error getting order stats for user {UserId}", userId);
            Console.WriteLine($"Ne peut pas GetUserOrderStatsAsync {ex.Message}");
            return new OrderStatsDTO();
        }
    }
}