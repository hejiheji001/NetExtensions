using Utils.Universal;

namespace Utils.ExpressionTree.Extensions {
    /// <summary>
    /// StringExtensions
    /// </summary>
    /// <remarks>
    /// c# 扩展方法奇思妙用基础篇二：string 常用扩展
    /// http://www.cnblogs.com/ldp615/archive/2009/08/14/1546437.html
    /// </remarks>
    public static class StringExtensions {
        public static bool IsNullOrEmpty(this string s) {
            return string.IsNullOrEmpty(s);
        }

        public static bool IsNotNullAndEmpty(this string s) {
            return !string.IsNullOrEmpty(s);
        }

		public static string NoBraces(this string str)
		{
			return str.Split("(")[0].Trim().Split("（")[0].Trim();
		}
	}
}
