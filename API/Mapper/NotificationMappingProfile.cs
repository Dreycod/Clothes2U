using Shared.DTO.Notification;
using API.Models.EntityFramework;
using AutoMapper;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<NotificationCreateDTO, Notification>()
            .ForMember(dest => dest.DateCreation, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.EstLu, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.NotificationTypeId, opt => opt.MapFrom(src => src.TypeId))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId));


        CreateMap<NotificationMessageCreateDTO, NotificationMessage>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.MessagePreview, opt => opt.MapFrom(src => src.MessagePreview))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));
        
        CreateMap<NotificationAvertissementCreateDTO, NotificationAvertissement>()
            .ForMember(dest => dest.MessageAvertissement, opt => opt.MapFrom(src => src.MessageModerateur))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));

        CreateMap<NotificationPropositionCreateDTO, NotificationProposition>()
            .ForMember(dest => dest.PropositionId, opt => opt.MapFrom(src => src.PropositionId))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));

        CreateMap<NotificationNouvelleAnnonceCreateDTO, NotificationNouvelleAnnonce>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));

        CreateMap<NotificationModificationAnnonceCreateDTO, NotificationModificationAnnonce>()
            .ForMember(dest => dest.AnnonceId, opt => opt.MapFrom(src => src.AnnonceId))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));

        
        CreateMap<NotificationAdminCreateDTO,NotificationAdmin>()
            .ForMember(dest => dest.AdminText, opt => opt.MapFrom(src => src.AdminText))
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId));

        
        
        CreateMap<Notification, NotificationDTO>()
            .ConvertUsing((src, dest, context) =>
            {
                if (src.NotificationAdmins != null)
                {
                    return new NotificationAdminDTO
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        EstLu = src.EstLu,
                        AdminText = src.NotificationAdmins.AdminText
                    };
                }
                else if (src.NotificationAvertissements != null)
                {
                    return new NotificationAvertissementDTO
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        EstLu = src.EstLu,
                        MessageAvertissement = src.NotificationAvertissements.MessageAvertissement
                    };
                }
                else if (src.NotificationMessages != null)
                {
                    return new NotificationMessageDTO
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        EstLu = src.EstLu,
                        ConversationId = src.NotificationMessages.Message.ConversationId,
                        MessagePreview = src.NotificationMessages.MessagePreview
                    };
                }
                else if (src.NotificationModifications != null)
                {
                    return new NotificationModificationAnnonceDTO
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        EstLu = src.EstLu,
                        ModificationAnnonceId = src.NotificationModifications.AnnonceId,
                        NomAuteur = src.NotificationModifications.Annonce?.Utilisateur?.Login,
                        Title = src.NotificationModifications.Annonce?.Title
                    };
                }
                else if (src.NotificationNouvellesAnnonces != null)
                {
                    return new NotificationNouvelleAnnonceDTO
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        NomAuteur = src.NotificationNouvellesAnnonces.Annonce.Utilisateur.Login,
                        EstLu = src.EstLu,
                        NouvelleAnnonceId = src.NotificationNouvellesAnnonces.AnnonceId
                    };
                }else if (src.NotificationProposition != null)
                {
                    return new NotificationPropositionDTO()
                    {
                        NotificationId = src.NotificationId,
                        DateCreation = src.DateCreation,
                        EstLu = src.EstLu,
                        PrixPropose = src.NotificationProposition.MessageDemande.PrixPropose ,
                        AncienPrixPropose = src.NotificationProposition.MessageDemande?.Offre?.PrixPropose,
                        ConversationId = src.NotificationProposition.MessageDemande.Message.ConversationId,
                        AnnonceTitle =  src.NotificationProposition.MessageDemande.Message.Conversation.LAnnonce.Title,
                        NomAuteur = src.NotificationProposition.MessageDemande.Message.Utilisateur.Login,
                    };
                }

                return null;
            });
    }
}