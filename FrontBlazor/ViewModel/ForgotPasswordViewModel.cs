using FrontBlazor.Services;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class ForgotPasswordViewModel
    {
        private readonly PasswordResetWebService _service;
        private readonly NavigationManager _nav;

        public string Email { get; set; } = "";
        public bool IsLoading { get; private set; }
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? SuccessMessage { get; private set; }

        public ForgotPasswordViewModel(
            PasswordResetWebService service,
            NavigationManager nav)
        {
            _service = service;
            _nav = nav;
        }

        public async Task SubmitAsync()
        {
            ErrorMessage = null;
            SuccessMessage = null;

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Veuillez entrer votre adresse email";
                return;
            }

            IsLoading = true;

            try
            {
                await _service.ForgotPasswordAsync(Email);

                // Toujours succès côté UX (sécurité)
                SuccessMessage =
                    "Si un compte existe avec cette adresse, un email a été envoyé.";

                Email = string.Empty;
            }
            catch
            {
                ErrorMessage = "Une erreur est survenue. Veuillez réessayer.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
