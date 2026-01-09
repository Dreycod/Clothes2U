using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.Xml.Linq;
using FrontBlazor.ViewModel.Generic;
using Shared.DTO.Favoris;
using Shared.DTO.Abonnement;
using Shared.DTO.Photo;
using Shared.DTO.Annonce;
using Shared.DTO.Utilisateur;
using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Signalement;

namespace FrontBlazor.ViewModel
{
    public class ProfilViewModel : ClientBaseViewModel
    {
        #region Variables
        public UtilisateurViewDTO? ViewingUser { get; set; } = null;
        public List<AnnonceDTO>? Annonces { get; set; } = null;
        public List<NoteUtilisateurDetailDTO>? Avis { get; set; } = null;
        public List<AnnonceDTO>? FavorisAnnonce { get; set; } = null;

        public int AvisCount { get; set; } = 0;
        public bool IsSameUser { get; set; } = false;
        private int? currentUserId = null;

        public bool showDotsDropdown;

        private readonly IUtilisateurService _utilisateurService;
        private readonly IAnnonceService _annonceService;
        private readonly IFavorisService<FavorisDTO> _favorisService;
        private readonly INoteUtilisateurService _noteUtilisateurService;
        private readonly IAuthService _authService;
        private readonly IAbonnementService<AbonnementDTO> _abonnementService;
        private readonly NavigationManager _navigationManager;
        private readonly IMediasService _mediaService;
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

        public bool ShowSignalerModal { get; set; } = false;

        public bool ShowSignalerAvisModal { get; set; } = false;

        public string SignalementRaison { get; set; } = string.Empty;

        public bool IsSubmittingReport { get; set; } = false;

        public int SignalementIdAvis { get; set; } = 0;

        public bool IsFollowing = true;
        public string FollowButtonText => IsFollowing ? "Se désabonner" : "Suivre";

        public event Action? OnStateChanged;
        public bool IsUpdatingNotifMail { get; set; } = false;
        public string? NotifMailErrorMessage { get; set; }
        #endregion

        public ProfilViewModel(
            IUtilisateurService utilisateurService,
            IAnnonceService annonceService,
            IFavorisService<FavorisDTO> favorisService,
            INoteUtilisateurService noteUtilisateurService,
            IAuthService authService,
            IAbonnementService<AbonnementDTO> abonnementService,
            NavigationManager navigationManager,
            LoginViewModel connexionViewModel,
            IMediasService mediasService,
            ISignalementService signalementService,
            IBloqueService bloqueService,
            INotificationService notificationService)
        : base(navigationManager, authService, notificationService)
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

