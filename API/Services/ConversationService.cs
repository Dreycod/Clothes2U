using API.DTO;
using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public class ConversationService : IConversationService
{
    private readonly IAnnonceRepository<Annonce,int, FilterDTO> _annonceManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    
    public ConversationService(IAnnonceRepository<Annonce,int, FilterDTO> annonceManager, IConversationRepository<Conversation, int> conversationManager)
    {
        _annonceManager = annonceManager;
        _conversationManager = conversationManager;
    }
    
    public async Task<Conversation> GetOrCreateConversation(int annonceId, int userId)
    {
        if (userId == null)
            throw new UnauthorizedAccessException();

        // 1. Vérifier si la conversation existe
        var existing = await _conversationManager.GetByUserAndAnnonceAsync(userId, annonceId);
        if (existing != null) return existing;
        var annonce = await _annonceManager.GetByIdAsync(annonceId);

        // 2. Créer une nouvelle conversation
        var conversation = new Conversation
        {
            StatutConversationId = 1,
            CreationDate = DateTime.UtcNow,
            Acheteur = new Achete
            {
                UtilisateurAcheteurId = userId,
            },
            Vendeur = new Vend
            {
                UtilisateurVendeurId = annonce.UtilisateurId
            },
            AnnonceId = annonceId,
        };

        await _conversationManager.AddAsync(conversation);
        return conversation;
    }
}