namespace CosmosDB
{
    public class IowaSales
    {
        public string? Invoice { get; set; }
        public int Date { get; set; }
        public int StoreNumber { get; set; }
        public string? StoreName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int ZipCode { get; set; }
        public int CountyNumber { get; set; }
        public string? CountyName { get; set; }
        public int CategoryNumber { get; set; }
        public string? CategoryName { get; set; }
        public int VendorNumber { get; set; }
        public string? VendorName { get; set; }
        public int ItemNumber { get; set; }
        public string? ItemName { get; set; }
        public int Pack { get; set; }
        public int BottleVolume { get; set; }
        public double StateBottleCost { get; set; }
        public double StateBootleRetail { get; set; }
        public int BootlesSold { get; set; }
        public double SaleDollars { get; set; }
        public double VolumeSoldLiters { get; set; }
        public double VolumeSoldGallons { get; set; }
        public string? id { get; set; }

    };
}