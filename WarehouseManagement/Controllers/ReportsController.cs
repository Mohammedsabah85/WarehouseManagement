using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Controllers
{
    public class ReportsController : Controller
    {
        private readonly WarehouseContext _context;

        public ReportsController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: Reports/LocationReport/5
        public async Task<IActionResult> LocationReport(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            var materials = await _context.Materials
                .Include(m => m.Category)
                .Where(m => m.LocationId == id && m.Quantity > 0)
                .OrderBy(m => m.Name)
                .ToListAsync();

            var viewModel = new LocationReportViewModel
            {
                Location = location,
                Materials = materials,
                TotalItems = materials.Count,
                TotalQuantity = materials.Sum(m => m.Quantity)
            };

            return View(viewModel);
        }

        // GET: Reports/PrintLocationReport/5
        public async Task<IActionResult> PrintLocationReport(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }

            var materials = await _context.Materials
                .Include(m => m.Category)
                .Where(m => m.LocationId == id && m.Quantity > 0)
                .OrderBy(m => m.Name)
                .ToListAsync();

            var viewModel = new LocationReportViewModel
            {
                Location = location,
                Materials = materials,
                TotalItems = materials.Count,
                TotalQuantity = materials.Sum(m => m.Quantity)
            };

            return View(viewModel);
        }

        // GET: Reports/AllLocations
        public async Task<IActionResult> AllLocations()
        {
            var locations = await _context.Locations
                .Include(l => l.Materials.Where(m => m.Quantity > 0))
                .ThenInclude(m => m.Category)
                .OrderBy(l => l.Name)
                .ToListAsync();

            return View(locations);
        }

        // GET: Reports/LowStock
        public async Task<IActionResult> LowStock(int threshold = 10)
        {
            var lowStockMaterials = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.Quantity <= threshold)
                .OrderBy(m => m.Quantity)
                .ToListAsync();

            ViewBag.Threshold = threshold;
            return View(lowStockMaterials);
        }

        // GET: Reports/ExpiringItems
        public async Task<IActionResult> ExpiringItems(int days = 30)
        {
            var expiringDate = DateTime.Now.AddDays(days);
            var expiringMaterials = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.ExpiryDate.HasValue && m.ExpiryDate <= expiringDate)
                .OrderBy(m => m.ExpiryDate)
                .ToListAsync();

            ViewBag.Days = days;
            return View(expiringMaterials);
        }

        // GET: Reports/TransferHistory
        public async Task<IActionResult> TransferHistory(int? materialId, int? locationId, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .AsQueryable();

            if (materialId.HasValue)
                query = query.Where(t => t.MaterialId == materialId.Value);

            if (locationId.HasValue)
                query = query.Where(t => t.FromLocationId == locationId.Value || t.ToLocationId == locationId.Value);

            if (startDate.HasValue)
                query = query.Where(t => t.TransferDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TransferDate <= endDate.Value);

            var transfers = await query
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            ViewBag.Materials = await _context.Materials.OrderBy(m => m.Name).ToListAsync();
            ViewBag.Locations = await _context.Locations.OrderBy(l => l.Name).ToListAsync();
            ViewBag.MaterialId = materialId;
            ViewBag.LocationId = locationId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(transfers);
        }

        // GET: Reports/InventoryValue
        public async Task<IActionResult> InventoryValue()
        {
            var materials = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => m.Quantity > 0 && m.Price.HasValue)
                .OrderByDescending(m => m.Price * m.Quantity)
                .ToListAsync();

            var totalValue = materials.Sum(m => (m.Price ?? 0) * m.Quantity);
            var totalItems = materials.Sum(m => m.Quantity);

            ViewBag.TotalValue = totalValue;
            ViewBag.TotalItems = totalItems;
            ViewBag.AverageValue = materials.Any() ? totalValue / materials.Count : 0;

            return View(materials);
        }

        // GET: Reports/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalMaterials = await _context.Materials.CountAsync(),
                TotalLocations = await _context.Locations.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                LowStockItems = await _context.Materials.CountAsync(m => m.Quantity <= 10),
                ExpiringItems = await _context.Materials.CountAsync(m =>
                    m.ExpiryDate.HasValue && m.ExpiryDate <= DateTime.Now.AddDays(30)),
                TotalValue = await _context.Materials
                    .Where(m => m.Price.HasValue)
                    .SumAsync(m => m.Price.Value * m.Quantity),
                PendingTransfers = await _context.Transfers.CountAsync(t => !t.IsConfirmed),
                RecentTransfers = await _context.Transfers
                    .Include(t => t.Material)
                    .Include(t => t.FromLocation)
                    .Include(t => t.ToLocation)
                    .OrderByDescending(t => t.TransferDate)
                    .Take(5)
                    .ToListAsync()
            };

            return View(stats);
        }

        // GET: API للحصول على بيانات الرسوم البيانية
        [HttpGet]
        public async Task<IActionResult> GetChartData(string type)
        {
            try
            {
                switch (type.ToLower())
                {
                    case "materials-by-category":
                        var categoryData = await _context.Categories
                            .Select(c => new
                            {
                                name = c.Name,
                                count = c.Materials.Count(m => m.Quantity > 0)
                            })
                            .Where(x => x.count > 0)
                            .ToListAsync();
                        return Json(categoryData);

                    case "materials-by-location":
                        var locationData = await _context.Locations
                            .Select(l => new
                            {
                                name = l.Name,
                                count = l.Materials.Count(m => m.Quantity > 0)
                            })
                            .Where(x => x.count > 0)
                            .ToListAsync();
                        return Json(locationData);

                    case "transfers-by-month":
                        var transferData = await _context.Transfers
                            .Where(t => t.TransferDate >= DateTime.Now.AddMonths(-6))
                            .GroupBy(t => new { t.TransferDate.Year, t.TransferDate.Month })
                            .Select(g => new
                            {
                                month = $"{g.Key.Year}-{g.Key.Month:00}",
                                count = g.Count()
                            })
                            .OrderBy(x => x.month)
                            .ToListAsync();
                        return Json(transferData);

                    case "low-stock-alert":
                        var lowStockData = await _context.Materials
                            .Include(m => m.Location)
                            .Where(m => m.Quantity <= 10)
                            .Select(m => new
                            {
                                name = m.Name,
                                quantity = m.Quantity,
                                location = m.Location.Name
                            })
                            .ToListAsync();
                        return Json(lowStockData);

                    default:
                        return BadRequest("نوع البيانات غير مدعوم");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "خطأ في جلب البيانات", details = ex.Message });
            }
        }
    }
}