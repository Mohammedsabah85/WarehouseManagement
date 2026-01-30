using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// نموذج المواد المستهلكة والتالفة
    /// </summary>
    public class ConsumedMaterial
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المادة")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "الكمية المستهلكة")]
        public int ConsumedQuantity { get; set; }

        [Display(Name = "تاريخ الشراء الأصلي")]
        public DateTime? OriginalPurchaseDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر المفرد عند الشراء")]
        public decimal? OriginalUnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "الثمن الكلي عند الشراء")]
        public decimal? OriginalTotalPrice { get; set; }

        [Display(Name = "نسبة الضرر (%)")]
        [Range(0, 100)]
        public int DamagePercentage { get; set; }

        [Display(Name = "مدة الاستعمال (بالأيام)")]
        public int? UsageDurationDays { get; set; }

        [Display(Name = "تاريخ الاستهلاك")]
        public DateTime ConsumptionDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "سبب الاستهلاك")]
        public ConsumptionReason Reason { get; set; }

        [Display(Name = "تفاصيل السبب")]
        [StringLength(500)]
        public string? ReasonDetails { get; set; }

        [Display(Name = "قرار اللجنة")]
        public CommitteeDecision Decision { get; set; }

        [Display(Name = "توصيات اللجنة")]
        [StringLength(500)]
        public string? CommitteeRecommendations { get; set; }

        [Display(Name = "المسؤول عن التقييم")]
        [StringLength(100)]
        public string? EvaluatedBy { get; set; }

        [Display(Name = "تاريخ التقييم")]
        public DateTime? EvaluationDate { get; set; }

        [Display(Name = "القيمة المتبقية")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ResidualValue { get; set; }

        [Display(Name = "الموقع الحالي")]
        [StringLength(200)]
        public string? CurrentLocation { get; set; }

        [Display(Name = "رقم محضر الإتلاف")]
        [StringLength(50)]
        public string? DisposalRecordNumber { get; set; }

        [Display(Name = "تم التصرف")]
        public bool IsDisposed { get; set; } = false;

        [Display(Name = "تاريخ التصرف")]
        public DateTime? DisposalDate { get; set; }

        [Display(Name = "الملاحظات")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Navigation Properties
        public virtual Material Material { get; set; } = null!;
    }

    /// <summary>
    /// أسباب الاستهلاك
    /// </summary>
    public enum ConsumptionReason
    {
        [Display(Name = "الاستعمال الاعتيادي")]
        NormalUse = 1,
        [Display(Name = "القدم")]
        Obsolescence = 2,
        [Display(Name = "عطل فني")]
        TechnicalFailure = 3,
        [Display(Name = "حادث")]
        Accident = 4,
        [Display(Name = "سوء استخدام")]
        Misuse = 5,
        [Display(Name = "عوامل خارجية")]
        ExternalFactors = 6,
        [Display(Name = "انتهاء العمر الافتراضي")]
        EndOfLife = 7,
        [Display(Name = "أخرى")]
        Other = 8
    }

    /// <summary>
    /// قرار اللجنة
    /// </summary>
    public enum CommitteeDecision
    {
        [Display(Name = "إعادة استعمال كلي")]
        FullReuse = 1,
        [Display(Name = "إعادة استعمال جزئي")]
        PartialReuse = 2,
        [Display(Name = "بيع")]
        Sell = 3,
        [Display(Name = "إتلاف")]
        Dispose = 4,
        [Display(Name = "إصلاح")]
        Repair = 5,
        [Display(Name = "تحويل للخردة")]
        Scrap = 6,
        [Display(Name = "قيد الدراسة")]
        UnderReview = 7
    }
}