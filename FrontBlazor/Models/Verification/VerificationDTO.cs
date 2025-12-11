namespace FrontBlazor.Models.Verification
{
    public enum VerificationType
    {
        Email = 1,
        Telephone = 2
    }

    public class SendVerificationCodeRequest
    {
        public VerificationType Type { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class VerifyCodeRequest
    {
        public string Code { get; set; } = null!;
        public VerificationType Type { get; set; }
    }

    public class VerificationResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? ExpiresAt { get; set; }
    }
}
