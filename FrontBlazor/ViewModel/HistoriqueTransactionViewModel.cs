using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Historique;

namespace FrontBlazor.ViewModel
{
    public class HistoriqueTransactionViewModel : ClientBaseViewModel
    {
        private readonly TransactionService _service;

        public HistoriqueTransactionViewModel(
            TransactionService service,
            IAuthService authService,
            NavigationManager navigationManager,
            INotificationService notificationService)
            : base(navigationManager, authService, notificationService)
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
            await base.LoadAsync();
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
