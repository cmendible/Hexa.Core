//----------------------------------------------------------------------------------------------
// <copyright file="RavenUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using Data;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;

    public class RavenUnitOfWorkFactory : BaseUnitOfWorkFactory, IDatabaseManager
    {
        private static EmbeddableDocumentStore documenFactory;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object can't be disposed here.")]
        public RavenUnitOfWorkFactory()
        {
            if (documenFactory == null)
            {
                documenFactory = new EmbeddableDocumentStore
                {
                    DataDirectory = "Data"
                };
                documenFactory.Conventions.AllowQueriesOnId = true;
                documenFactory.Conventions.FindIdentityProperty = prop => prop.Name == "Id";
                documenFactory.Initialize();
            }
        }

        public IDocumentSession CurrentDocumentSession
        {
            get
            {
                IRavenUnitOfWork unitOfWork = this.Current as IRavenUnitOfWork;
                Guard.IsNotNull(unitOfWork, "No UnitOfWork in scope!!!");

                return unitOfWork.DocumentSession;
            }
        }

        public DocumentStore CreateDocumentStore()
        {
            return documenFactory;
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

        public void ValidateDatabaseSchema()
        {
        }

        protected override INestableUnitOfWork InternalCreate(IUnitOfWork previousUnitOfWork)
        {
            IDocumentSession session = RavenUnitOfWorkFactory.documenFactory.OpenSession();
            return new RavenUnitOfWork(session, previousUnitOfWork, this);
        }
    }
}