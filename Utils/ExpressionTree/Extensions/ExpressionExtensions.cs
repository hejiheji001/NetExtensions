using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Utils.ExpressionTree.Extensions
{
	/// <summary>
	/// ExpressionExtensions
	/// </summary>
	/// <remark>
	/// c# 扩展方法奇思妙用基础篇九：Expression 扩展
	/// http://www.cnblogs.com/ldp615/archive/2011/09/15/expression-extension-methods.html
	/// </remark>
	public static class ExpressionExtensions {
        public static Expression AndAlso(this Expression left, Expression right) {
            return Expression.AndAlso(left, right);
        }
        public static Expression OrElse(this Expression left, Expression right) {
            return Expression.OrElse(left, right);
        }
        public static Expression Call(this Expression instance, string methodName, params Expression[] arguments) {
            return Expression.Call(instance, instance.Type.GetMethod(methodName), arguments);
        }
        public static Expression Property(this Expression expression, string propertyName) {
	        return Expression.Property(expression, propertyName);
		}

        public static Expression GreaterThan(this Expression left, Expression right) {
            return Expression.GreaterThan(left, right);
        }

        public static Expression Equal(this Expression left, Expression right) {
            return Expression.Equal(left, right);
        }

        public static Expression ToLambda(this Expression body, params  ParameterExpression[] parameters) {
            return Expression.Lambda(body, parameters);
        }
        public static Expression<TDelegate> ToLambda<TDelegate>(this Expression body, params  ParameterExpression[] parameters) {
            return Expression.Lambda<TDelegate>(body, parameters);
        }
		public static bool ContainsIgnoreCase<T>(this List<T> target, T value, IEqualityComparer<T> comparer)
		{
			return target.Contains(value, comparer);
		}
	}
}
