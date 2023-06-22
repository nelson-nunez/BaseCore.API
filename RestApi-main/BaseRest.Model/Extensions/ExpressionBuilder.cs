using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BaseRest.Core.Model.Extensions
{
    public enum OperatorComparer
    {
        Contains,
        StartsWith,
        EndsWith,
        Equals = ExpressionType.Equal,
        GreaterThan = ExpressionType.GreaterThan,
        GreaterThanOrEqual = ExpressionType.GreaterThanOrEqual,
        LessThan = ExpressionType.LessThan,
        LessThanOrEqual = ExpressionType.LessThan,
        NotEqual = ExpressionType.NotEqual
    }

    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> BuildPredicate<T>(object value, OperatorComparer comparer, params string[] properties)
        {
            var parameterExpression = Expression.Parameter(typeof(T), typeof(T).Name);
            return (Expression<Func<T, bool>>)BuildNavigationExpression(parameterExpression, comparer, value, properties);
        }

        private static Expression BuildNavigationExpression(Expression parameter, OperatorComparer comparer, object value, params string[] properties)
        {
            Expression resultExpression = null;
            Expression childParameter, predicate;
            Type childType = null;

            if (properties.Count() > 1)
            {
                //build path
                parameter = Expression.Property(parameter, properties[0]);
                var isCollection = typeof(IEnumerable).IsAssignableFrom(parameter.Type);
                //if it´s a collection we later need to use the predicate in the methodexpressioncall
                if (isCollection)
                {
                    childType = parameter.Type.GetGenericArguments()[0];
                    childParameter = Expression.Parameter(childType, childType.Name);
                }
                else
                {
                    childParameter = parameter;
                }
                //skip current property and get navigation property expression recursivly
                var innerProperties = properties.Skip(1).ToArray();
                predicate = BuildNavigationExpression(childParameter, comparer, value, innerProperties);
                if (isCollection)
                {
                    //build subquery
                    resultExpression = BuildSubQuery(parameter, childType, predicate);
                }
                else
                {
                    resultExpression = predicate;
                }
            }
            else
            {
                //build final predicate
                resultExpression = BuildCondition(parameter, properties[0], comparer, value);
            }

            return resultExpression;
        }

        private static Expression BuildSubQuery(Expression parameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            predicate = Expression.Call(anyMethod, parameter, predicate);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildCondition(Expression parameter, string property, OperatorComparer comparer, object value)
        {
            var childProperty = parameter.Type.GetProperty(property);
            var left = Expression.Property(parameter, childProperty);
            //var right = Expression.Constant(Convert.ChangeType(value, left.Type));

            object castedValue = null;
            var typeName = childProperty.PropertyType.Name;
            var nullType = Nullable.GetUnderlyingType(childProperty.PropertyType);
            if (nullType != null)
                typeName = nullType.Name;

            switch (typeName)
            {
                case "Int16":
                case "Int32":
                case "Int64":
                    try
                    {
                        castedValue = Convert.ToInt64(value);
                    }
                    catch (Exception)
                    {
                        throw new Exception("El valor para el filtro no es valido. Se esperaba Numero Entero");
                    }

                    break;
                case "DateTime":
                    try
                    {
                        castedValue = Convert.ToDateTime(value);
                    }
                    catch (Exception)
                    {
                        throw new Exception("El valor para el filtro no es valido. Se esperaba Fecha en formato 'dd/mm/aaaa'");
                    }
                    break;
                default:
                    try
                    {
                        castedValue = Convert.ToString(value);
                    }
                    catch (Exception)
                    {
                        throw new Exception($"El valor para el filtro no es valido. Se esperaba {childProperty.PropertyType.Name}");
                    }

                    break;
            }

            var right = Expression.Constant(castedValue);
            var predicate = BuildComparsion(left, comparer, right);
            return MakeLambda(parameter, predicate);
        }

        private static Expression BuildComparsion(Expression left, OperatorComparer comparer, Expression right)
        {
            var mask = new List<OperatorComparer> {
            OperatorComparer.Contains,
            OperatorComparer.StartsWith,
            OperatorComparer.EndsWith };

            //FIX: Entity.Property ToString for comparing with string filter valuer (MGOMEZ)
            //left = Expression.Call(Expression.Convert(left, typeof(object)), typeof(object).GetMethod("ToString"));

            if (mask.Contains(comparer) && left.Type != typeof(string))
            {
                comparer = OperatorComparer.Equals;
            }
            if (!mask.Contains(comparer))
            {
                return Expression.MakeBinary((ExpressionType)comparer, left, Expression.Convert(right, left.Type));
            }
            return BuildStringCondition(left, comparer, right);
        }

        private static Expression BuildStringCondition(Expression left, OperatorComparer comparer, Expression right)
        {
            //var compareMethod = typeof(string).GetMethods().Single(m => m.Name.Equals(Enum.GetName(typeof(OperatorComparer), comparer)) && m.GetParameters().Count() == 1);
            var compareMethod = typeof(string).GetMethods().First(m => m.Name.Equals(Enum.GetName(typeof(OperatorComparer), comparer)) && m.GetParameters().Count() == 1);
            //we assume ignoreCase, so call ToLower on paramter and memberexpression
            var toLowerMethod = typeof(string).GetMethods().Single(m => m.Name.Equals("ToLower") && m.GetParameters().Count() == 0);
            left = Expression.Call(left, toLowerMethod);
            right = Expression.Call(right, toLowerMethod);
            return Expression.Call(left, compareMethod, right);
        }

        private static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }
    }
}
