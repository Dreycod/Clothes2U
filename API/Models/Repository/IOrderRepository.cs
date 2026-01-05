using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IOrderRepository : IDataRepository<Commande, int>
{
    Task<List<Commande>> GetOrdersByUserIdAsync(int userId);
    Task<List<Commande>> GetOrdersBySellerIdAsync(int sellerId);
    Task<Commande?> GetOrderWithDetailsAsync(int orderId);
    Task<Commande?> GetOrderByPaymentIntentIdAsync(string paymentIntentId);
    Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? trackingNumber = null);
}