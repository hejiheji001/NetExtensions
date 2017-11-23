using System.Web.Mvc;

namespace Utils.ExpressionTree {
    public class QueryConditionBinderAttribute : CustomModelBinderAttribute {
        public override IModelBinder GetBinder() {
            return new QueryConditionExpressionModelBinder();
        }
    }
}