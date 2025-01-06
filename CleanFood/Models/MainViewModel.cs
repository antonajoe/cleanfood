using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CleanFood.Models
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Properties for Facilities
        private ObservableCollection<string> filteredFacilities;
        private string nameSearchText;
        private NameTrie nameTrie;
        private bool isNameListViewVisible;
        private bool isNameStackViewVisible;
        private ObservableCollection<string> paginatedFacilities; 
        private int currentPage;
        private const int pageSize = 10;

        public ObservableCollection<string> PaginatedFacilities 
        { 
            get => paginatedFacilities; 
            set 
            { 
                paginatedFacilities = value; 
                OnPropertyChanged(); 
            } 
        }

        public ObservableCollection<string> FilteredFacilities
        {
            get => filteredFacilities;
            set
            {
                filteredFacilities = value;
                OnPropertyChanged();
            }
        }

        public string NameSearchText
        {
            get => nameSearchText;
            set
            {
                nameSearchText = value;
                OnPropertyChanged();
                FilterFacilitiesByName();
            }
        }

        public bool IsNameListViewVisible
        {
            get => isNameListViewVisible;
            set
            {
                isNameListViewVisible = value;
                Console.WriteLine($"IsNameListViewVisible: {isNameListViewVisible}");
                OnPropertyChanged();
            }
        }

        public bool IsNameStackViewVisible
        {
            get => isNameStackViewVisible;
            set
            {
                isNameStackViewVisible = value;
                Console.WriteLine($"IsNameStackViewVisible: {isNameStackViewVisible}");
                OnPropertyChanged();
            }
        }

        // Properties for Zipcodes
        private ObservableCollection<Zipcode> allZipcodes;
        private ObservableCollection<Zipcode> filteredZipcodes;
        private string zipcodeSearchText;
        private bool isZipcodeListViewVisible;
        private bool isZipcodeStackViewVisible;

        public ObservableCollection<Zipcode> FilteredZipcodes
        {
            get => filteredZipcodes;
            set
            {
                filteredZipcodes = value;
                OnPropertyChanged();
            }
        }

        public string ZipcodeSearchText
        {
            get => zipcodeSearchText;
            set
            {
                zipcodeSearchText = value;
                OnPropertyChanged();
                FilterZipcodes();
            }
        }

        public bool IsZipcodeListViewVisible
        {
            get => isZipcodeListViewVisible;
            set
            {
                isZipcodeListViewVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsZipcodeStackViewVisible
        {
            get => isZipcodeStackViewVisible;
            set
            {
                isZipcodeStackViewVisible = value;
                Console.WriteLine($"IsZipcodeStackViewVisible: {isZipcodeStackViewVisible}");
                OnPropertyChanged();
            }
        }

        // Properties for Counties
        private ObservableCollection<County> allCounties;
        private ObservableCollection<County> filteredCounties;
        private string countySearchText;
        private bool isCountyListViewVisible;
        private bool isCountyStackViewVisible;

        public ObservableCollection<County> FilteredCounties
        {
            get => filteredCounties;
            set
            {
                filteredCounties = value;
                OnPropertyChanged();
            }
        }

        public string CountySearchText
        {
            get => countySearchText;
            set
            {
                countySearchText = value;
                OnPropertyChanged();
                FilterCounties();
            }
        }

        public bool IsCountyListViewVisible
        {
            get => isCountyListViewVisible;
            set
            {
                isCountyListViewVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsCountyStackViewVisible
        {
            get => isCountyStackViewVisible;
            set
            {
                isCountyStackViewVisible = value;
                Console.WriteLine($"IsCountyStackViewVisible: {isCountyStackViewVisible}");
                OnPropertyChanged();
            }
        }

        // Constructor
        public MainViewModel(List<Zipcode> zipcodes, List<County> counties, NameTrie nTrie)
        {
            // Initialize collections and ViewModels
            FilteredFacilities = new ObservableCollection<string>();
            allZipcodes = new ObservableCollection<Zipcode>(zipcodes);
            FilteredZipcodes = new ObservableCollection<Zipcode>();
            allCounties = new ObservableCollection<County>(counties);
            FilteredCounties = new ObservableCollection<County>();
            PaginatedFacilities = new ObservableCollection<string>(); 
            currentPage = 0; 
            nameTrie = nTrie;
            IsNameListViewVisible = false;
            IsNameStackViewVisible = true;
            IsZipcodeListViewVisible = false;         // On initialization all stackviews are visible and all listviews invisible
            IsZipcodeStackViewVisible = true;
            IsCountyListViewVisible = false;
            IsCountyStackViewVisible = true;
        }

        // Filter methods for each Entry
        private void FilterFacilitiesByName()
        {
            Console.WriteLine(NameSearchText);
            FilteredFacilities = new ObservableCollection<string>();
            PaginatedFacilities = new ObservableCollection<string>();
            currentPage = 0;
            if (NameSearchText.Length >= 3)
            {
                var lowerSearchText = NameSearchText.ToLower();
                FilteredFacilities = nameTrie.GetNames(lowerSearchText);
                if (FilteredFacilities.Count >= pageSize)
                {
                    
                    PaginatedFacilities = new ObservableCollection<string>(FilteredFacilities.Take(pageSize));
                    Console.WriteLine(PaginatedFacilities.Count);
                    Console.WriteLine(PaginatedFacilities.GetType());
                    currentPage++;  
                }
                else
                {
                    var pages = new ObservableCollection<string>(FilteredFacilities.Take(FilteredFacilities.Count));
                    PaginatedFacilities = pages;
                    Console.WriteLine(PaginatedFacilities.Count);
                    Console.WriteLine(PaginatedFacilities.GetType());
                }   
            }
        }

        private void FilterZipcodes()
        {
            FilteredZipcodes = new ObservableCollection<Zipcode>();
            Console.WriteLine("ZIPCODE: " + ZipcodeSearchText);
            if (ZipcodeSearchText.Length >= 3)
            {
                if (ZipcodeSearchText.Length == 5)
                {
                    FilteredZipcodes = new ObservableCollection<Zipcode>(allZipcodes.Where(z => z.Id.Equals(ZipcodeSearchText)));
                }
                else
                {
                    FilteredZipcodes = new ObservableCollection<Zipcode>(allZipcodes.Where(z => z.Id.StartsWith(ZipcodeSearchText)));
                }
            }
        }

        private void FilterCounties()
        {
            FilteredCounties = new ObservableCollection<County>();
            if (CountySearchText.Length >= 2)
            {
                var lowerText = CountySearchText.ToLower();
                Console.WriteLine("COUNTY: " + lowerText);
                FilteredCounties = new ObservableCollection<County>(allCounties.Where(c => c.Name.ToLower().StartsWith(lowerText)));
            }
        }
        public void LoadMoreItems() 
        { 
            var itemsToLoad = FilteredFacilities.Skip(currentPage * pageSize).Take(pageSize); 
            foreach (var item in itemsToLoad) 
            { 
                PaginatedFacilities.Add(item); 
            } 
            currentPage++; 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            Console.WriteLine($"Property changed: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}