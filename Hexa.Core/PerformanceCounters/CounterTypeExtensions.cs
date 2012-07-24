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

namespace Hexa.Core.Diagnostics
{
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public static class CounterTypeExtensions
    {
        #region Methods

        /// <summary>
        /// Gets the type of the base.
        /// </summary>
        /// <param name="counterType">Type of the counter.</param>
        /// <returns></returns>
        public static PerformanceCounterType? GetBaseType(this PerformanceCounterType counterType)
        {
            PerformanceCounterType? baseCounterType = null;

            switch (counterType)
            {
            case PerformanceCounterType.AverageCount64:
            case PerformanceCounterType.AverageTimer32:
                baseCounterType = PerformanceCounterType.AverageBase;
                break;
            case PerformanceCounterType.RawFraction:
                baseCounterType = PerformanceCounterType.RawBase;
                break;
            case PerformanceCounterType.CounterMultiTimer:
            case PerformanceCounterType.CounterMultiTimerInverse:
            case PerformanceCounterType.CounterMultiTimer100Ns:
            case PerformanceCounterType.CounterMultiTimer100NsInverse:
                baseCounterType = PerformanceCounterType.CounterMultiBase;
                break;
            case PerformanceCounterType.SampleCounter:
            case PerformanceCounterType.SampleFraction:
                baseCounterType = PerformanceCounterType.SampleBase;
                break;
            }

            return baseCounterType;
        }

        /// <summary>
        /// Gets the initial value.
        /// </summary>
        /// <param name="counterType">Type of the counter.</param>
        /// <returns></returns>
        public static long GetInitialValue(this PerformanceCounterType counterType)
        {
            if (counterType == PerformanceCounterType.AverageTimer32)
            {
                return Stopwatch.GetTimestamp();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Determines whether [is base counter] [the specified counter type].
        /// </summary>
        /// <param name="counterType">Type of the counter.</param>
        /// <returns>
        ///   <c>true</c> if [is base counter] [the specified counter type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBaseCounter(this PerformanceCounterType counterType)
        {
            return (counterType == PerformanceCounterType.AverageBase)
                   || (counterType == PerformanceCounterType.CounterMultiBase)
                   || (counterType == PerformanceCounterType.RawBase)
                   || (counterType == PerformanceCounterType.SampleBase);
        }

        #endregion Methods
    }
}