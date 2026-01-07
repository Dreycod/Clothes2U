using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO.Annonce;
using Shared.DTO.Couleur;
using Shared.DTO.Mesures;
using System.Collections.ObjectModel;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Shared.DTO;
using Shared.DTO.Categorie;
using Shared.DTO.EtatArticle;
using Shared.DTO.Marque;
using Shared.DTO.Taille;

namespace FrontBlazor.ViewModel
{
    /// <summary>
    /// ViewModel pour la création d'annonce
    /// Pattern EXACTEMENT identique à MessagerieViewModel
    /// </summary>
    public class CreationAnnonceViewModel : ClientBaseViewModel, IDisposable
    {
        private readonly IAnnonceService _annonceService;
        private readonly IMediasService _mediaService;
        private readonly IAuthService _authService;
        private readonly IMesureService _mesureService;
        private readonly NavigationManager _nav;
        public List<CategorieDTO> Categories { get; set; }
        public List<CouleurDTO> Couleurs { get; set; }
        public List<GenreDTO> Genres { get; set; }
        public List<MarqueDTO> Marques { get; set; }
        public List<TailleDTO> Tailles { get; set; }
        public List<EtatArticleDTO> Etats { get; set; }

        private readonly IListableService<CategorieDTO> _categorieService;
        private readonly IListableService<CouleurDTO> _couleurService;
        private readonly IListableService<MarqueDTO> _marqueService;
        private readonly IListableService<EtatArticleDTO> _etatService;
        private readonly IListableService<GenreDTO> _genreService;
        private readonly IListableService<TailleDTO> _tailleService;

        // État de l'annonce en cours de création
        public CreateAnnonceDTO NewAnnonce { get; private set; } = new();

