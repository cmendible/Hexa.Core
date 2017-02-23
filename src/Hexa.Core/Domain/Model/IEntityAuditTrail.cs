//----------------------------------------------------------------------------------------------
// <copyright file="IEntityAuditTrail.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public interface IEntityAuditTrail
    {
        Guid ChangeSetUniqueId
        {
            get;
            set;
        }

        string EntityUniqueId
        {
            get;
            set;
        }

        object NewValue
        {
            get;
            set;
        }

        object OldValue
        {
            get;
            set;
        }

        string PropertyName
        {
            get;
            set;
        }

        string UpdateBy
        {
            get;
            set;
        }

        DateTime UpdatedAt
        {
            get;
            set;
        }
    }
}