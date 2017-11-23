using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PacificEpoch.Lib.Utils.ExpressionTree.Extensions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Combines {
    public class AndCombineConvention : IExpressionCombineConvention {
        public int Order { get; set; }
        public Expression Combine(IDictionary<string, Expression> expressions)
        {
	        return expressions.Count > 0 ? expressions.Values.Aggregate((a, e) => a.AndAlso(e)) : null;
        }
    }
}
