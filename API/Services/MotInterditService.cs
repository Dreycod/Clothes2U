using API.Models.Repository;

namespace API.Services;

public class MotInterditService :  IMotInterditService
{
    private readonly IMotInterditRepository _motInterditManager;

    public MotInterditService(IMotInterditRepository motInterditManager)
    {
        _motInterditManager = motInterditManager;
    }

    public async Task<bool> ContientMotInterdit(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
            return false;

        var motsInterdits = await _motInterditManager.GetAllLibellesAsync();
        var phraseLower = phrase.ToLower();

        foreach (var motInterdit in motsInterdits)
        {
            if (phraseLower.Contains(motInterdit.ToLower()))
            {
                return true;
            }
        }

        return false;
    }
}