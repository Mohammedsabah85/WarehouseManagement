namespace WarehouseManagement.Models.ViewModels
{
    public class ExportViewModel
    {
        public string ExportType { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? LocationId { get; set; }
        public int? CategoryId { get; set; }
        public bool IncludeZeroQuantity { get; set; } = false;
        public List<Location> Locations { get; set; } = new List<Location>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}