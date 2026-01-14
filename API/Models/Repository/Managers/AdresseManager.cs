using API.Models.EntityFramework;
using API.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class AdresseManager : GenericCRUDManager<Adresse>, IAdresseRepository
{
    public AdresseManager(Clothes2UDbContext context) : base(context) { }

    private IQueryable<Adresse> BaseAdresseQuery()
    {
        return _context.Adresses
            .Include(a => a.Utilisateurs)
            .Include(a => a.Commandes)
            .AsSplitQuery();
    }
    
    public async Task<List<Adresse>> GetUserAddressesAsync(int userId)
    {
        return await BaseAdresseQuery()
            .Where(a => a.UtilisateurId == userId)
            .ToListAsync();
    }

    public async Task<Adresse?> GetByIdAsync(int id)
    {
        return await BaseAdresseQuery()
            .FirstOrDefaultAsync(a => a.AdresseId == id);
    }

    public async Task CreateAsync(Adresse adresse)
    {
        // ✅ FIX: Await the query properly
        var hasExistingAddresses = await _context.Adresses
            .AnyAsync(a => a.UtilisateurId == adresse.UtilisateurId);

        if (!hasExistingAddresses)
        {
            adresse.IsDefault = true;
        }

        await _context.Adresses.AddAsync(adresse);
        await _context.SaveChangesAsync();
    }

    // ✅ FIX: Remove the 'id' parameter - the adresse object already has its ID
    public async Task UpdateAsync(Adresse adresse)
    {
        var existing = await _context.Adresses
            .FirstOrDefaultAsync(a => a.AdresseId == adresse.AdresseId);

        if (existing == null)
        {
            throw new KeyNotFoundException($"Address with ID {adresse.AdresseId} not found");
        }

        // Update properties
        existing.AdresseRue = adresse.AdresseRue;
        existing.AdresseVille = adresse.AdresseVille;
        existing.AdresseCodePostal = adresse.AdresseCodePostal;
        existing.AdressePays = adresse.AdressePays;
        existing.IsDefault = adresse.IsDefault;

        await _context.SaveChangesAsync();
    }

    public async Task SetDefaultAsync(int id)
    {
        var address = await _context.Adresses
            .FirstOrDefaultAsync(a => a.AdresseId == id);

        if (address == null)
        {
            throw new KeyNotFoundException($"Address with ID {id} not found");
        }

        // Reset all other addresses for this user
        var userAddresses = await _context.Adresses
            .Where(a => a.UtilisateurId == address.UtilisateurId)
            .ToListAsync();

        foreach (var addr in userAddresses)
        {
            addr.IsDefault = false;
        }

        address.IsDefault = true;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var address = await _context.Adresses
            .FirstOrDefaultAsync(a => a.AdresseId == id);

        if (address == null)
        {
            throw new KeyNotFoundException($"Address with ID {id} not found");
        }

        // ✅ CHECK: Verify if address is used in any orders
        var isUsedInOrders = await _context.Commandes
            .AnyAsync(c => c.AdresseLivraisonId == id);

        if (isUsedInOrders)
        {
            // ❌ Cannot delete - address is referenced by orders
            throw new InvalidOperationException(
                "Cannot delete this address because it is associated with existing orders. " +
                "You can only delete addresses that are not used in any orders.");
        }

        // If deleting default address, set another as default
        if (address.IsDefault)
        {
            var anotherAddress = await _context.Adresses
                .Where(a => a.UtilisateurId == address.UtilisateurId && a.AdresseId != id)
                .FirstOrDefaultAsync();

            if (anotherAddress != null)
            {
                anotherAddress.IsDefault = true;
            }
        }

        _context.Adresses.Remove(address);
        await _context.SaveChangesAsync();
    }
}