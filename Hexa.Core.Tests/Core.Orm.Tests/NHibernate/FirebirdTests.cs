////#if !MONO

//////----------------------------------------------------------------------------------------------
////// <copyright file="FirebirdTests.cs" company="HexaSystems Inc">
////// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
////// </copyright>
//////-----------------------------------------------------------------------------------------------
////namespace Hexa.Core.Orm.Tests.NH
////{
////    using System.Configuration;

////    using Core.Data;
////    using Core.Domain;

////    using NUnit.Framework;
////    using Hexa.Core.Domain.Tests;
////    using System.Reflection;

////    [TestFixture]
////    public class FirebirdTests : SqlTest
////    {
////        protected override string ConnectionString()
////        {
////            return ConfigurationManager.ConnectionStrings["Firebird.Connection"].ConnectionString;
////        }

////        protected override NHUnitOfWorkFactory CreateNHContextFactory()
////        {
////            return new NHUnitOfWorkFactory(DbProvider.Firebird, ConnectionString(), string.Empty, new Assembly[] { Assembly.GetExecutingAssembly() });
////        }
////    }
////}

////#endif