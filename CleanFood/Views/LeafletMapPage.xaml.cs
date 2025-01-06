
namespace CleanFood
{
    public partial class LeafletMapPage : ContentPage
    {
        public LeafletMapPage()
        {
           
            InitializeComponent();
            LoadHtml();
        }

        private void LoadHtml()
        {
            var webView = new WebView();
            webView.Source = new HtmlWebViewSource { BaseUrl = @"map.html" };

        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
