using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class CategorieViewModel
    {
        private readonly ICategorieService<Categorie> _categorieService;
        public List<Categorie> Categories { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public CategorieViewModel(ICategorieService<Categorie> categorieService)
        {
            _categorieService = categorieService;
        }

        public async Task LoadCategoriesAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var result = await _categorieService.GetAllCategories();

                if (result != null)
                {
                    Categories = result;
                }
                else
                {
                    Categories = new List<Categorie>();
                    ErrorMessage = "Impossible de charger les catégories";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement des catégories";
                Categories = new List<Categorie>();
                Console.WriteLine($"LoadCategoriesAsync Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public Categorie? GetCategoryById(int categoryId)
        {
            return Categories.FirstOrDefault(c => c.IdCategorie == categoryId);
        }

        public SousCategorie? GetSubcategoryById(int subcategoryId)
        {
            foreach (var category in Categories)
            {
                var subcategory = category.SousCategories?.FirstOrDefault(sc => sc.SousCategorieId == subcategoryId);
                if (subcategory != null)
                {
                    return subcategory;
                }
            }
            return null;
        }
    }
}