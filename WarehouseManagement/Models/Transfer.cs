using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models
{
    public class Transfer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المادة")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "من الموقع")]
        public int FromLocationId { get; set; }

        [Required]
        [Display(Name = "إلى الموقع")]
        public int ToLocationId { get; set; }

        [Required]
        [Display(Name = "الكمية المنقولة")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "تاريخ النقل")]
        public DateTime TransferDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "السبب")]
        public string? Reason { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "المسؤول عن النقل")]
        public string TransferredBy { get; set; } = string.Empty;

        [Display(Name = "تم التأكيد")]
        public bool IsConfirmed { get; set; } = false;

        [Display(Name = "تاريخ التأكيد")]
        public DateTime? ConfirmedDate { get; set; }

        [StringLength(100)]
        [Display(Name = "تم التأكيد بواسطة")]
        public string? ConfirmedBy { get; set; }

        [StringLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Material Material { get; set; } = null!;
        public virtual Location FromLocation { get; set; } = null!;
        public virtual Location ToLocation { get; set; } = null!;
    }
}
