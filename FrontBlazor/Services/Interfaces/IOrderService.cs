using Shared.DTO;

namespace FrontBlazor.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDTO> CreateOrderAsync(CreateOrderDTO order);
    Task<List<OrderDTO>> GetUserOrdersAsync();
    Task<List<OrderDTO>> GetSellerOrdersAsync();
    Task<OrderDTO?> GetOrderByIdAsync(int orderId);
    Task<OrderDTO?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
    Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO statusUpdate);
    Task<OrderStatsDTO> GetUserOrderStatsAsync(int userId);
}