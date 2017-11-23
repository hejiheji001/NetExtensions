using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PacificEpoch.Lib.Utils.ExpressionTree.Extensions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties
{
	public class InArrayConvention : PropertyExpressionConventionBase
	{
		public InArrayConvention() : base(20) { }

		public override Expression BuildExpression(BuildPropertyExpressionContext context)
		{
			return context.Property.PropertyType != typeof(string) ? null : GetInArrayValueExpression(context, typeof(string));
		}

		private static Expression GetInArrayValueExpression(BuildPropertyExpressionContext context, Type type)
		{
			var name = $"{context.Property.Name}-in";
			var arrayQueryValue = context.ValueProvider.GetQueryValue(name, type);
			context.ModelState.AddIfValueNotNull(name, arrayQueryValue.ModelState);
			context.IsHandled |= arrayQueryValue.ModelState != null;
			if (arrayQueryValue.Value == null) return null;
			var arrayValue = arrayQueryValue.Value.ToString().Split(',').Select(s => s.Trim());
			return Expression.Call(typeof(Enumerable), "Contains", new[] { typeof(string) }, Expression.Constant(arrayValue, typeof(IEnumerable<string>)), context.PropertyExpression, Expression.Constant(StringComparer.OrdinalIgnoreCase));
		}
	}
}