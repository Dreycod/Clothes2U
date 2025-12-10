using FrontBlazor.Models.Moderation;

namespace FrontBlazor.Services.Interfaces;

public interface IMotsInterditsService
{
    Task<List<MotInterdit>> GetAllAsync();
    Task DeleteAsync(int id);
    Task<MotInterdit?> AddAsync(MotInterdit entity);
}