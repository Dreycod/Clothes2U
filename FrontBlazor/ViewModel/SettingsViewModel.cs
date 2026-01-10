using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared.DTO.Annonce;
using Shared.DTO.Bloque;
using Shared.DTO.LoginRegister;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel
{
    public class SettingsViewModel : ClientBaseViewModel
    {
        
        private readonly NavigationManager _navigationManager;
        private readonly IUtilisateurService _utilisateurService;
        private readonly IAuthService _authService;
        private readonly IMediasService _mediaService;
        private readonly IBloqueService _bloqueService;
        public string ActiveTab { get; set; } = "account";

        #region Variables
        public UtilisateurSettingsDTO? CurrentUser { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsUploadingPhoto { get; set; }
        public byte[]? UploadedPhotoBytes { get; set; }
        public string? UploadedPhotoFileName { get; set; }
        public string? PreviewPhotoUrl { get; set; }

        public bool IsUpdatingUsername { get; set; }
        public bool IsUpdatingEmail { get; set; }
        public string UsernameError { get; set; }
        public bool UsernameUpdateSuccess { get; set; }
        public string EmailError { get; set; }
        public bool EmailUpdateSuccess { get; set; }

        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsUpdatingPassword { get; set; }
        public string PasswordError { get; set; }
        public bool PasswordUpdateSuccess { get; set; }

        public bool EmailNotificationsEnabled { get; set; }
        public bool IsUpdatingNotifications { get; set; }
        public string NotificationError { get; set; }
        public bool NotificationUpdateSuccess { get; set; }

        public List<BloqueDetailDTO> UtilisateursBloques { get; set; } = new List<BloqueDetailDTO>();
        public bool IsLoadingBlocked { get; set; }
        public bool IsUnblocking { get; set; }

        public byte[]? ImgBytes { get; set; }
        #endregion


        public SettingsViewModel(
            NavigationManager navigationManager,
            IUtilisateurService utilisateurService,
            IAuthService authService,
            IMediasService mediaService,
            INotificationService notificationService,
            IBloqueService bloqueService)
            : base(navigationManager, authService, notificationService)
        {
            _navigationManager = navigationManager;
            _utilisateurService = utilisateurService;
            _authService = authService;
            _mediaService = mediaService;
            _bloqueService = bloqueService;

        }

        public async Task LoadSettings()
        {
            await base.LoadAsync();
            if (utilisateur == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }
            try
            {
                Console.WriteLine(IsLoggedIn);
                if (IsLoggedIn)
                {
                    CurrentUser = await _utilisateurService.GetUserSettingsById(utilisateur.UtilisateurId);
                }
                else
                {
                    _navigationManager.NavigateTo("/login");
                    return;
                }
                Username = CurrentUser.Login;
                Email = CurrentUser.Email;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                NotifyStateChanged();
            }
        }

        public async Task SetActiveTab(string tab)
        {
            ActiveTab = tab;

            if (tab == "blocked" && !UtilisateursBloques.Any())
            {
                Console.WriteLine("blockeds");
                await LoadBlockedUsers();
            }

            NotifyStateChanged();
        }

        public void TriggerFileInput()
        {

        }
        public async Task HandlePhotoUpload(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                UploadedPhotoFileName = file.Name;

                using var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                UploadedPhotoBytes = ms.ToArray();

                var base64 = Convert.ToBase64String(UploadedPhotoBytes);
                PreviewPhotoUrl = $"data:image/{GetImageFormat(UploadedPhotoFileName)};base64,{base64}";

                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la prévisualisation: {ex.Message}");
            }
        }

        public async Task ConfirmPhotoUpload()
        {
            if (UploadedPhotoBytes == null) return;

            IsUploadingPhoto = true;
            NotifyStateChanged();

            try
            {
                bool success = await _mediaService.UploadPhotoCompteAsync(
                    (int)CurrentUser.UtilisateurId,
                    UploadedPhotoBytes,
                    UploadedPhotoFileName
                );

                if (success)
                {
                    UploadedPhotoBytes = null;
                    UploadedPhotoFileName = null;
                    PreviewPhotoUrl = null;
                    NotifyStateChanged();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'upload: {ex.Message}");
            }
            finally
            {
                IsUploadingPhoto = false;
                NotifyStateChanged();
            }
        }

        public void CancelPhotoUpload()
        {
            UploadedPhotoBytes = null;
            UploadedPhotoFileName = null;
            PreviewPhotoUrl = null;
            NotifyStateChanged();
        }

        private string GetImageFormat(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" => "jpeg",
                ".png" => "png",
                ".gif" => "gif",
                ".webp" => "webp",
                _ => "jpeg"
            };
        }

        public async Task UpdateUsername()
        {
            UsernameError = string.Empty;

            if (string.IsNullOrWhiteSpace(Username))
            {
                UsernameError = "Le nom d'utilisateur ne peut pas être vide";
                NotifyStateChanged();
                return;
            }

            if (Username.Length < 3)
            {
                UsernameError = "Le nom d'utilisateur doit contenir au moins 3 caractères";
                NotifyStateChanged();
                return;
            }

            if (Username == CurrentUser.Login)
            {
                UsernameError = "Veuillez entrer un nouveau nom d'utilisateur";
                return;
            }

            IsUpdatingUsername = true;
            NotifyStateChanged();

            try
            {
                UtilisateurSettingsDTO utilisateurSettingsDTO = new UtilisateurSettingsDTO
                {
                    Login = Username
                };

                bool success = await _utilisateurService.PostUpdateUser(CurrentUser.UtilisateurId, utilisateurSettingsDTO);

                if (success)
                {
                    CurrentUser.Login = Username;
                    UsernameUpdateSuccess = true;
                }
                else
                {
                    UsernameError = "Erreur lors de la mise à jour";
                }
            }
            catch (Exception ex)
            {
                UsernameError = $"Erreur: {ex.Message}";
            }
            finally
            {
                IsUpdatingUsername = false;
                NotifyStateChanged();
            }
        }

        public async Task UpdateEmail()
        {
            EmailError = string.Empty;
            EmailUpdateSuccess = false;

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "L'email ne peut pas être vide";
                NotifyStateChanged();
                return;
            }

            if (!IsValidEmail(Email))
            {
                EmailError = "Veuillez entrer une adresse email valide";
                NotifyStateChanged();
                return;
            }

            if (Email == CurrentUser.Email)
            {
                EmailError = "Veuillez entrer un nouvel email";
                NotifyStateChanged();
                return;
            }

            IsUpdatingEmail = true;
            NotifyStateChanged();

            try
            {
                UtilisateurSettingsDTO utilisateurSettingsDTO = new UtilisateurSettingsDTO
                {
                    Email = Email
                };

                bool success = await _utilisateurService.PostUpdateUser(CurrentUser.UtilisateurId, utilisateurSettingsDTO);

                if (success)
                {
                    CurrentUser.Email = Email;
                    EmailUpdateSuccess = true;
                }
                else
                {
                    EmailError = "Erreur lors de la mise à jour";
                }
            }
            catch (Exception ex)
            {
                EmailError = $"Erreur: {ex.Message}";
            }
            finally
            {
                IsUpdatingEmail = false;
                NotifyStateChanged();
            }
        }

        public async Task UpdatePassword()
        {
            PasswordError = string.Empty;
            PasswordUpdateSuccess = false;

            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                PasswordError = "Veuillez entrer votre mot de passe actuel";
                NotifyStateChanged();
                return;
            }

            if (string.IsNullOrWhiteSpace(NewPassword))
            {
                PasswordError = "Veuillez entrer un nouveau mot de passe";
                NotifyStateChanged();
                return;
            }

            //if (NewPassword.Length < 6)
            //{
            //    PasswordError = "Le mot de passe doit contenir au moins 6 caractères";
            //    NotifyStateChanged();
            //    return;
            //}

            if (NewPassword != ConfirmPassword)
            {
                PasswordError = "Les mots de passe ne correspondent pas";
                NotifyStateChanged();
                return;
            }

            if (CurrentPassword == NewPassword)
            {
                PasswordError = "Le nouveau mot de passe doit être différent de l'ancien";
                NotifyStateChanged();
                return;
            }

            IsUpdatingPassword = true;
            NotifyStateChanged();

            try
            {
                ChangePasswordDTO password = new ChangePasswordDTO();
                password.Password = CurrentPassword;
                password.NewPassword = NewPassword;
                password.ConfirmNewPassword = ConfirmPassword;
                var response = await _authService.ModificationMotDePasse(password);

                if (!response.Success)
                {
                    PasswordError = response.ErrorMessage ?? "Erreur lors de la mise à jour";
                }
                else
                {
                    PasswordUpdateSuccess = true;
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                }
            }
            catch (Exception ex)
            {
                PasswordError = $"Erreur: {ex.Message}";
            }
            finally
            {
                IsUpdatingPassword = false;
                NotifyStateChanged();
            }
        }

        public async Task ToggleEmailNotifications()
        {
            //
        }

        private async Task LoadBlockedUsers()
        {
            IsLoadingBlocked = true;
            NotifyStateChanged();

            try
            {
                var result = await _bloqueService.GetUsersBloquee(CurrentUser.UtilisateurId);
                UtilisateursBloques = result ?? new List<BloqueDetailDTO>();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading blocked users: {ex.Message}");
                UtilisateursBloques = new List<BloqueDetailDTO>();
            }
            finally
            {
                IsLoadingBlocked = false;
                NotifyStateChanged();
            }
        }

        public async Task UnblockUser(int userId)
        {
            IsUnblocking = true;
            NotifyStateChanged();

            try
            {
                await _bloqueService.DeleteAsync(userId);
                UtilisateursBloques.Remove(UtilisateursBloques.FirstOrDefault(u => u.UtilisateurBloqueId == userId)!);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unblocking user: {ex.Message}");
            }
            finally
            {
                IsUnblocking = false;
                NotifyStateChanged();
            }
        }

        public string GetPhotoUrl(int? photoId)
        {
            if (photoId == null || photoId == 0)
            {
                return "";
            }

            return _mediaService.GetPhotoUrl((int)photoId);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}