using Shared.DTO.MotInterdit;

namespace FrontBlazor.Services.Interfaces;

public interface IMotsInterditsService
{
    Task<List<MotInterditDTO>> GetAllAsync();
    Task DeleteAsync(int id);
    Task<(MotInterditDTO? mot, string? error)> AddAsync(MotInterditDTO entity);
}