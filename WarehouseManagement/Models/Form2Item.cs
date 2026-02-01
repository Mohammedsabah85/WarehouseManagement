using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseManagement.Models
{
    /// <summary>
    /// نموذج رقم (2) - قائمة الموجودات الرئيسية
    /// Form 2 - Main Inventory Items List
    /// يمثل سجل الجرد السنوي للموجودات حسب النموذج الحكومي
    /// </summary>
    public class Form2Item
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// التسلسل - ت
        /// </summary>
        [Display(Name = "ت")]
        public int SequenceNumber { get; set; }

        /// <summary>
        /// اسم المادة
        /// </summary>
        [Required(ErrorMessage = "اسم المادة مطلوب")]
        [StringLength(200)]
        [Display(Name = "اسم المادة")]
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// وصف المادة - حديد/خشب/المنيوم/بلاستك/معدن
        /// الرمز "=" يعني نفس قيمة السطر السابق
        /// </summary>
        [StringLength(100)]
        [Display(Name = "وصف المادة (حديد/خشب/المنيوم)")]
        public string? MaterialDescription { get; set; }

        /// <summary>
        /// رقم الترميز - مثل 1/1/1, 2/1/1, 1/1/2
        /// </summary>
        [Required(ErrorMessage = "رقم الترميز مطلوب")]
        [StringLength(20)]
        [Display(Name = "رقم الترميز")]
        public string CodeNumber { get; set; } = string.Empty;

        /// <summary>
        /// الكمية بموجب الجرد (الكمية الفعلية)
        /// </summary>
        [Required]
        [Display(Name = "بموجب الجرد")]
        public int QuantityByInventory { get; set; }

        /// <summary>
        /// الكمية بموجب السجلات (الكمية المسجلة)
        /// </summary>
        [Required]
        [Display(Name = "بموجب السجلات")]
        public int QuantityByRecords { get; set; }

        /// <summary>
        /// الفرق بين الجرد والسجلات (محسوب)
        /// الرمز "-" يعني صفر (لا فرق)
        /// </summary>
        [Display(Name = "الفرق")]
        [NotMapped]
        public int QuantityDifference => QuantityByInventory - QuantityByRecords;

        /// <summary>
        /// الفرق المخزن في قاعدة البيانات
        /// </summary>
        [Display(Name = "الفرق")]
        public int StoredDifference { get; set; }

        /// <summary>
        /// محل التواجد - القسم أو الموقع
        /// الرمز "=" يعني نفس الموقع السابق
        /// </summary>
        [StringLength(200)]
        [Display(Name = "محل التواجد")]
        public string? LocationPlace { get; set; }

        /// <summary>
        /// الكلفة بالدينار - السعر الإجمالي
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "الكلفة بالدينار")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        public decimal CostInDinar { get; set; }

        /// <summary>
        /// الحالة - جيدة/جيد/متوسطة/ضعيفة
        /// </summary>
        [StringLength(50)]
        [Display(Name = "الحالة")]
        public string Condition { get; set; } = "جيدة";

        /// <summary>
        /// العائدية - الجهة المالكة (كهروميكانيك، إلخ)
        /// الرمز "=" يعني نفس الجهة السابقة
        /// </summary>
        [StringLength(200)]
        [Display(Name = "العائدية")]
        public string? Ownership { get; set; }

        /// <summary>
        /// الملاحظات - مثل "تم التثبيت في استمارة رقم(5)"
        /// </summary>
        [StringLength(500)]
        [Display(Name = "الملاحظات")]
        public string? Notes { get; set; }

        /// <summary>
        /// هل تم تثبيت هذا العنصر في نموذج 5 (المستهلكات)
        /// </summary>
        [Display(Name = "تم التثبيت في استمارة (5)")]
        public bool IsDocumentedInForm5 { get; set; } = false;

        /// <summary>
        /// رقم استمارة نموذج 5 المرتبطة
        /// </summary>
        [Display(Name = "رقم استمارة (5)")]
        public int? Form5Id { get; set; }

        /// <summary>
        /// سنة الجرد
        /// </summary>
        [Required]
        [Display(Name = "سنة الجرد")]
        public int InventoryYear { get; set; } = DateTime.Now.Year;

        /// <summary>
        /// تاريخ الجرد
        /// </summary>
        [Display(Name = "تاريخ الجرد")]
        public DateTime InventoryDate { get; set; } = DateTime.Now;

        /// <summary>
        /// القسم/الدائرة
        /// </summary>
        [StringLength(200)]
        [Display(Name = "القسم/الدائرة")]
        public string? Department { get; set; }

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
        /// دفعات الشراء المرتبطة بهذا العنصر
        /// </summary>
        public virtual ICollection<PurchaseBatch> PurchaseBatches { get; set; } = new List<PurchaseBatch>();

        /// <summary>
        /// سجلات نموذج 5 المرتبطة (المستهلكات)
        /// </summary>
        public virtual ICollection<Form5Item> Form5Items { get; set; } = new List<Form5Item>();

        // ربط اختياري بالمادة الأصلية في نظام المخازن
        [Display(Name = "المادة المرتبطة")]
        public int? MaterialId { get; set; }
        public virtual Material? Material { get; set; }
    }

    /// <summary>
    /// حالة المادة في الجرد
    /// </summary>
    public enum Form2Condition
    {
        [Display(Name = "ممتازة")]
        Excellent = 1,
        [Display(Name = "جيدة")]
        Good = 2,
        [Display(Name = "جيد")]
        GoodMale = 3,
        [Display(Name = "متوسطة")]
        Average = 4,
        [Display(Name = "ضعيفة")]
        Poor = 5,
        [Display(Name = "تالفة")]
        Damaged = 6
    }
}
