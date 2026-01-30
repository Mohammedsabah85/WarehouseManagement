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
    /// Controller للمواد المستهلكة
    /// </summary>
    public class ConsumedMaterialsController : Controller
    {
        private readonly WarehouseContext _context;

        public ConsumedMaterialsController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: ConsumedMaterials
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ConsumptionReason? reason, CommitteeDecision? decision)
        {
            var query = _context.ConsumedMaterials
                .Include(c => c.Material)
                    .ThenInclude(m => m.Category)
                .Include(c => c.Material)
                    .ThenInclude(m => m.Location)
                .AsQueryable();

            // تطبيق الفلاتر
            if (startDate.HasValue)
            {
                query = query.Where(c => c.ConsumptionDate >= startDate.Value);
                ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            }

            if (endDate.HasValue)
            {
                query = query.Where(c => c.ConsumptionDate <= endDate.Value);
                ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            }

            if (reason.HasValue)
            {
                query = query.Where(c => c.Reason == reason.Value);
                ViewBag.SelectedReason = reason.Value;
            }

            if (decision.HasValue)
            {
                query = query.Where(c => c.Decision == decision.Value);
                ViewBag.SelectedDecision = decision.Value;
            }

            var consumedMaterials = await query
                .OrderByDescending(c => c.ConsumptionDate)
                .ToListAsync();

            return View(consumedMaterials);
        }

        // GET: ConsumedMaterials/Create
        public async Task<IActionResult> Create(int? materialId)
        {
            var model = new ConsumedMaterial
            {
                ConsumptionDate = DateTime.Now
            };

            if (materialId.HasValue)
            {
                var material = await _context.Materials.FindAsync(materialId.Value);
                if (material != null)
                {
                    model.MaterialId = material.Id;
                    model.OriginalUnitPrice = material.Price;
                    ViewBag.SelectedMaterial = material;

                    // الحصول على آخر سعر شراء
                    var lastPurchase = await _context.PurchasePrices
                        .Where(p => p.MaterialId == materialId.Value)
                        .OrderByDescending(p => p.PurchaseDate)
                        .FirstOrDefaultAsync();

                    if (lastPurchase != null)
                    {
                        model.OriginalPurchaseDate = lastPurchase.PurchaseDate;
                        model.OriginalUnitPrice = lastPurchase.UnitPrice;
                    }
                }
            }

            await LoadSelectLists();
            return View(model);
        }

        // POST: ConsumedMaterials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumedMaterial consumedMaterial)
        {
            if (ModelState.IsValid)
            {
                // التحقق من توفر الكمية
                var material = await _context.Materials.FindAsync(consumedMaterial.MaterialId);
                if (material == null)
                {
                    ModelState.AddModelError("", "المادة غير موجودة");
                    await LoadSelectLists();
                    return View(consumedMaterial);
                }

                if (material.Quantity < consumedMaterial.ConsumedQuantity)
                {
                    ModelState.AddModelError("ConsumedQuantity", $"الكمية المتوفرة هي {material.Quantity} فقط");
                    await LoadSelectLists();
                    return View(consumedMaterial);
                }

                // حساب القيمة الإجمالية
                if (consumedMaterial.OriginalUnitPrice.HasValue)
                {
                    consumedMaterial.OriginalTotalPrice = consumedMaterial.OriginalUnitPrice.Value * consumedMaterial.ConsumedQuantity;

                    // حساب القيمة المتبقية
                    if (consumedMaterial.DamagePercentage > 0)
                    {
                        consumedMaterial.ResidualValue = consumedMaterial.OriginalTotalPrice.Value *
                            (100 - consumedMaterial.DamagePercentage) / 100;
                    }
                }

                // حساب مدة الاستعمال
                if (consumedMaterial.OriginalPurchaseDate.HasValue)
                {
                    consumedMaterial.UsageDurationDays = (int)(consumedMaterial.ConsumptionDate - consumedMaterial.OriginalPurchaseDate.Value).TotalDays;
                }

                _context.Add(consumedMaterial);

                // خصم الكمية من المخزون إذا كان القرار إتلاف أو بيع
                if (consumedMaterial.Decision == CommitteeDecision.Dispose ||
                    consumedMaterial.Decision == CommitteeDecision.Sell ||
                    consumedMaterial.Decision == CommitteeDecision.Scrap)
                {
                    material.Quantity -= consumedMaterial.ConsumedQuantity;
                    material.LastUpdated = DateTime.Now;
                    consumedMaterial.IsDisposed = true;
                    consumedMaterial.DisposalDate = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم تسجيل المادة المستهلكة بنجاح";
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectLists();
            return View(consumedMaterial);
        }

        // GET: ConsumedMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumedMaterial = await _context.ConsumedMaterials
                .Include(c => c.Material)
                    .ThenInclude(m => m.Category)
                .Include(c => c.Material)
                    .ThenInclude(m => m.Location)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consumedMaterial == null)
            {
                return NotFound();
            }

            return View(consumedMaterial);
        }

        // GET: ConsumedMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumedMaterial = await _context.ConsumedMaterials.FindAsync(id);
            if (consumedMaterial == null)
            {
                return NotFound();
            }

            await LoadSelectLists();
            return View(consumedMaterial);
        }

        // POST: ConsumedMaterials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConsumedMaterial consumedMaterial)
        {
            if (id != consumedMaterial.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // حساب القيمة الإجمالية
                    if (consumedMaterial.OriginalUnitPrice.HasValue)
                    {
                        consumedMaterial.OriginalTotalPrice = consumedMaterial.OriginalUnitPrice.Value * consumedMaterial.ConsumedQuantity;

                        // حساب القيمة المتبقية
                        if (consumedMaterial.DamagePercentage > 0)
                        {
                            consumedMaterial.ResidualValue = consumedMaterial.OriginalTotalPrice.Value *
                                (100 - consumedMaterial.DamagePercentage) / 100;
                        }
                    }

                    _context.Update(consumedMaterial);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث البيانات بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumedMaterialExists(consumedMaterial.Id))
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
            return View(consumedMaterial);
        }

        // GET: ConsumedMaterials/Report
        public async Task<IActionResult> Report(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.Now.AddMonths(-1);
            endDate ??= DateTime.Now;

            var consumedMaterials = await _context.ConsumedMaterials
                .Include(c => c.Material)
                    .ThenInclude(m => m.Category)
                .Where(c => c.ConsumptionDate >= startDate && c.ConsumptionDate <= endDate)
                .ToListAsync();

            var viewModel = new ConsumedMaterialsReportViewModel
            {
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                ConsumedMaterials = consumedMaterials,
                TotalConsumedItems = consumedMaterials.Count,
                TotalOriginalValue = consumedMaterials.Sum(c => c.OriginalTotalPrice ?? 0),
                TotalResidualValue = consumedMaterials.Sum(c => c.ResidualValue ?? 0)
            };

            viewModel.TotalLoss = viewModel.TotalOriginalValue - viewModel.TotalResidualValue;

            // إحصائيات حسب السبب
            viewModel.ConsumptionByReason = consumedMaterials
                .GroupBy(c => c.Reason)
                .ToDictionary(g => g.Key, g => g.Count());

            // إحصائيات حسب القرار
            viewModel.DecisionStatistics = consumedMaterials
                .GroupBy(c => c.Decision)
                .ToDictionary(g => g.Key, g => g.Count());

            // المواد الأكثر استهلاكاً
            viewModel.TopConsumedMaterials = consumedMaterials
                .GroupBy(c => new { c.Material.Name, c.Material.Code })
                .Select(g => new MaterialConsumptionSummary
                {
                    MaterialName = g.Key.Name,
                    MaterialCode = g.Key.Code,
                    TotalConsumedQuantity = g.Sum(c => c.ConsumedQuantity),
                    TotalLossValue = g.Sum(c => (c.OriginalTotalPrice ?? 0) - (c.ResidualValue ?? 0)),
                    AverageDamagePercentage = g.Average(c => c.DamagePercentage),
                    ConsumptionCount = g.Count()
                })
                .OrderByDescending(s => s.TotalLossValue)
                .Take(10)
                .ToList();

            return View(viewModel);
        }

        // GET: ConsumedMaterials/ProcessDecision/5
        public async Task<IActionResult> ProcessDecision(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumedMaterial = await _context.ConsumedMaterials
                .Include(c => c.Material)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consumedMaterial == null)
            {
                return NotFound();
            }

            return View(consumedMaterial);
        }

        // POST: ConsumedMaterials/ProcessDecision/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessDecision(int id, string action, string disposalRecordNumber)
        {
            var consumedMaterial = await _context.ConsumedMaterials
                .Include(c => c.Material)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consumedMaterial == null)
            {
                return NotFound();
            }

            switch (action)
            {
                case "dispose":
                    // التصرف بالمادة
                    consumedMaterial.IsDisposed = true;
                    consumedMaterial.DisposalDate = DateTime.Now;
                    consumedMaterial.DisposalRecordNumber = disposalRecordNumber;

                    // خصم من المخزون إذا لم يتم خصمها مسبقاً
                    if (!consumedMaterial.IsDisposed)
                    {
                        consumedMaterial.Material.Quantity -= consumedMaterial.ConsumedQuantity;
                        consumedMaterial.Material.LastUpdated = DateTime.Now;
                    }
                    break;

                case "return":
                    // إعادة للمخزون (إصلاح أو إعادة استخدام)
                    if (consumedMaterial.IsDisposed)
                    {
                        consumedMaterial.Material.Quantity += consumedMaterial.ConsumedQuantity;
                        consumedMaterial.Material.LastUpdated = DateTime.Now;
                        consumedMaterial.IsDisposed = false;
                        consumedMaterial.DisposalDate = null;
                    }
                    break;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم تنفيذ الإجراء بنجاح";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: ConsumedMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumedMaterial = await _context.ConsumedMaterials
                .Include(c => c.Material)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consumedMaterial == null)
            {
                return NotFound();
            }

            return View(consumedMaterial);
        }

        // POST: ConsumedMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumedMaterial = await _context.ConsumedMaterials
                .Include(c => c.Material)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (consumedMaterial != null)
            {
                // إرجاع الكمية للمخزون إذا كانت قد خصمت
                if (consumedMaterial.IsDisposed)
                {
                    consumedMaterial.Material.Quantity += consumedMaterial.ConsumedQuantity;
                    consumedMaterial.Material.LastUpdated = DateTime.Now;
                }

                _context.ConsumedMaterials.Remove(consumedMaterial);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف السجل بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ConsumedMaterialExists(int id)
        {
            return _context.ConsumedMaterials.Any(e => e.Id == id);
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