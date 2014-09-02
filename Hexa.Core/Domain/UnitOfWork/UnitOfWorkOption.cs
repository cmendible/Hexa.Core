//----------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkOption.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    public enum UnitOfWorkOption
    {
        NewOrReuse = 0,
        NewAndNested = 1
    }
}
