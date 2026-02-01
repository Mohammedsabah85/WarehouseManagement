using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller لدفعات الشراء - تفصيل أسعار الشراء حسب السنوات
    /// </summary>
    public class PurchaseBatchesController : Controller
    {
        private readonly WarehouseContext _context;

        public PurchaseBatchesController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: PurchaseBatches
        public async Task<IActionResult> Index(int? form2ItemId, int? year)
        {
            var query = _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .AsQueryable();

            if (form2ItemId.HasValue)
            {
                query = query.Where(p => p.Form2ItemId == form2ItemId.Value);
                ViewBag.Form2Item = await _context.Form2Items.FindAsync(form2ItemId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(p => p.PurchaseYear == year.Value);
            }

            var batches = await query
                .OrderBy(p => p.Form2ItemId)
                .ThenBy(p => p.PurchaseYear)
                .ToListAsync();

            // إحصائيات
            ViewBag.TotalBatches = batches.Count;
            ViewBag.TotalQuantity = batches.Sum(b => b.Quantity);
            ViewBag.TotalValue = batches.Sum(b => b.TotalPrice);
            ViewBag.AveragePrice = batches.Any() ? batches.Average(b => b.UnitPrice) : 0;

            // قوائم الفلاتر
            ViewBag.Years = await _context.PurchaseBatches
                .Select(p => p.PurchaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            ViewBag.Form2Items = await _context.Form2Items
                .OrderBy(f => f.MaterialName)
                .Select(f => new { f.Id, f.MaterialName, f.CodeNumber })
                .ToListAsync();

            return View(batches);
        }

        // GET: PurchaseBatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var batch = await _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (batch == null)
            {
                return NotFound();
            }

            return View(batch);
        }

        // GET: PurchaseBatches/Create
        public async Task<IActionResult> Create(int? form2ItemId)
        {
            var batch = new PurchaseBatch
            {
                PurchaseYear = DateTime.Now.Year
            };

            if (form2ItemId.HasValue)
            {
                var form2Item = await _context.Form2Items.FindAsync(form2ItemId.Value);
                if (form2Item != null)
                {
                    batch.Form2ItemId = form2Item.Id;
                    batch.MaterialName = form2Item.MaterialName;
                    batch.CodeNumber = form2Item.CodeNumber;
                    ViewBag.Form2Item = form2Item;
                }
            }

            await LoadSelectLists();
            return View(batch);
        }

        // POST: PurchaseBatches/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseBatch batch)
        {
            if (ModelState.IsValid)
            {
                // تحديد رقم التسلسل
                var maxSequence = await _context.PurchaseBatches
                    .Where(p => p.Form2ItemId == batch.Form2ItemId)
                    .MaxAsync(p => (int?)p.SequenceNumber) ?? 0;

                batch.SequenceNumber = maxSequence + 1;
                batch.StoredTotalPrice = batch.TotalPrice;
                batch.CreatedDate = DateTime.Now;

                // تحديث اسم المادة والترميز من Form2Item
                var form2Item = await _context.Form2Items.FindAsync(batch.Form2ItemId);
                if (form2Item != null)
                {
                    batch.MaterialName = form2Item.MaterialName;
                    batch.CodeNumber = form2Item.CodeNumber;
                }

                _context.PurchaseBatches.Add(batch);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة دفعة الشراء بنجاح";
                return RedirectToAction(nameof(Index), new { form2ItemId = batch.Form2ItemId });
            }

            await LoadSelectLists();
            return View(batch);
        }

        // GET: PurchaseBatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var batch = await _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (batch == null)
            {
                return NotFound();
            }

            await LoadSelectLists();
            return View(batch);
        }

        // POST: PurchaseBatches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseBatch batch)
        {
            if (id != batch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    batch.StoredTotalPrice = batch.TotalPrice;
                    _context.Update(batch);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث البيانات بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseBatchExists(batch.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index), new { form2ItemId = batch.Form2ItemId });
            }

            await LoadSelectLists();
            return View(batch);
        }

        // GET: PurchaseBatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var batch = await _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (batch == null)
            {
                return NotFound();
            }

            return View(batch);
        }

        // POST: PurchaseBatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var batch = await _context.PurchaseBatches.FindAsync(id);
            if (batch != null)
            {
                var form2ItemId = batch.Form2ItemId;
                _context.PurchaseBatches.Remove(batch);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف دفعة الشراء بنجاح";
                return RedirectToAction(nameof(Index), new { form2ItemId });
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: PurchaseBatches/Report
        public async Task<IActionResult> Report(int? form2ItemId)
        {
            var query = _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .AsQueryable();

            if (form2ItemId.HasValue)
            {
                query = query.Where(p => p.Form2ItemId == form2ItemId.Value);
            }

            var batches = await query
                .OrderBy(p => p.Form2Item.MaterialName)
                .ThenBy(p => p.PurchaseYear)
                .ToListAsync();

            // تجميع حسب المادة
            var groupedByMaterial = batches
                .GroupBy(b => new { b.Form2ItemId, b.MaterialName, b.CodeNumber })
                .Select(g => new
                {
                    Form2ItemId = g.Key.Form2ItemId,
                    MaterialName = g.Key.MaterialName,
                    CodeNumber = g.Key.CodeNumber,
                    Batches = g.ToList(),
                    TotalQuantity = g.Sum(b => b.Quantity),
                    TotalValue = g.Sum(b => b.TotalPrice),
                    AveragePrice = g.Average(b => b.UnitPrice)
                })
                .ToList();

            ViewBag.GroupedData = groupedByMaterial;
            ViewBag.TotalBatches = batches.Count;
            ViewBag.GrandTotalQuantity = batches.Sum(b => b.Quantity);
            ViewBag.GrandTotalValue = batches.Sum(b => b.TotalPrice);

            return View(batches);
        }

        // GET: PurchaseBatches/YearlyReport
        public async Task<IActionResult> YearlyReport(int? year)
        {
            year ??= DateTime.Now.Year;

            var batches = await _context.PurchaseBatches
                .Include(p => p.Form2Item)
                .Where(p => p.PurchaseYear == year)
                .OrderBy(p => p.Form2Item.MaterialName)
                .ToListAsync();

            ViewBag.Year = year;
            ViewBag.TotalBatches = batches.Count;
            ViewBag.TotalQuantity = batches.Sum(b => b.Quantity);
            ViewBag.TotalValue = batches.Sum(b => b.TotalPrice);

            // السنوات المتاحة
            ViewBag.AvailableYears = await _context.PurchaseBatches
                .Select(p => p.PurchaseYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(batches);
        }

        private bool PurchaseBatchExists(int id)
        {
            return _context.PurchaseBatches.Any(e => e.Id == id);
        }

        private async Task LoadSelectLists()
        {
            ViewData["Form2ItemId"] = new SelectList(
                await _context.Form2Items
                    .OrderBy(f => f.MaterialName)
                    .Select(f => new { f.Id, Display = $"{f.MaterialName} ({f.CodeNumber})" })
                    .ToListAsync(),
                "Id", "Display");
        }
    }
}
