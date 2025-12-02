using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class ListableViewModel<T> where T : class
    {
        private readonly IListableService<T> _listableService;

        public List<T> Items { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public ListableViewModel(IListableService<T> listableService)
        {
            _listableService = listableService;
        }

        public async Task LoadAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                Items = await _listableService.GetAllAsync() ?? new List<T>();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Impossible de charger les {typeof(T).Name}s";
            }
            finally
            {
                IsLoading = false;
            }
        }

    }
}
