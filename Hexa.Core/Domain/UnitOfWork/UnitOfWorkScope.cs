//----------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkScope.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    ///
    /// </summary>
    [Obsolete]
    public class UnitOfWorkScope
    {
        public static IUnitOfWork Start()
        {
            return Start<IUnitOfWork>();
        }

        public static IUnitOfWork Start<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork
        {
            return ServiceLocator.GetInstance<TUnitOfWork>();
        }
    }
}