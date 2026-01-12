using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO;

namespace API.Mapper;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // Commande -> OrderDTO
        CreateMap<Commande, OrderDTO>()
            .ForMember(dest => dest.TitreAnnonce, 
                opt => opt.MapFrom(src => src.Annonce.Title))
            .ForMember(dest => dest.PhotoAnnonce, 
                opt => opt.MapFrom(src => src.Annonce.Photos != null && src.Annonce.Photos.Any() 
                    ? src.Annonce.Photos.First().Photo.Image 
                    : null))
            .ForMember(dest => dest.PrixAnnonce, 
                opt => opt.MapFrom(src => src.Annonce.Prix))
            .ForMember(dest => dest.NomAcheteur, 
                opt => opt.MapFrom(src => $"{src.Acheteur.Login}"))
            .ForMember(dest => dest.EmailAcheteur, 
                opt => opt.MapFrom(src => src.Acheteur.Email))
            .ForMember(dest => dest.NomVendeur, 
                opt => opt.MapFrom(src => $"{src.Vendeur.Login} "))
            .ForMember(dest => dest.EmailVendeur, 
                opt => opt.MapFrom(src => src.Vendeur.Email))
            .ForMember(dest => dest.AdresseLivraison, 
                opt => opt.MapFrom(src => src.AdresseLivraison))
            .ForMember(dest => dest.StatutCommandeId, opt => opt.MapFrom(src => src.StatutCommandeId))
            .ForMember(dest => dest.DateCommande, opt => opt.MapFrom(src => src.DateCommande))
            .ForMember(dest => dest.StatutCommandeLibelle, opt => opt.MapFrom(src => src.StatutCommande.Libelle));

        // Adresse -> AdresseLivraisonDTO
        CreateMap<Adresse, AdresseLivraisonDTO>();
    }
}