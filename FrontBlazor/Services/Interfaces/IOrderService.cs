using Shared.DTO;

namespace FrontBlazor.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDTO> CreateOrderAsync(CreateOrderDTO order);
    Task<List<OrderDTO>> GetUserOrdersAsync(int userId);
    Task<List<OrderDTO>> GetSellerOrdersAsync(int sellerId);
    Task<OrderDTO?> GetOrderByIdAsync(int orderId);
    Task<OrderDTO?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
    Task<bool> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDTO statusUpdate);
    Task<OrderStatsDTO> GetUserOrderStatsAsync(int userId);
}