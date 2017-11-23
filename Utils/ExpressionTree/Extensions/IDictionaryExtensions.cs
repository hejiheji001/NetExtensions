using System.Collections.Generic;

namespace Utils.ExpressionTree.Extensions {
	public static class IDictionaryExtensions {
        public static IDictionary<TKey, TValue> AddIfValueNotNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
            where TValue : class {
            if (value != null) dictionary.Add(key, value);
            return dictionary;
        }
    }
}
