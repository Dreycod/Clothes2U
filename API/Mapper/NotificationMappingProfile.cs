using API.DTO.Notification;
using API.Models.EntityFramework;
using AutoMapper;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
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
                        ConversationId = src.NotificationMessages.Message?.ConversationId,
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
                        EstLu = src.EstLu,
                        NouvelleAnnonceId = src.NotificationNouvellesAnnonces.AnnonceId
                    };
                }

                return null;
            });
    }
}