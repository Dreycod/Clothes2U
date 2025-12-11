namespace FrontBlazor.Models.StateServices
{
    public class AnnonceStateService : IStateService<Annonce>
    {
        public Annonce CurrentEntity { get; set; }

        public Annonce GetEntity()
        {
            return this.CurrentEntity;
        }

        public void SetEntity(Annonce entity)
        {
            this.CurrentEntity = entity;
        }

        public void ClearEntity()
        {
            this.CurrentEntity = null;
        }
    }
}
