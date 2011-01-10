#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using System.Security.Permissions;
using System.Runtime.Serialization;

namespace Hexa.Core
{

    /// <summary>
    /// Object Pool Exception
    /// </summary>
	[Serializable]
    public class ObjectPoolException : CoreException
    {

		public ObjectPoolException()
		{ 
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPoolException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ObjectPoolException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectPoolException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ObjectPoolException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

		protected ObjectPoolException(SerializationInfo info, StreamingContext context)
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
