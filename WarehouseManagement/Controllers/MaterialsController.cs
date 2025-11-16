using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Controllers
{
    public class MaterialsController : Controller
    {
        private readonly WarehouseContext _context;

        public MaterialsController(WarehouseContext context)
        {
            _context = context;
        }

        // GET: Materials
        public async Task<IActionResult> Index()
        {
            var materials = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .OrderBy(m => m.Name)
                .ToListAsync();

            return View(materials);
        }

        // GET: Materials/Search
        public async Task<IActionResult> Search()
        {
            var viewModel = new MaterialSearchViewModel
            {
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                Locations = await _context.Locations.OrderBy(l => l.Name).ToListAsync()
            };

            return View(viewModel);
        }

        // POST: Materials/Search
        [HttpPost]
        public async Task<IActionResult> Search(MaterialSearchViewModel model)
        {
            var query = _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                query = query.Where(m =>
                    m.Name.Contains(model.SearchTerm) ||
                    m.Code.Contains(model.SearchTerm) ||
                    (m.Description != null && m.Description.Contains(model.SearchTerm)));
            }

            if (model.CategoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == model.CategoryId.Value);
            }

            if (model.LocationId.HasValue)
            {
                query = query.Where(m => m.LocationId == model.LocationId.Value);
            }

            if (model.LocationType.HasValue)
            {
                query = query.Where(m => m.Location.Type == model.LocationType.Value);
            }

            model.Results = await query.OrderBy(m => m.Name).ToListAsync();
            model.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            model.Locations = await _context.Locations.OrderBy(l => l.Name).ToListAsync();

            return View(model);
        }

        // GET: Materials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // GET: Materials/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name");
            ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: Materials/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,Description,Quantity,Unit,Price,ExpiryDate,CategoryId,LocationId,Notes")] Material material)
        {
            if (ModelState.IsValid)
            {
                // التحقق من أن الرمز فريد
                var existingMaterial = await _context.Materials
                    .FirstOrDefaultAsync(m => m.Code == material.Code);

                if (existingMaterial != null)
                {
                    ModelState.AddModelError("Code", "رمز المادة موجود مسبقاً");
                }
                else
                {
                    _context.Add(material);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم إضافة المادة بنجاح";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", material.CategoryId);
            ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", material.LocationId);
            return View(material);
        }

        // GET: Materials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials.FindAsync(id);
            if (material == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", material.CategoryId);
            ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", material.LocationId);
            return View(material);
        }

        // POST: Materials/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Code,Description,Quantity,Unit,Price,ExpiryDate,CategoryId,LocationId,CreatedDate,Notes")] Material material)
        {
            if (id != material.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // التحقق من أن الرمز فريد (باستثناء المادة الحالية)
                    var existingMaterial = await _context.Materials
                        .FirstOrDefaultAsync(m => m.Code == material.Code && m.Id != material.Id);

                    if (existingMaterial != null)
                    {
                        ModelState.AddModelError("Code", "رمز المادة موجود مسبقاً");
                        ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", material.CategoryId);
                        ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", material.LocationId);
                        return View(material);
                    }

                    material.LastUpdated = DateTime.Now;
                    _context.Update(material);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "تم تحديث المادة بنجاح";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaterialExists(material.Id))
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

            ViewData["CategoryId"] = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", material.CategoryId);
            ViewData["LocationId"] = new SelectList(await _context.Locations.ToListAsync(), "Id", "Name", material.LocationId);
            return View(material);
        }

        // GET: Materials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var material = await _context.Materials
                .Include(m => m.Category)
                .Include(m => m.Location)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (material == null)
            {
                return NotFound();
            }

            return View(material);
        }

        // POST: Materials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Materials.FindAsync(id);
            if (material != null)
            {
                // التحقق من أن المادة غير مستخدمة في عمليات نقل
                var hasTransfers = await _context.Transfers
                    .AnyAsync(t => t.MaterialId == material.Id);

                if (hasTransfers)
                {
                    TempData["Error"] = "لا يمكن حذف المادة لأنها مستخدمة في عمليات نقل";
                    return RedirectToAction(nameof(Delete), new { id = id });
                }

                _context.Materials.Remove(material);
                await _context.SaveChangesAsync();
                TempData["Success"] = "تم حذف المادة بنجاح";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MaterialExists(int id)
        {
            return _context.Materials.Any(e => e.Id == id);
        }
    }
}