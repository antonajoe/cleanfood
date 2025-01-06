namespace CleanFood
{
    public class Facility
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
        public string ZipCode { get; set; }
        public string Inspection_Comments { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string State { get; set; }
        public string DisplayName { get; set; }
    }
}