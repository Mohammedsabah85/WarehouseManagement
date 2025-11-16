using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods للتعامل مع Enums بطريقة أسهل
    /// تساعد في عرض النصوص العربية للقيم المختلفة
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// يحصل على النص المعروض للـ Enum من خاصية Display
        /// مثال: LocationType.Warehouse.GetDisplayName() سيعيد "مخزن"
        /// </summary>
        /// <param name="enumValue">قيمة الـ Enum</param>
        /// <returns>النص المعروض أو اسم الـ Enum إذا لم يوجد Display</returns>
        public static string GetDisplayName(this Enum enumValue)
        {
            // الحصول على معلومات الحقل للقيمة
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

            if (fieldInfo == null)
                return enumValue.ToString();

            // البحث عن خاصية Display
            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();

            // إرجاع النص من Display أو اسم الـ Enum
            return displayAttribute?.Name ?? enumValue.ToString();
        }

        /// <summary>
        /// يحصل على الوصف للـ Enum من خاصية Display
        /// </summary>
        /// <param name="enumValue">قيمة الـ Enum</param>
        /// <returns>الوصف أو null إذا لم يوجد</returns>
        public static string? GetDisplayDescription(this Enum enumValue)
        {
            var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
            var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute?.Description;
        }

        /// <summary>
        /// يحصل على جميع القيم المتاحة للـ Enum مع أسمائها المعروضة
        /// مفيد لإنشاء قوائم منسدلة
        /// </summary>
        /// <typeparam name="T">نوع الـ Enum</typeparam>
        /// <returns>قاموس يحتوي على القيم والأسماء</returns>
        public static Dictionary<T, string> GetDisplayValues<T>() where T : struct, Enum
        {
            var result = new Dictionary<T, string>();

            foreach (T value in Enum.GetValues<T>())
            {
                result[value] = value.GetDisplayName();
            }

            return result;
        }

        /// <summary>
        /// يحول string إلى Enum بطريقة آمنة
        /// </summary>
        /// <typeparam name="T">نوع الـ Enum</typeparam>
        /// <param name="value">القيمة النصية</param>
        /// <returns>قيمة الـ Enum أو null إذا فشل التحويل</returns>
        public static T? ToEnum<T>(this string value) where T : struct, Enum
        {
            if (Enum.TryParse<T>(value, true, out var result))
                return result;
            return null;
        }
    }
}
