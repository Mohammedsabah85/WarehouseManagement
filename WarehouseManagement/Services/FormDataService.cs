using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using WarehouseManagement.Models;
using WarehouseManagement.Models.ViewModels;

namespace WarehouseManagement.Services
{
    /// <summary>
    /// خدمة معالجة البيانات للنماذج الحكومية
    /// Data Processing Service for Government Forms
    /// </summary>
    public interface IFormDataService
    {
        /// <summary>
        /// تحويل النص العربي للرقم إلى قيمة عددية
        /// </summary>
        decimal ParseArabicNumber(string input);

        /// <summary>
        /// معالجة الرموز الخاصة ("=" و "-")
        /// </summary>
        string ProcessSpecialSymbol(string? currentValue, string? previousValue, string fieldName);

        /// <summary>
        /// معالجة قيمة الفرق ("-" تعني صفر)
        /// </summary>
        int ProcessDifferenceValue(string? value);

        /// <summary>
        /// التحقق من صحة رقم الترميز
        /// </summary>
        bool ValidateCodeNumber(string codeNumber);

        /// <summary>
        /// تنسيق الرقم للعرض بالعربية
        /// </summary>
        string FormatNumberForDisplay(decimal number);

        /// <summary>
        /// معالجة صفوف الاستيراد لنموذج 2
        /// </summary>
        List<Form2Item> ProcessForm2Import(List<Form2ImportRow> rows, int year, string? department);

        /// <summary>
        /// معالجة صفوف الاستيراد لنموذج 5
        /// </summary>
        List<Form5Item> ProcessForm5Import(List<Form5ImportRow> rows, string department, DateTime reportDate);
    }

    public class FormDataService : IFormDataService
    {
        /// <summary>
        /// تحويل النص العربي للرقم إلى قيمة عددية
        /// يعالج: الفواصل، الأرقام العربية، المسافات
        /// أمثلة: "118,130,000" → 118130000
        ///        "١٢٣,٤٥٦" → 123456
        /// </summary>
        public decimal ParseArabicNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // إزالة المسافات
            string cleaned = input.Trim();

            // معالجة الرموز الخاصة
            if (cleaned == "=" || cleaned == "-" || cleaned == "ـ")
                return 0;

            // تحويل الأرقام العربية إلى إنجليزية
            cleaned = ConvertArabicNumerals(cleaned);

            // إزالة الفواصل (فاصلة الآلاف)
            cleaned = cleaned.Replace(",", "").Replace("٬", "");

            // إزالة أي أحرف غير رقمية ماعدا النقطة العشرية
            cleaned = Regex.Replace(cleaned, @"[^\d.]", "");

            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }

