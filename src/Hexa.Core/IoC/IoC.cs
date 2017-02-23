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
        private static Action<Type, object> registerInstanceCallback;
        private static Action<Type, Type> registerTypeCallback;
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
        /// <param name="registerCallback">The register callback.</param>
        public static void Initialize(
            Action<Type, Type> registerType,
            Action<Type, object> registerInstance,
            Func<Type, object> resolve,
            Func<Type, IEnumerable<object>> resolveAll)
        {
            registerTypeCallback = registerType;
            registerInstanceCallback = registerInstance;
            resolveCallback = resolve;
            resolveAllCallback = resolveAll;
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="instance">The instance.</param>
        public static void RegisterInstance<I>(object instance)
        {
            if (registerInstanceCallback != null)
            {
                registerInstanceCallback(typeof(I), instance);
            }
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="instance">The instance.</param>
        public static void RegisterInstance(Type @type, object instance)
        {
            if (registerInstanceCallback != null)
            {
                registerInstanceCallback(@type, instance);
            }
        }

        /// <summary>
        /// Registers a service implementation.
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        public static void RegisterType<I, T>()
        where T : I
        {
            if (registerTypeCallback != null)
            {
                registerTypeCallback(typeof(I), typeof(T));
            }
        }

        /// <summary>
        /// Registers the type.
        /// </summary>
        /// <param name="interface">The @interface.</param>
        /// <param name="type">The type.</param>
        public static void RegisterType(Type @interface, Type @type)
        {
            if (registerTypeCallback != null)
            {
                registerTypeCallback(@interface, @type);
            }
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