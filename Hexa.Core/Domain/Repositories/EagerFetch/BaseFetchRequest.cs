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

namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class BaseFetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
        where TQueried : class
    {
        #region Fields

        private IQueryable<TQueried> query;

        #endregion Fields

        #region Constructors

        public BaseFetchRequest(IQueryable<TQueried> query)
        {
            this.query = query;
        }

        #endregion Constructors

        #region Properties

        public Type ElementType
        {
            get
            {
                return this.query.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return this.query.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.query.Provider;
            }
        }

        #endregion Properties

        #region Methods

        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        #endregion Methods
    }
}