using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods للتعامل مع HTML
    /// تساعد في إنشاء عناصر HTML مخصصة
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// ينشئ badge ملون حسب الكمية
        /// </summary>
        public static IHtmlContent QuantityBadge(this IHtmlHelper htmlHelper, int quantity, int lowThreshold = 10)
        {
            var badgeClass = quantity == 0 ? "bg-danger" :
                           quantity <= lowThreshold ? "bg-warning" : "bg-success";

            var badgeText = quantity == 0 ? "نفدت" :
                          quantity <= lowThreshold ? "قليل" : "متوفر";

            return new HtmlString($"<span class=\"badge {badgeClass}\">{badgeText}</span>");
        }

        /// <summary>
        /// ينشئ badge لحالة انتهاء الصلاحية
        /// </summary>
        public static IHtmlContent ExpiryBadge(this IHtmlHelper htmlHelper, DateTime? expiryDate)
        {
            if (!expiryDate.HasValue)
                return new HtmlString("<span class=\"text-muted\">غير محدد</span>");

            var daysRemaining = (expiryDate.Value - DateTime.Now).Days;

            if (daysRemaining <= 0)
                return new HtmlString("<span class=\"badge bg-danger\">منتهية</span>");
            else if (daysRemaining <= 7)
                return new HtmlString("<span class=\"badge bg-warning\">عاجل</span>");
            else if (daysRemaining <= 30)
                return new HtmlString("<span class=\"badge bg-info\">قريباً</span>");
            else
                return new HtmlString("<span class=\"badge bg-success\">سليمة</span>");
        }

        /// <summary>
        /// ينشئ أيقونة مع نص
        /// </summary>
        public static IHtmlContent IconText(this IHtmlHelper htmlHelper, string iconClass, string text)
        {
            return new HtmlString($"<i class=\"{iconClass}\"></i> {text}");
        }

        /// <summary>
        /// ينشئ رابط مع أيقونة
        /// </summary>
        public static IHtmlContent ActionLinkWithIcon(this IHtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, string iconClass, string cssClass = "")
        {
            var urlHelper = htmlHelper.ViewContext.HttpContext.RequestServices.GetService(typeof(IUrlHelper)) as IUrlHelper;
            var url = urlHelper?.Action(actionName, controllerName, routeValues);

            return new HtmlString($"<a href=\"{url}\" class=\"{cssClass}\"><i class=\"{iconClass}\"></i> {linkText}</a>");
        }

        /// <summary>
        /// ينشئ progress bar للكمية
        /// </summary>
        public static IHtmlContent QuantityProgressBar(this IHtmlHelper htmlHelper, int currentQuantity, int maxQuantity)
        {
            if (maxQuantity <= 0) return new HtmlString("");

            var percentage = (int)((double)currentQuantity / maxQuantity * 100);
            var progressClass = percentage < 25 ? "bg-danger" :
                               percentage < 50 ? "bg-warning" : "bg-success";

            return new HtmlString($@"
                <div class=""progress"" style=""height: 20px;"">
                    <div class=""progress-bar {progressClass}"" role=""progressbar"" 
                         style=""width: {percentage}%"" 
                         aria-valuenow=""{currentQuantity}"" 
                         aria-valuemin=""0"" 
                         aria-valuemax=""{maxQuantity}"">
                        {currentQuantity}/{maxQuantity}
                    </div>
                </div>");
        }
    }
}
