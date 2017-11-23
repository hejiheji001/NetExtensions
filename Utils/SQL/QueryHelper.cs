using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

// http://stackoverflow.com/questions/2841585/create-linq-to-entities-orderby-expression-on-the-fly/2841605#2841605
namespace Utils.SQL
{
	public static class QueryHelper
	{
		private static readonly MethodInfo OrderByMethod =
			typeof(Queryable).GetMethods().Single(method =>
		   method.Name == "OrderBy" && method.GetParameters().Length == 2);

		private static readonly MethodInfo OrderByDescendingMethod =
			typeof(Queryable).GetMethods().Single(method =>
		   method.Name == "OrderByDescending" && method.GetParameters().Length == 2);

		private static readonly MethodInfo ThenByMethod =
			typeof(Queryable).GetMethods().Single(method =>
			method.Name == "ThenBy" && method.GetParameters().Length == 2);

		private static readonly MethodInfo ThenByDescendingMethod =
			typeof(Queryable).GetMethods().Single(method =>
		   method.Name == "ThenByDescending" && method.GetParameters().Length == 2);

		public static bool PropertyExists<T>(string propertyName)
		{
			return typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase |
				BindingFlags.Public | BindingFlags.Instance) != null;
		}

		public static IQueryable<T> OrderByProperty<T>(
		   this IQueryable<T> source, string propertyName)
		{
			if (!PropertyExists<T>(propertyName))
			{
				return null;
			}
			ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
			Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
			LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
			MethodInfo genericMethod =
			  OrderByMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
			object ret = genericMethod.Invoke(null, new object[] { source, lambda });
			return (IQueryable<T>)ret;
		}

		public static IQueryable<T> OrderByPropertyDescending<T>(
			this IQueryable<T> source, string propertyName)
		{
			if (!PropertyExists<T>(propertyName))
			{
				return null;
			}
			ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
			Expression orderByProperty = Expression.Property(paramterExpression, propertyName);
			LambdaExpression lambda = Expression.Lambda(orderByProperty, paramterExpression);
			MethodInfo genericMethod =
			  OrderByDescendingMethod.MakeGenericMethod(typeof(T), orderByProperty.Type);
			object ret = genericMethod.Invoke(null, new object[] { source, lambda });
			return (IQueryable<T>)ret;
		}

		public static IQueryable<T> ThenByProperty<T>(
			this IQueryable<T> source, string propertyName)
		{
			if (!PropertyExists<T>(propertyName))
			{
				return null;
			}
			ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
			Expression thenByProperty = Expression.Property(paramterExpression, propertyName);
			LambdaExpression lambda = Expression.Lambda(thenByProperty, paramterExpression);
			MethodInfo genericMethod =
			  ThenByMethod.MakeGenericMethod(typeof(T), thenByProperty.Type);
			object ret = genericMethod.Invoke(null, new object[] { source, lambda });
			return (IQueryable<T>)ret;
		}

		public static IQueryable<T> ThenByPropertyDescending<T>(
			this IQueryable<T> source, string propertyName)
		{
			if (!PropertyExists<T>(propertyName))
			{
				return null;
			}
			ParameterExpression paramterExpression = Expression.Parameter(typeof(T));
			Expression thenByProperty = Expression.Property(paramterExpression, propertyName);
			LambdaExpression lambda = Expression.Lambda(thenByProperty, paramterExpression);
			MethodInfo genericMethod =
			  ThenByDescendingMethod.MakeGenericMethod(typeof(T), thenByProperty.Type);
			object ret = genericMethod.Invoke(null, new object[] { source, lambda });
			return (IQueryable<T>)ret;
		}
	}
}