using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WarehouseManagement.Models.ViewModels
{
    #region Form2 ViewModels

    /// <summary>
    /// ViewModel لعرض نموذج 2 مع التفاصيل
    /// </summary>
    public class Form2ViewModel
    {
        public int Year { get; set; } = DateTime.Now.Year;
        public string? Department { get; set; }
        public List<Form2Item> Items { get; set; } = new();
        public Form2Statistics Statistics { get; set; } = new();
        
        // فلاتر
        public string? SearchTerm { get; set; }
        public string? ConditionFilter { get; set; }
        public string? OwnershipFilter { get; set; }
    }

    /// <summary>
    /// إحصائيات نموذج 2
    /// </summary>
    public class Form2Statistics
    {
        public int TotalItems { get; set; }
        public int TotalQuantityByInventory { get; set; }
        public int TotalQuantityByRecords { get; set; }
        public int TotalDifference { get; set; }
        public decimal TotalCost { get; set; }
        public int ItemsWithShortage { get; set; }
        public int ItemsWithSurplus { get; set; }
        public int ItemsMatching { get; set; }
        public int ItemsInForm5 { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ViewModel لإنشاء/تعديل عنصر نموذج 2
    /// </summary>
    public class Form2CreateEditViewModel
    {
        public Form2Item Item { get; set; } = new();
        public List<PurchaseBatchViewModel> PurchaseBatches { get; set; } = new();
        public bool CreateWithPurchaseBatches { get; set; } = false;
    }

    /// <summary>
    /// ViewModel لدفعة شراء
    /// </summary>
    public class PurchaseBatchViewModel
    {
        public int Id { get; set; }
        public int PurchaseYear { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public string? Notes { get; set; }
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// ViewModel لتقرير نموذج 2 (للطباعة)
    /// </summary>
    public class Form2ReportViewModel
    {
        [Display(Name = "السنة")]
        public int Year { get; set; }

        [Display(Name = "القسم/الدائرة")]
        public string Department { get; set; } = string.Empty;

        [Display(Name = "تاريخ التقرير")]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Display(Name = "عناصر الجرد")]
        public List<Form2Item> Items { get; set; } = new();

        [Display(Name = "الإحصائيات")]
        public Form2Statistics Statistics { get; set; } = new();

        // معلومات إضافية للطباعة
        public string ReportTitle { get; set; } = "نموذج رقم (2)";
        public string OrganizationName { get; set; } = string.Empty;
        public string PreparedBy { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
    }

    #endregion

    #region Form5 ViewModels

    /// <summary>
    /// ViewModel لعرض نموذج 5 مع التفاصيل
    /// </summary>
    public class Form5ViewModel
    {
        public DateTime ReportDate { get; set; } = DateTime.Now;
        public string Department { get; set; } = "قسم الهندسة الكهروميكانيكية";
        public List<Form5Item> Items { get; set; } = new();
        public Form5Statistics Statistics { get; set; } = new();

        // فلاتر
        public string? SearchTerm { get; set; }
        public Form5ConsumptionReason? ReasonFilter { get; set; }
        public Form5CommitteeDecision? DecisionFilter { get; set; }
        public int? YearFilter { get; set; }
    }

    /// <summary>
    /// إحصائيات نموذج 5
    /// </summary>
    public class Form5Statistics
    {
        public int TotalItems { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalOriginalValue { get; set; }
        public decimal TotalResidualValue { get; set; }
        public decimal TotalLoss => TotalOriginalValue - TotalResidualValue;
        public int ItemsDisposed { get; set; }
        public int ItemsPending { get; set; }
        public Dictionary<Form5ConsumptionReason, int> ByReason { get; set; } = new();
        public Dictionary<Form5CommitteeDecision, int> ByDecision { get; set; } = new();
        public DateTime GeneratedDate { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// ViewModel لإنشاء/تعديل عنصر نموذج 5
    /// </summary>
    public class Form5CreateEditViewModel
    {
        public Form5Item Item { get; set; } = new();
        public List<Form5PurchaseDetailViewModel> PurchaseDetails { get; set; } = new();
        
        // قائمة عناصر نموذج 2 للربط
        public List<Form2ItemSelectViewModel> AvailableForm2Items { get; set; } = new();
        
        public bool LinkToForm2 { get; set; } = false;
        public int? SelectedForm2ItemId { get; set; }
    }

    /// <summary>
    /// ViewModel لتفاصيل الشراء في نموذج 5
    /// </summary>
    public class Form5PurchaseDetailViewModel
    {
        public int Id { get; set; }
        public int PurchaseYear { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public bool IsSelected { get; set; } = true;
    }

    /// <summary>
    /// ViewModel لاختيار عنصر من نموذج 2
    /// </summary>
    public class Form2ItemSelectViewModel
    {
        public int Id { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public string CodeNumber { get; set; } = string.Empty;
        public int QuantityByInventory { get; set; }
        public decimal CostInDinar { get; set; }
    }

    /// <summary>
    /// ViewModel لتقرير نموذج 5 (للطباعة)
    /// </summary>
    public class Form5ReportViewModel
    {
        [Display(Name = "تاريخ التقرير")]
        public DateTime ReportDate { get; set; }

        [Display(Name = "القسم/الدائرة")]
        public string Department { get; set; } = string.Empty;

        [Display(Name = "عناصر المستهلكات")]
        public List<Form5Item> Items { get; set; } = new();

        [Display(Name = "الإحصائيات")]
        public Form5Statistics Statistics { get; set; } = new();

        // معلومات إضافية للطباعة
        public string ReportTitle { get; set; } = "نموذج رقم (5) - قائمة بالموجودات المستهلكة";
        public string OrganizationName { get; set; } = string.Empty;
        public string PreparedBy { get; set; } = string.Empty;
        public string CommitteeMembers { get; set; } = string.Empty;
    }

    #endregion

    #region Combined ViewModels

    /// <summary>
    /// ViewModel لنقل عنصر من نموذج 2 إلى نموذج 5
    /// </summary>
    public class TransferToForm5ViewModel
    {
        public int Form2ItemId { get; set; }
        public Form2Item? Form2Item { get; set; }
        
        [Required(ErrorMessage = "الكمية المستهلكة مطلوبة")]
        [Display(Name = "الكمية المستهلكة")]
        public int ConsumedQuantity { get; set; }

        [Required]
        [Display(Name = "سبب الاستهلاك")]
        public Form5ConsumptionReason Reason { get; set; }

        [Display(Name = "تفاصيل السبب")]
        public string? ReasonDetails { get; set; }

        [Required]
        [Display(Name = "قرار اللجنة")]
        public Form5CommitteeDecision Decision { get; set; }

        [Display(Name = "تفاصيل القرار")]
        public string? DecisionDetails { get; set; }

        [Display(Name = "نسبة الضرر (%)")]
        [Range(0, 100)]
        public decimal? DamagePercentage { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// ViewModel لاستيراد البيانات
    /// </summary>
    public class ImportDataViewModel
    {
        [Display(Name = "نوع الاستيراد")]
        public ImportType Type { get; set; }

        [Display(Name = "السنة")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Display(Name = "القسم/الدائرة")]
        public string? Department { get; set; }

        // بيانات الاستيراد (سيتم ملؤها من الملف أو الإدخال اليدوي)
        public List<Form2ImportRow> Form2Rows { get; set; } = new();
        public List<Form5ImportRow> Form5Rows { get; set; } = new();
    }

    public enum ImportType
    {
        [Display(Name = "نموذج 2 - الموجودات")]
        Form2 = 1,
        [Display(Name = "نموذج 5 - المستهلكات")]
        Form5 = 2
    }

    /// <summary>
    /// صف استيراد لنموذج 2
    /// </summary>
    public class Form2ImportRow
    {
        public int SequenceNumber { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public string? MaterialDescription { get; set; }
        public string CodeNumber { get; set; } = string.Empty;
        public int QuantityByInventory { get; set; }
        public int QuantityByRecords { get; set; }
        public string? LocationPlace { get; set; }
        public string CostInDinarRaw { get; set; } = string.Empty; // لمعالجة الفواصل
        public string Condition { get; set; } = "جيدة";
        public string? Ownership { get; set; }
        public string? Notes { get; set; }
        public bool IsValid { get; set; } = true;
        public string? ValidationError { get; set; }
    }

    /// <summary>
    /// صف استيراد لنموذج 5
    /// </summary>
    public class Form5ImportRow
    {
        public int SequenceNumber { get; set; }
        public string MaterialItems { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PurchaseYear { get; set; }
        public string OriginalUnitPriceRaw { get; set; } = string.Empty;
        public string OriginalTotalPriceRaw { get; set; } = string.Empty;
        public string? DamagePercentage { get; set; }
        public string? UsageDuration { get; set; }
        public string ConsumptionReason { get; set; } = string.Empty;
        public string CommitteeDecision { get; set; } = string.Empty;
        public bool IsValid { get; set; } = true;
        public string? ValidationError { get; set; }
    }

    #endregion
}
