using API.Models.EntityFramework;
using AutoMapper;
using Shared.DTO;

namespace API.Mapper;

public class AdresseMappingProfile : Profile
{
    public AdresseMappingProfile()
    {
        CreateMap<Adresse, AdresseDTO>()
            .ForMember(dest => dest.AdresseId, opt => opt.MapFrom(src => src.AdresseId))
            .ForMember(dest => dest.AdresseRue, opt => opt.MapFrom(src => src.AdresseRue))
            .ForMember(dest => dest.AdresseVille, opt => opt.MapFrom(src => src.AdresseVille))
            .ForMember(dest => dest.AdresseCodePostal , opt => opt.MapFrom(src => src.AdresseCodePostal))
            .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.AdressePays , opt => opt.MapFrom(src => src.AdressePays))
            .ReverseMap();
        
        CreateMap<Adresse, UpdateAdresseDTO>()
            .ForMember(dest => dest.AdresseRue, opt => opt.MapFrom(src => src.AdresseRue))
            .ForMember(dest => dest.AdresseVille, opt => opt.MapFrom(src => src.AdresseVille))
            .ForMember(dest => dest.AdresseCodePostal , opt => opt.MapFrom(src => src.AdresseCodePostal))
            .ForMember(dest => dest.AdressePays , opt => opt.MapFrom(src => src.AdressePays))
            .ReverseMap();
        
        CreateMap<Adresse, CreateAdresseDTO>()
            .ForMember(dest => dest.AdresseRue, opt => opt.MapFrom(src => src.AdresseRue))
            .ForMember(dest => dest.AdresseVille, opt => opt.MapFrom(src => src.AdresseVille))
            .ForMember(dest => dest.AdresseCodePostal , opt => opt.MapFrom(src => src.AdresseCodePostal))
            .ForMember(dest => dest.UtilisateurId, opt => opt.MapFrom(src => src.UtilisateurId))
            .ForMember(dest => dest.AdressePays , opt => opt.MapFrom(src => src.AdressePays))
            .ReverseMap();
        
        CreateMap<Adresse, AdresseLivraisonDTO>()
            .ForMember(dest => dest.AdresseRue, opt => opt.MapFrom(src => src.AdresseRue))
            .ForMember(dest => dest.AdresseVille, opt => opt.MapFrom(src => src.AdresseVille))
            .ForMember(dest => dest.AdresseCodePostal , opt => opt.MapFrom(src => src.AdresseCodePostal))
            .ForMember(dest => dest.AdresseId, opt => opt.MapFrom(src => src.AdresseId))
            .ForMember(dest => dest.AdressePays , opt => opt.MapFrom(src => src.AdressePays))
            .ReverseMap();
    }
}