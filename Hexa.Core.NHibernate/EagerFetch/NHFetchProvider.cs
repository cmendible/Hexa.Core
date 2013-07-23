// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================
// Inspired on: https://github.com/PeteGoo/NHibernate.QueryService/blob/master/NHibernateQueryService.WebApi/ActionFilters/QueryableWithExpandsAttribute.cs

namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using NHibernate.Linq;

    public class NHFetchProvider : IFetchProvider
    {
        // code adjusted to prevent horizontal overflow
        public static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface))
                        {
                            continue;
                        }

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                                             BindingFlags.FlattenHierarchy
                                             | BindingFlags.Public
                                             | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                                           .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                                      | BindingFlags.Public | BindingFlags.Instance);
        }

        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        where TOriginating : class
        {
            var selector = GetFullPropertyName(relatedObjectSelector);
            string[] paths = selector.Split('.');

            var nHibQuery = query.Provider as NHibernate.Linq.DefaultQueryProvider;

            IQueryable currentQueryable = query.AsQueryable<TOriginating>();

            // We always start with the resulting element type
            var currentType = currentQueryable.ElementType;
            var isFirstFetch = true;
            foreach (string path in paths)
            {
                // Gather information about the property
                var propInfo = GetPublicProperties(currentType).Where(p => p.Name == path).SingleOrDefault();
                var propType = propInfo.PropertyType;

                // When this is the first segment of a path, we have to use Fetch instead of ThenFetch
                var propFetchFunctionName = isFirstFetch ? "Fetch" : "ThenFetch";

                // The delegateType is a type for the lambda creation to create the correct return value
                System.Type delegateType;

                if (typeof(IEnumerable).IsAssignableFrom(propType) || typeof(ICollection).IsAssignableFrom(propType))
                {
                    // We have to use "FetchMany" or "ThenFetchMany" when the target property is a collection
                    propFetchFunctionName += "Many";

                    // We only support IList<T> or something similar
                    propType = propType.GetGenericArguments().Single();
                    delegateType = typeof(Func<,>).MakeGenericType(
                                       currentType,
                                       typeof(IEnumerable<>).MakeGenericType(propType));
                }
                else
                {
                    delegateType = typeof(Func<,>).MakeGenericType(currentType, propType);
                }

                // Get the correct extension method (Fetch, FetchMany, ThenFetch, or ThenFetchMany)
                var fetchMethodInfo = typeof(EagerFetchingExtensionMethods).GetMethod(
                                          propFetchFunctionName,
                                          BindingFlags.Static |
                                          BindingFlags.Public |
                                          BindingFlags.InvokeMethod);
                var fetchMethodTypes = new List<System.Type>();
                fetchMethodTypes.AddRange(currentQueryable.GetType().GetGenericArguments().Take(isFirstFetch ? 1 : 2));
                fetchMethodTypes.Add(propType);
                fetchMethodInfo = fetchMethodInfo.MakeGenericMethod(fetchMethodTypes.ToArray());

                // Create an expression of type new delegateType(x => x.{seg.Name})
                Expression exprParam = System.Linq.Expressions.Expression.Parameter(propInfo.DeclaringType, "x");

                Expression exprProp = System.Linq.Expressions.Expression.Property(exprParam, path);
                var exprLambda = System.Linq.Expressions.Expression.Lambda(
                                     delegateType,
                                     exprProp,
                                     new System.Linq.Expressions.
                                     ParameterExpression[] { (ParameterExpression)exprParam });

                // Call the *Fetch* function
                var args = new object[] { currentQueryable, exprLambda };
                currentQueryable = (IQueryable)fetchMethodInfo.Invoke(null, args);

                currentType = propType;
                isFirstFetch = false;
            }

            return new BaseFetchRequest<TOriginating, TRelated>(currentQueryable.Cast<TOriginating>());
        }

        private static string GetFullPropertyName<T, TProperty>(Expression<Func<T, TProperty>> exp)
        {
            MemberExpression memberExp;
            if (!TryFindMemberExpression(exp.Body, out memberExp))
            {
                return string.Empty;
            }

            var memberNames = new Stack<string>();
            do
            {
                memberNames.Push(memberExp.Member.Name);
            }
            while (TryFindMemberExpression(memberExp.Expression, out memberExp));

            return string.Join(".", memberNames.ToArray());
        }

        private static bool IsConversion(Expression exp)
        {
            return exp.NodeType == ExpressionType.Convert || exp.NodeType == ExpressionType.ConvertChecked;
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
    }
}