namespace WarehouseManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMaterials { get; set; }
        public int TotalLocations { get; set; }
        public int TotalCategories { get; set; }
        public int LowStockItems { get; set; }
        public int ExpiringItems { get; set; }
        public decimal TotalValue { get; set; }
        public int PendingTransfers { get; set; }
        public List<Transfer> RecentTransfers { get; set; } = new List<Transfer>();
        public List<Material> CriticalItems { get; set; } = new List<Material>();
    }
}