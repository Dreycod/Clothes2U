using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Models.Repository.Managers;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO;

namespace API.Services;

public class ModerationDashboardService : IModerationDashboardService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ISignalementRepository _signalementManager;
    private readonly IDemandeRestaurationRepository<DemandeRestauration, int> _demandeRestaurationManager;
    private readonly IMapper _mapper;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ITicketRepository _ticketManager;

    public ModerationDashboardService(
        IServiceScopeFactory scopeFactory,
        ISignalementRepository signalementManager,
        IMapper mapper,
        IAnnonceRepository<Annonce, int, FilterDTO>  annonceManager,
        ITicketRepository  ticketManager,
        IDemandeRestaurationRepository<DemandeRestauration, int> demandeRestaurationManager
        )
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
        _signalementManager = signalementManager;
        _demandeRestaurationManager = demandeRestaurationManager;
        _annonceManager = annonceManager;
        _ticketManager = ticketManager;
    }
    

    public async Task<DashBoardStatistics> GetDashboardStatistics()
    {
        var signalementsTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ISignalementRepository>();
            return await repo.GetSignalementCount();
        });
        
        var supportsTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            return await repo.GetOpenTicketsCountAsync();
        });

        var analysesTask = Task.Run(async () =>
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IAnnonceRepository<Annonce, int, FilterDTO>>();
            return await repo.GetAnalyseCountAsync();
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


        await Task.WhenAll(signalementsTask, analysesTask, restaurationTask, decisionsStatsTask, supportsTask); 
        
        var decisionsStats = decisionsStatsTask.Result;
        
        return new DashBoardStatistics
        {
            SignalementsEnAttented = await signalementsTask,
            AnalysesEnAttente = await analysesTask,
            ResaurationEnAttente = await restaurationTask,
            DemandeSupport = await supportsTask,
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

    public async Task<List<ActivityDTO>> ListActivity()
    {
        IEnumerable<DemandeRestauration> demandes = await _demandeRestaurationManager.GetAllAsync();
        List<ActivityRestauration> activityRestaurations = _mapper.Map<List<ActivityRestauration>>(demandes);

        IEnumerable<Annonce> annoncesAnalyse = await _annonceManager.GetAllAnalyseAsync();
        List<ActivityDemandeAnalyse> activityDemandeAnalyse = _mapper.Map<List<ActivityDemandeAnalyse>>(annoncesAnalyse);

        IEnumerable<Ticket> tickets = await _ticketManager.GetOpenTicketsAsync();
        List<ActivityTicket> activityTickets = _mapper.Map<List<ActivityTicket>>(tickets);
        
        
        IEnumerable<Signalement> signalements = await _signalementManager.GetAllAsync();
        List<ActivitySignalement> activitySignalements = _mapper.Map<List<ActivitySignalement>>(signalements);
        var activities = activityRestaurations
            .Cast<ActivityDTO>()
            .Concat(activitySignalements)
            .Concat(activityDemandeAnalyse)
            .Concat(activityTickets)
            .OrderByDescending(a => a.Date)
            .Take(5)                     
            .ToList();
        return activities;
    }
}