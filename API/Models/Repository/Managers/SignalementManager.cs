using Shared.DTO.Signalement;
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
                .ThenInclude(av => av.Auteur)
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
            await _context.Signalements.AddAsync(signalement);
            await _context.SaveChangesAsync();
            if (annonceId.HasValue)
            {
                var signalementAnnonce = new SignalementAnnonce
                {
                    SignalementId = signalement.SignalementId,
                    AnnonceSignaleeId = annonceId.Value
                };
                await _context.SignalementAnnonces.AddAsync(signalementAnnonce);
            }
            else if (avisId.HasValue)
            {
                var signalementAvis = new SignalementAvis
                {
                    SignalementId = signalement.SignalementId,
                    AvisId = avisId.Value
                };
                await _context.SignalementAvises.AddAsync(signalementAvis);
            }
            else if (utilisateurSignaleId.HasValue)
            {
                var signalementUtilisateur = new SignalementUtilisateur
                {
                    SignalementId = signalement.SignalementId,
                    UtilisateurSignaleId = utilisateurSignaleId.Value
                };
                await _context.SignalementUtilisateurs.AddAsync(signalementUtilisateur);
            }
            await _context.SaveChangesAsync();
            return await GetByIdAsync(signalement.SignalementId) 
                   ?? throw new InvalidOperationException("Le signalement créé n'a pas pu être récupéré");
        }
        

    }
}
