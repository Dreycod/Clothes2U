using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO.Annonce;
using Shared.DTO.Couleur;
using Shared.DTO.Mesures;
using System.Collections.ObjectModel;

namespace FrontBlazor.ViewModel
{
    /// <summary>
    /// ViewModel pour la création d'annonce
    /// Pattern EXACTEMENT identique à MessagerieViewModel
    /// </summary>
    public class CreationAnnonceViewModel : ComponentBase, IDisposable
    {
        private readonly IAnnonceService _annonceService;
        private readonly IMediasService _mediaService;
        private readonly IAuthService _authService;
        private readonly IMesureService _mesureService;
        private readonly NavigationManager _nav;

        // État de l'annonce en cours de création
        public CreateAnnonceDTO NewAnnonce { get; private set; } = new();

        // ✅ CORRECTION: Même structure que MessagerieViewModel
        public List<IBrowserFile> SelectedFiles { get; set; } = new();
        public List<(IBrowserFile File, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();

        // IDs des couleurs sélectionnées (multi-sélection)
        public List<int> SelectedCouleurIds { get; private set; } = new();

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
            NavigationManager nav)
        {
            _annonceService = annonceService ?? throw new ArgumentNullException(nameof(annonceService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _mesureService = mesureService ?? throw new ArgumentNullException(nameof(mesureService));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
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

            // ✅ Charger les mesures pour le filtrage des tailles
            await LoadMesuresAsync();

            Console.WriteLine($"✅ ViewModel initialisé pour utilisateur {currentUser.UtilisateurId}");
            NotifyStateChanged();
        }

        /// <summary>
        /// Charge toutes les mesures en cache pour le filtrage
        /// </summary>
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

        /// <summary>
        /// Filtre les tailles disponibles selon la catégorie sélectionnée
        /// </summary>
        public void UpdateAvailableTailles(int categorieId)
        {
            if (_allMesures == null || categorieId == 0)
            {
                AvailableTailleIds.Clear();
                NotifyStateChanged();
                return;
            }

            // Filtrer les tailles selon la catégorie
            AvailableTailleIds = _allMesures
                .Where(m => m.CategorieId == categorieId)
                .Select(m => m.TailleId)
                .Distinct()
                .ToList();

            Console.WriteLine($"✅ {AvailableTailleIds.Count} tailles disponibles pour catégorie {categorieId}");

            // Reset la taille sélectionnée si elle n'est plus disponible
            if (NewAnnonce.TailleId != 0 && !AvailableTailleIds.Contains(NewAnnonce.TailleId))
            {
                NewAnnonce.TailleId = 0;
                Console.WriteLine("⚠️ Taille réinitialisée car non disponible pour cette catégorie");
            }

            NotifyStateChanged();
        }

        #endregion

        #region Photo Management - COPIE EXACTE de MessagerieViewModel

        /// <summary>
        /// ✅ COPIE EXACTE de MessagerieViewModel.OnImagesSelectedAsync
        /// </summary>
        public async Task OnImagesSelectedAsync(InputFileChangeEventArgs e)
        {
            const int maxPhotos = 5;
            const long maxFileSize = 10 * 1024 * 1024; // 10 MB

            // Vérifier qu'on ne dépasse pas le max
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
                    // Validation du fichier
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

                    // ✅ EXACTEMENT comme MessagerieViewModel
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

        /// <summary>
        /// ✅ COPIE EXACTE de MessagerieViewModel.RemoveSelectedPhotoAt
        /// </summary>
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

            // Validation
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

                // Préparer les données
                NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
                NewAnnonce.DateAnnonce = DateTime.UtcNow;
                NewAnnonce.StatutAnnonceId = 1;

                // Ajouter les couleurs sélectionnées
                NewAnnonce.Couleurs = SelectedCouleurIds
                    .Where(id => id > 0)
                    .Select(id => new EstDeCouleurDTO { CouleurId = id })
                    .ToList();

                Console.WriteLine($"📤 Envoi de l'annonce: {NewAnnonce.Titre}");
                Console.WriteLine($"   - Prix: {NewAnnonce.Prix}€");
                Console.WriteLine($"   - Photos: {SelectedFilePreviews.Count}");

                // ✅ Créer l'annonce
                await _annonceService.CreateAnnonce(NewAnnonce);
                Console.WriteLine($"✅ Annonce créée avec succès");

                // 📸 Upload des photos si présentes
                if (SelectedFilePreviews.Any())
                {
                    IsUploadingPhotos = true;
                    NotifyStateChanged();

                    Console.WriteLine($"📸 Upload de {SelectedFilePreviews.Count} photos...");

                    try
                    {
                        // Récupérer l'annonce créée
                        var userAnnonces = await _annonceService.GetAnnoncesByUserIdAsync(currentUser.UtilisateurId);
                        var createdAnnonce = userAnnonces?
                            .OrderByDescending(a => a.AnnonceId)
                            .FirstOrDefault(a => a.Titre == NewAnnonce.Titre);

                        if (createdAnnonce != null && createdAnnonce.AnnonceId > 0)
                        {
                            Console.WriteLine($"✅ Annonce retrouvée avec ID: {createdAnnonce.AnnonceId}");

                            // ✅ Extraire les data URLs (COMME MessagerieViewModel)
                            var photosDataUrls = SelectedFilePreviews
                                .Select(p => p.PreviewBase64)
                                .ToList();

                            // ✅ Utiliser le service existant
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

                // Redirection après succès
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