using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;

namespace WarehouseManagement.Controllers
{
    public class TransfersController : Controller
    {
        private readonly WarehouseContext _context;

        public TransfersController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: Transfers
        public async Task<IActionResult> Index()
        {
            var transfers = await _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .OrderByDescending(t => t.TransferDate)
                .ToListAsync();

            return View(transfers);
        }

        // GET: Transfers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = await _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // GET: Transfers/Create
        public async Task<IActionResult> Create(int? materialId)
        {
            // إذا تم تمرير materialId، نحدد المادة مسبقاً
            if (materialId.HasValue)
            {
                var material = await _context.Materials
                    .Include(m => m.Location)
                    .FirstOrDefaultAsync(m => m.Id == materialId.Value);

                if (material != null)
                {
                    ViewBag.SelectedMaterial = material;
                    ViewBag.FromLocationId = material.LocationId;
                }
            }

            await LoadSelectLists();
            return View();
        }

        // POST: Transfers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaterialId,FromLocationId,ToLocationId,Quantity,Reason,TransferredBy,Notes")] Transfer transfer)
        {
            if (ModelState.IsValid)
            {
                // التحقق من أن المواقع مختلفة
                if (transfer.FromLocationId == transfer.ToLocationId)
                {
                    ModelState.AddModelError("", "لا يمكن نقل المادة إلى نفس الموقع");
                    await LoadSelectLists();
                    return View(transfer);
                }

                // التحقق من وجود المادة في الموقع المصدر
                var material = await _context.Materials
                    .FirstOrDefaultAsync(m => m.Id == transfer.MaterialId && m.LocationId == transfer.FromLocationId);

                if (material == null)
                {
                    ModelState.AddModelError("", "المادة غير موجودة في الموقع المحدد");
                    await LoadSelectLists();
                    return View(transfer);
                }

                // التحقق من الكمية المتاحة
                if (material.Quantity < transfer.Quantity)
                {
                    ModelState.AddModelError("Quantity", $"الكمية المتاحة هي {material.Quantity} {material.Unit} فقط");
                    await LoadSelectLists();
                    return View(transfer);
                }

                // إنشاء عملية النقل
                transfer.TransferDate = DateTime.Now;
                _context.Add(transfer);
                await _context.SaveChangesAsync();

                TempData["Success"] = "تم إنشاء عملية النقل بنجاح. يرجى تأكيدها لإتمام النقل.";
                return RedirectToAction(nameof(Index));
            }

            await LoadSelectLists();
            return View(transfer);
        }

        // POST: Transfers/Confirm/5
        [HttpPost]
        public async Task<IActionResult> Confirm(int id, string confirmedBy)
        {
            if (string.IsNullOrWhiteSpace(confirmedBy))
            {
                TempData["Error"] = "يجب إدخال اسم المؤكد";
                return RedirectToAction(nameof(Index));
            }

            var transfer = await _context.Transfers
                .Include(t => t.Material)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
            {
                TempData["Error"] = "عملية النقل غير موجودة";
                return RedirectToAction(nameof(Index));
            }

            if (transfer.IsConfirmed)
            {
                TempData["Warning"] = "عملية النقل مؤكدة مسبقاً";
                return RedirectToAction(nameof(Index));
            }

            // التحقق من الكمية مرة أخرى
            var material = await _context.Materials
                .FirstOrDefaultAsync(m => m.Id == transfer.MaterialId && m.LocationId == transfer.FromLocationId);

            if (material == null || material.Quantity < transfer.Quantity)
            {
                TempData["Error"] = "الكمية المطلوبة غير متوفرة";
                return RedirectToAction(nameof(Index));
            }

            // تحديث كمية المادة في الموقع المصدر
            material.Quantity -= transfer.Quantity;
            material.LastUpdated = DateTime.Now;

            // البحث عن نفس المادة في الموقع الهدف
            var targetMaterial = await _context.Materials
                .FirstOrDefaultAsync(m => m.Code == material.Code && m.LocationId == transfer.ToLocationId);

            if (targetMaterial != null)
            {
                // إذا كانت المادة موجودة، نزيد الكمية
                targetMaterial.Quantity += transfer.Quantity;
                targetMaterial.LastUpdated = DateTime.Now;
            }
            else
            {
                // إنشاء مادة جديدة في الموقع الهدف
                var newMaterial = new Material
                {
                    Name = material.Name,
                    Code = material.Code + "-" + DateTime.Now.ToString("yyyyMMddHHmm"),
                    Description = material.Description,
                    Quantity = transfer.Quantity,
                    Unit = material.Unit,
                    Price = material.Price,
                    ExpiryDate = material.ExpiryDate,
                    CategoryId = material.CategoryId,
                    LocationId = transfer.ToLocationId,
                    Notes = $"منقول من {material.Location?.Name} في {DateTime.Now:yyyy-MM-dd}",
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now
                };
                _context.Materials.Add(newMaterial);
            }

            // تأكيد عملية النقل
            transfer.IsConfirmed = true;
            transfer.ConfirmedDate = DateTime.Now;
            transfer.ConfirmedBy = confirmedBy;

            await _context.SaveChangesAsync();
            TempData["Success"] = "تم تأكيد عملية النقل بنجاح";

            return RedirectToAction(nameof(Index));
        }

        // GET: Transfers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transfer = await _context.Transfers
                .Include(t => t.Material)
                .Include(t => t.FromLocation)
                .Include(t => t.ToLocation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transfer == null)
            {
                return NotFound();
            }

            return View(transfer);
        }

        // POST: Transfers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transfer = await _context.Transfers.FindAsync(id);
            if (transfer != null)
            {
                if (transfer.IsConfirmed)
                {
                    TempData["Error"] = "لا يمكن حذف عملية نقل مؤكدة";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                _context.Transfers.Remove(transfer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف عملية النقل بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: API endpoint للحصول على المواد في موقع معين
        [HttpGet]
        public async Task<IActionResult> GetMaterialsByLocation(int locationId)
        {
            var materials = await _context.Materials
                .Where(m => m.LocationId == locationId && m.Quantity > 0)
                .Select(m => new
                {
                    id = m.Id,
                    name = m.Name,
                    code = m.Code,
                    quantity = m.Quantity,
                    unit = m.Unit
                })
                .ToListAsync();

            return Json(materials);
        }

        private async Task LoadSelectLists()
        {
            ViewData["MaterialId"] = new SelectList(await _context.Materials.ToListAsync(), "Id", "Name");
            ViewData["FromLocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name");
            ViewData["ToLocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name");
        }

        private bool TransferExists(int id)
        {
            return _context.Transfers.Any(e => e.Id == id);
        }
    }
}
