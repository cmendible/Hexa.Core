//----------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkScope.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    /// <summary>
    ///
    /// </summary>
    public class UnitOfWorkScope
    {
        public static IUnitOfWork Start()
        {
            return Start<IUnitOfWork>();
        }

        public static IUnitOfWork Start<TUnitOfWork>()
        where TUnitOfWork : IUnitOfWork
        {
            IUnitOfWork unitOfWork = ServiceLocator.GetInstance<TUnitOfWork>();
            return unitOfWork;
        }
    }
}