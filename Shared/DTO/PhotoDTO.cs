using Shared.Interfaces;

namespace Shared.DTO.Photo
{
    // DTO pour l'upload de photos (indépendant de ASP.NET Core)
    public class PhotoUploadDTO : IEntity
    {
        // ID de la photo (null pour une nouvelle photo)
        public int PhotoId { get; set; }

        // Nom du fichier (ex: "photo.jpg")
        public string FileName { get; set; } = string.Empty;

        // Type MIME (ex: "image/jpeg")
        public string ContentType { get; set; } = "image/jpeg";

       // Données de la photo encodées en Base64
        public string Base64Data { get; set; } = string.Empty;

        // Taille du fichier en octets
        public long FileSize { get; set; }
        public int GetId()
        {
            return PhotoId;
        }
    }

    // DTO pour associer des photos à une annonce
    public class AnnoncePhotoDTO
    {
        public int AnnonceId { get; set; }
        public List<PhotoUploadDTO> Photos { get; set; } = new();
    }

    // DTO de réponse après upload
    public class PhotoResponseDTO : IEntity
    {
        public int PhotoId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public DateTime DateUpload { get; set; }
        public int GetId()
        {
            return PhotoId;
        }
    }
}