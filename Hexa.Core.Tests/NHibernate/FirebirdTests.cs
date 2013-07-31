#if !MONO

//----------------------------------------------------------------------------------------------
// <copyright file="FirebirdTests.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Sql
{
    using System.Configuration;

    using Core.Data;
    using Core.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class FirebirdTests : SqlTest
    {
        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Firebird.Connection"].ConnectionString;
        }

        protected override NHibernateUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHibernateUnitOfWorkFactory(DbProvider.Firebird, ConnectionString(), string.Empty, typeof(Entity).Assembly);
        }
    }
}

#endif