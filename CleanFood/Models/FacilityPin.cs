using Microsoft.Maui.Controls.Maps;

namespace CleanFood
{
    public class FacilityPin : Pin
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Last_Inspected { get; set; }
        public string Violations { get; set; }
        public double Critical_Violations { get; set; }
        public double Critical_Not_Corrected { get; set; }
        public double Non_Critical_Violations { get; set; }
        public string County { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Inspection_Comments { get; set; }

        public FacilityPin(Facility facility)
        {
            Index = facility.Index;
            Type = PinType.SearchResult;
            Location = new Location(facility.Latitude, facility.Longitude);
            Label = facility.Name;
        }
    }
}