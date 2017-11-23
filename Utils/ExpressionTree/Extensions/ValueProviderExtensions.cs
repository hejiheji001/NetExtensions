using Utils.ExpressionTree.Conventions.Properties;
using System;
using System.Web.Mvc;

namespace Utils.ExpressionTree.Extensions {
	public static class ValueProviderExtensions {
        public static QueryValueResult GetQueryValue(this IValueProvider valueProvider, string name, Type type) {
            var valueProviderResult = valueProvider.GetValue(name);
            if (valueProviderResult == null || valueProviderResult.AttemptedValue.IsNullOrEmpty()) 
                return new QueryValueResult(null, null);

            var modelState = new ModelState { Value = valueProviderResult };

            object value = null;
            try {
                value = valueProviderResult.ConvertTo(type);
            }
            catch (Exception ex) {
                modelState.Errors.Add(ex);
            }
            return new QueryValueResult(value, modelState);
        }
    }
}
