using CleanFood.Models;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace CleanFood
{
    public partial class MainPage : ContentPage
    {
        MainViewModel viewModel;
        private List<Facility> facilities;
        public List<Facility> mapFacilities;
        private List<Zipcode> zipcodes;
        private List<County> counties;
        private NameTrie nameTrie;
        private readonly Subject<string> nameTextChangedSubject = new Subject<string>();
        private readonly Subject<string> zipcodeTextChangedSubject = new Subject<string>();
        private readonly Subject<string> countyTextChangedSubject = new Subject<string>();

        private IDisposable nameDebounceSubscription;
        private IDisposable zipcodeDebounceSubscription;
        private IDisposable countyDebounceSubscription;

        public MainPage()
        {
            InitializeComponent();
            InitializeData();
            nameDebounceSubscription = nameTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnNameDebouncedTextChanged);

            zipcodeDebounceSubscription = zipcodeTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnZipcodeDebouncedTextChanged);

            countyDebounceSubscription = countyTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnCountyDebouncedTextChanged);
    
        }

        private async void InitializeData()
        {
            facilities = new List<Facility>();
            mapFacilities = new List<Facility>();
            nameTrie = new NameTrie();
            zipcodes = new List<Zipcode>();
            counties = new List<County>();

            await LoadFacilities();
            await LoadZipcodes();
            await LoadCounties();

            viewModel = new MainViewModel(zipcodes, counties, nameTrie);
            BindingContext = viewModel;
        }

        private async Task LoadFacilities()
        {
            Console.WriteLine("##### started LoadFacilities");
            var name = "inspections.csv";
            var stream = await FileSystem.OpenAppPackageFileAsync(name);
            var reader = new StreamReader(stream);
            string line = reader.ReadLine();
            string[] items = new string[] { };

            while (line != null)
            {
                items = line.Split(";");
                if (items.Length != 18)
                {
                    Console.WriteLine("ERROR AT INDEX " + items[0]);
                    Console.WriteLine(items.Length);
                }
                else
                {
                    var facility = new Facility
                    {
                        Index = int.Parse(items[0]),
                        Name = items[1],
                        Address = items[2],
                        Last_Inspected = items[3],
                        Violations = items[4],
                        Critical_Violations = double.Parse(items[5]),
                        Critical_Not_Corrected = double.Parse(items[6]),
                        Non_Critical_Violations = double.Parse(items[7]),
                        Description = items[8],
                        County = items[9],
                        Facility_Address = items[10],
                        City = items[11],
                        ZipCode = items[12],
                        Inspection_Comments = items[13],
                        Latitude = double.Parse(items[14]),
                        Longitude = double.Parse(items[15]),
                        State = items[16],
                        DisplayName = items[17]
                    };
                    facilities.Add(facility);
                    nameTrie.Insert(facility.Name, facility.DisplayName);
                }
                line = reader.ReadLine();
            }
            Console.WriteLine("Length of Facilities ");
            Console.WriteLine(facilities.Count);
        }

        private async Task LoadZipcodes()
        {
            var name1 = "zipcodes.txt";
            var stream1 = await FileSystem.OpenAppPackageFileAsync(name1);
            var reader1 = new StreamReader(stream1);
            string line = reader1.ReadLine();

            while (line != null)
            {
                Zipcode zipcode = new Zipcode();
                zipcode.Id = line;
                zipcodes.Add(zipcode);
                line = reader1.ReadLine();
            }
        }

        private async Task LoadCounties()
        {
            var name2 = "counties.txt";
            var stream2 = await FileSystem.OpenAppPackageFileAsync(name2);
            var reader2 = new StreamReader(stream2);
            string line = reader2.ReadLine();
            int m = 0;
            while (line != null)
            {
                County county = new County();
                county.Name = line;
                county.Index = m;
                m++;
                counties.Add(county);
                line = reader2.ReadLine();
            }
        }

        private async void OnNameSearchCompleted(object sender, EventArgs e)
        {
            // clear error labels
            ErrorLabel1.Text = "";
            ErrorLabel2.Text = "";
            ErrorLabel3.Text = "";

            var NameSearchText = NameSearchEntry.Text?.ToLower();

            Console.WriteLine("NameSearchText, " + NameSearchText);

            if (NameSearchText.Length > 2)
            {
                mapFacilities = facilities.Where(f => f.DisplayName.ToLower().StartsWith(NameSearchText)).ToList();         //.Contains(NameSearchText)).ToList();

                if (mapFacilities.Count == 0)
                {
                    ErrorLabel1.Text = $"No Results Found for {NameSearchText}. Please Try Again";
                }
                else
                {
                    // navigate to MapPage
                    var title = NameSearchText;
                    NameSearchEntry.Text = "";
                    await Navigation.PushAsync(new NavigationPage(new MapPage(mapFacilities, facilities, viewModel, title, "Name")));
                }
            }
            else
            {
                ErrorLabel1.Text = $"Please Enter at least 3 letters.";
            }
        }

        private async void OnZipcodeSearchCompleted(object sender, EventArgs e)
        {
            // clear error labels
            ErrorLabel1.Text = "";
            ErrorLabel2.Text = "";
            ErrorLabel3.Text = "";

            var ZipSearchText = ZipcodeSearchEntry.Text?.ToString();
            Console.WriteLine("ZipSearchText, " + ZipSearchText);

            if (ZipSearchText.Length > 3)
            {
                mapFacilities = facilities.Where(f => f.ZipCode.StartsWith(ZipSearchText)).ToList();
                if (mapFacilities.Count == 0)
                {
                    ErrorLabel2.Text = $"No Results Found for {ZipSearchText}. Please Try Again";
                }
                else
                {
                    // navigate to MapPage
                    var title = ZipSearchText;
                    ZipcodeSearchEntry.Text = "";
                    await Navigation.PushAsync(new NavigationPage(new MapPage(mapFacilities, facilities, viewModel, title, "Zipcode")));
                }
            }
            else
            {
                ErrorLabel2.Text = $"Please Enter a 5 Digit Zipcode.";
            }
        }

        private async void OnCountySearchCompleted(object sender, EventArgs e)
        {
            // clear error labels
            ErrorLabel1.Text = "";
            ErrorLabel2.Text = "";
            ErrorLabel3.Text = "";

            var CountySearchText = CountySearchEntry.Text?.ToLower();
            Console.WriteLine(CountySearchText + " " + CountySearchText);

            if (CountySearchText.Length > 2)
            {
                mapFacilities = facilities.Where(f => f.County.ToLower().StartsWith(CountySearchText)).ToList();
                if (mapFacilities.Count == 0)
                {
                    ErrorLabel3.Text = $"No Results Found for {CountySearchText}. Please Try Again";
                }
                else
                {
                    // navigate to MapPage
                    var title = CountySearchText;
                    CountySearchEntry.Text = "";
                    await Navigation.PushAsync(new NavigationPage(new MapPage(mapFacilities, facilities, viewModel, title, "County")));
                    
                }
            }
            else
            {
                ErrorLabel3.Text = $"Please Enter a County name.";
            }
        }

        private async void OnSearchAllButtonClicked(object sender, EventArgs e)
        {
            // Navigate to new page that displays leaflet.js map with all entries
            await Navigation.PushAsync(new NavigationPage(new LeafletMapPage()));
        }

        private void NameSearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            nameTextChangedSubject.OnNext(e.NewTextValue);
        }

        private void ZipcodeSearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            zipcodeTextChangedSubject.OnNext(e.NewTextValue);
        }

        private void CountySearchEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            countyTextChangedSubject.OnNext(e.NewTextValue);
        }

        private void OnNameDebouncedTextChanged(string text)
        {

            // needs to filter non alpha input characters, either here or in view model 
            // OR, modify NameTrie to handle numeric inputs

            if (text.Length > 2)
            {
                viewModel.NameSearchText = text;
                viewModel.IsZipcodeStackViewVisible = false;
                viewModel.IsCountyStackViewVisible = false;
                viewModel.IsNameListViewVisible = true;
            }
            else
            {
                viewModel.IsZipcodeStackViewVisible = true;
                viewModel.IsCountyStackViewVisible = true;
                viewModel.IsNameListViewVisible = false;
            }
        }

        private void OnZipcodeDebouncedTextChanged(string text)
        {
            Console.WriteLine("ZIPCODE ENTRY: " + text);
            if (text.Length > 2)
            {
                viewModel.ZipcodeSearchText = text;
                viewModel.IsNameStackViewVisible = false;
                viewModel.IsCountyStackViewVisible = false;
                viewModel.IsZipcodeListViewVisible = true;
            }
            else
            {
                viewModel.IsNameStackViewVisible = true;
                viewModel.IsCountyStackViewVisible = true;
                viewModel.IsZipcodeListViewVisible = false;
            }
        }

        private void OnCountyDebouncedTextChanged(string text)
        {
            if (text.Length >= 2)
            {
                viewModel.CountySearchText = text;
                viewModel.IsZipcodeStackViewVisible = false;
                viewModel.IsNameStackViewVisible = false;
                viewModel.IsCountyListViewVisible = true;
            }
            else
            {
                viewModel.IsZipcodeStackViewVisible = true;
                viewModel.IsNameStackViewVisible = true;
                viewModel.IsCountyListViewVisible = false;
            }
        }

        private void NameListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = e.Item as String;
            NameSearchEntry.Text = tappedItem;
        }

        private void ZipcodeListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = e.Item as Zipcode;
            ZipcodeSearchEntry.Text = tappedItem.Id;
        }

        private void CountyListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var tappedItem = e.Item as County;
            CountySearchEntry.Text = tappedItem.Name;
        }

        private void NameListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            var item = e.Item as string; 
            if (item == viewModel.PaginatedFacilities.LastOrDefault()) 
            { 
                viewModel.LoadMoreItems(); 
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Clear search texts
            viewModel.NameSearchText = string.Empty;
            viewModel.ZipcodeSearchText = string.Empty;
            viewModel.CountySearchText = string.Empty;

            // Reset visibility states
            viewModel.IsNameStackViewVisible = true;
            viewModel.IsZipcodeStackViewVisible = true;
            viewModel.IsCountyStackViewVisible = true;
            viewModel.IsNameListViewVisible = false;
            viewModel.IsZipcodeListViewVisible = false;
            viewModel.IsCountyListViewVisible = false;

            nameDebounceSubscription = nameTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnNameDebouncedTextChanged);

            zipcodeDebounceSubscription = zipcodeTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnZipcodeDebouncedTextChanged);

            countyDebounceSubscription = countyTextChangedSubject
                .Throttle(TimeSpan.FromMilliseconds(200))
                .Subscribe(OnCountyDebouncedTextChanged);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            nameDebounceSubscription.Dispose();
            zipcodeDebounceSubscription.Dispose();
            countyDebounceSubscription.Dispose();
        }
    }
}
