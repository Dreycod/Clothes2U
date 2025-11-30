namespace FrontBlazor.Models.StateServices;

public class UserStateService : IStateService<Utilisateur>
{
    public Utilisateur CurrentEntity { get; set; }

    public Utilisateur GetEntity()
    {
        return this.CurrentEntity;
    }

    public void SetEntity(Utilisateur entity)
    {
        this.CurrentEntity = entity;
    }

    public void ClearEntity()
    {
        this.CurrentEntity = null;
    }
}