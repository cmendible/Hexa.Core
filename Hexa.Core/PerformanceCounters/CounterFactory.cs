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
// Based on: http://perfmoncounterhelper.codeplex.com

#endregion Header

namespace Hexa.Core.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    public class CounterFactory
    {
        #region Methods

        /// <summary>
        /// Create an instance of CounterHelper to manage performance counters defined on T
        /// </summary>
        /// <typeparam name="T">enumerator that holds performance counter information</typeparam>
        /// <seealso cref="CounterCategoryAttribute"/>
        /// <seealso cref="CounterAttribute"/>
        /// <returns>returns an instance of CounterHelper</returns>
        public static ICounter<T> Create<T>()
        {
            return Create<T>(null);
        }

        /// <summary>
        /// Create an instance of CounterHelper to manage performance counters defined on T defininig an instance name for multi-instance counters.
        /// </summary>
        /// <typeparam name="T">enumerator that holds performance counter information</typeparam>
        /// <param name="instanceName">instance name to be used on multi-instance counters</param>
        /// <seealso cref="CounterCategoryAttribute"/>
        /// <seealso cref="CounterAttribute"/>
        /// <returns>returns an instance of CounterHelper</returns>
        public static ICounter<T> Create<T>(string instanceName)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Type {0} is not an enum",
                        enumType.Name));

            CounterCategoryAttribute categoryInfo = GetCategoryAttribute(enumType);

            if (categoryInfo == null)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        "Category is missing {0}",
                        enumType.Name));

            Array enumValues = Enum.GetValues(enumType);

            Dictionary<T, CounterAttribute> enumCounterAttributes = new Dictionary<T, CounterAttribute>();
            CounterAttribute attr;
            foreach (T performanceCounter in enumValues)
            {
                attr = GetCounterAttribute(enumType, performanceCounter);
                if (attr != null)
                {
                    enumCounterAttributes.Add(performanceCounter, attr);
                }
            }

            if (PerformanceCounterCategory.Exists(categoryInfo.Name))
            {
                ICounter<T> counterHelper;
                if (categoryInfo.InstanceType == PerformanceCounterCategoryType.MultiInstance)
                {

                    if (string.IsNullOrEmpty(instanceName))
                    {
                        instanceName = string.Format("{0}_{1}", AppDomain.CurrentDomain.FriendlyName, Process.GetCurrentProcess().Id);
                    }
                    counterHelper = new Counter<T>(instanceName, categoryInfo, enumCounterAttributes);
                }
                else
                {
                    counterHelper = new Counter<T>(categoryInfo, enumCounterAttributes);
                }
                return counterHelper;
            }
            return null;
        }

        /// <summary>
        /// Install a category of performance counters using the information on the enumerator
        /// </summary>
        /// <param name="enumType">enumerator that contains information on the performance counters</param>
        /// <seealso cref="CounterCategoryAttribute"/>
        /// <seealso cref="CounterAttribute"/>
        public static void Install<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Type {0} is not an enum",
                        enumType.Name));

            CounterCategoryAttribute categoryInfo = GetCategoryAttribute(enumType);

            if (categoryInfo == null)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        "Category is missing {0}",
                        enumType.Name));

            Array enumValues = Enum.GetValues(enumType);
            if (PerformanceCounterCategory.Exists(categoryInfo.Name))
            {
                PerformanceCounterCategory.Delete(categoryInfo.Name);
            }

            CounterCreationDataCollection categoryCounters = new CounterCreationDataCollection();

            CounterAttribute attr;
            foreach (object performanceCounter in enumValues)
            {
                attr = GetCounterAttribute(enumType, performanceCounter);
                if (attr != null)
                {
                    if (attr.CounterType.IsBaseCounter())
                    {
                        throw new NotSupportedException(attr.Name);
                    }

                    CounterCreationData counterData = new CounterCreationData(attr.Name, attr.Info, attr.CounterType);
                    categoryCounters.Add(counterData);
                    PerformanceCounterType? baseType = counterData.CounterType.GetBaseType();
                    if (baseType != null)
                    {
                        categoryCounters.Add(new CounterCreationData(GetCounterNameForBaseType(attr.Name),
                                             attr.Name, (PerformanceCounterType)baseType));
                    }
                }
            }
            if (categoryCounters.Count > 0)
            {
                PerformanceCounterCategory.Create(categoryInfo.Name, categoryInfo.Info, categoryInfo.InstanceType, categoryCounters);
            }
        }

        /// <summary>
        /// Uninstall a category of performance counters defined in this Enumerator
        /// </summary>
        /// <param name="typeT">enumerator that holds counters and defines PerformanceCounterCategoryAttribute and PerformanceCounterAttribute</param>
        /// <seealso cref="CounterCategoryAttribute"/>
        /// <seealso cref="CounterAttribute"/>
        /// <exception cref="System.NotSupportedException" />
        public static void Uninstall<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.InvariantCulture,
                        "Type {0} is not an enum",
                        enumType.Name));

            CounterCategoryAttribute categoryInfo = GetCategoryAttribute(enumType);

            if (categoryInfo == null)
                throw new NotSupportedException(
                    String.Format(
                        System.Globalization.CultureInfo.CurrentUICulture,
                        "Category is missing {0}",
                        enumType.Name));

            if (PerformanceCounterCategory.Exists(categoryInfo.Name))
            {
                PerformanceCounterCategory.Delete(categoryInfo.Name);
            }
        }

        /// <summary>
        /// Returns the counter name for the counter base given the counter name that needs a base
        /// </summary>
        /// <param name="counterName">counter name of the counter that needs a base</param>
        /// <returns>counter name to be used on the base counter</returns>
        internal static string GetCounterNameForBaseType(string counterName)
        {
            return string.Concat(counterName, "Base");
        }

        /// <summary>
        /// Get PerformanceCounterCategoryAttribute attached to an Enumeration
        /// </summary>
        /// <param name="enumType">enumerator</param>
        /// <returns>returns an instance of PerformanceCounterCategoryAttribute in case the attribute is found, otherwise null</returns>
        /// <seealso cref="CounterCategoryAttribute"/>
        /// <exception cref="System.NotSupportedException" />
        private static CounterCategoryAttribute GetCategoryAttribute(Type enumType)
        {
            if (enumType == null)
            {
                throw new ArgumentException("enumType");
            }

            CounterCategoryAttribute attr = Attribute.GetCustomAttribute(enumType, typeof(CounterCategoryAttribute)) as CounterCategoryAttribute;
            return attr;
        }

        /// <summary>
        /// Get PerformanceCounterAttribute attached to an item within an Enumeration
        /// </summary>
        /// <param name="enumType">enumerator</param>
        /// <param name="enumValue">value withing the enum</param>
        /// <returns>returns an instance of PerformanceCounterAttribute in case the attribute is found, otherwise null</returns>
        /// <seealso cref="CounterAttribute"/>
        /// <exception cref="System.NotSupportedException" />
        private static CounterAttribute GetCounterAttribute(Type enumType, object enumValue)
        {
            if (enumType == null)
            {
                throw new ArgumentException("enumType");
            }
            if (enumValue == null)
            {
                throw new ArgumentException("enuValue");
            }

            FieldInfo fieldInfo = enumType.GetField(enumValue.ToString());
            CounterAttribute[] attributes = fieldInfo.GetCustomAttributes(typeof(CounterAttribute), false) as CounterAttribute[];
            if ((attributes != null) && (attributes.Length > 0))
            {
                return attributes[0];
            }
            return null;
        }

        #endregion Methods
    }
}