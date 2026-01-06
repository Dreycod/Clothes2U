using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;

namespace API.Models.Repository.Managers;

public class ClusterManager : GenericCRUDManager<AnnoncePreferenceUtilisateur>, IClusterRepository
{
    public ClusterManager(Clothes2UDbContext context) : base(context) { }
    
    public async Task SaveClustersAsync(ClusteringResponseDTO clusteringResponse)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            var existingPrefs = await _context.AnnoncesPreferenceUtilisateurs
                .Where(a => a.UtilisateurId == clusteringResponse.UserId)
                .ToListAsync();
            
            _context.AnnoncesPreferenceUtilisateurs.RemoveRange(existingPrefs);
            await _context.SaveChangesAsync();
            foreach (var clusterResult in clusteringResponse.ClustersParCategorie)
            {
                foreach (var centroide in clusterResult.Centroides)
                {
                    var preference = new AnnoncePreferenceUtilisateur
                    {
                        UtilisateurId = clusteringResponse.UserId,
                        NomMarque = centroide.NomMarque,
                        Categorie = clusterResult.Categorie,
                        SousCategorie = centroide.SousCategorie,
                        Prix = centroide.PrixMoyen,
                        Taille = centroide.Taille,
                        EtatArticle = centroide.EtatArticle,
                        Ponderation = centroide.NbAnnonces,
                        CouleurDominante = string.IsNullOrWhiteSpace(centroide.CouleurDominante) 
                        ? "Non spécifié" 
                        : centroide.CouleurDominante,
                    };
                    
                    _context.AnnoncesPreferenceUtilisateurs.Add(preference);
                    await _context.SaveChangesAsync();
                }
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}