using Shared.DTO.Annonce;
using FrontBlazor.Services.Interfaces;
namespace FrontBlazor.Services
{
    public class AnnonceStateService : IStateService<AnnonceDTO>
    {
        public AnnonceDTO CurrentEntity { get; set; }

        public AnnonceDTO GetEntity()
        {
            return this.CurrentEntity;
        }

        public void SetEntity(AnnonceDTO entity)
        {
            this.CurrentEntity = entity;
        }

        public void ClearEntity()
        {
            this.CurrentEntity = null;
        }
    }
}
