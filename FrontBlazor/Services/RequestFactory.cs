using FrontBlazor.Models;
using FrontBlazor.Models;
using FrontBlazor.Models.LoginRegister;

namespace FrontBlazor.Services
{
    public static class RequestFactory
    {
        public static LoginRequest CreateRegisterRequest(string username, string email, string password, string confirmPassword)
        {
            return new LoginRequest
            {
                Login = username,
                Email = email,
                Password = password,
                PasswordConfirm = confirmPassword
            };
        }

        public static LoginRequest CreateLoginRequest(string login, string password)
        {
            return new LoginRequest
            {
                Login = login,
                Password = password,
                PasswordConfirm = password
            };
        }

        //public static FilterDTO CreateFilterRequest(
        //    string? motCle = null,
        //    string? marque = null,
        //    string? categorie = null,
        //    string? sousCategorie = null,
        //    string? taille = null,
        //    double? prix = null,
        //    SortField? sortBy = null,
        //    SortOrder sortOrder = SortOrder.Ascending)
        //{
        //    //return new AnnonceSearchRequestDTO
        //    //{
        //    //    CategoryId = categoryId,
        //    //    Keyword = keyword
        //    //};
        //}
        // TEMPORAIRE
        public static FilterDTO CreateFilterRequest(
            string? motCle = null,
            double? prix = null,
            SortField? sortBy = null,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            return new FilterDTO
            {
                MotCle = motCle,
                Prix = prix,
                SortBy = sortBy,
                SortOrder = sortOrder
            };
        }
    }

}
