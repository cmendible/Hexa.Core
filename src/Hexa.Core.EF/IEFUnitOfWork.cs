//----------------------------------------------------------------------------------------------
// <copyright file="IEFUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using Microsoft.EntityFrameworkCore;

    public interface IEFUnitOfWork : IUnitOfWork
    {
        DbContext DbContext { get; }
    }
}