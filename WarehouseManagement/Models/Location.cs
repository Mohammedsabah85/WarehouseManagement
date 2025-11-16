using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الموقع مطلوب")]
        [StringLength(100)]
        [Display(Name = "اسم الموقع")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "رمز الموقع مطلوب")]
        [StringLength(20)]
        [Display(Name = "رمز الموقع")]
        public string Code { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "نوع الموقع")]
        public LocationType Type { get; set; }

        [Display(Name = "السعة القصوى")]
        public int? MaxCapacity { get; set; }

        [Display(Name = "الطابق")]
        public int? Floor { get; set; }

        [StringLength(100)]
        [Display(Name = "المبنى")]
        public string? Building { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
        // إزالة الـ Transfers Navigation لتجنب التعقيد
    }

    public enum LocationType
    {
        [Display(Name = "مخزن")]
        Warehouse = 1,
        [Display(Name = "مكتب")]
        Office = 2,
        [Display(Name = "ورشة")]
        Workshop = 3,
        [Display(Name = "مختبر")]
        Laboratory = 4,
        [Display(Name = "غرفة اجتماعات")]
        MeetingRoom = 5,
        [Display(Name = "أخرى")]
        Other = 6
    }
}