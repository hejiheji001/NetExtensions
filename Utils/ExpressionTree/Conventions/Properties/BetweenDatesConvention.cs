using System;
using System.Linq.Expressions;
using Utils.ExpressionTree.Extensions;

namespace Utils.ExpressionTree.Conventions.Properties {
    public class BetweenDatesConvention : PropertyExpressionConventionBase {
        public BetweenDatesConvention() : base(20) {}

        public override Expression BuildExpression(BuildPropertyExpressionContext context) {
            if (context.Property.PropertyType.NotIn(typeof (DateTime), typeof (DateTime?))) return null;

            var after = GetAfterStartDateExpression(context);
            var before = GetBeforeEndDateExpression(context);

            if (after == null && before == null) return null;
            context.IsHandled = true;
            if (after != null && before != null)return after.AndAlso(before);
            return after ?? before;
        }

        private Expression GetAfterStartDateExpression(BuildPropertyExpressionContext context) {
            var name = $"{context.Property.Name}-start";
            var startDateQueryValue = context.ValueProvider.GetQueryValue(name, typeof (DateTime));
            context.ModelState.AddIfValueNotNull(name, startDateQueryValue.ModelState);
            context.IsHandled |= startDateQueryValue.ModelState != null;

            if (startDateQueryValue.Value == null) return null;
            var startDate = ((DateTime) startDateQueryValue.Value).Date;
            return Expression.GreaterThanOrEqual(
                context.PropertyExpression.Type == typeof(DateTime) ? context.PropertyExpression : context.PropertyExpression.Property("Value"),
                Expression.Constant(startDate)
                );
        }

        private Expression GetBeforeEndDateExpression(BuildPropertyExpressionContext context) {
            var name = $"{context.Property.Name}-end";
            var endDateQueryValue = context.ValueProvider.GetQueryValue(name, typeof(DateTime));
            context.ModelState.AddIfValueNotNull(name, endDateQueryValue.ModelState);
            context.IsHandled |= endDateQueryValue.ModelState != null;

            if (endDateQueryValue.Value == null) return null;
            var endDate = ((DateTime)endDateQueryValue.Value).Date.AddDays(1);
            return Expression.LessThanOrEqual(
                context.PropertyExpression.Type == typeof(DateTime) ? context.PropertyExpression : context.PropertyExpression.Property("Value"),
                Expression.Constant(endDate)
                );
        }
    }
}