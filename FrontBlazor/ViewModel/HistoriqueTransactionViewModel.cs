using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Historique;

namespace FrontBlazor.ViewModel
{
    public class HistoriqueTransactionViewModel : BaseViewModel
    {
        private readonly TransactionService _service;

        public HistoriqueTransactionViewModel(
            TransactionService service,
            IAuthService authService,
            NavigationManager navigationManager)
            : base(authService, navigationManager)
        {
            _service = service;
        }

        public bool IsLoading { get; private set; }
        public List<TransactionHistoriqueDTO> Achats { get; private set; } = new();
        public List<TransactionHistoriqueDTO> Ventes { get; private set; } = new();

        public async Task LoadAsync()
        {
            IsLoading = true;
            NotifyStateChanged();

            var user = await _authService.GetCurrentUserAsync();
            if (user == null)
            {
                IsLoading = false;
                NotifyStateChanged();
                return;
            }

            Achats = await _service.GetHistoriqueAcheteurAsync(user.UtilisateurId);
            Ventes = await _service.GetHistoriqueVendeurAsync(user.UtilisateurId);

            IsLoading = false;
            NotifyStateChanged();
        }

    }
}
