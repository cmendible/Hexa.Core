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
    using System.ComponentModel.Composition;

    using Raven.Client;

    [Export(typeof(IUnitOfWork))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RavenUnitOfWork : IUnitOfWork
    {
        #region Fields

        private IDocumentSession _session;

        #endregion Fields

        #region Constructors

        public RavenUnitOfWork(IDocumentSession session)
        {
            this._session = session;
        }

        #endregion Constructors

        #region Methods

        public void Commit()
        {
            this._session.SaveChanges();
        }

        public IEntitySet<TEntity> CreateSet<TEntity>()
            where TEntity : class
        {
            return new RavenEntitySet<TEntity>(this._session);
        }

        public void Dispose()
        {
            UnitOfWorkScope.DisposeCurrent();
            if (this._session != null)
            {
                this._session.Dispose();

                this._session = null;
            }
        }

        public void RollbackChanges()
        {
        }

        #endregion Methods
    }
}
