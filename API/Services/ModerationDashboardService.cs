using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
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

        var decisionsStatsTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IDecisionRepository>();
    
            var now = DateTime.UtcNow;
            var date24h = now.AddDays(-1);
            var dateWeek = now.AddDays(-7);
            var dateMonth = now.AddMonths(-1);

            var decisions24h = await repo.GetDecisionsCountFrom(date24h);
            var decisionsWeek = await repo.GetDecisionsCountFrom(dateWeek);
            var decisionsMonth = await repo.GetDecisionsCountFrom(dateMonth);

            var suspensions24h = await repo.GetSuspensionsCountFrom(date24h);
            var suspensionsWeek = await repo.GetSuspensionsCountFrom(dateWeek);
            var suspensionsMonth = await repo.GetSuspensionsCountFrom(dateMonth);

            var bannissements24h = await repo.GetBannissementsCountFrom(date24h);
            var bannissementsWeek = await repo.GetBannissementsCountFrom(dateWeek);
            var bannissementsMonth = await repo.GetBannissementsCountFrom(dateMonth);

            return new
            {
                Decisions = new
                {
                    Count24h = decisions24h,
                    CountWeek = decisionsWeek,
                    CountMonth = decisionsMonth
                },
                Suspensions = new
                {
                    Count24h = suspensions24h,
                    CountWeek = suspensionsWeek,
                    CountMonth = suspensionsMonth
                },
                Bannissements = new
                {
                    Count24h = bannissements24h,
                    CountWeek = bannissementsWeek,
                    CountMonth = bannissementsMonth
                }
            };
        });


        await Task.WhenAll(signalementsTask, suspendTask, restaurationTask, decisionsStatsTask);
        
        var decisionsStats = decisionsStatsTask.Result;
        
        return new DashBoardStatistics
        {
            SignalementsEnAttented = await signalementsTask,
            CompteSuspendus = await suspendTask,
            ResaurationEnAttente = await restaurationTask,
            DecisionsAujourdhui = new DecisionStatistics
            {
                SignalementsTraites = decisionsStatsTask.Result.Decisions.Count24h,
                ComptesBannis = decisionsStatsTask.Result.Bannissements.Count24h,
                ComptesSuspendus = decisionsStatsTask.Result.Suspensions.Count24h
            },
            DecisionsSemaine = new DecisionStatistics
            {
                SignalementsTraites = decisionsStatsTask.Result.Decisions.CountWeek,
                ComptesBannis = decisionsStatsTask.Result.Bannissements.CountWeek,
                ComptesSuspendus = decisionsStatsTask.Result.Suspensions.CountWeek
            },
            DecisionsMois = new DecisionStatistics
            {
                SignalementsTraites = decisionsStatsTask.Result.Decisions.CountMonth,
                ComptesBannis = decisionsStatsTask.Result.Bannissements.CountMonth,
                ComptesSuspendus = decisionsStatsTask.Result.Suspensions.CountMonth
            },
        };
    }
}