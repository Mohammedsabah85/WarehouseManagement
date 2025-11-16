using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Services
{
    public interface IReportService
    {
        Task<LocationReportViewModel> GenerateLocationReportAsync(int locationId);
        Task<List<Material>> GetLowStockMaterialsAsync(int threshold = 10);
        Task<List<Material>> GetExpiringMaterialsAsync(int days = 30);
        Task<byte[]> GenerateLocationReportPdfAsync(int locationId);
        Task<Dictionary<string, object>> GetDashboardStatisticsAsync();
    }
}