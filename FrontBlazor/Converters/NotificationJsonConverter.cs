using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using FrontBlazor.Models.Notification;

namespace FrontBlazor.Converters;

public class NotificationJsonConverter : JsonConverter<Notification>
{
    public override Notification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        string typeDiscriminator = "";
        if (root.TryGetProperty("libelleType", out var libelleElement))
        {
            typeDiscriminator = libelleElement.GetString() ?? "";
        }
        Notification notification = typeDiscriminator switch
        {
            "Administration" => new NotificationAdmin(),
            "Avertissement" => new NotificationAvertissement(),
            "Message" => new NotificationMessage(),
            "Modification annonce" => new NotificationModificationAnnonce(),
            "Nouvelle annonce" => new NotificationNouvelleAnnonce(),
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
            case NotificationAdmin admin:
                if (root.TryGetProperty("adminText", out var adminTextElement))
                    admin.AdminText = adminTextElement.GetString();
                break;

            case NotificationAvertissement avertissement:
                if (root.TryGetProperty("messageAvertissement", out var messageAvertissementElement))
                    avertissement.MessageAvertissement = messageAvertissementElement.GetString();
                break;

            case NotificationMessage message:
                if (root.TryGetProperty("conversationId", out var conversationIdElement))
                    message.ConversationId = conversationIdElement.GetInt32();
                if (root.TryGetProperty("messagePreview", out var messagePreviewElement))
                    message.MessagePreview = messagePreviewElement.GetString();
                break;

            case NotificationModificationAnnonce modifAnnonce:
                if (root.TryGetProperty("modificationAnnonceId", out var modificationAnnonceIdElement))
                    modifAnnonce.ModificationAnnonceId = modificationAnnonceIdElement.GetInt32();
                if (root.TryGetProperty("nomAuteur", out var nomAuteurElement))
                    modifAnnonce.NomAuteur = nomAuteurElement.GetString();
                if (root.TryGetProperty("title", out var titleElement))
                    modifAnnonce.Title = titleElement.GetString();
                break;

            case NotificationNouvelleAnnonce nouvelleAnnonce:
                if (root.TryGetProperty("nouvelleAnnonceId", out var nouvelleAnnonceIdElement))
                    nouvelleAnnonce.NouvelleAnnonceId = nouvelleAnnonceIdElement.GetInt32();
                break;
        }

        return notification;
    }

    public override void Write(Utf8JsonWriter writer, Notification value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteString("libelleType", value.LibelleType);
        writer.WriteNumber("notificationId", value.NotificationId);
        writer.WriteString("dateCreation", value.DateCreation);
        writer.WriteBoolean("estLu", value.EstLu);

        switch (value)
        {
            case NotificationAdmin admin:
                if (admin.AdminText != null)
                    writer.WriteString("adminText", admin.AdminText);
                break;

            case NotificationAvertissement avertissement:
                if (avertissement.MessageAvertissement != null)
                    writer.WriteString("messageAvertissement", avertissement.MessageAvertissement);
                break;

            case NotificationMessage message:
                if (message.ConversationId.HasValue)
                    writer.WriteNumber("conversationId", message.ConversationId.Value);
                if (message.MessagePreview != null)
                    writer.WriteString("messagePreview", message.MessagePreview);
                break;

            case NotificationModificationAnnonce modifAnnonce:
                writer.WriteNumber("modificationAnnonceId", modifAnnonce.ModificationAnnonceId);
                writer.WriteString("nomAuteur", modifAnnonce.NomAuteur);
                writer.WriteString("title", modifAnnonce.Title);
                break;

            case NotificationNouvelleAnnonce nouvelleAnnonce:
                writer.WriteNumber("nouvelleAnnonceId", nouvelleAnnonce.NouvelleAnnonceId);
                break;
        }

        writer.WriteEndObject();
    }
}