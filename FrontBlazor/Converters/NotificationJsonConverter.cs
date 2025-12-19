using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Shared.DTO.Notification;

namespace FrontBlazor.Converters;

public class NotificationJsonConverter : JsonConverter<NotificationDTO>
{
    public override NotificationDTO Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        string typeDiscriminator = "";
        if (root.TryGetProperty("libelleType", out var libelleElement))
        {
            typeDiscriminator = libelleElement.GetString() ?? "";
        }
        NotificationDTO notification = typeDiscriminator switch
        {
            "Administration" => new NotificationAdminDTO(),
            "Avertissement" => new NotificationAvertissementDTO(),
            "Message" => new NotificationMessageDTO(),
            "Modification annonce" => new NotificationModificationAnnonceDTO(),
            "Nouvelle annonce" => new NotificationNouvelleAnnonceDTO(),
            _ => throw new JsonException($"Type de notification inconnu: {typeDiscriminator}")
        };
        if (root.TryGetProperty("notificationId", out var notificationIdElement))
            notification.NotificationId = notificationIdElement.GetInt32();
        if (root.TryGetProperty("dateCreation", out var dateCreationElement))
            notification.DateCreation = dateCreationElement.GetDateTime();
        if (root.TryGetProperty("estLu", out var estLuElement))
            notification.EstLu = estLuElement.GetBoolean();
        switch (notification)
        {
            case NotificationAdminDTO admin:
                if (root.TryGetProperty("adminText", out var adminTextElement))
                    admin.AdminText = adminTextElement.GetString();
                break;

            case NotificationAvertissementDTO avertissement:
                if (root.TryGetProperty("messageAvertissement", out var messageAvertissementElement))
                    avertissement.MessageAvertissement = messageAvertissementElement.GetString();
                break;

            case NotificationMessageDTO message:
                if (root.TryGetProperty("conversationId", out var conversationIdElement))
                    message.ConversationId = conversationIdElement.GetInt32();
                if (root.TryGetProperty("messagePreview", out var messagePreviewElement))
                    message.MessagePreview = messagePreviewElement.GetString();
                break;

            case NotificationModificationAnnonceDTO modifAnnonce:
                if (root.TryGetProperty("modificationAnnonceId", out var modificationAnnonceIdElement))
                    modifAnnonce.ModificationAnnonceId = modificationAnnonceIdElement.GetInt32();
                if (root.TryGetProperty("nomAuteur", out var nomAuteurElement))
                    modifAnnonce.NomAuteur = nomAuteurElement.GetString();
                if (root.TryGetProperty("title", out var titleElement))
                    modifAnnonce.Title = titleElement.GetString();
                break;

            case NotificationNouvelleAnnonceDTO nouvelleAnnonce:
                if (root.TryGetProperty("nouvelleAnnonceId", out var nouvelleAnnonceIdElement))
                    nouvelleAnnonce.NouvelleAnnonceId = nouvelleAnnonceIdElement.GetInt32();
                break;
        }

        return notification;
    }

    public override void Write(Utf8JsonWriter writer, NotificationDTO value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("libelleType", value.LibelleType);
        writer.WriteNumber("notificationId", value.NotificationId);
        writer.WriteString("dateCreation", value.DateCreation);
        writer.WriteBoolean("estLu", value.EstLu);

        switch (value)
        {
            case NotificationAdminDTO admin:
                if (admin.AdminText != null)
                    writer.WriteString("adminText", admin.AdminText);
                break;

            case NotificationAvertissementDTO avertissement:
                if (avertissement.MessageAvertissement != null)
                    writer.WriteString("messageAvertissement", avertissement.MessageAvertissement);
                break;

            case NotificationMessageDTO message:
                if (String.IsNullOrEmpty(message.ConversationId.ToString()))
                    writer.WriteNumber("conversationId", message.ConversationId);
                if (message.MessagePreview != null)
                    writer.WriteString("messagePreview", message.MessagePreview);
                break;

            case NotificationModificationAnnonceDTO modifAnnonce:
                writer.WriteNumber("modificationAnnonceId", modifAnnonce.ModificationAnnonceId);
                writer.WriteString("nomAuteur", modifAnnonce.NomAuteur);
                writer.WriteString("title", modifAnnonce.Title);
                break;

            case NotificationNouvelleAnnonceDTO nouvelleAnnonce:
                writer.WriteNumber("nouvelleAnnonceId", (decimal)nouvelleAnnonce.NouvelleAnnonceId);
                break;
        }

        writer.WriteEndObject();
    }
}