        // Photos
        public List<IBrowserFile> SelectedFiles { get; set; } = new();
        public List<(IBrowserFile File, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();

        // IDs des couleurs sélectionnées (multi-sélection)
        public List<int> SelectedCouleurIds { get; private set; } = new();

        // Tags personnalisés
        public List<string> Tags { get; private set; } = new();

        // Cache des mesures pour filtrage des tailles
        private List<MesureDTO>? _allMesures;
        public List<int> AvailableTailleIds { get; private set; } = new();

        // État de l'UI
        public bool IsLoading { get; private set; } = false;
        public bool IsUploadingPhotos { get; private set; } = false;
        public List<string> ErrorMessages { get; private set; } = new();
        public bool? HasCreated { get; private set; } = null;

        // Event pour notifier les changements d'état
        public event Action? OnChange;

        public CreationAnnonceViewModel(
            IAnnonceService annonceService,
            IMediasService mediaService,
            IAuthService authService,
            IMesureService mesureService,
            IListableService<CategorieDTO> categorieService,
            IListableService<CouleurDTO> couleurService,
            IListableService<MarqueDTO> marqueService,
            IListableService<EtatArticleDTO> etatService,
            IListableService<GenreDTO> genreService,
            IListableService<TailleDTO> tailleService, 
            NavigationManager navigationManager,
            INotificationService notificationService,
            NavigationManager nav): base(navigationManager, authService, notificationService)
        {
            _annonceService = annonceService ?? throw new ArgumentNullException(nameof(annonceService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _mesureService = mesureService ?? throw new ArgumentNullException(nameof(mesureService));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
            _categorieService = categorieService;
            _marqueService = marqueService;
            _couleurService = couleurService;
            _etatService = etatService;
            _genreService = genreService;
            _tailleService = tailleService;
        }

        #region Initialization

        public async Task LoadAsync()
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                _nav.NavigateTo("/login");
                return;
            }

            NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
            NewAnnonce.DateAnnonce = DateTime.UtcNow;
            NewAnnonce.StatutAnnonceId = 1;

            IsLoading = true;
            Genres = await _genreService.GetAllAsync();
            Categories = await _categorieService.GetAllAsync();
            Marques = await _marqueService.GetAllAsync();
            Couleurs = await _couleurService.GetAllAsync();
            Etats = await _etatService.GetAllAsync();
            Tailles = await _tailleService.GetAllAsync();
            IsLoading = false;

            await LoadMesuresAsync();

            Console.WriteLine($"✅ ViewModel initialisé pour utilisateur {currentUser.UtilisateurId}");
            NotifyStateChanged();
        }

        private async Task LoadMesuresAsync()
        {
            try
            {
                _allMesures = await _mesureService.GetAllMesuresAsync();
                Console.WriteLine($"✅ {_allMesures?.Count ?? 0} mesures chargées");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur chargement mesures: {ex.Message}");
                _allMesures = new List<MesureDTO>();
            }
        }

        public void UpdateAvailableTailles(int sousCategorieId)
        {
            if (_allMesures == null || sousCategorieId == 0)
            {
                AvailableTailleIds.Clear();
                NotifyStateChanged();
                return;
            }

            AvailableTailleIds = _allMesures
                .Where(m => m.SousCategorieId == sousCategorieId)
                .Select(m => m.TailleId)
                .Distinct()
                .ToList();

            Console.WriteLine($"✅ {AvailableTailleIds.Count} tailles disponibles pour catégorie {sousCategorieId}");

            if (NewAnnonce.TailleId != 0 && !AvailableTailleIds.Contains(NewAnnonce.TailleId))
            {
                NewAnnonce.TailleId = 0;
                Console.WriteLine("⚠️ Taille réinitialisée car non disponible pour cette catégorie");
            }

            NotifyStateChanged();
        }

        #endregion

        #region Photo Management

        public async Task OnImagesSelectedAsync(InputFileChangeEventArgs e)
        {
            const int maxPhotos = 5;
            const long maxFileSize = 10 * 1024 * 1024;

            if (SelectedFilePreviews.Count >= maxPhotos)
            {
                AddError($"Maximum {maxPhotos} photos autorisées");
                return;
            }

            var filesToAdd = e.GetMultipleFiles(maxPhotos - SelectedFilePreviews.Count);

            foreach (var file in filesToAdd)
            {
                try
                {
                    if (file.Size > maxFileSize)
                    {
                        AddError($"{file.Name} est trop volumineux (max 10MB)");
                        continue;
                    }

                    if (!file.ContentType.StartsWith("image/"))
                    {
                        AddError($"{file.Name} n'est pas une image valide");
                        continue;
                    }

                    using var ms = new MemoryStream();
                    await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(ms);

                    var base64 = Convert.ToBase64String(ms.ToArray());
                    var dataUrl = $"data:{file.ContentType};base64,{base64}";

                    SelectedFilePreviews.Add((file, dataUrl));

                    Console.WriteLine($"✅ Photo ajoutée: {file.Name} ({file.Size} bytes)");
                }
                catch (Exception ex)
                {
                    AddError($"Erreur lors du chargement de {file.Name}: {ex.Message}");
                    Console.WriteLine($"❌ Erreur photo upload: {ex.Message}");
                }
            }

            NotifyStateChanged();
        }

        public void RemoveSelectedPhotoAt(int index)
        {
            if (index < 0 || index >= SelectedFilePreviews.Count)
                return;

            var removed = SelectedFilePreviews[index];
            SelectedFilePreviews.RemoveAt(index);

            if (SelectedFiles != null && index < SelectedFiles.Count)
                SelectedFiles.RemoveAt(index);

            Console.WriteLine($"🗑️ Photo supprimée: {removed.File.Name}");
            NotifyStateChanged();
        }

        #endregion

        #region Color Management

        public void ToggleCouleur(int couleurId)
        {
            if (SelectedCouleurIds.Contains(couleurId))
            {
                SelectedCouleurIds.Remove(couleurId);
                Console.WriteLine($"➖ Couleur {couleurId} désélectionnée");
            }
            else
            {
                SelectedCouleurIds.Add(couleurId);
                Console.WriteLine($"➕ Couleur {couleurId} sélectionnée");
            }

            NotifyStateChanged();
        }

        public bool IsCouleurSelected(int couleurId)
        {
            return SelectedCouleurIds.Contains(couleurId);
        }

        #endregion

        #region Tags Management

        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                return;

            var cleanTag = tag.Trim().ToLower();

            if (Tags.Any(t => t.Equals(cleanTag, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"⚠️ Tag '{cleanTag}' déjà ajouté");
                return;
            }

            Tags.Add(cleanTag);
            Console.WriteLine($"✅ Tag ajouté: {cleanTag}");
            NotifyStateChanged();
        }

        public void RemoveTag(string tag)
        {
            if (Tags.Remove(tag))
            {
                Console.WriteLine($"🗑️ Tag supprimé: {tag}");
                NotifyStateChanged();
            }
        }

        #endregion

        #region Validation

        public List<string> ValidateAnnonce()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NewAnnonce.Titre))
                errors.Add("Le nom de l'article est obligatoire");

            if (string.IsNullOrWhiteSpace(NewAnnonce.Description))
                errors.Add("La description est obligatoire");

            if (NewAnnonce.Prix <= 0)
                errors.Add("Le prix doit être supérieur à 0");

            if (NewAnnonce.MarqueId == 0)
                errors.Add("Veuillez sélectionner une marque");

