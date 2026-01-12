using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel.Moderation.Support
{
    public class CreateSupportTicketViewModel : ClientBaseViewModel
    {
        private readonly SupportWebService _service;
        private readonly NavigationManager _nav;

        public string Subject { get; set; } = "";
        public string Message { get; set; } = "";

        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public bool IsSuccess { get; private set; }

        public CreateSupportTicketViewModel(
            SupportWebService service,
            NavigationManager navigationManager,
            IAuthService authService,
            ISignalRService notificationHubService,
            INotificationService notificationService
            )
            : base(navigationManager, authService, notificationHubService, notificationService)
        {
            _service = service;
            _nav = navigationManager;
        }

        public async Task SubmitAsync()
        {
            IsLoading = true;
            ErrorMessage = null;
            var response = await _service.CreateTicketAsync(new()
            {
                Subject = Subject,
                Message = Message
            });
            if (response.Success)
            {
                IsSuccess = true;
                _nav.NavigateTo("/");
            }
            else
            {
                ErrorMessage = response.ErrorMessage;
            }
            IsLoading = false;
        }
    }
}
