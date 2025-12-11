namespace API.Services.VerificationSrvceV2
{
    public interface ISmsService
    {
        Task SendAsync(string phoneNumber, string message);
    }
}
