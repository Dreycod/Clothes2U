namespace FrontBlazor.ViewModel
{
    public class ProfilViewModel
    {
        public string Username { get; set; } = string.Empty;

        public string activeTab = "articles";
        public bool hasArticles = true;

        public void SetActiveTab(string tab)
        {
            activeTab = tab;
        }

        public async Task InitializeAsync()
        {
            //await GetUser()
           
        }

        public async Task AddArticle()
        {
            throw  new NotImplementedException();
        }
    }
}
