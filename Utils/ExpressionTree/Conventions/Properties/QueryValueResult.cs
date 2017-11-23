using System.Web.Mvc;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties {
    public class QueryValueResult {
        public QueryValueResult(object value, ModelState modelState) {
            Value = value;
            ModelState = modelState;
        }
        public object Value { get; private set; }
        public ModelState ModelState { get; private set; }
    }
}