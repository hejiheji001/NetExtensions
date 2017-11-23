using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using Utils.ExpressionTree.Conventions;
using Utils.ExpressionTree.Conventions.Combines;
using Utils.ExpressionTree.Conventions.Properties;
using Utils.ExpressionTree.Extensions;

namespace Utils.ExpressionTree {
    public class QueryConditionExpressionModelBinder : IModelBinder {
        private ConventionConfiguration _conventionConfiguration;

        public QueryConditionExpressionModelBinder(ConventionConfiguration conventionConfiguration) {
            _conventionConfiguration = conventionConfiguration;
        }

        public QueryConditionExpressionModelBinder(): this(ConventionConfiguration.Default) { }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var modelType = GetModelTypeFromExpressionType(bindingContext.ModelType);
            if (modelType == null) return null;

            var parameter = Expression.Parameter(modelType, modelType.Name[0].ToString());

            var dict = new Dictionary<string, Expression>();
            var propertyExpressionConvernions = _conventionConfiguration.GetConventions<IPropertyExpressionConvention>();
            foreach (var property in modelType.GetProperties()){
                foreach (var convention in propertyExpressionConvernions) {
                    var context = new BuildPropertyExpressionContext(
                        property,
                        bindingContext.ValueProvider,
                        controllerContext.Controller.ViewData.ModelState,
                        parameter.Property(property.Name)
                        );
                    var expression = convention.BuildExpression(context);
                    if(expression != null){
                        dict.Add(property.Name, expression);
                        break;
                    }
                    if (context.IsHandled) break;
                }
            }
            var body = default(Expression);
            foreach (var convention in _conventionConfiguration.GetConventions<IExpressionCombineConvention>())
            {
                body = convention.Combine(dict);
                if (body != null) break;
            }
            //if (body == null) body = Expression.Constant(true);
            return body.ToLambda(parameter);
        }
        /// <summary>
        /// 获取 Expression<Func<TXXX, bool>> 中 TXXX 的类型
        /// </summary>
        private Type GetModelTypeFromExpressionType(Type lambdaExpressionType) {

            if (lambdaExpressionType.GetGenericTypeDefinition() != typeof (Expression<>)) return null;

            var funcType = lambdaExpressionType.GetGenericArguments()[0];
            if (funcType.GetGenericTypeDefinition() != typeof (Func<,>)) return null;

            var funcTypeArgs = funcType.GetGenericArguments();
            if (funcTypeArgs[1] != typeof (bool)) return null;
            return funcTypeArgs[0];
        }
        /// <summary>
        /// 获取属性的查询值并处理 Controller.ModelState
        /// </summary>
        private object GetValueAndHandleModelState(PropertyInfo property, IValueProvider valueProvider, ControllerBase controller) {
            var result = valueProvider.GetValue(property.Name);
            if (result == null) return null;

            var modelState = new ModelState {Value = result};
            controller.ViewData.ModelState.Add(property.Name, modelState);

            object value = null;
            try{
                value = result.ConvertTo(property.PropertyType);
            }
            catch (Exception ex){
                modelState.Errors.Add(ex);
            }
            return value;
        }
    }
}