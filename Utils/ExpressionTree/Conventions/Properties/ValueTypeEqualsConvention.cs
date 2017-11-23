using System.Linq.Expressions;
using PacificEpoch.Lib.Utils.ExpressionTree.Extensions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties {
    public class ValueTypeEqualsConvention : PropertyExpressionConventionBase {

        public ValueTypeEqualsConvention():base(1) {}

        public override Expression BuildExpression(BuildPropertyExpressionContext context) {
            if (!context.Property.PropertyType.IsValueType) return null;

            var queryValue = context.ValueProvider.GetQueryValue(context.Property.Name, context.Property.PropertyType);
            context.ModelState.AddIfValueNotNull(context.Property.Name, queryValue.ModelState);
            context.IsHandled = queryValue.ModelState != null;

            return queryValue.Value == null ? null : context.PropertyExpression.Equal(Expression.Constant(queryValue.Value));
        }
    }
}