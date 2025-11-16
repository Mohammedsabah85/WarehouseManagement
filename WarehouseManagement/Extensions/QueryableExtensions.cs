using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WarehouseManagement.Extensions
{
    /// <summary>
    /// Extension Methods للتعامل مع قواعد البيانات
    /// تساعد في إنشاء استعلامات أكثر مرونة
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// يطبق الفلترة إذا كان الشرط صحيحاً
        /// مفيد للبحث المشروط
        /// </summary>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        /// يطبق الترتيب المشروط
        /// </summary>
        public static IQueryable<T> OrderByIf<T, TKey>(this IQueryable<T> query, bool condition, Expression<Func<T, TKey>> keySelector)
        {
            return condition ? query.OrderBy(keySelector) : query;
        }

        /// <summary>
        /// يطبق الترتيب التنازلي المشروط
        /// </summary>
        public static IQueryable<T> OrderByDescendingIf<T, TKey>(this IQueryable<T> query, bool condition, Expression<Func<T, TKey>> keySelector)
        {
            return condition ? query.OrderByDescending(keySelector) : query;
        }

        /// <summary>
        /// يطبق التصفح (Pagination) بطريقة آمنة
        /// </summary>
        public static async Task<PagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }

        /// <summary>
        /// البحث النصي في عدة حقول
        /// </summary>
        public static IQueryable<T> SearchInFields<T>(this IQueryable<T> query, string searchTerm, params Expression<Func<T, string>>[] fields)
        {
            if (string.IsNullOrWhiteSpace(searchTerm) || !fields.Any())
                return query;

            Expression<Func<T, bool>>? combinedExpression = null;

            foreach (var field in fields)
            {
                var containsExpression = Expression.Call(
                    field.Body,
                    typeof(string).GetMethod("Contains", new[] { typeof(string) })!,
                    Expression.Constant(searchTerm));

                var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, field.Parameters[0]);

                combinedExpression = combinedExpression == null ? lambda : combinedExpression.Or(lambda);
            }

            return combinedExpression != null ? query.Where(combinedExpression) : query;
        }
    }

    /// <summary>
    /// فئة مساعدة لنتائج التصفح
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    /// <summary>
    /// Extension Methods للتعبيرات
    /// </summary>
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(left!, right!), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression? Visit(Expression? node)
            {
                return node == _oldValue ? _newValue : base.Visit(node);
            }
        }
    }
}
