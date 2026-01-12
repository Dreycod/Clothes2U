using Shared.DTO.Conversation;
using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.Message;
using System.Collections.ObjectModel;

namespace API.Mapper;

public class ConversationMappingProfile : Profile
{
    public ConversationMappingProfile()
    {
        CreateMap<Message, MessageSignalementDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.content, opt => opt.MapFrom(src => src.MessageTexte.Content))
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => 
                src.MessageTexte.Photos != null 
                    ? src.MessageTexte.Photos.Select(p => p.PhotoId).ToList() 
                    : new List<int>()));
            
        CreateMap<Conversation, ConversationDTO>()
            .ForMember(dest => dest.ConversationId, opt => opt.MapFrom(src => src.ConversationId))
            .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src =>
                    src.Messages.OrderByDescending(m => m.MessageDate)
                        .FirstOrDefault().MessageTexte.Content ?? string.Empty))
            .ForMember(dest => dest.LastMessageDate, opt => opt.MapFrom(src =>
                src.Messages != null && src.Messages.Any()
                    ? src.Messages.OrderByDescending(m => m.MessageDate)
                        .First().MessageDate
                    : src.CreationDate))
            .ForMember(dest => dest.Interlocuteur,
                opt => opt.MapFrom((src, dest, _, context) =>
                    (int)context.Items["CurrentUserId"] == src.Acheteur.UtilisateurAcheteurId
                        ? src.Vendeur?.UtilisateurVendeur?.Login
                        : src.Acheteur?.UtilisateurAcheteur?.Login
                ))
            .ForMember(dest => dest.PhotoInterlocuteurId,
                opt => opt.MapFrom((src, dest, _, context) =>
                    (int)context.Items["CurrentUserId"] == src.Acheteur.UtilisateurAcheteurId
                        ? src.Vendeur?.UtilisateurVendeur?.PhotoProfil?.PhotoId ?? 0
                        : src.Acheteur?.UtilisateurAcheteur?.PhotoProfil?.PhotoId ?? 0
                ))
            .ForMember(dest => dest.TitreAnnonce, opt => opt.MapFrom(src => src.LAnnonce.Title))
            .ForMember(dest => dest.Prix, opt => opt.MapFrom(src => src.Prix != null ? src.Prix : src.LAnnonce.Prix))
            .ForMember(dest => dest.PrixAnnonce, opt => opt.MapFrom(src => src.LAnnonce.Prix))
            .ForMember(dest => dest.Negociable, opt => opt.MapFrom(src => src.LAnnonce.Negociable))
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.LAnnonce.AnnonceId))
            .ForMember(dest => dest.VendeurId, opt => opt.MapFrom(src => src.Vendeur.UtilisateurVendeurId))
            .ForMember(dest => dest.StatusConversationId, opt => opt.MapFrom(src => src.StatutConversationId))
            .ForMember(dets => dets.StatusConversation, opt => opt.MapFrom(src => src.StatutConversation.StatutConversationLibelle))
            .ForMember(dest => dest.PhotoAnnonceId, opt => opt.MapFrom(src =>
                src.LAnnonce.Photos.FirstOrDefault() != null
                    ? src.LAnnonce.Photos.First().Photo.PhotoId
                    : 0))
            .ForMember(dest => dest.ListMessages, opt => opt.Ignore())
            .AfterMap((src, dest, context) =>
            {
                var currentUserId = (int)context.Items["CurrentUserId"];
                var mappedMessages = new List<MessageDTO>();

                foreach (var message in src.Messages.OrderBy(m => m.MessageDate))
                {
                    MessageDTO dto = null;
                    if (message.MessageTexte != null)
                    {
                        dto = new MessageTextDTO
                        {
                            MessageId = message.MessageId,
                            Date = message.MessageDate,
                            Lu = message.MessageLu,
                            SenderId = message.UtilisateurId,
                            SenderName = message.Utilisateur?.Login ?? string.Empty,
                            SentByCurrentUser = message.UtilisateurId == currentUserId,
                            Content = message.MessageTexte.Content ?? string.Empty,
                            Photos = new List<int>(message.MessageTexte.Photos.Select(p => p.PhotoId).ToList() ?? new List<int>())
                        };
                    }
                    else if (message.MessageDemande != null)
                    {
                        dto = new MessageDemandeDTO
                        {
                            MessageId = message.MessageId,
                            Date = message.MessageDate,
                            Lu = message.MessageLu,
                            SenderId = message.UtilisateurId,
                            SenderName = message.Utilisateur?.Login ?? string.Empty,
                            SentByCurrentUser = message.UtilisateurId == currentUserId,
                            PrixPropose = message.MessageDemande.PrixPropose,
                            DemandeId = message.MessageDemande.DemandeId,
                            EstAcceptee = message.MessageDemande.EstAcceptee,
                            EstRepondue = message.MessageDemande.EstRepondue
                        };
                    }
                    else if (message.MessageEstPayee != null)
                    {
                        dto = new MessageEstPayeeDTO
                        {
                            MessageId = message.MessageId,
                            Date = message.MessageDate,
                            Lu = message.MessageLu,
                            SenderId = message.UtilisateurId,
                            SenderName = message.Utilisateur?.Login ?? string.Empty,
                            SentByCurrentUser = message.UtilisateurId == currentUserId,
                            EstAnnule = message.MessageEstPayee.EstAnnule,
                            EstEnvoye = message.MessageEstPayee.EstEnvoye,
                            MessageEstPayeeId = message.MessageEstPayee.MessageEstPayeeId
                            //PrixValide = message.MessageValidation.PropositionValidee?.PrixPropose ?? 0
                        };
                    }
                    else if (message.MessageEnvoieColis != null)
                    {
                        dto = new MessageEnvoieColisDTO
                        {
                            MessageId = message.MessageId,
                            Date = message.MessageDate,
                            Lu = message.MessageLu,
                            SenderId = message.UtilisateurId,
                            SenderName = message.Utilisateur?.Login ?? string.Empty,
                            SentByCurrentUser = message.UtilisateurId == currentUserId,
                            MessageEnvoieColisId = message.MessageEnvoieColis.MessageEnvoieColisId,
                            PhotoId = message.MessageEnvoieColis.PhotoId,
                            MessageEstPayeeId = message.MessageEnvoieColis.MessageEstPayeeId,
                            MessageEstRecuId = message.MessageEnvoieColis.MessageEstRecu?.MessageEstRecuId != null ? message.MessageEnvoieColis.MessageEstRecu.MessageEstRecuId : 0
                        };
                    }
                    else if (message.MessageEstRecu != null)
                    {
                        dto = new MessageEstRecuDTO
                        {
                            MessageId = message.MessageId,
                            Date = message.MessageDate,
                            Lu = message.MessageLu,
                            SenderId = message.UtilisateurId,
                            SenderName = message.Utilisateur?.Login ?? string.Empty,
                            SentByCurrentUser = message.UtilisateurId == currentUserId,
                            MessageEstRecuId = message.MessageEstRecu.MessageEstRecuId,
                            EstConforme = message.MessageEstRecu.EstConforme,
                            Description = message.MessageEstRecu.Description,
                            PhotoId = message.MessageEstRecu.PhotoId,
                            
                        };
                    }
                    if (dto != null)
                    {
                        mappedMessages.Add(dto);
                    }
                }
                dest.ListMessages = new ObservableCollection<MessageDTO>(mappedMessages);
            });
    }
}