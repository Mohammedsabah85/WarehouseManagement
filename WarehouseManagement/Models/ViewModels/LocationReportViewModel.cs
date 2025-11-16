namespace WarehouseManagement.Models.ViewModels
{
    public class LocationReportViewModel
    {
        public Location Location { get; set; }
        public List<Material> Materials { get; set; } = new List<Material>();
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
    }
}
