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
	/// Main class used to hold user identity and roles
	/// </summary>
	[Serializable]
	public class CorePrincipal : MarshalByRefObject, IPrincipal
	{
		private IIdentity m_identity;
		private string[] m_roles;

		/// <summary>
		/// Initializes a new instance of the <see cref="CorePrincipal"/> class.
		/// </summary>
		/// <param name="identity">The identity.</param>
		/// <param name="roles">The roles.</param>
		public CorePrincipal(IIdentity identity, string[] roles)
		{
			if (identity == null)
			{
				throw new ArgumentNullException(Hexa.Core.Resources.Resource.IdentityCanNotBeNull);
			}
			m_identity = identity;
			if (roles != null)
			{
				m_roles = new string[roles.Length];
				for (int i = 0; i < roles.Length; i++)
				{
					m_roles[i] = roles[i];
				}
			}
		}

		/// <summary>
		/// Determines whether the current principal belongs to the specified role.
		/// </summary>
		/// <param name="role">The name of the role for which to check membership.</param>
		/// <returns>
		/// true if the current principal is a member of the specified role; otherwise, false.
		/// </returns>
		public virtual bool IsInRole(string role)
		{
			if ((role != null) && (m_roles != null))
			{
				for (int i = 0; i < m_roles.Length; i++)
				{
					if ((m_roles[i] != null) && (string.Compare(m_roles[i], role, StringComparison.OrdinalIgnoreCase) == 0))
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Gets the identity of the current principal.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Security.Principal.IIdentity"/> object associated with the current principal.</returns>
		public virtual IIdentity Identity
		{
			get
			{
				return m_identity;
			}
		}

	}
}