        public async Task LoadUserProfile(string login)
        {
            IsLoading = true;
            UserNotFound = false;
            UserSuspended = false;
            await base.LoadAsync();
            await LoadCurrentUserId();
            try
            {
                UtilisateurViewDTO user = await _utilisateurService.GetByLoginAsync(login);

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

                IsBlockedByUser = user.BlockedByCurrentUser;
                IsFollowing = user.FolloweddByCurrentUser;

                 ViewingUser = user;

                var tasks = new List<Task>
                 {
                     Task.Run(async () => {
                         Annonces = await _annonceService.GetAnnoncesByUserIdAsync(ViewingUser.UtilisateurId);
                         IsLoadingArticles = false;
                         NotifyStateChanged();
                     }),
                     Task.Run(async () => {
                         Avis = await _noteUtilisateurService.GetAllNotesByUtilisateurId(ViewingUser.UtilisateurId);
                         AvisCount = Avis?.Count ?? 0;
                         IsLoadingAvis = false;
                         NotifyStateChanged();
                     })
                 };

                UtilisateurDTO? utilisateur = await _authService.GetCurrentUserAsync();
                if (utilisateur != null && ViewingUser != null && utilisateur.UtilisateurId == ViewingUser.UtilisateurId)
                {
                    IsSameUser = true;
                    IsLoadingFavoris = true;
                    Console.WriteLine("Task 1");
                    FavorisAnnonce = await _annonceService.GetByFavorisUtilisateur();
                    Console.WriteLine("Task 1");
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

        public async Task ToggleFavorite(AnnonceDTO annonce)
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

            bool wasFollowing = ViewingUser.FolloweddByCurrentUser;
            ViewingUser.FolloweddByCurrentUser = !ViewingUser.FolloweddByCurrentUser;

            NotifyStateChanged();

            try
            {
                if (!wasFollowing)
                {
                    ViewingUser.Abonnes += 1;
                    await _abonnementService.AddAbonnement(ViewingUser.UtilisateurId);
                }
                else
                {
                    ViewingUser.Abonnes -= 1;
                    await _abonnementService.DeleteAbonnement(ViewingUser.UtilisateurId);
                }
            }
            catch
            {
                ViewingUser.FolloweddByCurrentUser = wasFollowing;
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
                NoteUtilisateurCreateDTO newReview = new NoteUtilisateurCreateDTO
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

        public async Task ToggleNotifMailPreference()
        {
            // Sécurité front
            if (!IsSameUser || ViewingUser == null)
                return;

            // Règle métier : email vérifié
            if (!ViewingUser.ValidEmail)
            {
                NotifMailErrorMessage = "Vous devez vérifier votre adresse email pour activer les notifications.";
                NotifyStateChanged();
                return;
            }

            IsUpdatingNotifMail = true;
            NotifMailErrorMessage = null;
            NotifyStateChanged();

            bool newValue = !ViewingUser.PreferenceNotifMail;

            try
            {
                await _utilisateurService.UpdateNotifMailPreferenceAsync(
                    ViewingUser.UtilisateurId,
                    newValue);

                // Mise à jour locale si succès API
                ViewingUser.PreferenceNotifMail = newValue;
            }
            catch (Exception ex)
            {
                NotifMailErrorMessage = "Erreur lors de la mise à jour de la préférence.";
                Console.WriteLine(ex.Message);
            }
            finally
            {
                IsUpdatingNotifMail = false;
                NotifyStateChanged();
            }
        }


        public async void SignalerUtilisateur()
        {
            showDotsDropdown = false;
            SignalementUtilisateurCreateDTO signalement = new SignalementUtilisateurCreateDTO
            {
                SignalementMotif = "Inapproprié",
                UtilisateurSignaleId = ViewingUser!.UtilisateurId,
            };

            await _signalementService.CreateSignalement(signalement);
            
        }
        public void ToggleSignalerModal()
        {
            SignalementRaison = string.Empty;
            showDotsDropdown = false;
            if (CheckLoginStatus == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            ShowSignalerModal = !ShowSignalerModal;
        }
        public void OpenSignalerAvis(int noteId)
        {
            SignalementRaison = string.Empty;
            showDotsDropdown = false;
            SignalementIdAvis = noteId;
            ToggleSignalerAvisModal();
        }
        public void ToggleSignalerAvisModal()
        {
            showDotsDropdown = false;
            if (CheckLoginStatus == null)
            {
                _navigationManager.NavigateTo("/login");
            }

            ShowSignalerAvisModal = !ShowSignalerAvisModal;
        }
        public async Task SubmitReport()
        {
            if (CheckLoginStatus == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            IsSubmittingReport = true;
            try
            {
                SignalementUtilisateurCreateDTO newReport = new SignalementUtilisateurCreateDTO
                {
                    SignalementMotif = SignalementRaison,
                    UtilisateurSignaleId = ViewingUser!.UtilisateurId,
                };

                SignalementDetailsDTO result = await _signalementService.CreateSignalement(newReport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la soumission du signalement : {ex.Message}");
            }
            finally
            {
                IsSubmittingReport = false;
                ShowSignalerModal = false;
                SignalementRaison = string.Empty;
            }
        }
        public async Task SubmitAvisReport()
        {
            if (CheckLoginStatus == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            IsSubmittingReport = true;
            try
            {
                SignalementAvisCreateDTO newReport = new SignalementAvisCreateDTO
                {
                    SignalementMotif = SignalementRaison,
                    AvisId = SignalementIdAvis,

                };

                SignalementDetailsDTO result = await _signalementService.CreateSignalement(newReport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la soumission du signalement : {ex.Message}");
            }
            finally
            {
                IsSubmittingReport = false;
                ShowSignalerAvisModal = false;
                SignalementIdAvis = 0;
                SignalementRaison = string.Empty;
            }
        }
        private async Task LoadCurrentUserId()
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
                currentUserId = null;

            currentUserId = user?.UtilisateurId;
        }

        public bool IsSameUserAsReviewer(int noteurId)
        {
            if (currentUserId == null)
            {
                return false;
            }
            return currentUserId.Value == noteurId;
        }
        public string GetPhoto(int id)
        {
            return _mediaService.GetPhotoUrl(id);
        }

        public async Task<bool> CheckIfOwnerAnnonce(AnnonceDTO annonce)
        {
            UtilisateurDTO? currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
                return false;

            return annonce.IdAuteur == currentUser.UtilisateurId;
        }
    }
}
