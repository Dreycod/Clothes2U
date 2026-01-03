using Shared.DTO.Historique;
using Shared.DTO.Transaction;
using API.Models.EntityFramework;
using AutoMapper;

namespace API.Mapper
{
    public class TransactionMappingProfile : Profile
    {
        public TransactionMappingProfile()
        {
            CreateMap<Transaction, TransactionDTO>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Montant, opt => opt.MapFrom(src => src.TransactionMontant))
                .ForMember(dest => dest.TransactionEtat, opt => opt.MapFrom(src => src.TransactionEtat))
                .ReverseMap();

            CreateMap<Transaction, TransactionHistoriqueDTO>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.Montant, opt => opt.MapFrom(src => src.TransactionMontant))
                .ForMember(dest => dest.TypeTransaction, opt => opt.Ignore())
                .ForMember(dest => dest.DateDebutNegociation, opt => opt.MapFrom(src => src.Conversation.CreationDate))
                .ForMember(dest => dest.UtilisateurId, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
