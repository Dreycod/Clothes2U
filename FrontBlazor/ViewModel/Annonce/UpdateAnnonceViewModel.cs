using FrontBlazor.Exceptions;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.Interfaces.GenericIServices;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.Detection;
using Shared.DTO.EtatArticle;
using Shared.DTO.Marque;
using Shared.DTO.Mesures;
using Shared.DTO.Photo;
using Shared.DTO.Recense;
using Shared.DTO.Tag;
using Shared.DTO.Taille;
using Shared.DTO.Utilisateur;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace FrontBlazor.ViewModel;

public class UpdateAnnonceViewModel : ClientBaseViewModel, IDisposable
{
    private readonly IAnnonceService _annonceService;
    private readonly IMediasService _mediaService;
    private readonly IAuthService _authService;
    private readonly IListableService<MesureDTO> _mesureService;
    private readonly NavigationManager _nav;
    private readonly ICouleurService<CouleurDTO> _couleurService;
    private readonly ITagService<TagDTO> _tagService;
    public List<CategorieDTO> Categories { get; set; }
    public List<CouleurDTO> Couleurs { get; set; }
    public List<GenreDTO> Genres { get; set; }
    public List<MarqueDTO> Marques { get; set; }
    public List<TailleDTO> Tailles { get; set; }
    public List<EtatArticleDTO> Etats { get; set; }

    private readonly IListableService<CategorieDTO> _categorieService;
    private readonly IListableService<MarqueDTO> _marqueService;
    private readonly IListableService<EtatArticleDTO> _etatService;
    private readonly IListableService<GenreDTO> _genreService;
    private readonly IListableService<TailleDTO> _tailleService;
    // État de l'annonce en cours de création
    public PutAnnonceDTO NewAnnonce { get; private set; } = new();

    // Photos
    public List<IBrowserFile> SelectedFiles { get; set; } = new();
    public List<(IBrowserFile File, string PreviewBase64, bool IsDangerous, bool IsTextile)> SelectedFilePreviews { get; set; } = new();
    // IDs des couleurs sélectionnées (multi-sélection)
    public List<int> SelectedCouleurIds { get; private set; } = new();

    // Tags personnalisés
    public List<string> Tags { get; private set; } = new();

    // Cache des mesures pour filtrage des tailles
    private List<MesureDTO>? _allMesures;
    private List<TagDTO>? _allTags;
    public List<int> AvailableTailleIds { get; private set; } = new();

    // État de l'UI
    public bool IsLoading { get; private set; } = false;
    public bool IsUploadingPhotos { get; private set; } = false;
    public bool IsUpdatingTags { get; private set; } = false;
    public List<string> ErrorMessages { get; private set; } = new();
    public List<string> API_Messages { get; private set; } = new();
    public bool? HasCreated { get; private set; } = null;

    // Event pour notifier les changements d'état
    public event Action? OnChange;

