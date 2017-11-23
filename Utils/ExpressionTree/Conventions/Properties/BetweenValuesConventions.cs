using System;
using System.Linq.Expressions;
using Utils.ExpressionTree.Extensions;

namespace Utils.ExpressionTree.Conventions.Properties {
    public class BetweenValuesConventions : PropertyExpressionConventionBase {
        public BetweenValuesConventions() : base(20) { }

        public override Expression BuildExpression(BuildPropertyExpressionContext context) {
            if (context.Property.PropertyType.NotIn(typeof(int), typeof(int?), typeof(decimal), typeof(decimal?))) return null;

            Type type = context.Property.PropertyType;
            var upper = GetLargerValueExpression(context, type);
            var lower = GetSmallerValueExpression(context, type);

            if (upper == null && lower == null) return null;
            context.IsHandled = true;
            if (upper != null && lower != null) return upper.AndAlso(lower);
            return upper ?? lower;
        }

        private Expression GetSmallerValueExpression(BuildPropertyExpressionContext context, Type type) {
            var name = $"{context.Property.Name}-start";
            var lowerBoundQueryValue = context.ValueProvider.GetQueryValue(name, type);
            context.ModelState.AddIfValueNotNull(name, lowerBoundQueryValue.ModelState);
            context.IsHandled |= lowerBoundQueryValue.ModelState != null;

            if (lowerBoundQueryValue.Value == null) return null;
            var lowerBound = lowerBoundQueryValue.Value;
            return Expression.GreaterThanOrEqual(
                context.PropertyExpression.Type == type ?
                context.PropertyExpression :
                context.PropertyExpression.Property("Value"),
                Expression.Constant(lowerBound)
            );
        }

        private Expression GetLargerValueExpression(BuildPropertyExpressionContext context, Type type) {
            var name = $"{context.Property.Name}-end";
            var upperBoundValue = context.ValueProvider.GetQueryValue(name, type);
            context.ModelState.AddIfValueNotNull(name, upperBoundValue.ModelState);
            context.IsHandled |= upperBoundValue.ModelState != null;

            if (upperBoundValue.Value == null) return null;
            var upperBound = upperBoundValue.Value;
            return Expression.LessThanOrEqual(
                context.PropertyExpression.Type == type ?
                context.PropertyExpression :
                context.PropertyExpression.Property("Value"),
                Expression.Constant(upperBound)
            );
        }
    }
}

