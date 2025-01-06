using CleanFood.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;


namespace CleanFood
{
    public partial class MapPage : ContentPage
    {
        private List<Facility> facilities;
        private List<Facility> mapFacilities;
        private MainViewModel mainViewModel;
        public string PageTitle { get; set; }

        public MapPage(List<Facility> MapFacilities, List<Facility> Facilities, MainViewModel viewModel, String title, String type)
        {
            InitializeComponent();
            PageTitle = $"  Results for '{title}'";
            this.Title = $"  Results for '{title}'";
            facilities = Facilities;
            mapFacilities = MapFacilities;
            mainViewModel = viewModel;
            var Type = type;
  
            foreach (var faci in mapFacilities)
            {
                var pin = new FacilityPin(faci);
                pin.MarkerClicked += OnPinClicked;
                map.Pins.Add(pin);
            }

            if (mapFacilities.Any())
            {
                var centerLocation = new Location(mapFacilities.First().Latitude, mapFacilities.First().Longitude);
                if (Type == "Name")
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(centerLocation, Distance.FromMiles(1)));
                }
                else
                {
                    if (Type == "Zipcode" || Type == "County")
                    {
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(centerLocation, Distance.FromMiles(10)));
                    }
                }

                // TODO: Improve the logic for starting map distance:

                // compute max distance between lat/long pairs and map to a setting for distance from center that maximizes screen space used while showing all results

                // compute max distance between gps locations:
                 
                /*
                var maxLat = mapFacilities.Max(x => x.Latitude);             // northernmost point
                var maxLong = mapFacilities.Max(y =>  y.Longitude);          // eastermost point
                var minLat = mapFacilities.Min(x =>  x.Latitude);            // southernmost point
                var minLong = mapFacilities.Min(y => y.Longitude);           // westernmost point     
                Console.WriteLine(maxLat.GetType());
                var latDiff = maxLat.Latitude - minLat.Latitude;
                var longDiff = maxLong.Longitude - minLong.Longitude;
                */

                    
            }
            BindingContext = this;
        }

        private async void OnPinClicked(object sender, PinClickedEventArgs e) 
        {
            var pin = sender as FacilityPin; 
                       
            if (pin != null)
            {
                var facility = facilities[pin.Index];

                string popup = $"Address: {facility.Address}\nLast Inspection: {facility.Last_Inspected}\nViolations: {facility.Violations}\nCritical Violations: {facility.Critical_Violations}\nCritical Not Corrected: {facility.Critical_Not_Corrected}\nNon Critical Violations: {facility.Non_Critical_Violations}\nCounty: {facility.County}\nCity: {facility.City}\nZipcode: {facility.ZipCode}\nInspector's Comments: {facility.Inspection_Comments}";
                await DisplayAlert($"{facility.Name}", popup,"OK");
            }
        }
        private async void OnBackButtonClicked(object sender, EventArgs e) 
        {
            map.Pins.Clear();
            await Navigation.PopAsync(); 
        }
    }
}

