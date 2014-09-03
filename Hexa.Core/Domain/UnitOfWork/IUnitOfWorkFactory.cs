//----------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork Create(UnitOfWorkOption unitOfWorkOption = UnitOfWorkOption.Reuse);
    }
}
