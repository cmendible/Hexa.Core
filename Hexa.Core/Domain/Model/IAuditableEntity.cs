//----------------------------------------------------------------------------------------------
// <copyright file="IAuditableEntity.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public interface IAuditableEntity
    {
        DateTime CreatedAt
        {
            get;
            set;
        }

        string CreatedBy
        {
            get;
            set;
        }

        DateTime UpdatedAt
        {
            get;
            set;
        }

        string UpdatedBy
        {
            get;
            set;
        }
    }
}