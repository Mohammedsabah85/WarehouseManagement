using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الفئة")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}