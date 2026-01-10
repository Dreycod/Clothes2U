using FrontBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class ResetPasswordViewModel
    {
        private readonly PasswordResetWebService _service;
        private readonly NavigationManager _nav;

        public string Token { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public bool IsLoading { get; private set; }
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; set; }

        public ResetPasswordViewModel(
            PasswordResetWebService service,
            NavigationManager nav)
        {
            _service = service;
            _nav = nav;
        }

        public async Task SubmitAsync()
        {
            IsLoading = true;

            try
            {
                var ok = await _service.ResetPasswordAsync(Token, NewPassword);
                IsSuccess = ok;

                if (ok)
                    _nav.NavigateTo("/login");
            }
            catch
            {
                ErrorMessage = "Lien invalide ou expiré.";
            }

            IsLoading = false;
        }
    }
}
