using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WarehouseManagement.Models;

namespace WarehouseManagement.Models.ViewModels
{
    /// <summary>
    /// ViewModel لعملية الجرد
    /// </summary>
    public class InventoryProcessViewModel
    {
        [Required]
        [Display(Name = "السنة")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Required]
        [Display(Name = "المسؤول عن الجرد")]
        [StringLength(100)]
        public string InventoryBy { get; set; } = string.Empty;

        public List<InventoryItemViewModel> InventoryItems { get; set; } = new();
    }

    /// <summary>
    /// ViewModel لعنصر الجرد
    /// </summary>
    public class InventoryItemViewModel
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = string.Empty;
        public string MaterialCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string LocationName { get; set; } = string.Empty;

        [Display(Name = "اختر للجرد")]
        public bool IsSelected { get; set; }

        [Display(Name = "الكمية الفعلية")]
        public int ActualQuantity { get; set; }

        [Display(Name = "الكمية المسجلة")]
        public int RecordedQuantity { get; set; }

        [Display(Name = "الفرق")]
        public int Difference => ActualQuantity - RecordedQuantity;

        [Display(Name = "الحالة")]
        public MaterialCondition Condition { get; set; } = MaterialCondition.Good;

        [Display(Name = "العائدية")]
        [StringLength(200)]
        public string? Department { get; set; }

        [Display(Name = "محل التواجد الفعلي")]
        [StringLength(200)]
        public string? ActualLocation { get; set; }

        [Display(Name = "الملاحظات")]
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// ViewModel لتقرير الجرد السنوي
    /// </summary>
    public class InventoryReportViewModel
    {
        public int Year { get; set; }
        public List<AnnualInventory> Inventories { get; set; } = new();
        public int TotalItems { get; set; }
        public int ItemsWithShortage { get; set; }
        public int ItemsWithSurplus { get; set; }
        public int ItemsMatching { get; set; }
        public int TotalShortageQuantity { get; set; }
        public int TotalSurplusQuantity { get; set; }
        public decimal TotalValue { get; set; }
        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        // نسب إحصائية
        public double ShortagePercentage => TotalItems > 0 ? (double)ItemsWithShortage / TotalItems * 100 : 0;
        public double SurplusPercentage => TotalItems > 0 ? (double)ItemsWithSurplus / TotalItems * 100 : 0;
        public double MatchingPercentage => TotalItems > 0 ? (double)ItemsMatching / TotalItems * 100 : 0;
    }

    /// <summary>
    /// ViewModel لتقرير أسعار الموجودات
    /// </summary>
    public class PricingReportViewModel
    {
        public Material Material { get; set; } = null!;
        public List<PurchasePrice> PurchasePrices { get; set; } = new();
        public int TotalQuantityPurchased { get; set; }
        public decimal TotalAmountSpent { get; set; }
        public decimal AveragePrice { get; set; }
        public decimal CurrentValue { get; set; }
        public DateTime? FirstPurchaseDate { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
    }

    /// <summary>
    /// ViewModel لتقرير المستهلكات
    /// </summary>
    public class ConsumedMaterialsReportViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ConsumedMaterial> ConsumedMaterials { get; set; } = new();
        public int TotalConsumedItems { get; set; }
        public decimal TotalOriginalValue { get; set; }
        public decimal TotalResidualValue { get; set; }
        public decimal TotalLoss { get; set; }

        // إحصائيات حسب السبب
        public Dictionary<ConsumptionReason, int> ConsumptionByReason { get; set; } = new();

        // إحصائيات حسب القرار
        public Dictionary<CommitteeDecision, int> DecisionStatistics { get; set; } = new();

        // المواد الأكثر استهلاكاً
        public List<MaterialConsumptionSummary> TopConsumedMaterials { get; set; } = new();
    }

    /// <summary>
    /// ملخص استهلاك المادة
    /// </summary>
    public class MaterialConsumptionSummary
    {
        public string MaterialName { get; set; } = string.Empty;
        public string MaterialCode { get; set; } = string.Empty;
        public int TotalConsumedQuantity { get; set; }
        public decimal TotalLossValue { get; set; }
        public double AverageDamagePercentage { get; set; }
        public int ConsumptionCount { get; set; }
    }
}