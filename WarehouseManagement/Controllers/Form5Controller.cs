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
    /// Controller لنموذج رقم (5) - قائمة بالموجودات المستهلكة
    /// </summary>
    public class Form5Controller : Controller
    {
        private readonly WarehouseContext _context;
        private readonly IFormDataService _formDataService;

        public Form5Controller(WarehouseContext context, IFormDataService formDataService)
        {
            _context = context;
            _formDataService = formDataService;
        }

        // GET: Form5
        public async Task<IActionResult> Index(string? department, string? searchTerm,
            Form5ConsumptionReason? reasonFilter, Form5CommitteeDecision? decisionFilter, int? yearFilter)
        {
            var query = _context.Form5Items
                .Include(f => f.Form2Item)
                .Include(f => f.PurchaseDetails)
                .AsQueryable();

            // تطبيق الفلاتر
            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(f => f.Department == department);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.MaterialItems.Contains(searchTerm));
            }

            if (reasonFilter.HasValue)
            {
                query = query.Where(f => f.ConsumptionReason == reasonFilter.Value);
            }

            if (decisionFilter.HasValue)
            {
                query = query.Where(f => f.CommitteeDecision == decisionFilter.Value);
            }

            if (yearFilter.HasValue)
            {
                query = query.Where(f => f.PurchaseYear == yearFilter.Value || 
                                        f.ReportDate.Year == yearFilter.Value);
            }

            var items = await query
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            // حساب الإحصائيات
            var statistics = new Form5Statistics
            {
                TotalItems = items.Count,
                TotalQuantity = items.Sum(i => i.Quantity),
                TotalOriginalValue = items.Sum(i => i.OriginalTotalPriceDinar),
                TotalResidualValue = items.Sum(i => i.ResidualValue ?? 0),
                ItemsDisposed = items.Count(i => i.IsDisposed),
                ItemsPending = items.Count(i => !i.IsDisposed),
                ByReason = items.GroupBy(i => i.ConsumptionReason)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByDecision = items.GroupBy(i => i.CommitteeDecision)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            var viewModel = new Form5ViewModel
            {
                Department = department ?? "",
                Items = items,
                Statistics = statistics,
                SearchTerm = searchTerm,
                ReasonFilter = reasonFilter,
                DecisionFilter = decisionFilter,
                YearFilter = yearFilter
            };

            // قوائم الفلاتر
            ViewBag.Departments = await _context.Form5Items
                .Select(f => f.Department)
                .Distinct()
                .ToListAsync();

            ViewBag.Years = await _context.Form5Items
                .Select(f => f.PurchaseYear)
                .Union(_context.Form5Items.Select(f => f.ReportDate.Year))
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            return View(viewModel);
        }

        // GET: Form5/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form5Items
                .Include(f => f.Form2Item)
                    .ThenInclude(f2 => f2!.PurchaseBatches)
                .Include(f => f.PurchaseDetails)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Form5/Create
        public async Task<IActionResult> Create(int? form2ItemId)
        {
            var model = new Form5CreateEditViewModel
            {
                Item = new Form5Item
                {
                    ReportDate = DateTime.Now,
                    Department = "قسم الهندسة الكهروميكانيكية",
                    ConsumptionReason = Form5ConsumptionReason.NormalUse,
                    CommitteeDecision = Form5CommitteeDecision.UnderReview
                }
            };

            // إذا تم تمرير عنصر من نموذج 2
            if (form2ItemId.HasValue)
            {
                var form2Item = await _context.Form2Items
                    .Include(f => f.PurchaseBatches)
                    .FirstOrDefaultAsync(f => f.Id == form2ItemId.Value);

                if (form2Item != null)
                {
                    model.LinkToForm2 = true;
                    model.SelectedForm2ItemId = form2Item.Id;
                    model.Item.Form2ItemId = form2Item.Id;
                    model.Item.MaterialItems = form2Item.MaterialName;
                    model.Item.Quantity = form2Item.QuantityByInventory;
                    model.Item.Department = form2Item.Department ?? "قسم الهندسة الكهروميكانيكية";

                    if (form2Item.PurchaseBatches.Any())
                    {
                        var firstBatch = form2Item.PurchaseBatches.OrderBy(p => p.PurchaseYear).First();
                        model.Item.PurchaseYear = firstBatch.PurchaseYear;
                        model.Item.OriginalUnitPriceDinar = firstBatch.UnitPrice;
                        model.Item.OriginalTotalPriceDinar = form2Item.CostInDinar;
                    }
                }
            }

            await LoadAvailableForm2Items(model);
            return View(model);
        }

        // POST: Form5/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Form5CreateEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                // تحديد رقم التسلسل
                var maxSequence = await _context.Form5Items
                    .Where(f => f.Department == model.Item.Department)
                    .MaxAsync(f => (int?)f.SequenceNumber) ?? 0;

                model.Item.SequenceNumber = maxSequence + 1;

                // حساب الثمن الكلي
                if (model.Item.OriginalTotalPriceDinar == 0)
                {
                    model.Item.OriginalTotalPriceDinar = model.Item.Quantity * model.Item.OriginalUnitPriceDinar;
                }

                // حساب القيمة المتبقية
                if (model.Item.DamagePercentage.HasValue)
                {
                    model.Item.ResidualValue = model.Item.OriginalTotalPriceDinar *
                        (100 - model.Item.DamagePercentage.Value) / 100;
                }

                model.Item.CreatedDate = DateTime.Now;
                model.Item.LastUpdated = DateTime.Now;

                _context.Form5Items.Add(model.Item);

                // إضافة تفاصيل الشراء إذا وجدت
                if (model.PurchaseDetails.Any())
                {
                    foreach (var detail in model.PurchaseDetails.Where(d => d.IsSelected))
                    {
                        var purchaseDetail = new Form5PurchaseDetail
                        {
                            Form5Item = model.Item,
                            PurchaseYear = detail.PurchaseYear,
                            Quantity = detail.Quantity,
                            UnitPrice = detail.UnitPrice,
                            StoredTotalPrice = detail.TotalPrice
                        };
                        _context.Form5PurchaseDetails.Add(purchaseDetail);
                    }
                }

                // تحديث نموذج 2 إذا كان مرتبطاً
                if (model.Item.Form2ItemId.HasValue)
                {
                    var form2Item = await _context.Form2Items.FindAsync(model.Item.Form2ItemId.Value);
                    if (form2Item != null)
                    {
                        form2Item.IsDocumentedInForm5 = true;
                        form2Item.Form5Id = model.Item.Id;
                        form2Item.Notes = (form2Item.Notes ?? "") +
                            $" | تم التثبيت في استمارة رقم(5) بتاريخ {DateTime.Now:yyyy/MM/dd}";
                        form2Item.LastUpdated = DateTime.Now;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم إضافة العنصر المستهلك بنجاح";
                return RedirectToAction(nameof(Index));
            }

            await LoadAvailableForm2Items(model);
            return View(model);
        }

        // GET: Form5/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form5Items
                .Include(f => f.PurchaseDetails)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            var model = new Form5CreateEditViewModel
            {
                Item = item,
                PurchaseDetails = item.PurchaseDetails.Select(p => new Form5PurchaseDetailViewModel
                {
                    Id = p.Id,
                    PurchaseYear = p.PurchaseYear,
                    Quantity = p.Quantity,
                    UnitPrice = p.UnitPrice,
                    IsSelected = true
                }).ToList()
            };

            await LoadAvailableForm2Items(model);
            return View(model);
        }

        // POST: Form5/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Form5CreateEditViewModel model)
        {
            if (id != model.Item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // إعادة حساب الثمن الكلي
                    if (model.Item.OriginalTotalPriceDinar == 0)
                    {
                        model.Item.OriginalTotalPriceDinar = model.Item.Quantity * model.Item.OriginalUnitPriceDinar;
                    }

                    // حساب القيمة المتبقية
                    if (model.Item.DamagePercentage.HasValue)
                    {
                        model.Item.ResidualValue = model.Item.OriginalTotalPriceDinar *
                            (100 - model.Item.DamagePercentage.Value) / 100;
                    }

                    model.Item.LastUpdated = DateTime.Now;

                    _context.Update(model.Item);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث البيانات بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Form5ItemExists(model.Item.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadAvailableForm2Items(model);
            return View(model);
        }

        // GET: Form5/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form5Items
                .Include(f => f.Form2Item)
                .Include(f => f.PurchaseDetails)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Form5/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Form5Items
                .Include(f => f.PurchaseDetails)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (item != null)
            {
                // تحديث نموذج 2 إذا كان مرتبطاً
                if (item.Form2ItemId.HasValue)
                {
                    var form2Item = await _context.Form2Items.FindAsync(item.Form2ItemId.Value);
                    if (form2Item != null)
                    {
                        form2Item.IsDocumentedInForm5 = false;
                        form2Item.Form5Id = null;
                        form2Item.LastUpdated = DateTime.Now;
                    }
                }

                // حذف تفاصيل الشراء المرتبطة
                _context.Form5PurchaseDetails.RemoveRange(item.PurchaseDetails);
                _context.Form5Items.Remove(item);

                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف العنصر بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Form5/Report
        public async Task<IActionResult> Report(string? department, DateTime? reportDate)
        {
            reportDate ??= DateTime.Now;

            var items = await _context.Form5Items
                .Include(f => f.PurchaseDetails)
                .Where(f => string.IsNullOrEmpty(department) || f.Department == department)
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            var statistics = new Form5Statistics
            {
                TotalItems = items.Count,
                TotalQuantity = items.Sum(i => i.Quantity),
                TotalOriginalValue = items.Sum(i => i.OriginalTotalPriceDinar),
                TotalResidualValue = items.Sum(i => i.ResidualValue ?? 0),
                ItemsDisposed = items.Count(i => i.IsDisposed),
                ItemsPending = items.Count(i => !i.IsDisposed),
                ByReason = items.GroupBy(i => i.ConsumptionReason)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ByDecision = items.GroupBy(i => i.CommitteeDecision)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            var viewModel = new Form5ReportViewModel
            {
                ReportDate = reportDate.Value,
                Department = department ?? "قسم الهندسة الكهروميكانيكية",
                Items = items,
                Statistics = statistics
            };

            return View(viewModel);
        }

        // GET: Form5/PrintReport
        public async Task<IActionResult> PrintReport(string? department, DateTime? reportDate)
        {
            reportDate ??= DateTime.Now;

            var items = await _context.Form5Items
                .Include(f => f.PurchaseDetails)
                .Where(f => string.IsNullOrEmpty(department) || f.Department == department)
                .OrderBy(f => f.SequenceNumber)
                .ToListAsync();

            var statistics = new Form5Statistics
            {
                TotalItems = items.Count,
                TotalQuantity = items.Sum(i => i.Quantity),
                TotalOriginalValue = items.Sum(i => i.OriginalTotalPriceDinar),
                TotalResidualValue = items.Sum(i => i.ResidualValue ?? 0)
            };

            var viewModel = new Form5ReportViewModel
            {
                ReportDate = reportDate.Value,
                Department = department ?? "قسم الهندسة الكهروميكانيكية",
                Items = items,
                Statistics = statistics,
                ReportTitle = "نموذج رقم (5) - قائمة بالموجودات المستهلكة"
            };

            return View(viewModel);
        }

        // POST: Form5/ProcessDecision/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessDecision(int id, string action, string? disposalRecordNumber)
        {
            var item = await _context.Form5Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            switch (action.ToLower())
            {
                case "dispose":
                    item.IsDisposed = true;
                    item.DisposalDate = DateTime.Now;
                    item.DisposalRecordNumber = disposalRecordNumber;
                    TempData["Success"] = "تم تسجيل التصرف بالمادة بنجاح";
                    break;

                case "revert":
                    item.IsDisposed = false;
                    item.DisposalDate = null;
                    item.DisposalRecordNumber = null;
                    TempData["Success"] = "تم إلغاء التصرف بالمادة";
                    break;
            }

            item.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Form5/AddPurchaseDetail/5
        public async Task<IActionResult> AddPurchaseDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Form5Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            ViewBag.Form5Item = item;
            return View(new Form5PurchaseDetail { Form5ItemId = id.Value });
        }

        // POST: Form5/AddPurchaseDetail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPurchaseDetail(Form5PurchaseDetail detail)
        {
            if (ModelState.IsValid)
            {
                detail.StoredTotalPrice = detail.Quantity * detail.UnitPrice;
                _context.Form5PurchaseDetails.Add(detail);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إضافة تفاصيل الشراء بنجاح";
                return RedirectToAction(nameof(Details), new { id = detail.Form5ItemId });
            }

            ViewBag.Form5Item = await _context.Form5Items.FindAsync(detail.Form5ItemId);
            return View(detail);
        }

        private bool Form5ItemExists(int id)
        {
            return _context.Form5Items.Any(e => e.Id == id);
        }

        private async Task LoadAvailableForm2Items(Form5CreateEditViewModel model)
        {
            model.AvailableForm2Items = await _context.Form2Items
                .Where(f => !f.IsDocumentedInForm5)
                .OrderBy(f => f.SequenceNumber)
                .Select(f => new Form2ItemSelectViewModel
                {
                    Id = f.Id,
                    MaterialName = f.MaterialName,
                    CodeNumber = f.CodeNumber,
                    QuantityByInventory = f.QuantityByInventory,
                    CostInDinar = f.CostInDinar
                })
                .ToListAsync();
        }
    }
}
