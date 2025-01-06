namespace CleanFood
{
    public partial class App : Application
    {
        public App()
        {

            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            // Register the route for MapPage
            Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));
            Routing.RegisterRoute(nameof(LeafletMapPage), typeof(LeafletMapPage));
        }

        /*
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
        */
    }
}