        /// <summary>
        /// تحويل الأرقام العربية إلى إنجليزية
        /// </summary>
        private string ConvertArabicNumerals(string input)
        {
            var arabicNumerals = new Dictionary<char, char>
            {
                {'٠', '0'}, {'١', '1'}, {'٢', '2'}, {'٣', '3'}, {'٤', '4'},
                {'٥', '5'}, {'٦', '6'}, {'٧', '7'}, {'٨', '8'}, {'٩', '9'},
                {'۰', '0'}, {'۱', '1'}, {'۲', '2'}, {'۳', '3'}, {'۴', '4'},
                {'۵', '5'}, {'۶', '6'}, {'۷', '7'}, {'۸', '8'}, {'۹', '9'}
            };

            var result = input.ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (arabicNumerals.TryGetValue(result[i], out var englishNumeral))
                {
                    result[i] = englishNumeral;
                }
            }
            return new string(result);
        }

        /// <summary>
        /// معالجة الرموز الخاصة
        /// "=" = نسخ من السطر السابق
        /// "-" = حسب السياق (صفر أو غير محدد)
        /// </summary>
        public string ProcessSpecialSymbol(string? currentValue, string? previousValue, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(currentValue))
                return previousValue ?? string.Empty;

            var trimmed = currentValue.Trim();

            // "=" يعني نسخ من السطر السابق
            if (trimmed == "=")
            {
                return previousValue ?? string.Empty;
            }

            // "-" المعنى يعتمد على نوع الحقل
            if (trimmed == "-" || trimmed == "ـ")
            {
                switch (fieldName.ToLower())
                {
                    case "locationplace":
                    case "محل التواجد":
                        return "القسم"; // القيمة الافتراضية

                    case "ownership":
                    case "العائدية":
                        return string.Empty; // غير محدد

                    case "materialdescription":
                    case "وصف المادة":
                        return string.Empty; // غير محدد

                    case "difference":
                    case "الفرق":
                        return "0"; // صفر

                    default:
                        return string.Empty;
                }
            }

            return trimmed;
        }

        /// <summary>
        /// معالجة قيمة الفرق
        /// "-" تعني صفر (لا فرق)
        /// </summary>
        public int ProcessDifferenceValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0;

            var trimmed = value.Trim();

            if (trimmed == "-" || trimmed == "ـ")
                return 0;

            if (int.TryParse(ConvertArabicNumerals(trimmed.Replace(",", "")), out var result))
                return result;

            return 0;
        }

        /// <summary>
        /// التحقق من صحة رقم الترميز
        /// يجب أن يكون بصيغة x/y/z مثل 1/1/1 أو 2/1/4
        /// </summary>
        public bool ValidateCodeNumber(string codeNumber)
        {
            if (string.IsNullOrWhiteSpace(codeNumber))
                return false;

            // نمط الترميز: أرقام مفصولة بـ /
            var pattern = @"^\d+(/\d+)+$";
            return Regex.IsMatch(codeNumber.Trim(), pattern);
        }

        /// <summary>
        /// تنسيق الرقم للعرض
        /// </summary>
        public string FormatNumberForDisplay(decimal number)
        {
            return number.ToString("N0", new CultureInfo("ar-IQ"));
        }

        /// <summary>
        /// معالجة صفوف الاستيراد لنموذج 2
        /// </summary>
        public List<Form2Item> ProcessForm2Import(List<Form2ImportRow> rows, int year, string? department)
        {
            var result = new List<Form2Item>();
            string? prevDescription = null;
            string? prevLocation = null;
            string? prevOwnership = null;

            foreach (var row in rows)
            {
                try
                {
                    var item = new Form2Item
                    {
                        SequenceNumber = row.SequenceNumber,
                        MaterialName = row.MaterialName,
                        MaterialDescription = ProcessSpecialSymbol(row.MaterialDescription, prevDescription, "MaterialDescription"),
                        CodeNumber = row.CodeNumber,
                        QuantityByInventory = row.QuantityByInventory,
                        QuantityByRecords = row.QuantityByRecords,
                        StoredDifference = row.QuantityByInventory - row.QuantityByRecords,
                        LocationPlace = ProcessSpecialSymbol(row.LocationPlace, prevLocation, "LocationPlace"),
                        CostInDinar = ParseArabicNumber(row.CostInDinarRaw),
                        Condition = row.Condition,
                        Ownership = ProcessSpecialSymbol(row.Ownership, prevOwnership, "Ownership"),
                        Notes = row.Notes,
                        InventoryYear = year,
                        Department = department,
                        InventoryDate = DateTime.Now,
                        CreatedDate = DateTime.Now,
                        LastUpdated = DateTime.Now
                    };

                    // التحقق من وجود إشارة لنموذج 5
                    if (!string.IsNullOrEmpty(row.Notes) && 
                        (row.Notes.Contains("استمارة رقم(5)") || 
                         row.Notes.Contains("استمارة رقم (5)") ||
                         row.Notes.Contains("نموذج 5") ||
                         row.Notes.Contains("تم التثبيت")))
                    {
                        item.IsDocumentedInForm5 = true;
                    }

                    // حفظ القيم للصف التالي
                    prevDescription = item.MaterialDescription;
                    prevLocation = item.LocationPlace;
                    prevOwnership = item.Ownership;

                    row.IsValid = true;
                    result.Add(item);
                }
                catch (Exception ex)
                {
                    row.IsValid = false;
                    row.ValidationError = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// معالجة صفوف الاستيراد لنموذج 5
        /// </summary>
        public List<Form5Item> ProcessForm5Import(List<Form5ImportRow> rows, string department, DateTime reportDate)
        {
            var result = new List<Form5Item>();

            foreach (var row in rows)
            {
                try
                {
                    var item = new Form5Item
                    {
                        SequenceNumber = row.SequenceNumber,
                        MaterialItems = row.MaterialItems,
                        Quantity = row.Quantity,
                        PurchaseYear = row.PurchaseYear,
                        OriginalUnitPriceDinar = ParseArabicNumber(row.OriginalUnitPriceRaw),
                        OriginalTotalPriceDinar = ParseArabicNumber(row.OriginalTotalPriceRaw),
                        DamagePercentage = string.IsNullOrEmpty(row.DamagePercentage) ? null : ParseArabicNumber(row.DamagePercentage),
                        UsageDuration = row.UsageDuration,
                        ConsumptionReason = ParseConsumptionReason(row.ConsumptionReason),
                        CommitteeDecision = ParseCommitteeDecision(row.CommitteeDecision),
                        Department = department,
                        ReportDate = reportDate,
                        CreatedDate = DateTime.Now,
                        LastUpdated = DateTime.Now
                    };

                    // حساب الثمن الكلي إذا لم يكن موجوداً
                    if (item.OriginalTotalPriceDinar == 0 && item.OriginalUnitPriceDinar > 0)
                    {
                        item.OriginalTotalPriceDinar = item.Quantity * item.OriginalUnitPriceDinar;
                    }

                    row.IsValid = true;
                    result.Add(item);
                }
                catch (Exception ex)
                {
                    row.IsValid = false;
                    row.ValidationError = ex.Message;
                }
            }

            return result;
        }

        /// <summary>
        /// تحويل نص سبب الاستهلاك إلى enum
        /// </summary>
        private Form5ConsumptionReason ParseConsumptionReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                return Form5ConsumptionReason.NormalUse;

            var trimmed = reason.Trim().ToLower();

            if (trimmed.Contains("جراء الاستعمال") || trimmed.Contains("الاستعمال"))
                return Form5ConsumptionReason.NormalUse;
            if (trimmed.Contains("القدم") || trimmed.Contains("قديم"))
                return Form5ConsumptionReason.Obsolescence;
            if (trimmed.Contains("عطل") || trimmed.Contains("خلل"))
                return Form5ConsumptionReason.TechnicalFailure;
            if (trimmed.Contains("تقصير"))
                return Form5ConsumptionReason.NegligenceByOfficials;
            if (trimmed.Contains("حادث"))
                return Form5ConsumptionReason.Accident;
            if (trimmed.Contains("سوء"))
                return Form5ConsumptionReason.Misuse;

            return Form5ConsumptionReason.NormalUse;
        }

        /// <summary>
        /// تحويل نص قرار اللجنة إلى enum
        /// </summary>
        private Form5CommitteeDecision ParseCommitteeDecision(string decision)
        {
            if (string.IsNullOrWhiteSpace(decision))
                return Form5CommitteeDecision.UnderReview;

            var trimmed = decision.Trim().ToLower();

            if (trimmed.Contains("إعادة استعمال كلي") || trimmed.Contains("كلي"))
                return Form5CommitteeDecision.FullReuse;
            if (trimmed.Contains("إعادة استعمال") || trimmed.Contains("جزئي"))
                return Form5CommitteeDecision.PartialReuse;
            if (trimmed.Contains("بيع"))
                return Form5CommitteeDecision.Sell;
            if (trimmed.Contains("إتلاف") || trimmed.Contains("اتلاف"))
                return Form5CommitteeDecision.Dispose;
            if (trimmed.Contains("إصلاح") || trimmed.Contains("اصلاح"))
                return Form5CommitteeDecision.Repair;
            if (trimmed.Contains("خردة"))
                return Form5CommitteeDecision.Scrap;

            return Form5CommitteeDecision.UnderReview;
        }
    }

    /// <summary>
    /// قواعد معالجة الرموز الخاصة
    /// </summary>
    public static class SymbolProcessingRules
    {
        /// <summary>
        /// جدول معاني الرموز حسب الحقل
        /// </summary>
        public static readonly Dictionary<string, SymbolMeaning> FieldSymbolMeanings = new()
        {
            // حقل محل التواجد
            ["LocationPlace"] = new SymbolMeaning
            {
                FieldNameArabic = "محل التواجد",
                EqualsMeaning = "نسخ من السطر السابق",
                DashMeaning = "القسم (القيمة الافتراضية)",
                DefaultValue = "القسم"
            },
            
            // حقل الفرق
            ["Difference"] = new SymbolMeaning
            {
                FieldNameArabic = "الفرق",
                EqualsMeaning = "غير مطبق",
                DashMeaning = "صفر (لا فرق)",
                DefaultValue = "0"
            },
            
            // حقل العائدية
            ["Ownership"] = new SymbolMeaning
            {
                FieldNameArabic = "العائدية",
                EqualsMeaning = "نسخ من السطر السابق",
                DashMeaning = "غير محدد",
                DefaultValue = ""
            },
            
            // حقل وصف المادة
            ["MaterialDescription"] = new SymbolMeaning
            {
                FieldNameArabic = "وصف المادة",
                EqualsMeaning = "نسخ من السطر السابق",
                DashMeaning = "غير محدد",
                DefaultValue = ""
            },
            
            // حقل الكلفة
            ["CostInDinar"] = new SymbolMeaning
            {
                FieldNameArabic = "الكلفة بالدينار",
                EqualsMeaning = "غير مطبق",
                DashMeaning = "صفر",
                DefaultValue = "0"
            }
        };
    }

    /// <summary>
    /// تعريف معاني الرموز لحقل معين
    /// </summary>
    public class SymbolMeaning
    {
        public string FieldNameArabic { get; set; } = string.Empty;
        public string EqualsMeaning { get; set; } = string.Empty;
        public string DashMeaning { get; set; } = string.Empty;
        public string DefaultValue { get; set; } = string.Empty;
    }
}