    public UpdateAnnonceViewModel(
        IAnnonceService annonceService,
        IMediasService mediaService,
        IAuthService authService,
        IListableService<MesureDTO> mesureService,
        IListableService<CategorieDTO> categorieService,
        ICouleurService<CouleurDTO> couleurService,
        IListableService<MarqueDTO> marqueService,
        IListableService<EtatArticleDTO> etatService,
        IListableService<GenreDTO> genreService,
        IListableService<TailleDTO> tailleService,
        NavigationManager navigationManager,
        INotificationService notificationService,
        ITagService<TagDTO> tagService,
        ISignalRService notificationHubService,
        NavigationManager nav) : base(navigationManager, authService, notificationHubService, notificationService)
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
        _tagService = tagService;
        _tagService = tagService;
    }

    #region Initialization

    public async Task LoadAsync(int AnnonceId)
    {
        var currentUser = await _authService.GetCurrentUserAsync();
        if (currentUser == null)
        {
            _nav.NavigateTo("/login");
            return;
        }


        IsLoading = true;
        AnnonceDetailDTO? annonce = await _annonceService.GetAnnonceDetailById(AnnonceId);
        
        Genres = await _genreService.GetAllAsync();
        Categories = await _categorieService.GetAllAsync();
        Marques = await _marqueService.GetAllAsync();
        Couleurs = await _couleurService.GetAllAsync();
        Etats = await _etatService.GetAllAsync();
        Tailles = await _tailleService.GetAllAsync();
        IsLoading = false;
        await LoadMesuresAsync();
        await LoadTagsAsync();
        await LoadAnnonceAsync(AnnonceId, currentUser);

        Console.WriteLine($"✅ ViewModel initialisé pour utilisateur {currentUser.UtilisateurId}");
        NotifyStateChanged();
    }
    public async Task LoadAnnonceAsync(int AnnonceId, CurrentUtilisateurDTO currentUser)
    {
        // Récupérer le détail de l'annonce
        AnnonceDetailDTO? annonce = await _annonceService.GetAnnonceDetailById(AnnonceId);
        if (annonce == null) return;

        // Utilisateur actuel
        NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
        NewAnnonce.AnnonceId = annonce.AnnonceId;
        NewAnnonce.Titre = annonce.Title;
        NewAnnonce.Description = annonce.Description;
        NewAnnonce.DateAnnonce = annonce.DateAnnonce;
        NewAnnonce.EstNegociable = annonce.Negociable;
        NewAnnonce.Prix = annonce.Prix;
        NewAnnonce.StatutAnnonceId = annonce.StatutAnnonceId;

        // ---------------------------
        // Traduire les noms en IDs
        // ---------------------------

        // Marque
        var marque = Marques.FirstOrDefault(m => m.NomMarque == annonce.NomMarque);
        NewAnnonce.MarqueId = marque?.MarqueID ?? 0;

        // Etat
        var etat = Etats.FirstOrDefault(e => e.NomEtat == annonce.EtatArticle);
        NewAnnonce.EtatId = etat?.EtatArticleId ?? 0;

        // Taille
        var taille = Tailles.FirstOrDefault(t => t.Libelletaille == annonce.Taille);
        NewAnnonce.TailleId = taille?.TailleId ?? 0;

        // Categorie & SousCategorie
        if (!string.IsNullOrWhiteSpace(annonce.Categorie))
        {
            var categorie = Categories.FirstOrDefault(c => c.LibelleCategorie == annonce.Categorie);
            NewAnnonce.CategorieId = categorie?.IdCategorie ?? 0;

            if (!string.IsNullOrWhiteSpace(annonce.SousCategorie) && categorie != null)
            {
                var sousCat = categorie.SousCategories?.FirstOrDefault(sc => sc.LibelleSousCategorie == annonce.SousCategorie);
                NewAnnonce.SousCategorieId = sousCat?.SousCategorieId ?? 0;
            }
        }

        // Couleurs (IDs)
        if (annonce.Couleurs?.Any() == true)
        {
            Console.WriteLine($"✅ Couleurs de l'annonce: {string.Join(", ", annonce.Couleurs)}");
            NewAnnonce.Couleurs = annonce.Couleurs
                .Select(nom => Couleurs.FirstOrDefault(c => c.Nom == nom)?.CouleurId)
                .Where(id => id != null)
                .Select(id => id.Value)
                .ToList();


            SelectedCouleurIds.Clear(); 

            foreach (var couleurId in NewAnnonce.Couleurs)
            {
                ToggleCouleur(couleurId);
            }

            UpdateAvailableTailles(NewAnnonce.SousCategorieId);
        }
        else
        {
            NewAnnonce.Couleurs = new List<int>();
        }

        // Genre
        NewAnnonce.GenreId = Genres.FirstOrDefault(g => g.NomGenre == annonce.GenreAnnonce)?.GenreId ?? 0;
        
        List<int> photoIds = annonce.Photos;
        var simulatedFiles = new List<IBrowserFile>();

        foreach (var photoId in photoIds)
        {
            PhotoDTO? photo = await _mediaService.GetPhotoDTO(photoId);

            var file = new InMemoryBrowserFile(
                photo.Image,
                $"photo_{photo.PhotoId}.jpg",
                "image/jpeg"
            );

            var base64 = Convert.ToBase64String(photo.Image);
            var preview = $"data:image/jpeg;base64,{base64}";

            await AddImageAsync(file, photo.Image, preview);
        }

        // Create the InputFileChangeEventArgs for the simulated files
        var simulatedArgs = new InputFileChangeEventArgs(simulatedFiles);

        // Call the OnImagesSelectedAsync with the simulated args
        await OnImagesSelectedAsync(simulatedArgs);
    }

    private async Task LoadMesuresAsync()
    {
        try
        {
            _allMesures = await _mesureService.GetAllAsync();
            Console.WriteLine($"✅ {_allMesures?.Count ?? 0} mesures chargées");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur chargement mesures: {ex.Message}");
            _allMesures = new List<MesureDTO>();
        }
    }

    private async Task LoadTagsAsync()
    {
        try
        {
            _allTags = await _tagService.GetAllAsync();
            Console.WriteLine($"✅ {_allTags?.Count ?? 0} tags chargées");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur chargement tags: {ex.Message}");
            _allTags = new List<TagDTO>();
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

        Console.WriteLine($"✅ {AvailableTailleIds.Count} tailles disponibles pour sous catégorie {sousCategorieId}");

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

        Console.WriteLine($"📸 {e.GetMultipleFiles().Count} photos sélectionnées");
        var filesToAdd = e.GetMultipleFiles(maxPhotos - SelectedFilePreviews.Count);
        foreach (var file in filesToAdd)
        {
            try
            {
                if (file.Size > maxFileSize)
                {
                    AddAPIMessage($"{file.Name} est trop volumineux (max 10MB)");
                    continue;
                }

                if (!file.ContentType.StartsWith("image/"))
                {
                    AddAPIMessage($"{file.Name} n'est pas une image valide");
                    continue;
                }

                using var ms = new MemoryStream();
                await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(ms);

                var base64 = Convert.ToBase64String(ms.ToArray());
                var dataUrl = $"data:{file.ContentType};base64,{base64}";

                await AddImageAsync(file, ms.ToArray(), dataUrl);

            }
            catch (Exception ex)
            {
                AddAPIMessage($"Erreur lors du chargement de {file.Name}: {ex.Message}");
                Console.WriteLine($"❌ Erreur photo upload: {ex.Message}");
            }
        }

        NotifyStateChanged();
    }
    private async Task AddImageAsync(IBrowserFile file, byte[] imageBytes, string previewBase64)
    {
        const int maxPhotos = 5;

        if (SelectedFilePreviews.Count >= maxPhotos)
        {
            AddError($"Maximum {maxPhotos} photos autorisées");
            return;
        }

        PhotoUploadDTO photoUpload = new PhotoUploadDTO
        {
            Base64Data = previewBase64,
            FileName = file.Name
        };

        try
        {
            var detection = await _mediaService.DetectImageDanger(photoUpload);

            if (!detection.Success)
            {
                AddAPIMessage($"Image {file.Name} rejetée: {detection.ErrorMessage}");
                return;
            }
            Console.WriteLine($"✅ Image {file.Name} vérifie");
            Console.WriteLine($"✅ Danger : {detection.IsDangerous}");
            Console.WriteLine($"✅ Détection: {detection.DangerAccuracy}");
            Console.WriteLine($"✅ Textile: {detection.IsTextile}");
            Console.WriteLine($"✅ Détection: {detection.TextileAccuracy}");


            SelectedFilePreviews.Add((
                File: file,
                PreviewBase64: previewBase64,
                IsDangerous: detection.IsDangerous,
                IsTextile: detection.IsTextile
            ));

            Console.WriteLine($"✅ Photo ajoutée: {file.Name}");
            API_Messages.RemoveAll(m => m.Contains("FastAPI"));
        }
        catch (Exception ex)
        {
            AddAPIMessage($"Erreur image {file.Name}: {ex.Message}");
        }
    }

    public void RemoveSelectedPhotoAt(int index)
    {
        if (index < 0 || index >= SelectedFilePreviews.Count)
            return;

        var removed = SelectedFilePreviews[index];
        SelectedFilePreviews.RemoveAt(index);

        if (SelectedFiles != null && index < SelectedFiles.Count)
            SelectedFiles.RemoveAt(index);

        API_Messages.RemoveAll(m => m.Contains($"{removed.File.Name}"));

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

    // Dans la région #region Publishing

    public async Task PublishAnnonceAsync()
    {
        ErrorMessages.Clear();
        API_Messages.Clear();
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

        AnnonceDTO? createdAnnonce = null;

        try
        {
            var currentUser = await _authService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                AddError("Vous devez être connecté pour créer une annonce");
                HasCreated = false;
                IsLoading = false;
                NotifyStateChanged();
                return;
            }

            NewAnnonce.UtilisateurId = currentUser.UtilisateurId;
            NewAnnonce.DateAnnonce = DateTime.UtcNow;

            if (SelectedFilePreviews.Any(p => p.IsDangerous))
            {
                NewAnnonce.StatutAnnonceId = 3; // Mise en analyse
            }
            else
            {
                NewAnnonce.StatutAnnonceId = 1;
            }

            NewAnnonce.Couleurs = SelectedCouleurIds;
            NewAnnonce.Tags = Tags;

            Console.WriteLine($"📤 Envoi de l'annonce: {NewAnnonce.Titre}");
            Console.WriteLine($"   - Prix: {NewAnnonce.Prix}€");
            Console.WriteLine($"   - Photos: {SelectedFilePreviews.Count}");
            Console.WriteLine($"   - Tags: {string.Join(", ", Tags)}");

            // ✅ TENTATIVE DE CRÉATION - PEUT LEVER UNE EXCEPTION
            try
            {
                await _annonceService.UpdateAnnonce(NewAnnonce.AnnonceId,NewAnnonce);

                if (createdAnnonce == null)
                {
                    AddError("Erreur: L'annonce n'a pas pu être créée");
                    HasCreated = false;
                    return;
                }

                Console.WriteLine($"✅ Annonce créée avec succès: ID={createdAnnonce.AnnonceId}");
            }
            catch (MotInterditException ex)
            {
                // ✅ MOT INTERDIT DÉTECTÉ
                Console.WriteLine($"❌ Mot interdit détecté: {ex.Message}");
                AddError(ex.Message);
                HasCreated = false;
                return; // ✅ STOP ICI - PAS DE REDIRECTION
            }
            catch (BadRequestException ex)
            {
                // ✅ AUTRE ERREUR BADREQUEST
                Console.WriteLine($"❌ BadRequest: {ex.Message}");
                AddError($"Erreur de validation: {ex.Message}");
                HasCreated = false;
                return; // ✅ STOP ICI - PAS DE REDIRECTION
            }

            // ✅ À partir d'ici, l'annonce est créée avec succès

            // Association des couleurs
            if (createdAnnonce != null && createdAnnonce.AnnonceId > 0)
            {
                foreach (var couleurId in SelectedCouleurIds)
                {
                    var couleur = Couleurs.FirstOrDefault(c => c.CouleurId == couleurId);
                    if (couleur != null)
                    {
                        try
                        {
                            var edc = await _couleurService.CouleurToEdc(couleur, createdAnnonce);
                            if (edc != null)
                            {
                                Console.WriteLine($"✅ EstDeCouleur créé: ID={edc.EstDeCouleurId} pour CouleurID={couleur.CouleurId}");
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ CouleurToEdc a retourné null pour CouleurID={couleur.CouleurId}");
                            }
                        }
                        catch (Exception edcEx)
                        {
                            Console.WriteLine($"❌ Erreur création EstDeCouleur pour CouleurID={couleur.CouleurId}: {edcEx.Message}");
                            AddError($"La couleur '{couleur.Nom}' n'a pas pu être associée");
                        }
                    }
                }
            }

            // Upload des photos
            if (SelectedFilePreviews.Count != 0 && createdAnnonce != null)
            {
                IsUploadingPhotos = true;
                NotifyStateChanged();

                Console.WriteLine($"📸 Upload de {SelectedFilePreviews.Count} photos...");

                try
                {
                    var photosDataUrls = SelectedFilePreviews
                        .Select(p => new PhotoDataDTO
                        {
                            PreviewBase64 = p.PreviewBase64,
                            IsDangerous = p.IsDangerous
                        })
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

            // Association des tags
            if (Tags.Count > 0 && createdAnnonce != null)
            {
                IsUpdatingTags = true;
                NotifyStateChanged();
                Console.WriteLine($"🏷️ Ajout de {Tags.Count} tags...");

                try
                {
                    var tagDTOs = new List<TagDTO>();
                    var rejectedTags = new List<string>();

                    foreach (var tag in Tags)
                    {
                        var searchTag = _allTags?.FirstOrDefault(t =>
                            t.LibelleTag.Equals(tag, StringComparison.OrdinalIgnoreCase));

                        if (searchTag != null)
                        {
                            Console.WriteLine($"📌 Tag existant trouvé: {searchTag.LibelleTag}");
                            tagDTOs.Add(searchTag);
                        }
                        else
                        {
                            Console.WriteLine($"🆕 Création nouveau tag: {tag}");
                            var newTag = new CreateTagDTO { Libelle = tag };

                            try
                            {
                                var newTagDTO = await _tagService.AddAsync(newTag);

                                if (newTagDTO != null)
                                {
                                    Console.WriteLine($"✅ Tag créé: ID={newTagDTO.IdTag}, Libelle={newTagDTO.LibelleTag}");
                                    tagDTOs.Add(newTagDTO);
                                }
                                else
                                {
                                    Console.WriteLine($"⚠️ AddAsync a retourné null pour: {tag}");
                                    rejectedTags.Add(tag);
                                }
                            }
                            catch (HttpRequestException httpEx) when (httpEx.Message.Contains("400") || httpEx.Message.Contains("BadRequest"))
                            {
                                Console.WriteLine($"❌ Tag '{tag}' refusé: mot interdit");
                                AddError($"⚠️ Le tag '{tag}' contient un mot interdit et a été rejeté.");
                                rejectedTags.Add(tag);
                            }
                            catch (Exception addTagEx)
                            {
                                Console.WriteLine($"❌ Erreur création tag '{tag}': {addTagEx.Message}");
                                AddError($"Le tag '{tag}' n'a pas pu être créé");
                                rejectedTags.Add(tag);
                            }
                        }
                    }

                    // Retirer les tags rejetés
                    foreach (var rejectedTag in rejectedTags)
                    {
                        Tags.Remove(rejectedTag);
                    }

                    // Associer les tags valides
                    Console.WriteLine($"📎 Création de {tagDTOs.Count} associations Recense...");

                    foreach (var tagDTO in tagDTOs)
                    {
                        if (tagDTO == null) continue;

                        try
                        {
                            Console.WriteLine($"🔄 Association Tag {tagDTO.IdTag} avec Annonce {createdAnnonce.AnnonceId}");

                            var recense = await _tagService.TagToRecense(tagDTO, createdAnnonce);

                            if (recense != null)
                            {
                                Console.WriteLine($"✅ Recense créé: ID={recense.RecenseId}");
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ TagToRecense a retourné null");
                            }
                        }
                        catch (Exception tagEx)
                        {
                            Console.WriteLine($"❌ Erreur association tag '{tagDTO.LibelleTag}': {tagEx.Message}");
                            AddError($"Le tag '{tagDTO.LibelleTag}' n'a pas pu être associé");
                        }
                    }
                }
                finally
                {
                    IsUpdatingTags = false;
                    NotifyStateChanged();
                }
            }

            // ✅ SUCCÈS COMPLET
            HasCreated = true;
            NotifyStateChanged();

            await Task.Delay(2000);
            _nav.NavigateTo($"/profile/{currentUser.Login}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la publication: {ex.Message}");
            AddError($"Erreur inattendue: {ex.Message}");
            HasCreated = false;
        }
        finally
        {
            IsLoading = false;
            IsUploadingPhotos = false;
            IsUpdatingTags = false;
            NotifyStateChanged();
        }
    }

    #endregion

    #region API Messages Management
    public void AddAPIMessage(string message)
    {
        API_Messages.Add(message);
        NotifyStateChanged();
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

public class InMemoryBrowserFile : IBrowserFile
{
    private readonly byte[] _data;

    public InMemoryBrowserFile(byte[] data, string name, string contentType)
    {
        _data = data;
        Name = name;
        ContentType = contentType;
        Size = data.Length;
        LastModified = DateTimeOffset.Now;
    }

    public string Name { get; }
    public long Size { get; }
    public string ContentType { get; }
    public DateTimeOffset LastModified { get; }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
    {
        return new MemoryStream(_data);
    }
}