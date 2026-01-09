using Shared.Interfaces;

namespace Shared.DTO.Tag
{
    public class TagDTO : IEntity
    {
        public int IdTag { get; set; }
        public string LibelleTag { get; set; } = null!;
        public int GetId()
        {
            return IdTag;
        }
    }

    public class CreateTagDTO
    {
        public string Libelle { get; set; } = string.Empty;
    }
}
