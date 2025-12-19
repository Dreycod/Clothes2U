using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Photo;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Annonce;
using Shared.DTO.Couleur;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrontBlazor.ViewModel
{
    public class CreationAnnonceViewModel : INotifyPropertyChanged
    {
        private readonly IAnnonceService<CreateAnnonceDTO> _annonceService;
        private readonly IMediasService<PhotoResponseDTO> _mediaService;
        private readonly IAuthService _authService;

        // Observable properties
        private bool _isLoading;
        private bool _isUploadingPhotos;
        private List<string> _errorMessages = new();
        private bool? _hasCreated;
        private CreateAnnonceDTO _newAnnonce = new();
        private List<string> _uploadedPhotos = new();

        public CreationAnnonceViewModel(
            IAnnonceService<CreateAnnonceDTO> annonceService,
            IMediasService<PhotoResponseDTO> mediaService,
            IAuthService authService)
        {
            _annonceService = annonceService ?? throw new ArgumentNullException(nameof(annonceService));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        #region Observable Properties

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsUploadingPhotos
        {
            get => _isUploadingPhotos;
            set => SetProperty(ref _isUploadingPhotos, value);
        }

        public List<string> ErrorMessages
        {
            get => _errorMessages;
            set => SetProperty(ref _errorMessages, value);
        }

        public bool? HasCreated
        {
            get => _hasCreated;
            set => SetProperty(ref _hasCreated, value);
        }

        public CreateAnnonceDTO NewAnnonce
        {
            get => _newAnnonce;
            set => SetProperty(ref _newAnnonce, value);
        }

        public List<string> UploadedPhotos
        {
            get => _uploadedPhotos;
            set => SetProperty(ref _uploadedPhotos, value);
        }

        #endregion

        #region Bindable Properties for UI

        public string ArticleName
        {
            get => NewAnnonce.Titre ?? string.Empty;
            set
            {
                if (NewAnnonce.Titre != value)
                {
                    NewAnnonce.Titre = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal? Price
        {
            get => NewAnnonce.Prix == 0 ? null : NewAnnonce.Prix;
            set
            {
                var newValue = value ?? 0;
                if (NewAnnonce.Prix != newValue)
                {
                    NewAnnonce.Prix = newValue;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsNegotiable
        {
            get => NewAnnonce.EstNegociable;
            set
            {
                if (NewAnnonce.EstNegociable != value)
                {
                    NewAnnonce.EstNegociable = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => NewAnnonce.Description ?? string.Empty;
            set
            {
                if (NewAnnonce.Description != value)
                {
                    NewAnnonce.Description = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedMarqueId
        {
            get => NewAnnonce.MarqueId;
            set
            {
                if (NewAnnonce.MarqueId != value)
                {
                    NewAnnonce.MarqueId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedTailleId
        {
            get => NewAnnonce.TailleId;
            set
            {
                if (NewAnnonce.TailleId != value)
                {
                    NewAnnonce.TailleId = value;
                    OnPropertyChanged();
                }
            }
        }

        /*public int? SelectedCouleurId
        {
            get => NewAnnonce.CouleurId;
            set
            {
                if (NewAnnonce.CouleurId != value)
                {
                    NewAnnonce.CouleurId = value;
                    OnPropertyChanged();
                }
            }
        }*/

        public int SelectedGenreId
        {
            get => NewAnnonce.GenreId;
            set
            {
                if (NewAnnonce.GenreId != value)
                {
                    NewAnnonce.GenreId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedCategorieId
        {
            get => NewAnnonce.CategorieId;
            set
            {
                if (NewAnnonce.CategorieId != value)
                {
                    NewAnnonce.CategorieId = value;
                    // Reset subcategory when category changes
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedSousCategorieId
        {
            get => NewAnnonce.SousCategorieId;
            set
            {
                if (NewAnnonce.SousCategorieId != value)
                {
                    NewAnnonce.SousCategorieId = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SelectedEtatArticleId
        {
            get => NewAnnonce.EtatId;
            set
            {
                if (NewAnnonce.EtatId != value)
                {
                    NewAnnonce.EtatId = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Business Logic Methods

        public void InitializeNewAnnonce(CreateAnnonceDTO? existingAnnonce = null)
        {
            if (existingAnnonce != null)
            {
                NewAnnonce = existingAnnonce;
            }
            else
            {
                NewAnnonce = new CreateAnnonceDTO
                {
                    DateAnnonce = DateTime.UtcNow,
                    StatutAnnonceId = 1
                };
            }

            UploadedPhotos.Clear();
            ErrorMessages.Clear();
            HasCreated = null;
        }

        public async Task<bool> AddPhotoAsync(string base64Photo)
        {
            const int maxPhotos = 5;

            if (UploadedPhotos.Count >= maxPhotos)
            {
                AddError($"Maximum {maxPhotos} photos autorisées");
                return false;
            }

            if (string.IsNullOrWhiteSpace(base64Photo))
            {
                AddError("Photo invalide");
                return false;
            }

            UploadedPhotos.Add(base64Photo);
            OnPropertyChanged(nameof(UploadedPhotos));
            return true;
        }

        public void RemovePhoto(string photo)
        {
            if (UploadedPhotos.Remove(photo))
            {
                OnPropertyChanged(nameof(UploadedPhotos));
            }
        }

        public void ClearErrors()
        {
            ErrorMessages.Clear();
            OnPropertyChanged(nameof(ErrorMessages));
        }

        public void AddError(string errorMessage)
        {
            ErrorMessages.Add(errorMessage);
            OnPropertyChanged(nameof(ErrorMessages));
        }

        public List<string> ValidateAnnonce()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(NewAnnonce.Titre))
                errors.Add("Le nom de l'article est obligatoire");

            if (string.IsNullOrWhiteSpace(NewAnnonce.Description))
                errors.Add("La description est obligatoire");

            if (NewAnnonce.Prix <= 0)
                errors.Add("Le prix doit être supérieur à 0");

            if (!String.IsNullOrEmpty(NewAnnonce.MarqueId.ToString()) || NewAnnonce.MarqueId == 0)
                errors.Add("Veuillez sélectionner une marque");

            if (!String.IsNullOrEmpty(NewAnnonce.MarqueId.ToString()) || NewAnnonce.TailleId == 0)
                errors.Add("Veuillez sélectionner une taille");

            if (!String.IsNullOrEmpty(NewAnnonce.MarqueId.ToString()) || NewAnnonce.GenreId == 0)
                errors.Add("Veuillez sélectionner un genre");

            if (!String.IsNullOrEmpty(NewAnnonce.MarqueId.ToString()) || NewAnnonce.EtatId == 0)
                errors.Add("Veuillez sélectionner l'état de l'article");

            if (!String.IsNullOrEmpty(NewAnnonce.MarqueId.ToString()) || NewAnnonce.CategorieId == 0)
                errors.Add("Veuillez sélectionner une catégorie");

            return errors;
        }

        public async Task<bool> PublishAnnonceAsync()
        {
            ClearErrors();
            HasCreated = null;

            // Validation
            var validationErrors = ValidateAnnonce();
            if (validationErrors.Any())
            {
                ErrorMessages = validationErrors;
                HasCreated = false;
                return false;
            }

            IsLoading = true;

            try
            {
                // Get current user
                var currentUser = await _authService.GetCurrentUserAsync();
                if (currentUser == null)
                {
                    AddError("Vous devez être connecté pour créer une annonce");
                    HasCreated = false;
                    return false;
                }

                // Set user ID and dates
                NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
                NewAnnonce.DateAnnonce = DateTime.UtcNow;
                NewAnnonce.StatutAnnonceId = 1;

                // Create the announcement
                var createdAnnonce = await _annonceService.AddAsync(NewAnnonce);

                if (createdAnnonce == null)
                {
                    AddError("L'annonce n'a pas pu être créée. Erreur API.");
                    HasCreated = false;
                    return false;
                }

                // Update with created annonce (to get the ID)
                NewAnnonce = createdAnnonce;

                // Upload photos if any
                if (UploadedPhotos.Any())
                {
                    IsUploadingPhotos = true;

                    var uploadSuccess = await _mediaService.UploadMultiplePhotosAnnonceAsync(
                        1, // A CHANGER !!!!!!!!!!!!!!!!!!!!!!!!
                        UploadedPhotos
                    );

                    IsUploadingPhotos = false;

                    if (!uploadSuccess)
                    {
                        AddError("⚠️ L'annonce a été créée mais certaines photos n'ont pas pu être uploadées.");
                    }
                }

                HasCreated = true;
                return true;
            }
            catch (Exception ex)
            {
                AddError($"Erreur lors de la création de l'annonce: {ex.Message}");
                HasCreated = false;
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}