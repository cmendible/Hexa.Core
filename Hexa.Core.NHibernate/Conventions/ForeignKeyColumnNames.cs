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
                if (type.GetProperty("UniqueId") != null)
                {
                    return type.Name + "UniqueId";
                }

                return type.Name + "Id";
            }

            // else -- many-to-one
            if (property.PropertyType.GetProperty("UniqueId") != null)
            {
                return property.Name + "UniqueId";
            }

            return property.Name + "Id";
        }
    }
}