namespace FrontBlazor.Services
{
    public class CurrentUserService
    {
        // pas safe, juste pour faire fonctionner:
        // un utiliasteur malveillant pourrait modifier l'id stocké ici et accéder aux données d'un autre utilisateur
        // approche avec validation de token JWT serait plus secure mais plus complexe à implémenter
        public int? UserId { get; private set; }

        public void SetUserId(int id)
        {
            UserId = id;
        }

        public void Clear()
        {
            UserId = null;
        }
    }
}
