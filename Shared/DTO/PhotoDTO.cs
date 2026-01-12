using Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
        // bool pour validation moderateur
        public bool? EnAttenteValidation { get; set; }

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
    // DTO pour validation Photo
    public class PhotoDTO
    {
        [JsonPropertyName("photoId")]
        public int PhotoId { get; set; }
        [JsonPropertyName("image")]
        public byte[] Image { get; set; }
        [JsonPropertyName("enAttenteValidation")]
        public bool? EnAttenteValidation { get; set; }
    }
    public class PhotoDataDTO
    {
        public string PreviewBase64 { get; set; }
        public bool IsDangerous { get; set; }
    }

    // DTO de réponse après upload + validation
    public class PhotoResponseDTO : IEntity
    {
        public int PhotoId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool? EnAttenteValidation { get; set; }
        public DateTime DateUpload { get; set; }
        public int GetId()
        {
            return PhotoId;
        }
    }
}