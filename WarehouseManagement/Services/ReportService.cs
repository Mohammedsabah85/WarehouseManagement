using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Services
{
    public class ReportService : IReportService
    {
        private readonly WarehouseContext _context;

        public ReportService(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<LocationReportViewModel> GenerateLocationReportAsync(int locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location == null)
                throw new ArgumentException("Location not found");

            var materials = await _context.Materials
                .Include(m => m.Category)
                .Where(m => m.LocationId == locationId && m.Quantity > 0)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return new LocationReportViewModel
            {
                Location = location,
                Materials = materials,
                TotalItems = materials.Count,
                TotalQuantity = materials.Sum(m => m.Quantity)
            };
        }

        public async Task<List<Material>> GetLowStockMaterialsAsync(int threshold = 10)
        {
            return await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.Quantity <= threshold)
                .OrderBy(m => m.Quantity)
                .ToListAsync();
        }

        public async Task<List<Material>> GetExpiringMaterialsAsync(int days = 30)
        {
            var expiringDate = DateTime.Now.AddDays(days);
            return await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.ExpiryDate.HasValue && m.ExpiryDate <= expiringDate)
                .OrderBy(m => m.ExpiryDate)
                .ToListAsync();
        }

        public async Task<byte[]> GenerateLocationReportPdfAsync(int locationId)
        {
            // يمكن إضافة مكتبة PDF هنا مثل iTextSharp أو PdfSharp
            // هذا مثال أساسي
            var report = await GenerateLocationReportAsync(locationId);

            // TODO: تحويل التقرير إلى PDF
            // return PdfGenerator.GenerateFromHtml(htmlContent);

            return new byte[0]; // placeholder
        }

        public async Task<Dictionary<string, object>> GetDashboardStatisticsAsync()
        {
            var stats = new Dictionary<string, object>();

            stats["TotalMaterials"] = await _context.Materials.CountAsync();
            stats["TotalLocations"] = await _context.Locations.CountAsync();
            stats["TotalCategories"] = await _context.Categories.CountAsync();
            stats["TotalTransfers"] = await _context.Transfers.CountAsync();

            stats["LowStockItems"] = await _context.Materials.CountAsync(m => m.Quantity <= 10);
            stats["ExpiringItems"] = await _context.Materials.CountAsync(m =>
                m.ExpiryDate.HasValue && m.ExpiryDate <= DateTime.Now.AddDays(30));

            stats["TotalValue"] = await _context.Materials
                .Where(m => m.Price.HasValue)
                .SumAsync(m => m.Price.Value * m.Quantity);

            stats["PendingTransfers"] = await _context.Transfers.CountAsync(t => !t.IsConfirmed);

            return stats;
        }
    }
}
