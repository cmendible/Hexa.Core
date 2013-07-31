//----------------------------------------------------------------------------------------------
// <copyright file="IEntityFrameworkUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Data.Entity;

    public interface IEntityFrameworkUnitOfWork : IUnitOfWork
    {
        DbContext DbContext
        {
            get;
        }
    }
}