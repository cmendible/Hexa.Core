//----------------------------------------------------------------------------------------------
// <copyright file="INHUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Linq;
    using NHibernate;

    public interface INHUnitOfWork : INestableUnitOfWork
    {
        ISession Session { get; }
    }
}