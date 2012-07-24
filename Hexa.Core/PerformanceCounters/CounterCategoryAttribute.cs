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
    using System.Text;

    /// <summary>
    /// Attribute to be set to the category containing a set of performance counters
    /// It contains information about the category that would be used to configure how to managed this category.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public sealed class CounterCategoryAttribute : Attribute
    {
        #region Fields

        private string _info;
        private PerformanceCounterCategoryType _instanceType;
        private string _name;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">category name to be shown</param>
        /// <param name="instanceType">category Type (single or multiIntance)</param>
        /// <param name="info">Information to be shown for this category</param>
        /// <seealso cref="PerformanceCounterCategoryType"/>
        public CounterCategoryAttribute(string name, PerformanceCounterCategoryType instanceType, string info)
            : base()
        {
            this._name = name;
            this._info = info;
            this._instanceType = instanceType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get or Set information about this category
        /// </summary>
        public string Info
        {
            get
            {
                return this._info;
            }
        }

        /// <summary>
        /// Get category instance type
        /// </summary>
        public PerformanceCounterCategoryType InstanceType
        {
            get
            {
                return this._instanceType;
            }
        }

        /// <summary>
        /// Get or Set the name of the counter
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }
        }

        #endregion Properties
    }
}