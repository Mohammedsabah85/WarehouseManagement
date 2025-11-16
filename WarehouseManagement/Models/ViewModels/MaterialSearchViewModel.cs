// ===== Models/ViewModels/MaterialSearchViewModel.cs =====
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.ViewModels
{
    public class MaterialSearchViewModel
    {
        [Display(Name = "البحث")]
        public string? SearchTerm { get; set; }

        [Display(Name = "الفئة")]
        public int? CategoryId { get; set; }

        [Display(Name = "الموقع")]
        public int? LocationId { get; set; }

        [Display(Name = "نوع الموقع")]
        public LocationType? LocationType { get; set; }

        public List<Material> Results { get; set; } = new List<Material>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<Location> Locations { get; set; } = new List<Location>();
    }
}
