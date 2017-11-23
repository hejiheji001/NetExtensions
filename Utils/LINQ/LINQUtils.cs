using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Utils.ExpressionTree.Extensions;
using WebGrease.Css.Extensions;

namespace Utils.LINQ
{
	public static class LINQUtils
	{
		#region ExpandoObject To Type With Value

		public static T ConvertToTypeAndPropertiesOf<T>(this ExpandoObject source, T example) where T : class
		{
			try
			{
				IDictionary<string, object> dict = source;

				var ctor = example.GetType().GetConstructors().Single();

				var parameters = ctor.GetParameters();

				var parameterValues2 = new List<dynamic>();
				foreach (var p in parameters)
				{
					dynamic x;
					if (dict.ContainsKey(p.Name))
					{
						x = dict[p.Name];
					}
					else
					{
						var t = p.ParameterType;
						x = t.IsValueType ? Activator.CreateInstance(t) : null;
					}
					parameterValues2.Add(x);

				}

				//var parameterValues = parameters.Select(p => dict.ContainsKey(p.Name) ? dict[p.Name] : DefaultOfType(p.ParameterType)).ToArray();

				return (T)ctor.Invoke(parameterValues2.ToArray());
			}
			catch (Exception e)
			{
				var x = 1;
				return null;
			}
		}
		#endregion

		#region default value of type
		public static object DefaultOfType(Type t)
		{
			return t.IsValueType ? Activator.CreateInstance(t) : null; ;
		}
		#endregion

		#region ExpandoObject To Type
		public static T ConvertToAnonymousType<T>(this string[] source, T example) where T : class
		{
			var ctor = example.GetType().GetConstructors().Single();

			var parameters = ctor.GetParameters();

			var parameterValues = parameters.Select(p => "").ToArray();

			return (T)ctor.Invoke(parameterValues);
		}
		#endregion

		#region AnonymousObjectMutator
		public static T Set<T, TProperty>(this T instance, Expression<Func<T, TProperty>> propExpression, TProperty newValue) where T : class
		{
			const BindingFlags fieldFlags = BindingFlags.NonPublic | BindingFlags.Instance;
			string[] backingFieldFormats = { "<{0}>i__Field", "<{0}>" };
			var pi = (propExpression.Body as MemberExpression)?.Member;
			var backingFieldNames = backingFieldFormats.Select(x => String.Format(x, pi.Name)).ToList();
			var fi = typeof(T)
				.GetFields(fieldFlags)
				.FirstOrDefault(f => backingFieldNames.Contains(f.Name));
			if (fi == null)
				throw new NotSupportedException($"Cannot find backing field for {pi.Name}");
			fi.SetValue(instance, newValue);
			return instance;
		}

		#endregion

		#region Dictionary To Key Value Pair Separate

		public static string ToString(this IDictionary<string, object> source, string separator)
		{
			return String.Join(separator, source.Select(x => x.Key + "=" + x.Value).ToArray());
		}

		#endregion

		#region Get Property Value By Name

		public static T GetPropertyValue<T>(this object obj, string property)
		{
			try
			{
				var propertyInfo = obj.GetType().GetProperty(property);
				var x = propertyInfo.GetValue(obj, null);

				if (x.GetType().NotIn(typeof(decimal), typeof(int), typeof(long), typeof(string), typeof(double)))
				{
					x = default(T);
				}
				return (T)Convert.ChangeType(x, typeof(T));
			}
			catch(Exception e)
			{
				var a = 1;
				return default(T);
			}
			
		}

		#endregion

		#region Set Property Value By Name

		public static void SetPropertyValue<T>(this object obj, string property, object value)
		{
			var propertyInfo = obj.GetType().GetProperty(property);
			propertyInfo.SetValue(obj, value);
		}

		#endregion

		#region Object To Dictionary
		public static IDictionary<string, object> ToDictionary(this object source)
		{
			return source.ToDictionary<object>();
		}

		public static IDictionary<string, object> ToDictionary(this object source, IEnumerable<string> target)
		{
			return source.ToDictionary<object>(target);
		}

		public static IDictionary<string, T> ToDictionary<T>(this object source)
		{
			if (source == null)
			{
				ThrowExceptionWhenSourceArgumentIsNull();
			}

			var dictionary = new Dictionary<string, T>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
			{
				AddPropertyToDictionary(property, source, dictionary);
			}

			return dictionary;
		}

		public static IDictionary<string, T> ToDictionary<T>(this object source, IEnumerable<string> target)
		{
			if (source == null)
			{
				ThrowExceptionWhenSourceArgumentIsNull();
			}

			if (target == null)
			{
				ThrowExceptionWhenSourceArgumentIsNull();
			}

			var dictionary = new Dictionary<string, T>();
			foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
			{
				//var enumerable = target as string[];
				if (property.Name.In(target))
				{
					AddPropertyToDictionary(property, source, dictionary);
				}
			}

			return dictionary;
		}

		private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
		{
			var value = property.GetValue(source) ?? "";
			if (IsOfType<T>(value))
			{
				dictionary.Add(property.Name, (T) value);
			}
		}

		private static bool IsOfType<T>(object value)
		{
			return value is T;
		}

		private static void ThrowExceptionWhenSourceArgumentIsNull()
		{
			throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
		}
		#endregion

		#region Multi Sub Function

