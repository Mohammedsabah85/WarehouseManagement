using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.ViewModels;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    public class DashboardController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly IReportService _reportService;

        public DashboardController(WarehouseContext context, IReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var stats = await _reportService.GetDashboardStatisticsAsync();

            var recentTransfers = await _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .OrderByDescending(t => t.TransferDate)
                .Take(5)
                .ToListAsync();

            var criticalItems = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.Quantity <= 5 ||
                           (m.ExpiryDate.HasValue && m.ExpiryDate <= DateTime.Now.AddDays(7)))
                .Take(10)
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalMaterials = (int)stats["TotalMaterials"],
                TotalLocations = (int)stats["TotalLocations"],
                TotalCategories = (int)stats["TotalCategories"],
                LowStockItems = (int)stats["LowStockItems"],
                ExpiringItems = (int)stats["ExpiringItems"],
                TotalValue = (decimal)stats["TotalValue"],
                PendingTransfers = (int)stats["PendingTransfers"],
                RecentTransfers = recentTransfers,
                CriticalItems = criticalItems
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetChartData(string type)
        {
            switch (type.ToLower())
            {
                case "materials-by-category":
                    var categoryData = await _context.Categories
                        .Select(c => new {
                            Name = c.Name,
                            Count = c.Materials.Count
                        })
                        .ToListAsync();
                    return Json(categoryData);

                case "materials-by-location":
                    var locationData = await _context.Locations
                        .Select(l => new {
                            Name = l.Name,
                            Count = l.Materials.Count
                        })
                        .ToListAsync();
                    return Json(locationData);

                case "transfers-by-month":
                    var transferData = await _context.Transfers
                        .Where(t => t.TransferDate >= DateTime.Now.AddMonths(-6))
                        .GroupBy(t => new { t.TransferDate.Year, t.TransferDate.Month })
                        .Select(g => new {
                            Month = $"{g.Key.Year}-{g.Key.Month:00}",
                            Count = g.Count()
                        })
                        .ToListAsync();
                    return Json(transferData);

                default:
                    return BadRequest("Invalid chart type");
            }
        }
    }
}
