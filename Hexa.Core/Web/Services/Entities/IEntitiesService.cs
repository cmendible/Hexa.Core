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

using Hexa.Core.Domain;

namespace Hexa.Core.Web.UI.Services
{
    /// <summary>
    /// 
    /// </summary>
	public interface IEntitiesService
    {
        /// <summary>
        /// Gets the BL from session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		T GetObjectFromSession<T>();

        /// <summary>
        /// Sets the object in session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        void SetObjectInSession<T>(T obj);
    }
}
