using Shared.DTO;

namespace FrontBlazor.Services.Interfaces;

public interface IModerationDashboardService
{
    Task<DashBoardStatistics> GetStatistics();
    Task<List<ActivityDTO>> GetActivity();
}