		//public static Dictionary<string, decimal> GetSubFuncResult(this IEnumerable<BabaTemplates> source, string subFunc)
		//{
		//	if (subFunc.Equals(""))
		//	{
		//		return null;
		//	}
		//	else
		//	{
		//		var resultDic = new Dictionary<string, decimal>();
		//		var funcs = subFunc.Split(','); // [xxx#yyy, zzz#qqq]
		//		funcs.ForEach(f =>
		//		{
		//			var sumOrAvg = f.Split('#')[0];
		//			var property = f.Split('#')[1];
		//			decimal result = 0;
		//			if (sumOrAvg.Equals("SUM"))
		//			{
		//				result = source.Sum(s => s.GetPropertyValue<decimal>(property));
		//			}
		//			else if (sumOrAvg.Equals("AVG"))
		//			{
		//				result = source.Average(s => s.GetPropertyValue<decimal>(property));
		//			}
		//			resultDic.Add(f, result);
		//		});
		//		return resultDic;
		//	}
		//}

		public static Dictionary<string, decimal> GetSubFuncResult<T>(this IEnumerable<T> source, string subFunc)
		{
			if (subFunc.Equals(""))
			{
				return null;
			}
			else
			{
				var resultDic = new Dictionary<string, decimal>();
				var funcs = subFunc.Split(','); // [xxx#yyy, zzz#qqq]
				funcs.ForEach(f =>
				{
					var sumOrAvg = f.Split('#')[0];
					var property = f.Split('#')[1];
					decimal result = 0;
					if (sumOrAvg.Equals("SUM"))
					{
						result = source.Sum(s => s.GetPropertyValue<decimal>(property));
					}
					else if (sumOrAvg.Equals("AVG"))
					{
						result = source.Average(s => s.GetPropertyValue<decimal>(property));
					}

					if (result > 0)
					{
						resultDic.Add(f, result);
					}
				});
				return resultDic;
			}
		}

		public static Dictionary<string, decimal> GetSubFuncResult<T>(this IEnumerable<T> source, string subFunc, decimal factor = 1m)
		{
			if (subFunc.Equals(""))
			{
				return null;
			}
			else
			{
				var resultDic = new Dictionary<string, decimal>();
				var funcs = subFunc.Split(','); // [xxx#yyy, zzz#qqq]
				funcs.ForEach(f =>
				{
					var sumOrAvg = f.Split('#')[0];
					var property = f.Split('#')[1];
					decimal result = 0;
					if (sumOrAvg.Equals("SUM"))
					{
						result = source.Sum(s => s.GetPropertyValue<decimal>(property)) * factor;
					}
					else if (sumOrAvg.Equals("AVG"))
					{
						result = source.Average(s => s.GetPropertyValue<decimal>(property));
					}

					if (result > 0)
					{
						resultDic.Add(f, result);
					}
				});
				return resultDic;
			}
		}
		#endregion

		#region Cast Expression Tree From Father To Child

		public static LambdaExpression ChangeInputType<T, TResult>(this Expression<Func<T, TResult>> expression, Type newInputType)
		{
			if (!typeof(T).IsAssignableFrom(newInputType))
				throw new Exception($"{typeof(T)} is not assignable from {newInputType}.");
			var beforeParameter = expression.Parameters.Single();
			var afterParameter = Expression.Parameter(newInputType, beforeParameter.Name);
			var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
			return Expression.Lambda(visitor.Visit(expression.Body), afterParameter);
		}

		public static Expression<Func<T2, TResult>> ChangeInputType<T1, T2, TResult>(this Expression<Func<T1, TResult>> expression)
		{
			if (!typeof(T1).IsAssignableFrom(typeof(T2)))
				throw new Exception($"{typeof(T1)} is not assignable from {typeof(T2)}.");
			var beforeParameter = expression.Parameters.Single();
			var afterParameter = Expression.Parameter(typeof(T2), beforeParameter.Name);
			var visitor = new SubstitutionExpressionVisitor(beforeParameter, afterParameter);
			return Expression.Lambda<Func<T2, TResult>>(visitor.Visit(expression.Body), afterParameter);
		}

		private class SubstitutionExpressionVisitor : ExpressionVisitor
		{
			private readonly Expression before;
			private readonly Expression after;

			public SubstitutionExpressionVisitor(Expression before, Expression after)
			{
				this.before = before;
				this.after = after;
			}
			public override Expression Visit(Expression node)
			{
				return node == before ? after : base.Visit(node);
			}
		}
		#endregion

		#region Shuffle An IEnumerable

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
		{
			var elements = source.ToArray();
			for (var i = elements.Length - 1; i >= 0; i--)
			{
				// Swap element "i" with a random earlier element it (or itself)
				// ... except we don't really need to swap it fully, as we can
				// return it immediately, and afterwards it's irrelevant.
				var swapIndex = rng.Next(i + 1);
				yield return elements[swapIndex];
				elements[swapIndex] = elements[i];
			}
		}

		public static object MultiGroup<T>(T mrt, IEnumerable<string> groups, object template)
		{
			string[] strArray = groups as string[] ?? groups.ToArray<string>();
			if (!((IEnumerable<string>)strArray).Any<string>())
				return (object)true;
			IDictionary<string, object> dictionary = ((object)mrt).ToDictionary((IEnumerable<string>)strArray);
			ExpandoObject source = new ExpandoObject();
			foreach (string index in strArray)
			{
				try
				{
					((IDictionary<string, object>)source)[index] = dictionary[index];
				}
				catch (Exception ex)
				{
					throw;
				}
			}
			return source.ConvertToTypeAndPropertiesOf<object>(template);
		}

		#endregion
	}
}

