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
    /// Controller للجرد السنوي
    /// </summary>
    public class InventoryController : Controller
    {
        private readonly WarehouseContext _context;

        public InventoryController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public async Task<IActionResult> Index(int? year)
        {
            year ??= DateTime.Now.Year;

            var inventories = await _context.AnnualInventories
                .Include(i => i.Material)
                    .ThenInclude(m => m.Category)
                .Include(i => i.Material)
                    .ThenInclude(m => m.Location)
                .Where(i => i.Year == year)
                .OrderBy(i => i.Material.Category.Name)
                .ThenBy(i => i.Material.Name)
                .ToListAsync();

            ViewBag.SelectedYear = year;
            ViewBag.AvailableYears = await _context.AnnualInventories
                .Select(i => i.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(inventories);
        }

        // GET: Inventory/Create
        public async Task<IActionResult> Create()
        {
            await LoadSelectLists();
            var model = new AnnualInventory
            {
                Year = DateTime.Now.Year,
                InventoryDate = DateTime.Now
            };
            return View(model);
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnnualInventory inventory)
        {
            if (ModelState.IsValid)
            {
                // التحقق من عدم وجود جرد مسبق لنفس المادة في نفس السنة
                var existingInventory = await _context.AnnualInventories
                    .FirstOrDefaultAsync(i => i.MaterialId == inventory.MaterialId && i.Year == inventory.Year);

                if (existingInventory != null)
                {
                    ModelState.AddModelError("", "يوجد جرد مسبق لهذه المادة في نفس السنة");
                    await LoadSelectLists();
                    return View(inventory);
                }

                // الحصول على الكمية الحالية من السجلات
                var material = await _context.Materials.FindAsync(inventory.MaterialId);
                if (material != null)
                {
                    inventory.RecordedQuantity = material.Quantity;
                }

                _context.Add(inventory);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة الجرد بنجاح";
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectLists();
            return View(inventory);
        }

        // GET: Inventory/StartInventory
        public async Task<IActionResult> StartInventory(int? year)
        {
            year ??= DateTime.Now.Year;

            // الحصول على جميع المواد التي لم يتم جردها بعد
            var inventoriedMaterialIds = await _context.AnnualInventories
                .Where(i => i.Year == year)
                .Select(i => i.MaterialId)
                .ToListAsync();

            var materials = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .Where(m => !inventoriedMaterialIds.Contains(m.Id))
                .OrderBy(m => m.Category.Name)
                .ThenBy(m => m.Name)
                .ToListAsync();

            ViewBag.Year = year;
            return View(materials);
        }

        // POST: Inventory/ProcessInventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessInventory(InventoryProcessViewModel model)
        {
            if (model.InventoryItems == null || !model.InventoryItems.Any())
            {
                TempData["Error"] = "لا توجد مواد للجرد";
                return RedirectToAction(nameof(StartInventory));
            }

            foreach (var item in model.InventoryItems.Where(i => i.IsSelected))
            {
                var inventory = new AnnualInventory
                {
                    MaterialId = item.MaterialId,
                    Year = model.Year,
                    ActualQuantity = item.ActualQuantity,
                    RecordedQuantity = item.RecordedQuantity,
                    Condition = item.Condition,
                    Department = item.Department,
                    ActualLocation = item.ActualLocation,
                    InventoryDate = DateTime.Now,
                    InventoryBy = model.InventoryBy,
                    Notes = item.Notes
                };

                _context.AnnualInventories.Add(inventory);

                // تحديث كمية المادة في حال وجود فرق
                if (item.ActualQuantity != item.RecordedQuantity)
                {
                    var material = await _context.Materials.FindAsync(item.MaterialId);
                    if (material != null)
                    {
                        material.Quantity = item.ActualQuantity;
                        material.LastUpdated = DateTime.Now;
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم إتمام عملية الجرد بنجاح";
            return RedirectToAction(nameof(Index), new { year = model.Year });
        }

        // GET: Inventory/Report
        public async Task<IActionResult> Report(int? year)
        {
            year ??= DateTime.Now.Year;

            var inventories = await _context.AnnualInventories
                .Include(i => i.Material)
                    .ThenInclude(m => m.Category)
                .Include(i => i.Material)
                    .ThenInclude(m => m.Location)
                .Where(i => i.Year == year)
                .OrderBy(i => i.Material.Category.Name)
                .ThenBy(i => i.Material.Name)
                .ToListAsync();

            // حساب الإحصائيات
            var viewModel = new InventoryReportViewModel
            {
                Year = year.Value,
                Inventories = inventories,
                TotalItems = inventories.Count,
                ItemsWithShortage = inventories.Count(i => i.Difference < 0),
                ItemsWithSurplus = inventories.Count(i => i.Difference > 0),
                ItemsMatching = inventories.Count(i => i.Difference == 0),
                TotalShortageQuantity = inventories.Where(i => i.Difference < 0).Sum(i => Math.Abs(i.Difference)),
                TotalSurplusQuantity = inventories.Where(i => i.Difference > 0).Sum(i => i.Difference)
            };

            // حساب القيمة الإجمالية
            decimal totalValue = 0;
            foreach (var inv in inventories)
            {
                if (inv.Material.Price.HasValue)
                {
                    totalValue += inv.Material.Price.Value * inv.ActualQuantity;
                }
            }
            viewModel.TotalValue = totalValue;

            return View(viewModel);
        }

        // GET: Inventory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.AnnualInventories
                .Include(i => i.Material)
                    .ThenInclude(m => m.Category)
                .Include(i => i.Material)
                    .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // GET: Inventory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.AnnualInventories.FindAsync(id);
            if (inventory == null)
            {
                return NotFound();
            }

            await LoadSelectLists();
            return View(inventory);
        }

        // POST: Inventory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnnualInventory inventory)
        {
            if (id != inventory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventory);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث الجرد بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryExists(inventory.Id))
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
            return View(inventory);
        }

        // GET: Inventory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventory = await _context.AnnualInventories
                .Include(i => i.Material)
                    .ThenInclude(m => m.Category)
                .Include(i => i.Material)
                    .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        // POST: Inventory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventory = await _context.AnnualInventories.FindAsync(id);
            if (inventory != null)
            {
                _context.AnnualInventories.Remove(inventory);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف الجرد بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InventoryExists(int id)
        {
            return _context.AnnualInventories.Any(e => e.Id == id);
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