using System;
using System.Globalization;
using System.Linq.Expressions;
using PacificEpoch.Lib.Utils.ExpressionTree.Extensions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties {
    /// <summary>
    /// 日期等于约定，匹配名称以 day 或 date （不区分大小写）结束的 DateTime、DateTime? 属性
    /// </summary>
    public class DateEqualsConvention: PropertyExpressionConventionBase {
        public DateEqualsConvention():base(10) { }

        public override Expression BuildExpression(BuildPropertyExpressionContext context) {
            if (context.Property.PropertyType.NotIn(typeof(DateTime), typeof(DateTime?))) return null;
            if (!context.Property.Name.EndsWith("day", true, CultureInfo.CurrentCulture) && 
                !context.Property.Name.EndsWith("date", true, CultureInfo.CurrentCulture)) return null;

            var queryValue = context.ValueProvider.GetQueryValue(context.Property.Name, typeof(DateTime));
            context.ModelState.AddIfValueNotNull(context.Property.Name, queryValue.ModelState);
            context.IsHandled = queryValue.ModelState != null;
            if (queryValue.Value == null) return null;

            var date = ((DateTime)queryValue.Value).Date;
            var expression = context.PropertyExpression;
            if (expression.Type == typeof(DateTime?)) expression = expression.Property("Value");
            return expression.Property("Date").Equal(Expression.Constant(date));
        }
    }
}
