using System.Linq.Expressions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties {
    public interface IPropertyExpressionConvention: IConvention {
        Expression BuildExpression(BuildPropertyExpressionContext context);
    }
}
