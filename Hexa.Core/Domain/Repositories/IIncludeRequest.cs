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

    public interface IIncludeRequest<TEntity, TInclude> : IOrderedQueryable<TEntity>
    {
        IIncludeRequest<TOriginating, TRelated> Include<TOriginating, TRelated>(Expression<Func<TOriginating, TRelated>> path);

        IIncludeRequest<TOriginating, TRelated> IncludeMany<TOriginating, TRelated>(Expression<Func<TOriginating, IEnumerable<TRelated>>> path);

        IIncludeRequest<TQueried, TRelated> ThenInclude<TQueried, TFetch, TRelated>(Expression<Func<TFetch, TRelated>> path);

        IIncludeRequest<TQueried, TRelated> ThenIncludeMany<TQueried, TFetch, TRelated>(Expression<Func<TFetch, IEnumerable<TRelated>>> path);
    }
}