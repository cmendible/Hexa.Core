//----------------------------------------------------------------------------------------------
// <copyright file="IRavenUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using Raven.Client;

    public interface IRavenUnitOfWork : INestableUnitOfWork
    {
        IDocumentSession DocumentSession { get; }
    }
}