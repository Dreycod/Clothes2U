

namespace Shared.DTO.Tag
{
    public class RecenseDTO
    {
        public int RecenseId { get; set; }
        public int AnnonceId { get; set; }
        public int TagId { get; set; }
    }

    public class CreateRecenseDTO
    {
        public int AnnonceId { get; set; }
        public int TagId { get; set; }
    }
}
