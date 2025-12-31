using Shared.DTO;

namespace API.Services;

public interface IModerationDashboardService
{
    Task<DashBoardStatistics>  GetDashboardStatistics();
}