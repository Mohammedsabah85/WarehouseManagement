using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// تفصيل دفعات الشراء حسب السنوات
    /// Purchase Batches by Year - تفصيل أسعار الشراء لكل مادة
    /// كما في الصورة 2 - جدول تفصيل الشراء
    /// </summary>
    public class PurchaseBatch
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// مرجع عنصر نموذج 2
        /// </summary>
        [Required]
        [Display(Name = "عنصر نموذج (2)")]
        public int Form2ItemId { get; set; }

        /// <summary>
        /// التسلسل
        /// </summary>
        [Display(Name = "ت")]
        public int SequenceNumber { get; set; }

        /// <summary>
        /// اسم المادة (مكرر للسهولة)
        /// </summary>
        [StringLength(200)]
        [Display(Name = "اسم المادة")]
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// الرقم الرمزي - مثل 2/1/1
        /// </summary>
        [StringLength(20)]
        [Display(Name = "الرقم الرمزي")]
        public string CodeNumber { get; set; } = string.Empty;

        /// <summary>
        /// سنة الشراء لكل مادة
        /// </summary>
        [Required]
        [Display(Name = "سنة الشراء لكل مادة")]
        public int PurchaseYear { get; set; }

        /// <summary>
        /// تاريخ الشراء الكامل (اختياري)
        /// </summary>
        [Display(Name = "تاريخ الشراء")]
        public DateTime? PurchaseDate { get; set; }

        /// <summary>
        /// العدد - الكمية المشتراة في هذه الدفعة
        /// </summary>
        [Required]
        [Display(Name = "العدد")]
        public int Quantity { get; set; }

        /// <summary>
        /// سعر المفرد
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر المفرد")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// السعر الكلي (محسوب)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "السعر الكلي")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal TotalPrice => Quantity * UnitPrice;

        /// <summary>
        /// السعر الكلي المخزن
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "السعر الكلي")]
        public decimal StoredTotalPrice { get; set; }

        /// <summary>
        /// رقم الفاتورة (اختياري)
        /// </summary>
        [StringLength(50)]
        [Display(Name = "رقم الفاتورة")]
        public string? InvoiceNumber { get; set; }

        /// <summary>
        /// المورد (اختياري)
        /// </summary>
        [StringLength(200)]
        [Display(Name = "المورد")]
        public string? Supplier { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        [StringLength(500)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation Property
        public virtual Form2Item Form2Item { get; set; } = null!;
    }
}
