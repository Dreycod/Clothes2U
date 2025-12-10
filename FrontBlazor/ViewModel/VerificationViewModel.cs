using FrontBlazor.Models.Verification;
using FrontBlazor.Services;

namespace FrontBlazor.ViewModel
{
    public class VerificationViewModel
    {
        private readonly VerificationService _service;

        public VerificationViewModel(VerificationService service)
        {
            _service = service;
        }

        public VerificationType SelectedType { get; set; } = VerificationType.Email;
        public string Code { get; set; } = string.Empty;

        public string Message { get; private set; } = string.Empty;
        public DateTime? ExpiresAt { get; private set; }
        public bool CodeSent { get; private set; }
        public string PhoneNumber { get; set; } = string.Empty;


        public async Task SendCodeAsync()
        {
            var result = await _service.SendCodeAsync(
                SelectedType,
                SelectedType == VerificationType.Telephone ? PhoneNumber : null
            );

            if (result == null) return;

            Message = result.Message;
            ExpiresAt = result.ExpiresAt;
            CodeSent = result.Success;
        }


        public async Task VerifyAsync()
        {
            var result = await _service.VerifyCodeAsync(Code, SelectedType);
            if (result == null) return;

            Message = result.Message;
        }
    }
}
