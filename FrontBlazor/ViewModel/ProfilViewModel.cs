using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;

namespace FrontBlazor.ViewModel
{
    public class ProfilViewModel
    {
        #region Variables
        public UtilisateurView? ViewingUser { get; set; } = null;
        public List<Annonce>? Annonces { get; set; } = null;
        public List<NoteUtilisateur>? Avis { get; set; } = null;
        public List<Annonce>? FavorisAnnonce { get; set; } = null;

        public int AvisCount { get; set; } = 0;
        public bool IsSameUser { get; set; } = false;

        public bool showDotsDropdown;

        private readonly IReadableService<UtilisateurView> _utilisateurService;
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;
        private readonly INoteUtilisateurService<NoteUtilisateur> _noteUtilisateurService;
        private readonly IAuthService _authService;
        private readonly IAbonnementService<Abonnement> _abonnementService;
        private readonly NavigationManager _navigationManager;
        private readonly IMediasService<Photo> _mediaService;
        private readonly ISignalementService _signalementService;
        private readonly IBloqueService _bloqueService;

        public string ActiveTab { get; set; } = "articles";

        public bool IsLoading { get; set; } = true;
        public bool IsLoadingArticles { get; set; } = false;
        public bool IsLoadingFavoris { get; set; } = false;
        public bool IsLoadingAvis { get; set; } = false;
        public bool UserNotFound { get; set; } = false;
        public bool UserSuspended { get; set; } = false;
        public bool IsBlockedByUser { get; set; } = false;

        public bool ShowAddReviewModal { get; set; } = false;
        public int SelectedRating { get; set; } = 0;
        public string ReviewComment { get; set; } = string.Empty;
        public string ReviewErrorMessage { get; set; } = string.Empty;
        public bool IsSubmittingReview { get; set; } = false;
        public bool IsSubmittingBloque { get; set; } = false;

        public bool ShowBloqueModal { get; set; } = false;

        public bool IsFollowing = true;
        public string FollowButtonText => IsFollowing ? "Se désabonner" : "Suivre";

        public event Action? OnStateChanged;
        #endregion

        public ProfilViewModel(
            IReadableService<UtilisateurView> utilisateurService, IAnnonceService<Annonce> annonceService,
            IFavorisService<Favoris> favorisService,INoteUtilisateurService<NoteUtilisateur> noteUtilisateurService,
            IAuthService authService, IAbonnementService<Abonnement> abonnementService, NavigationManager navigationManager,
            LoginViewModel connexionViewModel, IMediasService<Photo> mediasService,
            ISignalementService signalementService, IBloqueService bloqueService)
        {
            _utilisateurService = utilisateurService;
            _annonceService = annonceService;
            _favorisService = favorisService;
            _noteUtilisateurService = noteUtilisateurService;
            _authService = authService;
            _abonnementService = abonnementService;
            _navigationManager = navigationManager;
            _mediaService = mediasService;
            _signalementService = signalementService;
            _bloqueService = bloqueService;
        }

        private void NotifyStateChanged() => OnStateChanged?.Invoke();

