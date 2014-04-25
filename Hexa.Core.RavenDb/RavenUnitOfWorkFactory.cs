//----------------------------------------------------------------------------------------------
// <copyright file="RavenUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using Data;
    using Raven.Client.Document;
    using Raven.Client.Embedded;

    public class RavenUnitOfWorkFactory : IDatabaseManager
    {
        private static EmbeddableDocumentStore _documenFactory;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification="Object can't be disposed here.")]
        public RavenUnitOfWorkFactory()
        {
            if (_documenFactory == null)
            {
                _documenFactory = new EmbeddableDocumentStore
                {
                    DataDirectory = "Data"
                };
                _documenFactory.Conventions.AllowQueriesOnId = true;
                _documenFactory.Conventions.FindIdentityProperty = prop => prop.Name == "Id";
                _documenFactory.Initialize();
            }
        }

        public DocumentStore Create()
        {
            return _documenFactory;
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
    }
}