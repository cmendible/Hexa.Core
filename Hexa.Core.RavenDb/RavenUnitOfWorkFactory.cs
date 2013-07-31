//----------------------------------------------------------------------------------------------
// <copyright file="RavenUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.ComponentModel.Composition;

    using Data;

    using Raven.Client.Document;
    using Raven.Client.Embedded;

    [Export(typeof(IDatabaseManager))]
    public class RavenUnitOfWorkFactory : IDatabaseManager
    {
        private static EmbeddableDocumentStore _documenFactory;

        public RavenUnitOfWorkFactory()
        {
            if (_documenFactory == null)
            {
                _documenFactory = new EmbeddableDocumentStore
                {
                    DataDirectory = "Data"
                };
                _documenFactory.Conventions.AllowQueriesOnId = true;
                _documenFactory.Conventions.FindIdentityProperty = prop => prop.Name == "UniqueId";
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