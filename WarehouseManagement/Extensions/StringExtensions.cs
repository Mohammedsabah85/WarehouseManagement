using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods للتعامل مع النصوص
    /// تساعد في معالجة النصوص العربية والإنجليزية
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// يتحقق من أن النص ليس فارغاً أو null
        /// </summary>
        public static bool IsNotEmpty(this string? str)
        {
            return !string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// يتحقق من أن النص ليس فارغاً أو مساحات فقط
        /// </summary>
        public static bool IsNotWhiteSpace(this string? str)
        {
            return !string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// يقطع النص إلى طول محدد مع إضافة نقاط
        /// مفيد لعرض المعاينات
        /// </summary>
        /// <param name="str">النص الأصلي</param>
        /// <param name="maxLength">الطول الأقصى</param>
        /// <returns>النص المقطوع</returns>
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str) || str.Length <= maxLength)
                return str;

            return str.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// ينظف النص من المسافات الزائدة
        /// </summary>
        public static string CleanWhitespace(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return Regex.Replace(str.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// يحول النص إلى تنسيق Title Case مناسب للعربية
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var textInfo = CultureInfo.CurrentCulture.TextInfo;
            return textInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// يتحقق من أن النص يحتوي على أرقام فقط
        /// مفيد للتحقق من أرقام المواد
        /// </summary>
        public static bool IsNumeric(this string str)
        {
            return !string.IsNullOrEmpty(str) && str.All(char.IsDigit);
        }

        /// <summary>
        /// يتحقق من أن النص يحتوي على أحرف عربية
        /// </summary>
        public static bool ContainsArabic(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            return str.Any(c => c >= 0x0600 && c <= 0x06FF);
        }

        /// <summary>
        /// يحول النص إلى slug مناسب للـ URLs
        /// </summary>
        public static string ToSlug(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            // تحويل للأحرف الصغيرة
            str = str.ToLower();

            // استبدال المسافات بـ dash
            str = Regex.Replace(str, @"\s+", "-");

            // إزالة الأحرف غير المرغوبة
            str = Regex.Replace(str, @"[^a-z0-9\u0600-\u06FF\-]", "");

            // إزالة الـ dashes المتتالية
            str = Regex.Replace(str, @"-+", "-");

            return str.Trim('-');
        }
    }
}