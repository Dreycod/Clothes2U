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


        //public static AnnonceSearchRequestDTO CreateFilterRequest(int? categoryId, string keyword)
        //{
        //    return new AnnonceSearchRequestDTO
        //    {
        //        CategoryId = categoryId,
        //        Keyword = keyword
        //    };
        //}
    }

}