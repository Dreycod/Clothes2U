using FrontBlazor.Models;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class ProfilViewModel
    {
        public int? ViewingUser_UserId { get; set; } = null;

        public string activeTab = "articles";
        public bool hasArticles = true;

        public void SetActiveTab(string tab)
        {
            activeTab = tab;
        }
        public async Task LoadUserProfil(int id)
        {
            ViewingUser_UserId = id;

            //await GetUser()

        }

        public async Task AddArticle()
        {
            throw  new NotImplementedException();
        }
    }
}
