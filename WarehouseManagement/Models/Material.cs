using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    public class Material
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المادة مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم المادة")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز المادة مطلوب")]
        [StringLength(50)]
        [Display(Name = "رمز المادة")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "الوحدة")]
        public string Unit { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "السعر")]
        public decimal? Price { get; set; }

        [Display(Name = "تاريخ انتهاء الصلاحية")]
        public DateTime? ExpiryDate { get; set; }

        [Required]
        [Display(Name = "نوع المادة")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "الموقع")]
        public int LocationId { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "آخر تحديث")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Category Category { get; set; } = null!;
        public virtual Location Location { get; set; } = null!;

        // إزالة Navigation Properties المعقدة لتجنب الأخطاء
        // يمكن الوصول للـ Transfers عبر DbContext مباشرة
    }
}