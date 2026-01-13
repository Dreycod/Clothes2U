namespace API.Services.Interfaces;

public interface ISuspendedUserDailyCheckService
{
    Task CheckSuspensions();
}