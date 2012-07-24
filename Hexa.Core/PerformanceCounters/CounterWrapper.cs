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
    /// Container of a relevant Performance Counter. It includes context information about that counter such as base performance counter and if it needs to increase/decrease automatically the base when the relevant one is increased / decreased.
    /// </summary>
    internal class CounterWrapper : IDisposable
    {
        #region Fields

        private bool _baseAutoIncreased;
        private PerformanceCounter _baseInstance;
        private bool _disposed; //false
        private PerformanceCounter _instance;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a container with only the relevant performance counter. This constructor sets the base instance in null and the autoincrease value to false
        /// If the performance counter you are passing as argument needs a base, you should consider using the other constructor.
        /// </summary>
        /// <param name="performanceCounterInstance">instance of performance counter</param>
        public CounterWrapper(PerformanceCounter performanceCounterInstance)
            : this(performanceCounterInstance, null, false)
        {
        }

        /// <summary>
        /// Creates a container with the relevant performance counter and the base one associated, setting also if the base counter should be increased / decreased when the relevant one is modified.
        /// If the autoincreased value is set to true, then when increasing or decreasing the relevant counter, the base is increased / decreased by 1. In case the autoincrease is set to false, the  user
        /// will need to manually update the base accordingly.
        /// </summary>
        /// <param name="performanceCounterInstance">instance of performance counter</param>
        /// <param name="performanceCounterBaseInstance">instance of performance counter being the base of the performanceCounterInstance</param>
        /// <param name="autoIncrease">true, to autoincrease the base, false if you prefer doing it manually.</param>
        public CounterWrapper(PerformanceCounter performanceCounterInstance, PerformanceCounter performanceCounterBaseInstance, bool autoIncrease)
        {
            this._instance = performanceCounterInstance;
            this._baseInstance = performanceCounterBaseInstance;
            this._baseAutoIncreased = autoIncrease;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the instance of the base performanceCounter associated with the relevant counter in case there is need of one.
        /// <remarks>This value can be null.</remarks>
        /// </summary>
        public PerformanceCounter BaseInstance
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("PerformanceCounterContainer");
                }

                return this._baseInstance;
            }
        }

        /// <summary>
        /// Get the instance of the relevant performanceCounter.
        /// </summary>
        public PerformanceCounter Instance
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("PerformanceCounterContainer");
                }

                return this._instance;
            }
        }

        /// <summary>
        /// Get if the  Base should be autoincreased. This value is used internally and checked only when the relevant counter is modified.
        /// </summary>
        public bool IsBaseAutoIncreased
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException("PerformanceCounterContainer");
                }

                return this._baseAutoIncreased;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Excplicit Call to dispose the object
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._disposed = true;
                this._instance.RawValue = this._instance.CounterType.GetInitialValue();
                this._instance.Dispose();
            }
        }

        #endregion Methods
    }
}