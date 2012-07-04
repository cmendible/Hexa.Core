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
    using Data;

    using Raven.Client.Embedded;

    public class RavenContextFactory : IUnitOfWorkFactory, IDatabaseManager
    {
        #region Fields

        private static EmbeddableDocumentStore _documenFactory;

        #endregion Fields

        #region Constructors

        public RavenContextFactory()
        {
            if (_documenFactory == null)
            {
                _documenFactory = new EmbeddableDocumentStore
                {
                    DataDirectory = "Data"
                };
                _documenFactory.Conventions.FindIdentityProperty = prop => prop.Name == "UniqueId";
                _documenFactory.Initialize();
            }
        }

        #endregion Constructors

        #region Methods

        public IUnitOfWork Create()
        {
            return new RavenUnitOfWork(_documenFactory.OpenSession());
        }

        public void CreateDatabase()
        {
        }

        public bool DatabaseExists()
        {
            return false;
        }

        public void DeleteDatabase()
        {
        }

        // Registers Raven IDocumentStore for testing purposes.
        public void RegisterSessionFactory(IoCContainer container)
        {
            container.RegisterInstance<EmbeddableDocumentStore>(_documenFactory);
        }

        public void ValidateDatabaseSchema()
        {
        }

        #endregion Methods
    }
}
