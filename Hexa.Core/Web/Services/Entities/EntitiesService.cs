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
using System.Web;

using Hexa.Core.Domain;

namespace Hexa.Core.Web.UI.Services
{
    public sealed class EntitiesService : IEntitiesService
    {
        #region IEntitiesService Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T GetObjectFromSession<T>()
        {
            object obj = HttpContext.Current.Session["BL"];

            if (!typeof(T).IsInterface && obj != null && obj.GetType() == typeof(T))
                return (T)obj;
            else if (typeof(T).IsInterface && obj != null && obj.GetType().GetInterface(typeof(T).Name) != null)
                return (T)obj;
            else
                return default(T);
        }

        public void SetObjectInSession<T>(T obj)
        {
            if (obj != null && !obj.Equals(default(T)))
                HttpContext.Current.Session["BL"] = obj;
            else
                HttpContext.Current.Session.Remove("BL");
        }

        #endregion
    }
}
