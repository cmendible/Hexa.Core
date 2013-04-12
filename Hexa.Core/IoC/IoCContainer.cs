#region Header

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

#endregion Header

namespace Hexa.Core
{
    using System;
    using System.Linq;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections.Generic;
    using System.Collections;

    public static class IoCContainer
    {
        #region Fields

        private static Action<Type, object> registerInstanceCallback;
        private static Action<Type, Type> registerTypeCallback;
        private static Func<Type, object> resolveCallback;
        private static Func<Type, IEnumerable<object>> resolveAllCallback;

        #endregion Fields

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCContainer"/> class.
        /// </summary>
        /// <param name="registerCallback">The register callback.</param>
        public static void Initialize(
            Action<Type, Type> registerType, 
            Action<Type, object> registerInstance,
            Func<Type, object> resolve,
            Func<Type, IEnumerable<object>> resolveAll
            )
        {
            registerTypeCallback = registerType;
            registerInstanceCallback = registerInstance;
            resolveCallback = resolve;
            resolveAllCallback = resolveAll;
        }

        #region Methods

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

        public static TDependency[] GetAllInstances<TDependency>()
        {
            IEnumerable<object> services = resolveAllCallback(typeof(TDependency));

            if (services != null)
            {
                return services.Cast<TDependency>().ToArray();
            }

            return default(TDependency[]);
        }

        public static TDependency GetInstance<TDependency>()
        {
            return (TDependency)GetInstance(typeof(TDependency));
        }

        public static object GetInstance(Type dependencyType)
        {
            return resolveCallback(dependencyType);
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

        #endregion Methods
    }
}