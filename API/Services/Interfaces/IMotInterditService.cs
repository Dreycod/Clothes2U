namespace API.Services;

public interface IMotInterditService
{
    Task<bool> ContientMotInterdit(string sentence);
    
}