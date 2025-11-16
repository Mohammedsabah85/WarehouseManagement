using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Data;

namespace WarehouseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialsApiController : ControllerBase
    {
        private readonly WarehouseContext _context;

        public MaterialsApiController(WarehouseContext context)
        {
            _context = context;
        }

        [HttpGet("bylocation/{locationId}")]
        public async Task<IActionResult> GetMaterialsByLocation(int locationId)
        {
            var materials = await _context.Materials
                .Where(m => m.LocationId == locationId && m.Quantity > 0)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Code,
                    m.Quantity,
                    m.Unit
                })
                .ToListAsync();

            return Ok(materials);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchMaterials(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Ok(new List<object>());
            }

            var materials = await _context.Materials
                .Where(m => m.Name.Contains(term) || m.Code.Contains(term))
                .Take(10)
                .Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Code,
                    m.Quantity,
                    m.Unit,
                    Location = m.Location.Name
                })
                .ToListAsync();

            return Ok(materials);
        }
    }
}