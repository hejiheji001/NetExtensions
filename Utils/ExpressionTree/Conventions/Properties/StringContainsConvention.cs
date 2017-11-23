using System;
using System.Linq.Expressions;
using PacificEpoch.Lib.Utils.ExpressionTree.Extensions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties {
    public class StringContainsConvention : PropertyExpressionConventionBase {

        public StringContainsConvention():base(10) { }

        public override Expression BuildExpression(BuildPropertyExpressionContext context) {
            if (context.Property.PropertyType != typeof(string)) return null;

            var queryValue = context.ValueProvider.GetQueryValue(context.Property.Name, context.Property.PropertyType);
            context.ModelState.AddIfValueNotNull(context.Property.Name, queryValue.ModelState);
            context.IsHandled = queryValue.ModelState != null;

			var indexOf = Expression.Call(context.PropertyExpression, "IndexOf", null,
			Expression.Constant(queryValue.Value, typeof(string)),
			Expression.Constant(StringComparison.InvariantCultureIgnoreCase));

			return (queryValue.Value as string).IsNullOrEmpty() ? null : Expression.GreaterThanOrEqual(indexOf, Expression.Constant(0));
        }
    }
}