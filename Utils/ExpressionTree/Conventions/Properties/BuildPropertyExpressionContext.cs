using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Utils.ExpressionTree.Conventions.Properties {
    public class BuildPropertyExpressionContext {
        public BuildPropertyExpressionContext(PropertyInfo property, IValueProvider valueProvider, ModelStateDictionary modelState, Expression propertyExpression) {
            Property = property;
            ValueProvider = valueProvider;
            ModelState = modelState;
            PropertyExpression = propertyExpression;
        }
        public PropertyInfo Property { get; private set; }
        public IValueProvider ValueProvider { get; private set; }
        public Expression PropertyExpression { get; private set; }
        public ModelStateDictionary ModelState { get; set; }
        public bool IsHandled { get; set; }
    }
}
