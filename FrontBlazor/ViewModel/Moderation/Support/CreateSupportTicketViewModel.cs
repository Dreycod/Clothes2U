using FrontBlazor.Services.WebService;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel.Moderation.Support
{
    public class CreateSupportTicketViewModel
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
            NavigationManager nav)
        {
            _service = service;
            _nav = nav;
        }

        public async Task SubmitAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                await _service.CreateTicketAsync(new()
                {
                    Subject = Subject,
                    Message = Message
                });

                IsSuccess = true;
                _nav.NavigateTo("/");
            }
            catch
            {
                ErrorMessage = "Impossible d'envoyer la demande de support.";
            }

            IsLoading = false;
        }
    }
}
