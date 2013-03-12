using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using System.Collections;
using System.Reflection;

namespace Hexa.Core.Domain
{
    public class NHFetchProvider : IFetchProvider
    {
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            var selector = GetFullPropertyName(relatedObjectSelector);
            string[] paths = selector.Split('.');

            var nHibQuery = query.Provider as NHibernate.Linq.DefaultQueryProvider;

            var currentQueryable = query;
            foreach (string path in paths)
            {
                // We always start with the resulting element type
                var currentType = currentQueryable.ElementType;
                var isFirstFetch = true;

                // Gather information about the property
                var propInfo = currentType.GetProperty(path);
                var propType = propInfo.PropertyType;

                // When this is the first segment of a path, we have to use Fetch instead of ThenFetch
                var propFetchFunctionName = (isFirstFetch ? "Fetch" : "ThenFetch");

                // The delegateType is a type for the lambda creation to create the correct return value
                System.Type delegateType;

                if (typeof(IEnumerable).IsAssignableFrom(propType) || typeof(ICollection).IsAssignableFrom(propType))
                {
                    // We have to use "FetchMany" or "ThenFetchMany" when the target property is a collection
                    propFetchFunctionName += "Many";

                    // We only support IList<T> or something similar
                    propType = propType.GetGenericArguments().Single();
                    delegateType = typeof(Func<,>).MakeGenericType(currentType,
                                                                    typeof(IEnumerable<>).MakeGenericType(propType));
                }
                else
                {
                    delegateType = typeof(Func<,>).MakeGenericType(currentType, propType);
                }

                // Get the correct extension method (Fetch, FetchMany, ThenFetch, or ThenFetchMany)
                var fetchMethodInfo = typeof(EagerFetchingExtensionMethods).GetMethod(propFetchFunctionName,
                                                                                    BindingFlags.Static |
                                                                                    BindingFlags.Public |
                                                                                    BindingFlags.InvokeMethod);
                var fetchMethodTypes = new List<System.Type>();
                fetchMethodTypes.AddRange(currentQueryable.GetType().GetGenericArguments().Take(isFirstFetch ? 1 : 2));
                fetchMethodTypes.Add(propType);
                fetchMethodInfo = fetchMethodInfo.MakeGenericMethod(fetchMethodTypes.ToArray());

                // Create an expression of type new delegateType(x => x.{seg.Name})
                var exprParam = System.Linq.Expressions.Expression.Parameter(currentType, "x");
                var exprProp = System.Linq.Expressions.Expression.Property(exprParam, path);
                var exprLambda = System.Linq.Expressions.Expression.Lambda(delegateType, exprProp,
                                                                            new System.Linq.Expressions.
                                                                                ParameterExpression[] { exprParam });

                // Call the *Fetch* function
                var args = new object[] { currentQueryable, exprLambda };
                currentQueryable = ((IQueryable)fetchMethodInfo.Invoke(null, args)).Cast<TOriginating>();

                currentType = propType;
                isFirstFetch = false;
            }



            var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
            return new NHFetchRequest<TOriginating, TRelated>(fetch);
        }

        // code adjusted to prevent horizontal overflow
        private static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
                return string.Empty;

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        // code adjusted to prevent horizontal overflow
        private static bool TryFindMemberExpression(Expression exp, out MemberExpression memberExp)
        {
            memberExp = exp as MemberExpression;
            if (memberExp != null)
            {
                // heyo! that was easy enough
                return true;
            }

            // if the compiler created an automatic conversion,
            // it'll look something like...
            // obj => Convert(obj.Property) [e.g., int -> object]
            // OR:
            // obj => ConvertChecked(obj.Property) [e.g., int -> long]
            // ...which are the cases checked in IsConversion
            if (IsConversion(exp) && exp is UnaryExpression)
            {
                memberExp = ((UnaryExpression)exp).Operand as MemberExpression;
                if (memberExp != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsConversion(Expression exp)
        {
            return (
                exp.NodeType == ExpressionType.Convert ||
                exp.NodeType == ExpressionType.ConvertChecked
            );
        }
    }
}
