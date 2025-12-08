using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class SearchAnnonceViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        FilterDTO filterRequest = new FilterDTO();
        public string? ErrorMessage { get; set; }

        public event Action? OnStateChange;

        #region ViewModels
        public ListableViewModel<Categorie> VM_Categorie { get; set; }
        public ListableViewModel<Marque> VM_Marque { get; set; }
        public ListableViewModel<Taille> VM_Taille { get; set; }
        public ListableViewModel<EtatArticle> VM_Etats { get; set; }
        public NavigationManager navigationManager { get; set; }
        public LoginViewModel VM_Login { get; set; }


        #endregion

        #region Attributes

        public bool IsLoading = false;
        public string? Query { get; set; }
        #region Filtre
        public HashSet<int> selectedCategories = new();
        public HashSet<int> selectedSubcategories = new();
        public HashSet<int> expandedCategories = new();
        public HashSet<int> selectedMarques = new();
        public HashSet<int> selectedEtats = new();
        public int? selectedTailleId = null;

        public int SliderMax = 500;
        public int SelectedPrice = 250;

        public bool conditionNew = false;
        public bool conditionVeryGood = false;
        public bool conditionGood = false;
        #endregion

        #region Pagination
        public int CurrentPage { get; set; } = 1;
        private int ItemsPerPage { get; set; } = 30;

        public int TotalItems = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        #endregion

        #endregion
        public SearchAnnonceViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, ListableViewModel<Categorie> vM_Categorie, ListableViewModel<Marque> vM_Marque, ListableViewModel<Taille> vM_Taille, ListableViewModel<EtatArticle> vM_Etats, NavigationManager navManager)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            VM_Categorie = vM_Categorie;
            VM_Marque = vM_Marque;
            VM_Taille = vM_Taille;
            VM_Etats = vM_Etats;
            navigationManager = navManager;
        }

        public async Task LoadAsync()
        {
            IsLoading = true;
            await VM_Categorie.LoadAsync();
            await VM_Marque.LoadAsync();
            await VM_Taille.LoadAsync();
            await VM_Etats.LoadAsync();
            // print loadasync de etats
            Console.WriteLine("Etats Loaded: " + VM_Etats.Items.Count);
            await ApplyFilters();
            IsLoading = false;
        }

        #region Annonces
        public async Task CountTotalItems()
        {
            var result = await GetAnnoncesByFiltreAsync(filterRequest, 1, 100000);
            if (result == null)
            {
                TotalItems = 0;
                return;
            }
            TotalItems = result.Count;
            Console.WriteLine($"Total Items Pour l'annonce: " + TotalItems.ToString());
        }

        public async Task LoadAnnonces()
        {
            await CountTotalItems();

            List<Annonce> result = await GetAnnoncesByFiltreAsync(filterRequest, CurrentPage, ItemsPerPage);
            if (result == null)
            {
                Annonces = new List<Annonce>();
                return;
            }
            Annonces = result;
        }
        public async Task<List<Annonce>?> GetAnnoncesByFiltreAsync(FilterDTO filterDto, int page, int pageSize)
        {
            try
            {
                return await _annonceService.GetAnnonceByFilter(filterDto, page, pageSize);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du filtrage des annonces";
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        //private void CalculateMaxPrice()
        //{
        //    if (Annonces != null && Annonces.Any())
        //    {
        //        var highestPrice = Annonces.Max(a => a.Prix);
        //        SliderMax = (int)Math.Ceiling(highestPrice);
        //        if (SelectedPrice == 0)
        //        {
        //            SelectedPrice = SliderMax/2;
        //        }
        //    }
        //}
        
        public async Task ToggleFavorite(int annonceId)
        {
            if (VM_Login.CheckLoginStatus == null)
            {
                navigationManager.NavigateTo("/login");
                return;
            }

            Annonce annonce = Annonces.First(a => a.AnnonceId == annonceId);
            bool isFavorite = annonce.IsLikedByCurrentUser;
            annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;

            if (!isFavorite)
                await _favorisService.AddFavoris(annonceId);
            else
                await _favorisService.DeleteFavoris(annonceId);

            OnStateChange?.Invoke();
        }
        #endregion

        #region ToggleCategories
        public async void ToggleCategory(int categoryId)
        {
            if (expandedCategories.Contains(categoryId))
            {
                expandedCategories.Remove(categoryId);
            }
            else
            {
                expandedCategories.Add(categoryId);
            }
            await ApplyFilters();
        }

        public async void ToggleCategorySelection(int categoryId, bool isChecked)
        {
            if (isChecked)
            {
                selectedCategories.Add(categoryId);
            }
            else
            {
                selectedCategories.Remove(categoryId);
            }
            await ApplyFilters();
        }

        public void ToggleSubcategorySelection(int subcategoryId, bool isChecked)
        {
            if (isChecked)
            {
                selectedSubcategories.Add(subcategoryId);
            }
            else
            {
                selectedSubcategories.Remove(subcategoryId);
            }
            OnStateChange?.Invoke();
        }

        public bool IsCategorySelected(int categoryId)
        {
            return selectedCategories.Contains(categoryId);
        }

        public bool IsSubcategorySelected(int subcategoryId)
        {
            return selectedSubcategories.Contains(subcategoryId);
        }
        #endregion

        #region ToggleMarques & Tailles
        public async void ToggleMarqueSelection(int marqueId, bool isChecked)
        {
            if (isChecked)
            {
                selectedMarques.Add(marqueId);
            }
            else
            {
                selectedMarques.Remove(marqueId);
            }
            await ApplyFilters();
        }

        public bool IsMarqueSelected(int marqueId)
        {
            return selectedMarques.Contains(marqueId);
        }
        public async void ToggleTailleSelection(int tailleId)
        {
            if (selectedTailleId == tailleId)
            {
                selectedTailleId = null;
            }
            else
            {
                selectedTailleId = tailleId;
            }
            await ApplyFilters();
        }

        public async void ToggleEtatSelection(int etatId, bool isChecked)
        {
            if (isChecked)
            {
                selectedEtats.Add(etatId);
            }
            else
            {
                selectedEtats.Remove(etatId);
            }
            await ApplyFilters();
        }

        public bool IsEtatSelected(int etatId)
        {
            return selectedEtats.Contains(etatId);
        }
        #endregion



        #region Filtrage
        public async void ResetFilters()
        {
            selectedCategories.Clear();
            selectedSubcategories.Clear();
            expandedCategories.Clear();
            selectedMarques.Clear();
            selectedTailleId = null;
            CurrentPage = 1;
            conditionNew = conditionVeryGood = conditionGood = false;

            await ApplyFilters();
        }

        public async Task ApplyFilters()
        {
            var selectedCategoryNames = VM_Categorie.Items
                .Where(c => selectedCategories.Contains(c.IdCategorie))
                .Select(c => c.LibelleCategorie)
                .ToList();

            var selectedSubcategoryNames = VM_Categorie.Items
                .SelectMany(c => c.SousCategories)
                .Where(sc => selectedSubcategories.Contains(sc.SousCategorieId))
                .Select(sc => sc.LibelleSousCategorie!)
                .ToList();

            var selectedMarqueNames = VM_Marque.Items
                .Where(m => selectedMarques.Contains(m.MarqueId))
                .Select(m => m.NomMarque)
                .ToList();

            var selectedTailleName = VM_Taille.Items
                .Where(t => selectedTailleId == t.TailleId)
                .Select(t => t.Libelletaille)
                .FirstOrDefault() ?? "";

            var selectedEtatsName = VM_Etats.Items
               .Where(sc => selectedEtats.Contains(sc.EtatArticleId))
                .Select(sc => sc.NomEtat!)
                .ToList();

            // CONSOLE OUTPUT 
            Console.WriteLine($"Selected Categories: {string.Join(", ", selectedCategoryNames)}");
            Console.WriteLine($"Selected Subcategories: {string.Join(", ", selectedSubcategoryNames)}");
            Console.WriteLine($"Selected Marques: {string.Join(", ", selectedMarqueNames)}");
            Console.WriteLine($"Selected Taille: {selectedTailleName}");
            Console.WriteLine($"Selected Etats: {selectedEtatsName}");
            Console.WriteLine($"Price Max: {SelectedPrice}");

            //  DTO
            filterRequest = new FilterDTO
            {
                MotCle = Query,
                Categories = selectedCategoryNames,
                SousCategories = selectedSubcategoryNames,
                Marques = selectedMarqueNames,
                Etats = selectedEtatsName,
                Tailles = string.IsNullOrEmpty(selectedTailleName) ? new List<string>() : new List<string> { selectedTailleName },
                PrixMax = SelectedPrice,
                PrixMin = 0
            };

            Console.WriteLine(filterRequest.MotCle+" Is the mot clé");    

            CurrentPage = 1;
            await LoadAnnonces();
            OnStateChange?.Invoke();
        }
        #endregion

        #region Pagination
        public async void GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;

            await LoadAnnonces();
            OnStateChange?.Invoke();
        }

        public List<int> GetVisiblePages()
        {
            var pages = new List<int>();
            var maxVisible = 7; 

            if (TotalPages <= maxVisible)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
            }
            else
            {
                pages.Add(1);

                if (CurrentPage > 3)
                {
                    pages.Add(-1);
                }

                int start = Math.Max(2, CurrentPage - 1);
                int end = Math.Min(TotalPages - 1, CurrentPage + 1);

                for (int i = start; i <= end; i++)
                {
                    pages.Add(i);
                }

                if (CurrentPage < TotalPages - 2)
                {
                    pages.Add(-1); 
                }

                pages.Add(TotalPages);
            }

            return pages;
        }

        public int GetStartItem()
        {
            return (CurrentPage - 1) * ItemsPerPage + 1;
        }

        public int GetEndItem()
        {
            return Math.Min(CurrentPage * ItemsPerPage, TotalItems);
        }
        #endregion
        public void NavigateToProductDetail(int? productId)
        {
            if (productId.HasValue)
            {
                navigationManager.NavigateTo($"/product/{productId}");
            }
        }
    }
}