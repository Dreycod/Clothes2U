using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class OrderManager : IOrderRepository
{
    private readonly Clothes2UDbContext _context;

    public OrderManager(Clothes2UDbContext context)
    {
        _context = context;
    }

    public async Task<Commande?> GetByIdAsync(int id)
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
                .ThenInclude(a => a.Photos)
            .Include(c => c.Acheteur)
            .Include(c => c.Vendeur)
            .Include(c => c.AdresseLivraison)
            .Include(c => c.StatutCommande)
            .Include(c => c.Conversation)
            .FirstOrDefaultAsync(c => c.CommandeId == id);
    }

    public async Task<IEnumerable<Commande>> GetAllAsync()
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
            .Include(c => c.Acheteur)
            .Include(c => c.Vendeur)
            .Include(c => c.AdresseLivraison)
            .Include(c => c.StatutCommande)
            .Include(c => c.Conversation)
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();
    }

    public async Task<Commande?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
                .ThenInclude(a => a.Photos)
            .Include(c => c.Acheteur)
            .Include(c => c.Conversation)
            .Include(c => c.StatutCommande)
            .Include(c => c.Vendeur)
            .Include(c => c.AdresseLivraison)
            .FirstOrDefaultAsync(c => c.CommandeId == orderId);
    }

    public async Task<List<Commande>> GetOrdersByUserIdAsync(int userId)
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
                .ThenInclude(a => a.Photos)
            .Include(c => c.Vendeur)
            .Include(c => c.StatutCommande)
            .Include(c => c.AdresseLivraison)
            .Include(c => c.Conversation)
            .Where(c => c.AcheteurId == userId)
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();
    }

    public async Task<List<Commande>> GetOrdersBySellerIdAsync(int sellerId)
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
                .ThenInclude(a => a.Photos)
            .Include(c => c.Acheteur)
            .Include(c => c.StatutCommande)
            .Include(c => c.AdresseLivraison)
            .Include(c => c.Conversation)
            .Where(c => c.VendeurId == sellerId)
            .OrderByDescending(c => c.DateCommande)
            .ToListAsync();
    }

    public async Task<Commande?> GetOrderByPaymentIntentIdAsync(string paymentIntentId)
    {
        return await _context.Commandes
            .Include(c => c.Annonce)
            .Include(c => c.Acheteur)
            .Include(c => c.Conversation)
            .Include(c => c.Vendeur)
            .Include(c => c.AdresseLivraison)
            .Include(c => c.StatutCommande)
            .FirstOrDefaultAsync(c => c.StripePaymentIntentId == paymentIntentId);
    }

    public async Task AddAsync(Commande entity)
    {
        entity.DateCommande = DateTime.UtcNow;
        await _context.Commandes.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Commande entity)
    {
        _context.Commandes.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, int statusCommandeId, string? trackingNumber = null)
    {
        var order = await _context.Commandes.FindAsync(orderId);
        if (order == null) return false;

        order.StatutCommandeId = statusCommandeId;
        
        if (!string.IsNullOrEmpty(trackingNumber))
        {
            order.NumeroSuivi = trackingNumber;
        }

        if (statusCommandeId == 2 && order.DateExpedition == null)
        {
            order.DateExpedition = DateTime.UtcNow;
        }
        else if (statusCommandeId == 3 && order.DateLivraison == null)
        {
            order.DateLivraison = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task DeleteAsync(Commande entity)
    {
        _context.Commandes.Remove(entity);
        await _context.SaveChangesAsync();
    }
}