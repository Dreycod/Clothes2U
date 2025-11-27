namespace FrontBlazor.Models.StateServices;

public class UserStateService : IStateService<Utilisateur>
{
    public Utilisateur CurrentEntity { get; set; }

    public void SetUser(Utilisateur entity)
    {
        this.CurrentEntity = entity;
    }

    public void ClearUser()
    {
        this.CurrentEntity = null;
    }
}