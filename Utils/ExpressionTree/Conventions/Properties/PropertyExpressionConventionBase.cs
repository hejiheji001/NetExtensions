// Decompiled with JetBrains decompiler
// Type: PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties.PropertyExpressionConventionBase
// Assembly: PacificEpoch.Lib, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8A4C8D4B-59EE-4F98-8F5C-6C58EADEFC42
// Assembly location: C:\Users\Administrator\Desktop\PacificEpoch.Lib.dll

using System.Linq.Expressions;

namespace PacificEpoch.Lib.Utils.ExpressionTree.Conventions.Properties
{
  public abstract class PropertyExpressionConventionBase : IPropertyExpressionConvention, IConvention
  {
    public int Order { get; set; }

    public PropertyExpressionConventionBase(int order)
    {
      this.Order = order;
    }

    public abstract Expression BuildExpression(BuildPropertyExpressionContext context);

    public override string ToString()
    {
      return string.Format("{0,8}{1}", (object) this.Order, (object) this.GetType().Name.Replace("Convention", ""));
    }
  }
}
