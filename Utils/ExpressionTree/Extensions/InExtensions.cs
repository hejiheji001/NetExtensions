using System.Collections.Generic;
using System.Linq;

namespace Utils.ExpressionTree.Extensions {
	/// <summary>
	/// InExtensions
	/// </summary>
	/// <remarks>
	/// c# 扩展方法奇思妙用高级篇一：改进 Scottgu 的 "In" 扩展
	/// http://www.cnblogs.com/ldp615/archive/2009/08/08/1541641.html
	/// </remarks>
	public static class InExtensions {
        public static bool In<T>(this T t, params T[] c) {
            return c.Any(i => i.Equals(t));
        }
        public static bool In<T>(this T t, IEnumerable<T> c) {
            return c.Any(i => i.Equals(t));
        }

        public static bool NotIn<T>(this T t, params T[] c) {
            return c.All(i => ! i.Equals(t));
        }
        public static bool NotIn<T>(this T t, IEnumerable<T> c) {
            return c.All(i => ! i.Equals(t));
        }
    }
}
