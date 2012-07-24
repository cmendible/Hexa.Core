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
    using System.Text;

    /// <summary>
    /// Class to wrap managing logic for performance counters
    /// </summary>
    /// <typeparam name="T">Enum Type that defines the performance counter</typeparam>
    internal class Counter<T> : ICounter<T>
    {
        #region Fields

        /// <summary>
        /// Failure constant value
        /// </summary>
        public const int FAILURE = -1;

        /// <summary>
        /// Counters dictionary
        /// </summary>
        private IDictionary<T, CounterWrapper> _counters;
        private bool _disposed; //false

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Internal Constructor for named instances (multi-instance counters)
        /// </summary>
        /// <param name="instanceName">name for this instance</param>
        /// <param name="categoryInfo">information about this category</param>
        /// <param name="enumCounterAttributes">enumerator attributes</param>
        /// <exception cref="System.NotSupportedException" />
        internal Counter(string instanceName, CounterCategoryAttribute categoryInfo, Dictionary<T, CounterAttribute> enumCounterAttributes)
            : this(enumCounterAttributes.Count, instanceName)
        {
            if ( (categoryInfo.InstanceType == PerformanceCounterCategoryType.MultiInstance)
                 && (string.IsNullOrEmpty(instanceName)) )
            {
                throw new NotSupportedException("Multisingle instance must have a name");
            }

            PerformanceCounter performanceCounter, performanceCounterBase = null;
            Dictionary<T, CounterAttribute>.Enumerator enumerator = enumCounterAttributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (categoryInfo.InstanceType == PerformanceCounterCategoryType.MultiInstance)
                {
                    performanceCounter = new PerformanceCounter(categoryInfo.Name,
                            enumerator.Current.Value.Name,
                            InstanceName,
                            false);
                }
                else
                {
                    performanceCounter = new PerformanceCounter(categoryInfo.Name,
                            enumerator.Current.Value.Name,
                            false);
                }
                PerformanceCounterType? baseType = performanceCounter.CounterType.GetBaseType();

                if (baseType != null)
                {
                    if (categoryInfo.InstanceType == PerformanceCounterCategoryType.MultiInstance)
                    {
                        performanceCounterBase = new PerformanceCounter(categoryInfo.Name,
                                CounterFactory.GetCounterNameForBaseType(enumerator.Current.Value.Name),
                                instanceName,
                                false);
                    }
                    else
                    {
                        performanceCounterBase = new PerformanceCounter(categoryInfo.Name,
                                CounterFactory.GetCounterNameForBaseType(enumerator.Current.Value.Name),
                                false);
                    }
                    performanceCounterBase.RawValue = performanceCounter.CounterType.GetInitialValue();
                }
                else
                {
                    performanceCounterBase = null;
                }

                CounterWrapper performanceCounterContainer = new CounterWrapper(performanceCounter, performanceCounterBase, enumerator.Current.Value.IsBaseAutoIncreased);
                this._counters.Add(enumerator.Current.Key, performanceCounterContainer);
            }
        }

        /// <summary>
        /// Internal Constructor for not named instances
        /// </summary>
        /// <param name="categoryInfo">information about this category</param>
        /// <param name="enumCounterAttributes">enumerator attributes</param>
        /// <exception cref="System.NotSupportedException" />
        internal Counter(CounterCategoryAttribute categoryInfo, Dictionary<T, CounterAttribute> enumCounterAttributes)
            : this(null, categoryInfo, enumCounterAttributes)
        {
        }

        private Counter(int capacity, string instanceName)
        {
            this._counters = new Dictionary<T, CounterWrapper>(capacity);
            if (string.IsNullOrEmpty(instanceName))
            {
                InstanceName = Guid.NewGuid().ToString();
            }
            else
            {
                InstanceName = instanceName;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Counter()
        {
            this.Dispose(false);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the instance name associated with this counterHelper
        /// </summary>
        public string InstanceName
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Get the value of a base counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <param name="value">value to be put on performance counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long BaseRawValue(T counterName, long value)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            if (counter.BaseInstance == null)
            {
                return FAILURE;
            }
            try
            {
                long rtnValue = counter.BaseInstance.RawValue = value;
                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Decrement value of the counter
        /// </summary>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long Decrement(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;

            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.Instance.Decrement();
                if ( (counter.BaseInstance != null) && (counter.IsBaseAutoIncreased) )
                {
                    counter.BaseInstance.Decrement();
                }

                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Decrement value of the base counter
        /// </summary>
        /// <param name="counterName">name of the counter to has his base counter decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long DecrementBase(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;

            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            if (counter.BaseInstance == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.BaseInstance.Decrement();
                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Decrement value of the counter by "value"
        /// </summary>
        /// <param name="value">value to decrement</param>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long DecrementBy(T counterName, long value)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            if (value > 0)
            {
                value *= -1;
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }
            try
            {
                long rtnValue = counter.Instance.IncrementBy(value);
                if ((counter.BaseInstance != null) && (counter.IsBaseAutoIncreased))
                {
                    counter.BaseInstance.Decrement();
                }

                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Excplicit Call to dispose the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Increment value of the counter
        /// </summary>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter </returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long Increment(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.Instance.Increment();
                if ((counter.BaseInstance != null) && (counter.IsBaseAutoIncreased))
                {
                    counter.BaseInstance.Increment();
                }

                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Increment value of the base counter
        /// </summary>
        /// <param name="counterName">name of the counter to has its base counter decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter </returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long IncrementBase(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }
            if (counter.BaseInstance == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.BaseInstance.Increment();
                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Increment value of the base counter by "value"
        /// </summary>
        /// <param name="value">value to increment</param>
        /// <param name="counterName">name of the counter to has its base counter decremented</param>
        /// <returns>returns -1 in case there was an error, otherwise it returns the final value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long IncrementBaseBy(T counterName, long value)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }
            if (counter.BaseInstance == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.BaseInstance.IncrementBy(value);
                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Increment value of the counter by "value"
        /// </summary>
        /// <param name="value">value to increment</param>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>retorna -1 si hubo un error, o devuelve el valor final</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public long IncrementBy(T counterName, long value)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }
            try
            {
                long rtnValue = counter.Instance.IncrementBy(value);
                if ((counter.BaseInstance != null) && (counter.IsBaseAutoIncreased))
                {
                    counter.BaseInstance.Increment();
                }

                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Get the value of a base counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public float NextBaseValue(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            if (counter.BaseInstance == null)
            {
                return FAILURE;
            }

            try
            {
                return counter.BaseInstance.NextValue();
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Get the value of a counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        public float NextValue(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            try
            {
                return counter.Instance.NextValue();
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Get the value of a counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        /// <param name="value">value to be put on performance counter</param>
        public long RawValue(T counterName, long value)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) == null)
            {
                return FAILURE;
            }

            try
            {
                long rtnValue = counter.Instance.RawValue = value;
                if ((counter.BaseInstance != null) && (counter.IsBaseAutoIncreased))
                {
                    counter.BaseInstance.Increment();
                }
                return rtnValue;
            }
            catch (InvalidOperationException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.ComponentModel.Win32Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (System.PlatformNotSupportedException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            return FAILURE;
        }

        /// <summary>
        /// Reset to default value the instance counter
        /// </summary>
        /// <param name="counterName">the counter name</param>
        public void Reset(T counterName)
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }

            CounterWrapper counter = null;
            if ((counter = this.GetContainer(counterName)) != null)
            {
                counter.Instance.RawValue = counter.Instance.CounterType.GetInitialValue();
                if (counter.IsBaseAutoIncreased && counter.BaseInstance != null)
                {
                    counter.BaseInstance.RawValue = counter.BaseInstance.CounterType.GetInitialValue();
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._disposed = true;
                if (this._counters != null && this._counters.Count > 0)
                {
                    CounterWrapper[] counters = new CounterWrapper[this._counters.Values.Count];
                    this._counters.Values.CopyTo(counters, 0);
                    this._counters.Clear();

                    foreach (CounterWrapper performanceCounterContainer in counters)
                    {
                        performanceCounterContainer.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// get the PerformanceCounterContainer associated with the given counterName.
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>PerformanceCounterContainer instance in case there is such. Otherwise null.</returns>
        private CounterWrapper GetContainer(T counterName)
        {
            CounterWrapper counter = null;

            if (!this._counters.TryGetValue(counterName, out counter))
            {
                return null;
            }
            return counter;
        }

        #endregion Methods
    }
}