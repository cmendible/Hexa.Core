//----------------------------------------------------------------------------------------------
// <copyright file="INestableUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Linq;

    public interface INestableUnitOfWork : IUnitOfWork
    {
        IUnitOfWork Previous { get; }
    }
}