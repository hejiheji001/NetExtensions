using System.Collections.Generic;
using System.Linq.Expressions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Combines {
    public interface IExpressionCombineConvention : IConvention {
        Expression Combine(IDictionary<string, Expression> expressions);
    }
}
