#region License

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

#endregion

using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

namespace Hexa.Core
{
	internal class DictionaryServicesContainer : ServiceLocatorImplBase
	{
		private static readonly log4net.ILog _Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private readonly IDictionary<Type, Type> _Types = null;
		private readonly IDictionary<Type, object> _Instances = null;

		public DictionaryServicesContainer()
		{
			_Types = new Dictionary<Type, Type>();
			_Instances = new Dictionary<Type, object>();
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private DictionaryServicesContainer(IDictionary<Type, Type> services, IDictionary<Type, object> instances)
		{
			_Types = services;
			_Instances = instances;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DictionaryServicesContainer Clone()
		{
			return new DictionaryServicesContainer(new Dictionary<Type, Type>(_Types), new Dictionary<Type, object>(_Instances));
		}

		public void RegisterType(Type @interface, Type @type)
		{
            lock (_Types)
            {
                _Types.Add(@interface, @type);
            }
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void RegisterType<I, T>()
				where T : I
		{
			this.RegisterType(typeof(I), typeof(T));
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I")]
        public void RegisterInstance<I>(object instance)
		{
            lock (_Instances)
            {
                _Instances.Add(typeof(I), instance);
            }
		}

        public void RegisterInstance(Type @interface, object instance)
        {
            lock (_Instances)
            {
                _Instances.Add(@interface, instance);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public object Resolve(Type type)
		{
			try
			{
				if (_Instances.ContainsKey(type))
					return _Instances[type];

                if (_Types.ContainsKey(type))
                    return ConstructObject(_Types[type]);

                if (type.IsGenericType)
                {
                    Type genericType = _Types[type.GetGenericTypeDefinition()];
                    Type[] args = type.GetGenericArguments();

                    lock (_Types)
                    {
                        _Types.Add(type, genericType.MakeGenericType(args));
                    }

                    return ConstructObject(_Types[type]);
                }
                else
                { 
                    throw new System.InvalidOperationException(string.Format("Unable to resolve key: {0}", type.AssemblyQualifiedName));
                }

			}
			catch (Exception ex)
			{
				_Log.Error(string.Format("Unable to resolve key: {0}", type.AssemblyQualifiedName), ex);
				throw;
			}
		}

        private object ConstructObject(Type type)
        {
            var constructor = type.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            if (parameters.Length == 0)
                return Activator.CreateInstance(type);
            else
            {
                var objects = new List<object>();
                foreach (var p in parameters)
                    objects.Add(Resolve(p.ParameterType));

                return constructor.Invoke(objects.ToArray());
            }
         }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public I Resolve<I>()
		{
			return (I)Resolve(typeof(I));
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void OverrideType(Type @interface, Type @type)
		{
            lock (_Types)
            {
                if (_Types.ContainsKey(@interface))
                    _Types.Remove(@interface);
            }

			RegisterType(@interface, @type);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void OverrideType<I, T>()
			where T : I
		{
			OverrideType(typeof(I), typeof(T));
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "I"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public void OverrideInstance<I>(object instance)
		{
            lock (_Instances)
            {
                if (_Instances.ContainsKey(typeof(I)))
                    _Instances.Remove(typeof(I));
            }

			RegisterInstance<I>(instance);
		}

		#region IServiceLocator Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
            try
            {
                return new List<object>() { Resolve(serviceType) };
            }
            catch
            {
                return new List<object>();
            }
		}

		protected override object DoGetInstance(Type serviceType, string key)
		{
			return Resolve(serviceType);
		}

		#endregion

	}
}
