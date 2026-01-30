using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// نموذج أسعار الشراء والتكاليف
    /// </summary>
    public class PurchasePrice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المادة")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "سنة الشراء")]
        public int PurchaseYear { get; set; }

        [Display(Name = "تاريخ الشراء")]
        public DateTime? PurchaseDate { get; set; }

        [Required]
        [Display(Name = "الكمية المشتراة")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر المفرد")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "السعر الكلي")]
        public decimal TotalPrice => Quantity * UnitPrice;

        [Display(Name = "رقم الفاتورة")]
        [StringLength(50)]
        public string? InvoiceNumber { get; set; }

        [Display(Name = "المورد")]
        [StringLength(200)]
        public string? Supplier { get; set; }

        [Display(Name = "طريقة الحصول")]
        public AcquisitionMethod Method { get; set; } = AcquisitionMethod.Purchase;

        [Display(Name = "مصدر النقل")]
        [StringLength(200)]
        public string? TransferSource { get; set; }

        [Display(Name = "العملة")]
        [StringLength(10)]
        public string Currency { get; set; } = "IQD";

        [Display(Name = "سعر الصرف")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal ExchangeRate { get; set; } = 1;

        [Display(Name = "الملاحظات")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ الإدخال")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual Material Material { get; set; } = null!;
    }

    /// <summary>
    /// طريقة الحصول على المادة
    /// </summary>
    public enum AcquisitionMethod
    {
        [Display(Name = "شراء")]
        Purchase = 1,
        [Display(Name = "مناقلة")]
        Transfer = 2,
        [Display(Name = "هدية")]
        Gift = 3,
        [Display(Name = "إعارة")]
        Loan = 4,
        [Display(Name = "أخرى")]
        Other = 5
    }
}