        public async Task LoadUserProfile(int id)
        {
            IsLoading = true;
            UserNotFound = false;
            UserSuspended = false;

            try
            {
                UtilisateurView user = await _utilisateurService.GetByIdAsync(id);

                if (user == null)
                {
                    UserNotFound = true;
                    IsLoading = false;
                    NotifyStateChanged();
                    return;
                }

                if (user.Statut == "Suspendu")
                {
                    UserSuspended = true;
                    IsLoading = false;
                    NotifyStateChanged();
                    return;
                }

                IsLoadingArticles = true;
                IsLoadingAvis = true;

                IsBlockedByUser = user.blockedByCurrentUser;
                IsFollowing = user.followeddByCurrentUser;

                ViewingUser = user;

                var tasks = new List<Task>
                 {
                     Task.Run(async () => {
                         Annonces = await _annonceService.GetAnnoncesByUserIdAsync(id);
                         IsLoadingArticles = false;
                         NotifyStateChanged();
                     }),
                     Task.Run(async () => {
                         Avis = await _noteUtilisateurService.GetAllNotesByUtilisateurId(id);
                         AvisCount = Avis?.Count ?? 0;
                         IsLoadingAvis = false;
                         NotifyStateChanged();
                     })
                 };

                Utilisateur? utilisateur = await _authService.GetCurrentUserAsync();
                if (utilisateur != null && ViewingUser != null && utilisateur.UtilisateurId == ViewingUser.UtilisateurId)
                {
                    IsSameUser = true;
                    IsLoadingFavoris = true;
                    FavorisAnnonce = await _annonceService.GetByFavorisUtilisateur();
                    IsLoadingFavoris = false;
                }
                else
                {
                    IsSameUser = false;
                }

                await Task.WhenAll(tasks);
            }
            catch
            {
                UserNotFound = true;
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        public void SetActiveTab(string tab)
        {
            ActiveTab = tab;
            NotifyStateChanged();
        }

        public async Task ToggleFavorite(Annonce annonce)
        {
            if (CheckLoginStatus == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            bool isFavorite = annonce.IsLikedByCurrentUser;
            annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;

            try
            {
                if (!isFavorite)
                {
                    await _favorisService.AddFavoris(annonce.AnnonceId);
                }
                else
                {
                    FavorisAnnonce?.Remove(annonce);
                    await _favorisService.DeleteFavoris(annonce.AnnonceId);
                }
                NotifyStateChanged();
            }
            catch
            {
                annonce.IsLikedByCurrentUser = isFavorite;
                NotifyStateChanged();
            }
        }

        public async Task ToggleAbonnement()
        {
            if (_authService.GetCurrentUserAsync() == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            if (ViewingUser == null) 
                return;

            bool wasFollowing = ViewingUser.followeddByCurrentUser;
            ViewingUser.followeddByCurrentUser = !ViewingUser.followeddByCurrentUser;

            if (!wasFollowing)
            {
                ViewingUser.Abonnes += 1;
            }
            else
            {
                ViewingUser.Abonnes -= 1;
            }

            NotifyStateChanged();

            try
            {
                if (!wasFollowing)
                {
                    await _abonnementService.AddAbonnement(ViewingUser.UtilisateurId);
                }
                else
                {
                    await _abonnementService.DeleteAbonnement(ViewingUser.UtilisateurId);
                }
            }
            catch
            {
                ViewingUser.followeddByCurrentUser = wasFollowing;
                if (!wasFollowing)
                {
                    ViewingUser.Abonnes -= 1;
                }
                else
                {
                    ViewingUser.Abonnes += 1;
                }
                NotifyStateChanged();
            }
        }

        public void ShowAddReview()
        {
            ShowAddReviewModal = true;
            SelectedRating = 0;
            ReviewComment = string.Empty;
            ReviewErrorMessage = string.Empty;
            NotifyStateChanged();
        }

        public void CloseAddReview()
        {
            ShowAddReviewModal = false;
            SelectedRating = 0;
            ReviewComment = string.Empty;
            ReviewErrorMessage = string.Empty;
            NotifyStateChanged();
        }

        public void SetRating(int rating)
        {
            SelectedRating = rating;
            ReviewErrorMessage = string.Empty;
            NotifyStateChanged();
        }

        public async Task SubmitReview()
        {
            // check if message transaction exists
            if (ViewingUser == null) return;

            ReviewErrorMessage = string.Empty;

            if (SelectedRating == 0)
            {
                ReviewErrorMessage = "Veuillez s�lectionner une note";
                NotifyStateChanged();
                return;
            }

            if (string.IsNullOrWhiteSpace(ReviewComment))
            {
                ReviewErrorMessage = "Veuillez entrer un commentaire";
                NotifyStateChanged();
                return;
            }

            if (ReviewComment.Length < 10)
            {
                ReviewErrorMessage = "Le commentaire doit contenir au moins 10 caract�res";
                NotifyStateChanged();
                return;
            }

            IsSubmittingReview = true;
            NotifyStateChanged();

            try
            {
                NoteUtilisateurCreate newReview = new NoteUtilisateurCreate
                {
                    CibleId = ViewingUser.UtilisateurId,
                    Note = SelectedRating,
                    Commentaire = ReviewComment
                };

                var result = await _noteUtilisateurService.AddNoteUtilisateur(newReview);
                if (result != null)
                {
                    Avis = await _noteUtilisateurService.GetAllNotesByUtilisateurId(ViewingUser.UtilisateurId);
                    AvisCount = Avis?.Count ?? 0;
                }
                CloseAddReview();
            }
            catch (Exception ex)
            {
                ReviewErrorMessage = $"Erreur lors de la publication de l'avis: {ex.Message}";
            }
            finally
            {
                IsSubmittingReview = false;
                NotifyStateChanged();
            }
        }
        public async Task<bool> CheckLoginStatus()
        {
            if (await _authService.GetCurrentUserAsync() != null)
                return true;
            return false;
        }
        public void NavigateToProductDetail(int? productId)
        {
            if (productId.HasValue)
            {
                _navigationManager.NavigateTo($"/product/{productId}", true);
            }
        }

        public void NavigateToAddArticle()
        {
            _navigationManager.NavigateTo("/add-article", true);
        }

        public void NavigateToHome()
        {
            _navigationManager.NavigateTo("/", true);
        }

        public void ToggleDotsDropdown()
        {
            showDotsDropdown = !showDotsDropdown;
        }

        public void ToggleBloqueModal()
        {
            showDotsDropdown = false;
            ShowBloqueModal = !ShowBloqueModal;
        }
        public async void ToggleBloque()
        {
            IsSubmittingBloque = true;
            if (!IsBlockedByUser)
            {
                await _bloqueService.CreateBloque(ViewingUser!.UtilisateurId);
            }
            else
            {
                await _bloqueService.DeleteAsync(ViewingUser!.UtilisateurId);
            }

            IsSubmittingBloque = false;
            ShowBloqueModal = false;
            _navigationManager.Refresh(true);
        }

        public void SignalerUtilisateur()
        {
            //showDotsDropdown = false;
            //Signalement signalement = new Signalement
            //{
            //     SignalementDate = DateTime.Now,
            //     SignalementMotif = "Inappropri",
            //    public string Type { get; set; } = null!;
            //    public string LoginUtilisateurSignale { get; set; } = null!;
            //    public int? PhotoProfilUtilisateurId { get; set; }
            //  };
            //_signalementService.AddAsync(signalement);
            
        }

        public string GetPhoto(int id)
        {
            return _mediaService.GetPhotoUrl(id);
        }
    }
}
