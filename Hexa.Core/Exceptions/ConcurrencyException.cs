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
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Hexa.Core
{

/// <summary>
/// Concurrency Exception.
/// </summary>
    [Serializable()]
    public class ConcurrencyException : CoreException
    {

        public ConcurrencyException()
        {
        }

        /// <summary>
        /// Exception unique id used for logging purposes.
        /// </summary>
        private Guid _UniqueId = GuidExtensions.NewCombGuid();

        /// <summary>
        /// Gets the unique id.
        /// </summary>
        /// <value>The unique id.</value>
        public Guid UniqueId
        {
            get
                {
                    return _UniqueId;
                }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ConcurrencyException(string message): base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcurrencyException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        public ConcurrencyException(string message, System.Exception ex) : base(message, ex)
        {
        }

        protected ConcurrencyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

    }
}