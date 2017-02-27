//----------------------------------------------------------------------------------------------
// <copyright file="IoC.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class IoC
    {
        private static Func<Type, IEnumerable<object>> resolveAllCallback;
        private static Func<Type, object> resolveCallback;

        public static TDependency[] GetAllInstances<TDependency>()
        {
            if (resolveAllCallback != null)
            {
                IEnumerable<object> services = resolveAllCallback(typeof(TDependency));

                if (services != null)
                {
                    return services.Cast<TDependency>().ToArray();
                }
            }

            return new List<TDependency>().ToArray();
        }

        public static TDependency GetInstance<TDependency>()
        {
            return (TDependency)GetInstance(typeof(TDependency));
        }

        public static object GetInstance(Type dependencyType)
        {
            return resolveCallback(dependencyType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IoC"/> class.
        /// </summary>
        public static void Initialize(
            Func<Type, object> resolve,
            Func<Type, IEnumerable<object>> resolveAll)
        {
            resolveCallback = resolve;
            resolveAllCallback = resolveAll;
        }

        public static TDependency TryGetInstance<TDependency>()
        {
            try
            {
                IEnumerable<TDependency> services = GetAllInstances<TDependency>();

                if (services != null)
                {
                    return (TDependency)services.FirstOrDefault();
                }
            }
            catch (NullReferenceException)
            {
                throw new InternalException("ServiceLocator has not been initialized; " +
                                            "I was trying to retrieve " + typeof(TDependency));
            }

            return default(TDependency);
        }
    }
}