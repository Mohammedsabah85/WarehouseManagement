using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// نموذج الجرد السنوي للمواد
    /// </summary>
    public class AnnualInventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "المادة")]
        public int MaterialId { get; set; }

        [Required]
        [Display(Name = "السنة")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "الكمية بموجب الجرد")]
        public int ActualQuantity { get; set; }

        [Required]
        [Display(Name = "الكمية بموجب السجلات")]
        public int RecordedQuantity { get; set; }

        [Display(Name = "الفرق")]
        public int Difference => ActualQuantity - RecordedQuantity;

        [Display(Name = "الحالة")]
        public MaterialCondition Condition { get; set; } = MaterialCondition.Good;

        [Display(Name = "العائدية")]
        [StringLength(200)]
        public string? Department { get; set; }

        [Display(Name = "محل التواجد")]
        [StringLength(200)]
        public string? ActualLocation { get; set; }

        [Display(Name = "تاريخ الجرد")]
        public DateTime InventoryDate { get; set; } = DateTime.Now;

        [Display(Name = "المسؤول عن الجرد")]
        [StringLength(100)]
        public string? InventoryBy { get; set; }

        [Display(Name = "الملاحظات")]
        [StringLength(500)]
        public string? Notes { get; set; }

        [Display(Name = "تم التوثيق")]
        public bool IsDocumented { get; set; } = false;

        [Display(Name = "رقم الاستمارة")]
        [StringLength(50)]
        public string? FormNumber { get; set; }

        // Navigation Properties
        public virtual Material Material { get; set; } = null!;
    }

    /// <summary>
    /// حالة المادة
    /// </summary>
    public enum MaterialCondition
    {
        [Display(Name = "ممتازة")]
        Excellent = 1,
        [Display(Name = "جيدة")]
        Good = 2,
        [Display(Name = "متوسطة")]
        Average = 3,
        [Display(Name = "ضعيفة")]
        Poor = 4,
        [Display(Name = "تالفة")]
        Damaged = 5
    }
}