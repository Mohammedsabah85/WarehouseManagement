using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// نموذج رقم (5) - قائمة بالموجودات المستهلكة
    /// Form 5 - Consumed/Depreciated Assets List
    /// قائمة بالموجودات المستهلكة العائدة لدائرة قسم الهندسة الكهروميكانيكية
    /// </summary>
    public class Form5Item
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// التسلسل - ت
        /// </summary>
        [Display(Name = "ت")]
        public int SequenceNumber { get; set; }

        /// <summary>
        /// مفردات المواد - اسم المادة
        /// </summary>
        [Required(ErrorMessage = "اسم المادة مطلوب")]
        [StringLength(200)]
        [Display(Name = "مفردات المواد")]
        public string MaterialItems { get; set; } = string.Empty;

        /// <summary>
        /// الكمية
        /// </summary>
        [Required]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        /// <summary>
        /// تاريخ الشراء (السنة)
        /// </summary>
        [Required]
        [Display(Name = "تاريخ الشراء")]
        public int PurchaseYear { get; set; }

        /// <summary>
        /// سعر المفرد عند الشراء - دينار
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "سعر المفرد عند الشراء (دينار)")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal OriginalUnitPriceDinar { get; set; }

        /// <summary>
        /// سعر المفرد عند الشراء - فلس
        /// </summary>
        [Display(Name = "سعر المفرد عند الشراء (فلس)")]
        public int OriginalUnitPriceFils { get; set; }

        /// <summary>
        /// الثمن الكلي عند الشراء - دينار
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "الثمن الكلي عند الشراء (دينار)")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal OriginalTotalPriceDinar { get; set; }

        /// <summary>
        /// الثمن الكلي عند الشراء - فلس
        /// </summary>
        [Display(Name = "الثمن الكلي عند الشراء (فلس)")]
        public int OriginalTotalPriceFils { get; set; }

        /// <summary>
        /// الثمن الكلي المحسوب
        /// </summary>
        [NotMapped]
        [Display(Name = "الثمن الكلي عند الشراء")]
        public decimal CalculatedTotalPrice => Quantity * OriginalUnitPriceDinar;

        /// <summary>
        /// نسبة الضرر (%)
        /// </summary>
        [Display(Name = "نسبة الضرر (%)")]
        [Range(0, 100)]
        public decimal? DamagePercentage { get; set; }

        /// <summary>
        /// مدة الاستعمال (نص حر - مثل "5 سنوات")
        /// </summary>
        [StringLength(100)]
        [Display(Name = "مدة الاستعمال")]
        public string? UsageDuration { get; set; }

        /// <summary>
        /// مدة الاستعمال بالأيام (محسوب)
        /// </summary>
        [Display(Name = "مدة الاستعمال (أيام)")]
        public int? UsageDurationDays { get; set; }

        /// <summary>
        /// أسباب استهلاك المواد وهل كان من جراء الاستعمال الاعتيادي أو من تقصير توصيات المسؤولين
        /// </summary>
        [Required]
        [Display(Name = "سبب الاستهلاك")]
        public Form5ConsumptionReason ConsumptionReason { get; set; } = Form5ConsumptionReason.NormalUse;

        /// <summary>
        /// تفاصيل سبب الاستهلاك
        /// </summary>
        [StringLength(500)]
        [Display(Name = "تفاصيل سبب الاستهلاك")]
        public string? ConsumptionReasonDetails { get; set; }

        /// <summary>
        /// توجيهات اللجنة بإعادة استعمالها بعض أو كلا أو بيعها أو إتلافها
        /// </summary>
        [Required]
        [Display(Name = "توجيهات اللجنة")]
        public Form5CommitteeDecision CommitteeDecision { get; set; } = Form5CommitteeDecision.UnderReview;

        /// <summary>
        /// تفاصيل توجيهات اللجنة
        /// </summary>
        [StringLength(500)]
        [Display(Name = "تفاصيل توجيهات اللجنة")]
        public string? CommitteeDecisionDetails { get; set; }

        /// <summary>
        /// القيمة المتبقية بعد الاستهلاك
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "القيمة المتبقية")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal? ResidualValue { get; set; }

        /// <summary>
        /// تاريخ التقرير - كما في العنوان
        /// </summary>
        [Required]
        [Display(Name = "تاريخ التقرير")]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        /// <summary>
        /// القسم/الدائرة - مثل "قسم الهندسة الكهروميكانيكية"
        /// </summary>
        [Required]
        [StringLength(200)]
        [Display(Name = "القسم/الدائرة")]
        public string Department { get; set; } = "قسم الهندسة الكهروميكانيكية";

        /// <summary>
        /// هل تم التصرف بالمادة
        /// </summary>
        [Display(Name = "تم التصرف")]
        public bool IsDisposed { get; set; } = false;

        /// <summary>
        /// تاريخ التصرف
        /// </summary>
        [Display(Name = "تاريخ التصرف")]
        public DateTime? DisposalDate { get; set; }

        /// <summary>
        /// رقم محضر التصرف
        /// </summary>
        [StringLength(50)]
        [Display(Name = "رقم محضر التصرف")]
        public string? DisposalRecordNumber { get; set; }

        /// <summary>
        /// ملاحظات
        /// </summary>
        [StringLength(1000)]
        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        /// <summary>
        /// تاريخ الإنشاء
        /// </summary>
        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// تاريخ آخر تحديث
        /// </summary>
        [Display(Name = "آخر تحديث")]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        /// <summary>
        /// المستخدم الذي أنشأ السجل
        /// </summary>
        [StringLength(100)]
        [Display(Name = "أنشئ بواسطة")]
        public string? CreatedBy { get; set; }

        // Navigation Properties
        /// <summary>
        /// مرجع عنصر نموذج 2 (اختياري)
        /// </summary>
        [Display(Name = "عنصر نموذج (2)")]
        public int? Form2ItemId { get; set; }
        public virtual Form2Item? Form2Item { get; set; }

        /// <summary>
        /// تفاصيل الشراء حسب السنوات (دفعات الشراء المرتبطة بهذا العنصر المستهلك)
        /// </summary>
        public virtual ICollection<Form5PurchaseDetail> PurchaseDetails { get; set; } = new List<Form5PurchaseDetail>();
    }

    /// <summary>
    /// تفاصيل الشراء لعنصر نموذج 5 (عندما يكون هناك عدة دفعات شراء بسنوات مختلفة)
    /// مثل: 43 قطعة في 2014 بسعر 570,000 + 30 قطعة في 2016 بسعر 490,000
    /// </summary>
    public class Form5PurchaseDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Form5ItemId { get; set; }

        /// <summary>
        /// سنة الشراء
        /// </summary>
        [Required]
        [Display(Name = "سنة الشراء")]
        public int PurchaseYear { get; set; }

        /// <summary>
        /// العدد/الكمية
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
        [NotMapped]
        [Display(Name = "السعر الكلي")]
        public decimal TotalPrice => Quantity * UnitPrice;

        /// <summary>
        /// السعر الكلي المخزن
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "السعر الكلي")]
        public decimal StoredTotalPrice { get; set; }

        // Navigation
        public virtual Form5Item Form5Item { get; set; } = null!;
    }

    /// <summary>
    /// أسباب استهلاك المواد - نموذج 5
    /// </summary>
    public enum Form5ConsumptionReason
    {
        [Display(Name = "جراء الاستعمال")]
        NormalUse = 1,

        [Display(Name = "الاستعمال الاعتيادي")]
        RegularUse = 2,

        [Display(Name = "القدم")]
        Obsolescence = 3,

        [Display(Name = "عطل فني")]
        TechnicalFailure = 4,

        [Display(Name = "تقصير المسؤولين")]
        NegligenceByOfficials = 5,

        [Display(Name = "حادث")]
        Accident = 6,

        [Display(Name = "سوء استخدام")]
        Misuse = 7,

        [Display(Name = "عوامل خارجية")]
        ExternalFactors = 8,

        [Display(Name = "انتهاء العمر الافتراضي")]
        EndOfLife = 9,

        [Display(Name = "أخرى")]
        Other = 10
    }

    /// <summary>
    /// توجيهات اللجنة - نموذج 5
    /// </summary>
    public enum Form5CommitteeDecision
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
