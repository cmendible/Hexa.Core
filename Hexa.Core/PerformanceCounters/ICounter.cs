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

    /// <summary>
    /// Interface for implementations that will hold all information and actions available regarding a performance counter.
    /// </summary>
    /// <typeparam name="T">enum type holding performance counter details</typeparam>
    public interface ICounter<T> : IDisposable
    {
        #region Methods

        /// <summary>
        /// Get the value of a base counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        /// <param name="value">value to be set</param>
        long BaseRawValue(T counterName, long value);

        /// <summary>
        /// Decrement value of the counter
        /// </summary>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long Decrement(T counterName);

        /// <summary>
        /// Decrement value of the base counter
        /// </summary>
        /// <param name="counterName">name of the counter to has his base counter decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long DecrementBase(T counterName);

        /// <summary>
        /// Decrement value of the counter by "value"
        /// </summary>
        /// <param name="value">value to decrement</param>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long DecrementBy(T counterName, long value);

        /// <summary>
        /// Increment value of the counter
        /// </summary>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter </returns>
        /// <exception cref="System.ObjectDisposedException" />
        long Increment(T counterName);

        /// <summary>
        /// Increment value of the base counter
        /// </summary>
        /// <param name="counterName">name of the counter to has its base counter decremented</param>
        /// <returns>returns FAILURE  in case there was an error otherwise the final value of the counter </returns>
        /// <exception cref="System.ObjectDisposedException" />
        long IncrementBase(T counterName);

        /// <summary>
        /// Increment value of the base counter by "value"
        /// </summary>
        /// <param name="value">value to increment</param>
        /// <param name="counterName">name of the counter to has its base counter decremented</param>
        /// <returns>returns -1 in case there was an error, otherwise it returns the final value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long IncrementBaseBy(T counterName, long value);

        /// <summary>
        /// Increment value of the counter by "value"
        /// </summary>
        /// <param name="value">value to increment</param>
        /// <param name="counterName">name of the counter to be decremented</param>
        /// <returns>retorna -1 si hubo un error, o devuelve el valor final</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long IncrementBy(T counterName, long value);

        /// <summary>
        /// Get the value of a base counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        float NextBaseValue(T counterName);

        /// <summary>
        /// Get the value of a counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        float NextValue(T counterName);

        /// <summary>
        /// Get the value of a counter
        /// </summary>
        /// <param name="counterName">name of the counter</param>
        /// <param name="value">value to put on performance counter</param>
        /// <returns>returns FAILURE si hubo un error,in case there was an error, otherwise it returns the not calculated value</returns>
        /// <exception cref="System.ObjectDisposedException" />
        long RawValue(T counterName, long value);

        /// <summary>
        /// Reset to default value the instance counter
        /// </summary>
        /// <param name="counterName">the counter name</param>
        void Reset(T counterName);

        #endregion Methods
    }
}