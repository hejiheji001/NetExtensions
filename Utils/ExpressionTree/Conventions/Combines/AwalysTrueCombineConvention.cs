using System.Collections.Generic;
using System.Linq.Expressions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Combines {
    public class AwalysTrueCombineConvention : IExpressionCombineConvention {

        public AwalysTrueCombineConvention() {
            Order = int.MinValue;
        }

        public int Order { get; set; }

        public Expression Combine(IDictionary<string, Expression> expressions) {
            return Expression.Constant(true);
        }

    }
}