            if (NewAnnonce.TailleId == 0)
                errors.Add("Veuillez sélectionner une taille");

            if (NewAnnonce.GenreId == 0)
                errors.Add("Veuillez sélectionner un genre");

            if (NewAnnonce.EtatId == 0)
                errors.Add("Veuillez sélectionner l'état de l'article");

            if (NewAnnonce.CategorieId == 0)
                errors.Add("Veuillez sélectionner une catégorie");

            if (NewAnnonce.SousCategorieId == 0)
                errors.Add("Veuillez sélectionner une sous-catégorie");

            if (!SelectedCouleurIds.Any())
                errors.Add("Veuillez sélectionner au moins une couleur");

            return errors;
        }

        #endregion

        #region Publishing

        public async Task PublishAnnonceAsync()
        {
            ErrorMessages.Clear();
            HasCreated = null;
            NotifyStateChanged();

            var validationErrors = ValidateAnnonce();
            if (validationErrors.Any())
            {
                ErrorMessages = validationErrors;
                HasCreated = false;
                NotifyStateChanged();
                return;
            }

            IsLoading = true;
            NotifyStateChanged();

            try
            {
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    AddError("Vous devez être connecté pour créer une annonce");
                    HasCreated = false;
                    return;
                }

                NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
                NewAnnonce.DateAnnonce = DateTime.UtcNow;
                NewAnnonce.StatutAnnonceId = 1;
                NewAnnonce.Couleurs = SelectedCouleurIds;

                // TODO: Ajouter les tags à l'annonce quand le DTO sera mis à jour
                // NewAnnonce.Tags = Tags;

                Console.WriteLine($"📤 Envoi de l'annonce: {NewAnnonce.Titre}");
                Console.WriteLine($"   - Prix: {NewAnnonce.Prix}€");
                Console.WriteLine($"   - Photos: {SelectedFilePreviews.Count}");
                Console.WriteLine($"   - Tags: {string.Join(", ", Tags)}");

                await _annonceService.CreateAnnonce(NewAnnonce);
                Console.WriteLine($"✅ Annonce créée avec succès");

                if (SelectedFilePreviews.Any())
                {
                    IsUploadingPhotos = true;
                    NotifyStateChanged();

                    Console.WriteLine($"📸 Upload de {SelectedFilePreviews.Count} photos...");

                    try
                    {
                        var userAnnonces = await _annonceService.GetAnnoncesByUserIdAsync(currentUser.UtilisateurId);
                        var createdAnnonce = userAnnonces?
                            .OrderByDescending(a => a.AnnonceId)
                            .FirstOrDefault(a => a.Titre == NewAnnonce.Titre);

                        if (createdAnnonce != null && createdAnnonce.AnnonceId > 0)
                        {
                            Console.WriteLine($"✅ Annonce retrouvée avec ID: {createdAnnonce.AnnonceId}");

                            var photosDataUrls = SelectedFilePreviews
                                .Select(p => p.PreviewBase64)
                                .ToList();

                            var uploadSuccess = await _mediaService.UploadMultiplePhotosAnnonceAsync(
                                createdAnnonce.AnnonceId,
                                photosDataUrls
                            );

                            if (!uploadSuccess)
                            {
                                AddError("⚠️ L'annonce a été créée mais certaines photos n'ont pas pu être uploadées.");
                            }
                            else
                            {
                                Console.WriteLine($"✅ Toutes les photos uploadées avec succès");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"⚠️ Impossible de retrouver l'annonce pour uploader les photos");
                            AddError("⚠️ L'annonce a été créée mais les photos n'ont pas pu être uploadées.");
                        }
                    }
                    catch (Exception photoEx)
                    {
                        Console.WriteLine($"❌ Erreur upload photos: {photoEx.Message}");
                        AddError("⚠️ L'annonce a été créée mais erreur lors de l'upload des photos.");
                    }
                    finally
                    {
                        IsUploadingPhotos = false;
                    }
                }

                HasCreated = true;
                NotifyStateChanged();

                await Task.Delay(2000);
                _nav.NavigateTo($"/profile/{currentUser.Login}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la publication: {ex.Message}");
                AddError($"Erreur: {ex.Message}");
                HasCreated = false;
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        #endregion

        #region Error Management

        private void AddError(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
            NotifyStateChanged();
        }

        public void ClearErrors()
        {
            ErrorMessages.Clear();
            NotifyStateChanged();
        }

        #endregion

        #region State Management

        private void NotifyStateChanged() => OnChange?.Invoke();

        public void Dispose()
        {
            Console.WriteLine("🧹 Dispose CreationAnnonceViewModel");
        }

        #endregion
    }
}