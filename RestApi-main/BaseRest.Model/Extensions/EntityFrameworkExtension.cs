using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BaseRest.Core.Model.Extensions
{
    public static class EntityFrameworkExtension
    {
        public static object ReflectPropertyValue(object source, string property)
        {
            return source.GetType().GetProperty(property).GetValue(source, null);
        }

  
        public static IQueryable<T> WhereStringContains<T>(this IQueryable<T> query, string propertyName, string contains)
        {
            var result = query;
            var predicate = PredicateBuilder.New<T>();
            //IQueryable<T> result = Enumerable.Empty<T>().AsQueryable();
            //string[] words = contains.Split(',');
            string[] atributes = propertyName.Split(',');

            List<IQueryable<T>> results = new List<IQueryable<T>>();
            //if (words.Count() != atributes.Count())
            //    return null;
            for (int i = 0; i < atributes.Count(); i++)
            {
                var propertyType = typeof(T).GetProperty(atributes[i]).PropertyType;

                var typeName = propertyType.Name;
                var nullType = Nullable.GetUnderlyingType(propertyType);
                if (nullType != null)
                    typeName = nullType.Name;

                var parameter = Expression.Parameter(typeof(T), "type");
                var propertyExpression = Expression.Property(parameter, atributes[i]);
                switch (typeName)
                {
                    case "Int16":
                    case "Int32":
                    case "Int64":
                    case "Boolean":
                    case "DateTime"://TODO: NO FUNCIONA
                        var type = typeof(T);
                        var x = Expression.Parameter(type, "x");
                        var member = Expression.Property(x, atributes[i]);
                        ConstantExpression constant;
                        MethodInfo toStringMethod = typeof(object).GetMethod("ToString");
                        MethodInfo method2 = typeof(string).GetMethod("Equals", new[] { typeof(string) });
                        constant = Expression.Constant(contains);
                        var memberToStringCall = Expression.Call(member, toStringMethod);
                        var call = Expression.Call(memberToStringCall, method2, constant);
                        //query = query.Where(Expression.Lambda<Func<T, bool>>(call, x));
                        predicate = predicate.Or(Expression.Lambda<Func<T, bool>>(Expression.Lambda<Func<T, bool>>(call, x)));
                        break;
                    case "String":
                        MethodInfo method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var someValue = Expression.Constant(contains, typeof(string));
                        var containsExpression = Expression.Call(propertyExpression, method, someValue);
                        //results.Add(query.Where(Expression.Lambda<Func<T, bool>>(containsExpression, parameter)));
                        //query = query.Where(Expression.Lambda<Func<T, bool>>(containsExpression, parameter));
                        predicate = predicate.Or(Expression.Lambda<Func<T, bool>>(containsExpression, parameter));
                        break;
                    default:
                        break;
                }
            }
            return query.Where(predicate);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> query, string propertyName)
        {
            var ordenamiento = "OrderBy";
            if (string.IsNullOrEmpty(propertyName))
            {
                propertyName = "Id";
                ordenamiento = "OrderBy";
            }
            if (propertyName.StartsWith("-"))
            {
                ordenamiento = "OrderByDescending";
                propertyName = propertyName.Replace("-", "");
            }
            var propertyType = typeof(T).GetProperty(propertyName).PropertyType;
            var parameter = Expression.Parameter(typeof(T), "type");
            var propertyExpression = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(propertyExpression, new[] { parameter });

            return typeof(Queryable).GetMethods()
                                    .Where(m => m.Name == ordenamiento && m.GetParameters().Length == 2)
                                    .Single()
                                    .MakeGenericMethod(new[] { typeof(T), propertyType })
                                    .Invoke(null, new object[] { query, lambda }) as IOrderedQueryable<T>;
        }

        public static IOrderedQueryable<T> OrderByMemberPath<T>(this IQueryable<T> query, string propertyName)
        {
            try
            {
                var orderMethodName = "OrderBy";
                if (string.IsNullOrEmpty(propertyName))
                {
                    propertyName = "Id";
                    orderMethodName = "OrderBy";
                }
                if (propertyName.StartsWith("-"))
                {
                    orderMethodName = "OrderByDescending";
                    propertyName = propertyName.Replace("-", "");
                }

                Type type = typeof(T);
                Type propertyType = type.GetProperty(propertyName)?.PropertyType; ;

                var param = Expression.Parameter(type, "x");
                Expression parent = param;

                var keyParts = propertyName.Split('.');
                for (int i = 0; i < keyParts.Length; i++)
                {
                    var keyPart = keyParts[i];
                    parent = Expression.Property(parent, keyPart);

                    if (keyParts.Length > 1)
                    {
                        if (i == 0)
                        {
                            propertyType = type.GetProperty(keyPart).PropertyType;
                        }
                        else
                        {
                            propertyType = propertyType.GetProperty(keyPart).PropertyType;
                        }
                    }
                }

                MethodCallExpression orderByExpression = Expression.Call(
                  typeof(Queryable),
                  orderMethodName,
                  new Type[] { type, propertyType },
                  query.Expression,
                  CreateExpression(type, propertyName)
                );

                return query.Provider.CreateQuery<T>(orderByExpression) as IOrderedQueryable<T>;
            }
            catch (Exception e)
            {
                return query as IOrderedQueryable<T>;
            }
        }

        static LambdaExpression CreateExpression(Type type, string propertyName)
        {
            var param = Expression.Parameter(type, "x");
            Expression body = param;
            foreach (var member in propertyName.Split('.'))
            {
                body = Expression.PropertyOrField(body, member);
            }
            return Expression.Lambda(body, param);
        }

        public enum Comparison
        {
            Equal,
            NotEqual,
            LessThan,
            LessThanOrEqual,
            GreaterThan,
            GreaterThanOrEqual
        }

        public static IQueryable<TSource>
            Compare<TSource, TValue>(this IQueryable<TSource> source, Expression<Func<TSource, TValue>> selector,
            TValue value, Comparison comparison)
        {
            Expression left = selector.Body;
            Expression right = Expression.Constant(value, typeof(TValue));

            BinaryExpression body;
            switch (comparison)
            {
                case Comparison.LessThan:
                    body = Expression.LessThan(left, right);
                    break;
                case Comparison.LessThanOrEqual:
                    body = Expression.LessThanOrEqual(left, right);
                    break;
                case Comparison.Equal:
                    body = Expression.Equal(left, right);
                    break;
                case Comparison.NotEqual:
                    body = Expression.NotEqual(left, right);
                    break;
                case Comparison.GreaterThan:
                    body = Expression.GreaterThan(left, right);
                    break;
                case Comparison.GreaterThanOrEqual:
                    body = Expression.GreaterThanOrEqual(left, right);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison));
            }
            return source.Where(Expression.Lambda<Func<TSource, bool>>(body, selector.Parameters));
        }


        public static IQueryable<T> GetQueryFilterBy<T>(this IQueryable<T> query, string filterBy, string filterValue)
        {
            if (!string.IsNullOrEmpty(filterBy) && !string.IsNullOrEmpty(filterValue))
                //query = query.WhereStringContains(filterBy, filterValue);
                query = query.WhereStringContainsV2<T>(filterBy, filterValue);
            return query;
        }

        public static IQueryable<T> WhereStringContainsV2<T>(this IQueryable<T> query, string filterBy, object filterValue)
        {
            if (!string.IsNullOrEmpty(filterBy))
            {
                var properties = filterBy.Split('|');
                Expression<Func<T, bool>> predicate = null;
                for (var i = 0; i < properties.Length; i++)
                {
                    var filters = properties[i].Split('.');
                    if (i == 0)
                        predicate = ExpressionBuilder.BuildPredicate<T>(filterValue, OperatorComparer.Contains, filters);
                    else
                        predicate = predicate.Or(ExpressionBuilder.BuildPredicate<T>(filterValue, OperatorComparer.Contains, filters));
                }

                query = query.Where(predicate);
            }

            //var predicate = ExpressionBuilder.BuildPredicate<T>(contains, OperatorComparer.Contains, propertyName);
            return query;
        }

        public static IQueryable<T> GetQueryOrderBy<T>(this IQueryable<T> query, string orderBy)
        {
            if (!string.IsNullOrEmpty(orderBy))
                //query = query.OrderBy<T>(orderBy);
                query = query.OrderByMemberPath<T>(orderBy);
            return query;
        }

        public static IQueryable<T> GetQueryPaged<T>(this IQueryable<T> query, int pageSize, int pageIndex, string orderBy)
        {
            if (pageSize > 0)
            {
                if (string.IsNullOrEmpty(orderBy))
                    query = query.OrderBy<T>("Id");
                query = query.Skip(pageSize * pageIndex).Take(pageSize);
                //query = query.Skip(pageSize * (pageIndex-1)).Take(pageSize);
            }

            return query;
        }
    }
}