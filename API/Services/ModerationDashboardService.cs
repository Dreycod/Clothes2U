using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO;

namespace API.Services;

public class ModerationDashboardService : IModerationDashboardService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ModerationDashboardService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<DashBoardStatistics> GetDashboardStatistics()
    {
        // ✅ Chaque tâche a son propre scope = son propre DbContext
        var signalementsTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISignalementRepository>();
            return await repo.GetSignalementCount();
        });

        var suspendTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IUtilisateurRepository>();
            return await repo.GetSuspendUserCount();
        });

        var restaurationTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDemandeRestaurationRepository<DemandeRestauration, int>>();
            return await repo.GetDemandeRestaurationCount();
        });

        // ✅ Attendre toutes les tâches en parallèle
        await Task.WhenAll(signalementsTask, suspendTask, restaurationTask);

        return new DashBoardStatistics
        {
            SignalementsEnAttented = await signalementsTask,
            CompteSuspendus = await suspendTask,
            ResaurationEnAttente = await restaurationTask
        };
    }
}