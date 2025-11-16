using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Controllers
{
    public class ExportController : Controller
    {
        private readonly WarehouseContext _context;

        public ExportController(WarehouseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new ExportViewModel
            {
                Locations = await _context.Locations.OrderBy(l => l.Name).ToListAsync(),
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ExportMaterials(ExportViewModel model)
        {
            var query = _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .AsQueryable();

            if (model.LocationId.HasValue)
                query = query.Where(m => m.LocationId == model.LocationId.Value);

            if (model.CategoryId.HasValue)
                query = query.Where(m => m.CategoryId == model.CategoryId.Value);

            if (!model.IncludeZeroQuantity)
                query = query.Where(m => m.Quantity > 0);

            var materials = await query.OrderBy(m => m.Name).ToListAsync();

            switch (model.ExportType.ToLower())
            {
                case "csv":
                    return ExportToCsv(materials);
                case "excel":
                    return ExportToExcel(materials);
                default:
                    return BadRequest("نوع التصدير غير مدعوم");
            }
        }

        private IActionResult ExportToCsv(List<Material> materials)
        {
            var csv = new StringBuilder();
            csv.AppendLine("رمز المادة,اسم المادة,الفئة,الموقع,الكمية,الوحدة,السعر,تاريخ انتهاء الصلاحية,ملاحظات");

            foreach (var material in materials)
            {
                csv.AppendLine($"{material.Code},{material.Name},{material.Category.Name},{material.Location.Name},{material.Quantity},{material.Unit},{material.Price},{material.ExpiryDate:yyyy-MM-dd},{material.Notes}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"materials-export-{DateTime.Now:yyyy-MM-dd}.csv");
        }

        private IActionResult ExportToExcel(List<Material> materials)
        {
            // يمكن استخدام مكتبة مثل EPPlus أو ClosedXML
            // هذا مثال أساسي باستخدام CSV بتنسيق Excel
            var content = new StringBuilder();
            content.AppendLine("رمز المادة\tاسم المادة\tالفئة\tالموقع\tالكمية\tالوحدة\tالسعر\tتاريخ انتهاء الصلاحية\tملاحظات");

            foreach (var material in materials)
            {
                content.AppendLine($"{material.Code}\t{material.Name}\t{material.Category.Name}\t{material.Location.Name}\t{material.Quantity}\t{material.Unit}\t{material.Price}\t{material.ExpiryDate:yyyy-MM-dd}\t{material.Notes}");
            }

            var bytes = Encoding.UTF8.GetBytes(content.ToString());
            return File(bytes, "application/vnd.ms-excel", $"materials-export-{DateTime.Now:yyyy-MM-dd}.xls");
        }

        [HttpPost]
        public async Task<IActionResult> ExportTransfers(ExportViewModel model)
        {
            var query = _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .AsQueryable();

            if (model.StartDate.HasValue)
                query = query.Where(t => t.TransferDate >= model.StartDate.Value);

            if (model.EndDate.HasValue)
                query = query.Where(t => t.TransferDate <= model.EndDate.Value);

            var transfers = await query.OrderByDescending(t => t.TransferDate).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("رقم العملية,المادة,من الموقع,إلى الموقع,الكمية,تاريخ النقل,المسؤول,مؤكد,السبب");

            foreach (var transfer in transfers)
            {
                csv.AppendLine($"{transfer.Id},{transfer.Material.Name},{transfer.FromLocation.Name},{transfer.ToLocation.Name},{transfer.Quantity},{transfer.TransferDate:yyyy-MM-dd HH:mm},{transfer.TransferredBy},{(transfer.IsConfirmed ? "نعم" : "لا")},{transfer.Reason}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", $"transfers-export-{DateTime.Now:yyyy-MM-dd}.csv");
        }
    }
}
