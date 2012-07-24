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
    using System.Diagnostics;

    /// <summary>
    /// Attribute used for Performance counter.
    /// It contains information about the counter that would be used to configure how to manage this variable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class CounterAttribute : Attribute
    {
        #region Fields

        private bool baseAutoIncreased;
        private PerformanceCounterType counterType;
        private string info;
        private string name;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the counter</param>
        /// <param name="info">Information about the counter</param>
        /// <param name="counterType">Type of counter</param>
        /// <seealso cref="PerformanceCounterType"/>
        public CounterAttribute(string name, string info, PerformanceCounterType counterType)
            : this(name, info, counterType, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the counter</param>
        /// <param name="info">Information about the counter</param>
        /// <param name="counterType">Type of counter</param>
        /// <param name="baseAutoIncreased">if true, each time the performance counter increased/decreased its base will be increased/decrease on 1 point. Otherwise all that base management will need to be handed on client code</param>
        /// <seealso cref="PerformanceCounterType"/>
        public CounterAttribute(string name, string info, PerformanceCounterType counterType, bool baseAutoIncreased)
            : base()
        {
            this.name = name;
            this.info = info;
            this.counterType = counterType;
            this.baseAutoIncreased = baseAutoIncreased;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get or Set counterType
        /// </summary>
        public PerformanceCounterType CounterType
        {
            get
            {
                return this.counterType;
            }
        }

        /// <summary>
        /// Get or Set information about the counter
        /// </summary>
        ///
        public string Info
        {
            get
            {
                return this.info;
            }
        }

        /// <summary>
        /// Indicates that, if this performance counter needs a base, it should be increased/decreased by 1 when the relevant one is increased/decreased.
        /// </summary>
        public bool IsBaseAutoIncreased
        {
            get
            {
                return this.baseAutoIncreased;
            }
        }

        /// <summary>
        /// Get or Set the counter name
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Specify the formula to be used to calculate the value when the method 'NextValue' is called
        /// </summary>
        /// <param name="performanceCounter">Counter</param>
        /// <returns>returns the CounterType</returns>
        public static implicit operator PerformanceCounterType(CounterAttribute performanceCounter)
        {
            return performanceCounter.CounterType;
        }

        /// <summary>
        /// Override method to returns counter name
        /// </summary>
        /// <returns>returns the counter name</returns>
        public override string ToString()
        {
            return this.Name;
        }

        #endregion Methods
    }
}