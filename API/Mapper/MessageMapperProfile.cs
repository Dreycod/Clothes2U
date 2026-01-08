using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO.Message;

namespace API.Mapper;

public class MessageMapperProfile : Profile
{
    public MessageMapperProfile()
    {
        CreateMap<Message, MessageDTO>()
            .Include<Message, MessageTextDTO>()
            .Include<Message, MessageDemandeDTO>()
            .Include<Message, MessageEstPayeeDTO>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.MessageDate))
            .ForMember(dest => dest.Lu, opt => opt.MapFrom(src => src.MessageLu));
       
        
        CreateMap<Message, MessageTextDTO>()
            .ForMember(dest => dest.MessageId,
                opt => opt.MapFrom(src => src.MessageId))

            .ForMember(dest => dest.Date,
                opt => opt.MapFrom(src => src.MessageDate))

            .ForMember(dest => dest.Lu,
                opt => opt.MapFrom(src => src.MessageLu))

            .ForMember(dest => dest.SenderId,
                opt => opt.MapFrom(src => src.UtilisateurId))

            .ForMember(dest => dest.ConversationId,
                opt => opt.MapFrom(src => src.ConversationId))

            .ForMember(dest => dest.Content,
                opt => opt.MapFrom(src => src.MessageTexte!.Content))

            .ForMember(dest => dest.Photos,
                opt => opt.MapFrom(src =>
                    src.MessageTexte != null
                        ? src.MessageTexte.Photos.Select(p => p.PhotoId).ToList()
                        : new List<int>()
                ));
        
        CreateMap<Message, MessageDemandeDTO>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.MessageDate))
            .ForMember(dest => dest.Lu, opt => opt.MapFrom(src => src.MessageLu))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.SentByCurrentUser, opt => opt.MapFrom((src, dest, _, context) =>
                src.UtilisateurId == (int)context.Items["CurrentUserId"]))
            .ForMember(dest => dest.PrixPropose, opt => opt.MapFrom(src =>
                src.MessageDemande != null ? src.MessageDemande.PrixPropose : 0))
            .ForMember(dest => dest.DemandeId, opt => opt.MapFrom(src =>
                src.MessageDemande != null ? src.MessageDemande.DemandeId : null))
            .ForMember(dest => dest.EstAcceptee, opt => opt.MapFrom(src => 
                src.MessageDemande != null ? src.MessageDemande.EstAcceptee : false))
            .ForMember(dest => dest.EstRepondue, opt => opt.MapFrom(src => 
                src.MessageDemande != null ? src.MessageDemande.EstRepondue : false));

        CreateMap<Message, MessageEstPayeeDTO>()
            .ForMember(dest => dest.MessageId, opt => opt.MapFrom(src => src.MessageId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.MessageDate))
            .ForMember(dest => dest.Lu, opt => opt.MapFrom(src => src.MessageLu))
            .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Utilisateur.Login))
            .ForMember(dest => dest.MessageEstPayeeId, opt =>opt.MapFrom(src => src.MessageEstPayee.MessageEstPayeeId))
            .ForMember(dest => dest.EstAnnule, opt => opt.MapFrom(src => src.MessageEstPayee.EstAnnule))
            .ForMember(dest => dest.EstEnvoye, opt => opt.MapFrom(src => src.MessageEstPayee.EstEnvoye));
    }
}