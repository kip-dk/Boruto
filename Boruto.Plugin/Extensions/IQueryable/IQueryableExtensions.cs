using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Boruto.Extensions.IQueryable
{
    public static class IQueryableExtensions
    {
        public static IQueryable<TSource> WhereIn<TSource, TValue>(this IQueryable<TSource> source, Expression<Func<TSource, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (null == source) { throw new ArgumentNullException("source"); }
            if (null == valueSelector) { throw new ArgumentNullException("valueSelector"); }
            if (null == values) { throw new ArgumentNullException("values"); }

            var equalExpressions = new List<BinaryExpression>();

            foreach (var value in values)
            {
                var equalsExpression = Expression.Equal(valueSelector.Body, Expression.Constant(value));
                equalExpressions.Add(equalsExpression);
            }

            ParameterExpression p = valueSelector.Parameters.Single();
            var combined = equalExpressions.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            var combinedLambda = Expression.Lambda<Func<TSource, bool>>(combined, p);

            return source.Where(combinedLambda);
        }
    }
}
