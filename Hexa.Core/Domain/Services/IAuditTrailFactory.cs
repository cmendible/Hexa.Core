//----------------------------------------------------------------------------------------------
// <copyright file="PagedElements.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public interface IAuditTrailFactory
    {
        IEntityAuditTrail CreateAuditTrail(
            Guid changeSetUniqueId,
            string entityName,
            string entityUniqueId,
            string propertyName,
            object oldValue,
            object newValue,
            string updatedBy,
            DateTime updatedAt);

        bool IsEntityRegistered(string entityName);
    }
}