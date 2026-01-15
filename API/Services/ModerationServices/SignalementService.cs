using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Interfaces;
using AutoMapper;
using Shared.DTO.Signalement;

namespace API.Services;

public class SignalementService : ISignalementService
{
    private readonly ISignalementRepository _signalementRepository;
    private readonly IMapper _mapper;

    public SignalementService(
        ISignalementRepository signalementRepository,
        IMapper mapper)
    {
        _signalementRepository = signalementRepository;
        _mapper = mapper;
    }
    public async Task<SignalementDetailsDTO> CreateSignalementAsync(SignalementCreateDTO dto, int currentUserId)
    {
        var signalement = _mapper.Map<Signalement>(dto);
        signalement.UtilisateurId = currentUserId;
        var (annonceId, avisId, utilisateurSignaleId) = ExtractSignalementIds(dto);
        var createdSignalement = await _signalementRepository.CreateWithRelationsAsync(
            signalement,
            annonceId,
            avisId,
            utilisateurSignaleId
        );

        return _mapper.Map<SignalementDetailsDTO>(createdSignalement);
    }

    private static (int? annonceId, int? avisId, int? utilisateurSignaleId) ExtractSignalementIds(SignalementCreateDTO dto)
    {
        int? annonceId = null;
        int? avisId = null;
        int? utilisateurSignaleId = null;

        switch (dto)
        {
            case SignalementAnnonceCreateDTO annonceDto:
                annonceId = annonceDto.AnnonceSignaleeId;
                break;
            case SignalementAvisCreateDTO avisDto:
                avisId = avisDto.AvisId;
                break;
            case SignalementUtilisateurCreateDTO utilisateurDto:
                utilisateurSignaleId = utilisateurDto.UtilisateurSignaleId;
                break;
        }

        return (annonceId, avisId, utilisateurSignaleId);
    }
}