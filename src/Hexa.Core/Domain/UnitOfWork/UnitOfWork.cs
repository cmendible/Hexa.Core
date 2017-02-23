//----------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    /// <summary>
    ///
    /// </summary>
    public static class UnitOfWork
    {
        public static IUnitOfWork Start()
        {
            IUnitOfWorkFactory factory = IoC.GetInstance<IUnitOfWorkFactory>();
            return factory.Create();
        }
    }
}