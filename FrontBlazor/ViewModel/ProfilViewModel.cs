using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class ProfilViewModel
    {
        public UtilisateurView? ViewingUser = null;
        public List<Annonce>? Annonces = null;
        public List<NoteUtilisateur>? Avis = null;
        public List<Annonce>? FavorisAnnonce = null;

        public int AvisCount = 0;

        private readonly IReadableService<UtilisateurView> _utilisateurService;
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;
        private readonly INoteUtilisateurService<NoteUtilisateur> _noteUtilisateurService;
        private readonly IAuthService _authService;
        private readonly IWritableService<Abonnement> _abonnementService;

        public string activeTab = "articles";

        public ProfilViewModel(IReadableService<UtilisateurView> utilisateurService, IAnnonceService<Annonce> annonceService, 
            IFavorisService<Favoris> favorisService, INoteUtilisateurService<NoteUtilisateur> noteUtilisateurService, 
            IAuthService authService, IWritableService<Abonnement> abonnementService)
        {
            _utilisateurService = utilisateurService;
            _annonceService = annonceService;
            _favorisService = favorisService;
            _noteUtilisateurService = noteUtilisateurService;
            _authService = authService;
            _abonnementService = abonnementService;
        }

        public async Task LoadUserProfile(int id)
        {
            ViewingUser = await _utilisateurService.GetByIdAsync(id) ?? null;
            Annonces = await _annonceService.GetAnnoncesByUserIdAsync(id) ?? null;
            Avis = await _noteUtilisateurService.GetAllNotesByUtilisateurId(id) ?? null;
            AvisCount = Avis.Count;
            Utilisateur? utilisateur = await _authService.GetCurrentUserAsync();
            if (utilisateur != null)
            {
                FavorisAnnonce = await _annonceService.GetByFavorisUtilisateur();
            }
        }

        public void SetActiveTab(string tab)
        {
            activeTab = tab;
        }

        public async Task ToggleFavorite(int annonceId)
        {
            Annonce annonce = Annonces.First(a => a.AnnonceId == annonceId);

            if (!annonce.IsLikedByCurrentUser)
                await _favorisService.AddFavoris(annonceId);
            else
                await _favorisService.DeleteFavoris(annonceId);

            annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;
        }

        public async Task ToggleAbonnement(int userId)
        {
            await _abonnementService.AddAsync(new Abonnement{AbonnementId = userId});

        }
    }
}
