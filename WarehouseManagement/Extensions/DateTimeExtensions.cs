using System.Globalization;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods للتعامل مع التواريخ
    /// تساعد في تنسيق التواريخ بالطريقة العربية
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// يحول التاريخ إلى نص عربي مقروء
        /// مثال: "منذ يومين" أو "قبل ساعة"
        /// </summary>
        public static string ToArabicTimeAgo(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "الآن";
            else if (timeSpan.TotalMinutes < 60)
                return $"منذ {(int)timeSpan.TotalMinutes} دقيقة";
            else if (timeSpan.TotalHours < 24)
                return $"منذ {(int)timeSpan.TotalHours} ساعة";
            else if (timeSpan.TotalDays < 30)
                return $"منذ {(int)timeSpan.TotalDays} يوم";
            else if (timeSpan.TotalDays < 365)
                return $"منذ {(int)(timeSpan.TotalDays / 30)} شهر";
            else
                return $"منذ {(int)(timeSpan.TotalDays / 365)} سنة";
        }

        /// <summary>
        /// يتحقق من أن التاريخ في الماضي
        /// </summary>
        public static bool IsInPast(this DateTime dateTime)
        {
            return dateTime < DateTime.Now;
        }

        /// <summary>
        /// يتحقق من أن التاريخ في المستقبل
        /// </summary>
        public static bool IsInFuture(this DateTime dateTime)
        {
            return dateTime > DateTime.Now;
        }

        /// <summary>
        /// يحصل على بداية اليوم (00:00:00)
        /// </summary>
        public static DateTime StartOfDay(this DateTime dateTime)
        {
            return dateTime.Date;
        }

        /// <summary>
        /// يحصل على نهاية اليوم (23:59:59)
        /// </summary>
        public static DateTime EndOfDay(this DateTime dateTime)
        {
            return dateTime.Date.AddDays(1).AddTicks(-1);
        }

        /// <summary>
        /// يحول التاريخ إلى التاريخ الهجري
        /// </summary>
        public static string ToHijriString(this DateTime dateTime)
        {
            var hijriCalendar = new HijriCalendar();
            var hijriDate = hijriCalendar.GetYear(dateTime) + "/" +
                           hijriCalendar.GetMonth(dateTime) + "/" +
                           hijriCalendar.GetDayOfMonth(dateTime);
            return hijriDate;
        }

        /// <summary>
        /// تنسيق التاريخ بالطريقة العربية
        /// </summary>
        public static string ToArabicDateString(this DateTime dateTime)
        {
            var culture = new CultureInfo("ar-SA");
            return dateTime.ToString("dd MMMM yyyy", culture);
        }

        /// <summary>
        /// يتحقق من أن التاريخ اليوم
        /// </summary>
        public static bool IsToday(this DateTime dateTime)
        {
            return dateTime.Date == DateTime.Today;
        }

        /// <summary>
        /// يحصل على عدد أيام العمل بين تاريخين (بدون الجمعة والسبت)
        /// </summary>
        public static int GetWorkDays(this DateTime startDate, DateTime endDate)
        {
            int workDays = 0;
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                    workDays++;
            }
            return workDays;
        }
    }
}
