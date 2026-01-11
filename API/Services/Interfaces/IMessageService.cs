namespace API.Services.Interfaces;

public interface IMessageService
{
    Task SendMessageCount(int userId);
}