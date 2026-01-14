namespace API.Services
{
    public interface IEmailTemplateService
    {
        string GetPasswordResetTemplate(string userName, string resetLink);
        string GetAccountSuspendedTemplate(string userName);
        string GetAccountBannedTemplate(string userName);
        string GetNewAnnonceTemplate(string userName, string vendeurName, string annonceTitle, string annonceUrl);
        string GetAnnonceUpdatedTemplate(string userName, string annonceTitle, string annonceUrl);
        string GetSupportResponseTemplate(string userName, string subject, string content, int ticketId);
        string GetClosedTicketTemplate(string name);
    }

    public class EmailTemplateService : IEmailTemplateService
    {
        private string GetBaseTemplate(string title, string content, string replyTo = null)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            background-color: #f5f5f5;
        }}
        .email-container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 12px;
            overflow: hidden;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 40px 30px;
            color: #333;
            line-height: 1.6;
        }}
        .button {{
            display: inline-block;
            padding: 12px 30px;
            background-color: #667eea;
            color: white !important;
            text-decoration: none;
            border-radius: 8px;
            font-weight: 500;
            margin: 20px 0;
        }}
        .button:hover {{
            background-color: #5568d3;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px 30px;
            text-align: center;
            color: #6b7280;
            font-size: 14px;
            border-top: 1px solid #e5e7eb;
        }}
        .alert {{
            padding: 15px;
            border-radius: 8px;
            margin: 20px 0;
        }}
        .alert-warning {{
            background-color: #fef3c7;
            border-left: 4px solid #f59e0b;
            color: #92400e;
        }}
        .alert-danger {{
            background-color: #fee2e2;
            border-left: 4px solid #ef4444;
            color: #991b1b;
        }}
        .alert-info {{
            background-color: #dbeafe;
            border-left: 4px solid #3b82f6;
            color: #1e40af;
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <h1>{title}</h1>
        </div>
        <div class='content'>
            {content}
        </div>
        <div class='footer'>
            <p>© 2025 Clothes2U - Tous droits réservés</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GetClosedTicketTemplate(string name)
        {
            var content = $@"
                <p>Bonjour {name}</p>
                <p>Vous venez de tenter de répondre à un ticket qui est clos.</p>
                <p>Merci d'en créer de vouloir créer un nouveau ticket si vous voulez joindre un modérateur.</p>
                </br>
                <p>Cordialement</p>
                <p>L'équipe <strong>Clothes2U</strong></p>
            ";
            return GetBaseTemplate("Ticket fermé", content);
        }

        public string GetPasswordResetTemplate(string userName, string resetLink)
        {
            var content = $@"
                <p>Bonjour <strong>{userName}</strong>,</p>
                <p>Vous avez demandé la réinitialisation de votre mot de passe.</p>
                <p>Cliquez sur le bouton ci-dessous pour créer un nouveau mot de passe :</p>
                <div style='text-align: center;'>
                    <a href='{resetLink}' class='button'>Réinitialiser mon mot de passe</a>
                </div>
                <p style='color: #6b7280; font-size: 14px; margin-top: 30px;'>
                    Ce lien expirera dans 24 heures. Si vous n'avez pas demandé cette réinitialisation, 
                    vous pouvez ignorer cet email en toute sécurité.
                </p>
            ";
            return GetBaseTemplate("Réinitialisation de mot de passe", content);
        }

        public string GetAccountSuspendedTemplate(string userName)
        {
            var content = $@"
                <p>Bonjour <strong>{userName}</strong>,</p>
                <div class='alert alert-warning'>
                    <strong>⚠️ Compte suspendu</strong>
                    <p style='margin: 10px 0 0 0;'>Votre compte a été temporairement suspendu suite à une violation de nos conditions d'utilisation.</p>
                </div>
                <p>Si vous pensez qu'il s'agit d'une erreur ou souhaitez plus d'informations, 
                   n'hésitez pas à contacter notre équipe de support.</p>
                <div style='text-align: center;'>
                    <a href='mailto:support@clothes2u.com' class='button'>Contacter le support</a>
                </div>
            ";
            return GetBaseTemplate("Compte suspendu", content);
        }

        public string GetAccountBannedTemplate(string userName)
        {
            var content = $@"
                <p>Bonjour <strong>{userName}</strong>,</p>
                <div class='alert alert-danger'>
                    <strong>🚫 Compte banni</strong>
                    <p style='margin: 10px 0 0 0;'>Votre compte a été définitivement banni suite à de graves violations de nos conditions d'utilisation.</p>
                </div>
                <p>Si vous pensez qu'il s'agit d'une erreur, vous pouvez faire appel de cette décision 
                   en contactant notre équipe de support dans les 30 jours.</p>
                <div style='text-align: center;'>
                    <a href='mailto:support@clothes2u.com' class='button'>Faire appel</a>
                </div>
            ";
            return GetBaseTemplate("Compte banni", content);
        }

        public string GetNewAnnonceTemplate(string userName, string vendeurName, string annonceTitle, string annonceUrl)
        {
            var content = $@"
                <p>Bonjour <strong>{userName}</strong>,</p>
                <div class='alert alert-info'>
                    <strong>🎉 Nouvelle annonce disponible !</strong>
                    <p style='margin: 10px 0 0 0;'>
                        Le vendeur <strong>{vendeurName}</strong> que vous suivez a publié une nouvelle annonce.
                    </p>
                </div>
                <h3 style='color: #1a1a1a; margin: 25px 0 15px 0;'>{annonceTitle}</h3>
                <div style='text-align: center;'>
                    <a href='{annonceUrl}' class='button'>Voir l'annonce</a>
                </div>
            ";
            return GetBaseTemplate("Nouvelle annonce disponible", content);
        }

        public string GetAnnonceUpdatedTemplate(string userName, string annonceTitle, string annonceUrl)
        {
            var content = $@"
                <p>Bonjour <strong>{userName}</strong>,</p>
                <p>L'annonce <strong>{annonceTitle}</strong> que vous avez ajoutée à vos favoris a été mise à jour.</p>
                <div style='text-align: center;'>
                    <a href='{annonceUrl}' class='button'>Voir les modifications</a>
                </div>
            ";
            return GetBaseTemplate("Annonce mise à jour", content);
        }

        public string GetSupportResponseTemplate(string userName, string subject, string content, int ticketId)
        {
            var htmlContent = $@"
        <p>Bonjour <strong>{userName}</strong>,</p>
        <p>Vous avez reçu une réponse à votre ticket de support :</p>
        <div style='background-color: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;'>
            {content.Replace("\n", "<br>")}
        </div>
        <p style='color: #6b7280; font-size: 14px; margin-top: 30px;'>
            <strong>💡 Astuce :</strong> Vous pouvez répondre directement à cet email pour continuer la conversation.
        </p>
        <p style='color: #9ca3af; font-size: 12px;'>
            Référence du ticket : #{ticketId}
        </p>
    ";
            return GetBaseTemplate($"[Ticket #{ticketId}] {subject}", htmlContent);
        }
    }
}