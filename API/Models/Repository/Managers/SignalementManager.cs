using API.DTO.Signalement;
using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class SignalementManager : GenericCRUDManager<Signalement>, ISignalementRepository
    {
        public SignalementManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<Signalement> BaseQuery()
        {
            return _context.Signalements
                .Include(s => s.TypeSignalement)
                .Include(s => s.Utilisateur)
                .Include(s => s.SignalementsAnnonce)
                .ThenInclude(sa => sa.Annonce)
                .ThenInclude(a => a.Utilisateur)  
                .Include(s => s.SignalementsAvis)  
                .ThenInclude(sa => sa.Avis)
                .ThenInclude(av => av.Cible)  
                .Include(s => s.SignalementsUtilisateur)  
                .ThenInclude(su => su.UtilisateurSignale)
                .AsSplitQuery();
        }

        public async override Task<IEnumerable<Signalement>> GetAllAsync()
        {
            return await BaseQuery().ToListAsync();
        }

        public override Task<Signalement?> GetByIdAsync(int id)
        {
            return BaseQuery().FirstOrDefaultAsync(s => s.SignalementId == id);
        }

        public async Task<IEnumerable<Signalement>> GetByUtilisateurAsync(int utilisateurId)
        {
            return await BaseQuery().Where(s => s.UtilisateurId == utilisateurId).ToListAsync();
        }

        public async Task<IEnumerable<Signalement>> GetByTypeAsync(int typeId)
        {
            return await BaseQuery().Where(s => s.SignalementTypeId == typeId).ToListAsync();
        }
        public async Task<Signalement> CreateWithRelationsAsync(
    Signalement signalement,
    int? annonceId,
    int? avisId,
    int? utilisateurSignaleId)
        {
            // Ajoute le signalement principal
            _context.Signalements.Add(signalement);
            await _context.SaveChangesAsync();

            // Ajout sous-entités selon type
            if (annonceId.HasValue)
            {
                _context.SignalementAnnonces.Add(new SignalementAnnonce
                {
                    SignalementId = signalement.SignalementId,
                    AnnonceSignaleeId = annonceId.Value
                });
            }

            if (avisId.HasValue)
            {
                _context.SignalementAvises.Add(new SignalementAvis
                {
                    SignalementId = signalement.SignalementId,
                    AvisId = avisId.Value
                });
            }

            if (utilisateurSignaleId.HasValue)
            {
                _context.SignalementUtilisateurs.Add(new SignalementUtilisateur
                {
                    SignalementId = signalement.SignalementId,
                    UtilisateurSignaleId = utilisateurSignaleId.Value
                });
            }

            await _context.SaveChangesAsync();
            return signalement;
        }

    }
}
