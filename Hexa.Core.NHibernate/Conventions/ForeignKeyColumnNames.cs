//----------------------------------------------------------------------------------------------
// <copyright file="ForeignKeyColumnNames.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    using FluentNHibernate;
    using FluentNHibernate.Conventions;

    public class ForeignKeyColumnNames : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            // many-to-many, one-to-many, join
            if (property == null)
            {
                return type.Name + "Id";
            }

            return property.Name + "Id";
        }
    }
}