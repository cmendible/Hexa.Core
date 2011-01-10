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

using System.Collections.Generic;

namespace Hexa.Core.Domain
{
    public class PagedElements<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> Elements { get; private set; }

        public int TotalElements { get; private set; }

        public PagedElements(IEnumerable<TEntity> elements, int totalElements)
        {
            Elements = elements;
            TotalElements = totalElements;
        }
    }
}
