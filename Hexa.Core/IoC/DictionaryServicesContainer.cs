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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    using log4net;

    using Microsoft.Practices.ServiceLocation;

    internal class DictionaryServicesContainer : ServiceLocatorImplBase
    {
        #region Fields

        private static readonly ILog log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IDictionary<Type, object> instances;
        private readonly IDictionary<Type, Type> types;

        #endregion Fields

        #region Constructors

        public DictionaryServicesContainer()
        {
            this.types = new Dictionary<Type, Type>();
            this.instances = new Dictionary<Type, object>();
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private DictionaryServicesContainer(IDictionary<Type, Type> services, IDictionary<Type, object> instances)
        {
            this.types = services;
            this.instances = instances;
        }

        #endregion Constructors

        #region Methods

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DictionaryServicesContainer Clone()
        {
            return new DictionaryServicesContainer(new Dictionary<Type, Type>(this.types),
                                                   new Dictionary<Type, object>(this.instances));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix"
                         , MessageId = "T"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void OverrideInstance<I>(object instance)
        {
            lock (this.instances)
            {
                if (this.instances.ContainsKey(typeof(I)))
                {
                    this.instances.Remove(typeof(I));
                }
            }

            this.RegisterInstance<I>(instance);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OverrideType(Type @interface, Type @type)
        {
            lock (this.types)
            {
                if (this.types.ContainsKey(@interface))
                {
                    this.types.Remove(@interface);
                }
            }

            this.RegisterType(@interface, @type);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix"
                         , MessageId = "T"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void OverrideType<I, T>()
            where T : I
        {
            this.OverrideType(typeof(I), typeof(T));
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter"),
        SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix"
                         , MessageId = "T"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I")]
        public void RegisterInstance<I>(object instance)
        {
            lock (this.instances)
            {
                this.instances.Add(typeof(I), instance);
            }
        }

        public void RegisterInstance(Type @interface, object instance)
        {
            lock (this.instances)
            {
                this.instances.Add(@interface, instance);
            }
        }

        public void RegisterType(Type @interface, Type @type)
        {
            lock (this.types)
            {
                this.types.Add(@interface, @type);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix"
                         , MessageId = "T"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void RegisterType<I, T>()
            where T : I
        {
            this.RegisterType(typeof(I), typeof(T));
        }

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
                         MessageId = "System.String.Format(System.String,System.Object)")]
        public object Resolve(Type type)
        {
            try
            {
                if (this.instances.ContainsKey(type))
                {
                    return this.instances[type];
                }

                if (this.types.ContainsKey(type))
                {
                    return this.ConstructObject(this.types[type]);
                }

                if (type.IsGenericType)
                {
                    Type genericType = this.types[type.GetGenericTypeDefinition()];
                    Type[] args = type.GetGenericArguments();

                    lock (this.types)
                    {
                        this.types.Add(type, genericType.MakeGenericType(args));
                    }

                    return this.ConstructObject(this.types[type]);
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unable to resolve key: {0}",
                                                        type.AssemblyQualifiedName));
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Unable to resolve key: {0}", type.AssemblyQualifiedName), ex);
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"),
        SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix"
                         , MessageId = "T"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"),
        SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public I Resolve<I>()
        {
            return (I)this.Resolve(typeof(I));
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
        {
            try
            {
                return new List<object>
                {
                    this.Resolve(serviceType)
                };
            }
            catch
            {
                return new List<object>();
            }
        }

        protected override object DoGetInstance(Type serviceType, string key)
        {
            return this.Resolve(serviceType);
        }

        private object ConstructObject(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];
            ParameterInfo[] parameters = constructor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                var objects = new List<object>();
                foreach (ParameterInfo p in parameters)
                {
                    objects.Add(Resolve(p.ParameterType));
                }

                return constructor.Invoke(objects.ToArray());
            }
        }

        #endregion Methods
    }
}