using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;
using WarehouseManagement.Services;

namespace WarehouseManagement.Controllers
{
    /// <summary>
    /// Controller لنموذج رقم (2) - قائمة الموجودات الرئيسية
    /// </summary>
    public class Form2Controller : Controller
    {
        private readonly WarehouseContext _context;
        private readonly IFormDataService _formDataService;

        public Form2Controller(WarehouseContext context, IFormDataService formDataService)
        {
            _context = context;
            _formDataService = formDataService;
        }

        // GET: Form2
        public async Task<IActionResult> Index(int? year, string? department, string? searchTerm, 
            string? conditionFilter, string? ownershipFilter)
        {
            year ??= DateTime.Now.Year;

            var query = _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .Include(f => f.Form5Items)
                .Where(f => f.InventoryYear == year)
                .AsQueryable();

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(f => f.Department == department);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => 
                    f.MaterialName.Contains(searchTerm) || 
                    f.CodeNumber.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(conditionFilter))
            {
                query = query.Where(f => f.Condition == conditionFilter);
            }

            if (!string.IsNullOrEmpty(ownershipFilter))
            {
                query = query.Where(f => f.Ownership == ownershipFilter);
            }

            var items = await query
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            // حساب الإحصائيات
            var statistics = new Form2Statistics
            {
                TotalItems = items.Count,
                TotalQuantityByInventory = items.Sum(i => i.QuantityByInventory),
                TotalQuantityByRecords = items.Sum(i => i.QuantityByRecords),
                TotalDifference = items.Sum(i => i.QuantityDifference),
                TotalCost = items.Sum(i => i.CostInDinar),
                ItemsWithShortage = items.Count(i => i.QuantityDifference < 0),
                ItemsWithSurplus = items.Count(i => i.QuantityDifference > 0),
                ItemsMatching = items.Count(i => i.QuantityDifference == 0),
                ItemsInForm5 = items.Count(i => i.IsDocumentedInForm5)
            };

            var viewModel = new Form2ViewModel
            {
                Year = year.Value,
                Department = department,
                Items = items,
                Statistics = statistics,
                SearchTerm = searchTerm,
                ConditionFilter = conditionFilter,
                OwnershipFilter = ownershipFilter
            };

            // قوائم الفلاتر
            ViewBag.Years = await _context.Form2Items
                .Select(f => f.InventoryYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            ViewBag.Departments = await _context.Form2Items
                .Where(f => f.Department != null)
                .Select(f => f.Department)
                .Distinct()
                .ToListAsync();

            ViewBag.Conditions = new List<string> { "ممتازة", "جيدة", "جيد", "متوسطة", "ضعيفة", "تالفة" };

            ViewBag.Ownerships = await _context.Form2Items
                .Where(f => f.Ownership != null)
                .Select(f => f.Ownership)
                .Distinct()
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Form2/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form2Items
                .Include(f => f.PurchaseBatches.OrderBy(p => p.PurchaseYear))
                .Include(f => f.Form5Items)
                .Include(f => f.Material)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Form2/Create
        public IActionResult Create()
        {
            var model = new Form2CreateEditViewModel
            {
                Item = new Form2Item
                {
                    InventoryYear = DateTime.Now.Year,
                    InventoryDate = DateTime.Now,
                    Condition = "جيدة"
                }
            };

            LoadSelectLists();
            return View(model);
        }

        // POST: Form2/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Form2CreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // التحقق من عدم تكرار رقم الترميز
                var existingCode = await _context.Form2Items
                    .AnyAsync(f => f.CodeNumber == model.Item.CodeNumber && 
                                   f.InventoryYear == model.Item.InventoryYear);

                if (existingCode)
                {
                    ModelState.AddModelError("Item.CodeNumber", 
                        "رقم الترميز موجود مسبقاً لهذه السنة");
                    LoadSelectLists();
                    return View(model);
                }

                // تحديد رقم التسلسل
                var maxSequence = await _context.Form2Items
                    .Where(f => f.InventoryYear == model.Item.InventoryYear)
                    .MaxAsync(f => (int?)f.SequenceNumber) ?? 0;
                
                model.Item.SequenceNumber = maxSequence + 1;
                model.Item.StoredDifference = model.Item.QuantityDifference;
                model.Item.CreatedDate = DateTime.Now;
                model.Item.LastUpdated = DateTime.Now;

                _context.Form2Items.Add(model.Item);

                // إضافة دفعات الشراء إذا وجدت
                if (model.CreateWithPurchaseBatches && model.PurchaseBatches.Any())
                {
                    foreach (var batch in model.PurchaseBatches.Where(b => b.IsSelected))
                    {
                        var purchaseBatch = new PurchaseBatch
                        {
                            Form2Item = model.Item,
                            MaterialName = model.Item.MaterialName,
                            CodeNumber = model.Item.CodeNumber,
                            PurchaseYear = batch.PurchaseYear,
                            Quantity = batch.Quantity,
                            UnitPrice = batch.UnitPrice,
                            StoredTotalPrice = batch.TotalPrice,
                            Notes = batch.Notes,
                            CreatedDate = DateTime.Now
                        };
                        _context.PurchaseBatches.Add(purchaseBatch);
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة العنصر بنجاح";
                return RedirectToAction(nameof(Index), new { year = model.Item.InventoryYear });
            }

            LoadSelectLists();
            return View(model);
        }

        // GET: Form2/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var model = new Form2CreateEditViewModel
            {
                Item = item,
                PurchaseBatches = item.PurchaseBatches.Select(p => new PurchaseBatchViewModel
                {
                    Id = p.Id,
                    PurchaseYear = p.PurchaseYear,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    Notes = p.Notes,
                    IsSelected = true
                }).ToList()
            };

            LoadSelectLists();
            return View(model);
        }

