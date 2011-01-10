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
using System.Security.Principal;

namespace Hexa.Core.Security
{
	/// <summary>
    /// Class used to hold user name info.
	/// </summary>
	[Serializable]
	public class CoreIdentity : MarshalByRefObject, IIdentity 
	{
		// Fields
		private string m_name;
		private string m_type;

		/// <summary>
		/// Initializes a new instance of the <see cref="CoreIdentity"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		public CoreIdentity(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException(Hexa.Core.Resources.Resource.NameCannotBeNull);
			}
			m_name = name;
			m_type = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CoreIdentity"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public CoreIdentity(string name, string type)
		{
			if (name == null)
			{
				throw new ArgumentNullException(Hexa.Core.Resources.Resource.NameCannotBeNull);
			}
			if (type == null)
			{
				throw new ArgumentNullException(Hexa.Core.Resources.Resource.TypeCannotBeNull);
			}
			m_name = name;
			m_type = type;
		}

		/// <summary>
		/// Gets the type of authentication used.
		/// </summary>
		/// <value></value>
		/// <returns>The type of authentication used to identify the user.</returns>
		public virtual string AuthenticationType
		{
			get
			{
				return m_type;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the user has been authenticated.
		/// </summary>
		/// <value></value>
		/// <returns>true if the user was authenticated; otherwise, false.</returns>
		public virtual bool IsAuthenticated
		{
			get
			{
				return !string.IsNullOrEmpty(m_name);
			}
		}

		/// <summary>
		/// Gets the name of the current user.
		/// </summary>
		/// <value></value>
		/// <returns>The name of the user on whose behalf the code is running.</returns>
		public virtual string Name
		{
			get
			{
				return m_name;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CoreIdentity"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="id">The id.</param>
		public CoreIdentity(string name, string type, string id) : this(name, type) 
		{
			Id = id;
		}

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		public string Id { get; private set; }
	}
}
