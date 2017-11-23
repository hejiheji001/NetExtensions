using System;
using System.Collections.Generic;
using System.Linq;

namespace Utils.ExpressionTree.Extensions {

	public static class ContainsExtensions
	{
        public static bool Contains(this string t, params string[] c) {
            return c.Any(i => t.Contains(i));
        }

        public static bool Contains(this string t, IEnumerable<string> c) {
            return c.Any(i => t.Contains(i));
        }

		public static bool Contains(this string t, StringComparison s, params string[] c)
		{
			return c.Any(i => t.IndexOf(i, s) > -1);
		}
	}
}
