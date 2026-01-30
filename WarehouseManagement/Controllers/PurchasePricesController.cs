using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;
using System.Linq;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller لأسعار الشراء
    /// </summary>
    public class PurchasePricesController : Controller
    {
        private readonly WarehouseContext _context;

        public PurchasePricesController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: PurchasePrices
        public async Task<IActionResult> Index(int? materialId, int? year, AcquisitionMethod? method)
        {
            var query = _context.PurchasePrices
                .Include(p => p.Material)
                    .ThenInclude(m => m.Category)
                .AsQueryable();

            // تطبيق الفلاتر
            if (materialId.HasValue)
            {
                query = query.Where(p => p.MaterialId == materialId.Value);
                ViewBag.SelectedMaterialId = materialId.Value;
            }

            if (year.HasValue)
            {
                query = query.Where(p => p.PurchaseYear == year.Value);
                ViewBag.SelectedYear = year.Value;
            }

            if (method.HasValue)
            {
                query = query.Where(p => p.Method == method.Value);
                ViewBag.SelectedMethod = method.Value;
            }

            var purchasePrices = await query
                .OrderByDescending(p => p.PurchaseYear)
                .ThenBy(p => p.Material.Name)
                .ToListAsync();

            // تحميل قوائم الاختيار
            ViewBag.Materials = new SelectList(
                await _context.Materials.OrderBy(m => m.Name).ToListAsync(),
                "Id", "Name", materialId);

            ViewBag.Years = await _context.PurchasePrices
                .Select(p => p.PurchaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(purchasePrices);
        }

        // GET: PurchasePrices/Create
        public async Task<IActionResult> Create(int? materialId)
        {
            var model = new PurchasePrice
            {
                PurchaseYear = DateTime.Now.Year,
                PurchaseDate = DateTime.Now
            };

            if (materialId.HasValue)
            {
                var material = await _context.Materials.FindAsync(materialId.Value);
                if (material != null)
                {
                    model.MaterialId = material.Id;
                    ViewBag.SelectedMaterial = material;
                }
            }

            await LoadSelectLists();
            return View(model);
        }

        // POST: PurchasePrices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchasePrice purchasePrice)
        {
            if (ModelState.IsValid)
            {
                // تحديث سنة الشراء من التاريخ
                if (purchasePrice.PurchaseDate.HasValue)
                {
                    purchasePrice.PurchaseYear = purchasePrice.PurchaseDate.Value.Year;
                }

                _context.Add(purchasePrice);

                // إضافة الكمية للمخزون إذا كانت عملية شراء أو مناقلة
                if (purchasePrice.Method == AcquisitionMethod.Purchase ||
                    purchasePrice.Method == AcquisitionMethod.Transfer)
                {
                    var material = await _context.Materials.FindAsync(purchasePrice.MaterialId);
                    if (material != null)
                    {
                        material.Quantity += purchasePrice.Quantity;
                        material.LastUpdated = DateTime.Now;

                        // تحديث السعر إذا كان أحدث شراء
                        var isLatest = !await _context.PurchasePrices
                            .AnyAsync(p => p.MaterialId == purchasePrice.MaterialId &&
                                          p.PurchaseDate > purchasePrice.PurchaseDate);

                        if (isLatest)
                        {
                            material.Price = purchasePrice.UnitPrice;
                        }
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم تسجيل عملية الشراء بنجاح";
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectLists();
            return View(purchasePrice);
        }

        // GET: PurchasePrices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchasePrice = await _context.PurchasePrices
                .Include(p => p.Material)
                    .ThenInclude(m => m.Category)
                .Include(p => p.Material)
                    .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchasePrice == null)
            {
                return NotFound();
            }

            return View(purchasePrice);
        }

        // GET: PurchasePrices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchasePrice = await _context.PurchasePrices.FindAsync(id);
            if (purchasePrice == null)
            {
                return NotFound();
            }

            await LoadSelectLists();
            return View(purchasePrice);
        }

        // POST: PurchasePrices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchasePrice purchasePrice)
        {
            if (id != purchasePrice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // تحديث سنة الشراء من التاريخ
                    if (purchasePrice.PurchaseDate.HasValue)
                    {
                        purchasePrice.PurchaseYear = purchasePrice.PurchaseDate.Value.Year;
                    }

                    _context.Update(purchasePrice);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث البيانات بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchasePriceExists(purchasePrice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectLists();
            return View(purchasePrice);
        }

        // GET: PurchasePrices/Report
        public async Task<IActionResult> Report(int? materialId)
        {
            List<PricingReportViewModel> reportData = new();

            IQueryable<Material> materialsQuery = _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location);

            if (materialId.HasValue)
            {
                materialsQuery = materialsQuery.Where(m => m.Id == materialId.Value);
            }

            var materials = await materialsQuery.ToListAsync();

            foreach (var material in materials)
            {
                var purchasePrices = await _context.PurchasePrices
                    .Where(p => p.MaterialId == material.Id)
                    .OrderBy(p => p.PurchaseYear)
                    .ToListAsync();

                if (purchasePrices.Any())
                {
                    var report = new PricingReportViewModel
                    {
                        Material = material,
                        PurchasePrices = purchasePrices,
                        TotalQuantityPurchased = purchasePrices.Sum(p => p.Quantity),
                        TotalAmountSpent = purchasePrices.Sum(p => p.TotalPrice),
                        FirstPurchaseDate = purchasePrices.Min(p => p.PurchaseDate),
                        LastPurchaseDate = purchasePrices.Max(p => p.PurchaseDate)
                    };

                    report.AveragePrice = report.TotalQuantityPurchased > 0
                        ? report.TotalAmountSpent / report.TotalQuantityPurchased
                        : 0;

                    report.CurrentValue = material.Quantity * (material.Price ?? report.AveragePrice);

                    reportData.Add(report);
                }
            }

            ViewBag.Materials = new SelectList(
                await _context.Materials.OrderBy(m => m.Name).ToListAsync(),
                "Id", "Name", materialId);

            return View(reportData);
        }

        // GET: PurchasePrices/YearlyReport
        public async Task<IActionResult> YearlyReport(int? year)
        {
            year ??= DateTime.Now.Year;

            var purchasePrices = await _context.PurchasePrices
                .Include(p => p.Material)
                    .ThenInclude(m => m.Category)
                .Where(p => p.PurchaseYear == year)
                .OrderBy(p => p.Material.Category.Name)
                .ThenBy(p => p.Material.Name)
                .ToListAsync();

            // إحصائيات السنة
            ViewBag.Year = year;
            ViewBag.TotalPurchases = purchasePrices.Count;
            ViewBag.TotalQuantity = purchasePrices.Sum(p => p.Quantity);
            ViewBag.TotalAmount = purchasePrices.Sum(p => p.TotalPrice);
            ViewBag.UniqueItems = purchasePrices.Select(p => p.MaterialId).Distinct().Count();

            // إحصائيات حسب طريقة الحصول
            ViewBag.MethodStatistics = purchasePrices
                .GroupBy(p => p.Method)
                .Select(g => new { Method = g.Key, Count = g.Count(), Total = g.Sum(p => p.TotalPrice) })
                .ToList();

            // إحصائيات حسب الفئة
            ViewBag.CategoryStatistics = purchasePrices
                .GroupBy(p => p.Material.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count(), Total = g.Sum(p => p.TotalPrice) })
                .OrderByDescending(c => c.Total)
                .ToList();

            // السنوات المتاحة
            ViewBag.AvailableYears = await _context.PurchasePrices
                .Select(p => p.PurchaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(purchasePrices);
        }

        // GET: PurchasePrices/ImportFromTransfers
        public async Task<IActionResult> ImportFromTransfers()
        {
            var transfers = await _context.Transfers
                .Include(t => t.Material)
                .Where(t => t.IsConfirmed &&
                           !_context.PurchasePrices.Any(p =>
                               p.MaterialId == t.MaterialId &&
                               p.TransferSource == "نقل رقم " + t.Id))
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            return View(transfers);
        }

        // POST: PurchasePrices/ImportFromTransfers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportFromTransfers(int[] selectedTransfers)
        {
            if (selectedTransfers == null || !selectedTransfers.Any())
            {
                TempData["Error"] = "الرجاء اختيار المناقلات المراد استيرادها";
                return RedirectToAction(nameof(ImportFromTransfers));
            }

            int importedCount = 0;
            foreach (var transferId in selectedTransfers)
            {
                var transfer = await _context.Transfers
                    .Include(t => t.Material)
                    .Include(t => t.FromLocation)
                    .FirstOrDefaultAsync(t => t.Id == transferId);

                if (transfer != null)
                {
                    var purchasePrice = new PurchasePrice
                    {
                        MaterialId = transfer.MaterialId,
                        PurchaseYear = transfer.TransferDate.Year,
                        PurchaseDate = transfer.TransferDate,
                        Quantity = transfer.Quantity,
                        UnitPrice = transfer.Material.Price ?? 0,
                        Method = AcquisitionMethod.Transfer,
                        TransferSource = $"نقل رقم {transfer.Id} من {transfer.FromLocation.Name}",
                        Notes = $"مناقلة بواسطة {transfer.TransferredBy} - {transfer.Reason}",
                        Currency = "IQD",
                        ExchangeRate = 1
                    };

                    _context.PurchasePrices.Add(purchasePrice);
                    importedCount++;
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"تم استيراد {importedCount} عملية مناقلة بنجاح";
            return RedirectToAction(nameof(Index));
        }

        // GET: PurchasePrices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchasePrice = await _context.PurchasePrices
                .Include(p => p.Material)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchasePrice == null)
            {
                return NotFound();
            }

            return View(purchasePrice);
        }

        // POST: PurchasePrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchasePrice = await _context.PurchasePrices.FindAsync(id);
            if (purchasePrice != null)
            {
                _context.PurchasePrices.Remove(purchasePrice);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف السجل بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PurchasePriceExists(int id)
        {
            return _context.PurchasePrices.Any(e => e.Id == id);
        }

        private async Task LoadSelectLists()
        {
            ViewData["MaterialId"] = new SelectList(
                await _context.Materials.OrderBy(m => m.Name).ToListAsync(),
                "Id",
                "Name");
        }
    }
}