        // POST: Form2/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Form2CreateEditViewModel model)
        {
            if (id != model.Item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    model.Item.StoredDifference = model.Item.QuantityDifference;
                    model.Item.LastUpdated = DateTime.Now;

                    _context.Update(model.Item);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث البيانات بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Form2ItemExists(model.Item.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index), new { year = model.Item.InventoryYear });
            }

            LoadSelectLists();
            return View(model);
        }

        // GET: Form2/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .Include(f => f.Form5Items)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Form2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .Include(f => f.Form5Items)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item != null)
            {
                // التحقق من عدم وجود ارتباطات
                if (item.Form5Items.Any())
                {
                    TempData["Error"] = "لا يمكن حذف العنصر لأنه مرتبط بنموذج 5";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // حذف دفعات الشراء المرتبطة
                _context.PurchaseBatches.RemoveRange(item.PurchaseBatches);
                _context.Form2Items.Remove(item);

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف العنصر بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Form2/Report
        public async Task<IActionResult> Report(int? year, string? department)
        {
            year ??= DateTime.Now.Year;

            var items = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .Where(f => f.InventoryYear == year)
                .Where(f => string.IsNullOrEmpty(department) || f.Department == department)
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            var statistics = new Form2Statistics
            {
                TotalItems = items.Count,
                TotalQuantityByInventory = items.Sum(i => i.QuantityByInventory),
                TotalQuantityByRecords = items.Sum(i => i.QuantityByRecords),
                TotalDifference = items.Sum(i => i.QuantityDifference),
                TotalCost = items.Sum(i => i.CostInDinar),
                ItemsWithShortage = items.Count(i => i.QuantityDifference < 0),
                ItemsWithSurplus = items.Count(i => i.QuantityDifference > 0),
                ItemsMatching = items.Count(i => i.QuantityDifference == 0),
                ItemsInForm5 = items.Count(i => i.IsDocumentedInForm5)
            };

            var viewModel = new Form2ReportViewModel
            {
                Year = year.Value,
                Department = department ?? "قسم الهندسة الكهروميكانيكية",
                Items = items,
                Statistics = statistics,
                ReportDate = DateTime.Now
            };

            return View(viewModel);
        }

        // GET: Form2/PrintReport
        public async Task<IActionResult> PrintReport(int? year, string? department)
        {
            year ??= DateTime.Now.Year;

            var items = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .Where(f => f.InventoryYear == year)
                .Where(f => string.IsNullOrEmpty(department) || f.Department == department)
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            var statistics = new Form2Statistics
            {
                TotalItems = items.Count,
                TotalQuantityByInventory = items.Sum(i => i.QuantityByInventory),
                TotalQuantityByRecords = items.Sum(i => i.QuantityByRecords),
                TotalDifference = items.Sum(i => i.QuantityDifference),
                TotalCost = items.Sum(i => i.CostInDinar)
            };

            var viewModel = new Form2ReportViewModel
            {
                Year = year.Value,
                Department = department ?? "قسم الهندسة الكهروميكانيكية",
                Items = items,
                Statistics = statistics,
                ReportDate = DateTime.Now
            };

            return View(viewModel);
        }

        // GET: Form2/TransferToForm5/5
        public async Task<IActionResult> TransferToForm5(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form2Items
                .Include(f => f.PurchaseBatches)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var viewModel = new TransferToForm5ViewModel
            {
                Form2ItemId = item.Id,
                Form2Item = item,
                ConsumedQuantity = item.QuantityByInventory,
                Reason = Form5ConsumptionReason.NormalUse,
                Decision = Form5CommitteeDecision.UnderReview
            };

            return View(viewModel);
        }

        // POST: Form2/TransferToForm5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferToForm5(TransferToForm5ViewModel model)
        {
            if (ModelState.IsValid)
            {
                var form2Item = await _context.Form2Items
                    .Include(f => f.PurchaseBatches)
                    .FirstOrDefaultAsync(f => f.Id == model.Form2ItemId);

                if (form2Item == null)
                {
                    return NotFound();
                }

                // التحقق من الكمية
                if (model.ConsumedQuantity > form2Item.QuantityByInventory)
                {
                    ModelState.AddModelError("ConsumedQuantity", 
                        $"الكمية المستهلكة أكبر من المتاح ({form2Item.QuantityByInventory})");
                    model.Form2Item = form2Item;
                    return View(model);
                }

                // إنشاء عنصر نموذج 5
                var form5Item = new Form5Item
                {
                    Form2ItemId = form2Item.Id,
                    SequenceNumber = await GetNextForm5Sequence(form2Item.Department ?? ""),
                    MaterialItems = form2Item.MaterialName,
                    Quantity = model.ConsumedQuantity,
                    PurchaseYear = form2Item.PurchaseBatches.FirstOrDefault()?.PurchaseYear ?? DateTime.Now.Year,
                    OriginalUnitPriceDinar = form2Item.CostInDinar / form2Item.QuantityByInventory,
                    OriginalTotalPriceDinar = (form2Item.CostInDinar / form2Item.QuantityByInventory) * model.ConsumedQuantity,
                    DamagePercentage = model.DamagePercentage,
                    ConsumptionReason = model.Reason,
                    ConsumptionReasonDetails = model.ReasonDetails,
                    CommitteeDecision = model.Decision,
                    CommitteeDecisionDetails = model.DecisionDetails,
                    Department = form2Item.Department ?? "قسم الهندسة الكهروميكانيكية",
                    ReportDate = DateTime.Now,
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now
                };

                // حساب القيمة المتبقية
                if (model.DamagePercentage.HasValue)
                {
                    form5Item.ResidualValue = form5Item.OriginalTotalPriceDinar * 
                        (100 - model.DamagePercentage.Value) / 100;
                }

                _context.Form5Items.Add(form5Item);

                // تحديث نموذج 2
                form2Item.IsDocumentedInForm5 = true;
                form2Item.Form5Id = form5Item.Id;
                form2Item.Notes = (form2Item.Notes ?? "") + 
                    $" | تم التثبيت في استمارة رقم(5) بتاريخ {DateTime.Now:yyyy/MM/dd}";
                form2Item.LastUpdated = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "تم نقل العنصر إلى نموذج 5 بنجاح";
                return RedirectToAction("Details", "Form5", new { id = form5Item.Id });
            }

            model.Form2Item = await _context.Form2Items.FindAsync(model.Form2ItemId);
            return View(model);
        }

        // API: البحث عن المواد
        [HttpGet]
        public async Task<IActionResult> SearchMaterials(string term, int year)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var materials = await _context.Form2Items
                .Where(f => f.InventoryYear == year && 
                           (f.MaterialName.Contains(term) || f.CodeNumber.Contains(term)))
                .Take(10)
                .Select(f => new
                {
                    f.Id,
                    f.MaterialName,
                    f.CodeNumber,
                    f.QuantityByInventory,
                    f.CostInDinar
                })
                .ToListAsync();

            return Json(materials);
        }

        private bool Form2ItemExists(int id)
        {
            return _context.Form2Items.Any(e => e.Id == id);
        }

        private async Task<int> GetNextForm5Sequence(string department)
        {
            var max = await _context.Form5Items
                .Where(f => f.Department == department)
                .MaxAsync(f => (int?)f.SequenceNumber) ?? 0;
            return max + 1;
        }

        private void LoadSelectLists()
        {
            ViewData["Materials"] = new SelectList(
                _context.Materials.OrderBy(m => m.Name).ToList(),
                "Id", "Name");
        }
    }
}
