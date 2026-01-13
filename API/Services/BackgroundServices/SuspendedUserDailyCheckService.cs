using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services.Interfaces;
using Shared.Enums;

namespace API.Services.BackgroundServices;

public class SuspendedUserDailyCheckService : ISuspendedUserDailyCheckService
{
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IDecisionRepository _decisionManager;

    public SuspendedUserDailyCheckService(
        IUtilisateurRepository utilisateurManager,
        IDecisionRepository decisionManager)
    {
        _utilisateurManager = utilisateurManager;
        _decisionManager = decisionManager;
    }

    public async Task CheckSuspensions()
    {
        Console.WriteLine("TRAITEMENT DES SUSPENSIONS ----------------------------------------------------------------------------------");
        List<Decision> decisions = await _decisionManager.GetCurrentDecisionSuspensions();
        foreach (Decision decision in decisions)
        {
            Console.WriteLine(decision.DecisionId);
            if (DateTime.UtcNow >= decision.DecisionSanction.SanctionSuspension.DateFinSuspension)
            {
                Console.WriteLine("Restauration du compte : " + decision.UtilisateurId);
                Utilisateur user = decision.Utilisateur;
                user.StatutId = (int)UtilisateurStatut.Actif;
                await  _utilisateurManager.UpdateAsync(user);
                decision.DecisionSanction.EstEnCours = false;
                await  _decisionManager.UpdateAsync(decision);
            }
        }
    }
}