//----------------------------------------------------------------------------------------------
// <copyright file="INHibernateUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    using Hexa.Core.Domain;

    using NHibernate;

    public interface INHibernateUnitOfWork : IUnitOfWork
    {
        ISession Session
        {
            get;
        }
    }
}