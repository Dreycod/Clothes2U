using API.Models.EntityFramework;

namespace API.DTO.Verification;

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