using API.Models;
using API.Models.EntityFramework;
using API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserDeletionService : IUserDeletionService
    {
        private readonly Clothes2UDbContext _context;

        public UserDeletionService(Clothes2UDbContext context)
        {
            _context = context;
        }

        public async Task DeleteUtilisateurAsync(int utilisateurId)
        {
            var utilisateur = await _context.Utilisateurs
                .Include(u => u.CommandesAchetees)
                .Include(u => u.CommandesVendues)
                .FirstOrDefaultAsync(u => u.UtilisateurId == utilisateurId);

            if (utilisateur == null)
                throw new Exception("Utilisateur introuvable");

            if (utilisateur.DeletedAt != null)
                throw new Exception("Utilisateur déjà supprimé");

            var now = DateTime.UtcNow;

            // Get les annonces actives (non achetés) 
            var annonces = await _context.Annonces
                .Where(a => a.UtilisateurId == utilisateurId && a.StatutAnnonceId == 1) // StatutAnnonceId 1 = Active
                .ToListAsync();

            // 1️ Historisation ACHATS
            foreach (var commande in utilisateur.CommandesAchetees)
            {
                _context.HistoriqueUtilisateurs.Add(new HistoriqueUtilisateur
                {
                    UtilisateurId = utilisateur.UtilisateurId,
                    TypeTransaction = "ACHAT",
                    AnnonceId = commande.AnnonceId,
                    Montant = commande.MontantTotal,
                    DateTransaction = commande.DateCommande,
                    DateSuppressionCompte = now,
                });
            }

            // 2️ Historisation VENTES
            foreach (var commande in utilisateur.CommandesVendues)
            {
                _context.HistoriqueUtilisateurs.Add(new HistoriqueUtilisateur
                {
                    UtilisateurId = utilisateur.UtilisateurId,
                    TypeTransaction = "VENTE",
                    AnnonceId = commande.AnnonceId,
                    Montant = commande.MontantTotal,
                    DateTransaction = commande.DateCommande,
                    DateSuppressionCompte = now,
                });
            }

            foreach (var annonce in annonces)
            {
                annonce.StatutAnnonceId = 2; // "Supprimée" / "Suspendu"

            }

            // 3️ Anonymisation
            utilisateur.Email = $"deleted_{utilisateur.UtilisateurId}@deleted.local";
            utilisateur.Login = $"deleted_{utilisateur.UtilisateurId}";
            utilisateur.Password = "deleted";
            utilisateur.Telephone = "";
            utilisateur.Description = "";
            utilisateur.ValidEmail = false;
            utilisateur.ValidTelephone = false;

            // 4️ Soft delete
            utilisateur.DeletedAt = now;

            // 5️ Statut "Supprimé"
            utilisateur.StatutId = 4; 

            await _context.SaveChangesAsync();
        }
    }
}
