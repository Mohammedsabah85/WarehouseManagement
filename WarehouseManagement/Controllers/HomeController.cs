using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using System.Diagnostics;

namespace WarehouseManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly WarehouseContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(WarehouseContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get dashboard statistics
            var totalMaterials = await _context.Materials.CountAsync();
            var totalLocations = await _context.Locations.CountAsync();
            var lowStockItems = await _context.Materials.CountAsync(m => m.Quantity <= 10);
            var expiringItems = await _context.Materials.CountAsync(m =>
                m.ExpiryDate.HasValue && m.ExpiryDate <= DateTime.Now.AddDays(30));

            ViewBag.TotalMaterials = totalMaterials;
            ViewBag.TotalLocations = totalLocations;
            ViewBag.LowStockItems = lowStockItems;
            ViewBag.ExpiringItems = expiringItems;

            // Get recent activities (last 10 transfers)
            var recentTransfers = await _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .OrderByDescending(t => t.TransferDate)
                .Take(10)
                .ToListAsync();

            ViewBag.RecentTransfers = recentTransfers;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}