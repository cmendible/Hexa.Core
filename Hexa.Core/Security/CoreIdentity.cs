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

namespace Hexa.Core.Security
{
    using System;
    using Resources;

    /// <summary>
    /// Class used to hold user name info.
    /// </summary>
    [Serializable]
    public class CoreIdentity : MarshalByRefObject, ICoreIdentity
    {
        // Fields
        private readonly string _id;
        private readonly string _name;
        private readonly string _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreIdentity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CoreIdentity(string name)
            : this(name, "Unknown", name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreIdentity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public CoreIdentity(string name, string type)
            : this(name, type, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreIdentity"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="id">The id.</param>
        public CoreIdentity(string name, string type, string id)
        {
            if (name == null)
            {
                throw new ArgumentNullException(Resource.NameCannotBeNull);
            }
            if (type == null)
            {
                throw new ArgumentNullException(Resource.TypeCannotBeNull);
            }
            if (id == null)
            {
                throw new ArgumentNullException(Resource.TypeCannotBeNull);
            }
            _name = name;
            _type = type;
            _id = id;
        }

        #region ICoreIdentity Members

        /// <summary>
        /// Gets the type of authentication used.
        /// </summary>
        /// <value></value>
        /// <returns>The type of authentication used to identify the user.</returns>
        public virtual string AuthenticationType
        {
            get { return _type; }
        }

        /// <summary>
        /// Gets a value that indicates whether the user has been authenticated.
        /// </summary>
        /// <value></value>
        /// <returns>true if the user was authenticated; otherwise, false.</returns>
        public virtual bool IsAuthenticated
        {
            get { return !string.IsNullOrEmpty(_name); }
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the user on whose behalf the code is running.</returns>
        public virtual string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get { return _id; }
        }

        #endregion
    }
}