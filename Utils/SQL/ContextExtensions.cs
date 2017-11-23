
using Utils.LINQ;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using WebGrease.Css.Extensions;

namespace Utils.SQL
{
	public static class ContextExtensions
	{
		public static string GetTableName<T>(this DbContext context) where T : class
		{
			ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

			return objectContext.GetTableName<T>();
		}

		public static string GetTableName<T>(this ObjectContext context) where T : class
		{
			string sql = context.CreateObjectSet<T>().ToTraceString();
			Regex regex = new Regex("FROM (?<table>.*) AS");
			Match match = regex.Match(sql);

			string table = match.Groups["table"].Value;
			return table;
		}

		public static List<T> DeleteObsoleteData<T>(this List<T> originalData, List<T> latestData, params string[] keys) where T : class
		{
			var len = keys.Length;
			var result = new List<T>();
			latestData.ForEach(d =>
			{
				var lastestValues = new Dictionary<string, dynamic>();
				keys.ForEach(k =>
				{
					lastestValues.Add(k, d.GetPropertyValue<dynamic>(k));
				});

				foreach(var o in originalData)
				{
					var flag = 0;
					lastestValues.Keys.ForEach(k =>
					{
						if(lastestValues[k] == o.GetPropertyValue<dynamic>(k))
						{
							flag++;
						}
					});

					if(flag == 2)
					{
						result.Add(o);
					}
				}
			});

			return result;
		}
	}
}
